<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class JobFileTransferManager_Main
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
        Me.SuspendLayout()
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
        Me.rtbQueuePollingConsole.Size = New System.Drawing.Size(702, 288)
        Me.rtbQueuePollingConsole.TabIndex = 10
        Me.rtbQueuePollingConsole.Text = ""
        '
        'JobFileTransferManager_Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(702, 288)
        Me.Controls.Add(Me.rtbQueuePollingConsole)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D
        Me.Name = "JobFileTransferManager_Main"
        Me.Text = "SMT Alentejo - Job File Transfer Manager"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents rtbQueuePollingConsole As System.Windows.Forms.RichTextBox

End Class
