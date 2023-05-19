Partial Public Class ucUSER_Billing
    Inherits UserControl

    Public Sub New 
        InitializeComponent()
    End Sub

    Private Sub ucUSER_Billing_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        UpdateBalance()
    End Sub

    Private Sub btnGoToPayPal_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnGoToPayPal.Click
        If Not IsNumeric(txtCreditToAdd.Text) Then
            MessageBox.Show("Please enter a value monetary amount")
        Else
            Dim url As String = BuildPayPalURL()
            If String.IsNullOrEmpty(url) Then
                MessageBox.Show("Unable to create PayPal url.")
            Else
                DebugWrite(url)
                System.Windows.Browser.HtmlPage.Window.Navigate(New Uri(url), "_newWindow") '"toolbar=1,menubar=1,resizable=1,scrollbars=1,top=0,left=0")
            End If
        End If
    End Sub

    Private Function BuildPayPalURL() As String
        Try
            'see: https://cms.paypal.com/us/cgi-bin/?cmd=_render-content&content_ID=developer/e_howto_html_Appx_websitestandard_htmlvariables
            'example: https://www.paypal.com/xclick/business=sequoyan.media.technology@gmail.com&amount=1.00&currency_code=EUR&item_name=Render+Monster+Credit
            Dim sb As New System.Text.StringBuilder
            sb.Append("https://www.paypal.com/xclick/")
            sb.Append("business=sequoyan.media.technology@gmail.com")
            sb.Append("&amount=" & txtCreditToAdd.Text)
            Select Case cbCurrency.SelectedItem.ToString
                Case "€"
                    sb.Append("&currency_code=EUR")
                Case "$"
                    sb.Append("&currency_code=USD")
            End Select
            sb.Append("&item_name=Render+Monster+Credit")
            sb.Append("&item_number=0001")
            sb.Append("&invoice=" & User.UserId)
            sb.Append("&no_note=1") 'do not display note field
            'sb.Append("&cn=Render+Monster") 'label on note field
            'sb.Append("&cpp_header_image=http://www.seqmt.com/graphics/btn.gif")
            'sb.Append("&cpp_headerback_color=000000")
            sb.Append("&cs=1") 'page background color, seems to not work.
            'sb.Append("image_url=http://www.seqmt.com/graphics/btn.gif")
            sb.Append("&no_shipping=1")
            sb.Append("&lc=GB")
            'sb.Append("address1")
            'sb.Append("address2")
            'sb.Append("city")
            'sb.Append("country")
            'sb.Append("first_name")
            'sb.Append("last_name")
            'sb.Append("night_phone_a")
            'sb.Append("night_phone_a")
            'sb.Append("night_phone_a")
            'sb.Append("state")
            'sb.Append("zip")

            'sb.Append("")
            Return sb.ToString
        Catch ex As Exception
            Return ""
        End Try
    End Function

#Region "BALANCE CHECK"

    Private Sub UpdateBalance()
        AddHandler AuthServiceClient.GetUserBalanceCompleted, AddressOf GetUserBalance_Completed
        AuthServiceClient.GetUserBalanceAsync("", User.UserId)
    End Sub

    Private Sub GetUserBalance_Completed(ByVal sender As Object, ByVal e As AlentejoAuthService.GetUserBalanceCompletedEventArgs)
        RemoveHandler AuthServiceClient.GetUserBalanceCompleted, AddressOf GetUserBalance_Completed
        Me.tbCurrentBalance.Text = "€" & Charges_FriendlyString(e.Result)
        CurrentBalance = e.Result
    End Sub

#End Region 'BALANCE CHECK

End Class
