Partial Public Class ucDeepZoomViewer
    Inherits UserControl

#Region "PROPERTIES"

    Private _zoom As Double = 1
    Private duringDrag As Boolean = False
    Private mouseDown As Boolean = False
    Private lastMouseDownPos As New Point()
    Private lastMousePos As New Point()
    Private lastMouseViewPort As New Point()
    Private WithEvents mwh As MouseWheelHelper
    Private XMLUrl As String

    Public Property ZoomFactor() As Double
        Get
            Return _zoom
        End Get
        Set(ByVal value As Double)
            _zoom = value
        End Set
    End Property

    Public Property CloseIsHidden() As Boolean
        Get
            Return cvClose.Visibility = Windows.Visibility.Collapsed
        End Get
        Set(ByVal value As Boolean)
            If value Then
                Me.cvClose.Visibility = Windows.Visibility.Collapsed
            Else
                Me.cvClose.Visibility = Windows.Visibility.Visible
            End If
        End Set
    End Property

#End Region 'PROPERTIES

#Region "EVENTS"

    Public Event evDeepZoomLoadSuccessful()
    Public Event evImageOpenFailed()
    Public Event evImageFailed()

#End Region 'EVENTS

#Region "CONSTRUCTOR"

    Public Sub New()
        InitializeComponent()
        mwh = New MouseWheelHelper(Me)
    End Sub

#End Region 'CONSTRUCTOR

#Region "FORM"

    Public Event evCloseMe()

    Private Sub CloseMe()
        RaiseEvent evCloseMe()
    End Sub

    Private OverCloseBox As Boolean = False

    Private Sub rtCLOSE_CLICK_CATCH_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles rtCLOSE_CLICK_CATCH.MouseEnter
        OverCloseBox = True
    End Sub

    Private Sub rtCLOSE_CLICK_CATCH_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles rtCLOSE_CLICK_CATCH.MouseLeave
        OverCloseBox = False
    End Sub

#End Region 'FORM

#Region "PUBLIC METHODS"

    Public Sub SetDeepZoomCompositionUrl(ByVal nXMLUrl As String)
        'XMLUrl = "../../../DeepZoomScratch/a8d51fd8-fbba-4a0c-bc69-e2751a233ec8/dd190a01-d17d-4198-b350-256a2e4dfce6/collection_images/dd190a01-d17d-4198-b350-256a2e4dfce6.xml"
        'XMLUrl = "../../../GeneratedImages/dzc_output.xml"
        'XMLUrl = "../../../DeepZoomScratch/testing/default.xml"
        XMLUrl = nXMLUrl
        Dim u As New Uri(XMLUrl, UriKind.Relative)
        msi.Source = New DeepZoomImageTileSource(u)
    End Sub

#End Region 'PUBLIC METHODS

#Region "MSI EVENTS"

    Private Sub msi_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles msi.Loaded
        'Me.bdMain.Width = msi.Width
        'Me.bdMain.Height = msi.Height
        'MinWidth="930" MaxWidth="930" Width="930" MinHeight="475" MaxHeight="475" Height="475"
    End Sub

    Private Sub msi_ImageOpenSucceeded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles msi.ImageOpenSucceeded
        DebugWrite("ImageOpenSucceeded")
        If CurrentBalance < 100 Then PlaceWatermark()
        msi.ViewportWidth = 1
        RaiseEvent evDeepZoomLoadSuccessful()
    End Sub

    Private Sub msi_ImageOpenFailed(ByVal sender As Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles msi.ImageOpenFailed
        DebugWrite("ImageOpenFailed")
        RaiseEvent evImageOpenFailed()
    End Sub

    Private Sub msi_ImageFailed(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles msi.ImageFailed
        DebugWrite("ImageFailed")
        RaiseEvent evImageFailed()
    End Sub

    'Private Sub msi_MotionFinished(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles msi.MotionFinished
    '    DebugWrite("MotionFinished")
    'End Sub

    'Private Sub msi_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles msi.MouseEnter
    '    ' If animation springs were turned off, then 
    '    ' turn them back on 
    '    If msi.UseSprings = False Then
    '        msi.UseSprings = True
    '    End If

    '    ' The ZoomAboutLogicalPoint method allows you to zoom and pan 
    '    ' in the same step. The first parameter is the zoom (3x) and the 
    '    ' third and fourth parameters are the respective x and y coordinates 
    '    ' of the logical point to zoom around. 
    '    Me.msi.ZoomAboutLogicalPoint(3, 0.5, 0.5)
    'End Sub

    'Private Sub msi_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles msi.MouseLeave
    '    Dim zoom As Double = 1
    '    zoom = zoom / 3
    '    ' This time the zoom is reversed (1/3) although the pan 
    '    ' remains the same - zoom back out from the middle. 
    '    Me.msi.ZoomAboutLogicalPoint(zoom, 0.5, 0.5)
    'End Sub

    'Private Sub msi_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles msi.MouseLeftButtonUp
    '    Me.msi.Source = Nothing
    '    Me.msi.Visibility = Windows.Visibility.Collapsed
    'End Sub

    'use the following to create open hand, closed hand cursors for moving the image around
    '    Private Sub msi_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles msi.MouseEnter
    '        Cursor = Cursors.Hand
    '    End Sub

    '    Private Sub msi_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles msi.MouseLeftButtonDown
    'Cursor = Cursors.
    '    End Sub

    '    Private Sub msi_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles msi.MouseLeftButtonUp

    '    End Sub

    '    Private Sub msi_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles msi.MouseLeave

    '    End Sub

#End Region 'MSI EVENTS

#Region "PAGE EVENTS"

    Private Sub ucDeepZoomViewer_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        lastMouseDownPos = e.GetPosition(msi)
        lastMouseViewPort = msi.ViewportOrigin

        mouseDown = True

        msi.CaptureMouse()
    End Sub

    Private Sub ucDeepZoomViewer_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        If OverCloseBox Then
            CloseMe()
            Exit Sub
        End If
        If Not duringDrag Then
            Dim shiftDown As Boolean = (Keyboard.Modifiers And ModifierKeys.Shift) = ModifierKeys.Shift
            Dim newzoom As Double = _zoom

            If shiftDown Then
                newzoom /= 2
            Else
                newzoom *= 2
            End If

            Zoom(newzoom, msi.ElementToLogicalPoint(Me.lastMousePos))
        End If
        duringDrag = False
        mouseDown = False

        msi.ReleaseMouseCapture()
    End Sub

    Private Sub ucDeepZoomViewer_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseMove
        'lastMousePos = e.GetPosition(msi)

        'If duringDrag Then
        '    Dim newPoint As Point = lastMouseViewPort
        '    newPoint.X += (lastMouseDownPos.X - lastMousePos.X) / msi.ActualWidth * msi.ViewportWidth
        '    newPoint.Y += (lastMouseDownPos.Y - lastMousePos.Y) / msi.ActualWidth * msi.ViewportWidth
        '    msi.ViewportOrigin = newPoint
        'End If

        lastMousePos = e.GetPosition(msi)
        If mouseDown AndAlso Not duringDrag Then
            duringDrag = True
            Dim w As Double = msi.ViewportWidth
            Dim o As New Point(msi.ViewportOrigin.X, msi.ViewportOrigin.Y)
            msi.UseSprings = False
            msi.ViewportOrigin = New Point(o.X, o.Y)
            msi.ViewportWidth = w
            _zoom = 1 / w
            msi.UseSprings = True
        End If

        If duringDrag Then
            Dim newPoint As Point = lastMouseViewPort
            newPoint.X += (lastMouseDownPos.X - lastMousePos.X) / msi.ActualWidth * msi.ViewportWidth
            newPoint.Y += (lastMouseDownPos.Y - lastMousePos.Y) / msi.ActualWidth * msi.ViewportWidth
            msi.ViewportOrigin = newPoint
        End If

    End Sub

#End Region 'PAGE EVENTS

#Region "MOUSE WHEEL"

    Private Sub mwh_Moved(ByVal sender As Object, ByVal e As MouseWheelEventArgs) Handles mwh.Moved
        e.Handled = True

        Dim newzoom As Double = _zoom

        If e.Delta < 0 Then
            newzoom /= 1.3
        Else
            newzoom *= 1.3
        End If

        Zoom(newzoom, msi.ElementToLogicalPoint(Me.lastMousePos))
        msi.CaptureMouse()
    End Sub

#End Region 'MOUSE WHEEL

#Region "ZOOMING"

    Private Sub Zoom(ByVal newzoom As Double, ByVal p As Point)
        If newzoom < 0.01 Then newzoom = 0.01
        msi.ZoomAboutLogicalPoint(newzoom / _zoom, p.X, p.Y)
        _zoom = newzoom
    End Sub

    Private Sub ZoomInClick(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Zoom(_zoom * 1.3, msi.ElementToLogicalPoint(New Point(0.5 * msi.ActualWidth, 0.5 * msi.ActualHeight)))
    End Sub

    Private Sub ZoomOutClick(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Zoom(_zoom / 1.3, msi.ElementToLogicalPoint(New Point(0.5 * msi.ActualWidth, 0.5 * msi.ActualHeight)))
    End Sub

    Private Sub GoHomeClick(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        Me.msi.ViewportWidth = 1
        Me.msi.ViewportOrigin = New Point(0, 0)
        ZoomFactor = 1
    End Sub

    Private Sub GoFullScreenClick(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs)
        If Not Application.Current.Host.Content.IsFullScreen Then
            Application.Current.Host.Content.IsFullScreen = True
        Else
            Application.Current.Host.Content.IsFullScreen = False
        End If
    End Sub

#End Region 'ZOOMING

#Region "MOVIE EVENTS"

    Private Sub EnterMovie(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
        VisualStateManager.GoToState(Me, "FadeIn", True)
    End Sub

    Private Sub LeaveMovie(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs)
        VisualStateManager.GoToState(Me, "FadeOut", True)
    End Sub

#End Region 'MOVIE EVENTS

#Region "WATERMARK"

    Private Sub PlaceWatermark()
        Dim WMC As New ucWatermark
        WMC.SetupWatermark(Width, Height)
        Canvas.SetLeft(WMC, CDbl(0))
        Canvas.SetTop(WMC, CDbl(0))
        Canvas.SetZIndex(WMC, CDbl(0))
        LayoutRoot.Children.Add(WMC)
    End Sub

#End Region 'WATERMARK

#Region "UNUSED METHODS"

    ' unused functions that show the inner math of Deep Zoom 
    Public Function getImageRect() As Rect
        Return New Rect(-msi.ViewportOrigin.X / msi.ViewportWidth, -msi.ViewportOrigin.Y / msi.ViewportWidth, 1 / msi.ViewportWidth, 1 / msi.ViewportWidth * msi.AspectRatio)
    End Function

    Public Function ZoomAboutPoint(ByVal img As Rect, ByVal zAmount As Double, ByVal pt As Point) As Rect
        Return New Rect(pt.X + (img.X - pt.X) / zAmount, pt.Y + (img.Y - pt.Y) / zAmount, img.Width / zAmount, img.Height / zAmount)
    End Function

    Public Sub LayoutDZI(ByVal rect As Rect)
        Dim ar As Double = msi.AspectRatio
        msi.ViewportWidth = 1 / rect.Width
        msi.ViewportOrigin = New Point(-rect.Left / rect.Width, -rect.Top / rect.Width)
    End Sub

#End Region 'UNUSED METHODS

End Class
