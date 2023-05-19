Imports SMT.Alentejo.Core.JobManagement

Partial Public Class ucJobViewerConsoleItem
    Inherits UserControl

    Private PM As cSMT_ATJ_JobProgressMessage

    Public Sub New(ByRef nPM As cSMT_ATJ_JobProgressMessage)
        InitializeComponent()
        PM = nPM
    End Sub

    Private Sub ucJobViewerConsoleItem_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        RenderProgressMessage(PM)
    End Sub

    Private Sub RenderProgressMessage(ByRef PM As cSMT_ATJ_JobProgressMessage)
        If PM Is Nothing Then Exit Sub

        Dim tb As TextBlock

        'Just a to-string
        tb = New TextBlock
        tb.Text = JobMessageToString(PM)
        tb.Foreground = New SolidColorBrush(Colors.White)
        SetPos(tb, 4, 2)
        cvContents.Children.Add(tb)

        ''STATUS
        'tb = New TextBlock
        'tb.Text = LJI.Status
        'tb.Foreground = New SolidColorBrush(Colors.White)
        'SetPos(tb, 200, 0)
        'cvContents.Children.Add(tb)

        ''SUBMITTED
        'tb = New TextBlock
        'tb.Text = New DateTime(CLng(LJI.Submitted)).ToString("ddd, dd MMM yy H:mm 'UTC'")
        'tb.Foreground = New SolidColorBrush(Colors.White)
        'SetPos(tb, 300, 0)
        'cvContents.Children.Add(tb)

        ''COMPLETED
        'tb = New TextBlock
        'tb.Text = New DateTime(CLng(LJI.Completed)).ToString("ddd, dd MMM yy H:mm 'UTC'")
        'tb.Foreground = New SolidColorBrush(Colors.White)
        'SetPos(tb, 475, 0)
        'cvContents.Children.Add(tb)

        ' ''VIEW LATEST IMAGE
        ''tb = New TextBlock
        ''tb.Foreground = New SolidColorBrush(Color.FromArgb(255, &HCC, &HCC, &HCC))
        ''tb.FontWeight = FontWeights.Bold
        ''tb.Cursor = Cursors.Hand
        ''AddHandler tb.MouseLeftButtonUp, AddressOf HandleViewLatestImage
        ''tb.Text = "View Latest Image"
        ''SetPos(tb, 650, 0)
        ''cvContents.Children.Add(tb)
    End Sub

    Private Shared Sub SetPos(ByRef e As UIElement, ByVal x As Integer, ByVal y As Integer)
        e.SetValue(Canvas.LeftProperty, CDbl(x))
        e.SetValue(Canvas.TopProperty, CDbl(y))
    End Sub

    Private Function JobMessageToString(ByRef m As cSMT_ATJ_JobProgressMessage) As String
        Select Case m._Type
            Case 1, 3
                Return m.Message
            Case 2
                'If(m.Benchmark > 0, " Benchmark=" & m.Benchmark, "") &
                Return "Sample Level=" & m.SampleLevel & If(Not String.IsNullOrEmpty(m.Message), " Elapsed=" & m.Message, "")
            Case Else
                Return ""
        End Select
    End Function

End Class
