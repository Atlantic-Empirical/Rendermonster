Imports System.IO

Partial Public Class ucUSER_SubmitRenderJob
    Inherits UserControl

#Region "FIELDS & PROPERTIES"

    Private Shared MXS As FileInfo
    Private Shared PathFiles As List(Of FileInfo)
    Private WithEvents MXSUploader As cUploader
    'Private WithEvents JSC As AlentejoJobService.AlentejoJobServiceSoapClient
    'Private Shared Job As SMT.Alentejo_SLA.JobSubmission.cSMT_ATJ_RenderJob_Maxwell

    Private ReadOnly Property MXSFileName() As String
        Get
            If User Is Nothing Then Return ""
            If txtJobName.Text = "" Then Return ""
            Return User.Username & "_" & txtJobName.Text & "_" & DateTime.UtcNow.Ticks & ".mxs"
        End Get
    End Property
    Private ReadOnly Property PathFileCount() As Byte
        Get
            If PathFiles Is Nothing Then Return 0
            Return PathFiles.Count
        End Get
    End Property

    Private AssignedJobId As String
    'Private CameraNames() As String

    'Private AlentejoOptions As ucRenderJobOptions_Alentejo
    'Private GeneralOptions As ucRenderJobOptions_General
    'Private MaxwellOptions As ucRenderJobOptions_Maxwell

#End Region 'FIELDS & PROPERTIES

#Region "CONSTRUCTOR"

    Public Sub New()
        InitializeComponent()
    End Sub

#End Region 'CONSTRUCTOR

#Region "PAGE"

    Private Sub ucMX_StartJob_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        Me.lblProgress.Visibility = Windows.Visibility.Collapsed
        Me.cvJobProfile.Visibility = Windows.Visibility.Collapsed
        If JobForResubmit IsNot Nothing Then ResubmitJobSetup()
        txtJobName.Focus()
        SetFonts(Me.LayoutRoot)
    End Sub

#End Region 'PAGE

#Region "JOB FILES"

    Private Sub btnBrowse_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnBrowse.Click
        Try
            Dim ofd As New OpenFileDialog()
            ofd.Filter = "Maxwell Scene (*.mxs)|*.mxs"
            ofd.Multiselect = False
            Dim b As Boolean = ofd.ShowDialog
            If b Then
                MXS = ofd.File
                Me.txtMXSPath.Text = ofd.File.Name
            End If
        Catch ex As Exception
            DebugWrite("Problem with browse. Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnPathFiles_Add_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPathFiles_Add.Click
        Try
            Dim ofd As New OpenFileDialog()
            ofd.Filter = "Maxwell Path Files (*.*)|*.*"
            ofd.Multiselect = True
            Dim b As Boolean = ofd.ShowDialog
            If b Then
                For Each fi In ofd.Files
                    If PathFiles Is Nothing Then PathFiles = New List(Of FileInfo)
                    PathFiles.Add(fi)
                Next
                UpdatePathListBox()
            End If
        Catch ex As Exception
            DebugWrite("Problem with PathFiles_Add(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnPathFiles_Clear_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPathFiles_Clear.Click
        PathFiles = Nothing
        Me.lbPathFiles.Items.Clear()
        UpdatePathListBox()
    End Sub

    Private Sub btnPathFiles_RemoveSelected_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnPathFiles_RemoveSelected.Click
        If PathFiles Is Nothing Then Exit Sub
        If lbPathFiles.SelectedItem Is Nothing Then Exit Sub
        For b As Byte = 0 To PathFiles.Count - 1
            If PathFiles(b).Name = lbPathFiles.SelectedItem.ToString Then
                PathFiles.RemoveAt(b)
                UpdatePathListBox()
                Exit Sub
            End If
        Next
    End Sub

    Private Sub UpdatePathListBox()
        Try
            If PathFiles Is Nothing Then Exit Sub
            Me.lbPathFiles.Items.Clear()
            For Each FI As FileInfo In PathFiles
                Me.lbPathFiles.Items.Add(FI.Name)
            Next
        Catch ex As Exception
            DebugWrite("Problem with RenderPathListBox(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnUploadJobFiles_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUploadJobFiles.Click
        If Me.txtJobName.Text = "" Then
            MessageBox.Show("Job name is required.")
            Exit Sub
        End If
        If Me.txtMXSPath.Text = "" Then
            MessageBox.Show("Please select an MXS file.")
            Exit Sub
        End If

        Me.btnUploadJobFiles.Content = "*** JOB FILES UPLOADING ***"
        Me.lblProgress.Visibility = Windows.Visibility.Visible
        MXSUploader = New cUploader(Me.Dispatcher)

        Dim lst As New List(Of cFileUploadInfo)
        lst.Add(New cFileUploadInfo(MXS.OpenRead, Me.txtMXSPath.Text))

        If Not PathFiles Is Nothing Then
            For Each pf As FileInfo In PathFiles
                lst.Add(New cFileUploadInfo(pf.OpenRead, pf.Name))
            Next
        End If

        Dim Headers As New Dictionary(Of String, String)
        Headers.Add("UserId", User.UserId)
        Headers.Add("JobName", txtJobName.Text)
        MXSUploader.UploadFiles(lst, Headers)
    End Sub

    Private Sub HandleMXSUploadProgress(ByVal Percent As Byte, ByVal BytesUploaded As Long, ByVal BytesTotal As Long) Handles MXSUploader.evProgress
        Me.rtProgress.Width = (Me.btnUploadJobFiles.Width / 100) * Percent
        Me.lblProgress.Text = Percent & "%"
    End Sub

    Private Sub HandleMSXUploadComplete(ByVal success As Boolean, ByVal ResponseCookies As Dictionary(Of String, String)) Handles MXSUploader.evUploadCompleted
        ' get JobId and camera names from response
        AssignedJobId = ResponseCookies("JobId")
        'maxwell
        MaxwellOptions.CameraNames = ResponseCookies("MaxwellCameras")
        ShowJobProfileControls()
    End Sub

#End Region 'JOB FILES

#Region "JOB RESUBMISSION"

    Private Sub ResubmitJobSetup()
        'PROJECT
        AssignedJobId = JobForResubmit.Id
        Me.txtJobName.Text = JobForResubmit.Name

        'ALENTEJO OPTIONS
        AlentejoOptions.CostCeiling = JobForResubmit.MaxCost
        AlentejoOptions.EmailSamples = JobForResubmit.EmailSamples
        AlentejoOptions.CoresRequested = JobForResubmit.CoresRequested

        'GENERAL OPTIONS
        GeneralOptions.MaxDuration = JobForResubmit_Maxwell.MaxDuration
        GeneralOptions.SampleLevels = JobForResubmit_Maxwell.SampleCount
        GeneralOptions.OutputImageFormat = JobForResubmit_Maxwell.OutputImageFormat
        GeneralOptions.Resolution = New Size(JobForResubmit_Maxwell.Width, JobForResubmit_Maxwell.Height)

        'MAXWELL OPTIONS
        MaxwellOptions.CameraNames = JobForResubmit_Maxwell.CameraNames
        MaxwellOptions.ActiveCamera = JobForResubmit_Maxwell.ActiveCamera
        MaxwellOptions.AnimationFrames = JobForResubmit_Maxwell.AnimationFrames
        MaxwellOptions.CreateMultilight = JobForResubmit_Maxwell.CreateMultilight
        MaxwellOptions.RenderWithTextures = JobForResubmit_Maxwell.RenderTextures
        MaxwellOptions.Render = JobForResubmit_Maxwell.Channels_Render
        MaxwellOptions.Shadow = JobForResubmit_Maxwell.Channels_Shadow
        MaxwellOptions.Alpha = JobForResubmit_Maxwell.Channels_Alpha
        MaxwellOptions.Alpha_Opaque = JobForResubmit_Maxwell.Channels_Alpha_Opaque
        MaxwellOptions.MaterialId = JobForResubmit_Maxwell.Channels_MaterialId
        MaxwellOptions.ObjectId = JobForResubmit_Maxwell.Channels_ObjectId
        MaxwellOptions.zBuffer = JobForResubmit_Maxwell.Channels_ZBuffer
        MaxwellOptions.zBuffer_min = JobForResubmit_Maxwell.Channels_ZBuffer_min
        MaxwellOptions.zBuffer_max = JobForResubmit_Maxwell.Channels_ZBuffer_max
        MaxwellOptions.Vignetting = JobForResubmit_Maxwell.Vignetting
        MaxwellOptions.ScatteringLens = JobForResubmit_Maxwell.ScatteringLens
        MaxwellOptions.UsePreviewEngine = JobForResubmit_Maxwell.UsePreviewEngine

        ShowJobProfileControls()
        JobForResubmit = Nothing
    End Sub

#End Region 'JOB RESUBMISSION

#Region "JOB PROFILE"

    Private Sub ShowJobProfileControls()
        ' hide the FileUpload canvas
        cvJobFileUpload.Visibility = Windows.Visibility.Collapsed
        'bring the JobProfile canvas into view
        Canvas.SetLeft(cvJobProfile, CDbl(40))
        Canvas.SetTop(cvJobProfile, CDbl(60))
        cvJobProfile.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub btnSubmitRenderJob_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSubmitRenderJob.Click
        Me.btnSubmitRenderJob.IsEnabled = False
        'btnSubmitRenderJob.Cursor = Cursors.Wait
        SubmitJobProfile()
    End Sub

    Private Sub SubmitJobProfile()
        Try
            Dim MXRJ As New SMT.Alentejo_SLA.AlentejoJobService.cSMT_ATJ_RenderJob_Maxwell
            With MXRJ

                'PROJECT
                .Id = AssignedJobId
                .Name = Replace(Me.txtJobName.Text, " ", "_")
                .UserId = User.UserId

                'ALENTEJO OPTIONS
                .MaxCost = AlentejoOptions.CostCeiling
                .EmailSamples = AlentejoOptions.EmailSamples
                .CoresRequested = AlentejoOptions.CoresRequested

                'GENERAL OPTIONS
                .MaxDuration = GeneralOptions.MaxDuration
                .SampleCount = GeneralOptions.SampleLevels
                .OutputImageFormat = GeneralOptions.OutputImageFormat
                .Width = GeneralOptions.Resolution.Width
                .Height = GeneralOptions.Resolution.Height

                'MAXWELL OPTIONS
                .ActiveCamera = MaxwellOptions.ActiveCamera
                .CameraNames = MaxwellOptions.CameraNames
                .AnimationFrames = MaxwellOptions.AnimationFrames
                .CreateMultilight = MaxwellOptions.CreateMultilight
                .RenderTextures = MaxwellOptions.RenderWithTextures
                .Channels_Render = MaxwellOptions.Render
                .Channels_Shadow = MaxwellOptions.Shadow
                .Channels_Alpha = MaxwellOptions.Alpha
                .Channels_Alpha_Opaque = MaxwellOptions.Alpha_Opaque
                .Channels_MaterialId = MaxwellOptions.MaterialId
                .Channels_ObjectId = MaxwellOptions.ObjectId
                .Channels_ZBuffer = MaxwellOptions.zBuffer
                .Channels_ZBuffer_min = MaxwellOptions.zBuffer_min
                .Channels_ZBuffer_max = MaxwellOptions.zBuffer_max
                .Vignetting = MaxwellOptions.Vignetting
                .ScatteringLens = MaxwellOptions.ScatteringLens
                .UsePreviewEngine = MaxwellOptions.UsePreviewEngine

                ' SET JOB FILES
                ReDim .JobFileNames(PathFileCount + 1 - 1) 'just to be clear that there is a spot for the project file and all path files
                .JobFileNames(0) = Path.GetFileName(txtMXSPath.Text)
                If Not PathFiles Is Nothing Then
                    For b As Byte = 0 To PathFileCount - 1
                        .JobFileNames(b + 1) = PathFiles(b).Name
                    Next
                End If

            End With

            'SetCursor(Me, Cursors.Wait)

            'JSC = New AlentejoJobService.AlentejoJobServiceSoapClient
            'JSC.MX_SubmitJobAsync("", MXRJ)

            AddHandler JobServiceClient.MX_SubmitJobCompleted, AddressOf Handle_JobSubmissionCompleted
            JobServiceClient.MX_SubmitJobAsync("", MXRJ)

        Catch ex As Exception
            MessageBox.Show("Problem with SubmitRenderJob(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Handle_JobSubmissionCompleted(ByVal sender As Object, ByVal e As AlentejoJobService.MX_SubmitJobCompletedEventArgs) 'Handles JobServiceClient.MX_SubmitJobCompleted 'JSC.MX_SubmitJobCompleted
        RemoveHandler JobServiceClient.MX_SubmitJobCompleted, AddressOf Handle_JobSubmissionCompleted
        'SetCursor(Me, Cursors.Arrow)
        If e.Result = "" Then
            MessageBox.Show("Job submission failed (0x0002).")
        Else
            MessageBox.Show("Your render job is submitted. Check your email for details." & vbNewLine & vbNewLine & "Thanks for using RenderMonster.")
            CurrentJobId = e.Result
            OpenDetail(Me, ePages.USER_JobViewer)
        End If
    End Sub

#End Region 'JOB PROFILE

End Class
