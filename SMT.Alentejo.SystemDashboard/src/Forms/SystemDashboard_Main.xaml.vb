Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.Alentejo.Core.UserManagement

Class SystemDashboard_Main

#Region "USERS"

    Private Sub userSelector_evUserSelected(ByVal Uid As String) Handles userSelector.evUserSelected
        DisplayUserViewer()
    End Sub

    Private Sub DisplayUserViewer()
        userSelector.Width = 0
        userViewer.UpdateUserViewer()
    End Sub

    Private Sub CloseUser() Handles userViewer.evCloseUser
        userSelector.Width = CType(tpUserActivity.Content, Canvas).ActualWidth
    End Sub

#End Region 'USERS

    Private Sub tpPayPal_RequestBringIntoView(ByVal sender As Object, ByVal e As System.Windows.RequestBringIntoViewEventArgs) Handles tpPayPal.RequestBringIntoView
        'PayPal.UpdatePayPalData()
    End Sub

End Class
