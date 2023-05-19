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
Public Class JobAPI
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetJobs(ByVal SessionToken As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetJobStatus(ByVal SessionToken As String, ByVal JobId As String, ByRef outJobStatus As Integer) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outJobStatus = GetJobAttribute(JobId, "_Status") - 100000
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetLiteJobInfo(ByVal SessionToken As String, ByVal JobInfoQuery As cSMT_ATJ_JobInfoQuery, ByRef outLiteJobInfo As List(Of cSMT_ATJ_LiteJobInfo)) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outLiteJobInfo = GetJobInfo_Lite(JobInfoQuery)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function TerminateJob(ByVal SessionToken As String, ByVal JobId As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            SetJobAttribute(JobId, "TerminateFlag", "True")
            SetJobStatus(JobId, eSMT_ATJ_RenderJob_Status.AWAITING_TERMINATION)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function ArchiveJob(ByVal SessionToken As String, ByVal JobId As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function DeleteJob(ByVal SessionToken As String, ByVal JobId As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetJobConsoleData(ByVal SessionToken As String, ByVal JobId As String, ByVal LastQueryTicks As Long, ByRef outConsoleData As List(Of cSMT_ATJ_JobProgressMessage)) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outConsoleData = GetJobProgress(JobId, LastQueryTicks, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.Status_major, AWS.SDB.eComparisonOperator.GreaterThanOrEqualTo)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetLatestImageFileInfoForJob(ByVal SessionToken As String, ByVal JobId As String, ByVal TokenLastImageGuid As String, ByRef outFileInfo As cSMT_ATJ_File) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outFileInfo = GetJobLatestImageFile(JobId, True, True, 900, TokenLastImageGuid)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetAllImageFileInfoForJob(ByVal SessionToken As String, ByVal JobId As String, ByRef outImageFileInfo As List(Of cSMT_ATJ_File)) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outImageFileInfo = GetImageFilesForJob(JobId)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try

        '    ' OLD - WORKS
        '    Dim pms As List(Of cSMT_ATJ_JobProgressMessage) = GetJobProgress(JobId, Nothing, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.NewRenderedImage, AWS.SDB.eComparisonOperator.EqualTo)
        '    Dim out As New List(Of String)
        '    For Each pm As cSMT_ATJ_JobProgressMessage In pms
        '        out.Add(pm.RenderedImageGuid)
        '    Next
        '    Return out.ToArray
    End Function

    <WebMethod()> _
    Public Function GetFileListForJob(ByVal SessionToken As String, ByVal JobId As String, ByRef outFiles As List(Of cSMT_ATJ_File)) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outFiles = GetAtjFiles(JobId)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    ''' <summary>
    ''' Returns the relative path to the DeepZoom XML that has been created for this image.
    ''' </summary>
    ''' <param name="JobId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function GetDeepZoopXMLPathForJob(ByVal SessionToken As String, ByVal JobId As String, ByVal Latest As Boolean, ByVal ImageGuid As String, ByRef outXMLPath As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            Dim guid As String
            If Latest Then
                guid = GetJobAttribute(JobId, "LatestImageId")
            Else
                guid = ImageGuid
            End If
            Dim dzc As New cSMT_ATJ_GenerateDeepZoom(JobId, guid, Server.MapPath("~\"))
            outXMLPath = "../../../" & dzc.GetXMLPath
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

End Class