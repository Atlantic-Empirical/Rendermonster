Imports System.Windows.Browser
Imports System.IO

Imports Xceed.Http.Uploader
Imports Xceed.Http

Imports SMT.Alentejo_SLA.SilverlightUploader

Partial Public Class ucUploadFiles
    Inherits UserControl

    Public Sub New()
        Xceed.Http.Licenser.LicenseKey = "UPS10-D8YXD-FU4KY-T8ZA"
        InitializeComponent()
    End Sub

    Private Sub btnSelectFile_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSelectFile.Click
        Me.ParseOptions()
        Me.m_result = StaticUploads.BeginStaticUploads()
    End Sub

    Private Sub btnAbortUploads_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnAbortUploads.Click
        Common.Aborted = True
        HttpUploader.AbortUpload(m_result)
    End Sub

    Private Shared Sub StaticUploadCallback(ByVal result As IAsyncResult)
        Try
            Dim response As HttpWebResponse

            'End the upload and get the response
            response = SilverlightUploader.StaticUploads.EndStaticUploads(result)

            'If the upload was not aborted
            If response IsNot Nothing OrElse (Not SilverlightUploader.Common.Aborted) Then
                'Look into the response
                SilverlightUploader.Common.InspectResponse(response)

                'Release the resources
                response.Close()
            End If

            'If the upload was aborted
            If SilverlightUploader.Common.Aborted Then
                'Update the UI appropriately
                'Common.Page.progressBorder.Visibility = Visibility.Collapsed
                'Common.Page.eventLogList.Items.Add("Upload aborted.")
            End If
        Catch e As Exception
            'Log the exception
            'Common.Page.eventLogList.Items.Add(e.ToString())
        End Try
    End Sub

    Private Sub ParseOptions()
        Common.Aborted = False
        Common.Callback = New AsyncCallback(AddressOf StaticUploadCallback)
        Me.SetOptions()
        Me.SetEvents()
    End Sub

    Private Sub SetOptions()
        Common.DataEncodingMode = DataEncodingMode.Auto

        Common.CompressionMethod = CompressionMethod.GZip
        'Common.CompressionMethod = CompressionMethod.Deflated
        'Common.CompressionMethod = CompressionMethod.None

        'Common.CompressionLevel = CompressionLevel.Lowest
        Common.CompressionLevel = CompressionLevel.Normal
        'Common.CompressionLevel = CompressionLevel.Highest
        'Common.CompressionLevel = CompressionLevel.None

        Common.UseOptions = True
        'If the options have not been initialized before
        If Common.Options Is Nothing Then
            Common.Options = New HttpOptions()
        End If
        'Actually set the HttpOptions instance
        Common.Options.DataEncodingMode = Common.DataEncodingMode
        Common.Options.CompressionMethod = Common.CompressionMethod
        Common.Options.CompressionLevel = Common.CompressionLevel

        Common.Url = New Uri(HtmlPage.Document.DocumentUri, "JobSubmission/FileCatcher.ashx")
    End Sub

    Private Sub SetEvents()
        'If events have not been initialized before
        If Common.Events Is Nothing Then
            Common.Events = New HttpEvents()
            Common.Events.UIThreadDispatcher = Me.Dispatcher
        End If

        'Reset all the events so they are not triggered more than once
        'RemoveHandler Common.Events.BuildingHttpWebRequest, AddressOf OnBuildingHttpWebRequest
        'RemoveHandler Common.Events.NewHttpWebRequestStream, AddressOf OnNewHttpWebRequestStream
        RemoveHandler Common.Events.ByteProgression, AddressOf OnByteProgression
        'RemoveHandler Common.Events.GettingHttpWebResponse, AddressOf OnGettingHttpWebResponse
        'RemoveHandler Common.Events.HttpWebResponseReceived, AddressOf OnHttpWebResponseReceived

        'Set this to false so that only if one or more events are subscribed to are we to use the events instance.
        Common.UseEvents = False

        ''Set the BuildingHttpWebRequest event
        'If CBool(Me.webRequestCheck.IsChecked) Then
        '    AddHandler Common.Events.BuildingHttpWebRequest, AddressOf OnBuildingHttpWebRequest
        '    Common.UseEvents = True
        'End If

        ''Set the NewHttpWebRequestStream event
        'If CBool(Me.requestStreamCheck.IsChecked) Then
        '    AddHandler Common.Events.NewHttpWebRequestStream, AddressOf OnNewHttpWebRequestStream
        '    Common.UseEvents = True
        'End If

        ''Set the ByteProgression event
        'If CBool(Me.bytesProgressionCheck.IsChecked) Then
        AddHandler Common.Events.ByteProgression, AddressOf OnByteProgression
        Common.UseEvents = True
        'End If

        ''Set the GettingHttpWebResponse event
        'If CBool(Me.webResponseCheck.IsChecked) Then
        '    AddHandler Common.Events.GettingHttpWebResponse, AddressOf OnGettingHttpWebResponse
        '    Common.UseEvents = True
        'End If

        ''Set the HtttpWebResponseReceived event
        'If CBool(Me.responseReceivedCheck.IsChecked) Then
        '    AddHandler Common.Events.HttpWebResponseReceived, AddressOf OnHttpWebResponseReceived
        '    Common.UseEvents = True
        'End If
    End Sub

#Region "Upload Event Handlers"

    'Private Sub OnBuildingHttpWebRequest(ByVal sender As Object, ByVal e As BuildingHttpWebRequestEventArgs)
    '    'Update the progress display
    '    Me.eventLogList.Items.Add("Building the request.")
    'End Sub

    'Private Sub OnNewHttpWebRequestStream(ByVal sender As Object, ByVal e As HttpWebRequestStreamEventArgs)
    '    'Update the progression display
    '    Me.eventLogList.Items.Add("Getting the request stream.")
    'End Sub

    Private Sub OnByteProgression(ByVal sender As Object, ByVal e As ByteProgressionEventArgs)
        'Update the ProgressBar
        'Me.progressBorder.Visibility = Visibility.Visible
        'Me.percentTextBlock.Text = e.AllFilesBytes.Percent.ToString()
        'Me.progressBorder.Width = e.AllFilesBytes.Percent * 200 \ 100
        'System.Diagnostics.Debug.WriteLine("progress = " & e.AllFilesBytes.Percent)
        Me.txtProgress.Text = e.AllFilesBytes.Percent & "%"
    End Sub

    'Private Sub OnGettingHttpWebResponse(ByVal sender As Object, ByVal e As HttpWebResponseEventArgs)
    '    'Use the ProgessBar as an animation, and start it
    '    Me.progressBorder.Visibility = Visibility.Collapsed
    '    If Me.m_progressStory Is Nothing Then
    '        Me.m_progressStory = TryCast(Me.Resources("loadingStaticProgressStory"), Storyboard)
    '    End If
    '    If Me.m_progressStory IsNot Nothing Then
    '        Me.m_progressStory.Begin()
    '    End If
    '    'Update the progress display
    '    Me.eventLogList.Items.Add("Getting the response.")
    'End Sub

    'Private Sub OnHttpWebResponseReceived(ByVal sender As Object, ByVal e As HttpWebResponseEventArgs)
    '    'Stop the animation
    '    If Me.m_progressStory IsNot Nothing Then
    '        Me.m_progressStory.Stop()
    '    End If
    '    'Update the progress display
    '    Me.eventLogList.Items.Add("The response has been received.")
    'End Sub

#End Region 'Upload Event Handlers

#Region "Private Fields"

    'The IAsyncResult returned by the component
    Private m_result As IAsyncResult

#End Region 'Private Fields

End Class
