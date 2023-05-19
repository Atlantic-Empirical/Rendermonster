Imports System.Web
Imports System.Web.Services
'Imports Xceed.Zip.ReaderWriter
'Imports Xceed.Compression
'Imports Xceed.Zip
Imports SMT.Alentejo.Core.FileManagement
Imports System.IO
Imports SMT.AWS.S3
Imports SMT.Alentejo.Core

Public Class FileRetrieval
    Implements System.Web.IHttpHandler

#Region "TRPF DEVELOPED"

    'Public Sub New()
    '    Xceed.Http.Licenser.LicenseKey = "UPS10-D8YXD-FU4KY-T8ZA"
    'End Sub

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest

        Dim SessionHash As String = context.Request.QueryString.Item("SessionHash")
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
        context.Response.AddHeader("Date", DateTime.UTCNow.ToString("r"))
        'context.Response.AddHeader("Content-Range", "bytes 0 - 8191/" & F.Size)
        context.Response.StatusCode = 200

        context.Response.Flush()

        context.Response.BufferOutput = False
        CopyStream(srcStream, context.Response.OutputStream)
    End Sub

    Private Shared Sub CopyStream(ByRef inStr As Stream, ByRef outStr As Stream, Optional ByVal ByteCount As UInt64 = UInt64.MaxValue)
        Try
            Dim readBytesCount As Integer = 0
            Dim readBytes As Byte() = New Byte(8191) {}

            readBytesCount = inStr.Read(readBytes, 0, readBytes.Length)
            While readBytesCount > 0 And readBytesCount < ByteCount
                outStr.Write(readBytes, 0, readBytesCount)
                readBytesCount = inStr.Read(readBytes, 0, readBytes.Length)
            End While
            inStr.Close()
        Catch ex As Exception
            'TODO - log a message and tell downloader that this failed
        End Try
    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

#End Region 'TRPF DEVELOPED

#Region "SAMPLE"

    '#Region "Constants used for HTTP communication"

    '    ' The boundary is used in multipart/byteranges responses
    '    ' to separate each ranges content. It should be as unique
    '    ' as possible to avoid confusion with binary content.
    '    Private Const MULTIPART_BOUNDARY As String = "<q1w2e3r4t5y6u7i8o9p0>"
    '    Private Const MULTIPART_CONTENTTYPE As String = "multipart/byteranges; boundary=" & MULTIPART_BOUNDARY

    '    Private Const HTTP_HEADER_ACCEPT_RANGES As String = "Accept-Ranges"
    '    Private Const HTTP_HEADER_ACCEPT_RANGES_BYTES As String = "bytes"
    '    Private Const HTTP_HEADER_CONTENT_TYPE As String = "Content-Type"
    '    Private Const HTTP_HEADER_CONTENT_RANGE As String = "Content-Range"
    '    Private Const HTTP_HEADER_CONTENT_LENGTH As String = "Content-Length"
    '    Private Const HTTP_HEADER_ENTITY_TAG As String = "ETag"
    '    Private Const HTTP_HEADER_LAST_MODIFIED As String = "Last-Modified"
    '    Private Const HTTP_HEADER_RANGE As String = "Range"
    '    Private Const HTTP_HEADER_IF_RANGE As String = "If-Range"
    '    Private Const HTTP_HEADER_IF_MATCH As String = "If-Match"
    '    Private Const HTTP_HEADER_IF_NONE_MATCH As String = "If-None-Match"
    '    Private Const HTTP_HEADER_IF_MODIFIED_SINCE As String = "If-Modified-Since"
    '    Private Const HTTP_HEADER_IF_UNMODIFIED_SINCE As String = "If-Unmodified-Since"
    '    Private Const HTTP_HEADER_UNLESS_MODIFIED_SINCE As String = "Unless-Modified-Since"

    '    Private Const HTTP_METHOD_GET As String = "GET"
    '    Private Const HTTP_METHOD_HEAD As String = "HEAD"

    '#End Region

    '#Region "IHTTPHandler"

    '    Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
    '        Get
    '            ' Allow ASP.NET to reuse instances of this class...
    '            Return True
    '        End Get
    '    End Property

    '    Public Sub ProcessRequest(ByVal objContext As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest

    '        ' The Response object from the Context
    '        Dim objResponse As HttpResponse = objContext.Response
    '        ' The Request object from the Context
    '        Dim objRequest As HttpRequest = objContext.Request

    '        ' File information object...
    '        Dim objFile As Download.FileInformation

    '        ' Long Arrays for Range values:
    '        ' ...Begin() contains start positions for each requested Range
    '        Dim alRequestedRangesBegin() As Long
    '        ' ...End() contains end positions for each requested Range
    '        Dim alRequestedRangesend() As Long

    '        ' Response Header value: Content Length...
    '        Dim iResponseContentLength As Int32

    '        ' The Stream we're using to download the file in chunks...
    '        Dim objStream As System.IO.FileStream
    '        ' Total Bytes to read (per requested range)
    '        Dim iBytesToRead As Int32
    '        ' Size of the Buffer for chunk-wise reading
    '        Dim iBufferSize As Int32 = 25000
    '        ' The Buffer itself
    '        Dim bBuffer(iBufferSize) As Byte
    '        ' Amount of Bytes read
    '        Dim iLengthOfReadChunk As Int32

    '        ' Indicates if the download was interrupted
    '        Dim bDownloadBroken As Boolean

    '        ' Indicates if this is a range request 
    '        Dim bIsRangeRequest As Boolean
    '        ' Indicates if this is a multipart range request
    '        Dim bMultipart As Boolean

    '        ' Loop counter used to iterate through the ranges
    '        Dim iLoop As Int32


    '        ' ToDo - your code here (Determine which file is requested)
    '        ' Using objRequest, determine which file is requested to
    '        ' be downloaded, and open objFile with that file:
    '        ' Example:
    '        ' objFile = New Download.FileInformation(<Full path to the requested file>)
    '        objFile = New Download.FileInformation(objContext.Server.MapPath("~/download.zip"))

    '        '    Dim SessionHash As String = context.Request.QueryString.Item("SessionHash")
    '        '    Dim FileId As String = Path.GetFileNameWithoutExtension(context.Request.FilePath)

    '        '    ' GET THE FILE INFO FROM FILES DOMAIN
    '        '    Dim F As cSMT_ATJ_File = GetAtjFile(FileId)

    '        '    ' CREATE THE STREAM TO THE FILE IN S3
    '        '    Dim srcStream As Stream = SMT_AWS_S3_InlineGet.GetS3ObjectStream(s3_bucket_files_us, FileId, False)

    '        '    Dim ext As String = Path.GetExtension(F.FileName).ToLower

    '        '    Select Case ext
    '        '        Case ".jpg", ".png", ".gif", ".tif", ".tga", ".bmp"
    '        '            context.Response.ContentType = "image/" & Replace(ext, ".", "")
    '        '        Case Else
    '        '            context.Response.ContentType = "application/" & Replace(ext, ".", "")
    '        '            'context.Response.ContentType = "application/x-zip-compressed"
    '        '    End Select

    '        '    'context.Response.Cache.SetCacheability(HttpCacheability.NoCache)
    '        '    'context.Response.Cache.SetNoStore()
    '        '    'context.Response.Cache.SetExpires(DateTime.MinValue)
    '        '    context.Response.AddHeader("Content-Disposition", "attachment; filename=" & Path.GetFileName(context.Request.FilePath)) 'F.FileName
    '        '    context.Response.AddHeader("Content-Length", F.Size)
    '        '    context.Response.AddHeader("Last-Modified", New DateTime(CLng(F.Timestamp)))
    '        '    context.Response.AddHeader("Accept-Ranges", "bytes")
    '        '    context.Response.AddHeader("ETag", "doesThisMatter")
    '        '    context.Response.AddHeader("Server", "SMT Alentejo")
    '        '    context.Response.AddHeader("Date", DateTime.UTCNow.ToString("r"))
    '        '    'context.Response.AddHeader("Content-Range", "bytes 0 - 8191/" & F.Size)
    '        '    context.Response.StatusCode = 200

    '        '    context.Response.BufferOutput = False
    '        '    CopyStream(srcStream, context.Response.OutputStream, 8192)
    '        '    context.Response.Flush()
    '        '    'context.Response.End()


    '        ' Clear the current output content from the buffer
    '        objResponse.Clear()

    '        If Not objRequest.HttpMethod.Equals(HTTP_METHOD_GET) Or Not objRequest.HttpMethod.Equals(HTTP_METHOD_HEAD) Then
    '            ' Currently, only the GET and HEAD methods 
    '            ' are supported...
    '            objResponse.StatusCode = 501  ' Not implemented

    '        ElseIf Not objFile.Exists Then
    '            ' The requested file could not be retrieved...
    '            objResponse.StatusCode = 404  ' Not found

    '        ElseIf objFile.Length > Int32.MaxValue Then
    '            ' The file size is too large... 
    '            objResponse.StatusCode = 413  ' Request Entity Too Large

    '        ElseIf Not ParseRequestHeaderRange(objRequest, alRequestedRangesBegin, alRequestedRangesend, objFile.Length, bIsRangeRequest) Then
    '            ' The Range request contained bad entries
    '            objResponse.StatusCode = 400  ' Bad Request

    '        ElseIf Not CheckIfModifiedSince(objRequest, objFile) Then
    '            ' The entity is still unmodified...
    '            objResponse.StatusCode = 304  ' Not Modified

    '        ElseIf Not CheckIfUnmodifiedSince(objRequest, objFile) Then
    '            ' The entity was modified since the requested date... 
    '            objResponse.StatusCode = 412  ' Precondition failed

    '        ElseIf Not CheckIfMatch(objRequest, objFile) Then
    '            ' The entity does not match the request... 
    '            objResponse.StatusCode = 412  ' Precondition failed

    '        ElseIf Not CheckIfNoneMatch(objRequest, objResponse, objFile) Then
    '            ' The entity does match the none-match request, the response code was set inside the CheckIfNoneMatch function

    '        Else
    '            ' Preliminary checks where successful... 

    '            If bIsRangeRequest AndAlso CheckIfRange(objRequest, objFile) Then
    '                ' This is a Range request... 

    '                ' If the Range arrays contain more than one entry, it even is a multipart range request...
    '                bMultipart = CBool(alRequestedRangesBegin.GetUpperBound(0) > 0)

    '                ' Loop through each Range to calculate the entire Response length
    '                For iLoop = alRequestedRangesBegin.GetLowerBound(0) To alRequestedRangesBegin.GetUpperBound(0)
    '                    ' The length of the content (for this range)
    '                    iResponseContentLength += Convert.ToInt32(alRequestedRangesend(iLoop) - alRequestedRangesBegin(iLoop)) + 1

    '                    If bMultipart Then
    '                        ' If this is a multipart range request, calculate the length of the intermediate headers to send
    '                        iResponseContentLength += MULTIPART_BOUNDARY.Length
    '                        iResponseContentLength += objFile.ContentType.Length
    '                        iResponseContentLength += alRequestedRangesBegin(iLoop).ToString.Length
    '                        iResponseContentLength += alRequestedRangesend(iLoop).ToString.Length
    '                        iResponseContentLength += objFile.Length.ToString.Length
    '                        ' 49 is the length of line break and other needed characters in one multipart header
    '                        iResponseContentLength += 49
    '                    End If

    '                Next iLoop

    '                If bMultipart Then
    '                    ' If this is a multipart range request, we must also calculate the length of the last intermediate header we must send
    '                    iResponseContentLength += MULTIPART_BOUNDARY.Length
    '                    ' 8 is the length of dash and line break characters
    '                    iResponseContentLength += 8

    '                Else
    '                    ' This is no multipart range request, so we must indicate the response Range of in the initial HTTP Header 
    '                    objResponse.AppendHeader(HTTP_HEADER_CONTENT_RANGE, "bytes " & alRequestedRangesBegin(0).ToString & "-" & alRequestedRangesend(0).ToString & "/" & objFile.Length.ToString)
    '                End If

    '                ' Range response 
    '                objResponse.StatusCode = 206 ' Partial Response


    '            Else
    '                ' This is not a Range request, or the requested Range entity ID does not match the current entity ID, so start a new download

    '                ' Indicate the file's complete size as content length
    '                iResponseContentLength = Convert.ToInt32(objFile.Length)

    '                ' Return a normal OK status...
    '                objResponse.StatusCode = 200
    '            End If


    '            ' Write the content length into the Response
    '            objResponse.AppendHeader(HTTP_HEADER_CONTENT_LENGTH, iResponseContentLength.ToString)

    '            ' Write the Last-Modified Date into the Response
    '            objResponse.AppendHeader(HTTP_HEADER_LAST_MODIFIED, objFile.LastWriteTimeUTC.ToString("r"))

    '            ' Tell the client software that we accept Range request
    '            objResponse.AppendHeader(HTTP_HEADER_ACCEPT_RANGES, HTTP_HEADER_ACCEPT_RANGES_BYTES)

    '            ' Write the file's Entity Tag into the Response (in quotes!)
    '            objResponse.AppendHeader(HTTP_HEADER_ENTITY_TAG, """" & objFile.EntityTag & """")


    '            ' Write the Content Type into the Response
    '            If bMultipart Then
    '                ' Multipart messages have this special Type.
    '                ' In this case, the file's actual mime type is
    '                ' written into the Response at a later time...
    '                objResponse.ContentType = MULTIPART_CONTENTTYPE
    '            Else
    '                ' Single part messages have the files content type...
    '                objResponse.ContentType = objFile.ContentType
    '            End If



    '            If objRequest.HttpMethod.Equals(HTTP_METHOD_HEAD) Then
    '                ' Only the HEAD was requested, so we can quit the Response right here... 
    '            Else

    '                ' Flush the HEAD information to the client...
    '                objResponse.Flush()

    '                ' Download is in progress...
    '                objFile.State = FileInformation.DownloadState.fsDownloadInProgress

    '                ' Open the file as filestream
    '                objStream = New System.IO.FileStream(objFile.FullName, IO.FileMode.Open, IO.FileAccess.Read, IO.FileShare.Read)

    '                ' Now, for each requested range, stream the chunks to the client:
    '                For iLoop = alRequestedRangesBegin.GetLowerBound(0) To alRequestedRangesBegin.GetUpperBound(0)

    '                    ' Move the stream to the desired start position...
    '                    objStream.Seek(alRequestedRangesBegin(iLoop), IO.SeekOrigin.Begin)

    '                    ' Calculate the total amount of bytes for this range
    '                    iBytesToRead = Convert.ToInt32(alRequestedRangesend(iLoop) - alRequestedRangesBegin(iLoop)) + 1

    '                    If bMultipart Then
    '                        ' If this is a multipart response, we must add 
    '                        ' certain headers before streaming the content:

    '                        ' The multipart boundary
    '                        objResponse.Output.WriteLine("--" & MULTIPART_BOUNDARY)

    '                        ' The mime type of this part of the content 
    '                        objResponse.Output.WriteLine(HTTP_HEADER_CONTENT_TYPE & ": " & objFile.ContentType)

    '                        ' The actual range
    '                        objResponse.Output.WriteLine(HTTP_HEADER_CONTENT_RANGE & ": bytes " & _
    '                                                     alRequestedRangesBegin(iLoop).ToString & "-" & _
    '                                                     alRequestedRangesend(iLoop).ToString & "/" & _
    '                                                     objFile.Length.ToString)

    '                        ' Indicating the end of the intermediate headers
    '                        objResponse.Output.WriteLine()

    '                    End If

    '                    ' Now stream the range to the client...
    '                    While iBytesToRead > 0

    '                        If objResponse.IsClientConnected Then
    '                            ' Read a chunk of bytes from the stream
    '                            iLengthOfReadChunk = objStream.Read(bBuffer, 0, Math.Min(bBuffer.Length, iBytesToRead))

    '                            ' Write the data to the current output stream.
    '                            objResponse.OutputStream.Write(bBuffer, 0, iLengthOfReadChunk)

    '                            ' Flush the data to the HTML output.
    '                            objResponse.Flush()

    '                            ' Clear the buffer
    '                            ReDim bBuffer(iBufferSize)

    '                            ' Reduce BytesToRead
    '                            iBytesToRead -= iLengthOfReadChunk

    '                        Else
    '                            ' The client was or has disconneceted from the server... stop downstreaming...
    '                            iBytesToRead = -1
    '                            bDownloadBroken = True

    '                        End If
    '                    End While

    '                    ' In Multipart responses, mark the end of the part 
    '                    If bMultipart Then objResponse.Output.WriteLine()

    '                    ' No need to proceed to the next part if the 
    '                    ' client was disconnected
    '                    If bDownloadBroken Then Exit For
    '                Next iLoop

    '                ' At this point, the response was finished or cancelled... 

    '                If bDownloadBroken Then
    '                    ' Download is broken...
    '                    objFile.State = FileInformation.DownloadState.fsDownloadBroken

    '                Else
    '                    If bMultipart Then
    '                        ' In multipart responses, close the response once more with 
    '                        ' the boundary and line breaks
    '                        objResponse.Output.WriteLine("--" & MULTIPART_BOUNDARY & "--")
    '                        objResponse.Output.WriteLine()
    '                    End If

    '                    ' The download was finished
    '                    objFile.State = FileInformation.DownloadState.fsDownloadFinished
    '                End If

    '                objStream.Close()

    '            End If

    '        End If

    '        objResponse.End()

    '    End Sub

    '#End Region

    '#Region "Private helper functions"

    '    Private Function CheckIfRange(ByVal objRequest As HttpRequest, ByVal objFile As Download.FileInformation) As Boolean
    '        Dim sRequestHeaderIfRange As String

    '        ' Checks the If-Range header if it was sent with the request.
    '        '
    '        ' Returns True if the header value matches the file's entity tag,
    '        '              or if no header was sent,
    '        ' returns False if a header was sent, but does not match the file.


    '        ' Retrieve If-Range Header value from Request (objFile.EntityTag if none is indicated)
    '        sRequestHeaderIfRange = RetrieveHeader(objRequest, HTTP_HEADER_IF_RANGE, objFile.EntityTag)

    '        ' If the requested file entity matches the current
    '        ' file entity, return True
    '        Return sRequestHeaderIfRange.Equals(objFile.EntityTag)
    '    End Function

    '    Private Function CheckIfMatch(ByVal objRequest As HttpRequest, ByVal objFile As Download.FileInformation) As Boolean
    '        Dim sRequestHeaderIfMatch As String
    '        Dim sEntityIDs() As String
    '        Dim bReturn As Boolean

    '        ' Checks the If-Match header if it was sent with the request.
    '        '
    '        ' Returns True if one of the header values matches the file's entity tag,
    '        '              or if no header was sent,
    '        ' returns False if a header was sent, but does not match the file.


    '        ' Retrieve If-Match Header value from Request (*, meaning any, if none is indicated)
    '        sRequestHeaderIfMatch = RetrieveHeader(objRequest, HTTP_HEADER_IF_MATCH, "*")

    '        If sRequestHeaderIfMatch.Equals("*") Then
    '            ' The server may perform the request as if the
    '            ' If-Match header does not exists...
    '            bReturn = True

    '        Else
    '            ' One or more Match IDs where sent by the client software...
    '            sEntityIDs = sRequestHeaderIfMatch.Replace("bytes=", "").Split(",".ToCharArray)

    '            ' Loop through all entity IDs, finding one 
    '            ' which matches the current file's ID will
    '            ' be enough to satisfy the If-Match
    '            For iLoop As Int32 = sEntityIDs.GetLowerBound(0) To sEntityIDs.GetUpperBound(0)
    '                If sEntityIDs(iLoop).Trim.Equals(objFile.EntityTag) Then
    '                    bReturn = True
    '                End If
    '            Next iLoop
    '        End If

    '        ' Return the result...
    '        Return bReturn
    '    End Function

    '    Private Function CheckIfNoneMatch(ByVal objRequest As HttpRequest, ByVal objResponse As HttpResponse, ByVal objFile As Download.FileInformation) As Boolean
    '        Dim sRequestHeaderIfNoneMatch As String
    '        Dim sEntityIDs() As String
    '        Dim bReturn As Boolean = True
    '        Dim sReturn As String

    '        ' Checks the If-None-Match header if it was sent with the request.
    '        '
    '        ' Returns True if one of the header values matches the file's entity tag,
    '        '              or if "*" was sent,
    '        ' returns False if a header was sent, but does not match the file, or
    '        '               if no header was sent.


    '        ' Retrieve If-None-Match Header value from Request (*, meaning any, if none is indicated)
    '        sRequestHeaderIfNoneMatch = RetrieveHeader(objRequest, HTTP_HEADER_IF_NONE_MATCH, String.Empty)

    '        If sRequestHeaderIfNoneMatch.Equals(String.Empty) Then
    '            ' Perform the request normally...
    '            bReturn = True

    '        ElseIf sRequestHeaderIfNoneMatch.Equals("*") Then
    '            ' The server must not perform the request 
    '            objResponse.StatusCode = 412  ' Precondition failed
    '            bReturn = False

    '        Else
    '            ' One or more Match IDs where sent by the client software...
    '            sEntityIDs = sRequestHeaderIfNoneMatch.Replace("bytes=", "").Split(",".ToCharArray)

    '            ' Loop through all entity IDs, finding one which 
    '            ' does not match the current file's ID will be
    '            ' enough to satisfy the If-None-Match
    '            For iLoop As Int32 = sEntityIDs.GetLowerBound(0) To sEntityIDs.GetUpperBound(0)
    '                If sEntityIDs(iLoop).Trim.Equals(objFile.EntityTag) Then
    '                    sReturn = sEntityIDs(iLoop)
    '                    bReturn = False
    '                End If
    '            Next iLoop

    '            If Not bReturn Then
    '                ' One of the requested entities matches the current file's tag,
    '                objResponse.AppendHeader("ETag", sReturn)
    '                objResponse.StatusCode = 304  ' Not Modified

    '            End If
    '        End If

    '        ' Return the result...
    '        Return bReturn
    '    End Function

    '    Private Function CheckIfModifiedSince(ByVal objRequest As HttpRequest, ByVal objFile As Download.FileInformation) As Boolean
    '        Dim sDate As String
    '        Dim dDate As Date
    '        Dim bReturn As Boolean

    '        ' Checks the If-Modified header if it was sent with the request.
    '        '
    '        ' Returns True, if the file was modified since the 
    '        '               indicated date (RFC 1123 format), or
    '        '               if no header was sent,
    '        ' returns False, if the file was not modified since
    '        '                the indicated date


    '        ' Retrieve If-Modified-Since Header value from Request (Empty if none is indicated)
    '        sDate = RetrieveHeader(objRequest, HTTP_HEADER_IF_MODIFIED_SINCE, String.Empty)

    '        If sDate.Equals(String.Empty) Then
    '            ' No If-Modified-Since date was indicated, 
    '            ' so just give this as True 
    '            bReturn = True

    '        Else
    '            Try
    '                ' ... to parse the indicated sDate to a datetime value
    '                dDate = DateTime.Parse(sDate)
    '                ' Return True if the file was modified since or at the indicated date...
    '                bReturn = objFile.LastWriteTimeUTC >= DateTime.Parse(sDate)

    '            Catch ex As Exception
    '                ' Converting the indicated date value failed, return False 
    '                bReturn = False

    '            End Try

    '        End If

    '        Return bReturn
    '    End Function

    '    Private Function CheckIfUnmodifiedSince(ByVal objRequest As HttpRequest, ByVal objFile As Download.FileInformation) As Boolean
    '        Dim sDate As String
    '        Dim dDate As Date
    '        Dim bReturn As Boolean


    '        ' Checks the If-Unmodified or Unless-Modified-Since header, if 
    '        ' one of them was sent with the request.
    '        '
    '        ' Returns True, if the file was not modified since the 
    '        '               indicated date (RFC 1123 format), or
    '        '               if no header was sent,
    '        ' returns False, if the file was modified since the indicated date


    '        ' Retrieve If-Unmodified-Since Header value from Request (Empty if none is indicated)
    '        sDate = RetrieveHeader(objRequest, HTTP_HEADER_IF_UNMODIFIED_SINCE, String.Empty)

    '        If sDate.Equals(String.Empty) Then
    '            ' If-Unmodified-Since was not sent, check Unless-Modified-Since... 
    '            sDate = RetrieveHeader(objRequest, HTTP_HEADER_UNLESS_MODIFIED_SINCE, String.Empty)
    '        End If


    '        If sDate.Equals(String.Empty) Then
    '            ' No date was indicated, 
    '            ' so just give this as True 
    '            bReturn = True

    '        Else
    '            Try
    '                ' ... to parse the indicated sDate to a datetime value
    '                dDate = DateTime.Parse(sDate)
    '                ' Return True if the file was not modified since the indicated date...
    '                bReturn = objFile.LastWriteTimeUTC < DateTime.Parse(sDate)

    '            Catch ex As Exception
    '                ' Converting the indicated date value failed, return False 
    '                bReturn = False

    '            End Try

    '        End If

    '        Return bReturn
    '    End Function

    '    Private Function ParseRequestHeaderRange(ByVal objRequest As HttpRequest, ByRef lBegin() As Long, ByRef lEnd() As Long, ByVal lMax As Long, ByRef bRangeRequest As Boolean) As Boolean
    '        Dim bValidRanges As Boolean
    '        Dim sSource As String
    '        Dim iLoop As Int32
    '        Dim sRanges() As String

    '        ' Parses the Range header from the Request (if there is one)
    '        ' Returns True, if the Range header was valid, or if there was no 
    '        '               Range header at all (meaning that the whole 
    '        '               file was requested)
    '        ' Returns False, if the Range header asked for unsatisfieable 
    '        '                ranges

    '        ' Retrieve Range Header value from Request (Empty if none is indicated)
    '        sSource = RetrieveHeader(objRequest, HTTP_HEADER_RANGE, String.Empty)

    '        If sSource.Equals(String.Empty) Then
    '            ' No Range was requested, return the entire file range...

    '            ReDim lBegin(0)
    '            ReDim lEnd(0)

    '            lBegin(0) = 0
    '            lEnd(0) = lMax - 1

    '            ' A valid range is returned
    '            bValidRanges = True
    '            ' no Range request
    '            bRangeRequest = False

    '        Else
    '            ' A Range was requested... 

    '            ' Preset value...
    '            bValidRanges = True

    '            ' Return True for the bRange parameter, telling the caller
    '            ' that the Request is indeed a Range request...
    '            bRangeRequest = True

    '            ' Remove "bytes=" from the beginning, and split the remaining 
    '            ' string by comma characters
    '            sRanges = sSource.Replace("bytes=", "").Split(",".ToCharArray)

    '            ReDim lBegin(sRanges.GetUpperBound(0))
    '            ReDim lEnd(sRanges.GetUpperBound(0))

    '            ' Check each found Range request for consistency
    '            For iLoop = sRanges.GetLowerBound(0) To sRanges.GetUpperBound(0)
    '                ' Split this range request by the dash character, 
    '                ' sRange(0) contains the requested begin-value,
    '                ' sRange(1) contains the requested end-value...
    '                Dim sRange() As String = sRanges(iLoop).Split("-".ToCharArray)

    '                ' Determine the end of the requested range
    '                If sRange(1).Equals(String.Empty) Then
    '                    ' No end was specified, take the entire range
    '                    lEnd(iLoop) = lMax - 1
    '                Else
    '                    ' An end was specified...
    '                    lEnd(iLoop) = Long.Parse(sRange(1))
    '                End If

    '                ' Determine the begin of the requested range
    '                If sRange(0).Equals(String.Empty) Then
    '                    ' No begin was specified, which means that
    '                    ' the end value indicated to return the last n
    '                    ' bytes of the file:

    '                    ' Calculate the begin
    '                    lBegin(iLoop) = lMax - 1 - lEnd(iLoop)
    '                    ' ... to the end of the file...
    '                    lEnd(iLoop) = lMax - 1

    '                Else
    '                    ' A normal begin value was indicated...
    '                    lBegin(iLoop) = Long.Parse(sRange(0))

    '                End If

    '                ' Check if the requested range values are valid, 
    '                ' return False if they are not.
    '                '
    '                ' Note:
    '                ' Do not clean invalid values up by fitting them into
    '                ' valid parameters using Math.Min and Math.Max, because
    '                ' some download clients (like Go!Zilla) might send invalid 
    '                ' (e.g. too large) range requests to determine the file limits!

    '                ' Begin and end must not exceed the file size
    '                If (lBegin(iLoop) > (lMax - 1)) Or (lEnd(iLoop) > (lMax - 1)) Then
    '                    bValidRanges = False
    '                End If

    '                ' Begin and end cannot be < 0
    '                If (lBegin(iLoop) < 0) Or (lEnd(iLoop) < 0) Then
    '                    bValidRanges = False
    '                End If

    '                ' End must be larger or equal to begin value
    '                If lEnd(iLoop) < lBegin(iLoop) Then
    '                    ' The requested Range is invalid...
    '                    bValidRanges = False
    '                End If

    '            Next iLoop

    '        End If

    '        Return bValidRanges
    '    End Function

    '    Private Function RetrieveHeader(ByVal objRequest As HttpRequest, ByVal sHeader As String, ByVal sDefault As String) As String
    '        Dim sReturn As String

    '        ' Retrieves the indicated Header's value from the Request,
    '        ' if the header was not sent, sDefault is returned.
    '        '
    '        ' If the value contains quote characters, they are removed.

    '        sReturn = objRequest.Headers.Item(sHeader)

    '        If (sReturn Is Nothing) OrElse sReturn.Equals(String.Empty) Then
    '            ' The Header wos not found in the Request, 
    '            ' return the indicated default value...
    '            Return sDefault

    '        Else
    '            ' Return the found header value, stripped of any quote characters...
    '            Return sReturn.Replace("""", "")

    '        End If

    '    End Function


    '    Private Function GenerateHash(ByVal objStream As System.IO.Stream, ByVal lBegin As Long, ByVal lEnd As Long) As String
    '        Dim bByte(Convert.ToInt32(lEnd)) As Byte

    '        objStream.Read(bByte, Convert.ToInt32(lBegin), Convert.ToInt32(lEnd - lBegin) + 1)

    '        'Instantiate an MD5 Provider object
    '        Dim Md5 As New System.Security.Cryptography.MD5CryptoServiceProvider

    '        'Compute the hash value from the source
    '        Dim ByteHash() As Byte = Md5.ComputeHash(bByte)

    '        'And convert it to String format for return
    '        Return Convert.ToBase64String(ByteHash)
    '    End Function


    '#End Region

#End Region 'SAMPLE

#Region "OLD 1"

    ''''' <summary>
    ''''' Compress the streamToCompress into the compressedStream.
    ''''' </summary>
    ''''' <param name="itemHeader">The ZipItemLocalHeader defining parameters to use for compression.</param>
    ''''' <param name="streamToCompress">The stream to compress.</param>
    ''''' <param name="compressedStream">The stream conatining the compressed data.</param>
    ''Private Shared Sub CompressStream(ByVal itemHeader As ZipItemLocalHeader, ByVal streamToCompress As Stream, ByVal compressedStream As Stream)
    ''    Dim zipWriter As ZipWriter = New ZipWriter(compressedStream)
    ''    AddHandler zipWriter.ByteProgression, AddressOf zipWriter_ByteProgression

    ''    ' Write the local header for the current item
    ''    zipWriter.WriteItemLocalHeader(itemHeader)

    ''    ' Read data from the streamToCompress and write it to the ZipReader
    ''    Dim readBytesCount As Integer = 0
    ''    Dim readBytes As Byte() = New Byte(8191) {}

    ''    readBytesCount = streamToCompress.Read(readBytes, 0, readBytes.Length)
    ''    While readBytesCount > 0
    ''        zipWriter.WriteItemData(readBytes, 0, readBytesCount)
    ''        readBytesCount = streamToCompress.Read(readBytes, 0, readBytes.Length)
    ''    End While

    ''    ' Close the zip file. This writes the central header of the zip file
    ''    zipWriter.CloseZipFile()
    ''End Sub

    ''''' <summary>
    ''''' Event handler for ZipWriter ByteProgression event
    ''''' </summary>
    ''''' <param name="sender"></param>
    ''''' <param name="e"></param>
    ''Private Shared Sub zipWriter_ByteProgression(ByVal sender As Object, ByVal e As ZipWriterByteProgressionEventArgs)
    ''    'Debug.WriteLine("Compressing: " & e.ZipItemLocalHeader.FileName & " " & e.BytesProcessed & " bytes.")
    ''End Sub





    ''Dim SessionHash As String = context.Request.QueryString.Item("SessionHash")
    ''Dim FileId As String = context.Request.QueryString.Item("FileId")

    ''' GET THE FILE INFO FROM FILES DOMAIN
    ''Dim F As cSMT_ATJ_File = GetAtjFile(FileId)

    ''' CREATE THE STREAM TO THE FILE IN S3
    ''Dim srcStream As Stream = SMT_AWS_S3_InlineGet.GetS3ObjectStream(s3_bucket_files_us, FileId, False)

    ''Dim ext As String = Path.GetExtension(F.FileName).ToLower

    ''Select Case ext
    ''    Case ".jpg", ".png", ".gif"
    ''        ' PREPARE THE RESPONSE
    ''        context.Response.ContentType = "application/" & Replace(ext, ".", "")
    ''        context.Response.AddHeader("Content-Disposition", "attachment; filename=" & F.FileName)
    ''        context.Response.AddHeader("Content-Length", F.Size)
    ''        CopyStream(srcStream, context.Response.OutputStream)

    ''    Case Else
    ''        ' ZIP THE FILE
    ''        ' PREPARE THE RESPONSE
    ''        context.Response.ContentType = "application/zip"
    ''        context.Response.AddHeader("Content-Disposition", "attachment; filename=" & System.IO.Path.GetFileNameWithoutExtension(F.FileName) & ".zip")
    ''        'context.Response.AddHeader("Content-Length", F.Size)

    ''        ' USE XCEED TO WRITE ZIP DATA TO THE RESPONSE STREAM
    ''        ' Create a ZipItemLocalHeader, defining what will be the name of the file in the Zip file in the compressedMemoryStream
    ''        Dim itemHeader As ZipItemLocalHeader = New ZipItemLocalHeader("\" & F.FileName, CompressionMethod.Deflated, CompressionLevel.Normal)
    ''        ' Do the compression
    ''        CompressStream(itemHeader, srcStream, context.Response.OutputStream)
    ''End Select

    ''context.Response.End()

#End Region 'OLD 1

End Class