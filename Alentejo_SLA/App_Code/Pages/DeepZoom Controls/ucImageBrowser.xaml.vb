Imports SMT.Alentejo_SLA.AlentejoJobService

Partial Public Class ucImageBrowser
    Inherits UserControl

    Public Event evCloseMe()

    Private JobId As String
    Private Files() As cSMT_ATJ_File
    Private Index As Integer = 0
    Private DeepZoomXMLUrl As String

    Private GetUrlPurpose As eGetDownloadReason

    Private ReadOnly Property CurrentImageFileId() As String
        Get
            Return CurrentFile.Id
        End Get
    End Property

    Private ReadOnly Property CurrentFile() As cSMT_ATJ_File
        Get
            Return Files(Index)
        End Get
    End Property

    Public Sub New(ByVal nJobId As String, ByRef nFiles() As cSMT_ATJ_File)
        InitializeComponent()
        JobId = nJobId
        Files = nFiles
        Array.Reverse(Files)
        DrawDots(Files.Length)
        dzvMain.CloseIsHidden = True
        If CurrentBalance < 100 Then PlaceWatermark()
        UpdateImage()
    End Sub

    Private Sub UpdateImage()
        'DebugWrite(Guids(Index))
        imgMain.Source = Nothing
        SetDot(Index)
        'waitAnimation.Visibility = Windows.Visibility.Visible
        ''SetCursor(Me, Cursors.Wait)
        'Me.Cursor = Cursors.Wait
        Waiting = True
        If Index = 0 Then
            AddHandler JobServiceClient.GetDeepZoopXMLPathForJobCompleted, AddressOf GetLatestImageDeepZoopXMLPathAsync_Callback
            JobServiceClient.GetDeepZoopXMLPathForJobAsync(JobId, False, Files(Index).Id)
            Me.imgMain.Visibility = Windows.Visibility.Collapsed
            Me.dzvMain.Visibility = Windows.Visibility.Visible

            GetUrlPurpose = eGetDownloadReason.ForDownloadOnly
            AddHandler JobServiceClient.GetImageUrlForGuidCompleted, AddressOf GetImageUrl_Callback
            JobServiceClient.GetImageUrlForGuidAsync(JobId, CurrentImageFileId)
        Else
            GetUrlPurpose = eGetDownloadReason.ForDisplayAndDownload
            AddHandler JobServiceClient.GetImageUrlForGuidCompleted, AddressOf GetImageUrl_Callback
            JobServiceClient.GetImageUrlForGuidAsync(JobId, CurrentImageFileId)
            Me.imgMain.Visibility = Windows.Visibility.Visible
            Me.dzvMain.Visibility = Windows.Visibility.Collapsed
        End If
        If Index = Files.Length - 1 Then
            cvLeftArrows.Visibility = Windows.Visibility.Collapsed
        Else
            cvLeftArrows.Visibility = Windows.Visibility.Visible
        End If
        If Index = 0 Then
            cvRightArrows.Visibility = Windows.Visibility.Collapsed
        Else
            cvRightArrows.Visibility = Windows.Visibility.Visible
        End If
        Me.lblImageLable.Text = If(Index = 0, "latest", Files.Length - Index & " of " & Files.Length)
        lblImageFileName.Text = CurrentFile.FileName
        lblImageResolution.Text = CurrentFile.Resolution
    End Sub

#Region "DEEPZOOM"

    Private Sub GetLatestImageDeepZoopXMLPathAsync_Callback(ByVal sender As Object, ByVal e As AlentejoJobService.GetDeepZoopXMLPathForJobCompletedEventArgs)
        RemoveHandler JobServiceClient.GetDeepZoopXMLPathForJobCompleted, AddressOf GetLatestImageDeepZoopXMLPathAsync_Callback
        If e.Result Is Nothing Then Exit Sub
        If InStr(e.Result.ToLower, "downloading") Or InStr(e.Result.ToLower, "failure") Then
            'need to try again
            DebugWrite("Trying to get deepzoom again.")
            System.Threading.Thread.Sleep(1000)
            UpdateImage()
        Else
            DebugWrite("Deepzoom xml=" & e.Result)
            DeepZoomXMLUrl = e.Result
            dzvMain.SetDeepZoomCompositionUrl(DeepZoomXMLUrl)
        End If
        ''SetCursor(Me, Cursors.Arrow)
        'Me.Cursor = Cursors.Arrow
    End Sub

    Private Sub dzvMain_evCloseJobViewer() Handles dzvMain.evCloseMe
        RaiseEvent evCloseMe()
    End Sub

    Private Sub dzvMain_evDeepZoomLoadSuccessful() Handles dzvMain.evDeepZoomLoadSuccessful
        'waitAnimation.Visibility = Windows.Visibility.Collapsed
        Waiting = False
    End Sub

    Private Sub dzvMain_evImageFailed() Handles dzvMain.evImageFailed
        Waiting = False
    End Sub

    Private Sub dzvMain_evImageOpenFailed() Handles dzvMain.evImageOpenFailed
        Waiting = False
    End Sub

#End Region 'DEEPZOOM

#Region "STATIC IMAGE"

    Private Sub GetImageUrl_Callback(ByVal sender As Object, ByVal e As GetImageUrlForGuidCompletedEventArgs)
        RemoveHandler JobServiceClient.GetImageUrlForGuidCompleted, AddressOf GetImageUrl_Callback
        ''SetCursor(Me, Cursors.Arrow)
        'Me.Cursor = Cursors.Arrow
        If String.IsNullOrEmpty(e.Result) Then
            'error 
        Else
            Dim u As New Uri(e.Result, UriKind.Absolute)
            If GetUrlPurpose = eGetDownloadReason.ForDisplayAndDownload Then
                Dim bi As New Imaging.BitmapImage
                bi.UriSource = u
                AddHandler bi.DownloadProgress, AddressOf ImageDisplayCallback
                imgMain.Stretch = Stretch.Uniform
                imgMain.Source = bi
            End If
            'hlDownload.NavigateUri = u
        End If
    End Sub

    Private Sub ImageDisplayCallback(ByVal sender As Object, ByVal e As System.Windows.Media.Imaging.DownloadProgressEventArgs)
        If e.Progress = 100 Then
            'waitAnimation.Visibility = Windows.Visibility.Collapsed
            'Me.Cursor = Cursors.Arrow
            Waiting = False
        End If
    End Sub

    Private Sub imgMain_ImageFailed(ByVal sender As Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles imgMain.ImageFailed
        Waiting = False
        MessageBox.Show("This image could not be displayed." & vbNewLine & vbNewLine & "Likely cause: The render engine did not finish outputting this image file before moving on to the next level.")
    End Sub

#End Region 'STATIC IMAGE

#Region "WAITING"

    Private Property Waiting() As Boolean
        Get
            Return _Waiting
        End Get
        Set(ByVal value As Boolean)
            _Waiting = value
            If _Waiting Then
                'Canvas.SetTop(waitAnimation, 206)
                waitAnimation.Visibility = Windows.Visibility.Visible
                ''SetCursor(Me, Cursors.Wait)
                'Me.Cursor = Cursors.Wait
            Else
                'Canvas.SetTop(waitAnimation, -1000)
                waitAnimation.Visibility = Windows.Visibility.Collapsed
                ''SetCursor(Me, Cursors.Arrow)
                'Me.Cursor = Cursors.Arrow
            End If
        End Set
    End Property
    Private _Waiting As Boolean = False

#End Region 'WAITING

#Region "ARROW MOUSE EVENTS"

    Private Sub ptLeftArrow_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ptLeftArrow.MouseEnter
        ptLeftArrow.Opacity = 0.5
    End Sub

    Private Sub ptLeftArrow_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ptLeftArrow.MouseLeave
        ptLeftArrow.Opacity = 1
    End Sub

    Private Sub ptLeftArrow_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ptLeftArrow.MouseLeftButtonUp
        If Index < Files.Length - 1 Then
            Index += 1
            UpdateImage()
        End If
    End Sub

    Private Sub ptRightArrow_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ptRightArrow.MouseEnter
        ptRightArrow.Opacity = 0.5
    End Sub

    Private Sub ptRightArrow_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles ptRightArrow.MouseLeave
        ptRightArrow.Opacity = 1
    End Sub

    Private Sub ptRightArrow_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles ptRightArrow.MouseLeftButtonUp
        If Index > 0 Then
            Index -= 1
            UpdateImage()
        End If
    End Sub

    Private Sub cvLeftArrowToEndBox_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvLeftArrowToEndBox.MouseEnter
        ptLeftArrow_ToEnd.Opacity = 0.5
    End Sub

    Private Sub cvLeftArrowToEndBox_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvLeftArrowToEndBox.MouseLeave
        ptLeftArrow_ToEnd.Opacity = 1
    End Sub

    Private Sub cvLeftArrowToEndBox_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles cvLeftArrowToEndBox.MouseLeftButtonUp
        Index = UBound(Files)
        UpdateImage()
    End Sub

    Private Sub cvRightArrowToEndBox_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvRightArrowToEndBox.MouseEnter
        ptRightArrow_ToEnd.Opacity = 0.5
    End Sub

    Private Sub cvRightArrowToEndBox_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles cvRightArrowToEndBox.MouseLeave
        ptRightArrow_ToEnd.Opacity = 1
    End Sub

    Private Sub cvRightArrowToEndBox_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles cvRightArrowToEndBox.MouseLeftButtonUp
        Index = 0
        UpdateImage()
    End Sub

#End Region 'ARROW MOUSE EVENTS

#Region "IMAGE DOWNLOAD"

    Private Sub hlDownload_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles hlDownload.Click
        DownloadFile(CurrentImageFileId)
    End Sub

#End Region 'IMAGE DOWNLOAD

#Region "DOTS"

    Private Sub DrawDots(ByVal Count As Byte)
        cvDots.Children.Clear()
        For b As Byte = 1 To Count
            cvDots.Children.Add(GetDot(b))
        Next
        cvDots.Width = Count * 10
    End Sub

    Private Sub SetDot(ByVal Index As Byte)
        Index = cvDots.Children.Count - 1 - Index
        For b As Byte = 0 To cvDots.Children.Count - 1
            If b = Index Then
                CType(cvDots.Children(b), Ellipse).Fill = New SolidColorBrush(Colors.White)
            Else
                CType(cvDots.Children(b), Ellipse).Fill = New SolidColorBrush(Color.FromArgb(255, &H88, &H88, &H88))
            End If
        Next
    End Sub

    Private Function GetDot(ByVal Index As Integer) As Ellipse
        Dim out As New Ellipse
        Canvas.SetLeft(out, CDbl((Index - 1) * 10))
        Canvas.SetTop(out, CDbl(0))
        out.Height = 6
        out.Width = 6
        out.Margin = New Thickness(2, 0, 2, 0)
        out.HorizontalAlignment = Windows.HorizontalAlignment.Center
        out.Fill = New SolidColorBrush(Color.FromArgb(255, &H88, &H88, &H88))
        Return out
    End Function

#End Region 'DOTS

#Region "WATERMARK"

    Private Sub PlaceWatermark()
        Dim WMC As New ucWatermark
        WMC.SetupWatermark(imgMain.Width, imgMain.Height)
        Canvas.SetLeft(WMC, CDbl(48))
        Canvas.SetTop(WMC, CDbl(3))
        LayoutRoot.Children.Add(WMC)
    End Sub

#End Region 'WATERMARK

    Private Enum eGetDownloadReason
        ForDisplayAndDownload
        ForDownloadOnly
    End Enum

    Private Sub rtCLOSE_CLICK_CATCH_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles rtCLOSE_CLICK_CATCH.MouseLeftButtonUp
        RaiseEvent evCloseMe()
    End Sub

End Class
