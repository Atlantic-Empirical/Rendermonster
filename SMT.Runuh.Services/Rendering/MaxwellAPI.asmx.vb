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
<System.Web.Services.WebService(Namespace:="http://runuh.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class MaxwellAPI
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Method for user submission of Maxwell render job metadata.
    ''' </summary>
    ''' <param name="MJ"></param>
    ''' <returns>The new Job.Id (GUID string)</returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Public Function SubmitJob(ByVal SessionToken As String, ByVal MJ As cSMT_ATJ_RenderJob_Maxwell, ByRef outJobId As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
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

            outJobId = jid
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetCameraNames(ByVal SessionToken As String, ByVal JobId As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetJobDetails(ByVal SessionToken As String, ByVal JobId As String, ByRef outJobDetails As cSMT_ATJ_RenderJob_Maxwell) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            outJobDetails = GetJob(JobId)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

End Class