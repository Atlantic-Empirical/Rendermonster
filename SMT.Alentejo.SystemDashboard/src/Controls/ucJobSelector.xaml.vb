Imports SMT.Alentejo.Core.JobManagement

Partial Public Class ucJobSelector

    Public Event evJobSelected()
    'Public Event evCancel()

    Public Jobs As List(Of cSMT_ATJ_RenderJob_Maxwell)

    Private Sub btnOk_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnOk.Click
        If lbJobs.SelectedItem Is Nothing Then
            MsgBox("Select a job.")
        Else
            CurrentJob = lbJobs.SelectedItem
            RaiseEvent evJobSelected()
        End If
    End Sub

    'Private Sub btnCancel_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnCancel.Click
    '    RaiseEvent evCancel()
    'End Sub

    Private Sub cbDateRange_SelectionChanged(ByVal sender As System.Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles cbDateRange.SelectionChanged
        Dim JIQ As New cSMT_ATJ_JobInfoQuery()
        Select Case cbDateRange.SelectedIndex
            Case 0
                JIQ.DateRange = Core.eSMT_ATJ_DateRange.All
            Case 1
                JIQ.DateRange = Core.eSMT_ATJ_DateRange.Past_7_Days
            Case 2
                JIQ.DateRange = Core.eSMT_ATJ_DateRange.Past_30_Days
        End Select
        JIQ.UserFilter = CurrentUser.Id
        Jobs = GetJobInfo(JIQ)
        UpdateJobList()
    End Sub

    Private Sub btnKeywordSearch_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnKeywordSearch.Click

    End Sub

    Private Sub UpdateJobList()
        Try
            lbJobs.Items.Clear()
            If Jobs Is Nothing OrElse Jobs.Count < 1 Then Exit Sub

            For Each j As cSMT_ATJ_RenderJob_Maxwell In Jobs
                lbJobs.Items.Add(j)
            Next

            'lvJobs.View = BuildGridView(Jobs)
            'lvJobs.ItemsSource = Jobs
        Catch ex As Exception
            MsgBox("Problem with UpdateJobList(). Error: " & ex.Message)
        End Try
    End Sub

    'Public Shared Function BuildGridView(ByVal Jobs As List(Of cSMT_ATJ_RenderJob_Maxwell)) As GridView
    '    Try
    '        Dim gv As New GridView
    '        gv.AllowsColumnReorder = False
    '        gv.ColumnHeaderToolTip = "Jobs"

    '        Dim tGVC As GridViewColumn

    '        tGVC = New GridViewColumn
    '        tGVC.Header = "Name"
    '        tGVC.Width = 100
    '        tGVC.DisplayMemberBinding = New Binding("Name")
    '        gv.Columns.Add(tGVC)

    '        'job name
    '        'job status
    '        'job date
    '        'file name


    '        Return gv
    '    Catch ex As Exception
    '        Throw New Exception("Problem with BuildGridView(). Error: " & ex.Message, ex)
    '    End Try
    'End Function

End Class
