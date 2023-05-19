Imports System.Diagnostics

Public Class LogReader_Main

    Private ReadOnly Property SelectedLogs() As EventLog()
        Get
            Select Case Me.cbLogName.SelectedItem.ToString
                Case "All"
                    Return System.Diagnostics.EventLog.GetEventLogs
                Case Else ' "Application" "SMT Alentejo"
                    Return New EventLog() {New EventLog(Me.cbLogName.SelectedItem.ToString, ".")}
            End Select
        End Get
    End Property

    Private ReadOnly Property SelectedEntryText() As String
        Get
            If lbLogEntries.SelectedItems Is Nothing OrElse lbLogEntries.SelectedItems.Count = 0 OrElse lbLogEntries.SelectedItems(0) Is Nothing Then
                Return ""
            Else
                Return CType(lbLogEntries.SelectedItems(0).Tag, cLogEntry).Entry.Message
            End If
        End Get
    End Property

    Private Function GetDesiredLogEntries() As List(Of cLogEntry)
        Dim out As New List(Of cLogEntry)
        For Each L As EventLog In SelectedLogs
            For Each E As EventLogEntry In L.Entries
                If cbSource.SelectedItem.ToString = "All" Then
                    out.Add(New cLogEntry(L.LogDisplayName, E))
                Else
                    If cbSource.SelectedItem.ToString = E.Source Then
                        out.Add(New cLogEntry(L.LogDisplayName, E))
                    End If
                End If
            Next
        Next
        Return out
    End Function

    Private Sub LogReader_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.cbLogName.SelectedIndex = 0
        Me.cbSource.SelectedIndex = 0
        cbTimer_CheckedChanged(Me, Nothing)
    End Sub

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnUpdate.Click
        UpdateView()
    End Sub

    Private Sub btnClearLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearLog.Click
        If MsgBox("Are you sure you want to clear the log(s)?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        Dim ELs() As EventLog = SelectedLogs
        For Each EL As EventLog In ELs
            EL.Clear()
        Next
        UpdateView()
    End Sub

    Private Sub UpdateView()
        RenderLogEntries(GetDesiredLogEntries)
    End Sub

    Private Sub RenderLogEntries(ByRef Entries As List(Of cLogEntry))
        Dim tLVI As ListViewItem
        Me.lbLogEntries.Items.Clear()
        Me.lbLogEntries.BeginUpdate()
        For i As Integer = Entries.Count - 1 To 0 Step -1
            tLVI = New ListViewItem(Entries(i).LogName)
            tLVI.Tag = Entries(i)
            tLVI.SubItems.Add(Entries(i).Entry.Source)
            tLVI.SubItems.Add(Entries(i).Entry.EntryType.ToString)
            tLVI.SubItems.Add(Entries(i).Entry.TimeWritten.ToString("MM/dd/yyyy HH:mm:ss"))
            Me.lbLogEntries.Items.Add(tLVI)
        Next
        Me.lbLogEntries.EndUpdate()

        Me.rtbLog.Clear()
        Dim sb As System.Text.StringBuilder
        For i As Integer = 0 To Entries.Count - 1
            sb = New System.Text.StringBuilder
            sb.Append(Entries(i).Entry.TimeWritten.ToString("MM/dd/yyyy HH:mm:ss") & " - ")
            sb.Append(Entries(i).Entry.Source & " - ")
            sb.Append(Entries(i).Entry.EntryType.ToString)
            sb.Append(vbNewLine)
            sb.Append(Entries(i).Entry.Message)
            sb.Append(vbNewLine)
            sb.Append("--------------------------------------------------------")
            sb.Append(vbNewLine)
            rtbLog.AppendText(sb.ToString)
        Next
        rtbLog.ScrollToCaret()
    End Sub

    Private Sub lbLogEntries_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles lbLogEntries.SelectedIndexChanged
        Me.txtMessage.Text = SelectedEntryText
    End Sub

    Private Class cLogEntry
        Public LogName As String
        Public Entry As EventLogEntry

        Public Sub New(ByVal nLogName As String, ByVal nEntry As EventLogEntry)
            LogName = nLogName
            Entry = nEntry
        End Sub
    End Class

    Private WithEvents TMR As Windows.Forms.Timer

    Private Sub cbTimer_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbTimer.CheckedChanged
        If cbTimer.Checked Then
            If TMR Is Nothing Then TMR = New Windows.Forms.Timer
            TMR.Interval = 5000
            TMR.Start()
        Else
            If TMR IsNot Nothing Then
                TMR.Stop()
            End If
        End If
    End Sub

    Private Sub TMR_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles TMR.Tick
        UpdateView()
    End Sub

End Class
