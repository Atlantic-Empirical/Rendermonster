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
Imports SMT.NextLimit.Maxwell

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://rm.seqmt.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class AlentejoJobService
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Method for user submission of Maxwell render job metadata.
    ''' </summary>
    ''' <param name="MJ"></param>
    ''' <returns>The new Job.Id (GUID string)</returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function MX_SubmitJob(ByVal SessionToken As String, ByVal MJ As cSMT_ATJ_RenderJob_Maxwell) As String
        Try
            Dim jid As String = SaveJob(MJ)
            SetJobStatus(jid, eSMT_ATJ_RenderJob_Status.PREPROCESSING)
            SetJobStatus(jid, eSMT_ATJ_RenderJob_Status.Preprocessing_WaitingForJobFileTransferToS3)

            'CONFIGURE SAMPLE-LEVEL/NODE SETTINGS
            Dim PerNodeSampleLevel As Double = ComputeNodeLevelTarget(MJ.CoresRequested / 8, MJ.SampleCount)
            SetJobAttribute(jid, "SampleCount_PerNode", Math.Round(PerNodeSampleLevel, 5, MidpointRounding.AwayFromZero))

            'Dim path As String = Environment.GetEnvironmentVariable("ALENTEJO_JOB_STORAGE")
            'If path = "" Then Throw New Exception("ALENTEJO_JOB_STORAGE environment variable is not set.")
            'path &= "\" & MJ.UserId & "\" & MJ.Name & "\"

            'Enqueue message so files get transfered to S3 and the pipeline can continue!
            'EnqueueJobFileTransferMessage(New cJobFileTransferMessage_ToS3(jid, s3_bucket_job_files, path))
            Enqueue_JobNeedingFileTransferToS3(jid)

            Return jid
        Catch ex As Exception
            Return eAlentejoJobSubmitResultCode.GenericFailure.ToString
        End Try
    End Function

    <WebMethod()> _
    Public Function QueryJobInfo(ByVal SessionToken As String, ByVal JobInfoQuery As cSMT_ATJ_JobInfoQuery) As List(Of cSMT_ATJ_LiteJobInfo)
        'If Not AuthenticateWebServiceCall(SessionToken) Then Throw New Exception("Session invalid.")
        Try
            Dim out As List(Of cSMT_ATJ_LiteJobInfo)
            out = GetJobInfo_Lite(JobInfoQuery)
            Return out
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function GetJobById(ByVal SessionToken As String, ByVal JobId As String) As cSMT_ATJ_RenderJob_Base
        Try
            Return GetJob(JobId)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function GetLatestImage(ByVal JobId As String, ByVal TokenLastImageGuid As String) As cSMT_ATJ_File
        Try
            Return GetJobLatestImageFile(JobId, True, True, 900, TokenLastImageGuid)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function GetImageUrlForGuid(ByVal JobId As String, ByVal ImageGuid As String) As String
        Try
            Dim S3 As New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey, False)
            Dim out As String = S3.GetObjectUrl_Signed(s3_bucket_files_us, ImageGuid, 900)
            Return out
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

    <WebMethod()> _
    Public Function GetImageFileInfoForJob(ByVal JobId As String) As List(Of cSMT_ATJ_File)
        Try
            Return GetImageFilesForJob(JobId)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return Nothing
        End Try

        '    ' OLD - WORKS
        '    Dim pms As List(Of cSMT_ATJ_JobProgressMessage) = GetJobProgress(JobId, Nothing, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.NewRenderedImage, AWS.SDB.eComparisonOperator.EqualTo)
        '    Dim out As New List(Of String)
        '    For Each pm As cSMT_ATJ_JobProgressMessage In pms
        '        out.Add(pm.RenderedImageGuid)
        '    Next
        '    Return out.ToArray
    End Function

    ''' <summary>
    ''' Returns the relative path to the DeepZoom XML that has been created for this image.
    ''' </summary>
    ''' <param name="JobId"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function GetDeepZoopXMLPathForJob(ByVal JobId As String, ByVal Latest As Boolean, ByVal ImageGuid As String) As String
        Try
            Dim guid As String
            If Latest Then
                guid = GetJobAttribute(JobId, "LatestImageId")
            Else
                guid = ImageGuid
            End If
            Dim dzc As New cSMT_ATJ_GenerateDeepZoom(JobId, guid, Server.MapPath("~\"))
            Dim out As String = "../../../" & dzc.GetXMLPath
            Return out
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

    <WebMethod()> _
    Public Function GetProgressData(ByVal SessionToken As String, ByVal JobId As String, ByVal TokenTimestampTicks As String) As List(Of cSMT_ATJ_JobProgressMessage)
        Try
            Return GetJobProgress(JobId, TokenTimestampTicks, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.Status_major, AWS.SDB.eComparisonOperator.GreaterThanOrEqualTo)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function GetFileList(ByVal SessionToken As String, ByVal JobId As String) As List(Of cSMT_ATJ_File)
        Try
            Return GetAtjFiles(JobId)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function GetJobStatus(ByVal SessionToken As String, ByVal JobId As String) As Integer
        Try
            Return GetJobAttribute(JobId, "_Status") - 100000
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

    <WebMethod()> _
    Public Sub TerminateJob(ByVal SessionToken As String, ByVal JobId As String)
        Try
            SetJobAttribute(JobId, "TerminateFlag", "True")
            SetJobStatus(JobId, eSMT_ATJ_RenderJob_Status.AWAITING_TERMINATION)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, JobId, ex.Message, ex.StackTrace)
        End Try
    End Sub

End Class
