Imports SMT.Alentejo.Core.JobManagement

Partial Public Class JobViewer

    Private Job As cSMT_ATJ_RenderJob_Maxwell

    Public Sub New(ByVal nJob As cSMT_ATJ_RenderJob_Maxwell)
        InitializeComponent()
        Job = nJob
    End Sub

    Private Sub JobViewer_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        jobViewer.UpdateJobDisplay(Job)
        Me.Title = Job.Name
    End Sub

End Class
