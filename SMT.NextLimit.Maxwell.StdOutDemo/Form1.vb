Imports System.Diagnostics

Public Class Form1

    Private Sub btnStartGeneric_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartGeneric.Click
        StartProcessRedirectOutput("TestConsole.exe", "")
    End Sub

    Private Sub btnStartMaxwell_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartMaxwell.Click
        StartProcessRedirectOutput("C:\Program Files\Next Limit\Maxwell\mxcl.exe", "-mxs:" & Chr(34) & Me.txtMXSPath.Text & Chr(34) & " -stdout")
    End Sub

    Private WithEvents Proc As Process

    Private Sub StartProcessRedirectOutput(ByVal ExePath As String, ByVal CommandLine As String)
        Dim result As Boolean = False

        Proc = New System.Diagnostics.Process
        Proc.StartInfo.FileName = ExePath
        Proc.StartInfo.Arguments = CommandLine
        Proc.StartInfo.UseShellExecute = False 'Set UseShellExecute to False for redirection
        Proc.StartInfo.CreateNoWindow = True
        Proc.StartInfo.RedirectStandardOutput = True

        'hookup event
        AddHandler _Proc.OutputDataReceived, AddressOf ProcessOutput   'Stdout Output Event

        'Start the process
        result = Proc.Start()
        If Not result Then Throw New Exception("Failed to start process.")

        'Start the asynchronous read of the output streams
        _Proc.BeginOutputReadLine()

    End Sub

    Public Sub ProcessOutput(ByVal sender As Object, ByVal outLine As DataReceivedEventArgs) 'Handles _Proc.OutputDataReceived, _Proc.ErrorDataReceived
        Dim proc As System.Diagnostics.Process
        If Not String.IsNullOrEmpty(outLine.Data) Then
            proc = CType(sender, System.Diagnostics.Process)
            AddLogLine("Pid-" & proc.Id & " = " & outLine.Data)
        End If
    End Sub

#Region "LOG WINDOW"

    Private Sub AddLogLine(ByVal msg As String)
        If Me Is Nothing OrElse Me.IsDisposed Then Exit Sub
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _RtbAppend_Delegate Is Nothing Then _RtbAppend_Delegate = New RtbAppend_Delegate(AddressOf RtbAppend)
            Invoke(_RtbAppend_Delegate, New Object() {msg})
        Else
            RtbAppend(msg)
        End If
    End Sub

    Private Sub RtbAppend(ByVal Msg As String)
        Me.rtbQueuePollingConsole.AppendText(Msg & vbNewLine)
        Me.rtbQueuePollingConsole.ScrollToCaret()
    End Sub
    Private Delegate Sub RtbAppend_Delegate(ByVal Msg As String)
    Private _RtbAppend_Delegate As RtbAppend_Delegate

#End Region 'LOG WINDOW

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If Proc IsNot Nothing Then
            Proc.Kill()
            Proc.Dispose()
        End If
    End Sub

End Class
