Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Partial Public Class ucRenderJobOptions_Maxwell 
	Inherits UserControl

	Public Sub New()
		' Required to initialize variables
		InitializeComponent()
	End Sub

    Private Sub txt_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtZBuffer_max.GotFocus, txtZBuffer_min.GotFocus
        CType(sender, TextBox).Text = ""
    End Sub

    Public Property CameraNames() As String
        Get
            Dim sb As New System.Text.StringBuilder
            For Each cn As String In cbCamera.Items
                sb.Append(cn & "||")
            Next
            Return sb.ToString
        End Get
        Set(ByVal value As String)
            Dim CameraNames() As String = Split(value, "||")
            cbCamera.Items.Clear()
            cbCamera.Items.Add("")
            For Each c As String In CameraNames
                Me.cbCamera.Items.Add(c)
            Next
            cbCamera.SelectedIndex = 0
        End Set
    End Property

    Public Property ActiveCamera() As String
        Get
            Return Me.cbCamera.SelectedItem.ToString
        End Get
        Set(ByVal value As String)
            For i As Integer = 0 To cbCamera.Items.Count - 1
                If cbCamera.Items(i).ToString = value Then
                    cbCamera.SelectedIndex = i
                    Exit For
                End If
            Next
        End Set
    End Property

    Public Property AnimationFrames() As String
        Get
            Return Me.txtAnimationFrames.Text
        End Get
        Set(ByVal value As String)
            If String.IsNullOrEmpty(value) Then
                Me.txtAnimationFrames.Text = ""
            Else
                Me.txtAnimationFrames.Text = value
            End If
        End Set
    End Property

    Public Property CreateMultilight() As Boolean
        Get
            Return Me.cbMultilight.IsChecked
        End Get
        Set(ByVal value As Boolean)
            Me.cbMultilight.IsChecked = value
        End Set
    End Property

    Public Property RenderWithTextures() As Boolean
        Get
            Return Me.cbRenderWithTextures.IsChecked
        End Get
        Set(ByVal value As Boolean)
            Me.cbRenderWithTextures.IsChecked = value
        End Set
    End Property

    Public Property Render() As Boolean
        Get
            Return cbRender.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbRender.IsChecked = value
        End Set
    End Property

    Public Property Shadow() As Boolean
        Get
            Return cbShadow.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbShadow.IsChecked = value
        End Set
    End Property

    Public Property Alpha() As Boolean
        Get
            Return cbAlpha.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbAlpha.IsChecked = value
        End Set
    End Property

    Public Property Alpha_Opaque() As Boolean
        Get
            Return cbOpaque.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbOpaque.IsChecked = value
        End Set
    End Property

    Public Property MaterialId() As Boolean
        Get
            Return cbMaterialId.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbMaterialId.IsChecked = value
        End Set
    End Property

    Public Property ObjectId() As Boolean
        Get
            Return cbObjectId.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbObjectId.IsChecked = value
        End Set
    End Property

    Public Property zBuffer() As Boolean
        Get
            Return cbZBuffer.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbZBuffer.IsChecked = value
        End Set
    End Property

    Public Property zBuffer_min() As UInt16
        Get
            Return If(IsNumeric(Me.txtZBuffer_min.Text), CType(Me.txtZBuffer_min.Text, UInt16), 0)
        End Get
        Set(ByVal value As UInt16)
            Me.txtZBuffer_min.Text = value
        End Set
    End Property

    Public Property zBuffer_max() As UInt16
        Get
            Return If(IsNumeric(Me.txtZBuffer_max.Text), CType(Me.txtZBuffer_max.Text, UInt16), 0)
        End Get
        Set(ByVal value As UInt16)
            Me.txtZBuffer_max.Text = value
        End Set
    End Property

    Public Property Vignetting() As UInt16
        Get
            Return If(IsNumeric(Me.txtVignettingValue.Text), CType(Me.txtVignettingValue.Text, UInt16), 0)
        End Get
        Set(ByVal value As UInt16)
            Me.txtVignettingValue.Text = value
        End Set
    End Property

    Public Property ScatteringLens() As UInt16
        Get
            Return If(IsNumeric(Me.txtScatteringLensValue.Text), CType(Me.txtScatteringLensValue.Text, UInt16), 0)
        End Get
        Set(ByVal value As UInt16)
            Me.txtScatteringLensValue.Text = value
        End Set
    End Property

    Public Property UsePreviewEngine() As Boolean
        Get
            Return cbPreviewEngine.IsChecked
        End Get
        Set(ByVal value As Boolean)
            cbPreviewEngine.IsChecked = value
        End Set
    End Property

    Private Sub txtScatteringLensValue_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtScatteringLensValue.LostFocus, txtVignettingValue.LostFocus
        Dim tb As TextBox = CType(sender, TextBox)
        If Not IsNumeric(tb.Text) OrElse tb.Text < 0 Then
            tb.Text = 0
        Else
            If tb.Text > 1000 Then
                tb.Text = 0
                MessageBox.Show("Value must be between 0 and 1000.")
            End If
        End If

    End Sub

End Class
