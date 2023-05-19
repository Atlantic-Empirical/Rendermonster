
Public Module FONTS

    Public Sub SetFonts(ByVal StartCtl As Panel)
        Dim c As Control
        Dim t As TextBlock
        For Each uie As UIElement In StartCtl.Children
            Dim tp As Type = uie.GetType
            If tp Is GetType(TextBlock) Then
                t = TryCast(uie, TextBlock)
                t.FontFamily = GetFont(1, eAtjFont.HelveticaNeueExtended_Std)
            End If
            If tp Is GetType(Button) Or tp Is GetType(TextBox) Then
                c = TryCast(uie, Control)
                c.FontFamily = GetFont(1, eAtjFont.HelveticaNeueExtended_Std)
            End If
            If tp Is GetType(Canvas) Then
                SetFonts(uie)
            End If
        Next
    End Sub

    Public Function GetFont(ByVal LevelsRemoved As Byte, ByVal Font As eAtjFont) As FontFamily
        Dim pth As String

        Select Case Font
            Case eAtjFont.HelveticaNeueExtended_Lite
                pth = "Fonts/HelveticaExtLt.ttf#Helvetica43-ExtendedLight"
            Case eAtjFont.HelveticaNeueExtended_Med
                pth = "Fonts/HelveticaExtMed.ttf#Helvetica63-ExtendedMedium"
            Case eAtjFont.HelveticaNeueExtended_Std
                pth = "Fonts/HelveticaExt.ttf#Helvetica53-Extended"
            Case eAtjFont.HelveticaNeueExtended_Thin
                pth = "Fonts/HelveticaExtThn.ttf#Helvetica33-ExtendedThin"
        End Select

        For b As Byte = 0 To LevelsRemoved - 1
            If LevelsRemoved = 0 Then
                pth = "/" & pth
            Else
                pth = "../" & pth
            End If
        Next
        Return New FontFamily(pth)
    End Function

    Public Enum eAtjFont
        HelveticaNeueExtended_Std
        HelveticaNeueExtended_Lite
        HelveticaNeueExtended_Med
        HelveticaNeueExtended_Thin
    End Enum

End Module
