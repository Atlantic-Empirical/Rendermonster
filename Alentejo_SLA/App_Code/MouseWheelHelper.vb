Imports System.Windows.Browser

Public Class MouseWheelEventArgs
    Inherits EventArgs
    Private m_delta As Double
    Private m_handled As Boolean = False

    Public Sub New(ByVal delta As Double)
        Me.m_delta = delta
    End Sub

    Public ReadOnly Property Delta() As Double
        Get
            Return Me.m_delta
        End Get
    End Property

    ' Use handled to prevent the default browser behavior! 
    Public Property Handled() As Boolean
        Get
            Return Me.m_handled
        End Get
        Set(ByVal value As Boolean)
            Me.m_handled = value
        End Set
    End Property
End Class

Public Class MouseWheelHelper

    Public Event Moved As EventHandler(Of MouseWheelEventArgs)
    Private Shared _worker As Worker
    Private isMouseOver As Boolean = False

    Public Sub New(ByVal element As FrameworkElement)

        If MouseWheelHelper._worker Is Nothing Then
            MouseWheelHelper._worker = New Worker()
        End If

        AddHandler _worker.Moved, AddressOf HandleMouseWheel

        AddHandler element.MouseEnter, AddressOf Me.HandleMouseEnter
        AddHandler element.MouseLeave, AddressOf Me.HandleMouseLeave
        AddHandler element.MouseMove, AddressOf Me.HandleMouseMove
    End Sub

    Private Sub HandleMouseWheel(ByVal sender As Object, ByVal args As MouseWheelEventArgs)
        If Me.isMouseOver Then
            RaiseEvent Moved(Me, args)
        End If
    End Sub

    Private Sub HandleMouseEnter(ByVal sender As Object, ByVal e As EventArgs)
        Me.isMouseOver = True
    End Sub

    Private Sub HandleMouseLeave(ByVal sender As Object, ByVal e As EventArgs)
        Me.isMouseOver = False
    End Sub

    Private Sub HandleMouseMove(ByVal sender As Object, ByVal e As EventArgs)
        Me.isMouseOver = True
    End Sub

    Private Class Worker

        Public Event Moved As EventHandler(Of MouseWheelEventArgs)

        Public Sub New()

            If HtmlPage.IsEnabled Then
                HtmlPage.Window.AttachEvent("DOMMouseScroll", AddressOf HandleMouseWheel)
                HtmlPage.Window.AttachEvent("onmousewheel", AddressOf HandleMouseWheel)
                HtmlPage.Document.AttachEvent("onmousewheel", AddressOf HandleMouseWheel)
            End If

        End Sub

        Private Sub HandleMouseWheel(ByVal sender As Object, ByVal args As HtmlEventArgs)
            Dim delta As Double = 0

            Dim eventObj As ScriptObject = args.EventObject

            If eventObj.GetProperty("wheelDelta") IsNot Nothing Then
                delta = CDbl(eventObj.GetProperty("wheelDelta")) / 120


                If HtmlPage.Window.GetProperty("opera") IsNot Nothing Then
                    delta = -delta
                End If
            ElseIf eventObj.GetProperty("detail") IsNot Nothing Then
                delta = -CDbl(eventObj.GetProperty("detail")) / 3

                If HtmlPage.BrowserInformation.UserAgent.IndexOf("Macintosh") <> -1 Then
                    delta = delta * 3
                End If
            End If

            If delta <> 0 Then 'AndAlso Me.Moved IsNot Nothing
                Dim wheelArgs As New MouseWheelEventArgs(delta)
                RaiseEvent Moved(Me, wheelArgs)
                If wheelArgs.Handled Then
                    args.PreventDefault()
                End If
            End If
        End Sub

    End Class

End Class
