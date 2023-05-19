#Region "IMPORTS"

Imports SMT.AWS.EC2
Imports SMT.Alentejo.Core
Imports SMT.Alentejo.Core.Consts
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.JobManagement.Classes
Imports SMT.Alentejo.Core.InstanceManagement
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.NextLimit.Maxwell
Imports SMT.AWS.S3
Imports SMT.AWS.Authorization
Imports System.IO
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.AtjTrace
Imports SMT.Alentejo.Credit.Write
Imports System.Text

#End Region 'IMPORTS

Public Class RenderManager_Main

#Region "FIELDS & PROPERTIES"

    Private HEAVY_TRACE As Boolean = True

#Region "THE JOB"

    Private the_job As cSMT_ATJ_RenderJob_Base

    Private Property the_job_maxwell() As cSMT_ATJ_RenderJob_Maxwell
        Get
            If the_job Is Nothing Then Return Nothing
            Return TryCast(the_job, cSMT_ATJ_RenderJob_Maxwell)
        End Get
        Set(ByVal value As cSMT_ATJ_RenderJob_Maxwell)
            the_job = value
        End Set
    End Property

#End Region 'THE JOB

#Region "INSTANCE INFORMATION"

    Private ReadOnly Property InstanceId() As String
        Get
            If __InstanceId = "" Then __InstanceId = _InstanceId
            Return __InstanceId
        End Get
    End Property
    Private __InstanceId As String = ""

    Private ReadOnly Property LocalIp() As String
        Get
            If __LocalIp = "" Then __LocalIp = _LocalIp
            Return __LocalIp
        End Get
    End Property
    Private __LocalIp As String = ""

    Private ReadOnly Property PublicIp() As String
        Get
            If __PublicIp = "" Then __PublicIp = _PublicIp
            Return __PublicIp
        End Get
    End Property
    Private __PublicIp As String = ""

    Private ReadOnly Property LocalHostname() As String
        Get
            If __LocalHostname = "" Then __LocalHostname = _LocalHostname
            Return __LocalHostname
        End Get
    End Property
    Private __LocalHostname As String = ""

    Private ReadOnly Property PublicHostname() As String
        Get
            If __PublicHostname = "" Then __PublicHostname = _PublicHostname
            Return __PublicHostname
        End Get
    End Property
    Private __PublicHostname As String = ""

#End Region 'INSTANCE INFORMATION

#Region "PATHS"

    Private ReadOnly Property JobFileNameWithoutExtension() As String
        Get
            If the_job Is Nothing Then Return ""
            Return Path.GetFileNameWithoutExtension(the_job.JobFileNames(0))
        End Get
    End Property

    Private ReadOnly Property job_output_image(Optional ByVal Merged As Boolean = False) As String
        Get
            If the_job Is Nothing Then Return ""
            If IsCooperativeRender And Merged Then
                Return PathToLastCompletedMerge_JPG
            Else
                Return job_output_dir & "render_output" & the_job.OutputImageFormat
            End If
        End Get
    End Property

    Private ReadOnly Property job_output_dir() As String
        Get
            If the_job Is Nothing Then Return ""
            Return ALENTEJO_RENDER_ENGINE_OUTPUT & the_job.Id & "\"
        End Get
    End Property

    Private ReadOnly Property job_output_mxi(Optional ByVal Merged As Boolean = False) As String
        Get
            If the_job Is Nothing Then Return ""
            If IsCooperativeRender And Merged Then
                Return PathToLastCompletedMerge_MXI
            Else
                Return job_output_dir & "render_output.mxi"
            End If
        End Get
    End Property

    Private ReadOnly Property merge_dir() As String
        Get
            Dim out As String = job_output_dir & "merge\"
            If Not Directory.Exists(out) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property

#End Region 'PATHS

#Region "BENCHMARK & SAMPLE LEVEL"

    Private ReadOnly Property CurrentSampleLevel(ByVal LocalOnly As Boolean) As Double
        Get
            If IsCooperativeRender And Not LocalOnly Then
                Return _CooperativeSampleLevel
            Else
                Return _UniSampleLevel
            End If
        End Get
    End Property

    Private ReadOnly Property CurrentSampleLevelFriendlyString(ByVal LocalOnly As Boolean) As String
        Get
            Return Math.Round(CurrentSampleLevel(LocalOnly), 2).ToString.Replace(".", "-")
        End Get
    End Property

    Private ReadOnly Property CooperativeBenchmark() As Double
        Get
            Return Math.Round(_CooperativeBenchmark, 2)
        End Get
    End Property
    Private _CooperativeBenchmark As Double = 0

    Private ReadOnly Property CooperativeSampleLevel() As Double
        Get
            Return Math.Round(_CooperativeSampleLevel, 2)
        End Get
    End Property
    Private _CooperativeSampleLevel As Double = 0

    ''' <summary>
    ''' "non-cooperative" sample level (when there is only one node doing the rendering - this one)
    ''' </summary>
    ''' <remarks></remarks>
    Private _UniSampleLevel As Double = 0
    Private _UniBenchmark As Double = 0

#End Region 'BENCHMARK & SAMPLE LEVEL

    ''' <summary>
    ''' Interval in seconds between checks of storage for jobs assigned to this instance.
    ''' </summary>
    ''' <remarks></remarks>
    Private job_loop_interval As Byte = 5
    Private assigned_to_jobId As String = ""

    Private ReadOnly Property IsMasterInstance() As Boolean
        Get
            If String.IsNullOrEmpty(_IsMasterInstance) Then
                _IsMasterInstance = GetInstanceAttribute(InstanceId, "IsMaster")
            End If
            Return Boolean.Parse(_IsMasterInstance)
            'Return Boolean.Parse(GetInstanceAttribute(InstanceId, "IsMaster"))
        End Get
    End Property
    Private _IsMasterInstance As String = Nothing

    Private ReadOnly Property IsCooperativeRender() As Boolean
        Get
            If the_job Is Nothing Then Return False
            Return the_job.CoresRequested > 8
        End Get
    End Property

    Private ReadOnly Property DoCooperativeRenderMergeForEachLevel() As Boolean
        Get
            If the_job_maxwell Is Nothing Then Return False
            Return Not String.IsNullOrEmpty(the_job_maxwell.Option_MergeCooperativeLevels) AndAlso the_job_maxwell.Option_MergeCooperativeLevels = "True"
        End Get
    End Property

#End Region 'FIELDS & PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        AddConsoleLine("INITIALIZED" & TS)
        _HandleTransferStatusEvent_UIThread_Delegate = New HandleTransferStatusEvent_UIThread_Delegate(AddressOf HandleTransferStatusEvent_UIThread)
    End Sub

#End Region 'CONSTRUCTOR

#Region "FORM"

    Private Sub RenderManager_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        StartupRoutiene()
    End Sub

#End Region 'FORM

#Region "STARTUP"

    Private Sub StartupRoutiene()
        Try
            AddConsoleLine("ENTER_STARTUP_ROUTIENE")
            AddConsoleLine("InstanceId  =  " & InstanceId)
            AddConsoleLine("LocalIp  =  " & LocalIp)
            AddConsoleLine("PublicHostname  =  " & PublicHostname)
            SetInstanceAttribute(InstanceId, "LocalIp", LocalIp)
            SetInstanceAttribute(InstanceId, "PublicIp", PublicIp)
            SetInstanceAttribute(InstanceId, "LocalHostname", LocalHostname)
            SetInstanceAttribute(InstanceId, "PublicHostname", PublicHostname)
            StartJobWaitLoop()
            AddConsoleLine("EXIT_STARTUP_ROUTIENE")
            AddConsoleLine("")
            WriteEvent("RENDER MANAGER STARTED", "Render Manager", EventLogEntryType.Information, eSystemLog.System)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, InstanceId, ex.Message, ex.StackTrace)
            'TODO: should we hard fail here? Just go into loop? What? IMPORTANT!
        End Try
    End Sub

#End Region 'STARTUP

#Region "WAIT FOR JOB LOOP"

    Private WithEvents JOB_WAIT_LOOP As System.Timers.Timer

    Private Sub StartJobWaitLoop()
        JOB_WAIT_LOOP = New System.Timers.Timer(1000 * job_loop_interval)
        JOB_WAIT_LOOP.Start()
        JobLoopTasks()
    End Sub

    Private Sub KillJobWaitLoop()
        JOB_WAIT_LOOP.Stop()
        JOB_WAIT_LOOP.Dispose()
    End Sub

    Private Sub JOB_WAIT_LOOP_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles JOB_WAIT_LOOP.Elapsed
        AddConsoleLine("POLL" & TS)
        JobLoopTasks()
    End Sub

    Private Sub JobLoopTasks()
        Try
            assigned_to_jobId = GetInstanceAttribute(InstanceId, "AssignedToJobId")
            If String.IsNullOrEmpty(assigned_to_jobId) Then Exit Sub

            'TIDY up the directories
            CleanupDirectories()

            'WE'VE GOT ONE!
            WriteEvent("Job Received: " & assigned_to_jobId, "Render Manager", EventLogEntryType.Information, eSystemLog.System)
            KillJobWaitLoop()

            AddConsoleLine("************")
            AddConsoleLine("JOB RECEIVED" & TS)
            AddConsoleLine("JobId = " & assigned_to_jobId)
            AddConsoleLine("Perform Role: " & If(IsMasterInstance, "Master", "Node"))

            PeformJob()

            AddConsoleLine("************")

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, assigned_to_jobId, ex.Message, ex.StackTrace)
            'TODO: what to do here? Hard fail? 
        Finally

        End Try
    End Sub

#End Region 'WAIT FOR JOB LOOP

#Region "PERFORM JOB"

    Private Sub PeformJob()
        Try
            AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name)

            ' GET THE JOB FROM STORAGE
            _IsMasterInstance = Nothing
            the_job_maxwell = GetJob(assigned_to_jobId)

            ' UPDATE THE USER'S CREDIT
            If IsMasterInstance Then UpdateUserCredit(True)

            ' CLEAR THE BENCHMARK FOR THIS INSTANCE
            UpdateInstanceBenchMark(0)

            ' CHECK FOR MANUAL TERMINATION SWITCH
            If Not String.IsNullOrEmpty(the_job.TerminateFlag) Then
                TerminateRender()
            Else
                StartTerminationTimer()
            End If

            ' KICK OFF FILE RETRIEVAL
            RetrieveFilesForJob()

        Catch ex As Exception
            Throw New Exception("Problem with PeformJob(). Error: " & ex.Message, ex)
            'TODO: again.
        End Try
    End Sub

    Private Sub ContinueAfterJobFilesRetrievedSuccessfully()
        Try
            If IsMasterInstance Then
                AssessAndCoordinateRenderNodes()
                SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Preprocessing_FinalJobPrepPreRendering)
                'RunMXCL(System.Environment.ProcessorCount - 1)
            Else
                'RunMXCL(System.Environment.ProcessorCount)
            End If
#If DEBUG Then
            RunMXCL(System.Environment.ProcessorCount -1) 'provide some overhead to work with the instance
#Else
            RunMXCL(System.Environment.ProcessorCount)
#End If
        Catch ex As Exception
            Throw New Exception("Problem with ContinueAfterJobFilesRetrievedSuccessfully(). Error: " & ex.Message, ex)
        End Try
    End Sub

#Region "TERMINATION POLLING"

    Private WithEvents TERMINATION_TIMER As System.Timers.Timer

    Private Sub StartTerminationTimer()
        TERMINATION_TIMER = New System.Timers.Timer(5000)
        TERMINATION_TIMER.Start()
    End Sub

    Private Sub StopTerminationTimer()
        TERMINATION_TIMER.Stop()
        TERMINATION_TIMER.Dispose()
    End Sub

    Private Sub TERMINATION_TIMER_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles TERMINATION_TIMER.Elapsed
        AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name)
        Dim tf As String = GetJobAttribute(the_job.Id, "TerminateFlag")
        If String.IsNullOrEmpty(tf) Then Exit Sub
        TerminateRender()
    End Sub

    Private Sub TerminateRender()
        AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name)
        TERMINATION_TIMER.Stop()
        AddConsoleLine("**************************")
        AddConsoleLine("*MXCL MANUALLY TERMINATED*")
        AddConsoleLine("**************************")
        'DeleteJobAttribute(the_job.Id, "TerminateFlag")
        If MXCL Is Nothing Then
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.COMPLETED)
            SetInstanceStatus(InstanceId, eSMT_ATJ_Instance_Status.NotAvailable_PendingPeerNodeExit)
        Else
            MXCL.Dispose()
            FinalizeJob()
        End If
    End Sub

#End Region 'TERMINATION POLLING

#Region "JOB FINALIZATION"

    Private Sub FinalizeJob()
        Try
            'how do we know if this is the result of a failure or of completion of rendering?
            'LOOK AT THE OUTPUT. 
            '   If there is supposed to be an MXI and there isn't then it is a failure.
            '   If there is no image output then it is a failure

            AddConsoleLine("FinalizeJob()")

            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.POSTPROCESSING)
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Postprocessing_InitialActivities)

            If IsCooperativeRender Then
                Threading.Thread.Sleep(5000) 'giving the mxcl class time to clean up and flush the final sample level to render manager. the need for this became apparent because the EXIT event was coming before the last sample level and this was causing corruption in the merged image/mxi.
                StartRenderNodeTimer()
            Else
                TransferFinalRenderOutputFiles()
            End If

        Catch ex As Exception
            AddConsoleLine("")
            AddConsoleLine("MasterJobFinalization FAILED.")
            AddConsoleLine("")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Failure_RenderJobFinalization)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub FinalizeJob_FileUploadComplete()
        Try
            AddConsoleLine("FinalizeJob_FileUploadComplete()")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.COMPLETED)
            SetInstanceAttribute(InstanceId, "AssignedToJobId", "")
            SetInstanceStatus(InstanceId, eSMT_ATJ_Instance_Status.Available)
            If IsCooperativeRender Then
                'Release the render nodes to be used on other jobs.
                For Each rni As cSMT_ATJ_RenderNodeStatus In RenderNodes
                    SetInstanceStatus(rni.Id, eSMT_ATJ_Instance_Status.Available)
                Next
            End If
            StartJobWaitLoop()
            Dim msg As String = "Job completed. Returning to polling. JobId = " & the_job.Id
            AddConsoleLine(msg)
            WriteEvent(msg, "Render Manager", EventLogEntryType.Information, eSystemLog.System)
        Catch ex As Exception
            AddConsoleLine("")
            AddConsoleLine("FinalizeJob_Master_FileUploadComplete FAILED.")
            AddConsoleLine("")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Failure_RenderJobFinalization)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

#End Region 'JOB FINALIZATION

#Region "S3"

    Private ReadOnly Property S3() As cSMT_AWS_S3
        Get
            Return New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
        End Get
    End Property

    Private Sub RetrieveFilesForJob()
        Try
            AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name)
            If the_job Is Nothing Then Throw New Exception("Unexpected. the_job_maxwell is Nothing.")
            AddConsoleLine("Retrieving files for job: " & the_job.Name & " (" & the_job.Id & ")")
            If IsMasterInstance Then SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Preprocessing_RetrievingJobFiles)

            Dim TM As New cSMT_AWS_S3_TransferManager(S3)
            TM.Name = "JobFilesDownload"
            AddHandler TM.evTransferStatus, AddressOf HandleTransferStatus_TransferThread

            For b As Byte = 0 To UBound(the_job.JobFileGuids)
                AddConsoleLine("Start download: " & the_job.JobFileNames(b))
                TM.DownloadFile(the_job.JobFileGuids(b), ALENTEJO_INSTANCE_JOB_FILE_STORAGE & the_job.JobFileNames(b), cSMT_AWS_S3_FileTransfer.eBucketLocation.US, s3_bucket_files_us)
            Next

        Catch ex As Exception
            Throw New Exception("Problem with RetrieveFilesForJob(). Error: " & ex.Message, ex)
            'TODO: do something sensible here.
        End Try
    End Sub

    Private Sub TransferFinalRenderOutputFiles()
        Try
            AddConsoleLine("TransferRenderOutputFiles()")

            If the_job Is Nothing Then Throw New Exception("Unexpected. the_job is Nothing.")

            AddConsoleLine("Moving output files to S3 for job: " & the_job.Name & " (" & the_job.Id & ")")

            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Postprocessing_TransferringRenderFilesToS3)

            ' CREATE THE TRANSFER JOB
            Dim upload_files As New List(Of String)
            Dim upload_guids As New List(Of String)
            Dim g As String
            Dim fi As FileInfo

            'THIS IS ONLY APPROPRIATE IF THE JOB WAS RENDERED WITH MAXWELL
            'here we need the mxi and the last sample
            If the_job_maxwell IsNot Nothing Then
                If Not File.Exists(job_output_mxi(True)) Then
                    SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, the_job.Id, "MXI file does not exist as expected.", "")
                Else
                    upload_files.Add(job_output_mxi(True))
                    g = Guid.NewGuid.ToString
                    upload_guids.Add(g)
                    SetJobAttribute(the_job.Id, "MXIFileId", g)

                    'store in files domain
                    fi = New FileInfo(job_output_mxi(True))
                    SaveAtjFile(New cSMT_ATJ_File(g, JobFileNameWithoutExtension & ".mxi", the_job.Id, cSMT_ATJ_File.eAtjFileType.MXI, fi.Length, the_job.UserId))
                End If
            End If

            upload_files.Add(job_output_image(True))
            g = Guid.NewGuid.ToString
            upload_guids.Add(g)

            'store in files domain
            fi = New FileInfo(job_output_image)
            SaveAtjFile(New cSMT_ATJ_File(g, JobFileNameWithoutExtension & the_job.OutputImageFormat, the_job.Id, cSMT_ATJ_File.eAtjFileType.FinalOutputImage, fi.Length, the_job.UserId))

            'start the transfers
            Dim TM As New cSMT_AWS_S3_TransferManager(S3)
            TM.Name = "FinalFilesUpload"
            AddHandler TM.evTransferStatus, AddressOf HandleTransferStatus_TransferThread
            Dim t As cAS3_ObjectReference
            For b As Byte = 0 To upload_guids.Count - 1
                AddConsoleLine("Start upload: " & upload_files(b))
                t = TM.UploadFile(upload_files(b), cSMT_AWS_S3_FileTransfer.eBucketLocation.US, s3_bucket_files_us, upload_guids(b))
                If t Is Nothing Then
                    AddConsoleLine("FAILURE TO START UPLOAD: " & upload_files(b))
                End If
            Next

        Catch ex As Exception
            AddConsoleLine("")
            AddConsoleLine("TransferRenderOutputFiles FAILED.")
            AddConsoleLine("")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Failure_FileTransferToS3)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

#Region "S3:TRANSFER EVENTS"

    Private Sub HandleTransferStatus_TransferThread(ByVal sender As Object, ByVal e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
        Dim t As cSMT_AWS_S3_TransferManager = CType(sender, cSMT_AWS_S3_TransferManager)
        Select Case e.EventType
            Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Progress
                'Debug.WriteLine("TransferProgress = " & FileName & " Bytes Transferred = " & BytesTransferred & " Bytes Remaining = " & BytesRemaining & " Elapsed = " & ElapsedSeconds & "s")
                AddConsoleLine("Transfer Name=" & t.Name & " TransferProgress=" & e.FileName & " Bytes Transferred=" & e.BytesTransferred & " Bytes Remaining=" & e.BytesRemaining & " Elapsed=" & e.ElapsedSeconds & "s")
            Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Success
                AddConsoleLine("Transfer Name=" & t.Name & " Transfer succeeded = " & e.FileName & " Key = " & e.ObjectKey)
                Me.Invoke(_HandleTransferStatusEvent_UIThread_Delegate, New Object() {t, e})
            Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Failure
                AddConsoleLine("Transfer Name=" & t.Name & " Transfer failed = " & e.FileName & " Key = " & e.ObjectKey)
                'Me.Invoke(_HandleTransferStatusEvent_UIThread_Delegate, New Object() {t, e})

                If Path.GetExtension(e.FileName).ToLower = ".mxs" Then
                    'failed to get the job file. perhaps a timeout. just try again
                    RetrieveFilesForJob()
                End If

        End Select
    End Sub

    Private Sub HandleTransferStatusEvent_UIThread(ByRef sender As cSMT_AWS_S3_TransferManager, ByRef e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
        Try
            AddConsoleLine(sender.Name & " - " & e.ToString)

            Select Case sender.Name

                Case "JobFilesDownload"
                    If sender.TransfersCompleted Then
                        AddConsoleLine("")
                        AddConsoleLine("JOB FILES RETRIEVED SUCCESSFULLY")
                        AddConsoleLine("")
                        ContinueAfterJobFilesRetrievedSuccessfully()
                    End If

                Case "FinalFilesUpload"
                    SetFileAvailable(e.ObjectKey)
                    Select Case Path.GetExtension(e.FileName).ToLower
                        Case ".mxi"
                            SetJobAttribute(the_job.Id, "MXI", e.ObjectKey)

                        Case the_job.OutputImageFormat.ToLower
                            SetJobAttribute(the_job.Id, "FinalImage", e.ObjectKey)
                            SetJobAttribute(the_job.Id, "LatestImageId", e.ObjectKey)
                    End Select

                    If sender.TransfersCompleted Then
                        FinalizeJob_FileUploadComplete()
                    End If

                Case "IntermediateFileUpload"
                    SetFileAvailable(e.ObjectKey)
                    SetJobAttribute(the_job.Id, "LatestImageId", e.ObjectKey)

            End Select

            If sender.TransfersCompleted Then
                sender.Dispose()
            End If

        Catch ex As Exception
            AddConsoleLine("Problem in HandleTransferStatusEvent_UIThread(). Error: " & ex.Message)
        End Try
    End Sub
    Private Delegate Sub HandleTransferStatusEvent_UIThread_Delegate(ByRef sender As cSMT_AWS_S3_TransferManager, ByRef e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
    Private _HandleTransferStatusEvent_UIThread_Delegate As HandleTransferStatusEvent_UIThread_Delegate

#End Region 'S3:TRANSFER EVENTS

#End Region 'S3

#End Region 'PERFORM JOB

#Region "MASTER"

#Region "MASTER:COOPERATIVE RENDERING"

    Private RenderNodes As List(Of cSMT_ATJ_RenderNodeStatus)
    Private MergeNumber As Byte = 0
    Private PathToLastCompletedMerge_MXI As String
    Private PathToLastCompletedMerge_JPG As String

    ''' <summary>
    ''' Timer used to loop while waiting for node instances to complete rendering.
    ''' </summary>
    ''' <remarks></remarks>
    Private WithEvents RENDER_NODE_STATUS_TIMER As System.Timers.Timer

    ''' <summary>
    ''' Merges the MXIs from each of the nodes into one MXI, gets the sample level and an image.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub PerformMerge(Optional ByVal Final As Boolean = False)
        Try
            AddConsoleLine("PerformMerge() - ENTER")
            Dim OutputPath As String = job_output_dir & "merged"
            Dim MXIs As New List(Of String)
            For Each rns As cSMT_ATJ_RenderNodeStatus In RenderNodes
                If String.IsNullOrEmpty(rns.MxiPath) Then
                    rns.MxiPath = GetRenderNodeJobMxiPath(rns.Id)
                End If
                'Vet the UNC paths to the render node MXIs
                If Not File.Exists(rns.MxiPath) Then
                    AddConsoleLine("MXI is not reachable/does not exist: " & rns.MxiPath)
                    Exit Sub
                End If
                MXIs.Add(rns.MxiPath)
            Next
            If Not File.Exists(job_output_mxi) Then
                AddConsoleLine("MXI is not reachable/does not exist: " & job_output_mxi)
            End If
            MXIs.Add(job_output_mxi) 'the local MXI

            Dim merge_mxi_path As String = merge_dir & "merge_" & MergeNumber & ".mxi"
            Dim merge_jpg_path As String = merge_dir & "merge_" & MergeNumber & ".jpg"

            'log the info
            For Each mxiPath As String In MXIs
                AddConsoleLine(mxiPath)
            Next
            AddConsoleLine(merge_jpg_path)

            Dim mxii As cMXIInfo = MergeMXIs(MXIs.ToArray, merge_mxi_path, merge_jpg_path)
            _CooperativeSampleLevel = mxii.SampleLevel
            _CooperativeBenchmark = 0 'figure this out, if possible, currently waiting for a reply from NL
            PathToLastCompletedMerge_MXI = merge_mxi_path
            PathToLastCompletedMerge_JPG = merge_jpg_path

            If Final Then
                TransferFinalRenderOutputFiles()
            Else
                Dim m As New cSMT_ATJ_JobProgressMessage(the_job.Id, "", cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.NewSampleLevel)
                m.Benchmark = CooperativeBenchmark
                m.SampleLevel = CooperativeSampleLevel
                AddJobProgressMessage(m)
                UpdateUserCredit(False)
            End If
            AddConsoleLine("PerformMerge() - EXIT")
            MergeNumber += 1
        Catch ex As Exception
            AddConsoleLine("Problem with PerformMerge(). Error: " & ex.Message)
        End Try
    End Sub

    ''' <summary>
    ''' Setup the array of paths to the shared folders on each of the associated render nodes.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub AssessAndCoordinateRenderNodes()
        Try
            If the_job Is Nothing Then Throw New Exception("Unexpected. the_job is Nothing.")
            AddConsoleLine("ENTER RENDER NODE SETUP")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Preprocessing_CheckingForRenderNodes)

            Dim rns_csv As String = GetJobAttribute(the_job.Id, "RenderNodeInstanceIds")
            RenderNodes = New List(Of cSMT_ATJ_RenderNodeStatus)
            For Each rni As String In Split(rns_csv, ",")
                RenderNodes.Add(New cSMT_ATJ_RenderNodeStatus(rni))
            Next

            AddConsoleLine("EXIT RENDER NODE SETUP")
        Catch ex As Exception
            AddConsoleLine("")
            AddConsoleLine("RENDER NODE COORDINATION FAILED.")
            AddConsoleLine("")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Failure_RenderNodeCoordination)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub StartRenderNodeTimer()
        RENDER_NODE_STATUS_TIMER = New System.Timers.Timer(5000)
        RENDER_NODE_STATUS_TIMER.Start()
    End Sub

    Private Sub StopRenderNodeTimer()
        RENDER_NODE_STATUS_TIMER.Stop()
        RENDER_NODE_STATUS_TIMER.Dispose()
        RENDER_NODE_STATUS_TIMER = Nothing
    End Sub

    Private Sub RENDER_NODE_STATUS_TIMER_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles RENDER_NODE_STATUS_TIMER.Elapsed
        AddConsoleLine("RENDER_NODE_STATUS_TIMER_Elapsed()")
        'check the status of the instances that were used by this job.
        'if their 'assignedtojobid is null or empty then the job is done on that node.
        Dim Complete As Boolean = True
        For Each rns As cSMT_ATJ_RenderNodeStatus In RenderNodes
            rns.Complete = (GetInstanceAttribute(rns.Id, "Status") = "-2")
            Complete = rns.Complete
        Next
        If Complete Then
            AddConsoleLine("Nodes are finished.")
            StopRenderNodeTimer()
            PerformMerge(True)
        Else
            AddConsoleLine("Nodes are not finished.")
        End If
    End Sub

    Private Function GetRenderNodeJobMxiPath(ByRef RenderInstanceId As String) As String
        Try
            Dim out As New StringBuilder
            out.Append("\\")
            For Each rni As cSMT_ATJ_RenderNodeStatus In RenderNodes
                If rni.Id = RenderInstanceId Then
                    If String.IsNullOrEmpty(rni.LocalIp) Then
                        rni.LocalIp = GetInstanceAttribute(rni.Id, "LocalIp")
                    End If
                    out.Append(rni.LocalIp & "\")
                    out.Append("alentejo_render_engine_output\")
                    out.Append(the_job.Id & "\")
                    out.Append("render_output.mxi")
                    Return out.ToString
                End If
            Next
            Return ""
        Catch ex As Exception
            'TODO: ...
            Return ""
        End Try
    End Function

    Private Function ComputeCooperativeBenchmark() As String
        Try
            Dim out As Integer = 0
            out += _UniBenchmark
            Dim tBM As String
            For Each rn As cSMT_ATJ_RenderNodeStatus In RenderNodes
                tBM = GetInstanceAttribute(rn.Id, "Benchmark")
                If Not String.IsNullOrEmpty(tBM) AndAlso IsNumeric(tBM) Then
                    out += tBM
                Else
                    'it is "N/A" or hasn't been set yet by render manager on the other instance
                End If
            Next
            Return out
        Catch ex As Exception
            AddConsoleLine("Problem with ComputeCooperativeBenchmark(). Error: " & ex.Message)
            Return "N/A"
        End Try
    End Function

#End Region 'MASTER:COOPERATIVE RENDERING

#Region "MASTER:CREDIT MANAGEMENT"

    Private PreviousSampleLevelTicks As Long = 0
    Private ChargedForJob As Double

    Private Sub UpdateUserCredit(ByVal OnLaunch As Boolean)
        Try
            If OnLaunch Then
                Dim startup_fee As Double = 5 + the_job.CoresRequested / 8 * 2
                ChargedForJob = startup_fee
                ReduceUserBalance(the_job.UserId, startup_fee) 'need to calculate this based on how long since they submitted the project, this charges them for the startup time of the instance if any.
            Else
                Dim recent_usage As Double = CalculateCreditReduction(DateTime.UtcNow.Ticks - PreviousSampleLevelTicks)
                ChargedForJob += recent_usage
                ReduceUserBalance(the_job.UserId, recent_usage)
            End If
            SetJobCharges(the_job.Id, Math.Round(ChargedForJob, 2))
            PreviousSampleLevelTicks = DateTime.UtcNow.Ticks
        Catch ex As Exception
            AddConsoleLine("UpdateUserCredit FAILED. " & ex.Message & " - " & ex.StackTrace)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Function CalculateCreditReduction(ByRef TicksElapsed As Long) As Double
        Dim ts As New TimeSpan(TicksElapsed)
        Dim PER_CORE_PER_HOUR As Byte = 2
        Dim PER_CORE_PER_MINUTE As Double = PER_CORE_PER_HOUR / 60
        Return ts.TotalMinutes * PER_CORE_PER_MINUTE * the_job.CoresRequested
    End Function

#End Region 'MASTER:CREDIT MANAGEMENT"

#End Region 'MASTER

#Region "RENDER ENGINE"

    Private Sub RunMXCL(ByVal ThreadCount As Byte)
        Try
            If the_job_maxwell Is Nothing Then Throw New Exception("Unexpected. the_job_maxwell is Nothing.")

            AddConsoleLine("ENTER RUN MXCL (MASTER)")
            If IsMasterInstance Then SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Preprocessing_FinalJobPrepPreRendering)
            MergeNumber = 0 'reset the merge number

            'FIRST - verify that all job-referenced files exist as expected.
            For Each f As String In the_job.JobFileNames
                If Not File.Exists(ALENTEJO_INSTANCE_JOB_FILE_STORAGE & f) Then Throw New Exception("A job file was not found where expected: " & ALENTEJO_INSTANCE_JOB_FILE_STORAGE & f)
            Next

            'NEXT - confirm that the output directory exists
            If Not Directory.Exists(job_output_dir) Then Directory.CreateDirectory(job_output_dir)

            'NEXT - build the MXCL commandline
            MXCL = New cMXCL
            With MXCL

                'MANDATORY
                .SETTING_MXS_PATH = ALENTEJO_INSTANCE_JOB_FILE_STORAGE & the_job.JobFileNames(0)
                .SETTING_RESOLUTION = New System.Drawing.Size(the_job.Width, the_job.Height)
                .SETTING_OUTPUT_PATH = job_output_image
                .SETTING_SAMPLE_LEVELS = the_job_maxwell.SampleCount_PerNode
                .SETTING_MAX_TIME = the_job_maxwell.MaxDuration
                .SETTING_MXI_PATH = job_output_mxi
#If DEBUG Then
                .SETTING_THREADS = 1
#Else
                .SETTING_THREADS = ThreadCount
#End If

                'OPTIONAL
                .SETTING_BITMAP_PATHS = "" 'this will force it to look in the same dir as mxs.
                .SETTING_MULTILIGHT = the_job_maxwell.CreateMultilight
                .SETTING_RENDER_ENGINE_IS_PREVIEW = the_job_maxwell.UsePreviewEngine
                .SETTING_ANIMATION = the_job_maxwell.AnimationFrames
                .SETTING_CAMERA_NAME = the_job_maxwell.ActiveCamera
                .SETTING_SCATTERING_LENS = the_job_maxwell.ScatteringLens
                .SETTING_VIGNETTING = the_job_maxwell.Vignetting
                .SETTING_CHANNELS_RENDER = the_job_maxwell.Channels_Render
                .SETTING_CHANNELS_ALPHA = the_job_maxwell.Channels_Alpha
                .SETTING_CHANNELS_OPAQUE_ALPHA = the_job_maxwell.Channels_Alpha_Opaque
                .SETTING_CHANNELS_SHADOW = the_job_maxwell.Channels_Shadow
                .SETTING_CHANNELS_MATERIAL = the_job_maxwell.Channels_MaterialId
                .SETTING_CHANNELS_OBJECTID = the_job_maxwell.Channels_ObjectId
                .SETTING_CHANNELS_ZBUFFER = the_job_maxwell.Channels_ZBuffer
                .SETTING_CHANNELS_ZBUFFER_MIN = the_job_maxwell.Channels_ZBuffer_min
                .SETTING_CHANNELS_ZBUFFER_MAX = the_job_maxwell.Channels_ZBuffer_max

                'NETWORK RENDERING
                .SETTING_IS_SERVER = ""
                .SETTING_IS_MANAGER = False

            End With

            'NEXT - run MXCL
            MXCL.Run()

            'FINALLY - set the job status
            If IsMasterInstance Then SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.RENDERING)
            AddConsoleLine("EXIT RUN MXCL (MASTER)")
        Catch ex As Exception
            AddConsoleLine("")
            AddConsoleLine("RUN MXCL (MASTER) FAILED.")
            AddConsoleLine("")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Failure_RunMXCL)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

#Region "RENDER ENGINE:MONITORING"

    Private WithEvents MXCL As cMXCL

    Private Sub HandleMXCLMessage(ByVal msg As String) Handles MXCL.evGeneralLog
        AddConsoleLine("MXCL LOG MESSAGE | " & msg)
    End Sub

    Private Sub HandleMXCLExit() Handles MXCL.evProcessExit
        Try
            AddConsoleLine("*************")
            AddConsoleLine("*MXCL EXITED*")
            AddConsoleLine("*************")

            'how do we know if this is the result of a failure or of completion of rendering?
            'LOOK AT THE OUTPUT. 
            '   If there is supposed to be an MXI and there isn't then it is a failure.
            '   If there is no image output then it is a failure

            AddConsoleLine("HandleMXCLExit()")

            If IsCooperativeRender And Not IsMasterInstance Then
                'we bail out here.
                SetInstanceAttribute(InstanceId, "AssignedToJobId", "")
                SetInstanceStatus(InstanceId, eSMT_ATJ_Instance_Status.NotAvailable_PendingPeerNodeExit)
                StartJobWaitLoop()
                Dim msg As String = "Job completed. Returning to polling. This was a render node so its status will be PendingPeerNodeExit until the master changes it. JobId = " & the_job.Id
                AddConsoleLine(msg)
            Else
                FinalizeJob()
            End If

        Catch ex As Exception
            AddConsoleLine("")
            AddConsoleLine("MasterJobFinalization FAILED.")
            AddConsoleLine("")
            SetJobStatus(the_job.Id, eSMT_ATJ_RenderJob_Status.Failure_RenderJobFinalization)
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), the_job.Id, ex.Message, ex.StackTrace)
        End Try
    End Sub

    Private Sub HandleMXCLProgressUpdate(ByRef LogLine As cMXCLLogLine) Handles MXCL.evProgressUpdate
        Try
            AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name & " - ENTER")
            AddConsoleLine(LogLine.ToString)

            If IsMasterInstance Then
                Dim m As cSMT_ATJ_JobProgressMessage
                If LogLine.Benchmark > 0 Then
                    UpdateUserCredit(False)
                    _UniSampleLevel = LogLine.SampleLevel
                    _UniBenchmark = LogLine.Benchmark
                    If IsCooperativeRender And DoCooperativeRenderMergeForEachLevel Then Exit Sub ' the progress messages for Cooperative renders are sent from PerformMerge()
                    m = New cSMT_ATJ_JobProgressMessage(the_job.Id, LogLine.Elapsed, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.NewSampleLevel)
                    If IsCooperativeRender Then
                        m.SampleLevel = ComputeSampleLevelEstimateForNodes(_UniSampleLevel, the_job.CoresRequested / 8)
                        m.Benchmark = ComputeCooperativeBenchmark()
                    Else
                        m.SampleLevel = LogLine.SampleLevel
                        m.Benchmark = LogLine.Benchmark
                    End If
                Else
                    m = New cSMT_ATJ_JobProgressMessage(the_job.Id, LogLine.GeneralMessage, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.RenderEngineMessage)
                End If
                AddJobProgressMessage(m)
                AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name & " - EXIT")
            Else
                'do only on nodes
            End If

            If LogLine.Benchmark > 0 Then
                UpdateInstanceBenchMark(LogLine.Benchmark)
            End If
        Catch ex As Exception
            AddConsoleLine("Problem with HandleMXCLProgressUpdate(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub UpdateInstanceBenchMark(ByVal NewBenchmark As Double)
        SetInstanceAttribute(InstanceId, "Benchmark", NewBenchmark)
    End Sub

#End Region 'RENDER ENGINE:MONITORING

#Region "RENDER ENGINE:OUTPUT MONITORING"

    Private Sub HandleIntermediateFileAvailable(ByVal ImgPath As String) Handles MXCL.evIntermediateImageAvailable
        Try
            Dim DoingLocalSampleOnly As Boolean = True

            If IsCooperativeRender Then
                If IsMasterInstance Then
                    If DoCooperativeRenderMergeForEachLevel Then
                        PerformMerge()
                        ImgPath = PathToLastCompletedMerge_JPG
                        DoingLocalSampleOnly = False
                    End If
                Else
                    'bail out
                    'these files are not stored in S3.
                    Exit Sub
                End If
            End If
            If My.Settings.TRACE_LEVEL > 2 Then AddConsoleLine("HandleIntermediateFileAvailable() - ENTER")

            Dim ImgG As String = Guid.NewGuid.ToString

            'copy the file to S3
            SendIntermediateImageToS3(ImgPath, ImgG)

            'store in files domain
            Dim fi As New FileInfo(ImgPath)
            SaveAtjFile(New cSMT_ATJ_File(ImgG, "preview_image_" & CurrentSampleLevelFriendlyString(DoingLocalSampleOnly) & the_job.OutputImageFormat, the_job.Id, cSMT_ATJ_File.eAtjFileType.IntermediateOutputImage, fi.Length, the_job.UserId))

            SetFileAttribute(ImgG, "Resolution", the_job.Width & "X" & the_job.Height)

            If My.Settings.TRACE_LEVEL > 2 Then AddConsoleLine("HandleIntermediateFileAvailable() - EXIT")
        Catch ex As Exception
            AddConsoleLine("Problem with HandleIntermediateFileAvailable(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub SendIntermediateImageToS3(ByRef SrcPath As String, ByVal g As String)
        Try
            Dim TM As New cSMT_AWS_S3_TransferManager(S3)
            TM.Name = "IntermediateFileUpload"
            AddHandler TM.evTransferStatus, AddressOf HandleTransferStatus_TransferThread
            If My.Settings.TRACE_LEVEL > 2 Then AddConsoleLine("Calling TM.UploadFile")
            If TM.UploadFile(SrcPath, cSMT_AWS_S3_FileTransfer.eBucketLocation.US, s3_bucket_files_us, g) Is Nothing Then
                Throw New Exception("Failed to start upload. SrcPath=" & SrcPath)
            End If
            If My.Settings.TRACE_LEVEL > 2 Then AddConsoleLine("TM.UploadFile returned ok.")
        Catch ex As Exception
            AddConsoleLine("Problem with SendIntermediateImageToS3(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'RENDER ENGINE:OUTPUT MONITORING

#End Region 'RENDER ENGINE

#Region "CONSOLE"

    Private ReadOnly Property atjTRACE() As cSMT_ATJ_TRACE
        Get
            If _atjTRACE Is Nothing Then _atjTRACE = New cSMT_ATJ_TRACE("Render Manager")
            Return _atjTRACE
        End Get
    End Property
    Private _atjTRACE As cSMT_ATJ_TRACE

    Private Sub AddConsoleLine(ByVal msg As String)
#If DEBUG Then
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _AppendConsole_Delegate Is Nothing Then _AppendConsole_Delegate = New AppendConsole_Delegate(AddressOf int_AppendConsole)
            Me.Invoke(_AppendConsole_Delegate, New Object() {msg})
        Else
            int_AppendConsole(msg)
        End If
#Else
        atjTRACE.LogMessage(msg, EventLogEntryType.Information)
#End If
    End Sub

    Private Sub int_AppendConsole(ByVal Msg As String)
        If rtbQueuePollingConsole.Lines.Length > 1000 Then
            Dim tStr(499) As String
            Array.Copy(rtbQueuePollingConsole.Lines, 500, tStr, 0, 500)
            rtbQueuePollingConsole.Lines = tStr
            Me.rtbQueuePollingConsole.AppendText(vbNewLine)
        End If
        Me.rtbQueuePollingConsole.AppendText(Msg & vbNewLine)
        Me.rtbQueuePollingConsole.ScrollToCaret()
    End Sub
    Private Delegate Sub AppendConsole_Delegate(ByVal Msg As String)
    Private _AppendConsole_Delegate As AppendConsole_Delegate

    Private Sub ClearConsole()
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _ClearConsole_Delegate Is Nothing Then _ClearConsole_Delegate = New ClearConsole_Delegate(AddressOf int_ClearConsole)
            Me.Invoke(_ClearConsole_Delegate)
        Else
            int_ClearConsole()
        End If
    End Sub

    Private Sub int_ClearConsole()
        rtbQueuePollingConsole.Clear()
    End Sub
    Private Delegate Sub ClearConsole_Delegate()
    Private _ClearConsole_Delegate As ClearConsole_Delegate

    Private Sub rtbQueuePollingConsole_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtbQueuePollingConsole.DoubleClick
        Me.rtbQueuePollingConsole.Clear()
    End Sub

#End Region 'CONSOLE

#Region "HELPER"

    Private Sub CleanupDirectories()
        DeleteDirectoryContents(ALENTEJO_RENDER_ENGINE_OUTPUT)
        DeleteDirectoryContents(ALENTEJO_INSTANCE_JOB_FILE_STORAGE)
    End Sub

    Private Sub DeleteDirectoryContents(ByVal Dir As String)
        Try
            Dim di As New DirectoryInfo(Dir)
            For Each fso As FileSystemInfo In di.GetFileSystemInfos
                If fso.Extension = "" Then DeleteDirectoryContents(fso.FullName) 'recurse
                fso.Delete()
            Next
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            AddConsoleLine("Problem in DeleteDirectoryContents(). Error: " & ex.Message)

            'we take this very seriously.
            'it is probably due to a previous render not completing successfully so there's still a file 
            'handle open to one of the output files. Restarting the app should kill this handle.
            'because this cleanup process takes place before any job attributes are modified the new 
            'rm will pickup and run with the job successfully after the restart, theoretically.
            Application.Exit()
        End Try
    End Sub

    Private Class cSMT_ATJ_RenderNodeStatus
        Public Id As String
        Public Complete As Boolean
        Public LocalIp As String
        Public MxiPath As String
        Public Sub New(ByVal nId As String)
            Id = nId
            Complete = False
        End Sub
    End Class

#End Region 'HELPER

End Class
