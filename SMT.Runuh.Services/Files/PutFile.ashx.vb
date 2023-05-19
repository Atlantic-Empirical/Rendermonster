Imports System.Net
Imports System.Web
Imports System.Web.Services
'Imports Xceed.FileSystem
'Imports Xceed.Http.Server
Imports SMT.Alentejo.Core.Consts
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.SessionManagement
Imports SMT.AWS.S3
'Imports Affirma.ThreeSharp.Model
Imports System.Threading

Public Class PutFile
    Implements System.Web.IHttpAsyncHandler

    Public Function BeginProcessRequest(ByVal context As System.Web.HttpContext, ByVal cb As System.AsyncCallback, ByVal extraData As Object) As System.IAsyncResult Implements System.Web.IHttpAsyncHandler.BeginProcessRequest
        context.Response.Write("<p>Begin IsThreadPoolThread is " & Thread.CurrentThread.IsThreadPoolThread & "</p>" & vbCrLf)
        Dim asynch As New AsynchOperation(cb, context, extraData)
        asynch.StartAsyncWork()
        Return asynch
    End Function

    Public Sub EndProcessRequest(ByVal result As System.IAsyncResult) Implements System.Web.IHttpAsyncHandler.EndProcessRequest
    End Sub

    Public Sub ProcessRequest(ByVal context As System.Web.HttpContext) Implements System.Web.IHttpHandler.ProcessRequest
        Throw New InvalidOperationException()
    End Sub

    Public ReadOnly Property IsReusable() As Boolean Implements System.Web.IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class

Class AsynchOperation
    Implements IAsyncResult

    Private _completed As Boolean
    Private _state As [Object]
    Private _callback As AsyncCallback
    Private _context As HttpContext

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
        _callback = callback
        _context = context
        _state = state
        _completed = False
    End Sub

    Public Sub StartAsyncWork()
        ThreadPool.QueueUserWorkItem(New WaitCallback(AddressOf StartAsyncTask), Nothing)
    End Sub

    Private Sub StartAsyncTask(ByVal workItemState As [Object])
        _context.Response.Write("<p>Completion IsThreadPoolThread is " & Thread.CurrentThread.IsThreadPoolThread & "</p>" & vbCrLf)

        '_context.Response.Write("Hello World from Async Handler!")

        Try
            'Set a default cookie, so we know that if the cookie is empty on the client side,
            'it means an unhandled exception happened on the sever, before the Page_Load event was triggered.
            'context.Response.Cookies.Add(New System.Web.HttpCookie("OnServer", "The server was reached"))

            'get request properties
            Dim SessionToken As String = _context.Request.Headers("SessionToken")
            If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")

            Dim FileCategory As cSMT_ATJ_File.eAtjFileType = _context.Request.Headers("FileCategory")
            Dim FileName As String = _context.Request.Headers("FileName")
            If String.IsNullOrEmpty(SessionToken) Then
                _context.Response.StatusCode = CInt(Fix(HttpStatusCode.Unauthorized))
                _completed = False
                _callback(Me)
                Exit Sub
            End If
            If String.IsNullOrEmpty(FileName) Then
                _context.Response.StatusCode = CInt(Fix(HttpStatusCode.ExpectationFailed))
                _completed = False
                _callback(Me)
                Exit Sub
            End If

            Dim UserId As String = GetUserIdForSession(SessionToken)

            'Dim BucketName As String = _context.Request.Headers.Item("Bucket")
            Dim BucketName As String = s3_bucket_files_us
            Dim ObjectKey As String = Guid.NewGuid.ToString

            Dim success As Boolean = SMT_AWS_S3_Inline.UploadFile(BucketName, ObjectKey, False, _context.Request.ContentType, _context.Request.ContentLength, _context.Request.InputStream)

            If success Then
                _context.Response.StatusCode = CInt(Fix(HttpStatusCode.OK))
                _context.Response.Cookies.Add(New System.Web.HttpCookie("ObjectKey", ObjectKey))
            Else
                _context.Response.StatusCode = CInt(Fix(HttpStatusCode.InternalServerError))
            End If

            Dim f As New cSMT_ATJ_File(ObjectKey, FileName, "", FileCategory, _context.Request.ContentLength, UserId)
            SaveAtjFile(f)

        Catch ex As Exception
            ' We use the cookies to inform the Silverlight client of the exact exception since the StatusCode
            ' is not sufficient and the response stream is set to null on the client side when an exception
            ' occurs on the server side, so we can't use it to write the exception to it.
            ' You can use ex.ToString() to get the complete exception message and stack trace 
            _context.Response.Cookies.Add(New System.Web.HttpCookie("ExceptionInFileUploads", ex.Message))

            'Write the exception to the output windows (when debuging)
            System.Diagnostics.Debug.WriteLine(ex.ToString())
            'Set the response status code to indicate that an exception occurred.
            _context.Response.StatusCode = CInt(Fix(HttpStatusCode.InternalServerError))
        End Try


        _completed = True
        _callback(Me)

    End Sub 'StartAsyncTask

End Class 'AsynchOperation

'Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpAsyncHandler.ProcessRequest
'    Try
'        'Set a default cookie, so we know that if the cookie is empty on the client side,
'        'it means an unhandled exception happened on the sever, before the Page_Load event was triggered.
'        'context.Response.Cookies.Add(New System.Web.HttpCookie("OnServer", "The server was reached"))

'        'get request properties
'        Dim BucketName As String = context.Request.Headers.Item("Bucket")
'        'Dim BucketName As String = s3_bucket_files_us
'        Dim ObjectKey As String = Guid.NewGuid.ToString

'        Dim success As Boolean = SMT_AWS_S3_Inline.UploadFile(BucketName, ObjectKey, False, context.Request.ContentType, context.Request.ContentLength, context.Request.InputStream)

'        If success Then
'            context.Response.StatusCode = CInt(Fix(HttpStatusCode.OK))
'            context.Response.Cookies.Add(New System.Web.HttpCookie("ObjectKey", ObjectKey))
'        Else
'            context.Response.StatusCode = CInt(Fix(HttpStatusCode.InternalServerError))
'        End If

'    Catch ex As Exception
'        ' We use the cookies to inform the Silverlight client of the exact exception since the StatusCode
'        ' is not sufficient and the response stream is set to null on the client side when an exception
'        ' occurs on the server side, so we can't use it to write the exception to it.
'        ' You can use ex.ToString() to get the complete exception message and stack trace 
'        context.Response.Cookies.Add(New System.Web.HttpCookie("ExceptionInFileUploads", ex.Message))

'        'Write the exception to the output windows (when debuging)
'        System.Diagnostics.Debug.WriteLine(ex.ToString())
'        'Set the response status code to indicate that an exception occurred.
'        context.Response.StatusCode = CInt(Fix(HttpStatusCode.InternalServerError))
'    End Try

'End Sub

'ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
'    Get
'        Return False
'    End Get
'End Property

'End Class

'Public Class PutFile
'    Implements System.Web.IHttpHandler

'    Shared Sub New()
'        Xceed.Http.Licenser.LicenseKey = "UPS10-D8YXD-FU4KY-T8ZA"
'    End Sub

'    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
'        Try
'            'Set a default cookie, so we know that if the cookie is empty on the client side,
'            'it means an unhandled exception happened on the sever, before the Page_Load event was triggered.
'            'context.Response.Cookies.Add(New System.Web.HttpCookie("OnServer", "The server was reached"))

'            Dim userid As String = context.Request.Headers.Item("UserId")
'            Dim jobname As String = context.Request.Headers.Item("JobName")
'            'Dim jobid As String = context.Request.Headers.Item("JobId")
'            Dim jobid As String = Guid.NewGuid.ToString
'            Dim path As String = ALENTEJO_SERVER_JOB_FILE_STORAGE & "\" & userid & "\" & jobid & "\"

'            If Not System.IO.Directory.Exists(path) Then System.IO.Directory.CreateDirectory(path)

'            'Initialize the Xceed FileSystem object that will serve as the destination, in this case, a folder
'            Dim destinationFolder As AbstractFolder = New DiskFolder(path)
'            'Use the request to get the list of files that were uploaded to the server
'            Dim files As New UploadedFiles(context.Request)

'            'Copy those files to the destination folder
'            files.CopyFilesTo(destinationFolder, True, True)

'            'Set the response status to OK since if we get to this line, everything went well
'            context.Response.StatusCode = CInt(Fix(HttpStatusCode.OK))

'            ' generate a jobId
'            context.Response.Cookies.Add(New System.Web.HttpCookie("JobId", jobid))

'            '' get camera names and add to response
'            'Dim cameras() As String = GetCameraNamesForMXS(path)
'            'Dim sb As New StringBuilder
'            'For b As Byte = 0 To UBound(cameras)
'            '    If b = UBound(cameras) Then
'            '        sb.Append(cameras(b))
'            '    Else
'            '        sb.Append(cameras(b) & "||")
'            '    End If
'            'Next
'            'context.Response.Cookies.Add(New System.Web.HttpCookie("MaxwellCameras", sb.ToString))

'        Catch ex As Exception
'            ' We use the cookies to inform the Silverlight client of the exact exception since the StatusCode
'            ' is not sufficient and the response stream is set to null on the client side when an exception
'            ' occurs on the server side, so we can't use it to write the exception to it.
'            ' You can use ex.ToString() to get the complete exception message and stack trace 
'            context.Response.Cookies.Add(New System.Web.HttpCookie("ExceptionInFileUploads", ex.Message))

'            'Write the exception to the output windows (when debuging)
'            System.Diagnostics.Debug.WriteLine(ex.ToString())
'            'Set the response status code to indicate that an exception occurred.
'            context.Response.StatusCode = CInt(Fix(HttpStatusCode.InternalServerError))
'        End Try

'    End Sub

'    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
'        Get
'            Return False
'        End Get
'    End Property

'End Class
