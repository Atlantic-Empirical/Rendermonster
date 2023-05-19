Imports System
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes

Partial Public Class ucRenderJobOptions_Alentejo 
	Inherits UserControl

	Public Sub New()
		' Required to initialize variables
		InitializeComponent()
	End Sub

    Private Sub txt_GotFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtCPUs.GotFocus, txtLimitCost.GotFocus
        CType(sender, TextBox).Text = ""
    End Sub

    Private Sub txtCPUs_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtCPUs.LostFocus
        If Not IsNumeric(Me.txtCPUs.Text) OrElse Me.txtCPUs.Text > 256 Then
            Me.txtCPUs.Text = 8
        Else
            Select Case txtCPUs.Text
                Case 3, 4
                    Me.txtCPUs.Text = 2
                Case 5, 6, 7
                    Me.txtCPUs.Text = 8
                Case Else
                    Dim re As Decimal = Decimal.Remainder(Me.txtCPUs.Text, 8)
                    If re <> 0 Then
                        Me.txtCPUs.Text = Decimal.Ceiling(Me.txtCPUs.Text / 8) * 8
                    End If
            End Select
        End If
        Select Case txtCPUs.Text
            Case 1, 2
                Me.txtRAM.Text = "2"
            Case Else
                Me.txtRAM.Text = InstancesRequested * 7
        End Select
        Me.txtCostPerHour.Text = "€" & (CoresRequested * 2)
    End Sub

    Private Sub txtLimitCost_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtLimitCost.LostFocus
        Dim tb As TextBox = CType(sender, TextBox)
        If Not IsNumeric(tb.Text) Then
            tb.Text = 50
        ElseIf tb.Text < 50 AndAlso tb.Text <> 0 Then
            MessageBox.Show("Enter 0 to specify no limit or enter a value greater than or equal to '50'.")
            tb.Text = 50
        End If
    End Sub

    Public Property CoresRequested() As UInt16
        Get
            Return If(IsNumeric(Me.txtCPUs.Text), CType(Me.txtCPUs.Text, UInt16), 8)
        End Get
        Set(ByVal value As UInt16)
            Me.txtCPUs.Text = value
        End Set
    End Property

    Private ReadOnly Property InstancesRequested() As Byte
        Get
            Return CoresRequested / 8
        End Get
    End Property

    Public Property CostCeiling() As UInt16
        Get
            Return If(IsNumeric(Me.txtLimitCost.Text), CType(Me.txtLimitCost.Text, UInt16), 0)
        End Get
        Set(ByVal value As UInt16)
            Me.txtLimitCost.Text = value
        End Set
    End Property

    Public Property EmailSamples() As Boolean
        Get
            Return Me.cbEmailSamples.IsChecked
        End Get
        Set(ByVal value As Boolean)
            Me.cbEmailSamples.IsChecked = value
        End Set
    End Property

End Class
