Imports SMT.Alentejo.Core.AWS.S3
Imports SMT.Alentejo.Core.Consts
Imports SMT.Alentejo.Core.InstanceManagement
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.AWS.S3
Imports SMT.AWS.Authorization
Imports System.IO
Imports System.Threading
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.AtjTrace
Imports SMT.Alentejo.Core.UserManagement

Public Class JobFileTransferManager_Main

#Region "FIELDS & PROPERTIES"

    Private ReadOnly Property S3() As cSMT_AWS_S3
        Get
            If _S3 Is Nothing Then _S3 = New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
            Return _S3
        End Get
    End Property
    Private _S3 As cSMT_AWS_S3
    Private TransferJobs As List(Of cTransferJob)
    Private WithEvents ONE_HOUR_TIMER As New System.Timers.Timer(3600000)
    Private WithEvents QUEUE_POLLING_TIMER As New System.Timers.Timer(1000 * 5)

#End Region 'FIELDS & PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        _AddTransferJobDelegate = New AddTransferJobDelegate(AddressOf AddTransferJob)
        _HandleTransferStatusEvent_UIThread_Delegate = New HandleTransferStatusEvent_UIThread_Delegate(AddressOf HandleTransferStatusEvent_UIThread)
    End Sub

#End Region 'CONSTRUCTOR

#Region "FORM"

    Private Sub JobFileTransferManager_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        QUEUE_POLLING_TIMER.Start()
        ONE_HOUR_TIMER.Start()
        CleanupTempDir() ' Do it on startup of this app because it will be an hour before it happens again.
    End Sub

#End Region 'FORM

#Region "QUEUE POLLING"

    Private Sub QUEUE_POLLING_TIMER_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QUEUE_POLLING_TIMER.Elapsed
        AddConsoleLine("POLL" & TS)
        PollQueue()
    End Sub

    Private Sub PollQueue()
        'Me.QUEUE_POLLING_TIMER.Stop()
        Try
            Dim T As Thread
            'Dim m As cJobFileTransferMessage_ToS3
            Dim m As String

            m = Pop_JobNeedingFileTransferToS3()
            While m IsNot Nothing
                AddConsoleLine("************")
                AddConsoleLine("JOB RECEIVED" & TS)
                AddConsoleLine("JobId = " & m)
                'If ValidateFileTransferMessage(m) Then
                '    T = New Thread(AddressOf ProcessFileTransferJobMessage)
                '    T.Start(m)
                '    'AddConsoleLine("NEW JOB: " & m.JobId & " - " & m.TargetBucket & " - " & m.LocalFilePaths(0))
                'Else
                '    AddConsoleLine("INVALID MESSAGE RECEIVED")
                'End If
                T = New Thread(AddressOf ProcessFileTransferJobMessage)
                T.Start(m)
                m = Pop_JobNeedingFileTransferToS3()
                AddConsoleLine("************")
            End While

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), "", ex.Message, ex.StackTrace)
            AddConsoleLine("Problem in PollQueue(). Error: " & ex.Message)
        Finally
            'Me.QUEUE_POLLING_TIMER.Start()
        End Try
    End Sub

    ''' <summary>
    ''' NOTE: This method is run on a worker thread.
    ''' </summary>
    ''' <param name="JobId"></param>
    ''' <remarks></remarks>
    Private Sub ProcessFileTransferJobMessage(ByVal JobId As String)
        Dim tj As cTransferJob
        Dim dir As String
        Dim files() As String
        Dim j As cSMT_ATJ_RenderJob_Maxwell
        Try
            ' VALIDATE THE MESSAGE
            'If Not ValidateFileTransferMessage(m) Then Throw New Exception("Job message is invalid.")

            ' SET JOB STATUS
            SetJobStatus(JobId, eSMT_ATJ_RenderJob_Status.Preprocessing_TransferringJobFilesToS3)

            ' LOOKUP USERNAME AND JOBNAME
            j = GetJob(JobId)
            If j Is Nothing Then Throw New Exception("Failed to get job (" & JobId & ").")
            If String.IsNullOrEmpty(j.UserId) Then Throw New Exception("UserName is invalid.")
            If String.IsNullOrEmpty(j.Name) Then Throw New Exception("JobName is invalid.")

            AddConsoleLine("Retrieved job from storage. Job name = " & j.Name)

            ' VALIDATE SOURCE DIRECTORY
            dir = ALENTEJO_SERVER_JOB_FILE_STORAGE & j.UserId & "\" & j.Id & "\"
            AddConsoleLine(dir)
            If Not Directory.Exists(dir) Then Throw New Exception("Source directory does not exist.")
            files = Directory.GetFiles(dir)
            If files Is Nothing OrElse files.Count < 1 Then Throw New Exception("Source directory contains no files.")

            ' CREATE THE TRANSFER JOB
            tj = New cTransferJob(JobId, j.UserId, j.Name, files, s3_bucket_files_us, True)

            ' ADD TO TRANSFER LIST
            _AddTransferJobDelegate.Invoke(tj)

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), JobId, ex.Message, ex.StackTrace)
            AddConsoleLine("WARNING: Problem with ProcessFileTransferJobMessage(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'QUEUE POLLING

#Region "TRANSFER JOBS"

    Private Sub AddTransferJob(ByVal TJ As cTransferJob)
        Try
            If TransferJobs Is Nothing Then TransferJobs = New List(Of cTransferJob)

            Dim g As String

            ' ADD THE JOB TO THE LIST
            TransferJobs.Add(TJ)

            Dim d As New Dictionary(Of String, String)
            Dim fi As FileInfo

            Dim TM As New cSMT_AWS_S3_TransferManager(S3)
            TM.Name = TJ.JobId
            AddHandler TM.evTransferStatus, AddressOf HandleTransferSucceeded_TransferThread

            For b As Byte = 0 To UBound(TJ.Files.Files)
                If Not File.Exists(TJ.Files.Files(b)) Then
                    SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), TJ.JobId, "The job file to be copied to S3 does not exist at the specified path (" & TJ.Files.Files(b) & ").", "")
                End If

                ' GENERATE GUID FOR FILE
                g = Guid.NewGuid.ToString

                ' STORE GUID IN LOCAL TRANSFER JOB FOR LOOKUP ON SUCCESS/FAILURE OF TRANSFER
                TJ.Files.Guids(b) = g

                ' STORE GUID FOR PASSING TO SDB
                d.Add(Path.GetFileName(TJ.Files.Files(b)), g)

                Dim uid As String = GetUserIdByUsername(TJ.UserName)

                fi = New FileInfo(TJ.Files.Files(b))
                Select Case Path.GetExtension(TJ.Files.Files(b)).ToLower
                    Case ".mxs" 'add later supported model file formats here
                        SaveAtjFile(New cSMT_ATJ_File(g, fi.Name, TJ.JobId, cSMT_ATJ_File.eAtjFileType.Model, fi.Length, uid))
                    Case Else
                        SaveAtjFile(New cSMT_ATJ_File(g, fi.Name, TJ.JobId, cSMT_ATJ_File.eAtjFileType.Texture, fi.Length, uid))
                End Select

                AddConsoleLine("Starting upload = " & TJ.Files.Files(b))

                ' KICK OFF THE ACTUAL TRANSFER
                TM.UploadFile(TJ.Files.Files(b), cSMT_AWS_S3_FileTransfer.eBucketLocation.US, TJ.TargetBucket, g)
            Next

            ' SAVE GUIDS TO SDB
            SetS3ObjectKeyGuidsToJobFileNames(TJ.JobId, d)

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), TJ.JobId, ex.Message, ex.StackTrace)
        End Try
    End Sub
    Private _AddTransferJobDelegate As AddTransferJobDelegate
    Private Delegate Sub AddTransferJobDelegate(ByVal TJ As cTransferJob)

#End Region 'TRANSFER JOBS

#Region "TRANSFER EVENTS"

    Private Sub HandleTransferSucceeded_TransferThread(ByVal sender As Object, ByVal e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
        Me.Invoke(_HandleTransferStatusEvent_UIThread_Delegate, New Object() {sender, e})
    End Sub

    Private Sub HandleTransferStatusEvent_UIThread(ByRef sender As cSMT_AWS_S3_TransferManager, ByRef e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
        Try
            Select Case e.EventType
                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Success
                    AddConsoleLine(sender.Name & " - " & e.ToString)

                    ' Set file upload status in cTransferJob
                    ' and find the job
                    Dim tj As cTransferJob = Nothing
                    For Each j As cTransferJob In TransferJobs
                        For b As Byte = 0 To UBound(j.Files.Files)
                            If j.Files.Guids(b) = e.ObjectKey Then
                                tj = j
                                j.Files.Status(b) = "True"
                                File.Delete(j.Files.Files(b)) 'delete local file
                                SetFileAvailable(j.Files.Guids(b))
                            End If
                        Next
                    Next

                    If tj Is Nothing Then Throw New Exception("Unexpected.")

                    If tj.IsComplete Then

                        TransferJobs.Remove(tj)
                        Directory.Delete(Path.GetDirectoryName(tj.Files.Files(0)), True)

                        If tj.IsSuccessful Then
                            'all files transferred to S3 successfully
                            AddConsoleLine("Transfers complete for " & tj.JobName.ToUpper)
                            SetJobStatus(tj.JobId, eSMT_ATJ_RenderJob_Status.Preprocessing_WaitingForInstanceStartLogic)
                            Enqueue_JobNeedingInstance(tj.JobId)
                            AddConsoleLine("Job submitted for Instance Dispatching = " & tj.JobName.ToUpper)
                        Else
                            'there was a failure of one or more files
                            AddConsoleLine("")
                            AddConsoleLine("TRANSFERS FAILED FOR JOB " & tj.JobName.ToUpper)
                            AddConsoleLine("")
                            SetJobStatus(tj.JobId, eSMT_ATJ_RenderJob_Status.Failure_FileTransferToS3)
                            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), tj.JobId, e.Message, "")
                            'TODO: send email to user notifying them of failure
                            'use msg that was passed in
                        End If
                    Else
                        'wait for the other uploads to finish.
                    End If

                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Progress
                    AddConsoleLine(sender.Name & " - " & e.FileName & " " & e.BytesRemaining & "B" & " " & e.ElapsedSeconds & "s")

                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Failure
                    AddConsoleLine(sender.Name & " - " & e.ToString)

            End Select

        Catch ex As Exception
            AddConsoleLine("Problem in HandleTransferSucceedFailEvent_UIThread(). Error: " & ex.Message)
        End Try
    End Sub
    Private Delegate Sub HandleTransferStatusEvent_UIThread_Delegate(ByRef sender As cSMT_AWS_S3_TransferManager, ByRef e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
    Private _HandleTransferStatusEvent_UIThread_Delegate As HandleTransferStatusEvent_UIThread_Delegate

#End Region 'TRANSFER EVENTS

#Region "CLEANUP TEMP DIRECTORY"

    Private Sub ONE_HOUR_TIMER_Tick(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles ONE_HOUR_TIMER.Elapsed
        CleanupTempDir()
    End Sub

    Private Sub CleanupTempDir()
        Try
            Dim storage As New DirectoryInfo(ALENTEJO_SERVER_JOB_FILE_STORAGE)
            Dim msg As String = ""
            For Each d As DirectoryInfo In storage.GetDirectories
                If DateTime.Now.Subtract(d.CreationTime).TotalHours > atj_local_job_file_storage_persistence Then
                    msg = "Deleting orphened job files: " & d.FullName
                    AddConsoleLine(msg)
                    SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", msg, d.FullName)
                    d.Delete(True)
                End If
            Next
        Catch ex As Exception
            AddConsoleLine("Problem in CleanupTempDir(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'CLEANUP TEMP DIRECTORY

#Region "CONSOLE"

    Private ReadOnly Property atjTRACE() As cSMT_ATJ_TRACE
        Get
            If _atjTRACE Is Nothing Then _atjTRACE = New cSMT_ATJ_TRACE("Job File Transfer Manager")
            Return _atjTRACE
        End Get
    End Property
    Private _atjTRACE As cSMT_ATJ_TRACE

    Private Sub AddConsoleLine(ByVal msg As String)
#If TRACE Then
        atjTRACE.LogMessage(msg, EventLogEntryType.Information)
#Else
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _AppendConsole_Delegate Is Nothing Then _AppendConsole_Delegate = New AppendConsole_Delegate(AddressOf int_AppendConsole)
            Me.Invoke(_AppendConsole_Delegate, New Object() {msg})
        Else
            int_AppendConsole(msg)
        End If
#End If
    End Sub

    Private Sub int_AppendConsole(ByVal Msg As String)
        If rtbQueuePollingConsole.Lines.Count > 1000 Then
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

End Class
