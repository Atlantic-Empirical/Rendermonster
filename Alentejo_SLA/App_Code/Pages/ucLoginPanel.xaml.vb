Partial Public Class ucLoginPanel
    Inherits UserControl

    Public Event evLogin(ByVal SessionId As String)

    Private ReadOnly Property PasswordHash() As String
        Get
            Return HashPassword(Password.Password)
        End Get
    End Property
    Public Property Failure() As Boolean
        Get
            Return _Failure
        End Get
        Set(ByVal value As Boolean)
            _Failure = value
            FailureMessage.Visibility = If(_Failure, Windows.Visibility.Visible, Windows.Visibility.Collapsed)
        End Set
    End Property
    Private _Failure As Boolean

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ucLoginPanel_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Input.KeyEventArgs) Handles Me.KeyUp
        If e.Key = Key.Enter Then
            Login()
        End If
    End Sub

    Private Sub ucLoginPanel_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Username.Focus()
    End Sub

    Private Sub ButtonLogin_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles ButtonLogin.Click
        Login()
    End Sub

    Private Sub Login()
        If Username.Text = "" OrElse Password.Password = "" Then Exit Sub
        'SetCursor(Me, Cursors.Wait)
        ShowBigWait()
        AddHandler AuthServiceClient.LoginCompleted, AddressOf Service_LoginCompleted
        AuthServiceClient.LoginAsync(Username.Text, PasswordHash)
    End Sub

    Private Sub Service_LoginCompleted(ByVal sender As Object, ByVal e As AlentejoAuthService.LoginCompletedEventArgs)
        RemoveHandler AuthServiceClient.LoginCompleted, AddressOf Service_LoginCompleted
        If String.IsNullOrEmpty(e.Result) Then
            Failure = True
            'SetCursor(Me, Cursors.Arrow)
            HideBigWait()
        Else
            CurrentSessionId = e.Result
            AddHandler AuthServiceClient.GetUserInfoLiteCompleted, AddressOf GetUserInfoLite_Completed
            AuthServiceClient.GetUserInfoLiteAsync(Username.Text)
        End If
    End Sub

    Private Sub GetUserInfoLite_Completed(ByVal sender As Object, ByVal e As AlentejoAuthService.GetUserInfoLiteCompletedEventArgs)
        RemoveHandler AuthServiceClient.GetUserInfoLiteCompleted, AddressOf GetUserInfoLite_Completed
        'SetCursor(Me, Cursors.Arrow)
        If e.Result Is Nothing Then
            MessageBox.Show("Problem logging in.")
        Else
            User = New cSMT_ATJ_ClientSideUserProfile(Username.Text, PasswordHash, e.Result.Id, e.Result.FirstName, e.Result.LastName)
            RaiseEvent evLogin(CurrentSessionId)
        End If
    End Sub

    Private Sub SomethingChanged(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Username.TextChanged, Password.PasswordChanged
        If Failure Then Failure = False ' reset the login failure message
    End Sub

End Class
