Imports SMT.Alentejo.Core.UserManagement

Partial Public Class ucUserViewer

    Public Event evCloseUser()

    Public Sub UpdateUserViewer()
        tbUserHeader.Text = CurrentUser.FirstName & " " & CurrentUser.LastName & " - " & CurrentUser.Company
    End Sub

    Private Sub ptCloseUser_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ptCloseUser.MouseLeftButtonUp
        RaiseEvent evCloseUser()
    End Sub

    Private Sub HandleJobSelected() Handles jobSelector.evJobSelected
        OpenJobViewer()
        'jobViewer.UpdateJobDisplay()
        'jobSelector.Width = 0
    End Sub

    Private Sub HandleCloseJob()
        'jobSelector.Width = CType(Me.Content, Canvas).ActualWidth
    End Sub

    'Public Sub OpenJobViewer()
    '    Dim currentDispatcher As System.Windows.Threading.Dispatcher = System.Windows.Application.Current.Dispatcher
    '    currentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, New OpenWindowDelegate(AddressOf _OpenJobViewer))

    '    'Dim currentDispatcher As System.Windows.Threading.Dispatcher = System.Windows.Application.Current.Dispatcher
    '    'currentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, New OpenWindowDelegate(AddressOf _OpenJobViewer))

    '    ''Dim TS As New ThreadStart(Function() currentDispatcher.Invoke(OpenWindowDelegate, Nothing))
    '    'Dim TS As New ThreadStart(System.Windows.Threading.DispatcherPriority.Normal, New OpenWindowDelegate(AddressOf _OpenJobViewer))

    '    'Dim T As New Thread(AddressOf _OpenJobViewer)
    '    'T.Start()
    'End Sub

    'Private Sub _OpenJobViewer()
    '    Dim jv As New JobViewer(CurrentJob)
    '    'jv.Show()
    '    jv.Show()
    'End Sub
    'Delegate Sub OpenWindowDelegate()

End Class
