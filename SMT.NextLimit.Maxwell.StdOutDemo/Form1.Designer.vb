<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.btnStartMaxwell = New System.Windows.Forms.Button
        Me.rtbQueuePollingConsole = New System.Windows.Forms.RichTextBox
        Me.btnStartGeneric = New System.Windows.Forms.Button
        Me.txtMXSPath = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'btnStartMaxwell
        '
        Me.btnStartMaxwell.Location = New System.Drawing.Point(199, 6)
        Me.btnStartMaxwell.Name = "btnStartMaxwell"
        Me.btnStartMaxwell.Size = New System.Drawing.Size(172, 23)
        Me.btnStartMaxwell.TabIndex = 15
        Me.btnStartMaxwell.Text = "Start Maxwell"
        Me.btnStartMaxwell.UseVisualStyleBackColor = True
        '
        'rtbQueuePollingConsole
        '
        Me.rtbQueuePollingConsole.BackColor = System.Drawing.Color.Black
        Me.rtbQueuePollingConsole.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.rtbQueuePollingConsole.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.rtbQueuePollingConsole.ForeColor = System.Drawing.Color.Gold
        Me.rtbQueuePollingConsole.Location = New System.Drawing.Point(0, 41)
        Me.rtbQueuePollingConsole.Name = "rtbQueuePollingConsole"
        Me.rtbQueuePollingConsole.ReadOnly = True
        Me.rtbQueuePollingConsole.Size = New System.Drawing.Size(654, 254)
        Me.rtbQueuePollingConsole.TabIndex = 14
        Me.rtbQueuePollingConsole.Text = ""
        '
        'btnStartGeneric
        '
        Me.btnStartGeneric.Location = New System.Drawing.Point(12, 6)
        Me.btnStartGeneric.Name = "btnStartGeneric"
        Me.btnStartGeneric.Size = New System.Drawing.Size(181, 23)
        Me.btnStartGeneric.TabIndex = 13
        Me.btnStartGeneric.Text = "Start Generic Console App"
        Me.btnStartGeneric.UseVisualStyleBackColor = True
        '
        'txtMXSPath
        '
        Me.txtMXSPath.Location = New System.Drawing.Point(377, 8)
        Me.txtMXSPath.Name = "txtMXSPath"
        Me.txtMXSPath.Size = New System.Drawing.Size(264, 20)
        Me.txtMXSPath.TabIndex = 16
        Me.txtMXSPath.Text = "N:\PRODUCT DEVELOPMENT\Alentejo\Maxwell Projects\Vase diamond\vase diamond.mxs"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(654, 295)
        Me.Controls.Add(Me.txtMXSPath)
        Me.Controls.Add(Me.btnStartMaxwell)
        Me.Controls.Add(Me.rtbQueuePollingConsole)
        Me.Controls.Add(Me.btnStartGeneric)
        Me.Name = "Form1"
        Me.Text = "Form1"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnStartMaxwell As System.Windows.Forms.Button
    Friend WithEvents rtbQueuePollingConsole As System.Windows.Forms.RichTextBox
    Friend WithEvents btnStartGeneric As System.Windows.Forms.Button
    Friend WithEvents txtMXSPath As System.Windows.Forms.TextBox

End Class
