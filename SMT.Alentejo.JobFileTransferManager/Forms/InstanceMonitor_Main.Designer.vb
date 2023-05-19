<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class InstanceMonitor_Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container
        Dim ListViewGroup1 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("Assigned", System.Windows.Forms.HorizontalAlignment.Left)
        Dim ListViewGroup2 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("Unassigned", System.Windows.Forms.HorizontalAlignment.Left)
        Dim ListViewGroup3 As System.Windows.Forms.ListViewGroup = New System.Windows.Forms.ListViewGroup("Terminated", System.Windows.Forms.HorizontalAlignment.Left)
        Me.INSTANCE_MONITOR_TIMER = New System.Windows.Forms.Timer(Me.components)
        Me.cmMain = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.cmTerminateInstance = New System.Windows.Forms.ToolStripMenuItem
        Me.cmAT_Pause = New System.Windows.Forms.ToolStripMenuItem
        Me.rtbQueuePollingConsole = New System.Windows.Forms.RichTextBox
        Me.Label1 = New System.Windows.Forms.Label
        Me.lblOverallTransferSpeed = New System.Windows.Forms.Label
        Me.lvInstances = New System.Windows.Forms.ListView
        Me.chInstanceId = New System.Windows.Forms.ColumnHeader
        Me.chAMIId = New System.Windows.Forms.ColumnHeader
        Me.chInstanceType = New System.Windows.Forms.ColumnHeader
        Me.chInstanceStartTime = New System.Windows.Forms.ColumnHeader
        Me.chInstanceScheduledTerminationTime = New System.Windows.Forms.ColumnHeader
        Me.chInstanceHasBeenRunningForMinutes = New System.Windows.Forms.ColumnHeader
        Me.chInstanceScheduledForTerminationIn = New System.Windows.Forms.ColumnHeader
        Me.chJobId = New System.Windows.Forms.ColumnHeader
        Me.chJobName = New System.Windows.Forms.ColumnHeader
        Me.chJobUserId = New System.Windows.Forms.ColumnHeader
        Me.chJobUsername = New System.Windows.Forms.ColumnHeader
        Me.chPublicDns = New System.Windows.Forms.ColumnHeader
        Me.cmMain.SuspendLayout()
        Me.SuspendLayout()
        '
        'INSTANCE_MONITOR_TIMER
        '
        Me.INSTANCE_MONITOR_TIMER.Enabled = True
        Me.INSTANCE_MONITOR_TIMER.Interval = 10000
        '
        'cmMain
        '
        Me.cmMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmTerminateInstance, Me.cmAT_Pause})
        Me.cmMain.Name = "cmActiveTransfer"
        Me.cmMain.Size = New System.Drawing.Size(176, 48)
        '
        'cmTerminateInstance
        '
        Me.cmTerminateInstance.Name = "cmTerminateInstance"
        Me.cmTerminateInstance.Size = New System.Drawing.Size(175, 22)
        Me.cmTerminateInstance.Text = "Terminate Instance"
        '
        'cmAT_Pause
        '
        Me.cmAT_Pause.Enabled = False
        Me.cmAT_Pause.Name = "cmAT_Pause"
        Me.cmAT_Pause.Size = New System.Drawing.Size(175, 22)
        Me.cmAT_Pause.Text = "Pause Transfer"
        '
        'rtbQueuePollingConsole
        '
        Me.rtbQueuePollingConsole.BackColor = System.Drawing.Color.Black
        Me.rtbQueuePollingConsole.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbQueuePollingConsole.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.rtbQueuePollingConsole.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbQueuePollingConsole.ForeColor = System.Drawing.Color.Gold
        Me.rtbQueuePollingConsole.Location = New System.Drawing.Point(0, 304)
        Me.rtbQueuePollingConsole.Name = "rtbQueuePollingConsole"
        Me.rtbQueuePollingConsole.ReadOnly = True
        Me.rtbQueuePollingConsole.Size = New System.Drawing.Size(981, 191)
        Me.rtbQueuePollingConsole.TabIndex = 10
        Me.rtbQueuePollingConsole.Text = ""
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(1, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(164, 13)
        Me.Label1.TabIndex = 11
        Me.Label1.Text = "Total Running Render Instances:"
        '
        'lblOverallTransferSpeed
        '
        Me.lblOverallTransferSpeed.AutoSize = True
        Me.lblOverallTransferSpeed.Location = New System.Drawing.Point(171, 9)
        Me.lblOverallTransferSpeed.Name = "lblOverallTransferSpeed"
        Me.lblOverallTransferSpeed.Size = New System.Drawing.Size(13, 13)
        Me.lblOverallTransferSpeed.TabIndex = 13
        Me.lblOverallTransferSpeed.Text = "[]"
        '
        'lvInstances
        '
        Me.lvInstances.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chInstanceId, Me.chAMIId, Me.chInstanceType, Me.chInstanceStartTime, Me.chInstanceScheduledTerminationTime, Me.chInstanceHasBeenRunningForMinutes, Me.chInstanceScheduledForTerminationIn, Me.chJobId, Me.chJobName, Me.chJobUserId, Me.chJobUsername, Me.chPublicDns})
        Me.lvInstances.ContextMenuStrip = Me.cmMain
        Me.lvInstances.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.lvInstances.FullRowSelect = True
        Me.lvInstances.GridLines = True
        ListViewGroup1.Header = "Assigned"
        ListViewGroup1.Name = "lgAssigned"
        ListViewGroup2.Header = "Unassigned"
        ListViewGroup2.Name = "lgUnassigned"
        ListViewGroup3.Header = "Terminated"
        ListViewGroup3.Name = "lgTerminated"
        Me.lvInstances.Groups.AddRange(New System.Windows.Forms.ListViewGroup() {ListViewGroup1, ListViewGroup2, ListViewGroup3})
        Me.lvInstances.HideSelection = False
        Me.lvInstances.Location = New System.Drawing.Point(0, 25)
        Me.lvInstances.Name = "lvInstances"
        Me.lvInstances.ShowGroups = False
        Me.lvInstances.Size = New System.Drawing.Size(981, 279)
        Me.lvInstances.TabIndex = 14
        Me.lvInstances.UseCompatibleStateImageBehavior = False
        Me.lvInstances.View = System.Windows.Forms.View.Details
        '
        'chInstanceId
        '
        Me.chInstanceId.Text = "Instance Id"
        Me.chInstanceId.Width = 73
        '
        'chAMIId
        '
        Me.chAMIId.Text = "AMI Id"
        '
        'chInstanceType
        '
        Me.chInstanceType.Text = "Type"
        '
        'chInstanceStartTime
        '
        Me.chInstanceStartTime.Text = "Start"
        Me.chInstanceStartTime.Width = 56
        '
        'chInstanceScheduledTerminationTime
        '
        Me.chInstanceScheduledTerminationTime.Text = "End"
        '
        'chInstanceHasBeenRunningForMinutes
        '
        Me.chInstanceHasBeenRunningForMinutes.Text = "Min. Active"
        Me.chInstanceHasBeenRunningForMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        Me.chInstanceHasBeenRunningForMinutes.Width = 76
        '
        'chInstanceScheduledForTerminationIn
        '
        Me.chInstanceScheduledForTerminationIn.Text = "Dies In"
        Me.chInstanceScheduledForTerminationIn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'chJobId
        '
        Me.chJobId.Text = "Job Id"
        Me.chJobId.Width = 123
        '
        'chJobName
        '
        Me.chJobName.Text = "Job Name"
        Me.chJobName.Width = 115
        '
        'chJobUserId
        '
        Me.chJobUserId.Text = "Job UId"
        Me.chJobUserId.Width = 67
        '
        'chJobUsername
        '
        Me.chJobUsername.Text = "Job Username"
        Me.chJobUsername.Width = 102
        '
        'chPublicDns
        '
        Me.chPublicDns.Text = "DNS"
        Me.chPublicDns.Width = 125
        '
        'InstanceMonitor_Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(981, 495)
        Me.Controls.Add(Me.lvInstances)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.rtbQueuePollingConsole)
        Me.Controls.Add(Me.lblOverallTransferSpeed)
        Me.Name = "InstanceMonitor_Main"
        Me.Text = "SMT Alentejo - Instance Monitor"
        Me.cmMain.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents INSTANCE_MONITOR_TIMER As System.Windows.Forms.Timer
    Friend WithEvents cmMain As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents cmTerminateInstance As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents cmAT_Pause As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents rtbQueuePollingConsole As System.Windows.Forms.RichTextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents lblOverallTransferSpeed As System.Windows.Forms.Label
    Friend WithEvents lvInstances As System.Windows.Forms.ListView
    Friend WithEvents chInstanceId As System.Windows.Forms.ColumnHeader
    Friend WithEvents chInstanceStartTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents chInstanceScheduledTerminationTime As System.Windows.Forms.ColumnHeader
    Friend WithEvents chInstanceHasBeenRunningForMinutes As System.Windows.Forms.ColumnHeader
    Friend WithEvents chInstanceScheduledForTerminationIn As System.Windows.Forms.ColumnHeader
    Friend WithEvents chInstanceType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chAMIId As System.Windows.Forms.ColumnHeader
    Friend WithEvents chJobId As System.Windows.Forms.ColumnHeader
    Friend WithEvents chJobName As System.Windows.Forms.ColumnHeader
    Friend WithEvents chJobUserId As System.Windows.Forms.ColumnHeader
    Friend WithEvents chJobUsername As System.Windows.Forms.ColumnHeader
    Friend WithEvents chPublicDns As System.Windows.Forms.ColumnHeader

End Class
