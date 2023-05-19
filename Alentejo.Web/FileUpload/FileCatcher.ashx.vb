Imports System.Net
Imports System.Web
Imports System.Web.Services
Imports Xceed.FileSystem
Imports Xceed.Http.Server
Imports SMT.NextLimit.Maxwell
Imports SMT.Alentejo.Core.Consts

Public Class FileCatcher
    Implements System.Web.IHttpHandler

    Shared Sub New()
        Xceed.Http.Licenser.LicenseKey = "UPS10-D8YXD-FU4KY-T8ZA"
    End Sub

    Sub ProcessRequest(ByVal context As HttpContext) Implements IHttpHandler.ProcessRequest
        Try
            'Set a default cookie, so we know that if the cookie is empty on the client side,
            'it means an unhandled exception happened on the sever, before the Page_Load event was triggered.
            'context.Response.Cookies.Add(New System.Web.HttpCookie("OnServer", "The server was reached"))

            Dim userid As String = context.Request.Headers.Item("UserId")
            Dim jobname As String = context.Request.Headers.Item("JobName")
            'Dim jobid As String = context.Request.Headers.Item("JobId")
            Dim jobid As String = Guid.NewGuid.ToString
            Dim path As String = ALENTEJO_SERVER_JOB_FILE_STORAGE & "\" & userid & "\" & jobid & "\"

            If Not System.IO.Directory.Exists(path) Then System.IO.Directory.CreateDirectory(path)

            'Initialize the Xceed FileSystem object that will serve as the destination, in this case, a folder
            Dim destinationFolder As AbstractFolder = New DiskFolder(path)
            'Use the request to get the list of files that were uploaded to the server
            Dim files As New UploadedFiles(context.Request)

            'Copy those files to the destination folder
            files.CopyFilesTo(destinationFolder, True, True)

            'Set the response status to OK since if we get to this line, everything went well
            context.Response.StatusCode = CInt(Fix(HttpStatusCode.OK))

            ' generate a jobId
            context.Response.Cookies.Add(New System.Web.HttpCookie("JobId", jobid))

            ' get camera names and add to response
            Dim cameras() As String = GetCameraNamesForMXS(path)
            Dim sb As New StringBuilder
            For b As Byte = 0 To UBound(cameras)
                If b = UBound(cameras) Then
                    sb.Append(cameras(b))
                Else
                    sb.Append(cameras(b) & "||")
                End If
            Next
            context.Response.Cookies.Add(New System.Web.HttpCookie("MaxwellCameras", sb.ToString))

        Catch ex As Exception
            ' We use the cookies to inform the Silverlight client of the exact exception since the StatusCode
            ' is not sufficient and the response stream is set to null on the client side when an exception
            ' occurs on the server side, so we can't use it to write the exception to it.
            ' You can use ex.ToString() to get the complete exception message and stack trace 
            context.Response.Cookies.Add(New System.Web.HttpCookie("ExceptionInFileUploads", ex.Message))

            'Write the exception to the output windows (when debuging)
            System.Diagnostics.Debug.WriteLine(ex.ToString())
            'Set the response status code to indicate that an exception occurred.
            context.Response.StatusCode = CInt(Fix(HttpStatusCode.InternalServerError))
        End Try

    End Sub

    ReadOnly Property IsReusable() As Boolean Implements IHttpHandler.IsReusable
        Get
            Return False
        End Get
    End Property

End Class