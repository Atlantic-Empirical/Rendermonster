Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports Cooper.Silverlight.Controls

Partial Public Class ucJobBrowser 
	Inherits UserControl

#Region "PROPERTIES"

    Private Jobs() As AlentejoJobService.cSMT_ATJ_LiteJobInfo

#End Region 'PROPERTIES

#Region "EVENTS"

    Public Event evViewJob(ByRef JobId As String)

#End Region 'EVENTS

#Region "CONSTRUCTOR"

    Public Sub New()
        ' Required to initialize variables
        InitializeComponent()

        ScrollViewerMouseWheelSupport.Initialize(LayoutRoot)
        svJobs.AddMouseWheelSupport()

        ' SETUP FONTS
        lblDateFilter.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Std)
        lblStatusFilter.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Std)
        lblHeader.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Std)
        cbDateRange.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Std)
        cbStatusFilter.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Std)
    End Sub

#End Region 'CONSTRUCTOR

#Region "FORM"

    Private Sub ucJobBrowser_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Try
            UpdateTable()
        Catch ex As Exception

        End Try
    End Sub

#End Region 'FORM

#Region "FILTERS"

    Public Property UserFilter() As String
        Get

        End Get
        Set(ByVal value As String)

        End Set
    End Property

    Public Property StartDate() As DateTime
        Get
            Return _StartDate
        End Get
        Set(ByVal value As DateTime)
            _StartDate = value
        End Set
    End Property
    Private _StartDate As DateTime

    Public Property EndDate() As DateTime
        Get
            Return _EndDate
        End Get
        Set(ByVal value As DateTime)
            _EndDate = value
        End Set
    End Property
    Private _EndDate As DateTime

    Public Property JobStatus() As String
        Get
            Return _JobStatus
        End Get
        Set(ByVal value As String)
            _JobStatus = value
        End Set
    End Property
    Private _JobStatus As String

#End Region 'FILTERS

#Region "JOB DATA RETRIEVAL"

    Private Mutex_QueryJobInfo As Boolean = False

    Private Sub GetJobs()
        If Mutex_QueryJobInfo Then Exit Sub
        Mutex_QueryJobInfo = True
        Me.cvWaitAnimation.Children.Add(New ucWaitAnimation)
        Dim Q As New AlentejoJobService.cSMT_ATJ_JobInfoQuery
        Q.DateRange = Me.cbDateRange.SelectedIndex
        Q.UserFilter = User.UserId
        Q.StatusFilter = Me.cbStatusFilter.SelectedIndex
        AddHandler JobServiceClient.QueryJobInfoCompleted, AddressOf QueryJobInfo_Callback
        JobServiceClient.QueryJobInfoAsync("", Q)
    End Sub

    Private Sub QueryJobInfo_Callback(ByVal sender As Object, ByVal e As AlentejoJobService.QueryJobInfoCompletedEventArgs)
        RemoveHandler JobServiceClient.QueryJobInfoCompleted, AddressOf QueryJobInfo_Callback
        Mutex_QueryJobInfo = False
        Me.cvWaitAnimation.Children.Clear()
        If e.Result Is Nothing Then
            'SetCursor(Me, Cursors.Arrow)
        Else
            Jobs = e.Result
            RenderTable()
        End If
    End Sub

#End Region 'JOB DATA RETRIEVAL

#Region "TABLE RENDERING"

    Public Sub UpdateTable()
        'SetCursor(Me, Cursors.Wait)
        GetJobs()
    End Sub

    Private Sub RenderTable()
        Try
            spJobs.Children.Clear()
            Dim el As elJobItemRow
            For Each LJI As AlentejoJobService.cSMT_ATJ_LiteJobInfo In Jobs
                el = New elJobItemRow(LJI)
                AddHandler el.evClicked, AddressOf HandleJobClick
                AddHandler el.evViewLatestImage, AddressOf HandleJobImageViewClick
                spJobs.Children.Add(el)
            Next
            'SetCursor(Me, Cursors.Arrow)
        Catch ex As Exception
#If DEBUG Then
            MessageBox.Show("Unable to load jobs. Error: " & ex.Message & vbNewLine & vbNewLine & ex.StackTrace)
#Else
            MessageBox.Show("A problem has occurred.")
#End If
            OpenDetail(Me, ePages.USER_Home)
        End Try
    End Sub

#End Region 'TABLE RENDERING

#Region "UI EVENTS"

    Private Sub HandleJobClick(ByRef JobId As String)
        CurrentJobId = JobId
        OpenDetail(Me, ePages.USER_JobViewer)
        'RaiseEvent evViewJob(JobId)
    End Sub

    Private Sub HandleJobImageViewClick(ByRef JobId As String)
        ViewImageJobId = JobId
        AddHandler JobServiceClient.GetDeepZoopXMLPathForJobCompleted, AddressOf GetLatestImageDeepZoopXMLPathAsync_Callback
        JobServiceClient.GetDeepZoopXMLPathForJobAsync(JobId, True, "")
    End Sub

    Private Sub GetLatestImageDeepZoopXMLPathAsync_Callback(ByVal sender As Object, ByVal e As AlentejoJobService.GetDeepZoopXMLPathForJobCompletedEventArgs)
        RemoveHandler JobServiceClient.GetDeepZoopXMLPathForJobCompleted, AddressOf GetLatestImageDeepZoopXMLPathAsync_Callback
        If e.Result Is Nothing Then Exit Sub
        If InStr(e.Result.ToLower, "downloading") Or InStr(e.Result.ToLower, "failure") Then
            DebugWrite("Trying to get deepzoom again.")
            System.Threading.Thread.Sleep(1000)
            HandleJobImageViewClick(ViewImageJobId)
        Else
            ViewImage(e.Result)
        End If
    End Sub

    Private Sub btnUpdateTable_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUpdateTable.Click
        UpdateTable()
    End Sub

#End Region 'UI EVENTS

#Region "IMAGE VIEWER"

    Private ViewImageJobId As String

    Public Sub ViewImage(ByVal Url As String)
        dzvMain.SetDeepZoomCompositionUrl(Url)
        dzvMain.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub dzvMain_evCloseJobViewer() Handles dzvMain.evCloseMe
        dzvMain.Visibility = Windows.Visibility.Collapsed
    End Sub

#End Region 'IMAGE VIEWER

End Class
