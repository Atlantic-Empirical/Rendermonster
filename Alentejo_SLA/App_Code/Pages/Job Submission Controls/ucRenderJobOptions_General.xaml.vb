Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Partial Public Class ucRenderJobOptions_General 
	Inherits UserControl

	Public Sub New()
		' Required to initialize variables
		InitializeComponent()
	End Sub

    Private Sub txt_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtDuration.GotFocus, txtSampleCount.GotFocus
        CType(sender, TextBox).Text = ""
    End Sub

    Public Property MaxDuration() As UInt16
        Get
            Return If(IsNumeric(Me.txtDuration.Text), CType(Me.txtDuration.Text, UInt16), 360)
        End Get
        Set(ByVal value As UInt16)
            Me.txtDuration.Text = value
        End Set
    End Property

    Public Property SampleLevels() As Byte
        Get
            Return If(IsNumeric(Me.txtSampleCount.Text), CType(Me.txtSampleCount.Text, Byte), 25)
        End Get
        Set(ByVal value As Byte)
            Me.txtSampleCount.Text = value
        End Set
    End Property

    Public Property OutputImageFormat() As String
        Get
            Return "." & Me.cbOutputFormat.SelectedItem.ToString
        End Get
        Set(ByVal value As String)
            Me.cbOutputFormat.SelectedItem = value
        End Set
    End Property

    Public Property Resolution() As Size
        Get
            Dim w, h As UInt16
            w = If(IsNumeric(Me.txtResolution_W.Text), CType(Me.txtResolution_W.Text, UInt16), 650)
            h = If(IsNumeric(Me.txtResolution_H.Text), CType(Me.txtResolution_H.Text, UInt16), 400)
            Return New Size(w, h)
        End Get
        Set(ByVal value As Size)
            Me.txtResolution_W.Text = value.Width
            Me.txtResolution_H.Text = value.Height
        End Set
    End Property

    Private Sub txtDuration_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtDuration.LostFocus
        Dim tb As TextBox = CType(sender, TextBox)
        If Not IsNumeric(tb.Text) OrElse tb.Text < 0 Then
            tb.Text = 0
        Else
            If tb.Text > 720 Then
                MessageBox.Show("Please contact us for permission to run a render longer than 12 hours.")
                tb.Text = 360
            End If
        End If
    End Sub

    Private Sub txtSampleCount_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtSampleCount.LostFocus
        Dim tb As TextBox = CType(sender, TextBox)
        If Not IsNumeric(tb.Text) OrElse tb.Text < 1 Then
            tb.Text = 6
        Else
            If tb.Text > 256 Then
                MessageBox.Show("Maximum number of sample levels is 256.")
                tb.Text = 25
            End If
        End If
    End Sub

    Private Sub txtResolution_W_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtResolution_W.LostFocus, txtResolution_H.LostFocus
        Dim tb As TextBox = CType(sender, TextBox)
        If Not IsNumeric(tb.Text) OrElse tb.Text < 8 Then
            tb.Text = 400
        Else
            If tb.Text > 32768 Then
                MessageBox.Show("Maximum value is 32768.")
                tb.Text = 400
            End If
        End If
    End Sub

End Class
