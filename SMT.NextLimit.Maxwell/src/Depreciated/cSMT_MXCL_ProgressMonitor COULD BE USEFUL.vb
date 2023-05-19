
'Public Class cSMT_MXCL_ProgressMonitor

'    '#Region "CONSTRUCTOR"

'    '    Public Sub New()
'    '        CONSOLE_POLLING_TIMER = New System.Windows.Forms.Timer
'    '        CONSOLE_POLLING_TIMER.Interval = 1000 * poll_interval
'    '        CONSOLE_POLLING_TIMER.Start()
'    '    End Sub

'    '    Public Sub Dispose()
'    '        CONSOLE_POLLING_TIMER.Stop()
'    '        CONSOLE_POLLING_TIMER.Dispose()
'    '    End Sub

'    '#End Region 'CONSTRUCTOR

'    '#Region "PUBLIC"

'    '#Region "PUBLIC:EVENTS"

'    '    Public Event evNewLogLine(ByRef LogLine As cMXCLLogLine)
'    '    Public Event evInitialized()

'    '#End Region 'PUBLIC:EVENTS

'    '#Region "PUBLIC:PROPERTIES"

'    '    Public ReadOnly Property LatestBenchmark() As Double
'    '        Get

'    '        End Get
'    '    End Property

'    '    Public ReadOnly Property LatestSampleLevel() As Double
'    '        Get

'    '        End Get
'    '    End Property

'    '    Public ReadOnly Property LatestLogLine() As cMXCLLogLine
'    '        Get

'    '        End Get
'    '    End Property

'    '    Public ReadOnly Property LogLines() As cMXCLLogLine()
'    '        Get

'    '        End Get
'    '    End Property
'    '    Private _LogLines As List(Of cMXCLLogLine)

'    '    ''' <summary>
'    '    ''' Returns the lines up to the beginning of rendering.
'    '    ''' </summary>
'    '    ''' <value></value>
'    '    ''' <returns></returns>
'    '    ''' <remarks></remarks>
'    '    Public ReadOnly Property InitializationLines() As cMXCLLogLine()
'    '        Get

'    '        End Get
'    '    End Property

'    '#End Region 'PUBLIC:PROPERTIES

'    '#Region "PUBLIC METHODS"

'    '#End Region 'PUBLIC:METHODS

'    '#End Region 'PUBLIC

'    '#Region "PRIVATE"

'    '#Region "PRIVATE:POLLING"

'    '    Private WithEvents CONSOLE_POLLING_TIMER As System.Windows.Forms.Timer 'must not use System.Timers.Timer because it returns on a new thread and the clipboard cannot be accessed from non-UI threads.

'    '    ''' <summary>
'    '    ''' Console polling interval in seconds.
'    '    ''' </summary>
'    '    ''' <remarks></remarks>
'    '    Private poll_interval As Byte = 7

'    '    Private Sub CONSOLE_POLLING_TIMER_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles CONSOLE_POLLING_TIMER.Tick
'    '        ParseConsoleData(ScrapeConsole)
'    '    End Sub

'    '    Private Sub ParseConsoleData(ByVal lines As String)
'    '        Try
'    '            If String.IsNullOrEmpty(lines) Then Exit Sub
'    '            Dim ls() As String = Split(lines, vbCrLf)
'    '            Dim tmpLL As cMXCLLogLine
'    '            If _LogLines Is Nothing Then
'    '                _LogLines = New List(Of cMXCLLogLine)
'    '                For Each l As String In ls
'    '                    tmpLL = New cMXCLLogLine(l)
'    '                    _LogLines.Add(tmpLL)
'    '                    RaiseEvent evNewLogLine(tmpLL)
'    '                Next
'    '            Else
'    '                If ls.Count = _LogLines.Count Then Exit Sub 'no new lines found
'    '                'just process the new ones
'    '                Dim newLineCount As UInt16 = ls.Count - _LogLines.Count
'    '                For i As Integer = UBound(ls) - (newLineCount - 1) To UBound(ls)
'    '                    tmpLL = New cMXCLLogLine(ls(i))
'    '                    _LogLines.Add(tmpLL)
'    '                    RaiseEvent evNewLogLine(tmpLL)
'    '                Next
'    '            End If
'    '        Catch ex As Exception
'    '            Throw New Exception("Problem with ParseConsoleData(). Error: " & ex.Message)
'    '        End Try
'    '    End Sub

'    '#End Region 'PRIVATE:POLLING

'    '#End Region 'PRIVATE

'End Class

''#Region "CLASSES"

''<Serializable()> _
''Public Class cMXCLLogLine

''    Public Timestamp As DateTime
''    Public SampleLevel As Double
''    Public Benchmark As Double
''    Public GeneralMessage As String

''    Public Sub New()
''    End Sub

''    Public Sub New(ByVal Line As String)
''        Try
''            If InStr(Line, "[") = 1 Then
''                'we have a time stamp
''                Dim pos_ts_close As Integer = InStr(Line, "]")
''                If Mid(Line, pos_ts_close + 1, 3) = " SL" Then
''                    'EXAMPLES:
''                    '   [09/December/2008 13:04:42] SL of 2.00. Benchmark of 30.963. Time: 22s

''                    'we have a render progress update, parse it for interesting data
''                    Dim pos_SL_of As Integer = InStr(Line, "SL of ")
''                    Dim pos_Benchmark_of As Integer = InStr(Line, "Benchmark of")
''                    SampleLevel = Val(Mid(Line, pos_SL_of + 6, pos_Benchmark_of - pos_SL_of + 6))
''                    Dim pos_Time As Integer = InStr(Line, "Time:")
''                    Benchmark = Val(Mid(Line, pos_Benchmark_of + 13, pos_Time - pos_Benchmark_of + 13))
''                Else
''                    'EXAMPLES:
''                    '   [09/December/2008 13:02:49] Reading MXS:N:/PRODUCT DEVELOPMENT/Alentejo/Maxwell Projects/Vase diamond/Vase diamond.mxs
''                    '   [09/December/2008 13:04:58] Render finished succesfully 
''                    GeneralMessage = Mid(Line, pos_ts_close + 2)
''                End If
''            Else
''                'All lines that do not begin with a time stamp.
''                If Line = "" Then Line = " "
''                GeneralMessage = Line
''            End If
''            Timestamp = DateTime.UtcNow
''        Catch ex As Exception
''            Throw New Exception("Failure to parse line into cMXCLLogLine(). Error: " & ex.Message, ex)
''        End Try
''    End Sub

''    Public Overrides Function ToString() As String
''        If Not String.IsNullOrEmpty(GeneralMessage) Then
''            Return DateUTCTo_ISO8601(Timestamp) & " " & GeneralMessage
''        Else
''            Return DateUTCTo_ISO8601(Timestamp) & " " & "Sample Level = " & SampleLevel & " Benchmark = " & Benchmark
''        End If
''    End Function

''    Private Function DateUTCTo_ISO8601(ByRef dt As DateTime) As String
''        Return dt.ToString("s") & "Z"
''    End Function

''End Class

''#End Region 'CLASSES
