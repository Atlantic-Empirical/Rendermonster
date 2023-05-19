#Region "IMPORTS"

Imports SMT.Alentejo.Core.InstanceManagement
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.AWS.EC2
Imports SMT.Alentejo.Core
Imports SMT.AWS.Authorization
Imports SMT.AWS.S3
Imports System.IO
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.ApplicationManagement

#End Region 'IMPORTS

Public Class InstanceHealthService

#Region "FIELDS & PROPERTIES"

    Private HEAVY_TRACE As Boolean = True

    Private ReadOnly Property ServerType() As eSMT_ATJ_Server
        Get
            Return Environment.GetEnvironmentVariable("ALENTEJO_SERVER_TYPE")
        End Get
    End Property

#Region "PROPERTIES:INSTANCE INFO"

    Private ReadOnly Property InstanceId() As String
        Get
            If __InstanceId = "" Then __InstanceId = _InstanceId
            Return __InstanceId
        End Get
    End Property
    Private __InstanceId As String = ""

    ''' <summary>
    ''' This method calls the core code that checks the instance console output to see if 
    ''' windows is ready on this instance. Once the core method returns true once it is no 
    ''' longer called.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private ReadOnly Property _WindowsIsReady() As Boolean
        Get
            If Not __WindowsIsReady Then
                __WindowsIsReady = WindowsIsReady(InstanceId)
                Return __WindowsIsReady
            Else
                Return True
            End If
        End Get
    End Property
    Private __WindowsIsReady As Boolean = False

#End Region 'PROPERTIES:INSTANCE INFO

#Region "PROPERTIES:TIMER"

    Private WithEvents HEALTH_POLLING_TIMER As System.Timers.Timer
    Private POLL_INTERVAL As Byte = 90 '    'In seconds.
    Private FirstTimerTick As Boolean = True

#End Region 'PROPERTIES:TIMER

    Private ReadOnly Property S3Object() As cSMT_AWS_S3
        Get
            If _S3 Is Nothing Then _S3 = New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
            Return _S3
        End Get
    End Property
    Private _S3 As cSMT_AWS_S3

    Private ReadOnly Property RootProgramDirectory() As String
        Get
            Return "C:\Program Files\SMT\Alentejo\"
        End Get
    End Property

    Private ReadOnly Property Executables() As String()
        Get
            If Applications Is Nothing Then Return Nothing
            If ApplicationFiles Is Nothing Then Return Nothing
            Dim out As New List(Of String)
            For Each a As cSMT_ATJ_Application In Applications
                For Each f As cSMT_ATJ_File In ApplicationFiles
                    If f.AppId = a.Id And f._IsPrimaryExecutable Then
                        out.Add(f.FileName)
                    End If
                Next
            Next
            Return out.ToArray
        End Get
    End Property

    Private Applications As List(Of cSMT_ATJ_Application)
    Private ApplicationFiles As List(Of cSMT_ATJ_File)

    Private ReadOnly Property System_Password() As String
        Get
            Dim out As String
            Select Case ServerType
                Case eSMT_ATJ_Server.Centraal
                    out = "L173slso"
                Case Else
                    out = "dante"
            End Select
            LogMessage("System_Password=" & out)
            Return out
        End Get
    End Property
    Private System_Username As String

    Private ReadOnly Property UserLogon() As Boolean
        Get
            Dim out As Boolean = ServerType <> eSMT_ATJ_Server.Centraal
            LogMessage("UserLogon=" & out.ToString)
            Return out
        End Get
    End Property

#End Region 'FIELDS & PROPERITES

#Region "SERVICE EVENTS"

    Protected Overrides Sub OnStart(ByVal args() As String)
        SetupEventLog()
        LogMessage("SERVICE STARTED", EventLogEntryType.Information, False)
        LogMessage("Server type: " & ServerType.ToString, EventLogEntryType.Information, False)
        RetrieveExecutables()
    End Sub

    Protected Overrides Sub OnPause()
        LogMessage("SERVICE PAUSED", EventLogEntryType.Warning, False)
        HEALTH_POLLING_TIMER.Stop()
    End Sub

    Protected Overrides Sub OnContinue()
        LogMessage("SERVICE RESUMED", EventLogEntryType.Information, False)
        HEALTH_POLLING_TIMER.Start()
    End Sub

    Protected Overrides Sub OnStop()
        LogMessage("SERVICE STOPPED", EventLogEntryType.Warning, False)
        HEALTH_POLLING_TIMER.Stop()
    End Sub

    ''' <summary>
    ''' Allow changes in the timer interval.
    ''' </summary>
    ''' <param name="command">Number of minutes for timer.</param>
    ''' <remarks></remarks>
    Protected Overrides Sub OnCustomCommand(ByVal command As Integer)
        LogMessage("CUSTOM COMMAND = " & command, EventLogEntryType.Warning, False)
        POLL_INTERVAL = command
        HEALTH_POLLING_TIMER.Stop()
        HEALTH_POLLING_TIMER.Dispose()
        HEALTH_POLLING_TIMER = New System.Timers.Timer(1000 * 60 * POLL_INTERVAL)
        HEALTH_POLLING_TIMER.Start()
    End Sub

#End Region 'SERVICE EVENTS

#Region "EXECUTABLE RETRIEVAL"

    Private Sub RetrieveExecutables()
        Try
            LogMessage(System.Reflection.MethodBase.GetCurrentMethod.Name)
            Applications = GetAppListForServer(ServerType)
            ApplicationFiles = New List(Of cSMT_ATJ_File)
            Dim exePath As String
            For Each a As cSMT_ATJ_Application In Applications
                exePath = RetrieveApp(a.Id, RootProgramDirectory & a.Name & "\")
                If InStr(exePath.ToLower, "logreader") = 0 Then 'only run it if it is not the log reader
                    LocalRunProcess(exePath)
                End If
            Next
            AllExecutablesRetrievedSuccessfully_StartPolling()
        Catch ex As Exception
            LogMessage("Problem with RetrieveExecutables(). Error: " & ex.Message, EventLogEntryType.Error, True)
            System.Threading.Thread.Sleep(5000)
            RetrieveExecutables()
        End Try
    End Sub

    ''' <summary>
    ''' Returns the path to the primary executable.
    ''' </summary>
    ''' <param name="AppId"></param>
    ''' <param name="TargetPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function RetrieveApp(ByVal AppId As String, ByVal TargetPath As String) As String
        Try
            LogMessage(System.Reflection.MethodBase.GetCurrentMethod.Name)
            LogMessage("AppId: " & AppId & " TargetPath: " & TargetPath)

            Dim out As String = ""
            Dim fs As List(Of cSMT_ATJ_File) = GetApplicationFiles(AppId)

            'store the files for this app
            ApplicationFiles.AddRange(fs)

            'kill any processes for the existing executable
            For Each f As cSMT_ATJ_File In fs
                If f._IsPrimaryExecutable Then
                    KillProcesses(f.FileName)
                    System.Threading.Thread.Sleep(500)
                    Exit For
                End If
            Next

            'clean out existing directory or create it
            LogMessage("Emptying directory")
            If Directory.Exists(TargetPath) Then
                Dim di As New DirectoryInfo(TargetPath)
                For Each fsi As FileSystemInfo In di.GetFileSystemInfos
                    fsi.Delete()
                Next
            Else
                Directory.CreateDirectory(TargetPath)
            End If

            'download the files
            LogMessage("Starting downloads")
            For Each f As cSMT_ATJ_File In fs
                LogMessage("Start Download: " & f.FileName)
                S3Object.GetFileObject(s3_bucket_appfiles_us, f.Id, TargetPath & f.FileName, False)
                If f._IsPrimaryExecutable Then
                    out = TargetPath & f.FileName
                End If
            Next
            Return out
        Catch ex As Exception
            LogMessage("Problem with RetrieveExecutables(). Error: " & ex.Message, EventLogEntryType.Error, True)
            System.Threading.Thread.Sleep(5000)
            Return RetrieveApp(AppId, TargetPath) 'circular loop
        End Try
    End Function

    Private Sub AllExecutablesRetrievedSuccessfully_StartPolling()
        LogMessage(System.Reflection.MethodBase.GetCurrentMethod.Name)
        FirstTimerTick = True
        HEALTH_POLLING_TIMER = New System.Timers.Timer(1000 * POLL_INTERVAL)
        HEALTH_POLLING_TIMER.Start()
    End Sub

#End Region 'EXECUTABLE RETRIEVAL

#Region "PROCESS MANAGEMENT"

    Private Sub LocalRunProcess(ByVal ExePath As String)
        Try
            LogMessage("Attempting to run process: " & ExePath)
            Select Case ServerType
                Case eSMT_ATJ_Server.Centraal
                    Process.Start(ExePath)
                Case Else
                    RunProcess(ExePath, UserLogon, System_Username, System_Password)
            End Select
        Catch ex As Exception
            Throw New Exception("Problem with LocalRunProcess(). Error: " & ex.Message, ex)
        End Try
    End Sub

    ''' <summary>
    ''' Kills all processes of this name.
    ''' </summary>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Private Sub KillProcesses(ByVal e As String)
        Try
            LogMessage(System.Reflection.MethodBase.GetCurrentMethod.Name & " - " & e)
            Dim Ps() As Process = GetProcesses(e)
            LogMessage(Ps.Count & " processes to kill.")
            If Ps IsNot Nothing AndAlso Ps.Count > 1 Then
                For Each p As Process In Ps
                    LogMessage("Killing pid " & p.Id)
                    p.Kill()
                Next
            End If
        Catch ex As Exception
            LogMessage("Problem in KillProcesses(). Error: " & ex.Message & " StackTrace: " & ex.StackTrace, EventLogEntryType.Error, True)
        End Try
    End Sub

    Private Function GetProcessCount(ByVal e As String) As Byte
        Dim p() As Process = GetProcesses(e)
        If p Is Nothing Then Return 0
        Return p.Count
    End Function

    Private Function GetProcesses(ByVal e As String) As Process()
        Return Process.GetProcessesByName(Path.GetFileNameWithoutExtension(e))
    End Function

    Private Function GetProcess(ByVal e) As Process
        Dim p() As Process = GetProcesses(e)
        If p Is Nothing Then Return Nothing
        Return p(0)
    End Function

#End Region 'PROCESS MANAGEMENT

#Region "TIMER"

    Private Sub HEALTH_POLLING_TIMER_Tick(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles HEALTH_POLLING_TIMER.Elapsed
        If FirstTimerTick Then FirstTimerTick = False
        PerformHealthAudit(FirstTimerTick)
    End Sub

#End Region 'TIMER

#Region "HEALTH AUDIT"

    Private Sub PerformHealthAudit(Optional ByVal OnServiceStart As Boolean = False)
        Try
            If HEAVY_TRACE Then LogMessage("ENTERING | PerformHealthAudit().", EventLogEntryType.Information, False)

            If Not _WindowsIsReady() Then
                LogMessage("Windows is not ready.", EventLogEntryType.Information, False)
                Exit Sub
            End If

            'FIRST - check on and if needed sort out
            For Each e As String In Executables
                If Not IsAppHealthy(e) Then
                    If OnServiceStart Then
                        LogMessage("Starting " & e & " (on service startup).", EventLogEntryType.Information, False)
                    Else
                        LogMessage("DIAGNOSIS = " & e & " is not healthy." & vbNewLine & "Terminating any " & e & " Processes and attempting to restart one.", EventLogEntryType.Warning, True)
                    End If
                    KillProcesses(e)
                    For Each a As cSMT_ATJ_Application In Applications
                        For Each f As cSMT_ATJ_File In ApplicationFiles
                            If f.AppId = a.Id And f._IsPrimaryExecutable And f.FileName = e Then
                                If InStr(f.FileName.ToLower, "logreader") = 0 Then 'only run it if it is not the log reader
                                    LocalRunProcess(RootProgramDirectory & a.Name & "\" & f.FileName)
                                End If
                            End If
                        Next
                    Next
                Else
                    'LogMessage("DIAGNOSIS = " & e & " is healthy.", EventLogEntryType.Information, False)
                End If
            Next

            'SECOND
            HealthPing(InstanceId)

            If HEAVY_TRACE Then LogMessage("EXITING | PerformHealthAudit().", EventLogEntryType.Information, False)
        Catch ex As Exception
            LogMessage("Problem in PerformHealthAudit(). Error: " & ex.Message & " StackTrace: " & ex.StackTrace, EventLogEntryType.Error, True)
        End Try
    End Sub

    Private Function IsAppHealthy(ByVal e As String) As Boolean
        If HEAVY_TRACE Then LogMessage("ENTERING | IsAppHealthy()")
        Select Case GetProcessCount(e)
            Case 0
                Return False
            Case 1
                ' we're ok
            Case Is > 1
                LogMessage("More than one " & e & " was found to be running.")
                Return False
        End Select

        If Not GetProcess(e).Responding Then Return False

        If HEAVY_TRACE Then LogMessage("EXITING | IsAppHealthy()")
        Return True
    End Function

#End Region 'HEALTH AUDIT

#Region "LOGGING"

    Private log_name As String = "SMT Alentejo"
    Private log_source As String = "Instance Health Service"

    Private Sub SetupEventLog()
        If Not EventLog.Exists(log_name) Or Not EventLog.SourceExists(log_source) Then
            EventLog.CreateEventSource(log_source, log_name)
        Else
            ClearEventLog() 'clear the log each time the service starts
        End If
    End Sub

    Private Sub LogMessage(ByVal Msg As String, Optional ByVal EntryType As EventLogEntryType = EventLogEntryType.Information, Optional ByVal SendToAlentejoSystemMessageQueue As Boolean = False)
        Try
            'WRITE TO LOCAL SYSTEM LOG
            AlentejoInstanceHealthEventLog.WriteEntry(Msg, EntryType)
            'WriteEvent(Msg, log_source, EventLogEntryType.Information, eSystemLog.System)
        Catch ex As Exception
            Try
                'SEND TO THE ALENTEJO SYSTEM EXCEPTION QUEUE
                SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, InstanceId & " - " & Process.GetCurrentProcess.Id, ex.Message, ex.StackTrace)
            Catch ex1 As Exception
                'eat it
            End Try
        End Try
        If SendToAlentejoSystemMessageQueue Then
            Try
                'SEND TO THE ALENTEJO SYSTEM EXCEPTION QUEUE
                SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, InstanceId & " - " & Process.GetCurrentProcess.Id, Msg, EntryType.ToString)
            Catch ex As Exception
                'eat it
            End Try
        End If
    End Sub

    Private Sub ClearEventLog()
        AlentejoInstanceHealthEventLog.Clear()
    End Sub

#End Region 'LOGGING

End Class
