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
        Me.btnEncrypt = New System.Windows.Forms.Button
        Me.txtBalance = New System.Windows.Forms.TextBox
        Me.btnDecrypt = New System.Windows.Forms.Button
        Me.txtCipher = New System.Windows.Forms.TextBox
        Me.Button1 = New System.Windows.Forms.Button
        Me.Button2 = New System.Windows.Forms.Button
        Me.txtUserId = New System.Windows.Forms.TextBox
        Me.SuspendLayout()
        '
        'btnEncrypt
        '
        Me.btnEncrypt.Location = New System.Drawing.Point(254, 12)
        Me.btnEncrypt.Name = "btnEncrypt"
        Me.btnEncrypt.Size = New System.Drawing.Size(75, 23)
        Me.btnEncrypt.TabIndex = 0
        Me.btnEncrypt.Text = "->"
        Me.btnEncrypt.UseVisualStyleBackColor = True
        '
        'txtBalance
        '
        Me.txtBalance.Location = New System.Drawing.Point(12, 12)
        Me.txtBalance.Multiline = True
        Me.txtBalance.Name = "txtBalance"
        Me.txtBalance.Size = New System.Drawing.Size(236, 104)
        Me.txtBalance.TabIndex = 1
        '
        'btnDecrypt
        '
        Me.btnDecrypt.Location = New System.Drawing.Point(254, 41)
        Me.btnDecrypt.Name = "btnDecrypt"
        Me.btnDecrypt.Size = New System.Drawing.Size(75, 23)
        Me.btnDecrypt.TabIndex = 2
        Me.btnDecrypt.Text = "<-"
        Me.btnDecrypt.UseVisualStyleBackColor = True
        '
        'txtCipher
        '
        Me.txtCipher.Location = New System.Drawing.Point(345, 12)
        Me.txtCipher.Multiline = True
        Me.txtCipher.Name = "txtCipher"
        Me.txtCipher.Size = New System.Drawing.Size(236, 104)
        Me.txtCipher.TabIndex = 3
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(98, 118)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 5
        Me.Button1.Text = "<-"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(421, 122)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 4
        Me.Button2.Text = "->"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'txtUserId
        '
        Me.txtUserId.Location = New System.Drawing.Point(179, 121)
        Me.txtUserId.Name = "txtUserId"
        Me.txtUserId.Size = New System.Drawing.Size(236, 20)
        Me.txtUserId.TabIndex = 6
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(596, 146)
        Me.Controls.Add(Me.txtUserId)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.txtCipher)
        Me.Controls.Add(Me.btnDecrypt)
        Me.Controls.Add(Me.txtBalance)
        Me.Controls.Add(Me.btnEncrypt)
        Me.Name = "Form1"
        Me.Text = "SMT.Alentejo.Credit.TestHarness"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnEncrypt As System.Windows.Forms.Button
    Friend WithEvents txtBalance As System.Windows.Forms.TextBox
    Friend WithEvents btnDecrypt As System.Windows.Forms.Button
    Friend WithEvents txtCipher As System.Windows.Forms.TextBox
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents txtUserId As System.Windows.Forms.TextBox

End Class
