Imports SMT.NextLimit.Maxwell
Imports System.IO
Imports System.Text

Module Module1

    Private WithEvents MXCL As cSMT_MXCL

    Sub Main()
        RunTestRender()
    End Sub

    Private Sub RunTestRender()
        Try
            Dim Proc As New System.Diagnostics.Process

            Proc.StartInfo.FileName = "C:\Program Files\Next Limit\Maxwell\mxcl.exe"
            Proc.StartInfo.Arguments = " -stdout"
            Proc.StartInfo.UseShellExecute = False 'Set UseShellExecute to False for redirection
            Proc.StartInfo.CreateNoWindow = False
            Proc.StartInfo.RedirectStandardOutput = False
            Proc.StartInfo.RedirectStandardError = False
            Proc.StartInfo.RedirectStandardInput = False

            'Tell the process to raise its Exited event.
            'Proc.EnableRaisingEvents = True

            'Setup event handlers
            'AddHandler Proc.Exited, AddressOf ProcessExited               'Exit Event
            'AddHandler Proc.OutputDataReceived, AddressOf ProcessOutput   'Stdout Output Event
            'AddHandler Proc.ErrorDataReceived, AddressOf ProcessOutput    'Stderr Output Event

            'Start the process
            Dim result As Boolean = Proc.Start()
            If Not result Then Throw New Exception("Failed to start process.")

            'AttachConsole.AttachConsole(Proc.Id)

            'Start the asynchronous read of the output streams
            'Proc.BeginOutputReadLine()
            'Proc.BeginErrorReadLine()

            'Get the stream for StdIn
            'StandardInput = Proc.StandardInput

            Dim i As Integer = 0
            While i < 50
                'Console.
            End While


        Catch ex As Exception
            MsgBox("Problem with RunTestRender(). Error: " & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    'Private Sub Handle_MXCLProgressUpdate(ByRef LogLine As cMXCLLogLine) Handles MXCL.evProgressUpdate
    '    Console.WriteLine("MXCL RENDER PROGRESS | " & LogLine.ToString)
    'End Sub

End Module
