Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports SMT.Alentejo_SLA.AlentejoJobService
Imports Cooper.Silverlight.Controls
Imports System.IO

Partial Public Class ucJobViewer 
	Inherits UserControl

#Region "PROPERTIES"

    Private ReadOnly Property JobId() As String
        Get
            Return CurrentJobId
        End Get
    End Property

    Public ReadOnly Property RenderJob() As cSMT_ATJ_RenderJob_Base
        Get
            Return _RenderJob
        End Get
    End Property
    Private _RenderJob As cSMT_ATJ_RenderJob_Base

    Private ReadOnly Property MaxwellJob() As cSMT_ATJ_RenderJob_Maxwell
        Get
            If RenderJob Is Nothing Then Return Nothing
            Return TryCast(RenderJob, cSMT_ATJ_RenderJob_Maxwell)
        End Get
    End Property

    Private GreyOutRect As Rectangle
    Private Disposed As Boolean = False
    Private StopUpdating As Boolean = False

#End Region 'PROPERTIES

#Region "EVENTS"

    Public Event evCloseJobViewer()

#End Region 'EVENTS

#Region "CONSTRUCTOR"

    Public Sub New()
        ' Required to initialize variables
        InitializeComponent()
        ScrollViewerMouseWheelSupport.Initialize(cvLayoutRoot)
        svConsole.AddMouseWheelSupport()
        SetFonts(Me.cvLayoutRoot)
    End Sub

#End Region 'CONSTRUCTOR

#Region "FORM"

    Private Sub ucJobViewer_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If CurrentBalance < 100 Then PlaceWatermark()
        StartAnimationTimer()
        StartJobUpdateTimer()
        RetrieveJob()
    End Sub

    Private Sub Close()
        Disposed = True
        CurrentJobId = ""
        KillAnimationTimer()
        'RaiseEvent evCloseJobViewer()
        OpenDetail(Me, ePages.USER_JobBrowser)
    End Sub

    Private Sub rtCLOSE_CLICK_CATCH_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles rtCLOSE_CLICK_CATCH.MouseLeftButtonUp
        Close()
    End Sub

#End Region 'FORM

#Region "JOB RETRIEVAL"

    Private Mutex_GetJob As Boolean = False

    Private Sub RetrieveJob()
        If Mutex_GetJob Then Exit Sub
        DebugWrite("RetrieveJob()")
        Mutex_GetJob = True
        AddHandler JobServiceClient.GetJobByIdCompleted, AddressOf GetJobById_Callback
        JobServiceClient.GetJobByIdAsync("", JobId)
    End Sub

    Private Sub GetJobById_Callback(ByVal sender As Object, ByVal e As GetJobByIdCompletedEventArgs)
        If Disposed Then Exit Sub
        RemoveHandler JobServiceClient.GetJobByIdCompleted, AddressOf GetJobById_Callback
        Mutex_GetJob = False
        If e.Result Is Nothing Then
            'error
        Else
            _RenderJob = e.Result
            RefreshProgress()
            LoadLatestImage()
            RefreshConsole()
            RefreshFileList()
            RefreshStats()
            'SetupRenderControls()
        End If
    End Sub

#End Region 'JOB RETRIEVAL

#Region "TIMERS"

#Region "TIMERS:JOB UPDATE"

    Private WithEvents JOB_UPDATE_TIMER As System.Windows.Threading.DispatcherTimer

    Private Sub StartJobUpdateTimer()
        JOB_UPDATE_TIMER = New System.Windows.Threading.DispatcherTimer
        JOB_UPDATE_TIMER.Interval = New TimeSpan(10000000 * 10)
        JOB_UPDATE_TIMER.Start()
    End Sub

    Private Sub KillJobUpdateTimer()
        JOB_UPDATE_TIMER.Stop()
        JOB_UPDATE_TIMER = Nothing
    End Sub

    Private Sub JOB_UPDATE_TIMER_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles JOB_UPDATE_TIMER.Tick
        If StopUpdating Then
            JOB_UPDATE_TIMER.Stop()
        Else
            RetrieveJob()
        End If
    End Sub

#End Region 'TIMERS:JOB UPDATE

#Region "TIMERS:ANIMATION"

    Private WithEvents ANIMATION_TIMER As System.Windows.Threading.DispatcherTimer

    Private Sub StartAnimationTimer()
        ANIMATION_TIMER = New System.Windows.Threading.DispatcherTimer
        ANIMATION_TIMER.Interval = New TimeSpan(10000000 / 20) '20 fps
        ANIMATION_TIMER.Start()
    End Sub

    Private Sub KillAnimationTimer()
        ANIMATION_TIMER.Stop()
        ANIMATION_TIMER = Nothing
    End Sub

    Private Sub ANIMATION_TIMER_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles ANIMATION_TIMER.Tick
        AnimateProgress()
    End Sub

#End Region 'TIMERS:ANIMATION

#End Region 'TIMERS

#Region "LATEST IMAGE"

    Private LatestImage As cSMT_ATJ_File
    Private Mutex_LatestImage As Boolean = False

    Private Sub LoadLatestImage()
        If Mutex_LatestImage Then Exit Sub
        DebugWrite("LoadLatestImage()")
        Mutex_LatestImage = True
        'If LatestImage IsNot Nothing And RenderJob.Status = eSMT_ATJ_RenderJob_Status.COMPLETED Then Exit Sub
        AddHandler JobServiceClient.GetLatestImageCompleted, AddressOf GetLatestImage_Callback
        JobServiceClient.GetLatestImageAsync(JobId, If(LatestImage Is Nothing, "", LatestImage.Id))
    End Sub

    Private Sub GetLatestImage_Callback(ByVal sender As Object, ByVal e As GetLatestImageCompletedEventArgs)
        If Disposed Then Exit Sub
        RemoveHandler JobServiceClient.GetLatestImageCompleted, AddressOf GetLatestImage_Callback
        DebugWrite("GetLatestImage_Callback()")
        Mutex_LatestImage = False
        If e.Result Is Nothing Then
            'error or already have latest
        Else
            If String.IsNullOrEmpty(e.Result.Url) Then Exit Sub
            'DebugWrite("Loading image = " & e.Result)
            'Canvas.SetZIndex(cvImageWaitAnimation, CDbl(1))
            Me.cvImageWaitAnimation.Children.Add(New ucWaitAnimation)
            Dim u As New Uri(e.Result.Url, UriKind.Absolute)
            LatestImage = e.Result

            Dim bi As New Imaging.BitmapImage
            DebugWrite("attempting to display image at " & u.ToString)
            bi.UriSource = u
            AddHandler bi.DownloadProgress, AddressOf ImageDisplayCallback
            Me.imgSample.Stretch = Stretch.Uniform
            Me.imgSample.Source = bi
            Me.imgSample.Cursor = Cursors.Hand
            Me.imgSample.Visibility = Windows.Visibility.Visible
        End If
    End Sub

    Private Sub imgSample_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles imgSample.MouseLeftButtonUp
        DisplayImageBrowser()
    End Sub

    Private WithEvents ImageBrowser As ucImageBrowser

    Private Sub DisplayImageBrowser()
        AddHandler JobServiceClient.GetImageFileInfoForJobCompleted, AddressOf GetImageFileInfoForJob_Callback
        'SetCursor(Me, Cursors.Wait)
        JobServiceClient.GetImageFileInfoForJobAsync(JobId)
    End Sub

    Private Sub GetImageFileInfoForJob_Callback(ByVal sender As Object, ByVal e As GetImageFileInfoForJobCompletedEventArgs)
        If Disposed Then Exit Sub
        RemoveHandler JobServiceClient.GetImageFileInfoForJobCompleted, AddressOf GetImageFileInfoForJob_Callback
        'SetCursor(Me, Cursors.Arrow)
        If e.Result Is Nothing OrElse e.Result.Length = 0 Then Exit Sub
        ImageBrowser = New ucImageBrowser(JobId, e.Result)
        cvLayoutRoot.Children.Add(ImageBrowser)
    End Sub

    Private Sub CloseImageBrowser()
        cvLayoutRoot.Children.Remove(ImageBrowser)
    End Sub

    Private Sub HandleCloseImageBrowser() Handles ImageBrowser.evCloseMe
        CloseImageBrowser()
    End Sub

    Private Sub PlaceWatermark()
        Dim WMC As New ucWatermark
        WMC.SetupWatermark(bdImageSample.Width, bdImageSample.Height)
        Canvas.SetLeft(WMC, CDbl(0))
        Canvas.SetTop(WMC, CDbl(28))
        cvLayoutRoot.Children.Add(WMC)
    End Sub

    Private Sub ImageDisplayCallback(ByVal sender As Object, ByVal e As System.Windows.Media.Imaging.DownloadProgressEventArgs)
        If Disposed Then Exit Sub
        If e.Progress = 100 Then
            cvImageWaitAnimation.Children.Clear()
            'Canvas.SetZIndex(cvImageWaitAnimation, CDbl(-1))
            lblImageFileName.Text = LatestImage.FileName
        End If
    End Sub

    Private Sub imgSample_ImageFailed(ByVal sender As Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles imgSample.ImageFailed
        cvImageWaitAnimation.Children.Clear()
        'MessageBox.Show("This image could not be displayed." & vbNewLine & vbNewLine & "Likely cause: The render engine did not finish outputting this image file before moving on to the next level.")
        DebugWrite("This image could not be displayed." & vbNewLine & vbNewLine & "Likely cause: The render engine did not finish outputting this image file before moving on to the next level.")
    End Sub

#End Region 'LATEST IMAGE

#Region "PROGRESS"

    Private Mutex_RefreshProgress As Boolean = False

    Private Sub RefreshProgress()
        If Mutex_RefreshProgress Then Exit Sub
        Mutex_RefreshProgress = True
        AddHandler JobServiceClient.GetJobStatusCompleted, AddressOf RefreshProgress_Callback
        JobServiceClient.GetJobStatusAsync("", JobId)
    End Sub

    Private Sub RefreshProgress_Callback(ByVal sender As Object, ByVal e As GetJobStatusCompletedEventArgs)
        If Disposed Then Exit Sub
        RemoveHandler JobServiceClient.GetJobStatusCompleted, AddressOf RefreshProgress_Callback
        Mutex_RefreshProgress = False
        If 1 = 2 Then
            'error or no files
        Else
            SetJobStatus(e.Result)
        End If
    End Sub

    Private Sub SetJobStatus(ByVal Status As Integer)
        Select Case Status
            Case Is < 0
                StopUpdating = True
                Me.lblProgress.Text = "FAILURE"
                'FAILURE
                Select Case CType(_RenderJob.Status, eSMT_ATJ_RenderJob_Status)
                    Case eSMT_ATJ_RenderJob_Status.Failure_FileTransferToS3, eSMT_ATJ_RenderJob_Status.Failure_Generic, eSMT_ATJ_RenderJob_Status.Failure_RenderNodeCoordination, eSMT_ATJ_RenderJob_Status.Failure_FileTransferFromS3
                        SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Failed)
                        SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Not_Started)
                        SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                        SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

                    Case eSMT_ATJ_RenderJob_Status.Failure_RunMXCL
                        SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Failed)
                        SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                        SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

                    Case eSMT_ATJ_RenderJob_Status.Failure_RenderJobFinalization
                        SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Failed)
                        SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

                End Select

            Case 100 To 199 'PRE PROCESSING
                Me.lblProgress.Text = "PRE-PROCESSING"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Running)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Not_Started)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

            Case 200 To 299 'RENDERING
                Me.lblProgress.Text = "RENDERING"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Running)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

            Case 300 To 399 'POST PROCESSING
                Me.lblProgress.Text = "POST-PROCESSING"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Running)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

            Case 400 'COMPLETED
                StopUpdating = True
                Me.lblProgress.Text = "COMPLETED"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Completed)

            Case 500 'ARCHIVED
                StopUpdating = True
                Me.lblProgress.Text = "ARCHIVED"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Archived)

            Case eSMT_ATJ_RenderJob_Status.NOT_INDICATED

        End Select

        Select Case Status
            Case Is < 0, 400, 500
                hlTerminateResubmitRender.Content = "Resubmit"
            Case Else
                hlTerminateResubmitRender.Content = "Terminate"
        End Select
    End Sub

    Private Sub SetProgressElementStatus(ByRef el As FrameworkElement, ByRef Status As eProgressElementStatus)
        Dim ell As Ellipse = CType(el, Ellipse)
        Dim rt As New RotateTransform

        'COLOR
        Select Case Status
            Case eProgressElementStatus.Not_Started
                ell.Fill = New SolidColorBrush(Colors.Yellow)
                rt.Angle = 90
                ell.RenderTransform = rt
            Case eProgressElementStatus.Running
                ell.Fill = New SolidColorBrush(Colors.Blue)
            Case eProgressElementStatus.Completed
                ell.Fill = New SolidColorBrush(Colors.Green)
                rt.Angle = 0
                ell.RenderTransform = rt
            Case eProgressElementStatus.Failed
                ell.Fill = New SolidColorBrush(Colors.Red)
                rt.Angle = -45
                ell.RenderTransform = rt
            Case eProgressElementStatus.Archived
                ell.Fill = New SolidColorBrush(Colors.Gray)
                rt.Angle = 0
                ell.RenderTransform = rt
        End Select

        'ANIMATION
        Select Case Status
            Case eProgressElementStatus.Running
                'make sure it is being animated
                If elCurrent IsNot ell Then
                    elCurrent = ell
                    elCurrentAngle = 90
                End If
            Case Else
                'make sure it is not being animated
                If elCurrent Is ell Then
                    elCurrent = Nothing
                End If
        End Select
    End Sub

    Private Enum eProgressElementStatus
        Not_Started
        Running
        Completed
        Failed
        Archived
    End Enum

#Region "PROGRESS:ANIMATION"

    Private elCurrent As Ellipse
    Private elCurrentAngle As Double

    Private Sub AnimateProgress()
        If elCurrent Is Nothing Then Exit Sub
        Dim rt As New RotateTransform
        elCurrentAngle += 1
        rt.Angle = elCurrentAngle
        elCurrent.RenderTransform = rt
    End Sub

#End Region 'PROGRESS:ANIMATION

#End Region 'PROGRESS

#Region "CONSOLE"

    Private ProgressItems As List(Of cSMT_ATJ_JobProgressMessage)
    Private ReadOnly Property LatestProgressItemTicks() As String
        Get
            If ProgressItems Is Nothing OrElse ProgressItems.Count = 0 Then
                Return ""
            Else
                Return CStr(CLng(ProgressItems.Item(ProgressItems.Count - 1).Timestamp) + 1)
            End If
        End Get
    End Property
    Private Mutex_RefreshConsole As Boolean = False
    Private ConsoleItemCount As UShort = 0

    Private Sub RefreshConsole()
        If Mutex_RefreshConsole Then Exit Sub
        DebugWrite("RefreshConsole() - Latest progress item ticks = " & LatestProgressItemTicks)
        Mutex_RefreshConsole = True
        'If ProgressItems IsNot Nothing And RenderJob.Status = eSMT_ATJ_RenderJob_Status.COMPLETED Then Exit Sub
        AddHandler JobServiceClient.GetProgressDataCompleted, AddressOf GetProgressData_Callback
        JobServiceClient.GetProgressDataAsync("", JobId, LatestProgressItemTicks)
        Me.cvConsoleWaitAnimation.Children.Add(New ucWaitAnimation)
    End Sub

    Private Sub GetProgressData_Callback(ByVal sender As Object, ByVal e As GetProgressDataCompletedEventArgs)
        If Disposed Then Exit Sub
        DebugWrite("GetProgressData_Callback()")
        RemoveHandler JobServiceClient.GetProgressDataCompleted, AddressOf GetProgressData_Callback
        Mutex_RefreshConsole = False
        Me.cvConsoleWaitAnimation.Children.Clear()
        Try
            If e.Result Is Nothing Then
                DebugWrite("Failed to retrieve console data.")
            Else
                If ProgressItems Is Nothing Then ProgressItems = New List(Of cSMT_ATJ_JobProgressMessage)
                ProgressItems.AddRange(e.Result)
                RenderConsoleItems()
            End If
        Catch ex As Exception
            MessageBox.Show("Problem with RefreshConsole(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub RenderConsoleItems()
        If ProgressItems Is Nothing Then Exit Sub
        If ConsoleItemCount = ProgressItems.Count Then
            Exit Sub
        Else
            ConsoleItemCount = ProgressItems.Count
        End If

        Me.spConsole.Children.Clear()
        For Each pi As cSMT_ATJ_JobProgressMessage In ProgressItems
            Me.spConsole.Children.Add(New ucJobViewerConsoleItem(pi))
            If Not String.IsNullOrEmpty(pi.Benchmark) Then Me.lblSTAT_Benchmark.Text = pi.Benchmark
            If Not String.IsNullOrEmpty(pi.SampleLevel) Then Me.lblSTAT_SampleLevel.Text = pi.SampleLevel & " / " & MaxwellJob.SampleCount
        Next
        Me.svConsole.UpdateLayout()
        Me.svConsole.ScrollToVerticalOffset(Double.MaxValue)
    End Sub

#End Region 'CONSOLE

#Region "FILES"

    Private Files As cSMT_ATJ_File()
    Private Mutex_RefreshFileList As Boolean = False
    Private MXILocalFileStream As Stream

    Private Sub RefreshFileList()
        If Mutex_RefreshFileList Then Exit Sub
        Mutex_RefreshFileList = True
        DebugWrite("RefreshFileList()")
        AddHandler JobServiceClient.GetFileListCompleted, AddressOf GetFileList_Callback
        JobServiceClient.GetFileListAsync("", JobId)
        cvFilesWaitAnimation.Children.Add(New ucWaitAnimation)
    End Sub

    Private Sub GetFileList_Callback(ByVal sender As Object, ByVal e As GetFileListCompletedEventArgs)
        If Disposed Then Exit Sub
        DebugWrite("GetFileList_Callback()")
        RemoveHandler JobServiceClient.GetFileListCompleted, AddressOf GetFileList_Callback
        Mutex_RefreshFileList = False
        cvFilesWaitAnimation.Children.Clear()
        Me.lbFILE_FileList.Items.Clear()
        If e.Result Is Nothing OrElse e.Result.Length = 0 Then
            'error or no files
        Else
            Dim SelectedFileIndex As Integer = lbFILE_FileList.SelectedIndex
            Files = e.Result
            Dim tTb As TextBlock
            For Each f As cSMT_ATJ_File In Files
                tTb = New TextBlock
                tTb.Text = f.FileName
                tTb.FontWeight = FontWeights.Bold
                tTb.FontSize = CDbl(12)
                tTb.Margin = New Thickness(0.0, 0.0, 0.0, 0.0)
                If String.IsNullOrEmpty(f.IsAvailable) Then
                    tTb.Foreground = New SolidColorBrush(Colors.Red)
                Else
                    tTb.Foreground = New SolidColorBrush(Color.FromArgb(&HFF, &H4B, &H4D, &HBA))
                End If
                lbFILE_FileList.Items.Add(tTb)
            Next
            lbFILE_FileList.SelectedIndex = SelectedFileIndex
        End If
    End Sub

    Private Sub lbFILE_FileList_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbFILE_FileList.SelectionChanged
        'If InStr(CType(lbFILE_FileList.SelectedItem, TextBlock).Text.ToLower, ".mxi") Then
        '    Dim f As cSMT_ATJ_File = Files(lbFILE_FileList.SelectedIndex)
        '    RemoveHandler hlDownloadSelectedFile.Click, AddressOf hlDownloadSelectedFile_Click
        '    hlDownloadSelectedFile.NavigateUri = New Uri(f.Url, UriKind.Absolute)
        '    hlDownloadSelectedFile.TargetName = "_blank"
        'Else
        '    AddHandler hlDownloadSelectedFile.Click, AddressOf hlDownloadSelectedFile_Click
        '    hlDownloadSelectedFile.NavigateUri = Nothing
        'End If
    End Sub

    Private Sub hlDownloadSelectedFile_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles hlDownloadSelectedFile.Click
        Dim f As cSMT_ATJ_File = Files(lbFILE_FileList.SelectedIndex)

        If f.Type = eAtjFileType.MXI Then
            DownloadFile_DirectFromS3(f.Url)

            'Dim sfd As New SaveFileDialog
            'sfd.DefaultExt = ".mxi"
            'sfd.Filter = "Maxwell MXI (*.mxi)|*.mxi"
            'If sfd.ShowDialog = True Then
            '    MXILocalFileStream = sfd.OpenFile

            '    Dim myHttpWebRequest1 As HttpWebRequest = DirectCast(HttpWebRequest.Create(New Uri(f.Url)), HttpWebRequest)
            '    'Dim myRequestState As New RequestState()
            '    'myRequestState.request = myHttpWebRequest1

            '    ' Start the asynchronous request. 
            '    Dim result As IAsyncResult = DirectCast(myHttpWebRequest1.BeginGetResponse(New AsyncCallback(AddressOf MXIReadCallback), IAsyncResult)

            '    ' Release the HttpWebResponse resource. 
            '    'myRequestState.response.Close()

            '    'request.BeginGetResponse(New AsyncCallback(AddressOf MXIReadCallback), request)
            'End If

        Else
            DownloadFile(f.Id)
        End If

    End Sub

    'Private BUFFER_SIZE As Integer = 102400

    'Private Sub MXIReadCallback(ByVal asynchronousResult As IAsyncResult)
    '    Dim request As HttpWebRequest = DirectCast(asynchronousResult.AsyncState, HttpWebRequest)
    '    Dim response As HttpWebResponse = DirectCast(request.EndGetResponse(asynchronousResult), HttpWebResponse)
    '    Dim dlstream As Stream = response.GetResponseStream
    '    Dim b(BUFFER_SIZE) As Byte
    '    Dim asynchronousInputRead As IAsyncResult = dlstream.BeginRead(b, 0, BUFFER_SIZE, New AsyncCallback(AddressOf MXIReadCallback), Nothing)


    '    'Using dlStream As Stream = response.GetResponseStream()

    '    '    Dim b(102400) As Byte

    '    '    While dlStream.Position < (dlStream.Length - b.Length)
    '    '        dlStream.Read(b, 0, b.Length)
    '    '        MXILocalFileStream.Write(b, 0, b.Length)
    '    '    End While

    '    '    'get last bytes
    '    '    ReDim b(dlStream.Length - dlStream.Position)
    '    '    dlStream.Read(b, 0, b.Length)
    '    '    MXILocalFileStream.Write(b, 0, b.Length)

    '    '    dlStream.Close()
    '    '    MXILocalFileStream.Close()

    '    'End Using
    'End Sub

    'Private Sub MXIReadCallback(ByVal asyncResult As IAsyncResult)
    '    Try

    '        'Dim myRequestState As RequestState = DirectCast(asyncResult.AsyncState, RequestState)
    '        Dim responseStream As Stream = myRequestState.streamResponse
    '        Dim read As Integer = responseStream.EndRead(asyncResult)
    '        ' Read the HTML page and then do something with it 
    '        If read > 0 Then
    '            myRequestState.requestData.Append(Encoding.UTF8.GetString(myRequestState.BufferRead, 0, read))
    '            Dim asynchronousResult As IAsyncResult = responseStream.BeginRead(myRequestState.BufferRead, 0, BUFFER_SIZE, New AsyncCallback(ReadCallBack), myRequestState)
    '        Else
    '            If myRequestState.requestData.Length > 1 Then
    '                Dim stringContent As String
    '                ' do something with the response stream here 
    '                stringContent = myRequestState.requestData.ToString()
    '            End If

    '            responseStream.Close()

    '            allDone.[Set]()

    '        End If
    '    Catch e As WebException
    '        ' Need to handle the exception 

    '    End Try
    'End Sub

#End Region 'FILES

#Region "STATS"

    Private Sub RefreshStats()

        ' STATIC
        Me.lblJobName.Text = "JobName: " & RenderJob.Name
        Me.lblSTAT_Cores.Text = RenderJob.CoresRequested
        Me.lblSTAT_CameraName.Text = MaxwellJob.ActiveCamera
        Dim StartTime As New DateTime(RenderJob.Submitted_Ticks, DateTimeKind.Utc)
        Me.lblSTAT_Time_Start.Text = StartTime.ToString("r")
        Me.lblSTAT_JobFile.Text = RenderJob.JobFileNames(0)
        Me.lblSTAT_Resolution.Text = RenderJob.Width & " x " & RenderJob.Height

        ' DYNAMIC
        Dim ts As TimeSpan
        If RenderJob.Status = eSMT_ATJ_RenderJob_Status.COMPLETED Then
            If Not String.IsNullOrEmpty(RenderJob.Completed_Ticks) Then
                Dim EndTime As New DateTime(RenderJob.Completed_Ticks, DateTimeKind.Utc)
                Me.lblSTAT_Time_End.Text = EndTime.ToString("r")
                ts = New TimeSpan(EndTime.Ticks - StartTime.Ticks)
                Me.lblSTAT_Time_Elapsed.Text = Math.Round(ts.TotalMinutes, 0) & " min"
            End If
        Else
            ts = New TimeSpan(DateTime.UtcNow.Ticks - StartTime.Ticks)
            Me.lblSTAT_Time_Elapsed.Text = If(ts.TotalMinutes < 0, "0", Math.Round(ts.TotalMinutes, 0).ToString) & " min / " & MaxwellJob.MaxDuration & " min"
        End If
        Me.lblSTAT_Cost_Current.Text = "€" & Charges_FriendlyString()

        'are updated by the console update code
        'Me.lblSTAT_Benchmark.Tex t = ""
        'Me.lblSTAT_SampleLevel.Text = ""
    End Sub

    Private ReadOnly Property Charges_FriendlyString() As String
        Get
            If String.IsNullOrEmpty(RenderJob.Charges) Then Return "0.00"
            If InStr(RenderJob.Charges, ".") = 0 Then
                Return RenderJob.Charges & ".00"
            Else
                Dim s() As String = Split(RenderJob.Charges, ".")
                If s(1).Length < 2 Then
                    Return RenderJob.Charges & "0"
                Else
                    Return RenderJob.Charges
                End If
            End If
        End Get
    End Property

    'Private Function DummyCalculateCost(ByRef ts As TimeSpan) As String
    '    Dim SETUP_COST As Byte = 5
    '    Dim HOURLY_COST_PER_CORE As Double = 2
    '    Dim HOURLY_COST As Double = HOURLY_COST_PER_CORE * RenderJob.CoresRequested
    '    Dim CostSoFar As Double = SETUP_COST + (HOURLY_COST * ts.TotalHours)
    '    Dim CostSoFarStr As String = CStr(Math.Round(CostSoFar, 2))
    '    Dim s() As String = Split(CostSoFarStr, ".")
    '    CostSoFarStr = s(0) & "." & If(s.Length = 1, "00", s(1).PadRight(2, "0"))
    '    Dim MaxCostStr As String = If(RenderJob.MaxCost > 0, " / €" & RenderJob.MaxCost, "")
    '    Return "€" & CostSoFarStr & MaxCostStr
    'End Function

#End Region 'STATS

#Region "RENDER CONTROL"

    'Private Sub SetupRenderControls()
    '    If RenderJob.Status = eSMT_ATJ_RenderJob_Status.COMPLETED Then
    '        hlTerminateResubmitRender.Content = "Resubmit"
    '    Else
    '        hlTerminateResubmitRender.Content = "Terminate"
    '    End If
    'End Sub

    Private Sub hlStop_Resubmit_Render_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles hlTerminateResubmitRender.Click
        If RenderJob.Status = eSMT_ATJ_RenderJob_Status.COMPLETED Then
            JobForResubmit = RenderJob
            OpenDetail(Me, ePages.RENDER_SubmitJob)
        Else
            JobServiceClient.TerminateJobAsync("", RenderJob.Id)
        End If
    End Sub

    Private WithEvents ClonedJobNameDlg As InputBox_dlg

    Private Sub hlCloneRender_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles hlCloneRender.Click
        GreyOutRect = GreyOutControl(Me.cvLayoutRoot)
        ClonedJobNameDlg = New InputBox_dlg("Cloned job name:")
        Canvas.SetTop(ClonedJobNameDlg, CDbl(200))
        Canvas.SetLeft(ClonedJobNameDlg, CDbl(400))
        Canvas.SetZIndex(ClonedJobNameDlg, 200)
        Me.cvLayoutRoot.Children.Add(ClonedJobNameDlg)
    End Sub

    Private Sub CloneRender(ByVal NewName As String) Handles ClonedJobNameDlg.evOK
        If String.IsNullOrEmpty(NewName) Then
            MessageBox.Show("Invalid name.")
        Else
            JobForResubmit = RenderJob
            JobForResubmit.Name = NewName
            JobForResubmit.Id = Guid.NewGuid.ToString
            OpenDetail(Me, ePages.RENDER_SubmitJob)
            GrayOutClear(Me.cvLayoutRoot, GreyOutRect)
        End If
    End Sub

    Private Sub CloseClonedJobNameDlg() Handles ClonedJobNameDlg.evCancel
        Me.cvLayoutRoot.Children.Remove(ClonedJobNameDlg)
        GrayOutClear(Me.cvLayoutRoot, GreyOutRect)
    End Sub

#End Region 'RENDER CONTROL

#Region "EXPANDERS"

    Private Sub spExpanders_LayoutUpdated(ByVal sender As Object, ByVal e As System.EventArgs) Handles spExpanders.LayoutUpdated
        SetFileListSize()
    End Sub

    Private Sub SetupExpanders()
        SetFileListSize()
    End Sub

    Private Sub exRenderInfo_Expanded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles exRenderInfo.Expanded
        Me.exRenderInfo.Height = bdRenderInfo.ActualHeight + 30
        SetFileListSize(True)
    End Sub

    Private Sub exRenderInfo_Collapsed(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles exRenderInfo.Collapsed
        Me.exRenderInfo.Height = CDbl(23)
        SetFileListSize()
    End Sub

    Private Sub exFileList_Expanded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles exFileList.Expanded
        SetFileListSize()
    End Sub

    Private Sub SetFileListSize(Optional ByVal ForceRenderInfoIsExpanded As Boolean = False)
        If spExpanders.ActualHeight = 0 Then Exit Sub
        Dim spH As Double = Me.spExpanders.ActualHeight
        Dim bdFileListCanvasTop As Double = Canvas.GetTop(bdFileList)
        Dim exFileListHeader As Double = 23
        Dim hlDownloadHeight As Double = hlDownloadSelectedFile.ActualHeight + 4
        Dim otherExpandersTotalHeight As Double = 0
        If ForceRenderInfoIsExpanded Then
            otherExpandersTotalHeight += bdRenderInfo.ActualHeight + 30
        Else
            otherExpandersTotalHeight += If(Me.exRenderInfo.IsExpanded, Me.exRenderInfo.ActualHeight, 23)
        End If
        Me.bdFileList.Height = CDbl(spH - otherExpandersTotalHeight - exFileListHeader - bdFileListCanvasTop - hlDownloadHeight - 5)
        Canvas.SetTop(hlDownloadSelectedFile, CDbl(bdFileListCanvasTop + bdFileList.Height + 6))
    End Sub

#End Region 'EXPANDERS

End Class
