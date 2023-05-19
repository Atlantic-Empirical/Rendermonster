Imports SMT.Alentejo.Core.AtjTrace

Friend Class AppMgr

    <STAThread()> _
    Public Shared Sub Main()
        Try
            Dim frm As New RenderManager_Main
            Application.Run(frm)
        Catch ex As Exception
            Dim atjTRACE As New cSMT_ATJ_TRACE("Render Manager")
            atjTRACE.LogMessage("Problem in Main(). Error: " & ex.Message & " StackTrace: " & ex.StackTrace, EventLogEntryType.Information)
        End Try
    End Sub

End Class
