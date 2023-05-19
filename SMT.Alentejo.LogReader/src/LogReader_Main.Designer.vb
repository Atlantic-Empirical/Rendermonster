<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class LogReader_Main
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
        Me.cbLogName = New System.Windows.Forms.ComboBox
        Me.cbSource = New System.Windows.Forms.ComboBox
        Me.btnUpdate = New System.Windows.Forms.Button
        Me.lbLogEntries = New System.Windows.Forms.ListView
        Me.chLog = New System.Windows.Forms.ColumnHeader
        Me.chSource = New System.Windows.Forms.ColumnHeader
        Me.chType = New System.Windows.Forms.ColumnHeader
        Me.chDate = New System.Windows.Forms.ColumnHeader
        Me.txtMessage = New System.Windows.Forms.TextBox
        Me.btnClearLog = New System.Windows.Forms.Button
        Me.rtbLog = New System.Windows.Forms.RichTextBox
        Me.cbTimer = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'cbLogName
        '
        Me.cbLogName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLogName.FormattingEnabled = True
        Me.cbLogName.Items.AddRange(New Object() {"SMT Alentejo", "Application", "System"})
        Me.cbLogName.Location = New System.Drawing.Point(12, 12)
        Me.cbLogName.Name = "cbLogName"
        Me.cbLogName.Size = New System.Drawing.Size(121, 21)
        Me.cbLogName.TabIndex = 0
        '
        'cbSource
        '
        Me.cbSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbSource.FormattingEnabled = True
        Me.cbSource.Items.AddRange(New Object() {"All", "Render Manager", "Instance Dispatcher", "Job File Transfer Manager", "Instance Health Service", "Server Health Service", "SMT.AWS.dll"})
        Me.cbSource.Location = New System.Drawing.Point(139, 12)
        Me.cbSource.Name = "cbSource"
        Me.cbSource.Size = New System.Drawing.Size(121, 21)
        Me.cbSource.TabIndex = 1
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(266, 12)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(75, 21)
        Me.btnUpdate.TabIndex = 2
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'lbLogEntries
        '
        Me.lbLogEntries.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chLog, Me.chSource, Me.chType, Me.chDate})
        Me.lbLogEntries.FullRowSelect = True
        Me.lbLogEntries.GridLines = True
        Me.lbLogEntries.HideSelection = False
        Me.lbLogEntries.Location = New System.Drawing.Point(12, 39)
        Me.lbLogEntries.MultiSelect = False
        Me.lbLogEntries.Name = "lbLogEntries"
        Me.lbLogEntries.Size = New System.Drawing.Size(440, 239)
        Me.lbLogEntries.TabIndex = 3
        Me.lbLogEntries.UseCompatibleStateImageBehavior = False
        Me.lbLogEntries.View = System.Windows.Forms.View.Details
        '
        'chLog
        '
        Me.chLog.Text = "Log"
        Me.chLog.Width = 80
        '
        'chSource
        '
        Me.chSource.Text = "Source"
        Me.chSource.Width = 140
        '
        'chType
        '
        Me.chType.Text = "Type"
        Me.chType.Width = 75
        '
        'chDate
        '
        Me.chDate.Text = "Date"
        Me.chDate.Width = 140
        '
        'txtMessage
        '
        Me.txtMessage.Location = New System.Drawing.Point(458, 39)
        Me.txtMessage.Multiline = True
        Me.txtMessage.Name = "txtMessage"
        Me.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtMessage.Size = New System.Drawing.Size(389, 239)
        Me.txtMessage.TabIndex = 4
        '
        'btnClearLog
        '
        Me.btnClearLog.Location = New System.Drawing.Point(347, 12)
        Me.btnClearLog.Name = "btnClearLog"
        Me.btnClearLog.Size = New System.Drawing.Size(75, 21)
        Me.btnClearLog.TabIndex = 5
        Me.btnClearLog.Text = "Clear"
        Me.btnClearLog.UseVisualStyleBackColor = True
        '
        'rtbLog
        '
        Me.rtbLog.Location = New System.Drawing.Point(12, 284)
        Me.rtbLog.Name = "rtbLog"
        Me.rtbLog.Size = New System.Drawing.Size(835, 247)
        Me.rtbLog.TabIndex = 6
        Me.rtbLog.Text = ""
        '
        'cbTimer
        '
        Me.cbTimer.AutoSize = True
        Me.cbTimer.Checked = True
        Me.cbTimer.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbTimer.Location = New System.Drawing.Point(428, 16)
        Me.cbTimer.Name = "cbTimer"
        Me.cbTimer.Size = New System.Drawing.Size(75, 17)
        Me.cbTimer.TabIndex = 7
        Me.cbTimer.Text = "Run Timer"
        Me.cbTimer.UseVisualStyleBackColor = True
        '
        'LogReader_Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(859, 543)
        Me.Controls.Add(Me.cbTimer)
        Me.Controls.Add(Me.rtbLog)
        Me.Controls.Add(Me.btnClearLog)
        Me.Controls.Add(Me.txtMessage)
        Me.Controls.Add(Me.lbLogEntries)
        Me.Controls.Add(Me.btnUpdate)
        Me.Controls.Add(Me.cbSource)
        Me.Controls.Add(Me.cbLogName)
        Me.Name = "LogReader_Main"
        Me.Text = "SMT Alentejo - Log Reader"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents cbLogName As System.Windows.Forms.ComboBox
    Friend WithEvents cbSource As System.Windows.Forms.ComboBox
    Friend WithEvents btnUpdate As System.Windows.Forms.Button
    Friend WithEvents lbLogEntries As System.Windows.Forms.ListView
    Friend WithEvents chType As System.Windows.Forms.ColumnHeader
    Friend WithEvents chDate As System.Windows.Forms.ColumnHeader
    Friend WithEvents chSource As System.Windows.Forms.ColumnHeader
    Friend WithEvents chLog As System.Windows.Forms.ColumnHeader
    Friend WithEvents txtMessage As System.Windows.Forms.TextBox
    Friend WithEvents btnClearLog As System.Windows.Forms.Button
    Friend WithEvents rtbLog As System.Windows.Forms.RichTextBox
    Friend WithEvents cbTimer As System.Windows.Forms.CheckBox

End Class
