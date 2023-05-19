Imports Microsoft.VisualBasic
Imports System.Web
Imports System.Threading
Imports System.Web.Services
Imports Xceed.Zip.ReaderWriter
Imports Xceed.Compression
Imports Xceed.Zip
Imports SMT.Alentejo.Core.FileManagement
Imports System.IO
Imports SMT.AWS.S3
Imports SMT.Alentejo.Core

Public Class FileDeliveryAsyncHandler
    Implements IHttpAsyncHandler

    Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

    Public Function BeginProcessRequest(ByVal context As System.Web.HttpContext, ByVal cb As System.AsyncCallback, ByVal extraData As Object) As System.IAsyncResult Implements System.Web.IHttpAsyncHandler.BeginProcessRequest
        'Dim SessionHash As String = context.Request.QueryString.Item("SessionHash")
        'Dim FileId As String = context.Request.QueryString.Item("FileId")

        Dim SessionHash As String = context.Request.QueryString.Item("SessionHash")
        Dim FileId As String = Path.GetFileNameWithoutExtension(context.Request.FilePath)

        System.Diagnostics.Debug.WriteLine("SessionHash=" & SessionHash)
        System.Diagnostics.Debug.WriteLine("FileId=" & FileId)

        'context.Response.ContentType = "application/zip"
        'context.Response.AddHeader("Content-Disposition", "attachment; filename=" & Path.GetFileName(context.Request.FilePath))
        'context.Response.AddHeader("Content-Length", 1)
        'context.Response.BufferOutput = True
        'context.Response.StatusCode = 200
        'context.Response.End()

        Dim async As New AsyncOperation(cb, context, extraData)
        async.FileId = FileId
        async.StartAsyncWork()
        Return async
    End Function

    Public Sub EndProcessRequest(ByVal result As System.IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest

    End Sub

    Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
        Throw New InvalidOperationException()
    End Sub

End Class

Class AsyncOperation
    Implements IAsyncResult

    Private _completed As Boolean
    Private _state As [Object]
    Private _callback As AsyncCallback
    Private _context As HttpContext

    Public FileId As String

    ReadOnly Property IsCompleted() As Boolean Implements IAsyncResult.IsCompleted
        Get
            Return _completed
        End Get
    End Property

    ReadOnly Property AsyncWaitHandle() As WaitHandle Implements IAsyncResult.AsyncWaitHandle
        Get
            Return Nothing
        End Get
    End Property

    ReadOnly Property AsyncState() As [Object] Implements IAsyncResult.AsyncState
        Get
            Return _state
        End Get
    End Property

    ReadOnly Property CompletedSynchronously() As Boolean Implements IAsyncResult.CompletedSynchronously
        Get
            Return False
        End Get
    End Property

    Public Sub New(ByVal callback As AsyncCallback, ByVal context As HttpContext, ByVal state As [Object])
        Xceed.Http.Licenser.LicenseKey = "UPS10-D8YXD-FU4KY-T8ZA"
        _callback = callback
        _context = context
        _state = state
        _completed = False
    End Sub

    Public Sub StartAsyncWork()
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf StartAsyncTask), Nothing)
    End Sub

    Private Sub StartAsyncTask(ByVal workItemState As [Object])

        ' GET THE FILE INFO FROM FILES DOMAIN
        Dim F As cSMT_ATJ_File = GetAtjFile(FileId)

        ' CREATE THE STREAM TO THE FILE IN S3
        Dim srcStream As Stream = SMT_AWS_S3_Inline.GetS3ObjectStream(s3_bucket_files_us, FileId, False)

        Dim ext As String = Path.GetExtension(F.FileName).ToLower

        Select Case ext
            Case ".jpg", ".png", ".gif", ".mxi"
                ' PREPARE THE RESPONSE
                _context.Response.ContentType = "application/" & Replace(ext, ".", "")
                _context.Response.AddHeader("Content-Disposition", "attachment; filename=" & F.FileName)
                _context.Response.AddHeader("Content-Length", F.Size)
                CopyStream(srcStream, _context.Response.OutputStream)

            Case Else
                ' ZIP THE FILE
                ' PREPARE THE RESPONSE
                _context.Response.ContentType = "application/zip"
                _context.Response.AddHeader("Content-Disposition", "attachment; filename=" & System.IO.Path.GetFileNameWithoutExtension(F.FileName) & ".zip")
                'context.Response.AddHeader("Content-Length", F.Size)

                ' USE XCEED TO WRITE ZIP DATA TO THE RESPONSE STREAM
                ' Create a ZipItemLocalHeader, defining what will be the name of the file in the Zip file in the compressedMemoryStream
                Dim itemHeader As ZipItemLocalHeader = New ZipItemLocalHeader("\" & F.FileName, CompressionMethod.Deflated, CompressionLevel.Normal)
                ' Do the compression
                CompressStream(itemHeader, srcStream, _context.Response.OutputStream)
        End Select

        _completed = True
        _callback(Me)

    End Sub

    'ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
    '    Get
    '        Return False
    '    End Get
    'End Property

    ''' <summary>
    ''' Compress the streamToCompress into the compressedStream.
    ''' </summary>
    ''' <param name="itemHeader">The ZipItemLocalHeader defining parameters to use for compression.</param>
    ''' <param name="streamToCompress">The stream to compress.</param>
    ''' <param name="compressedStream">The stream conatining the compressed data.</param>
    Private Shared Sub CompressStream(ByVal itemHeader As ZipItemLocalHeader, ByVal streamToCompress As Stream, ByVal compressedStream As Stream)
        Dim zipWriter As ZipWriter = New ZipWriter(compressedStream)
        AddHandler zipWriter.ByteProgression, AddressOf zipWriter_ByteProgression

        ' Write the local header for the current item
        zipWriter.WriteItemLocalHeader(itemHeader)

        ' Read data from the streamToCompress and write it to the ZipReader
        Dim readBytesCount As Integer = 0
        Dim readBytes As Byte() = New Byte(8191) {}

        readBytesCount = streamToCompress.Read(readBytes, 0, readBytes.Length)
        While readBytesCount > 0
            zipWriter.WriteItemData(readBytes, 0, readBytesCount)
            readBytesCount = streamToCompress.Read(readBytes, 0, readBytes.Length)
        End While

        ' Close the zip file. This writes the central header of the zip file
        zipWriter.CloseZipFile()
    End Sub

    ''' <summary>
    ''' Event handler for ZipWriter ByteProgression event
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    Private Shared Sub zipWriter_ByteProgression(ByVal sender As Object, ByVal e As ZipWriterByteProgressionEventArgs)
        'Debug.WriteLine("Compressing: " & e.ZipItemLocalHeader.FileName & " " & e.BytesProcessed & " bytes.")
    End Sub

    Private Shared Sub CopyStream(ByRef inStr As Stream, ByRef outStr As Stream)
        Dim readBytesCount As Integer = 0
        Dim readBytes As Byte() = New Byte(8191) {}

        readBytesCount = inStr.Read(readBytes, 0, readBytes.Length)
        While readBytesCount > 0
            outStr.Write(readBytes, 0, readBytesCount)
            readBytesCount = inStr.Read(readBytes, 0, readBytes.Length)
        End While
        inStr.Close()
    End Sub

End Class
