Imports SMT.Alentejo.Core.UserManagement

Partial Public Class ucUserSelector

    Public Event evUserSelected(ByVal Uid As String)
    Public Event evCancel()

    Public SelectedUser As cSMT_ATJ_User_Lite
    Private Users As List(Of cSMT_ATJ_User_Lite)

    Public ReadOnly Property SelectedUserId() As String
        Get
            If SelectedUser Is Nothing Then Return ""
            Return SelectedUser.Id
        End Get
    End Property

    Private Sub btnDoIt_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDoIt.Click
        Try
            Users = Nothing
            lbUsers.Items.Clear()
            Dim UIQ As New cSMT_ATJ_UserInfoQuery
            UIQ.ActiveDateRange = Core.eSMT_ATJ_DateRange.All
            UIQ.Keyword = Me.txtKeyword.Text
            Users = GetUsersLite(UIQ)
            If Users Is Nothing OrElse Users.Count < 1 Then Exit Sub
            For Each u As cSMT_ATJ_User_Lite In Users
                lbUsers.Items.Add(u.FirstName & " " & u.LastName)
            Next
        Catch ex As Exception
            MsgBox("Problem occurred in running search. Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnSelectThisUser_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSelectThisUser.Click
        If SelectedUser Is Nothing Then Exit Sub
        CurrentUser = SelectedUser
        RaiseEvent evUserSelected(SelectedUser.Id)
    End Sub

    Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)
        RaiseEvent evCancel()
    End Sub

    Private Sub lbUsers_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbUsers.SelectionChanged
        If lbUsers.SelectedIndex < 0 Then Exit Sub
        SelectedUser = Users(lbUsers.SelectedIndex)
        tbCompany.Text = SelectedUser.Company
        tbFirstName.Text = SelectedUser.FirstName
        tbLastName.Text = SelectedUser.LastName
        tbUsername.Text = SelectedUser.Username
    End Sub

End Class
