Imports System.IO
Imports System.Text
Imports SMT.NextLimit.Maxwell.Win.ProcessExecution
Imports System.Drawing
Imports System.Drawing.Imaging

Public Module MXIMERGE

    Private WithEvents MXIMERGE As cProcessManager

    Private ReadOnly Property MM_ExePath() As String
        Get
            If File.Exists(_MM_ExePath_64) Then
                Return _MM_ExePath_64
            ElseIf File.Exists(_MM_ExePath_32) Then
                Return _MM_ExePath_32
            Else
                Throw New Exception("mximerge is not installed on this system.")
            End If
        End Get
    End Property
    Private _MM_ExePath_32 As String = "C:\Program Files\Next Limit\Maxwell\mximerge_win32.exe"
    Private _MM_ExePath_64 As String = "C:\Program Files\Next Limit\Maxwell\mximerge_x64.exe"

    Public Function MergeMXIs(ByRef MXIPaths() As String, ByVal OutputMXIPath As String, Optional ByVal OutImagePath As String = "") As cMXIInfo
        Try
            Dim sb As New StringBuilder

            For Each mxi As String In MXIPaths
                sb.Append(WrapStringInQuotes(mxi) & " ")
            Next

            If String.IsNullOrEmpty(OutImagePath) Then
                sb.Append(WrapStringInQuotes(OutputMXIPath))
            Else
                sb.Append(WrapStringInQuotes(OutputMXIPath) & " " & WrapStringInQuotes(OutImagePath))
            End If

            Dim _Proc As New System.Diagnostics.Process
            _Proc.StartInfo.FileName = MM_ExePath
            _Proc.StartInfo.Arguments = sb.ToString
            _Proc.StartInfo.UseShellExecute = False                 'Set UseShellExecute to False for redirection
            _Proc.StartInfo.CreateNoWindow = True
            _Proc.StartInfo.RedirectStandardOutput = True
            _Proc.StartInfo.RedirectStandardError = False
            _Proc.StartInfo.RedirectStandardInput = False
            _Proc.EnableRaisingEvents = True                        'Tell the process to raise its Exited event.
            _Proc.Start()
            _Proc.WaitForExit()
            Dim s As String = _Proc.StandardOutput.ReadToEnd()

            If InStr(s.ToLower, "mxi successfully merged") Then
                'Great, now get the info about the merged MXI.
                _Proc = New System.Diagnostics.Process
                _Proc.StartInfo.FileName = MM_ExePath
                _Proc.StartInfo.Arguments = OutputMXIPath
                _Proc.StartInfo.UseShellExecute = False                 'Set UseShellExecute to False for redirection
                _Proc.StartInfo.CreateNoWindow = True
                _Proc.StartInfo.RedirectStandardOutput = True
                _Proc.StartInfo.RedirectStandardError = False
                _Proc.StartInfo.RedirectStandardInput = False
                _Proc.EnableRaisingEvents = True                        'Tell the process to raise its Exited event.
                _Proc.Start()
                _Proc.WaitForExit()
                s = _Proc.StandardOutput.ReadToEnd()
                Return New cMXIInfo(s, MM_ExePath & " " & sb.ToString)
            Else
                Throw New Exception("The merge failed." & vbNewLine & s)
            End If
        Catch ex As Exception
            Throw New Exception("Problem with MergeMXIs(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Function GetMXIInfo(ByVal MXIPath As String) As cMXIInfo
        Try
            Dim _Proc As New System.Diagnostics.Process
            _Proc.StartInfo.FileName = MM_ExePath
            _Proc.StartInfo.Arguments = MXIPath
            _Proc.StartInfo.UseShellExecute = False
            _Proc.StartInfo.CreateNoWindow = True
            _Proc.StartInfo.RedirectStandardOutput = True
            _Proc.StartInfo.RedirectStandardError = False
            _Proc.StartInfo.RedirectStandardInput = False
            _Proc.Start()
            _Proc.WaitForExit()
            Dim s As String = _Proc.StandardOutput.ReadToEnd()
            Return New cMXIInfo(s, MM_ExePath & " " & MXIPath)
        Catch ex As Exception
            Throw New Exception("Problem with GetMXIInfo(). Error: " & ex.Message, ex)
        End Try
    End Function

End Module

Public Class cMXIInfo

    Public SampleLevel As Double
    Public Resolution As Size
    Public CommandLine As String

    Public Sub New(ByVal MXIMergeConsoleOutput As String, ByVal nCommandLine As String)
        Dim lines() As String = Split(MXIMergeConsoleOutput, vbNewLine)
        Dim s() As String
        For Each l As String In lines
            If InStr(l.ToLower, "resolution") Then
                s = Split(l, ":")
                s = Split(s(1), "x")
                Resolution = New Size(s(0), s(1))
            ElseIf InStr(l.ToLower, "sampling level") Then
                s = Split(l, ":")
                SampleLevel = s(1)
            End If
        Next
        CommandLine = nCommandLine
    End Sub

End Class
