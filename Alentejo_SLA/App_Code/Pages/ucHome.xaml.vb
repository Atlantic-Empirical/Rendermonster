Partial Public Class ucHome
    Inherits UserControl

    Public Sub New 
        InitializeComponent()
    End Sub

    Private Sub ucHome_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'Me.btnSignup.Visibility = Windows.Visibility.Collapsed
    End Sub

    Private Sub btnSignup_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSignup.Click
        OpenDetail(Me, ePages.Signup)
    End Sub

    Private Sub LoginPage_Login() Handles LoginControl.evLogin
        LoginControl = Nothing
        OpenDetail(Me, ePages.USER_Home)
        'SetCursor(Me, Cursors.Arrow)
    End Sub

End Class
