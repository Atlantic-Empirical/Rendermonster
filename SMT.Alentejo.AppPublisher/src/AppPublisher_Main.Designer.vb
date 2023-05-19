<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppPublisher_Main
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
        Me.cbJobFileTransferManager = New System.Windows.Forms.CheckBox
        Me.cbInstanceDispatcher = New System.Windows.Forms.CheckBox
        Me.cbRenderManager_MX = New System.Windows.Forms.CheckBox
        Me.cbMergeManager_MX = New System.Windows.Forms.CheckBox
        Me.rbDebug = New System.Windows.Forms.RadioButton
        Me.rbRelease = New System.Windows.Forms.RadioButton
        Me.Panel1 = New System.Windows.Forms.Panel
        Me.btnPublish = New System.Windows.Forms.Button
        Me.rtbQueuePollingConsole = New System.Windows.Forms.RichTextBox
        Me.txtSMTCodePathRoot = New System.Windows.Forms.TextBox
        Me.TabControl1 = New System.Windows.Forms.TabControl
        Me.TabPage1 = New System.Windows.Forms.TabPage
        Me.TabPage3 = New System.Windows.Forms.TabPage
        Me.cbRET_JobFileTransferManager = New System.Windows.Forms.CheckBox
        Me.txtRET_Directory = New System.Windows.Forms.TextBox
        Me.cbRET_InstanceDispatcher = New System.Windows.Forms.CheckBox
        Me.cbRET_RenderManagerMX = New System.Windows.Forms.CheckBox
        Me.btnRetrieve = New System.Windows.Forms.Button
        Me.cbRET_MergeManagerMX = New System.Windows.Forms.CheckBox
        Me.TabPage2 = New System.Windows.Forms.TabPage
        Me.btnDeleteServerMap = New System.Windows.Forms.Button
        Me.btnPrintServerMaps = New System.Windows.Forms.Button
        Me.btnSetServerMap = New System.Windows.Forms.Button
        Me.cbApps = New System.Windows.Forms.ComboBox
        Me.cbServers = New System.Windows.Forms.ComboBox
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer
        Me.cbPUB_LogReader = New System.Windows.Forms.CheckBox
        Me.cbRET_LogReader = New System.Windows.Forms.CheckBox
        Me.Panel1.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage3.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'cbJobFileTransferManager
        '
        Me.cbJobFileTransferManager.AutoSize = True
        Me.cbJobFileTransferManager.Location = New System.Drawing.Point(5, 5)
        Me.cbJobFileTransferManager.Name = "cbJobFileTransferManager"
        Me.cbJobFileTransferManager.Size = New System.Drawing.Size(149, 17)
        Me.cbJobFileTransferManager.TabIndex = 0
        Me.cbJobFileTransferManager.Text = "Job File Transfer Manager"
        Me.cbJobFileTransferManager.UseVisualStyleBackColor = True
        '
        'cbInstanceDispatcher
        '
        Me.cbInstanceDispatcher.AutoSize = True
        Me.cbInstanceDispatcher.Location = New System.Drawing.Point(5, 23)
        Me.cbInstanceDispatcher.Name = "cbInstanceDispatcher"
        Me.cbInstanceDispatcher.Size = New System.Drawing.Size(121, 17)
        Me.cbInstanceDispatcher.TabIndex = 1
        Me.cbInstanceDispatcher.Text = "Instance Dispatcher"
        Me.cbInstanceDispatcher.UseVisualStyleBackColor = True
        '
        'cbRenderManager_MX
        '
        Me.cbRenderManager_MX.AutoSize = True
        Me.cbRenderManager_MX.Location = New System.Drawing.Point(5, 41)
        Me.cbRenderManager_MX.Name = "cbRenderManager_MX"
        Me.cbRenderManager_MX.Size = New System.Drawing.Size(125, 17)
        Me.cbRenderManager_MX.TabIndex = 2
        Me.cbRenderManager_MX.Text = "Render Manager MX"
        Me.cbRenderManager_MX.UseVisualStyleBackColor = True
        '
        'cbMergeManager_MX
        '
        Me.cbMergeManager_MX.AutoSize = True
        Me.cbMergeManager_MX.Enabled = False
        Me.cbMergeManager_MX.Location = New System.Drawing.Point(5, 59)
        Me.cbMergeManager_MX.Name = "cbMergeManager_MX"
        Me.cbMergeManager_MX.Size = New System.Drawing.Size(120, 17)
        Me.cbMergeManager_MX.TabIndex = 3
        Me.cbMergeManager_MX.Text = "Merge Manager MX"
        Me.cbMergeManager_MX.UseVisualStyleBackColor = True
        '
        'rbDebug
        '
        Me.rbDebug.AutoSize = True
        Me.rbDebug.Location = New System.Drawing.Point(3, 27)
        Me.rbDebug.Name = "rbDebug"
        Me.rbDebug.Size = New System.Drawing.Size(57, 17)
        Me.rbDebug.TabIndex = 4
        Me.rbDebug.Text = "Debug"
        Me.rbDebug.UseVisualStyleBackColor = True
        '
        'rbRelease
        '
        Me.rbRelease.AutoSize = True
        Me.rbRelease.Checked = True
        Me.rbRelease.Location = New System.Drawing.Point(3, 5)
        Me.rbRelease.Name = "rbRelease"
        Me.rbRelease.Size = New System.Drawing.Size(64, 17)
        Me.rbRelease.TabIndex = 5
        Me.rbRelease.TabStop = True
        Me.rbRelease.Text = "Release"
        Me.rbRelease.UseVisualStyleBackColor = True
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.rbRelease)
        Me.Panel1.Controls.Add(Me.rbDebug)
        Me.Panel1.Location = New System.Drawing.Point(465, 6)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(72, 50)
        Me.Panel1.TabIndex = 6
        '
        'btnPublish
        '
        Me.btnPublish.Location = New System.Drawing.Point(5, 80)
        Me.btnPublish.Name = "btnPublish"
        Me.btnPublish.Size = New System.Drawing.Size(752, 36)
        Me.btnPublish.TabIndex = 7
        Me.btnPublish.Text = "Publish"
        Me.btnPublish.UseVisualStyleBackColor = True
        '
        'rtbQueuePollingConsole
        '
        Me.rtbQueuePollingConsole.BackColor = System.Drawing.Color.Black
        Me.rtbQueuePollingConsole.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbQueuePollingConsole.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbQueuePollingConsole.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbQueuePollingConsole.ForeColor = System.Drawing.Color.Gold
        Me.rtbQueuePollingConsole.Location = New System.Drawing.Point(0, 0)
        Me.rtbQueuePollingConsole.Name = "rtbQueuePollingConsole"
        Me.rtbQueuePollingConsole.ReadOnly = True
        Me.rtbQueuePollingConsole.Size = New System.Drawing.Size(768, 223)
        Me.rtbQueuePollingConsole.TabIndex = 11
        Me.rtbQueuePollingConsole.Text = ""
        '
        'txtSMTCodePathRoot
        '
        Me.txtSMTCodePathRoot.Location = New System.Drawing.Point(465, 59)
        Me.txtSMTCodePathRoot.Name = "txtSMTCodePathRoot"
        Me.txtSMTCodePathRoot.Size = New System.Drawing.Size(292, 20)
        Me.txtSMTCodePathRoot.TabIndex = 12
        Me.txtSMTCodePathRoot.Text = "T:\CODE\SMT\Solutions\ALENTEJO\SMT.Alentejo."
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage3)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(768, 145)
        Me.TabControl1.TabIndex = 13
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.cbPUB_LogReader)
        Me.TabPage1.Controls.Add(Me.cbJobFileTransferManager)
        Me.TabPage1.Controls.Add(Me.txtSMTCodePathRoot)
        Me.TabPage1.Controls.Add(Me.cbInstanceDispatcher)
        Me.TabPage1.Controls.Add(Me.cbRenderManager_MX)
        Me.TabPage1.Controls.Add(Me.btnPublish)
        Me.TabPage1.Controls.Add(Me.cbMergeManager_MX)
        Me.TabPage1.Controls.Add(Me.Panel1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(760, 119)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Publishing"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage3
        '
        Me.TabPage3.Controls.Add(Me.cbRET_LogReader)
        Me.TabPage3.Controls.Add(Me.cbRET_JobFileTransferManager)
        Me.TabPage3.Controls.Add(Me.txtRET_Directory)
        Me.TabPage3.Controls.Add(Me.cbRET_InstanceDispatcher)
        Me.TabPage3.Controls.Add(Me.cbRET_RenderManagerMX)
        Me.TabPage3.Controls.Add(Me.btnRetrieve)
        Me.TabPage3.Controls.Add(Me.cbRET_MergeManagerMX)
        Me.TabPage3.Location = New System.Drawing.Point(4, 22)
        Me.TabPage3.Name = "TabPage3"
        Me.TabPage3.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage3.Size = New System.Drawing.Size(760, 119)
        Me.TabPage3.TabIndex = 2
        Me.TabPage3.Text = "Retrieval"
        Me.TabPage3.UseVisualStyleBackColor = True
        '
        'cbRET_JobFileTransferManager
        '
        Me.cbRET_JobFileTransferManager.AutoSize = True
        Me.cbRET_JobFileTransferManager.Location = New System.Drawing.Point(6, 6)
        Me.cbRET_JobFileTransferManager.Name = "cbRET_JobFileTransferManager"
        Me.cbRET_JobFileTransferManager.Size = New System.Drawing.Size(149, 17)
        Me.cbRET_JobFileTransferManager.TabIndex = 13
        Me.cbRET_JobFileTransferManager.Text = "Job File Transfer Manager"
        Me.cbRET_JobFileTransferManager.UseVisualStyleBackColor = True
        '
        'txtRET_Directory
        '
        Me.txtRET_Directory.Location = New System.Drawing.Point(558, 3)
        Me.txtRET_Directory.Name = "txtRET_Directory"
        Me.txtRET_Directory.Size = New System.Drawing.Size(196, 20)
        Me.txtRET_Directory.TabIndex = 18
        Me.txtRET_Directory.Text = "C:\Temp\Alentejo\AppRetrieval\"
        '
        'cbRET_InstanceDispatcher
        '
        Me.cbRET_InstanceDispatcher.AutoSize = True
        Me.cbRET_InstanceDispatcher.Location = New System.Drawing.Point(6, 24)
        Me.cbRET_InstanceDispatcher.Name = "cbRET_InstanceDispatcher"
        Me.cbRET_InstanceDispatcher.Size = New System.Drawing.Size(121, 17)
        Me.cbRET_InstanceDispatcher.TabIndex = 14
        Me.cbRET_InstanceDispatcher.Text = "Instance Dispatcher"
        Me.cbRET_InstanceDispatcher.UseVisualStyleBackColor = True
        '
        'cbRET_RenderManagerMX
        '
        Me.cbRET_RenderManagerMX.AutoSize = True
        Me.cbRET_RenderManagerMX.Location = New System.Drawing.Point(6, 42)
        Me.cbRET_RenderManagerMX.Name = "cbRET_RenderManagerMX"
        Me.cbRET_RenderManagerMX.Size = New System.Drawing.Size(125, 17)
        Me.cbRET_RenderManagerMX.TabIndex = 15
        Me.cbRET_RenderManagerMX.Text = "Render Manager MX"
        Me.cbRET_RenderManagerMX.UseVisualStyleBackColor = True
        '
        'btnRetrieve
        '
        Me.btnRetrieve.Location = New System.Drawing.Point(5, 80)
        Me.btnRetrieve.Name = "btnRetrieve"
        Me.btnRetrieve.Size = New System.Drawing.Size(748, 36)
        Me.btnRetrieve.TabIndex = 17
        Me.btnRetrieve.Text = "Retrieve"
        Me.btnRetrieve.UseVisualStyleBackColor = True
        '
        'cbRET_MergeManagerMX
        '
        Me.cbRET_MergeManagerMX.AutoSize = True
        Me.cbRET_MergeManagerMX.Enabled = False
        Me.cbRET_MergeManagerMX.Location = New System.Drawing.Point(6, 60)
        Me.cbRET_MergeManagerMX.Name = "cbRET_MergeManagerMX"
        Me.cbRET_MergeManagerMX.Size = New System.Drawing.Size(120, 17)
        Me.cbRET_MergeManagerMX.TabIndex = 16
        Me.cbRET_MergeManagerMX.Text = "Merge Manager MX"
        Me.cbRET_MergeManagerMX.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.btnDeleteServerMap)
        Me.TabPage2.Controls.Add(Me.btnPrintServerMaps)
        Me.TabPage2.Controls.Add(Me.btnSetServerMap)
        Me.TabPage2.Controls.Add(Me.cbApps)
        Me.TabPage2.Controls.Add(Me.cbServers)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(760, 119)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "Server App Mapping"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'btnDeleteServerMap
        '
        Me.btnDeleteServerMap.Location = New System.Drawing.Point(306, 33)
        Me.btnDeleteServerMap.Name = "btnDeleteServerMap"
        Me.btnDeleteServerMap.Size = New System.Drawing.Size(108, 23)
        Me.btnDeleteServerMap.TabIndex = 4
        Me.btnDeleteServerMap.Text = "Delete Server Map"
        Me.btnDeleteServerMap.UseVisualStyleBackColor = True
        '
        'btnPrintServerMaps
        '
        Me.btnPrintServerMaps.Location = New System.Drawing.Point(3, 90)
        Me.btnPrintServerMaps.Name = "btnPrintServerMaps"
        Me.btnPrintServerMaps.Size = New System.Drawing.Size(75, 23)
        Me.btnPrintServerMaps.TabIndex = 3
        Me.btnPrintServerMaps.Text = "Print Maps"
        Me.btnPrintServerMaps.UseVisualStyleBackColor = True
        '
        'btnSetServerMap
        '
        Me.btnSetServerMap.Location = New System.Drawing.Point(306, 4)
        Me.btnSetServerMap.Name = "btnSetServerMap"
        Me.btnSetServerMap.Size = New System.Drawing.Size(108, 23)
        Me.btnSetServerMap.TabIndex = 2
        Me.btnSetServerMap.Text = "Set Server Map"
        Me.btnSetServerMap.UseVisualStyleBackColor = True
        '
        'cbApps
        '
        Me.cbApps.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbApps.FormattingEnabled = True
        Me.cbApps.Location = New System.Drawing.Point(139, 6)
        Me.cbApps.Name = "cbApps"
        Me.cbApps.Size = New System.Drawing.Size(161, 21)
        Me.cbApps.TabIndex = 1
        '
        'cbServers
        '
        Me.cbServers.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbServers.FormattingEnabled = True
        Me.cbServers.Location = New System.Drawing.Point(8, 6)
        Me.cbServers.Name = "cbServers"
        Me.cbServers.Size = New System.Drawing.Size(125, 21)
        Me.cbServers.TabIndex = 0
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TabControl1)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.rtbQueuePollingConsole)
        Me.SplitContainer1.Size = New System.Drawing.Size(768, 372)
        Me.SplitContainer1.SplitterDistance = 145
        Me.SplitContainer1.TabIndex = 14
        '
        'cbPUB_LogReader
        '
        Me.cbPUB_LogReader.AutoSize = True
        Me.cbPUB_LogReader.Location = New System.Drawing.Point(171, 5)
        Me.cbPUB_LogReader.Name = "cbPUB_LogReader"
        Me.cbPUB_LogReader.Size = New System.Drawing.Size(82, 17)
        Me.cbPUB_LogReader.TabIndex = 13
        Me.cbPUB_LogReader.Text = "Log Reader"
        Me.cbPUB_LogReader.UseVisualStyleBackColor = True
        '
        'cbRET_LogReader
        '
        Me.cbRET_LogReader.AutoSize = True
        Me.cbRET_LogReader.Location = New System.Drawing.Point(170, 6)
        Me.cbRET_LogReader.Name = "cbRET_LogReader"
        Me.cbRET_LogReader.Size = New System.Drawing.Size(82, 17)
        Me.cbRET_LogReader.TabIndex = 19
        Me.cbRET_LogReader.Text = "Log Reader"
        Me.cbRET_LogReader.UseVisualStyleBackColor = True
        '
        'AppPublisher_Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(768, 372)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "AppPublisher_Main"
        Me.Text = "SMT Alentejo - App Publisher"
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.TabPage3.ResumeLayout(False)
        Me.TabPage3.PerformLayout()
        Me.TabPage2.ResumeLayout(False)
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents cbJobFileTransferManager As System.Windows.Forms.CheckBox
    Friend WithEvents cbInstanceDispatcher As System.Windows.Forms.CheckBox
    Friend WithEvents cbRenderManager_MX As System.Windows.Forms.CheckBox
    Friend WithEvents cbMergeManager_MX As System.Windows.Forms.CheckBox
    Friend WithEvents rbDebug As System.Windows.Forms.RadioButton
    Friend WithEvents rbRelease As System.Windows.Forms.RadioButton
    Friend WithEvents Panel1 As System.Windows.Forms.Panel
    Friend WithEvents btnPublish As System.Windows.Forms.Button
    Friend WithEvents rtbQueuePollingConsole As System.Windows.Forms.RichTextBox
    Friend WithEvents txtSMTCodePathRoot As System.Windows.Forms.TextBox
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents btnPrintServerMaps As System.Windows.Forms.Button
    Friend WithEvents btnSetServerMap As System.Windows.Forms.Button
    Friend WithEvents cbApps As System.Windows.Forms.ComboBox
    Friend WithEvents cbServers As System.Windows.Forms.ComboBox
    Friend WithEvents btnDeleteServerMap As System.Windows.Forms.Button
    Friend WithEvents SplitContainer1 As System.Windows.Forms.SplitContainer
    Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
    Friend WithEvents cbRET_JobFileTransferManager As System.Windows.Forms.CheckBox
    Friend WithEvents txtRET_Directory As System.Windows.Forms.TextBox
    Friend WithEvents cbRET_InstanceDispatcher As System.Windows.Forms.CheckBox
    Friend WithEvents cbRET_RenderManagerMX As System.Windows.Forms.CheckBox
    Friend WithEvents btnRetrieve As System.Windows.Forms.Button
    Friend WithEvents cbRET_MergeManagerMX As System.Windows.Forms.CheckBox
    Friend WithEvents cbPUB_LogReader As System.Windows.Forms.CheckBox
    Friend WithEvents cbRET_LogReader As System.Windows.Forms.CheckBox

End Class
