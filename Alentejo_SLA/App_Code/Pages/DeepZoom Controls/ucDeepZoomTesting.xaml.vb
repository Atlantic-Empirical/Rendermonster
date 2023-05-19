Imports System
Imports System.Collections.Generic
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Partial Public Class ucDeepZoomTesting
    Inherits UserControl

    Private zoom As Double = 1
    Private duringDrag As Boolean = False
    Private mouseDown As Boolean = False
    Private lastMouseDownPos As New Point()
    Private lastMousePos As New Point()
    Private lastMouseViewPort As New Point()

    Public Property ZoomFactor() As Double
        Get
            Return zoom
        End Get
        Set(ByVal value As Double)
            zoom = value
        End Set
    End Property

    Public Sub New()
        InitializeComponent()

        ' 
        ' Firing an event when the MultiScaleImage is Loaded 
        ' 
        AddHandler Me.msi.Loaded, AddressOf msi_Loaded

        ' 
        ' Firing an event when all of the images have been Loaded 
        ' 
        AddHandler Me.msi.ImageOpenSucceeded, AddressOf msi_ImageOpenSucceeded

    End Sub


    '    ' 
    '    ' Handling all of the mouse and keyboard functionality 
    '    ' 
    '        Me.MouseMove += Function(ByVal sender As Object, ByVal e As MouseEventArgs) Do 
    '    lastMousePos = e.GetPosition(msi)

    '    If duringDrag Then
    '        Dim newPoint As Point = lastMouseViewPort
    '        newPoint.X += (lastMouseDownPos.X - lastMousePos.X) / msi.ActualWidth * msi.ViewportWidth
    '        newPoint.Y += (lastMouseDownPos.Y - lastMousePos.Y) / msi.ActualWidth * msi.ViewportWidth
    '        msi.ViewportOrigin = newPoint
    '    End If
    'End Sub

    '        Me.MouseLeftButtonDown += Function(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Do 
    '            lastMouseDownPos = e.GetPosition(msi) 
    '            lastMouseViewPort = msi.ViewportOrigin 

    '            mouseDown = True 

    '            msi.CaptureMouse() 
    '        End Function 

    '        Me.MouseLeftButtonUp += Function(ByVal sender As Object, ByVal e As MouseButtonEventArgs) Do 
    '            If Not duringDrag Then 
    'Dim shiftDown As Boolean = (Keyboard.Modifiers And ModifierKeys.Shift) = ModifierKeys.Shift
    'Dim newzoom As Double = zoom

    '                If shiftDown Then 
    '                    newzoom /= 2 
    '                Else 
    '                    newzoom *= 2 
    '                End If 

    '                Zoom(newzoom, msi.ElementToLogicalPoint(Me.lastMousePos)) 
    '            End If 
    '            duringDrag = False 
    '            mouseDown = False 

    '            msi.ReleaseMouseCapture() 
    '        End Function 

    '        Me.MouseMove += Function(ByVal sender As Object, ByVal e As MouseEventArgs) Do 
    '            lastMousePos = e.GetPosition(msi) 
    '            If mouseDown AndAlso Not duringDrag Then 
    '                duringDrag = True 
    'Dim w As Double = msi.ViewportWidth
    'Dim o As New Point(msi.ViewportOrigin.X, msi.ViewportOrigin.Y)
    '                msi.UseSprings = False 
    '                msi.ViewportOrigin = New Point(o.X, o.Y) 
    '                msi.ViewportWidth = w 
    '                zoom = 1 / w 
    '                msi.UseSprings = True 
    '            End If 

    '            If duringDrag Then 
    'Dim newPoint As Point = lastMouseViewPort
    '                newPoint.X += (lastMouseDownPos.X - lastMousePos.X) / msi.ActualWidth * msi.ViewportWidth 
    '                newPoint.Y += (lastMouseDownPos.Y - lastMousePos.Y) / msi.ActualWidth * msi.ViewportWidth 
    '                msi.ViewportOrigin = newPoint 
    '            End If 
    '        End Function 

    '        New MouseWheelHelper(Me).Moved += Function(ByVal sender As Object, ByVal e As MouseWheelEventArgs) Do 
    '            e.Handled = True 

    'Dim newzoom As Double = zoom

    '            If e.Delta < 0 Then 
    '                newzoom /= 1.3 
    '            Else 
    '                newzoom *= 1.3 
    '            End If 

    '            Zoom(newzoom, msi.ElementToLogicalPoint(Me.lastMousePos)) 
    '            msi.CaptureMouse() 
    '        End Function 
    '    End Sub 

    Private Sub msi_ImageOpenSucceeded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        'If collection, this gets you a list of all of the MultiScaleSubImages 
        ' 
        'foreach (MultiScaleSubImage subImage in msi.SubImages) 
        '{ 
        ' // Do something 
        '} 

        msi.ViewportWidth = 1
    End Sub

    Private Sub msi_Loaded(ByVal sender As Object, ByVal e As RoutedEventArgs)
        ' Hook up any events you want when the image has successfully been opened 
    End Sub

    Private Sub ZoomSub(ByVal newzoom As Double, ByVal p As Point)
        If newzoom < 0.5 Then
            newzoom = 0.5
        End If

        msi.ZoomAboutLogicalPoint(newzoom / zoom, p.X, p.Y)
        zoom = newzoom
    End Sub

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

End Class
