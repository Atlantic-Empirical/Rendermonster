Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.Consts
Imports SMT.AWS.S3
Imports SMT.AWS.Authorization
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.SystemwideMessaging

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://runuh.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class FileAPI
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetFileList(ByVal SessionToken As String, ByVal JobId As String, ByRef outFileList As List(Of cSMT_ATJ_File)) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outFileList = GetAtjFiles(JobId)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetUrlForFile(ByVal SessionToken As String, ByVal ImageFileId As String, ByRef outUrl As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            Dim S3 As New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey, False)
            outUrl = S3.GetObjectUrl_Signed(s3_bucket_files_us, ImageFileId, 900)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function SetFileType(ByVal SessionToken As String, ByVal FileId As String, ByVal FileType As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

End Class