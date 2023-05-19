<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RenderManager_Main
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
        Me.rtbQueuePollingConsole = New System.Windows.Forms.RichTextBox
        Me.AlentejoInstanceHealthEventLog = New System.Diagnostics.EventLog
        CType(Me.AlentejoInstanceHealthEventLog, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'rtbQueuePollingConsole
        '
        Me.rtbQueuePollingConsole.BackColor = System.Drawing.Color.Black
        Me.rtbQueuePollingConsole.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbQueuePollingConsole.Cursor = System.Windows.Forms.Cursors.Arrow
        Me.rtbQueuePollingConsole.Dock = System.Windows.Forms.DockStyle.Fill
        Me.rtbQueuePollingConsole.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.rtbQueuePollingConsole.ForeColor = System.Drawing.Color.Gold
        Me.rtbQueuePollingConsole.Location = New System.Drawing.Point(0, 0)
        Me.rtbQueuePollingConsole.Name = "rtbQueuePollingConsole"
        Me.rtbQueuePollingConsole.ReadOnly = True
        Me.rtbQueuePollingConsole.Size = New System.Drawing.Size(701, 288)
        Me.rtbQueuePollingConsole.TabIndex = 11
        Me.rtbQueuePollingConsole.Text = ""
        '
        'AlentejoInstanceHealthEventLog
        '
        Me.AlentejoInstanceHealthEventLog.Log = "SMT Alentejo"
        Me.AlentejoInstanceHealthEventLog.Source = "Render Manager"
        Me.AlentejoInstanceHealthEventLog.SynchronizingObject = Me
        '
        'RenderManager_Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(701, 288)
        Me.Controls.Add(Me.rtbQueuePollingConsole)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "RenderManager_Main"
        Me.Text = "SMT Alentejo - Render Manager"
        CType(Me.AlentejoInstanceHealthEventLog, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rtbQueuePollingConsole As System.Windows.Forms.RichTextBox
    Friend WithEvents AlentejoInstanceHealthEventLog As System.Diagnostics.EventLog

End Class
