Imports SMT.Alentejo.Core.InstanceManagement

''' <summary>
''' There are two distinct activities in this form:
'''    1) Monitoring the jobs-needing-instances queue and running the start-instance-logic
'''    2) Monitoring the instances-domain and providing information about the active instances.
''' </summary>
''' <remarks></remarks>
Public Class InstanceMonitor_Main

#Region "FIELDS & PROPERTIES"

#End Region 'FIELDS & PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

#End Region 'CONSTRUCTOR

#Region "INSTANCE MONITORING"

    Private Sub INSTANCE_MONITOR_TIMER_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles INSTANCE_MONITOR_TIMER.Tick
        UpdateInstanceList()
        UpdateGauges()
    End Sub

    Private Sub UpdateInstanceList()
        Try
            Me.lvInstances.BeginUpdate()

            lvInstances.Groups(0).Items.Clear()
            lvInstances.Groups(1).Items.Clear()
            lvInstances.Groups(2).Items.Clear()

            For Each I As cSMT_ATJ_Instance In GetInstances(eSMT_ATJ_Instance_Status.NotAvailable_Terminated)

                Select Case CType(I.Status, eSMT_ATJ_Instance_Status)
                    Case eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob
                        'TODO: need logic for grouping instances with the same jobId. It will be easier to look at.
                        lvInstances.Groups(0).Items.Add(GetLVIForInstance(I))
                    Case eSMT_ATJ_Instance_Status.Available
                        lvInstances.Groups(1).Items.Add(GetLVIForInstance(I))
                    Case Else
                        lvInstances.Groups(2).Items.Add(GetLVIForInstance(I))
                End Select
            Next

            Me.lvInstances.EndUpdate()
        Catch ex As Exception
            AddConsoleLine("Problem with UpdateInstanceList(). Error: " & ex.Message)
        End Try
    End Sub

    Private Function GetLVIForInstance(ByRef I As cSMT_ATJ_Instance) As ListViewItem
        Try
            Dim out As New ListViewItem
            out.Tag = I

            out.Text = I.Id
            out.SubItems.Add(I.AmiId)
            out.SubItems.Add(I.InstanceType)
            out.SubItems.Add(I.LaunchTime)
            out.SubItems.Add(I.ScheduledTerminationTime)
            out.SubItems.Add(I.MinutesActive)
            out.SubItems.Add(I.MinutesUntilTermination)
            out.SubItems.Add(I.AssignedToJobId)

            'HERE LOOK UP THE JOB
            out.SubItems.Add("job_name")
            out.SubItems.Add("job_uid")
            out.SubItems.Add("job_username")

            out.SubItems.Add(I.PublicHostname)

            Return out
        Catch ex As Exception
            AddConsoleLine("Problem with GetLVIForInstance(). Error: " & ex.Message)
            Return Nothing
        End Try
    End Function

#Region "LISTVIEW:CONTEXT MENU"

    Private Temp_LVI As ListViewItem

    Private Sub lvTransfers_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs)
        Dim LV As ListView = TryCast(sender, ListView)
        If e.Button = MouseButtons.Right AndAlso ModifierKeys = Keys.None Then
            Temp_LVI = LV.GetItemAt(e.X, e.Y)
            If Temp_LVI Is Nothing Then Exit Sub
            Me.cmMain.Show(LV, e.X, e.Y)
        End If
    End Sub

    Private Sub miTerminateInstance(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmTerminateInstance.Click
        Try
            AddConsoleLine("Terminate instance is not implemented.")
            'use Temp_LVI to find the instance clicked on
        Catch ex As Exception
            AddConsoleLine("Problem with miTerminateInstance(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'LISTVIEW:CONTEXT MENU

#Region "GAUGES"

    Private Sub UpdateGauges()
        Try
            'If TM Is Nothing Then Exit Sub
            'Me.lblOverallTransferSpeed.Text = TM.OverallBitrate_Up.ToString
        Catch ex As Exception
            AddConsoleLine("Problem with UpdateOverallBitrates(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'GAUGES

#End Region 'INSTANCE MONITORING

#Region "CONSOLE"

    Private Sub AddConsoleLine(ByVal msg As String)
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _ConsoleRtbAppend_Delegate Is Nothing Then _ConsoleRtbAppend_Delegate = New ConsoleRtbAppend_Delegate(AddressOf ConsoleRtbAppend)
            Me.Invoke(_ConsoleRtbAppend_Delegate, New Object() {msg})
        Else
            ConsoleRtbAppend(msg)
        End If
    End Sub

    Private Sub ConsoleRtbAppend(ByVal Msg As String)
        Me.rtbQueuePollingConsole.AppendText(Msg & vbNewLine)
        Me.rtbQueuePollingConsole.ScrollToCaret()
    End Sub
    Private Delegate Sub ConsoleRtbAppend_Delegate(ByVal Msg As String)
    Private _ConsoleRtbAppend_Delegate As ConsoleRtbAppend_Delegate

#End Region 'CONSOLE

End Class
