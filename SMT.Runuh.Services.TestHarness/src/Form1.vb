Public Class Form1

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SetupWebserviceClients()
    End Sub

#Region "SIGNUP/SIGNIN"

    Private Sub btnSignup_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSignup.Click

    End Sub

    Private Sub btnSignin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSignin.Click

    End Sub

    Private Sub btnSignout_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSignout.Click

    End Sub

    Private Sub txtUsername_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles txtUsername.LostFocus
        Dim b As Boolean
        'wscUserAPI.UsernameExists(Me.txtUsername.Text, b)
        AddHandler wscUserAPI.UsernameExistsCompleted, AddressOf UsernameExists_Callback
        wscUserAPI.UsernameExistsAsync(Me.txtUsername.Text, b)
    End Sub

    Private Sub UsernameExists_Callback(ByVal sender As Object, ByVal e As UserAPI.UsernameExistsCompletedEventArgs)
        RemoveHandler wscUserAPI.UsernameExistsCompleted, AddressOf UsernameExists_Callback
        If e.outExists Then
            Me.txtUsername.BackColor = Color.Coral
        Else
            Me.txtUsername.BackColor = Color.White
        End If
    End Sub

#End Region 'SIGNUP/SIGNIN

End Class
