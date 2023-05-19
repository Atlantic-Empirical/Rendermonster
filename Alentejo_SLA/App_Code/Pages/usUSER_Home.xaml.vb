Imports System.Windows.Browser
Imports SMT.Alentejo_SLA.AlentejoJobService
Imports Cooper.Silverlight.Controls

Partial Public Class ucUSER_Home
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
        If User Is Nothing Then OpenDetail(Me, ePages.Home)
        'ScrollViewerMouseWheelSupport.Initialize(LayoutRoot)
        'lbRecentJobs.ScrollViewer.AddMouseWheelSupport()

        'Me.lblPassword.Text = User.PasswordHash
        'Me.hlDownloadFile.NavigateUri = New Uri(HtmlPage.Document.DocumentUri, "GetFile.zip?SessionHash=&FileId=b0375918-eb48-4327-80b4-aa9e89c3cc4b")
        'Me.hlDownloadFile.NavigateUri = New Uri(HtmlPage.Document.DocumentUri, "b0375918-eb48-4327-80b4-aa9e89c3cc4b.atjdl")
        'btnReviewJobs.FontFamily = GetFont(1, eAtjFont.HelveticaNeueExtended_Med)
        'btnStartJob.FontFamily = GetFont(1, eAtjFont.HelveticaNeueExtended_Med)
        SetFonts(Me.LayoutRoot)
    End Sub

    Private Sub ucUSER_Home_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        HideBigWait()
        LoadRecentJobs()
    End Sub

    Private Sub btnStartJob_Maxwell_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnStartJob.Click
        If CurrentBalance < 10 Then
            Me.btnStartJob.IsEnabled = False
            Me.btnStartJob.BorderBrush = New SolidColorBrush(Colors.Red)
            Me.btnStartJob.BorderThickness = New Thickness(2.0)
            MessageBox.Show("Your account balance is too low." & vbNewLine & "Please add credit then try again.")
        Else
            OpenDetail(Me, ePages.RENDER_SubmitJob)
        End If
    End Sub

    Private Sub btnReviewJobs_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnReviewJobs.Click
        'SetCursor(Me, Cursors.Wait)
        OpenDetail(Me, ePages.USER_JobBrowser)
    End Sub

    Private Sub hlDownloadSelectedFile_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles hlDownloadFile.Click
        'Dim ifDownloader As HtmlElement = HtmlPage.Document.GetElementById("ifDownloader")
        'ifDownloader.RemoveAttribute("src")
        'ifDownloader.SetAttribute("src", New Uri(HtmlPage.Document.DocumentUri, "FileUpload/FileRetrieval.ashx?SessionHash=&FileId=ffafe8d8-367e-41b0-a4be-f889579eecd8").AbsoluteUri)
        'DownloadFile("ffafe8d8-367e-41b0-a4be-f889579eecd8") 'mxs
        'DownloadFile("36dd6ea2-ad50-415a-be3a-724740f7533d") 'jpg
        'DownloadFile("b0375918-eb48-4327-80b4-aa9e89c3cc4b") 'mxi
        'OpenDetail(Me, ePages.Signup)
        GreyOutControl(Me.LayoutRoot)
    End Sub

    Private Sub lbRecentJobs_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lbRecentJobs.MouseLeftButtonUp
        Dim item As Object = CType(sender, ListBox).SelectedItem
        If item Is Nothing Then Exit Sub
        CurrentJobId = GetSelectedJobId(item.ToString)
        OpenDetail(Me, ePages.USER_JobViewer)
    End Sub

    Private Function GetSelectedJobId(ByRef JobName As String) As String
        For Each Job As cSMT_ATJ_LiteJobInfo In Jobs
            If Job.Name = JobName Then Return Job.Id
        Next
        Return ""
    End Function

    'Private Sub lbRecentJobs_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles lbRecentJobs.MouseMove
    '    Dim p As Point = e.GetPosition(sender)
    '    Dim hits As List(Of UIElement) = System.Windows.Media.VisualTreeHelper.FindElementsInHostCoordinates(p, sender)
    '    For Each hit As Control In hits
    '        DebugWrite(hit.Name)
    '    Next
    'End Sub

#Region "JOB DATA RETRIEVAL"

    Private Mutex_QueryJobInfo As Boolean = False
    Private Jobs() As cSMT_ATJ_LiteJobInfo

    Private Sub LoadRecentJobs()
        If Mutex_QueryJobInfo Then Exit Sub
        Mutex_QueryJobInfo = True
        Dim Q As New AlentejoJobService.cSMT_ATJ_JobInfoQuery
        Q.DateRange = 1
        Q.UserFilter = User.UserId
        Q.StatusFilter = 0
        AddHandler JobServiceClient.QueryJobInfoCompleted, AddressOf QueryJobInfo_Callback
        JobServiceClient.QueryJobInfoAsync("", Q)
        Me.cvImageWaitAnimation.Children.Add(New ucWaitAnimation)
    End Sub

    Private Sub QueryJobInfo_Callback(ByVal sender As Object, ByVal e As AlentejoJobService.QueryJobInfoCompletedEventArgs)
        RemoveHandler JobServiceClient.QueryJobInfoCompleted, AddressOf QueryJobInfo_Callback
        Mutex_QueryJobInfo = False
        If e.Result Is Nothing Then

        Else
            Me.lbRecentJobs.Items.Clear()
            Jobs = e.Result
            For Each j As cSMT_ATJ_LiteJobInfo In Jobs
                Me.lbRecentJobs.Items.Add(j.Name)
            Next
        End If
        'SetCursor(Me, Cursors.Arrow)
        Me.cvImageWaitAnimation.Children.Clear()
    End Sub

#End Region 'JOB DATA RETRIEVAL

End Class
