'Imports SMT.NextLimit.Maxwell
Imports System.IO

Public Class MaxwellTestHarness_Main

    Private WithEvents MXCL As cMXCL
    'Private WithEvents MXIMERGE As cMXIMERGE

    Private Sub MaxwellTestHarness_Main_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If MXCL IsNot Nothing Then MXCL.Dispose()
    End Sub

    Private Sub btnScrapEen_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScrapEen.Click
        'RunTestRender()
        Try
            Dim rnsMxiPath As String = "\\10.248.163.63\alentejo_render_engine_output\b2d85360-6116-423c-b629-04048fd5c853\render_output.mxi"
            If Not File.Exists(rnsMxiPath) Then
                AddConsoleLine("MXI is not reachable...")
            End If
            AddConsoleLine("MXI is reachable.")
        Catch ex As Exception
            MsgBox("ScrapEenFailed. Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        'AddConsoleLine(ScrapeConsole)

        'Dim ll As New cMXCLLogLine("[09/December/2008 10:38:35] SL of 2.26. Benchmark of 15.992. Time: 51s")
        'Debug.WriteLine(ll.ToString)

        'DeleteDirectoryContents("C:\ALENTEJO\ALENTEJO_RENDER_ENGINE_OUTPUT")
        Dim stuff() As String = CallTestMethod(txtMXSPath.Text)
        For Each s As String In stuff
            AddConsoleLine(s)
        Next
        AddConsoleLine("---")

    End Sub

    Private Sub RunTestRender()
        Try
            MXCL = New cMXCL
            With MXCL

                'MANDATORY
                .SETTING_MXS_PATH = "N:\PRODUCT DEVELOPMENT\Alentejo\Maxwell Projects\Simple Rect\SimpleRect.mxs"
                .SETTING_RESOLUTION = New System.Drawing.Size(600, 450)
                .SETTING_OUTPUT_PATH = "C:\Temp\ALENTEJO_MAXWELL_OUTPUT\" & "testOut_" & DateTime.Now.Ticks & ".jpg"
                .SETTING_SAMPLE_LEVELS = "2"
                .SETTING_MAX_TIME = "10"

                'OPTIONAL
                .SETTING_ANIMATION = ""
                .SETTING_CAMERA_NAME = Chr(34) & "two word" & Chr(34)
                .SETTING_MULTILIGHT = False
                .SETTING_MXI_PATH = "C:\Temp\ALENTEJO_RENDER_ENGINE_OUTPUT\" & "testOut_" & DateTime.Now.Ticks & ".mxi"
                .SETTING_SCATTERING_LENS = 0
                .SETTING_VIGNETTING = 0
                .SETTING_BITMAP_PATHS = ""
                .SETTING_IS_SERVER = ""
                .SETTING_IS_MANAGER = False
                .SETTING_RENDER_ENGINE_IS_PREVIEW = False

                .SETTING_CHANNELS_RENDER = True
                .SETTING_CHANNELS_ALPHA = False
                .SETTING_CHANNELS_OPAQUE_ALPHA = False
                .SETTING_CHANNELS_SHADOW = False
                .SETTING_CHANNELS_MATERIAL = False
                .SETTING_CHANNELS_OBJECTID = False
                .SETTING_CHANNELS_ZBUFFER = False
                .SETTING_CHANNELS_ZBUFFER_MAX = 0
                .SETTING_CHANNELS_ZBUFFER_MIN = 0

            End With

            MXCL.Run()

        Catch ex As Exception
            MsgBox("Problem with RunTestRender(). Error: " & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub HandleMXCLMessage(ByVal msg As String) Handles MXCL.evGeneralLog
        AddConsoleLine(msg)
    End Sub

    Private Sub Handle_MXCLProgressUpdate(ByRef LogLine As cMXCLLogLine) Handles MXCL.evProgressUpdate
        AddConsoleLine("MXCL RENDER PROGRESS | " & LogLine.ToString)
    End Sub

    Private Sub HandleMXCLExit() Handles MXCL.evProcessExit
        AddConsoleLine("MXCL exited.")
    End Sub

#Region "CONSOLE"

    Private Sub AddConsoleLine(ByVal msg As String)
        If Me Is Nothing OrElse Me.IsDisposed Then Exit Sub
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _ConsoleRtbAppend_Delegate Is Nothing Then _ConsoleRtbAppend_Delegate = New ConsoleRtbAppend_Delegate(AddressOf ConsoleRtbAppend)
            Invoke(_ConsoleRtbAppend_Delegate, New Object() {msg})
        Else
            ConsoleRtbAppend(msg)
        End If
    End Sub

    Private Sub ConsoleRtbAppend(ByVal Msg As String)
        Me.rtbQueuePollingConsole.AppendText(Msg & vbNewLine)
        Me.rtbQueuePollingConsole.ScrollToCaret()
    End Sub
    Private Delegate Sub ConsoleRtbAppend_Delegate(ByVal Msg As String)
    Private _ConsoleRtbAppend_Delegate As ConsoleRtbAppend_Delegate

#End Region 'CONSOLE

    Private Sub btnGetCameraNames_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetCameraNames.Click
        Dim cameraNames() As String = GetCameraNamesForMXS(Me.txtMXSPath.Text)
        AddConsoleLine("Camera names:")
        For Each cn As String In cameraNames
            AddConsoleLine(cn)
        Next
        AddConsoleLine("---")
    End Sub

    Private Sub btnBrowse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBrowse.Click
        MsgBox("inop")
    End Sub

    Private Sub MaxwellTestHarness_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'Me.txtMXSPath.Text = "N:\PRODUCT DEVELOPMENT\Alentejo\Maxwell Projects\Vase diamond\Vase diamond.mxs"
        'Me.txtMXSPath.Text = "N:\PRODUCT DEVELOPMENT\Alentejo\Maxwell Projects\Simple Rect\simplerect.mxs"
        txtMXSPath.Text = "N:\PRODUCT DEVELOPMENT\Alentejo\Maxwell Projects\benchwell_scene_07.03.08_win\sculpture.mxs"
    End Sub

    Private Sub DeleteDirectoryContents(ByVal Dir As String)
        Try
            If Not System.IO.Directory.Exists(Dir) Then Exit Sub
            Dim di As New System.IO.DirectoryInfo(Dir)
            For Each fso As System.IO.FileSystemInfo In di.GetFileSystemInfos
                If fso.Extension = "" Then
                    DeleteDirectoryContents(fso.FullName)
                End If
                fso.Delete()
            Next
        Catch ex As Exception
            'SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            AddConsoleLine("Problem in DeleteDirectoryContents(). Error: " & ex.Message)

            'we take this very seriously.
            'it is probably due to a previous render not completing successfully so there's still a file 
            'handle open to one of the output files. Restarting the app should kill this handle.
            'because this cleanup process takes place before any job attributes are modified the new 
            'rm will pickup and run with the job successfully after the restart, theoretically.
            Application.Exit()
        End Try
    End Sub

    Private Sub btnNeperianLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnNeperianLog.Click
        Try
            Dim nodes As Byte = 1
            Dim targ As Double = 30
            Dim result As Double = ComputeNodeLevelTarget(nodes, targ)
            AddConsoleLine("Required sample level per machine = " & result)
        Catch ex As Exception
            MsgBox("problem with neperian")
        End Try
    End Sub

    Private Sub btnSL_2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSL_2.Click
        Try
            Dim sls As New List(Of Double)
            sls.Add(6)
            sls.Add(6)
            AddConsoleLine(ComputeCombinedSampleLevel(sls.ToArray))
        Catch ex As Exception
            MsgBox("problem with neperian")
        End Try
    End Sub

    Private Sub btnMXIMerge_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMXIMerge.Click
        Try
            Dim mxi_paths As New List(Of String)
            mxi_paths.Add("C:\Temp\MXIs\sr1.mxi")
            mxi_paths.Add("C:\Temp\MXIs\sr2.mxi")
            Dim mi As cMXIInfo = MergeMXIs(mxi_paths.ToArray, "C:\Temp\MXIs\sr.mxi", "C:\Temp\MXIs\sr.jpg")
            Debug.WriteLine(mi.Resolution.Width & "x" & mi.Resolution.Height)
            Debug.WriteLine(mi.SampleLevel)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub btnMergeThese_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnMergeThese.Click
        Dim mxi_paths As New List(Of String)
        mxi_paths.Add(txtMXIPath_1.Text)
        mxi_paths.Add(txtMXIPath_2.Text)
        Dim mi As cMXIInfo = MergeMXIs(mxi_paths.ToArray, "C:\merged.mxi", "C:\merged.jpg")
        Debug.WriteLine(mi.Resolution.Width & "x" & mi.Resolution.Height)
        Debug.WriteLine(mi.SampleLevel)
    End Sub

End Class
