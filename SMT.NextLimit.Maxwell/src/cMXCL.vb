Imports System.IO
Imports System.Text
Imports SMT.NextLimit.Maxwell.Win.ProcessExecution
Imports System.Drawing
Imports System.Drawing.Imaging

Public Class cMXCL
    Implements IDisposable

#Region "FIELDS & PROPERTIES"

    Private ReadOnly Property MXCL_PATH() As String
        Get
            'Return "cmd"
            'Return WrapStringInQuotes("T:\SOURCE_CODE_WORKING\SMT\Applications\UtilityApps\ConsoleApp_JustWritesToStdOutUntilKilled\bin\Debug\TestConsole.exe")
            Dim path As String = Environment.GetEnvironmentVariable("MAXWELL_ROOT") & "\mxcl.exe"
            If path = "" OrElse Not File.Exists(path) Then
                Throw New Exception("Maxwell is not installed.")
            Else
                Return WrapStringInQuotes(path)
            End If
        End Get
    End Property

    Public RenderStartTicks As Long
    Public OutputImagePath As String

#End Region 'FIELDS & PROPERTIES

#Region "EVENTS"

    Public Event evGeneralLog(ByVal msg As String)
    Public Event evProgressUpdate(ByRef LogLine As cMXCLLogLine)
    Public Event evProcessExit()
    Public Event evNewSampleLevel(ByVal NewLevel As Double)

#End Region 'EVENTS

#Region "CONSTRUCTOR - DESTRUCTOR"

    Public Sub Dispose() Implements IDisposable.Dispose
        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.Dispose() - ENTER")
        If MXCL IsNot Nothing Then
            MXCL.Shutdown()
            MXCL.Dispose()
        End If
        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.Dispose() - EXIT")
    End Sub

#End Region 'CONSTRUCTOR - DESTRUCTOR

#Region "COMMAND LINE"

#Region "COMMAND LINE:ARGUMENTS"

#Region "COMMAND LINE:ARGUMENTS:REQUIRED"

    Public WriteOnly Property SETTING_MXS_PATH() As String
        Set(ByVal value As String)
            If Not File.Exists(value) Then Throw New Exception("MXS file does not exist.")
            cl_mxs_path = "-mxs:" & WrapStringInQuotes(value)
        End Set
    End Property
    Public cl_mxs_path As String = ""

    Public WriteOnly Property SETTING_RESOLUTION() As System.Drawing.Size
        Set(ByVal value As System.Drawing.Size)
            'If value.Width = 0 Or value.Height = 0 Then Throw New Exception("Invalid resolution.")
            'TODO: get max values from NL.
            If value.Width = 0 Then value.Width = 650
            If value.Height = 0 Then value.Height = 400
            cl_resolution = "-res:" & value.Width & "x" & value.Height
        End Set
    End Property
    Public cl_resolution As String = ""

    ''' <summary>
    ''' The full path including file name of the output target.
    ''' The extension of the output file specified here determines the 
    ''' output file format.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property SETTING_OUTPUT_PATH() As String
        Set(ByVal value As String)
            If Not Path.HasExtension(value) Then Throw New Exception("Invalid output path. Filename must be included.")
            Dim ext As String = Path.GetExtension(value)
            Select Case ext.ToLower
                Case ".tga", ".jpg", ".tif", ".png", ".bmp"
                    'acceptable
                Case Else
                    Throw New Exception("Invalid output path. File type is not supported.")
            End Select
            If Directory.Exists(Path.GetDirectoryName(value)) Then Directory.Delete(Path.GetDirectoryName(value), True)
            Directory.CreateDirectory(Path.GetDirectoryName(value))
            cl_output_path = "-output:" & WrapStringInQuotes(value)
            OutputImagePath = value

            ''SETUP FILE SYSTEM WATCHER
            'SetupFileSystemWatcher(value)
        End Set
    End Property
    Public cl_output_path As String = ""

#End Region 'COMMAND LINE:ARGUMENTS:REQUIRED

#Region "COMMAND LINE:ARGUMENTS:REQUIRED:USE DEFAULT VALUES"

    Public WriteOnly Property SETTING_MAX_TIME() As UInt16
        Set(ByVal value As UInt16)
            'Don't spend more than 24 hours per frame!
            'This should help  keep EC2 instances from running forever.
            If value > 1440 Then Throw New Exception("Invalid render time.")
            If value = 0 Then
                cl_max_time = ""
            Else
                cl_max_time = "-time:" & value
            End If
        End Set
    End Property
    Public cl_max_time As String = "-time:360" 'default to six hours per frame

    Public WriteOnly Property SETTING_SAMPLE_LEVELS() As Double
        Set(ByVal value As Double)
            If value = 0 Then Throw New Exception("Invalid sample levels.")
            cl_sample_levels = "-sampling:" & value.ToString
        End Set
    End Property
    Public cl_sample_levels As String = "-sampling:25" 'default to 25 sample levels

#End Region 'COMMAND LINE:ARGUMENTS:REQUIRED:USE DEFAULT VALUES

#Region "COMMAND LINE:ARGUMENTS:OPTIONAL"

    Public WriteOnly Property SETTING_ANIMATION() As String
        Set(ByVal value As String)
            If value = "" Then
                cl_animation = ""
            Else
                cl_animation = "-animation:" & value
            End If
        End Set
    End Property
    Public cl_animation As String = ""

    Public WriteOnly Property SETTING_CAMERA_NAME() As String
        Set(ByVal value As String)
            If value = "" Then
                cl_camera_name = ""
            Else
                If InStr(value, " ") Then
                    cl_camera_name = "-camera:" & WrapStringInQuotes(value)
                Else
                    cl_camera_name = "-camera:" & value
                End If
            End If
        End Set
    End Property
    Public cl_camera_name As String = ""

    Public WriteOnly Property SETTING_MULTILIGHT() As Boolean
        Set(ByVal value As Boolean)
            If value Then
                cl_multilight = "-ml"
            Else
                cl_multilight = ""
            End If
        End Set
    End Property
    Public cl_multilight As String = ""

    ''' <summary>
    ''' Only required if MXI should be placed somewhere other than the same directory as Output file.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property SETTING_MXI_PATH() As String
        Set(ByVal value As String)
            If value = "" Then
                cl_mxi_path = ""
            Else
                If Not Directory.Exists(Path.GetDirectoryName(value)) Then Directory.CreateDirectory(Path.GetDirectoryName(value))
                cl_mxi_path = "-mxi:" & WrapStringInQuotes(value)
            End If
        End Set
    End Property
    Public cl_mxi_path As String = ""

    Public WriteOnly Property SETTING_VIGNETTING() As UInt16
        Set(ByVal value As UInt16)
            If value > 1000 Then Throw New Exception("Invalid Vignetting value. Must be less than or equal to 1000.")
            If value = 0 Then
                cl_vignetting = ""
            Else
                cl_vignetting = "-vignetting:" & value
            End If
        End Set
    End Property
    Public cl_vignetting As String = ""

    Public WriteOnly Property SETTING_SCATTERING_LENS() As UInt16
        Set(ByVal value As UInt16)
            If value > 1000 Then Throw New Exception("Invalid Scatterling Lens value. Must be less than or equal to 1000.")
            If value = 0 Then
                cl_scattering_lens = ""
            Else
                cl_scattering_lens = "-scatteringLens:" & value
            End If
        End Set
    End Property
    Public cl_scattering_lens As String = ""

    Public WriteOnly Property SETTING_BITMAP_PATHS() As String
        Set(ByVal value As String)
            If value = "" Then
                cl_bitmaps_path = ""
            Else
                If value = "0" Then
                    cl_bitmaps_path = "-bitmaps:0"
                Else
                    If Not Directory.Exists(value) Then Directory.CreateDirectory(value)
                    cl_bitmaps_path = "-bitmaps:" & WrapStringInQuotes(value)
                End If
            End If
        End Set
    End Property
    Public cl_bitmaps_path As String = "-bitmaps:0"

    ''' <summary>
    ''' Set to the IP address of the Maxwell Network Render Manager.
    ''' </summary>
    ''' <value></value>
    ''' <remarks></remarks>
    Public WriteOnly Property SETTING_IS_SERVER() As String
        Set(ByVal value As String)
            If value Is "" Then
                cl_server = ""
            Else
                If Not IsValidIP(value) Then Throw New Exception("Invalid IP address.")
                cl_server = "-server:" & value
                cl_manager = ""
            End If
        End Set
    End Property
    Public cl_server As String = ""

    Public WriteOnly Property SETTING_IS_MANAGER() As Boolean
        Set(ByVal value As Boolean)
            If value Then
                cl_manager = "-manager"
                cl_server = "" 'make sure we're not accidentially both!
            Else
                cl_manager = ""
            End If
        End Set
    End Property
    Public cl_manager As String = ""

    Public WriteOnly Property SETTING_RENDER_ENGINE_IS_PREVIEW() As Boolean
        Set(ByVal value As Boolean)
            If value Then
                cl_render_engine = "-rs:0"
            Else
                cl_render_engine = "-rs:1"
            End If
        End Set
    End Property
    Public cl_render_engine As String = "-rs:1"

#Region "COMMAND LINE:ARGUMENTS:OPTIONAL:CHANNELS"

    Public SETTING_CHANNELS_RENDER As Boolean = True
    Public SETTING_CHANNELS_ALPHA As Boolean = True
    Public SETTING_CHANNELS_OPAQUE_ALPHA As Boolean = False
    Public SETTING_CHANNELS_SHADOW As Boolean = False
    Public SETTING_CHANNELS_MATERIAL As Boolean = False
    Public SETTING_CHANNELS_OBJECTID As Boolean = False
    Public SETTING_CHANNELS_ZBUFFER As Boolean = False
    Public SETTING_CHANNELS_ZBUFFER_MAX As UInt16 = 0
    Public SETTING_CHANNELS_ZBUFFER_MIN As UInt16 = UInt16.MaxValue

    Private ReadOnly Property cl_channels() As String
        Get
            Dim sb As New StringBuilder
            If SETTING_CHANNELS_RENDER Then sb.Append("r,")
            If SETTING_CHANNELS_ALPHA Then sb.Append("a,")
            If SETTING_CHANNELS_OPAQUE_ALPHA Then sb.Append("ao,")
            If SETTING_CHANNELS_SHADOW Then sb.Append("s,")
            If SETTING_CHANNELS_MATERIAL Then sb.Append("m,")
            If SETTING_CHANNELS_OBJECTID Then sb.Append("o,")
            If SETTING_CHANNELS_ZBUFFER Then
                sb.Append("z<" & SETTING_CHANNELS_ZBUFFER_MIN & ">,<" & SETTING_CHANNELS_ZBUFFER_MAX & ">")
            End If
            Dim out As String = sb.ToString
            If out.Length > 0 AndAlso out.Chars(out.Length - 1) = "," Then
                out = Left(out, out.Length - 1)
            End If
            If out = "" Then
                Return out
            Else
                Return "-channels:" & out
            End If
        End Get
    End Property

#End Region 'COMMAND LINE:ARGUMENTS:OPTIONAL:CHANNELS

#End Region 'COMMAND LINE:ARGUMENTS:OPTIONAL

#Region "COMMAND LINE:ARGUMENTS:INTERNAL"

    'INTERNAL DEFAULT VALUE

    Public WriteOnly Property SETTING_THREADS() As Byte
        Set(ByVal value As Byte)
            cl_threads = "-threads:" & value
        End Set
    End Property
    Public cl_threads As String = "-threads:0"

    '#If DEBUG Then
    '    ''' <summary>
    '    ''' Maxwell defaults to the number of cores if this value is 0.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public cl_threads As String = "-threads:1"
    '#Else
    '    ''' <summary>
    '    ''' Maxwell defaults to the number of cores if this value is 0.
    '    ''' </summary>
    '    ''' <remarks></remarks>
    '    Public cl_threads As String = "-threads:0"
    '#End If

    Public cl_no_wait As String = "-nowait" '-nowait
    Public cl_stdout As String = "-stdout" '-stdout
    Public cl_silent As String = "" '-silent
    Private ReadOnly Property cl_cpu_id() As String '-idcpu:<id>
        Get
            Dim r As New Random
            Return "-idcpu:" & r.Next(0, 9999)
        End Get
    End Property
    Private cl_display As String = "" '-display

#If DEBUG Then
    Private cl_priority As String = "-priority:low" '-priority:low
#Else
    Private cl_priority As String = ""
#End If

#End Region 'COMMAND LINE:ARGUMENTS:INTERNAL

#End Region 'COMMAND LINE:ARGUMENTS

#Region "COMMAND LINE:CONSTRUCTION"

    Public Function BuildCommandLine() As String
        Try
            Dim sb As New StringBuilder

            ' MANDATORY ARGUMENTS
            If cl_mxs_path = "" Then
                Throw New Exception("Invalid cl_mxs_path.")
            Else
                sb.Append(cl_mxs_path & " ")
            End If
            If cl_resolution = "" Then
                Throw New Exception("Invalid cl_resolution.")
            Else
                sb.Append(cl_resolution & " ")
            End If
            If cl_sample_levels = "" Then
                Throw New Exception("Invalid cl_sample_levels.")
            Else
                sb.Append(cl_sample_levels & " ")
            End If
            If cl_output_path = "" Then
                Throw New Exception("Invalid cl_output_path.")
            Else
                sb.Append(cl_output_path & " ")
            End If

            ' INTERNAL ARGUMENTS
            sb.Append(cl_threads & " ")
            sb.Append(cl_no_wait & " ")
            sb.Append(cl_stdout & " ")
            If cl_silent <> "" Then sb.Append(cl_silent & " ")
            If cl_cpu_id <> "" Then sb.Append(cl_cpu_id & " ")
            If cl_display <> "" Then sb.Append(cl_display & " ")
            If cl_priority <> "" Then sb.Append(cl_priority & " ")

            ' OPTIONAL USER ARGUMENTS
            If cl_max_time <> "" Then sb.Append(cl_max_time & " ")
            If cl_animation <> "" Then sb.Append(cl_animation & " ")
            If cl_camera_name <> "" Then sb.Append(cl_camera_name & " ")
            If cl_multilight <> "" Then sb.Append(cl_multilight & " ")
            If cl_mxi_path <> "" Then sb.Append(cl_mxi_path & " ")
            If cl_vignetting <> "" Then sb.Append(cl_vignetting & " ")
            If cl_scattering_lens <> "" Then sb.Append(cl_scattering_lens & " ")
            If cl_bitmaps_path <> "" Then sb.Append(cl_bitmaps_path & " ")
            If cl_server <> "" Then sb.Append(cl_server & " ")
            If cl_manager <> "" Then sb.Append(cl_manager & " ")
            If cl_render_engine <> "" Then sb.Append(cl_render_engine & " ")
            If cl_channels <> "" Then sb.Append(cl_channels & " ")

            Dim out As String = sb.ToString
            Debug.WriteLine("MAXWELL COMMAND LINE = ")
            Debug.WriteLine(out)
            Return out
        Catch ex As Exception
            Throw New Exception("Problem with BuildCommandLine(). Error: " & ex.Message, ex)
        End Try
    End Function

#End Region 'COMMAND LINE:CONSTRUCTION

#End Region 'COMMAND LINE

#Region "MXCL PROCESS"

    Private WithEvents MXCL As cProcessManager

    Public Sub Run()
        Try
            'OutputFileNumber = 0

            Dim CL As String = BuildCommandLine()
            RaiseEvent evGeneralLog("COMMAND = " & MXCL_PATH & " " & CL)
            MXCL = New cProcessManager("123456", "MyMXCL", MXCL_PATH, CL)
            'ProgressMonitor = New cSMT_MXCL_ProgressMonitor()
            If Not MXCL.StartProcess() Then Throw New Exception("Failed to start the process.")
            RenderStartTicks = DateTime.UtcNow.Ticks

            ''Dim cmdpth As String = "C:\Windows\System32\cmd.exe"
            'Dim args As String = "/C mxcl -mxs:" & Chr(34) & "N:\PRODUCT DEVELOPMENT\Alentejo\Maxwell Projects\Vase diamond\vase diamond.mxs" & Chr(34) & " -nowait -stdout"
            'MXCL = New cProcessManager("123456", "MyMXCL", MXCL_PATH, args)
            'If Not MXCL.StartProcess() Then Throw New Exception("Failed to start the process.")

            'MXCLThread = New Thread(AddressOf MXCL.StartProcess)
            'MXCLThread.SetApartmentState(ApartmentState.MTA)
            'MXCLThread.Start()

            'MXCL = New cProcessManager("123456", "MyMXCL", MXCL_PATH) ', CL
            'MXCL.RunProcess(CL)
        Catch ex As Exception
            Throw New Exception("Problem with Run(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub HandleMXCL_stderr_stdout(ByVal ClientProcessID As Integer, ByVal Name As String, ByVal PID As Integer, ByVal Line As String) Handles MXCL.evLogLineOutput
        ParseConsoleData(Line)
    End Sub

    Public Sub HandleMXCL_ProcessExit(ByVal ClientProcessID As Integer, ByVal Name As String, ByVal PID As Integer, ByVal Succeeded As Boolean) Handles MXCL.evProcessExited
        'ProgressMonitor.Dispose()
        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.HandleMXCL_ProcessExit()")
        RaiseEvent evProcessExit()
    End Sub

#End Region 'MXCL PROCESS

#Region "RENDER PROGRESS"

#Region "PROGRESS:PROPERTIES"

    Public Benchmark As Double = 0
    Public Property SampleLevel() As Double
        Get
            Return _SampleLevel
        End Get
        Set(ByVal value As Double)
            If value = Nothing Then Exit Property
            If value <> _SampleLevel Then
                RaiseEvent evNewSampleLevel(value)
                GrabImageForSampleLevel(value)
            End If
            _SampleLevel = value
        End Set
    End Property
    Private _SampleLevel As Double = 0

    Public ReadOnly Property LatestLogLine() As cMXCLLogLine
        Get
            If _LogLines Is Nothing Then Return Nothing
            Return _LogLines(_LogLines.Count - 1)
        End Get
    End Property

    'Public ReadOnly Property LogLines() As cMXCLLogLine()
    '    Get

    '    End Get
    'End Property
    Private _LogLines As List(Of cMXCLLogLine)

#End Region 'PROGRESS:PROPERTIES

    Private Sub ParseConsoleData(ByVal line As String)
        Try
            If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.ParseConsoleData() - ENTER")
            If String.IsNullOrEmpty(line) Then Exit Sub
            Dim tmpLL As New cMXCLLogLine(line)
            If _LogLines Is Nothing Then _LogLines = New List(Of cMXCLLogLine)
            _LogLines.Add(tmpLL)
            Benchmark = tmpLL.Benchmark
            SampleLevel = tmpLL.SampleLevel
            RaiseEvent evProgressUpdate(tmpLL)
            If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.ParseConsoleData() - EXIT")
        Catch ex As Exception
            Throw New Exception("Problem with ParseConsoleData(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'RENDER PROGRESS

#Region "FILE SYSTEM WATCHER"

    Public Event evIntermediateImageAvailable(ByVal ImgPath As String)

    Private Sub GrabImageForSampleLevel(ByVal NewSampleLevel As Double)
        Try
            If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.GrabImageForSampleLevel() - ENTER")
            Dim SampleLevelString As String = NewSampleLevel.ToString.Replace(".", "-").PadLeft(3, "0")
            'If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog(NewSampleLevel)
            'If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog(NewSampleLevel.ToString)


            ' OLD - WORKS
            Dim ts As New TimeSpan(DateTime.UtcNow.Ticks - RenderStartTicks)

            'OUTPUT IMAGE
            Dim ImgSavePth As String = Path.GetDirectoryName(OutputImagePath) & "\SL" & SampleLevelString & "_" & CInt(ts.TotalSeconds) & Path.GetExtension(OutputImagePath)
            File.Copy(OutputImagePath, ImgSavePth)
            Dim fi As New FileInfo(ImgSavePth)
            If fi.Length = 0 Then
                fi.Delete()
                Exit Sub
            End If

            RaiseEvent evIntermediateImageAvailable(ImgSavePth)

            'OUTPUT ALPHA
            ' for now we're just saving off copies at each level, not sending them to S3 until we know that is needed
            Dim AlpSrcPth As String = Path.GetDirectoryName(OutputImagePath) & "\" & Path.GetFileNameWithoutExtension(OutputImagePath) & "_alpha" & Path.GetExtension(OutputImagePath)
            If File.Exists(AlpSrcPth) Then
                Dim AlpTgtPth As String = Path.GetDirectoryName(OutputImagePath) & "\SL" & SampleLevelString & "_" & Path.GetFileNameWithoutExtension(OutputImagePath) & "_alpha_" & CInt(ts.TotalSeconds) & Path.GetExtension(OutputImagePath)
                Dim AlpG As String = Guid.NewGuid.ToString
                File.Copy(AlpSrcPth, AlpTgtPth)
            End If

            If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.GrabImageForSampleLevel() - EXIT")

        Catch ex As Exception
            Throw New Exception("Problem with GrabImageForSampleLevel(). Error: " & ex.Message, ex)
        End Try
    End Sub

    'Private WithEvents OUTPUT_MONITOR_TIMER As System.Timers.Timer

    'Private Sub OUTPUT_MONITOR_TIMER_Start()
    '    OUTPUT_MONITOR_TIMER = New System.Timers.Timer(10000)
    '    OUTPUT_MONITOR_TIMER.Start()
    'End Sub

    'Private Sub OUTPUT_MONITOR_TIMER_Kill()
    '    OUTPUT_MONITOR_TIMER.Stop()
    '    OUTPUT_MONITOR_TIMER.Dispose()
    'End Sub

    'Private Sub OUTPUT_MONITOR_TIMER_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles OUTPUT_MONITOR_TIMER.Elapsed

    'End Sub

    'Private WithEvents FSW As FileSystemWatcher
    'Public Event evIntermediateImageAvailable(ByVal ImgPath As String)
    'Private OutputFileNumber As UInt16 = 1

    'Private Sub SetupFileSystemWatcher(ByVal job_output_image As String)
    '    FSW = New FileSystemWatcher(Path.GetDirectoryName(job_output_image))
    '    FSW.Filter = Path.GetFileName(Path.GetFileName(job_output_image))
    '    FSW.NotifyFilter = NotifyFilters.LastWrite 'Or NotifyFilters.CreationTime Or NotifyFilters.LastAccess Or NotifyFilters.Size
    '    FSW.IncludeSubdirectories = False
    '    FSW.EnableRaisingEvents = True
    'End Sub

    'Private Sub FSW_OnChanged(ByVal sender As Object, ByVal e As FileSystemEventArgs) Handles FSW.Created, FSW.Changed
    '    Try
    '        '' NEW - trying to be faster and more flexible
    '        'Dim FS As New FileStream(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)
    '        'Dim b(FS.Length - 1) As Byte
    '        'FS.Read(b, 0, FS.Length)
    '        'FS.Close()

    '        'Try
    '        '    Dim MS As New MemoryStream(b)
    '        '    Dim bm As New Bitmap(MS)

    '        '    Dim ts As New TimeSpan(DateTime.UtcNow.Ticks - RenderStartTicks)
    '        '    Dim MasterSavePath As String = Path.GetDirectoryName(e.FullPath) & "\" & OutputFileNumber.ToString.PadLeft(3, "0") & "_" & Path.GetFileNameWithoutExtension(e.Name) & "_" & CInt(ts.TotalSeconds)
    '        '    Dim PreviewSavePth As String = MasterSavePath & ".jpg"
    '        '    'MasterSavePath &= Path.GetExtension(e.Name)

    '        '    'SAVE THE JPEG - the files output here should be used for all preview activities in Silverlight
    '        '    Dim ici As ImageCodecInfo = GetEncoderInfo("image/jpeg")
    '        '    Dim eps As New EncoderParameters(1)
    '        '    eps.Param(0) = New EncoderParameter(Imaging.Encoder.Quality, 100)
    '        '    bm.Save(PreviewSavePth, ici, eps)
    '        '    'bm.Save(ImgSavePth & "_preview.jpg", Imaging.ImageFormat.Jpeg)
    '        '    bm.Dispose()
    '        '    bm = Nothing

    '        '    ''SAVE THE NATIVE IMAGE - the files output here are the 'master' that the user should be able to download (targa, tiff, bmp, jpg, png, whatever is supported by the render engine)
    '        '    'FS = New FileStream(MasterSavePath, FileMode.OpenOrCreate, FileAccess.ReadWrite)
    '        '    'FS.Write(b, 0, b.Length)
    '        '    'FS.Close()

    '        '    'CLEAN UP
    '        '    MS.Dispose()
    '        '    MS = Nothing

    '        '    RaiseEvent evIntermediateImageAvailable(PreviewSavePth)

    '        '    OutputFileNumber += 1
    '        'Catch ex As Exception
    '        '    Debug.WriteLine("ERROR: problem in FSW_OnChanged. In image handling code.")
    '        'End Try

    '        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.FSW_OnChanged() - ENTER")

    '        ' OLD - WORKS
    '        Dim ts As New TimeSpan(DateTime.UtcNow.Ticks - RenderStartTicks)

    '        'OUTPUT IMAGE
    '        Dim ImgSavePth As String = Path.GetDirectoryName(e.FullPath) & "\" & OutputFileNumber.ToString.PadLeft(3, "0") & "_inter_SL" & Replace(CStr(SampleLevel), ".", "-") & "_" & CInt(ts.TotalSeconds) & Path.GetExtension(e.Name)
    '        File.Copy(e.FullPath, ImgSavePth)
    '        Dim fi As New FileInfo(ImgSavePth)
    '        If fi.Length = 0 Then
    '            fi.Delete()
    '            Exit Sub
    '        End If

    '        RaiseEvent evIntermediateImageAvailable(ImgSavePth)

    '        'OUTPUT ALPHA
    '        ' for now we're just saving off copies at each level, not sending them to S3 until we know that is needed
    '        Dim AlpSrcPth As String = Path.GetDirectoryName(e.FullPath) & "\" & Path.GetFileNameWithoutExtension(e.Name) & "_alpha" & Path.GetExtension(e.Name)
    '        If File.Exists(AlpSrcPth) Then
    '            Dim AlpTgtPth As String = Path.GetDirectoryName(e.FullPath) & "\" & OutputFileNumber.ToString.PadLeft(3, "0") & "_" & Path.GetFileNameWithoutExtension(e.Name) & "_alpha_" & CInt(ts.TotalSeconds) & Path.GetExtension(e.Name)
    '            Dim AlpG As String = Guid.NewGuid.ToString
    '            File.Copy(AlpSrcPth, AlpTgtPth)
    '        End If

    '        OutputFileNumber += 1

    '        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.FSW_OnChanged() - EXIT")

    '    Catch ex As Exception
    '        Throw New Exception("Problem with FSW_OnChanged(). Error: " & ex.Message, ex)
    '    End Try
    'End Sub

    Private Function GetEncoderInfo(ByVal mimeType As String) As ImageCodecInfo
        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.GetEncoderInfo() - ENTER")
        Dim j As Integer = 0
        Dim encoders As ImageCodecInfo() = Nothing
        encoders = ImageCodecInfo.GetImageEncoders()
        For j = 0 To encoders.Length
            If encoders(j).MimeType = mimeType Then
                Return encoders(j)
            End If
        Next
        If My.Settings.TRACE_LEVEL > 2 Then RaiseEvent evGeneralLog("cSMT_MXCL.GetEncoderInfo() - EXIT")
        Return Nothing
    End Function

#End Region 'FILE SYSTEM WATCHER

#Region "HELPER CODE"

    ''' <summary>
    ''' method to validate an IP address
    ''' using regular expressions. The pattern
    ''' being used will validate an ip address
    ''' with the range of 1.0.0.0 to 255.255.255.255
    ''' </summary>
    ''' <param name="addr">Address to validate</param>
    ''' <returns></returns>
    Public Shared Function IsValidIP(ByVal addr As String) As Boolean
        'create our match pattern
        Dim pattern As String = "^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\." & _
                                "([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$"
        'create our Regular Expression object
        Dim check As New RegularExpressions.Regex(pattern)
        'boolean variable to hold the status
        Dim valid As Boolean = False
        'check to make sure an ip address was provided
        If addr = "" Then
            'no address provided so return false
            valid = False
        Else
            'address provided so use the IsMatch Method
            'of the Regular Expression object
            valid = check.IsMatch(addr, 0)
        End If
        'return the results
        Return valid
    End Function

#End Region 'HELPER CODE

End Class

#Region "CLASSES"

<Serializable()> _
Public Class cMXCLLogLine

    Public Timestamp As DateTime
    Public SampleLevel As Double
    Public Benchmark As Double
    Public GeneralMessage As String
    Public Elapsed As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal Line As String)
        Try

            'NEW - works with stdout messages from mxcl
            If Left(Line, 2) = "SL" Then
                'EXAMPLES:
                '   "SL of 5.00. Benchmark of 72.218. Time: 1m27s"

                'we have a render progress update, parse it for interesting data
                Dim pos_SL_of As Integer = InStr(Line, "SL of ")
                Dim pos_Benchmark_of As Integer = InStr(Line, "Benchmark of")
                SampleLevel = Val(Mid(Line, pos_SL_of + 6, pos_Benchmark_of - pos_SL_of + 6))
                Dim pos_Time As Integer = InStr(Line, "Time:")
                Benchmark = Val(Mid(Line, pos_Benchmark_of + 13, pos_Time - pos_Benchmark_of + 13))
                Elapsed = Right(Line, Line.Length - pos_Time - 5)
            Else
                'EXAMPLES:
                GeneralMessage = Line
            End If
            Timestamp = DateTime.UtcNow




            'OLD - works with scraped messages
            'If InStr(Line, "[") = 1 Then
            '    'we have a time stamp
            '    Dim pos_ts_close As Integer = InStr(Line, "]")
            '    If Mid(Line, pos_ts_close + 1, 3) = " SL" Then
            '        'EXAMPLES:
            '        '   [09/December/2008 13:04:42] SL of 2.00. Benchmark of 30.963. Time: 22s

            '        'we have a render progress update, parse it for interesting data
            '        Dim pos_SL_of As Integer = InStr(Line, "SL of ")
            '        Dim pos_Benchmark_of As Integer = InStr(Line, "Benchmark of")
            '        SampleLevel = Val(Mid(Line, pos_SL_of + 6, pos_Benchmark_of - pos_SL_of + 6))
            '        Dim pos_Time As Integer = InStr(Line, "Time:")
            '        Benchmark = Val(Mid(Line, pos_Benchmark_of + 13, pos_Time - pos_Benchmark_of + 13))
            '    Else
            '        'EXAMPLES:
            '        '   [09/December/2008 13:02:49] Reading MXS:N:/PRODUCT DEVELOPMENT/Alentejo/Maxwell Projects/Vase diamond/Vase diamond.mxs
            '        '   [09/December/2008 13:04:58] Render finished succesfully 
            '        GeneralMessage = Mid(Line, pos_ts_close + 2)
            '    End If
            'Else
            '    'All lines that do not begin with a time stamp.
            '    If Line = "" Then Line = " "
            '    GeneralMessage = Line
            'End If
            'Timestamp = DateTime.UtcNow
        Catch ex As Exception
            Throw New Exception("Failure to parse line into cMXCLLogLine(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Overrides Function ToString() As String
        Try
            If Not String.IsNullOrEmpty(GeneralMessage) Then
                Return DateUTCTo_ISO8601(Timestamp) & " " & GeneralMessage
            Else
                Return DateUTCTo_ISO8601(Timestamp) & " " & "Sample Level = " & SampleLevel & " Benchmark = " & Benchmark & " Elapsed = " & Elapsed
            End If
        Catch ex As Exception
            Throw New Exception("Problem with ToString(). Error: " & ex.Message, ex)
        End Try
    End Function

    Private Function DateUTCTo_ISO8601(ByRef dt As DateTime) As String
        Return dt.ToString("s") & "Z"
    End Function

End Class

#End Region 'CLASSES
