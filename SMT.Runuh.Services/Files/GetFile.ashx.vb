Imports System.Web
Imports System.Web.Services
Imports SMT.Alentejo.Core.FileManagement
Imports System.IO
Imports SMT.AWS.S3
Imports SMT.Alentejo.Core

Public Class GetFile
    Implements System.Web.IHttpHandler

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim SessionToken As String = context.Request.QueryString.Item("SessionToken")

        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")

        Dim FileId As String = Path.GetFileNameWithoutExtension(context.Request.FilePath)

        ' GET THE FILE INFO FROM FILES DOMAIN
        Dim F As cSMT_ATJ_File = GetAtjFile(FileId)

        ' CREATE THE STREAM TO THE FILE IN S3
        Dim srcStream As Stream = SMT_AWS_S3_Inline.GetS3ObjectStream(s3_bucket_files_us, FileId, False)

        Dim ext As String = Path.GetExtension(F.FileName).ToLower

        Select Case ext
            Case ".jpg", ".png", ".gif", ".tif", ".tga", ".bmp"
                context.Response.ContentType = "image/" & Replace(ext, ".", "")
            Case Else
                context.Response.ContentType = "application/" & Replace(ext, ".", "")
                'context.Response.ContentType = "application/x-zip-compressed"
        End Select

        context.Response.AddHeader("Content-Disposition", "attachment; filename=" & F.FileName) 'Path.GetFileName(context.Request.FilePath)
        context.Response.AddHeader("Content-Length", F.Size)
        context.Response.AddHeader("Last-Modified", New DateTime(CLng(F.Timestamp)))
        context.Response.AddHeader("Accept-Ranges", "bytes")
        context.Response.AddHeader("ETag", "doesThisMatter")
        context.Response.AddHeader("Server", "SMT Alentejo")
        context.Response.AddHeader("Date", DateTime.UtcNow.ToString("r"))
        'context.Response.AddHeader("Content-Range", "bytes 0 - 8191/" & F.Size)
        context.Response.StatusCode = 200

        context.Response.Flush()

        context.Response.BufferOutput = False
        CopyStream(srcStream, context.Response.OutputStream)
    End Sub

    Private Shared Sub CopyStream(ByRef inStr As Stream, ByRef outStr As Stream, Optional ByVal ByteCount As UInt64 = UInt64.MaxValue)
        Dim readBytesCount As Integer = 0
        Dim readBytes As Byte() = New Byte(8191) {}

        readBytesCount = inStr.Read(readBytes, 0, readBytes.Length)
        While readBytesCount > 0 And readBytesCount < ByteCount
            outStr.Write(readBytes, 0, readBytesCount)
            readBytesCount = inStr.Read(readBytes, 0, readBytes.Length)
        End While
        inStr.Close()
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class
