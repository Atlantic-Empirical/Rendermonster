Partial Public Class ucWatermark
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub SetupWatermark(ByVal Width As UShort, ByVal Height As UShort)
        If Height < 71 Then Exit Sub
        If Width < 100 Then Exit Sub

        ' so the watermark does not overlap the border (if any) of its parent control. this allows for a 3px border on all sides
        Width -= 6
        Height -= 6

        Dim SMT As Image
        Dim RM As TextBlock

        Me.Width = Width
        Me.Height = Height

        ' Place a band of SMT logos
        For i As Integer = 0 To Width / 100
            SMT = GetSMTImage()
            Canvas.SetTop(SMT, CDbl(0.1 * Height))
            Canvas.SetLeft(SMT, CDbl(i * 100))
            LayoutRoot.Children.Add(SMT)
        Next

        ' Place a band of RENDERMONSTER
        For i As Integer = 0 To Width / 300
            RM = GetRMText()
            Canvas.SetTop(RM, CDbl(Height / 2 - 80))
            Canvas.SetLeft(RM, CDbl(i * 300))
            LayoutRoot.Children.Add(RM)
        Next

        ' Place a band of RENDERMONSTER
        For i As Integer = 0 To Width / 300
            RM = GetRMText()
            Canvas.SetTop(RM, CDbl(Height / 2 + 40))
            Canvas.SetLeft(RM, CDbl(i * 300))
            LayoutRoot.Children.Add(RM)
        Next

        ' Place a band of SMT logos
        For i As Integer = 0 To Width / 100
            SMT = GetSMTImage()
            Canvas.SetTop(SMT, CDbl((0.9 * Height) - 31))
            Canvas.SetLeft(SMT, CDbl(i * 100))
            LayoutRoot.Children.Add(SMT)
        Next

        LayoutRoot.IsHitTestVisible = False
        LayoutRoot.Clip = CanvasClipGeometerty
        'LayoutRoot.Background = New SolidColorBrush(Color.FromArgb(200, 0, 0, 255))
    End Sub

    Private Function GetSMTImage() As Image
        Dim out As New Image
        out.Width = 100
        out.Height = 31
        out.Source = ResourceHelper.GetBitmap("Graphics/Companies/SMT-Logo.png")
        out.Opacity = 0.1
        Return out
    End Function

    Private Function GetRMText() As TextBlock
        Dim out As New TextBlock
        out.Text = "RENDERMONSTER"
        Dim stream As System.IO.Stream = ResourceHelper.GetStream("Fonts/impact.ttf")
        Dim fs As FontSource = New FontSource(stream)
        'Dim fs As FontSource = ResourceHelper.GetFontSource("Resources/impact.ttf")
        out.FontSource = fs
        out.FontFamily = New FontFamily("Impact")
        out.FontSize = 40
        out.Opacity = 0.1
        out.FontWeight = FontWeights.Bold
        out.FontStyle = FontStyles.Italic
        Return out
    End Function

    Private ReadOnly Property CanvasClipGeometerty() As RectangleGeometry
        Get
            Dim out As New RectangleGeometry
            out.Rect = New Rect(0, 0, Width, Height)
            Return out
        End Get
    End Property

End Class
