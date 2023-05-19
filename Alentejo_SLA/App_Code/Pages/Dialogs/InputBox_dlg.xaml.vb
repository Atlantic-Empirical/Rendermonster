Partial Public Class InputBox_dlg
    Inherits UserControl

    Public Event evOK(ByVal Value As String)
    Public Event evCancel()

    Public Sub New(ByVal Title As String)
        InitializeComponent()
        lblTitle.Text = Title
    End Sub

    Private Sub btnOK_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnOK.Click
        RaiseEvent evOK(txtInputBox.Text)
    End Sub

    Private Sub btnCancel_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
        RaiseEvent evCancel()
    End Sub

    'Private Sub InputBox_dlg_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
    '    LayoutRoot.Clip = CanvasClipGeometerty
    'End Sub

    'Private ReadOnly Property CanvasClipGeometerty() As RectangleGeometry
    '    Get
    '        Dim out As New RectangleGeometry
    '        out.Rect = New Rect(0, 0, Width, Height)
    '        out.RadiusX = 2
    '        out.RadiusY = 2
    '        Return out
    '    End Get
    'End Property

End Class
