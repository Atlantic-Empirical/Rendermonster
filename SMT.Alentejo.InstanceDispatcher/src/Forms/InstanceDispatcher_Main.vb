Imports SMT.Alentejo.Core.Consts
Imports SMT.Alentejo.Core.InstanceManagement
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports System.Threading
Imports SMT.Alentejo.Core.AtjTrace

Public Class InstanceDispatcher_Main

#Region "FIELDS & PROPERTIES"

    Private WithEvents INSTANCE_KILL_TIMER As New System.Timers.Timer(1000 * 60 * 2)
    Private WithEvents QUEUE_POLLING_TIMER As New System.Timers.Timer(1000 * 5)

#End Region 'FIELDS & PROPERTIES

#Region "FORM EVENTS"

    Private Sub InstanceDispatcher_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        AddConsoleLine("INITIALIZED" & TS)
        QUEUE_POLLING_TIMER.Start()
        INSTANCE_KILL_TIMER.Start()
        InstanceTerminationLogic() 'do on startup
    End Sub

#End Region 'FORM EVENTS

#Region "QUEUE POLLING"

    Private Sub QUEUE_POLLING_TIMER_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QUEUE_POLLING_TIMER.Elapsed
        AddConsoleLine("POLL" & TS)
        PollQueue()
    End Sub

    Private Sub PollQueue()
        Try
            Dim T As Thread
            Dim m As String

            m = Pop_JobNeedingInstance()
            While m IsNot Nothing
                T = New Thread(AddressOf ProcessJobNeedsInstanceMessage)
                T.Start(m)
                m = Pop_JobNeedingInstance()
            End While

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), "", ex.Message, ex.StackTrace)
            AddConsoleLine("Problem in PollQueue(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'QUEUE POLLING

#Region "INSTANCE SPIN-UP"

    ''' <summary>
    ''' NOTE: This method is run on a worker thread.
    ''' </summary>
    ''' <param name="JobId"></param>
    ''' <remarks></remarks>
    Private Sub ProcessJobNeedsInstanceMessage(ByVal JobId As String)
        Dim j As cSMT_ATJ_RenderJob_Maxwell
        Try
            AddConsoleLine("************")
            AddConsoleLine("JOB RECEIVED" & TS)
            AddConsoleLine("JobId = " & JobId)

            ' SET JOB STATUS
            SetJobStatus(JobId, eSMT_ATJ_RenderJob_Status.Preprocessing_InInstanceStartLogic)

            ' LOOKUP JOB
            j = GetJob(JobId)
            If j Is Nothing Then Throw New Exception("Failed to get job (" & JobId & ").")

            ' INSTANCE START LOGIC
            InstanceStartLogic(j)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), JobId, ex.Message, ex.StackTrace)
            AddConsoleLine("WARNING: Problem with ProcessFileTransferJobMessage(). Error: " & ex.Message)
        Finally
            AddConsoleLine("************")
        End Try
    End Sub

    Private Sub InstanceStartLogic(ByRef j As cSMT_ATJ_RenderJob_Maxwell)
        Try
            AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name & " - ENTER")

            Dim RequestedInstanceCount As Byte = j.CoresRequested / 8
            AddConsoleLine(RequestedInstanceCount & " instances requested.")
            Dim InstancesStillNeededCount As Byte = RequestedInstanceCount
            Dim PartialAssignment As Boolean = False
            Dim instanceIds As New List(Of String)

            If RenderInstancesAvailable > 0 AndAlso Not LOCAL_RENDERING Then
                'there are available instances,  assign them to this job
                AddConsoleLine("Assigning existing instance(s).")
                InstancesStillNeededCount = AssignInstancesToJob(j.Id, InstancesStillNeededCount, True, instanceIds)
                For Each i As String In instanceIds
                    AddConsoleLine("Instance assigned: " & i)
                Next
                PartialAssignment = InstancesStillNeededCount > 0
                If PartialAssignment Then
                    AddConsoleLine(InstancesStillNeededCount & " still needed, start them up.")
                End If
            End If

            If InstancesStillNeededCount > 0 Then
                AddConsoleLine("Starting " & InstancesStillNeededCount & " new instance(s).")
                'need add'tl instances
                'if PartialAssignment is set then we're only starting render nodes here. don't set the first to master
                'or we'll end up with two.
                InstancesStillNeededCount = RunRenderInstances(j.Id, InstancesStillNeededCount, Not PartialAssignment, instanceIds)
                If InstancesStillNeededCount > 0 Then
                    SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, j.Id, "Failed to launch the required number of instances. Requested=" & j.CoresRequested / 8 & " StillNeeded=" & InstancesStillNeededCount, "")
                    SetJobAttribute(j.Id, "CoresUnavailable", InstancesStillNeededCount * 8)
                Else
                    For Each i As String In instanceIds
                        AddConsoleLine("Instance started: " & i)
                    Next
                End If

            End If

            SetJobStatus(j.Id, eSMT_ATJ_RenderJob_Status.Preprocessing_WaitingForMasterRenderInstanceToComeOnline)
            AddConsoleLine(RequestedInstanceCount - InstancesStillNeededCount & " of " & RequestedInstanceCount & " instance(s) dedicated to job " & j.Id)
            AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name & " - EXIT")
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), j.Id, ex.Message, ex.StackTrace)
            AddConsoleLine("WARNING: Problem with InstanceStartLogic(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'INSTANCE SPIN-UP

#Region "INSTANCE KILL MONITOR"

    Private Sub INSTANCE_KILL_TIMER_Tick(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles INSTANCE_KILL_TIMER.Elapsed
        InstanceTerminationLogic()
    End Sub

    ''' <summary>
    ''' This method is all about making Alentejo robust and self-healing. Spend as much time as needed
    ''' to get the logic and implementation correct.
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub InstanceTerminationLogic()
        Me.INSTANCE_KILL_TIMER.Stop()
        Try

            AddConsoleLine("")
            AddConsoleLine("ENTER INSTANCE TERMINATION LOGIC")

            Dim RunningInstancesInStorage As New List(Of String)
            Dim sb As System.Text.StringBuilder


            ' GO THROUGH EACH INSTANCE IN STORAGE AND CONFIRM THAT THE ASSOCIATED EC2 INSTANCE IS IN THE CORRECT STATE

            Dim SDBInstances As List(Of cSMT_ATJ_Instance) = GetInstances(eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob)
            Dim Status As eSMT_ATJ_Instance_Status
            Dim Running As Boolean
            Dim Timedout As Boolean
            Dim Healthy As Boolean
            For Each i As cSMT_ATJ_Instance In SDBInstances
                If i.Id = "i-test01234" Then GoTo NextInstance

                sb = New System.Text.StringBuilder
                sb.Append(i.Id & " ")

                Healthy = IsInstanceHealthy(i.Id)
                Timedout = InstanceIsDueForTermination(i.Id)
                Running = InstanceIsRunning(i.Id)
                Status = [Enum].Parse(GetType(eSMT_ATJ_Instance_Status), i.Status)

                sb.Append("is " & Status.ToString & " ")

                Select Case status
                    Case eSMT_ATJ_Instance_Status.Available, eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob, eSMT_ATJ_Instance_Status.NotAvailable_PendingPeerNodeExit
                        'running, healthy, not timed out
                        If Not Running Then
                            sb.Append("is not running ")

                            'this instance is terminated, shutting down, or was not found
                            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", "Leaked instance " & i.Id, "")
                            SetInstanceStatus(i.Id, eSMT_ATJ_Instance_Status.NotAvailable_Terminated)

                            'it may have crashed, been mistakenly terminated, terminated by AWS, or struck by lightning
                            'this is a good opportunity to make Alentejo self-healing

                            'are there any jobs depending on it?
                            Dim j As cSMT_ATJ_RenderJob_Maxwell = GetJobByAttribute("MasterInstanceId", "")
                            If j Is Nothing Then
                                sb.Append("and no jobs were depending on it.")
                                AddConsoleLine(sb.ToString)
                                Exit Sub 'we're good
                            Else
                                sb.Append("and jobs WERE depending on it ")

                                If j.Status < eSMT_ATJ_RenderJob_Status.COMPLETED Then
                                    'AH HA! We have a problem. This instance WAS needed to complete this job.
                                    'put this job back in the jobs-needing-instances queue
                                    sb.Append("the job was not completed so its going back in the jobs-needing-instances queue.")
                                    Enqueue_JobNeedingInstance(j.Id)
                                    SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, j.Id, "LOST INSTANCE: Was needed by job " & j.Id & ". Job has been returned to the jobs-needing-instances queue.", "")
                                    SetJobStatus(j.Id, eSMT_ATJ_RenderJob_Status.Preprocessing_WaitingForInstanceStartLogic)
                                Else
                                    sb.Append("but the job completed successfully.")
                                End If

                            End If

                        Else
                            sb.Append("is running ")
                            RunningInstancesInStorage.Add(i.Id)
                        End If
                        If Healthy Then
                            sb.Append("is healthy ")
                        Else
                            sb.Append("is not healthy ")
                            'TODO: develop this logic
                            'do we kill it?
                            'are we surely sure it is dead?
                            'if so we need to know if a job is depending on it and put that job 
                            'back in the jobs-needing-instances queue.
                            'and terminate this sick instance.
                        End If
                        If Timedout Then
                            sb.Append("and is due for termination ")
                            If Status = eSMT_ATJ_Instance_Status.Available Then
                                sb.Append("and is not being used actively.")
                                AddConsoleLine("TERMINATING INSTANCE: " & i.Id)
                                TerminateRenderInstance(i.Id)
                                SetInstanceStatus(i.Id, eSMT_ATJ_Instance_Status.NotAvailable_ShuttingDown)
                                SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", "INSTANCE TIMED OUT: " & i.Id & ".", "")
                            Else
                                sb.Append("and is being used actively (will not be terminated).")
                            End If
                        Else
                            sb.Append("and is not due for termination.")
                        End If
                    Case eSMT_ATJ_Instance_Status.NotAvailable_ShuttingDown
                        If Not Running Then
                            sb.Append("is not running.")
                            SetInstanceStatus(i.Id, eSMT_ATJ_Instance_Status.NotAvailable_Terminated)
                        Else
                            sb.Append("is running.")
                        End If
                    Case eSMT_ATJ_Instance_Status.NotAvailable_Terminated
                        'not running
                        If Running Then
                            sb.Append("is running.")
                            TerminateRenderInstance(i.Id)
                            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", "TRYING AGAIN TO TERMINATE INSTANCE: " & i.Id & ".", "")
                        Else
                            sb.Append("is not running.")
                        End If
                    Case eSMT_ATJ_Instance_Status.NOT_INDICATED
                        '?
                    Case eSMT_ATJ_Instance_Status.NotAvailable_DoesNotExist
                        '?
                End Select
                AddConsoleLine(sb.ToString)
NextInstance:
            Next

            ' Now look at all instances in the Alentejo group and find any that are no longer referenced 
            ' in the instances-domain but are still running. This is a differnt kind of leak!
            ' Kill such instances without prejudice.

            AddConsoleLine("Starting verification of all instances in Alentejo group.")

            Dim RunningEC2Instances As List(Of String) = GetGroupRunningInstances()
            For Each gi As String In RunningEC2Instances

                If RunningInstancesInStorage.IndexOf(gi) = -1 Then

                    'The instance was leaked out of the Instances-domain!
                    'could have been accidentially deleted from the domain
                    'could be actively working on a render so treat it gently!

                    sb = New System.Text.StringBuilder
                    sb.Append(gi & " is running but is not in instances-domain (leaked) ")

                    If IsInstanceHealthy(gi) And Not InstanceIsDueForTermination(gi) Then
                        sb.Append(gi & " it is healthy and is not due for termination. Putting it back in the quiver (instances-domain).")
                        SaveRunningInstanceToDomain(gi)
                    Else
                        sb.Append(gi & " is either unhealthy or is due to expire. Terminating it.")
                        AddConsoleLine("TERMINATING INSTANCE: " & gi)
                        TerminateRenderInstance(gi)
                        SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", "LEAKED INSTANCE: " & gi & ". Instance terminated.", "")
                    End If

                    AddConsoleLine(sb.ToString)
                End If
            Next

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod(), "", ex.Message, ex.StackTrace)
            AddConsoleLine("Problem in InstanceTerminationLogic(). Error: " & ex.Message)
        Finally
            Me.INSTANCE_KILL_TIMER.Start()
            AddConsoleLine("EXIT INSTANCE TERMINATION LOGIC")
            AddConsoleLine("")
        End Try
    End Sub

#End Region 'INSTANCE KILL MONITOR

#Region "CONSOLE"

    Private ReadOnly Property atjTRACE() As cSMT_ATJ_TRACE
        Get
            If _atjTRACE Is Nothing Then _atjTRACE = New cSMT_ATJ_TRACE("Instance Dispatcher")
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

End Class
