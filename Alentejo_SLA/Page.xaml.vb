Partial Public Class RootPage
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        SetFonts(Me.LayoutRoot)
        cvUserControls.Visibility = Windows.Visibility.Collapsed 'the login routine will make it visible if the user is logged in.
    End Sub

    Private Sub RootPage_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        LoginTestRoutiene()
    End Sub

#Region "DETAIL"

    Public Sub LoadDetail(ByVal DetailPage As ePages)
        CloseDetail()
        Dim uc As UIElement = Nothing
        Select Case DetailPage
            Case ePages.LOGIN
                LoginPage = New ucLoginPanel()
                uc = LoginPage
            Case ePages.Home
                uc = New ucHome
            Case ePages.Signup
                uc = New ucSignup
            Case ePages.USER_Home
                'uc = New ucDeepZoomTesting
                uc = New ucUSER_Home
            Case ePages.USER_JobViewer
                uc = New ucJobViewer
            Case ePages.ACCOUNT_Main

            Case ePages.ACCOUNT_Billing
                uc = New ucUSER_Billing
            Case ePages.RENDER_SubmitJob
                uc = New ucUSER_SubmitRenderJob
                'uc = New ucUploadFiles
            Case ePages.USER_JobBrowser
                uc = New ucJobBrowser
        End Select
        If uc Is Nothing Then Exit Sub
        Me.cvContent.Children.Add(uc)
        If User IsNot Nothing Then
            Me.lblUsername.Text = User.FirstName & " " & User.LastName
            'Me.llLogout.Visibility = Windows.Visibility.Visible
            cvUserControls.Visibility = Windows.Visibility.Visible
            UpdateBalance()
            StartTimer()
        End If
    End Sub

    Public Sub CloseDetail()
        Me.cvContent.Children.Clear()
    End Sub

#End Region 'DETAIL

#Region "LOGIN"

    Private WithEvents LoginPage As ucLoginPanel

    Private Sub LoginTestRoutiene()
        'SetCursor(Me, Cursors.Wait)
        If User IsNot Nothing Then

            AddHandler AuthServiceClient.LoginCompleted, AddressOf Service_LoginCompleted
            AuthServiceClient.LoginAsync(User.Username, User.PasswordHash)

            'verify that the server agrees that they are logged in
            'AddHandler AuthServiceClient.IsLoggedInCompleted, AddressOf AuthService_IsLoggedInCompleted
            'AuthServiceClient.IsLoggedInAsync()
        Else
            LoadDetail(ePages.Home)
            'SetCursor(Me, Cursors.Arrow)
        End If
    End Sub

    Private Sub Service_LoginCompleted(ByVal sender As Object, ByVal e As AlentejoAuthService.LoginCompletedEventArgs)
        RemoveHandler AuthServiceClient.LoginCompleted, AddressOf Service_LoginCompleted
        If String.IsNullOrEmpty(e.Result) Then
            LoadDetail(ePages.LOGIN)
            LoginPage.Failure = True
            'SetCursor(Me, Cursors.Arrow)
        Else
            HandleLoginSucceeded(e.Result)
        End If
    End Sub

    'Private Sub LoginTestRoutiene()
    '    'SetCursor(Me, Cursors.Wait)
    '    If User IsNot Nothing Then
    '        AddHandler AuthServiceClient.LoginCompleted, AddressOf Service_LoginCompleted
    '        AuthServiceClient.LoginAsync(User.Username, User.PasswordHash, String.Empty, True)

    '        'verify that the server agrees that they are logged in
    '        'AddHandler AuthServiceClient.IsLoggedInCompleted, AddressOf AuthService_IsLoggedInCompleted
    '        'AuthServiceClient.IsLoggedInAsync()
    '    Else
    '        LoadDetail(ePages.Home)
    '        'SetCursor(Me, Cursors.Arrow)
    '    End If

    '    ' From when this was in App.xaml.vb
    '    'ByRef e As StartupEventArgs
    '    'If e.InitParams.ContainsKey("LoggedIn") Then
    '    '    Dim SuccessfulLogin As Boolean = e.InitParams("LoggedIn") = "True"
    '    '    If SuccessfulLogin Then
    '    '        AddHandler AuthServiceClient.IsLoggedInCompleted, AddressOf AuthService_IsLoggedInCompleted
    '    '        AuthServiceClient.IsLoggedInAsync()
    '    '    Else
    '    '        ShowLogin()
    '    '        LoginPage.Failure = True
    '    '    End If
    '    'Else
    '    '    ShowLogin()
    '    'End If

    'End Sub

    'Private Sub Service_LoginCompleted(ByVal sender As Object, ByVal e As AuthService.LoginCompletedEventArgs)
    '    RemoveHandler AuthServiceClient.LoginCompleted, AddressOf Service_LoginCompleted
    '    If e.Result Then
    '        HandleLoginSucceeded()
    '    Else
    '        LoadDetail(ePages.LOGIN)
    '        LoginPage.Failure = True
    '        'SetCursor(Me, Cursors.Arrow)
    '    End If
    'End Sub

    Private Sub HandleLoginSucceeded(ByVal SessionId As String) Handles LoginPage.evLogin
        UpdateBalance()
        StartTimer()
        CurrentSessionId = SessionId
        LoginPage = Nothing
        LoadDetail(ePages.USER_Home)
        Me.lblUsername.Text = User.FirstName & " " & User.LastName
        'Me.llLogout.Visibility = Windows.Visibility.Visible
        cvUserControls.Visibility = Windows.Visibility.Visible
        'SetCursor(Me, Cursors.Arrow)
    End Sub

    Public Sub ShowBigWait()
        cvWaitAnimation.Children.Add(New ucWaitAnimation)
        Canvas.SetZIndex(cvWaitAnimation, 5000)
    End Sub

    Public Sub HideBigWait()
        cvWaitAnimation.Children.Clear()
        Canvas.SetZIndex(cvWaitAnimation, -5000)
    End Sub

    Private Sub llLogout_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles llLogout.MouseLeftButtonUp
        User = Nothing
        LoadDetail(ePages.Home)
        Me.lblUsername.Text = ""
        'Me.llLogout.Visibility = Windows.Visibility.Collapsed
        cvUserControls.Visibility = Windows.Visibility.Collapsed
    End Sub

#End Region 'LOGIN

#Region "TIMER"

    Private WithEvents THREE_MINUTE_TIMER As System.Windows.Threading.DispatcherTimer

    Private Sub StartTimer()
        THREE_MINUTE_TIMER = New System.Windows.Threading.DispatcherTimer
        THREE_MINUTE_TIMER.Interval = New TimeSpan(0, 3, 0)
        THREE_MINUTE_TIMER.Start()
    End Sub

    Private Sub KillJobUpdateTimer()
        THREE_MINUTE_TIMER.Stop()
        THREE_MINUTE_TIMER = Nothing
    End Sub

    Private Sub THREE_MINUTE_TIMER_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles THREE_MINUTE_TIMER.Tick
        UpdateBalance()
    End Sub

#End Region 'TIMER

#Region "BALANCE CHECK"

    Private Sub UpdateBalance()
        AddHandler AuthServiceClient.GetUserBalanceCompleted, AddressOf GetUserBalance_Completed
        AuthServiceClient.GetUserBalanceAsync("", User.UserId)
    End Sub

    Private Sub GetUserBalance_Completed(ByVal sender As Object, ByVal e As AlentejoAuthService.GetUserBalanceCompletedEventArgs)
        RemoveHandler AuthServiceClient.GetUserBalanceCompleted, AddressOf GetUserBalance_Completed
        Me.lblBalance.Text = "€" & Charges_FriendlyString(e.Result)
        CurrentBalance = e.Result
        cvAccountBalance_MouseLeave(Me, Nothing)
    End Sub

#End Region 'BALANCE CHECK

    Private Sub imgRM_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgRM.MouseLeftButtonDown
        If User IsNot Nothing Then
            LoadDetail(ePages.User_Home)
        Else
            LoadDetail(ePages.Home)
        End If
    End Sub

    Private Sub imgSMT_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgSMT.MouseLeftButtonUp
        System.Windows.Browser.HtmlPage.Window.Navigate(New Uri("http://www.seqmt.com"))
    End Sub

    Private Sub cvAccountBalance_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvAccountBalance.MouseEnter
        lblBalance.Foreground = New SolidColorBrush(Colors.Blue)
    End Sub

    Private Sub cvAccountBalance_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvAccountBalance.MouseLeave
        If CurrentBalance < 10 Then
            Me.lblBalance.Foreground = New SolidColorBrush(Colors.Red)
        Else
            Me.lblBalance.Foreground = New SolidColorBrush(Color.FromArgb(&HFF, &H42, &H42, &H42))
        End If
    End Sub

    Private Sub cvAccountBtn_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvAccountBtn.MouseEnter
        lblAccount.Foreground = New SolidColorBrush(Colors.Blue)
    End Sub

    Private Sub cvAccountBtn_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvAccountBtn.MouseLeave
        lblAccount.Foreground = New SolidColorBrush(Color.FromArgb(&HFF, &H42, &H42, &H42))
    End Sub

    Private Sub cvSupportBtn_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvSupportBtn.MouseEnter
        lblSupport.Foreground = New SolidColorBrush(Colors.Blue)
    End Sub

    Private Sub cvSupportBtn_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvSupportBtn.MouseLeave
        lblSupport.Foreground = New SolidColorBrush(Color.FromArgb(&HFF, &H42, &H42, &H42))
    End Sub

    Private Sub cvAccountBtn_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles cvAccountBtn.MouseLeftButtonUp
        LoadDetail(ePages.ACCOUNT_Main)
    End Sub

    Private Sub cvAccountBalance_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles cvAccountBalance.MouseLeftButtonUp
        LoadDetail(ePages.ACCOUNT_Billing)
    End Sub

    Private Sub cvSupportBtn_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles cvSupportBtn.MouseLeftButtonUp
        LoadDetail(ePages.SUPPORT_Main)
    End Sub

End Class
