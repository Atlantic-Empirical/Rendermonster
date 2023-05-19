<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AlentejoTestHarness_Form
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
        Me.tcMain = New System.Windows.Forms.TabControl
        Me.tpJobSubmission = New System.Windows.Forms.TabPage
        Me.btnInstancesAvailable = New System.Windows.Forms.Button
        Me.btnScrap = New System.Windows.Forms.Button
        Me.btnRetrieveJob = New System.Windows.Forms.Button
        Me.btnCreateAndStoreJob = New System.Windows.Forms.Button
        Me.btnPutFileTest = New System.Windows.Forms.Button
        Me.tcMain.SuspendLayout()
        Me.tpJobSubmission.SuspendLayout()
        Me.SuspendLayout()
        '
        'tcMain
        '
        Me.tcMain.Controls.Add(Me.tpJobSubmission)
        Me.tcMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tcMain.Location = New System.Drawing.Point(0, 0)
        Me.tcMain.Name = "tcMain"
        Me.tcMain.SelectedIndex = 0
        Me.tcMain.Size = New System.Drawing.Size(774, 544)
        Me.tcMain.TabIndex = 0
        '
        'tpJobSubmission
        '
        Me.tpJobSubmission.Controls.Add(Me.btnPutFileTest)
        Me.tpJobSubmission.Controls.Add(Me.btnInstancesAvailable)
        Me.tpJobSubmission.Controls.Add(Me.btnScrap)
        Me.tpJobSubmission.Controls.Add(Me.btnRetrieveJob)
        Me.tpJobSubmission.Controls.Add(Me.btnCreateAndStoreJob)
        Me.tpJobSubmission.Location = New System.Drawing.Point(4, 22)
        Me.tpJobSubmission.Name = "tpJobSubmission"
        Me.tpJobSubmission.Padding = New System.Windows.Forms.Padding(3)
        Me.tpJobSubmission.Size = New System.Drawing.Size(766, 518)
        Me.tpJobSubmission.TabIndex = 1
        Me.tpJobSubmission.Text = "Job Submission"
        Me.tpJobSubmission.UseVisualStyleBackColor = True
        '
        'btnInstancesAvailable
        '
        Me.btnInstancesAvailable.Location = New System.Drawing.Point(8, 64)
        Me.btnInstancesAvailable.Name = "btnInstancesAvailable"
        Me.btnInstancesAvailable.Size = New System.Drawing.Size(156, 23)
        Me.btnInstancesAvailable.TabIndex = 3
        Me.btnInstancesAvailable.Text = "Instances Available"
        Me.btnInstancesAvailable.UseVisualStyleBackColor = True
        '
        'btnScrap
        '
        Me.btnScrap.Location = New System.Drawing.Point(185, 6)
        Me.btnScrap.Name = "btnScrap"
        Me.btnScrap.Size = New System.Drawing.Size(156, 23)
        Me.btnScrap.TabIndex = 2
        Me.btnScrap.Text = "Scrap"
        Me.btnScrap.UseVisualStyleBackColor = True
        '
        'btnRetrieveJob
        '
        Me.btnRetrieveJob.Location = New System.Drawing.Point(8, 35)
        Me.btnRetrieveJob.Name = "btnRetrieveJob"
        Me.btnRetrieveJob.Size = New System.Drawing.Size(156, 23)
        Me.btnRetrieveJob.TabIndex = 1
        Me.btnRetrieveJob.Text = "Retrieve Job"
        Me.btnRetrieveJob.UseVisualStyleBackColor = True
        '
        'btnCreateAndStoreJob
        '
        Me.btnCreateAndStoreJob.Location = New System.Drawing.Point(8, 6)
        Me.btnCreateAndStoreJob.Name = "btnCreateAndStoreJob"
        Me.btnCreateAndStoreJob.Size = New System.Drawing.Size(156, 23)
        Me.btnCreateAndStoreJob.TabIndex = 0
        Me.btnCreateAndStoreJob.Text = "Create and Store Job"
        Me.btnCreateAndStoreJob.UseVisualStyleBackColor = True
        '
        'btnPutFileTest
        '
        Me.btnPutFileTest.Location = New System.Drawing.Point(8, 113)
        Me.btnPutFileTest.Name = "btnPutFileTest"
        Me.btnPutFileTest.Size = New System.Drawing.Size(156, 23)
        Me.btnPutFileTest.TabIndex = 4
        Me.btnPutFileTest.Text = "Put File Test"
        Me.btnPutFileTest.UseVisualStyleBackColor = True
        '
        'AlentejoTestHarness_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(774, 544)
        Me.Controls.Add(Me.tcMain)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Name = "AlentejoTestHarness_Form"
        Me.Text = "SMT Test Harness - Alentejo"
        Me.tcMain.ResumeLayout(False)
        Me.tpJobSubmission.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tcMain As System.Windows.Forms.TabControl
    Friend WithEvents tpJobSubmission As System.Windows.Forms.TabPage
    Friend WithEvents btnCreateAndStoreJob As System.Windows.Forms.Button
    Friend WithEvents btnRetrieveJob As System.Windows.Forms.Button
    Friend WithEvents btnScrap As System.Windows.Forms.Button
    Friend WithEvents btnInstancesAvailable As System.Windows.Forms.Button
    Friend WithEvents btnPutFileTest As System.Windows.Forms.Button

End Class
