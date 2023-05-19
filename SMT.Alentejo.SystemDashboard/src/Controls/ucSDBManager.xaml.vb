Imports SMT.Alentejo.Core
Imports Amazon.SimpleDB
Imports Amazon.SimpleDB.Model
Imports SMT.AWS.SDB
Imports SMT.AWS.Authorization
Imports System.ComponentModel

Partial Public Class ucSDBManager

#Region "PROPERTIES"

    Private Enum eDomains_FriendlyNames
        JOBS
        JOB_PROGRESS
        JOB_FILES
        USERS
        USER_SESSIONS
        INSTANCES
        SYSTEM_SETTINGS
        SYSTEM_MESSAGES
        APPLICATIONS
        APPLICATION_FILES
    End Enum

    'Public SDB As cSMT_AWS_SDB

    Public Const sdb_domain_jobs As String = "smt_atj_jobs"
    Public Const sdb_domain_job_progress As String = "smt_atj_job_progress"
    Public Const sdb_domain_system_messages As String = "smt_atj_system_messages"
    Public Const sdb_domain_instances As String = "smt_atj_instances"
    Public Const sdb_domain_files As String = "smt_atj_files"
    Public Const sdb_domain_users As String = "smt_atj_users"
    Public Const sdb_domain_system_settings As String = "smt_atj_system_settings"
    Public Const sdb_domain_sessions As String = "smt_atj_sessions"
    Public Const sdb_domain_applications As String = "smt_atj_applications"
    Public Const sdb_domain_application_files As String = "smt_atj_application_files"

    Private ReadOnly Property SelectedDomainName() As String
        Get
            If Me.lbDomains.SelectedItem = Nothing Then Return ""

            Dim e As eDomains_FriendlyNames = [Enum].Parse(GetType(eDomains_FriendlyNames), Me.lbDomains.SelectedItem.ToString)
            Select Case e
                Case eDomains_FriendlyNames.APPLICATION_FILES
                    Return sdb_domain_application_files

                Case eDomains_FriendlyNames.APPLICATIONS
                    Return sdb_domain_applications

                Case eDomains_FriendlyNames.INSTANCES
                    Return sdb_domain_instances

                Case eDomains_FriendlyNames.JOB_FILES
                    Return sdb_domain_files

                Case eDomains_FriendlyNames.JOB_PROGRESS
                    Return sdb_domain_job_progress

                Case eDomains_FriendlyNames.JOBS
                    Return sdb_domain_jobs

                Case eDomains_FriendlyNames.SYSTEM_MESSAGES
                    Return sdb_domain_system_messages

                Case eDomains_FriendlyNames.SYSTEM_SETTINGS
                    Return sdb_domain_system_settings

                Case eDomains_FriendlyNames.USER_SESSIONS
                    Return sdb_domain_sessions

                Case eDomains_FriendlyNames.USERS
                    Return sdb_domain_users

            End Select

            Return Me.lbDomains.SelectedItem.ToString
        End Get
    End Property

    Private ReadOnly Property SelectedItemName() As String
        Get
            If Me.lbItems.SelectedItem = Nothing Then Return ""
            Return Me.lbItems.SelectedItem.ToString
        End Get
    End Property

#End Region 'PROPERTIES

#Region "FORM"

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        'SDB = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
        LoadDomainNames()
    End Sub

    Private Sub ucSDBManager_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    End Sub

#End Region 'FORM

#Region "DOMAINS"

    Private Sub LoadDomainNames()
        For Each s As String In [Enum].GetNames(GetType(eDomains_FriendlyNames))
            Me.lbDomains.Items.Add(s)
        Next
    End Sub

    Private Sub lbDomains_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbDomains.SelectionChanged
        UpdateItemList()
    End Sub

#End Region 'DOMAINS

#Region "ITEMS"

    Private Sub UpdateItemList()
        Try
            If SelectedDomainName = "" Then Exit Sub
            Cursor = Cursors.Wait
            lbItems.Items.Clear()

            Dim startIndex As Integer = lbItems.SelectedIndex
            lbItems.Items.Clear()

            Dim r As List(Of String) = SDB.Query(SelectedDomainName, "").ItemName
            For Each d As String In r
                lbItems.Items.Add(d)
            Next
            lbAttributes.Items.Clear()
            If startIndex > -1 Then
                If startIndex > lbItems.Items.Count - 1 Then startIndex = -1
                Me.lbItems.SelectedIndex = startIndex
                UpdateAttributeList()
            End If
        Catch ex As Exception
            MsgBox("Problem with UpdateItemList(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Arrow
        End Try
    End Sub

    Private Sub lbItems_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbItems.SelectionChanged
        UpdateAttributeList()
    End Sub

    Private Sub btnITM_DeleteSelected_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnITM_DeleteSelected.Click
        If MsgBox("Delete selected?", MsgBoxStyle.YesNo Or MsgBoxStyle.Question) = MsgBoxResult.No Then Exit Sub
        Cursor = Cursors.Wait
        SDB.DeleteAttributes(SelectedItemName, SelectedDomainName)
        lbItems.SelectedIndex = -1
        UpdateItemList()
        Cursor = Cursors.Arrow
    End Sub

    Private Sub btnITM_DeleteAll_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnITM_DeleteAll.Click
        If MsgBox("Delete all?", MsgBoxStyle.YesNo Or MsgBoxStyle.Question) = MsgBoxResult.No Then Exit Sub
        Cursor = Cursors.Wait
        lbItems.SelectedIndex = -1
        Dim bw As New BackgroundWorker
        bw.WorkerReportsProgress = True
        bw.WorkerSupportsCancellation = True
        AddHandler bw.DoWork, AddressOf DeleteAllItems
        AddHandler bw.RunWorkerCompleted, AddressOf DeleteAllItems_Completed
        AddHandler bw.ProgressChanged, AddressOf DeleteAllItems_ProgressChanged
        bw.RunWorkerAsync(SelectedDomainName)
        Cursor = Cursors.Arrow
    End Sub

    Private Sub DeleteAllItems(ByVal sender As Object, ByVal e As DoWorkEventArgs)
        Dim worker As BackgroundWorker = CType(sender, BackgroundWorker)
        'Dim tws As cSMT_AWS_THREADING_SDB_DeleteAttributes
        'Dim t As Threading.Thread
        Dim QR As New QueryResult
        Dim Items As List(Of String)
        Dim oop As Boolean = True
        While oop
            QR = SDB.Query(e.Argument, "", 250, QR.NextToken)
            Items = QR.ItemName
            For i As Integer = 0 To Items.Count - 1

                'tws = New cSMT_AWS_THREADING_SDB_DeleteAttributes(Items(i), e.Argument, SDB, i + 1, Items.Count, worker)
                'AddHandler tws.evComplete, AddressOf DeleteItemCallback
                't = New Threading.Thread(AddressOf tws.ThreadProc)
                't.Start()

                SDB.DeleteAttributes(Items(i), e.Argument)
                worker.ReportProgress(Math.Round(i / (Items.Count - 1), 2) * 100, e.Argument & " (" & i + 1 & "/" & Items.Count & If(Not String.IsNullOrEmpty(QR.NextToken), " more to come", "") & ")")
            Next
            oop = Not String.IsNullOrEmpty(QR.NextToken)
        End While
    End Sub

    Private Sub DeleteAllItems_Completed(ByVal sender As Object, ByVal e As RunWorkerCompletedEventArgs)
        'GetAllItemsForDomain()
        Debug.WriteLine("Delete all items completed.")
    End Sub

    Private Sub DeleteAllItems_ProgressChanged(ByVal sender As Object, ByVal e As ProgressChangedEventArgs)
        Debug.WriteLine("Delete all items in " & e.UserState & " is " & e.ProgressPercentage & "% completed.")
    End Sub

#End Region 'ITEMS

#Region "ATTRIBUTES"

    Private Sub UpdateAttributeList()
        Try
            If SelectedDomainName = "" Or SelectedItemName = "" Then Exit Sub
            Cursor = Cursors.Wait
            Me.lbAttributes.Items.Clear()
            Dim atts As List(Of Attribute) = SDB.GetAttributes(SelectedItemName, SelectedDomainName)
            atts.Sort(New cSMT_ATJ_SDBAttribute_Sorter)
            For Each a As Attribute In atts
                Me.lbAttributes.Items.Add(New cSMT_AWS_SDB_Attribute(a))
            Next
            'Me.txtSA_txtItemName.Text = SelectedItemName
        Catch ex As Exception
            MsgBox("Problem with UpdateAttributeList(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Arrow
        End Try
    End Sub

    Private Sub lbAttributes_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles lbAttributes.SelectionChanged
        If Me.lbAttributes.SelectedItem Is Nothing Then Exit Sub
        Dim a As cSMT_AWS_SDB_Attribute = CType(Me.lbAttributes.SelectedItem, cSMT_AWS_SDB_Attribute)
        txtAttributeName.Text = a.Attri.Name
        txtAttributeValue.Text = a.Attri.Value
    End Sub

    Private Sub btnATT_DeleteSelected_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnATT_DeleteSelected.Click
        Cursor = Cursors.Wait
        Dim la As New List(Of Attribute)
        la.Add(CType(Me.lbAttributes.SelectedItem, cSMT_AWS_SDB_Attribute).Attri)
        SDB.DeleteAttributes(SelectedItemName, SelectedDomainName, la)
        UpdateAttributeList()
        Cursor = Cursors.Arrow
    End Sub

    Private Sub btnSetAttribute_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSetAttribute.Click
        Try
            Cursor = Cursors.Wait
            Dim lra As New List(Of ReplaceableAttribute)
            Dim ra As New ReplaceableAttribute()
            ra.Name = Me.txtAttributeName.Text
            ra.Value = Me.txtAttributeValue.Text
            ra.Replace = cbAttributeSet_Replace.IsChecked
            lra.Add(ra)
            SDB.PutAttributes(lra, SelectedItemName, SelectedDomainName)
            UpdateAttributeList()
        Catch ex As Exception
            MsgBox("Problem with btnSetAttribute_Click(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Arrow
        End Try
    End Sub

#End Region 'ATTRIBUTES

End Class
