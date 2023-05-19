Imports SMT.Alentejo_SLA.AlentejoJobService

Partial Public Class elJobItemRow
    Inherits UserControl

    Public LJI As AlentejoJobService.cSMT_ATJ_LiteJobInfo
    Public Event evClicked(ByRef JobId As String)
    Public Event evViewLatestImage(ByRef JobId As String)

    Public Sub New(ByRef nLji As AlentejoJobService.cSMT_ATJ_LiteJobInfo)
        InitializeComponent()
        LJI = nLji
    End Sub

    Private Sub elPurchaseItemRow_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If LJI Is Nothing Then Exit Sub

        Dim tb As TextBlock

        'NAME
        tb = New TextBlock
        tb.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Thin)
        tb.FontSize = CDbl(12)
        tb.Text = LJI.Name
        tb.Foreground = New SolidColorBrush(Colors.White)
        SetPos(tb, 4, 0)
        cvContents.Children.Add(tb)

        'STATUS
        tb = New TextBlock
        tb.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Thin)
        tb.FontSize = CDbl(12)
        tb.Text = LJI.Status_Public
        tb.Foreground = New SolidColorBrush(Colors.White)
        SetPos(tb, 200, 0)
        cvContents.Children.Add(tb)

        'SUBMITTED
        tb = New TextBlock
        tb.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Thin)
        tb.FontSize = CDbl(12)
        tb.Text = New DateTime(CLng(LJI.Submitted_Ticks), DateTimeKind.Utc).ToString("ddd, dd MMM yy H:mm 'UTC'")
        tb.Foreground = New SolidColorBrush(Colors.White)
        SetPos(tb, 320, 0)
        cvContents.Children.Add(tb)

        'COMPLETED
        tb = New TextBlock
        tb.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Thin)
        tb.FontSize = CDbl(12)
        tb.Text = New DateTime(CLng(LJI.Completed_Ticks), DateTimeKind.Utc).ToString("ddd, dd MMM yy H:mm 'UTC'")
        tb.Foreground = New SolidColorBrush(Colors.White)
        SetPos(tb, 495, 0)
        cvContents.Children.Add(tb)

        'VIEW LATEST IMAGE
        tb = New TextBlock
        tb.FontFamily = GetFont(2, eAtjFont.HelveticaNeueExtended_Thin)
        tb.FontSize = CDbl(12)
        tb.Foreground = New SolidColorBrush(Color.FromArgb(255, &HCC, &HCC, &HCC))
        tb.FontWeight = FontWeights.Bold
        tb.Cursor = Cursors.Hand
        AddHandler tb.MouseLeftButtonUp, AddressOf HandleViewLatestImage
        tb.Text = "View Latest Image"
        SetPos(tb, 670, 0)
        cvContents.Children.Add(tb)

    End Sub

    Private Shared Sub SetPos(ByRef e As UIElement, ByVal x As Integer, ByVal y As Integer)
        e.SetValue(Canvas.LeftProperty, CDbl(x))
        e.SetValue(Canvas.TopProperty, CDbl(y))
    End Sub

    Private Sub elJobItemRow_MouseEnter(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseEnter
        Me.cvContents.Background = New SolidColorBrush(Color.FromArgb(255, &H77, &H77, &H77))
        'Me.cvLayoutRoot.Background = New SolidColorBrush(Color.FromArgb(255, 160, 160, 220))
        Cursor = Cursors.Hand
    End Sub

    Private Sub elJobItemRow_MouseLeftButtonDown(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonDown
        Me.cvContents.Background = New SolidColorBrush(Color.FromArgb(255, &H99, &H99, &H99))
        'Me.cvLayoutRoot.Background = New SolidColorBrush(Color.FromArgb(255, 180, 180, 240))
    End Sub

    Private Sub elJobItemRow_MouseLeftButtonUp(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles Me.MouseLeftButtonUp
        Dim tb As TextBlock = TryCast(e.OriginalSource, TextBlock)
        If tb IsNot Nothing Then If tb.Text = "View Latest Image" Then Exit Sub
        Me.cvContents.Background = New SolidColorBrush(Color.FromArgb(255, &H77, &H77, &H77))
        'Me.cvLayoutRoot.Background = New SolidColorBrush(Color.FromArgb(255, 160, 160, 220))
        RaiseEvent evClicked(LJI.Id)
    End Sub

    Private Sub elJobItemRow_MouseLeave(ByVal sender As Object, ByVal e As System.Windows.Input.MouseEventArgs) Handles Me.MouseLeave
        Me.cvContents.Background = New SolidColorBrush(Color.FromArgb(255, &H55, &H55, &H55))
        'Me.cvLayoutRoot.Background = New SolidColorBrush(Color.FromArgb(255, 0, 0, &H8B))
        Cursor = Cursors.Arrow
    End Sub

    Private Sub HandleViewLatestImage(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs)
        RaiseEvent evViewLatestImage(LJI.Id)
    End Sub

End Class
