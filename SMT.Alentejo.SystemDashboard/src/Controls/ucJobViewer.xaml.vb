Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.InstanceManagement

Partial Public Class ucJobViewer

    Private Job As cSMT_ATJ_RenderJob_Maxwell
    Public Event evCloseJob()
    Private StopUpdating As Boolean = False

    Private Sub btnSelectJob_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSelectJob.Click
        RaiseEvent evCloseJob()
    End Sub

    Public Sub UpdateJobDisplay(Optional ByVal DisplayJob As cSMT_ATJ_RenderJob_Maxwell = Nothing)
        Try
            If DisplayJob Is Nothing Then
                Job = CurrentJob
            Else
                Job = DisplayJob
            End If
            lblJobName.Text = Job.Name
            RefreshProgress()
            LoadLatestImage()
            RefreshConsole()
            RefreshFileList()
            RefreshStats()
            RefreshInstances()
        Catch ex As Exception
            MsgBox("Problem with UpdateJobDisplay(). Error: " & ex.Message)
        End Try
    End Sub

#Region "IMAGE"

    Private LatestImage As cSMT_ATJ_File

    Private Sub LoadLatestImage()
        LatestImage = GetJobLatestImageFile(Job.Id, True, True, 900, If(LatestImage Is Nothing, Nothing, LatestImage.Id))
        Me.cvImageWaitAnimation.Children.Add(New ucWaitAnimation)

        Dim bi As New Imaging.BitmapImage
        bi.BeginInit()
        bi.UriSource = New Uri(LatestImage.Url, UriKind.Absolute)
        bi.EndInit()
        AddHandler bi.DownloadProgress, AddressOf ImageDisplayCallback
        Me.imgSample.Stretch = Stretch.Uniform
        Me.imgSample.Source = bi
        Me.imgSample.Cursor = Cursors.Hand
        Me.imgSample.Visibility = Windows.Visibility.Visible
    End Sub

    Private Sub ImageDisplayCallback(ByVal sender As Object, ByVal e As System.Windows.Media.Imaging.DownloadProgressEventArgs)
        If e.Progress = 100 Then
            cvImageWaitAnimation.Children.Clear()
            lblImageFileName.Text = LatestImage.FileName
        Else
            'Debug.WriteLine(e.Progress)
        End If
    End Sub

    Private Sub imgSample_ImageFailed(ByVal sender As Object, ByVal e As System.Windows.ExceptionRoutedEventArgs) Handles imgSample.ImageFailed
        cvImageWaitAnimation.Children.Clear()
        'MessageBox.Show("This image could not be displayed." & vbNewLine & vbNewLine & "Likely cause: The render engine did not finish outputting this image file before moving on to the next level.")
        Debug.WriteLine("This image could not be displayed." & vbNewLine & vbNewLine & "Likely cause: The render engine did not finish outputting this image file before moving on to the next level.")
    End Sub

#End Region 'IMAGE

#Region "PROGRESS"

    Private Sub RefreshProgress()
        Select Case Job.Status
            Case Is < 0
                StopUpdating = True
                Me.lblProgress.Text = "FAILURE"
                'FAILURE
                Select Case Job.Status
                    Case eSMT_ATJ_RenderJob_Status.Failure_FileTransferToS3, eSMT_ATJ_RenderJob_Status.Failure_Generic, eSMT_ATJ_RenderJob_Status.Failure_RenderNodeCoordination, eSMT_ATJ_RenderJob_Status.Failure_FileTransferFromS3
                        SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Failed)
                        SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Not_Started)
                        SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                        SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

                    Case eSMT_ATJ_RenderJob_Status.Failure_RunMXCL
                        SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Failed)
                        SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                        SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

                    Case eSMT_ATJ_RenderJob_Status.Failure_RenderJobFinalization
                        SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Completed)
                        SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Failed)
                        SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

                End Select

            Case 100 To 199 'PRE PROCESSING
                Me.lblProgress.Text = "PRE-PROCESSING"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Running)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Not_Started)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

            Case 200 To 299 'RENDERING
                Me.lblProgress.Text = "RENDERING"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Running)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Not_Started)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

            Case 300 To 399 'POST PROCESSING
                Me.lblProgress.Text = "POST-PROCESSING"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Running)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Not_Started)

            Case 400 'COMPLETED
                StopUpdating = True
                Me.lblProgress.Text = "COMPLETED"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Completed)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Completed)

            Case 500 'ARCHIVED
                StopUpdating = True
                Me.lblProgress.Text = "ARCHIVED"
                SetProgressElementStatus(elPROG_JobSubmitted, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_PreProcessing, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_Rendering, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_Finalization, eProgressElementStatus.Archived)
                SetProgressElementStatus(elPROG_Complete, eProgressElementStatus.Archived)

            Case eSMT_ATJ_RenderJob_Status.NOT_INDICATED

        End Select

        'Select Case Status
        '    Case Is < 0, 400, 500
        '        hlTerminateResubmitRender.Content = "Resubmit"
        '    Case Else
        '        hlTerminateResubmitRender.Content = "Terminate"
        'End Select
    End Sub

    Private Sub SetProgressElementStatus(ByRef el As FrameworkElement, ByRef Status As eProgressElementStatus)
        Dim ell As Ellipse = CType(el, Ellipse)
        Dim rt As New RotateTransform

        'COLOR
        Select Case Status
            Case eProgressElementStatus.Not_Started
                ell.Fill = New SolidColorBrush(Colors.Yellow)
                rt.Angle = 90
                ell.RenderTransform = rt
            Case eProgressElementStatus.Running
                ell.Fill = New SolidColorBrush(Colors.Blue)
            Case eProgressElementStatus.Completed
                ell.Fill = New SolidColorBrush(Colors.Green)
                rt.Angle = 0
                ell.RenderTransform = rt
            Case eProgressElementStatus.Failed
                ell.Fill = New SolidColorBrush(Colors.Red)
                rt.Angle = -45
                ell.RenderTransform = rt
            Case eProgressElementStatus.Archived
                ell.Fill = New SolidColorBrush(Colors.Gray)
                rt.Angle = 0
                ell.RenderTransform = rt
        End Select

        'ANIMATION
        Select Case Status
            Case eProgressElementStatus.Running
                'make sure it is being animated
                If elCurrent IsNot ell Then
                    elCurrent = ell
                    elCurrentAngle = 90
                End If
            Case Else
                'make sure it is not being animated
                If elCurrent Is ell Then
                    elCurrent = Nothing
                End If
        End Select
    End Sub

    Private Enum eProgressElementStatus
        Not_Started
        Running
        Completed
        Failed
        Archived
    End Enum

#Region "PROGRESS:ANIMATION"

    Private elCurrent As Ellipse
    Private elCurrentAngle As Double

    Private Sub AnimateProgress()
        If elCurrent Is Nothing Then Exit Sub
        Dim rt As New RotateTransform
        elCurrentAngle += 1
        rt.Angle = elCurrentAngle
        elCurrent.RenderTransform = rt
    End Sub

#End Region 'PROGRESS:ANIMATION

#End Region 'PROGRESS

#Region "CONSOLE"

    Private ProgressItems As List(Of cSMT_ATJ_JobProgressMessage)
    Private ReadOnly Property LatestProgressItemTicks() As String
        Get
            If ProgressItems Is Nothing OrElse ProgressItems.Count = 0 Then
                Return ""
            Else
                Return CStr(CLng(ProgressItems.Item(ProgressItems.Count - 1).Timestamp) + 1)
            End If
        End Get
    End Property
    Private ConsoleItemCount As UShort = 0

    Private Sub RefreshConsole()
        Me.cvConsoleWaitAnimation.Children.Add(New ucWaitAnimation)
        Me.cvConsoleWaitAnimation.Children.Clear()
        Try
            If ProgressItems Is Nothing Then ProgressItems = New List(Of cSMT_ATJ_JobProgressMessage)
            Dim tPI As List(Of cSMT_ATJ_JobProgressMessage) = GetJobProgress(Job.Id, LatestProgressItemTicks, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.Status_major, AWS.SDB.eComparisonOperator.GreaterThanOrEqualTo)
            ProgressItems.AddRange(tPI)

            If ProgressItems Is Nothing Then Exit Sub
            If ConsoleItemCount = ProgressItems.Count Then
                Exit Sub
            Else
                ConsoleItemCount = ProgressItems.Count
            End If

            Me.spConsole.Children.Clear()
            For Each pi As cSMT_ATJ_JobProgressMessage In ProgressItems
                Me.spConsole.Children.Add(New ucJobViewerConsoleItem(pi))

                ''TODO: stats
                'If Not String.IsNullOrEmpty(pi.Benchmark) Then Me.lblSTAT_Benchmark.Text = pi.Benchmark
                'If Not String.IsNullOrEmpty(pi.SampleLevel) Then Me.lblSTAT_SampleLevel.Text = pi.SampleLevel & " / " & MaxwellJob.SampleCount

            Next
            Me.svConsole.UpdateLayout()
            Me.svConsole.ScrollToVerticalOffset(Double.MaxValue)

        Catch ex As Exception
            MessageBox.Show("Problem with RefreshConsole(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'CONSOLE

#Region "FILES"

    Private Files As List(Of cSMT_ATJ_File)

    Private Sub RefreshFileList()
        'cvFilesWaitAnimation.Children.Add(New ucWaitAnimation)

        Me.lbFiles.Items.Clear()

        Files = GetAtjFiles(Job.Id)
        If Files Is Nothing OrElse Files.Count < 1 Then Exit Sub

        Dim SelectedFileIndex As Integer = lbFiles.SelectedIndex
        Dim tTb As TextBlock
        For Each f As cSMT_ATJ_File In Files
            tTb = New TextBlock
            tTb.Text = f.FileName
            tTb.FontWeight = FontWeights.Bold
            tTb.FontSize = CDbl(12)
            tTb.Margin = New Thickness(0.0, 0.0, 0.0, 0.0)
            If String.IsNullOrEmpty(f.IsAvailable) Then
                tTb.Foreground = New SolidColorBrush(Colors.Red)
            Else
                tTb.Foreground = New SolidColorBrush(Color.FromArgb(&HFF, &H4B, &H4D, &HBA))
            End If
            lbFiles.Items.Add(tTb)
        Next
        lbFiles.SelectedIndex = SelectedFileIndex
        'cvFilesWaitAnimation.Children.Clear()
    End Sub

    Private Sub btnDownloadFile_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnDownloadFile.Click
        'Dim f As cSMT_ATJ_File = Files(lbFILE_FileList.SelectedIndex)
        'DownloadFile(f.Id)
    End Sub

#End Region 'FILES

#Region "STATS"

    Private Sub RefreshStats()
        '' STATIC
        'Me.lblJobName.Text = "JobName: " & Job.Name
        'Me.lblSTAT_Cores.Text = Job.CoresRequested
        'Me.lblSTAT_CameraName.Text = Job.ActiveCamera
        'Dim StartTime As New DateTime(Job.Submitted_Ticks, DateTimeKind.Utc)
        'Me.lblSTAT_Time_Start.Text = StartTime.ToString("r")
        'Me.lblSTAT_JobFile.Text = Job.JobFileNames(0)
        'Me.lblSTAT_Resolution.Text = Job.Width & " x " & Job.Height

        '' DYNAMIC
        'Dim ts As TimeSpan
        'If Job.Status = eSMT_ATJ_RenderJob_Status.COMPLETED Then
        '    If Not String.IsNullOrEmpty(Job.Completed_Ticks) Then
        '        Dim EndTime As New DateTime(Job.Completed_Ticks, DateTimeKind.Utc)
        '        Me.lblSTAT_Time_End.Text = EndTime.ToString("r")
        '        ts = New TimeSpan(EndTime.Ticks - StartTime.Ticks)
        '        Me.lblSTAT_Time_Elapsed.Text = Math.Round(ts.TotalMinutes, 0) & " min"
        '    End If
        'Else
        '    ts = New TimeSpan(DateTime.UtcNow.Ticks - StartTime.Ticks)
        '    Me.lblSTAT_Time_Elapsed.Text = If(ts.TotalMinutes < 0, "0", Math.Round(ts.TotalMinutes, 0).ToString) & " min / " & MaxwellJob.MaxDuration & " min"
        'End If
        'Me.lblSTAT_Cost_Current.Text = "€" & Charges_FriendlyString()

        ''are updated by the console update code
        ''Me.lblSTAT_Benchmark.Tex t = ""
        ''Me.lblSTAT_SampleLevel.Text = ""
    End Sub

    Private ReadOnly Property Charges_FriendlyString() As String
        Get
            If String.IsNullOrEmpty(Job.Charges) Then Return "0.00"
            If InStr(Job.Charges, ".") = 0 Then
                Return Job.Charges & ".00"
            Else
                Dim s() As String = Split(Job.Charges, ".")
                If s(1).Length < 2 Then
                    Return Job.Charges & "0"
                Else
                    Return Job.Charges
                End If
            End If
        End Get
    End Property

#End Region 'STATS

#Region "INSTANCES"

    Private Sub RefreshInstances()
        Try
            Dim MasterInstanceId As String = GetJobAttribute(Job.Id, "MasterInstanceId")
            Dim MI As cSMT_ATJ_Instance = GetInstance(MasterInstanceId)

            Dim RenderInstanceIds As String = GetJobAttribute(Job.Id, "RenderInstanceIds")
            Dim aRenderInstanceIds() As String
            Dim RIs As List(Of cSMT_ATJ_Instance)
            If Not String.IsNullOrEmpty(RenderInstanceIds) Then
                aRenderInstanceIds = Split(RenderInstanceIds, ",")
                RIs = New List(Of cSMT_ATJ_Instance)
                For Each s As String In aRenderInstanceIds
                    RIs.Add(GetInstance(s))
                Next
            End If

            Me.lbInstances.Items.Clear()
            Me.lbInstances.Items.Add(MI)
            If RIs IsNot Nothing Then
                For Each i As cSMT_ATJ_Instance In RIs
                    lbInstances.Items.Add(i)
                Next
            End If

        Catch ex As Exception
            Throw New Exception("Problem with RefreshInstances(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub lbInstances_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Input.MouseButtonEventArgs) Handles lbInstances.MouseDoubleClick
        If MsgBox("Connect to instance?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
        RunRDP(CType(lbInstances.SelectedItem, cSMT_ATJ_Instance).PublicHostname)
    End Sub

#End Region 'INSTANCES

End Class
