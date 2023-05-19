Imports System.Windows.Browser
Imports System.IO

Imports Xceed.Http.Uploader
Imports Xceed.Http

Imports SMT.Alentejo_SLA.SilverlightUploader

Public Class cUploader

#Region "CONSTRUCTOR"

    Public Sub New(ByVal nDispatcher As System.Windows.Threading.Dispatcher)
        Xceed.Http.Licenser.LicenseKey = "UPS10-D8YXD-FU4KY-T8ZA"
        m_Dispatcher = nDispatcher
    End Sub

#End Region 'CONSTRUCTOR

#Region "API"

    Public Sub UploadFile(ByVal Filter As String, ByVal Multiple As Boolean)
        Me.ParseOptions()
        Common.Multiple = Multiple
        Common.FileFilter = Filter
        m_result = StaticUploads.BeginStaticUploads()
    End Sub

    Public Sub UploadFile(ByVal Str As Stream, ByVal FileName As String)
        Me.ParseOptions()
        Dim lst As New List(Of FileToUpload)
        lst.Add(New FileToUpload(Str, "Single", FileName))
        m_result = StaticUploads.BeginSendFile(lst)
    End Sub

    Public Sub UploadFiles(ByVal Files As List(Of cFileUploadInfo), Optional ByVal Headers As Dictionary(Of String, String) = Nothing)
        Try
            Me.ParseOptions()
            Dim lst As New List(Of FileToUpload)
            For Each f As cFileUploadInfo In Files
                lst.Add(New FileToUpload(f.Str, "Multiple", f.FileName))
            Next

            If Headers IsNot Nothing Then
                For Each h As KeyValuePair(Of String, String) In Headers
                    If SilverlightUploader.Common.Options.HttpWebRequestHeaders.ContainsKey(h.Key) Then
                        SilverlightUploader.Common.Options.HttpWebRequestHeaders(h.Key) = h.Value
                    Else
                        SilverlightUploader.Common.Options.HttpWebRequestHeaders.Add(h.Key, h.Value)
                    End If
                Next
            End If

            m_result = StaticUploads.BeginSendFiles(lst)
        Catch ex As Exception
            Throw New Exception("Problem with UploadFiles(). Error: " & ex.Message)
        End Try
    End Sub

    Public Sub Abort()
        Common.Aborted = True
        HttpUploader.AbortUpload(m_result)
    End Sub

#End Region 'API

#Region "CALLBACK"

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

            Dim list() As String = HtmlPage.Document.Cookies.Split(";"c)
            Dim ResponseCookies As New Dictionary(Of String, String)
            Dim cs() As String
            For Each c As String In list
                cs = Split(c, "=")
                ResponseCookies.Add(cs(0).Trim, cs(1).Trim)
            Next

            RaiseEvent evUploadCompleted(response.StatusCode = HttpStatusCode.OK, ResponseCookies)
        Catch e As Exception
            'Log the exception
            'Common.Page.eventLogList.Items.Add(e.ToString())
        End Try
    End Sub

#End Region 'CALLBACK

#Region "SETUP"

    Private Sub ParseOptions()
        Common.Aborted = False
        Common.Callback = New AsyncCallback(AddressOf StaticUploadCallback)
        Common.FileFilter = ""
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

        Common.Url = New Uri(HtmlPage.Document.DocumentUri, "FileUpload/FileCatcher.ashx")
    End Sub

    Private Sub SetEvents()
        'If events have not been initialized before
        If Common.Events Is Nothing Then
            Common.Events = New HttpEvents()
            Common.Events.UIThreadDispatcher = m_Dispatcher
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

#End Region 'SETUP

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
        'Me.txtProgress.Text = e.AllFilesBytes.Percent & "%"
        RaiseEvent evProgress(e.AllFilesBytes.Percent, e.AllFilesBytes.Processed, e.AllFilesBytes.Total)
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

    Private m_Dispatcher As System.Windows.Threading.Dispatcher

#End Region 'Private Fields

#Region "PUBLIC EVENTS"

    Public Shared Event evProgress(ByVal OverallPercentage As Byte, ByVal BytesUploaded As Long, ByVal BytesTotal As Long)
    Public Shared Event evUploadCompleted(ByVal Success As Boolean, ByVal ResponseCookies As Dictionary(Of String, String))

#End Region 'PUBLIC EVENTS

End Class

Public Class cFileUploadInfo
    Public Str As Stream
    Public FileName As String
    Public Sub New(ByVal nStr As Stream, ByVal nFileName As String)
        Str = nStr
        FileName = nFileName
    End Sub
End Class
