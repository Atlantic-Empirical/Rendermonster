Imports SMT.PayPal.API
Imports SMT.PayPal.API.SOAP
Imports com.paypal.soap.api
Imports System.Collections.ObjectModel

Partial Public Class ucPayPal

    Private ReadOnly Property PP() As cPayPalClient_SOAP
        Get
            If _PP Is Nothing Then _PP = New cPayPalClient_SOAP
            Return _PP
        End Get
    End Property
    Private _PP As cPayPalClient_SOAP
    Private Balance As cSMT_PP_Amount
    Private Transactions As List(Of cSMT_PP_TransactionLite)

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUpdate.Click
        UpdatePayPalData()
    End Sub

    Public Sub UpdatePayPalData()
        Try
            Cursor = Cursors.Wait
            Balance = PP.GetBalance
            Me.tbPayPalBalance.Text = Balance.Currency & " " & Balance.Amount
            'get transactions since the beginning of the month
            Transactions = PP.TransactionSeach(New Date(Date.UtcNow.Year, Date.UtcNow.Month, 1), Date.UtcNow)
            'Transactions = PP.TransactionSeach()
            lvPayPalTransactions.ItemsSource = New cInstanceArray(Transactions.ToArray)
            lvPayPalTransactions.View = GetGridView()
        Catch ex As Exception
            MsgBox("Problem with UpdatePayPalData(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Arrow
        End Try
    End Sub

    Public Class cInstanceArray
        Inherits ObservableCollection(Of cSMT_ATJ_PayPalTransactionLiteView)
        Public Sub New(ByRef Transactions() As cSMT_PP_TransactionLite)
            For Each i As cSMT_PP_TransactionLite In Transactions
                Add(New cSMT_ATJ_PayPalTransactionLiteView(i))
            Next
        End Sub
    End Class

    Private Function GetGridView() As GridView
        Try
            Dim myGridView As New GridView()
            myGridView.AllowsColumnReorder = True
            myGridView.ColumnHeaderToolTip = "Transaction Information"

            Dim tgvc As GridViewColumn

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Status")
            tgvc.Header = "Status"
            tgvc.Width = 80
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Id")
            tgvc.Header = "Id"
            tgvc.Width = 40
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Type")
            tgvc.Header = "Type"
            tgvc.Width = 60
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Timestamp")
            tgvc.Header = "Time"
            tgvc.Width = 150
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Timezone")
            tgvc.Header = "Timezone"
            tgvc.Width = 60
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Payer_DisplayName")
            tgvc.Header = "Payer"
            tgvc.Width = 200
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Net")
            tgvc.Header = "Net"
            tgvc.Width = 90
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Gross")
            tgvc.Header = "Gross"
            tgvc.Width = 90
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Fee")
            tgvc.Header = "Fee"
            tgvc.Width = 90
            myGridView.Columns.Add(tgvc)

            Return myGridView
        Catch ex As Exception
            Throw New Exception("Problem with GetGridView(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Class cSMT_ATJ_PayPalTransactionLiteView

        Private Tx As cSMT_PP_TransactionLite

        Public Sub New(ByRef i As cSMT_PP_TransactionLite)
            Tx = i
        End Sub

        Public ReadOnly Property Status() As String
            Get
                Return Tx.Status
            End Get
        End Property

        Public ReadOnly Property Id() As String
            Get
                Return Tx.Id
            End Get
        End Property

        Public ReadOnly Property Type() As String
            Get
                Return Tx.Type
            End Get
        End Property

        Public ReadOnly Property Timestamp() As String
            Get
                Return Tx.Timestamp
                'Dim d As DateTime = DateFrom_ISO8601(Instance.LaunchTime)
                'Return d.ToString("ddd, dd MMM HH:mm")
            End Get
        End Property

        Public ReadOnly Property Timezone() As String
            Get
                Return Tx.Timezone
            End Get
        End Property

        Public ReadOnly Property Payer() As String
            Get
                Return Tx.Payer
            End Get
        End Property

        Public ReadOnly Property Payer_DisplayName() As String
            Get
                Return Tx.Payer_DisplayName
            End Get
        End Property

        Public ReadOnly Property Net() As String
            Get
                Return Tx.Net.FriendlyString
            End Get
        End Property

        Public ReadOnly Property Gross() As String
            Get
                Return Tx.Gross.FriendlyString
            End Get
        End Property

        Public ReadOnly Property Fee() As String
            Get
                Return Tx.Fee.FriendlyString
            End Get
        End Property

    End Class

    Private Sub lvPayPalTransactions_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lvPayPalTransactions.MouseDoubleClick
        If lvPayPalTransactions.SelectedItem Is Nothing Then Exit Sub
        DisplayTransactionDetail(Transactions(lvPayPalTransactions.SelectedIndex).Id)
    End Sub

    Private Sub DisplayTransactionDetail(ByVal TransactionId As String)
        Try
            Cursor = Cursors.Wait
            Dim details As PaymentTransactionType = PP.GetTransactionDetails(TransactionId)

            Debug.Write("hi")
        Catch ex As Exception
            MsgBox("Problem with DisplayTransactionDetail(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Arrow
        End Try
    End Sub

End Class
