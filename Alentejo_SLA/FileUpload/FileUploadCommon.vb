Imports System
Imports System.IO
Imports System.Net
Imports System.Collections.Generic
Imports System.Windows.Controls
Imports System.Windows.Browser

Imports Xceed.Http
Imports Xceed.Http.Uploader

Namespace SilverlightUploader

    Public NotInheritable Class Common

        Private Sub New()
        End Sub

#Region "Property : Url"
        'Gets or sets the page to access on the server
        Public Shared Property Url() As Uri
            Get
                Return m_url
            End Get
            Set(ByVal value As Uri)
                m_url = value
            End Set
        End Property
#End Region 'Property : Url

#Region "Property : UseOptions"
        'Gets or sets whether HttpOptions should be used when uploading files
        Public Shared Property UseOptions() As Boolean
            Get
                Return m_useOptions
            End Get
            Set(ByVal value As Boolean)
                m_useOptions = value
            End Set
        End Property
#End Region 'Property : UseOptions

#Region "Property : UseEvents"
        'Gets or sets whether HttpEvents should be used when uploading files
        Public Shared Property UseEvents() As Boolean
            Get
                Return m_useEvents
            End Get
            Set(ByVal value As Boolean)
                m_useEvents = value
            End Set
        End Property
#End Region 'Property : UseEvents

#Region "Property : Options"
        'Gets or sets the HttpOptions object
        Public Shared Property Options() As HttpOptions
            Get
                Return m_options
            End Get
            Set(ByVal value As HttpOptions)
                m_options = value
            End Set
        End Property
#End Region 'Property : Options

#Region "Property : Events"
        'Gets or sets the HttpEvents object
        Public Shared Property Events() As HttpEvents
            Get
                Return m_events
            End Get
            Set(ByVal value As HttpEvents)
                m_events = value
            End Set
        End Property
#End Region 'Property : Events

#Region "Property : Multiple"
        'Gets or sets whether we are uploading a single file or multiple files
        Public Shared Property Multiple() As Boolean
            Get
                Return m_multiple
            End Get
            Set(ByVal value As Boolean)
                m_multiple = value
            End Set
        End Property
#End Region 'Property : Multiple

#Region "Property : File Filter"
        'Gets or sets whether we are uploading a single file or multiple files
        Public Shared Property FileFilter() As String
            Get
                Return m_FileFilter
            End Get
            Set(ByVal value As String)
                m_FileFilter = value
            End Set
        End Property
#End Region 'Property : Multiple

#Region "Property : Callback"
        'Gets or sets the callback method to be called by the asynchronous upload methods
        Public Shared Property Callback() As AsyncCallback
            Get
                Return m_callback
            End Get
            Set(ByVal value As AsyncCallback)
                m_callback = value
            End Set
        End Property
#End Region 'Property : Callback

#Region "Property : Aborted"
        'Gets or sets whether the upload is Aborted
        Public Shared Property Aborted() As Boolean
            Get
                Return m_aborted
            End Get
            Set(ByVal value As Boolean)
                m_aborted = value
            End Set
        End Property
#End Region 'Property : Aborted

#Region "Property : UploadType"
        'Gets or sets the type of upload to do: static, instance, or deferred
        Public Shared Property UploadType() As UploadType
            Get
                Return m_uploadType
            End Get
            Set(ByVal value As UploadType)
                m_uploadType = value
            End Set
        End Property
#End Region 'Property : UploadType

#Region "Property : DataEncodingMode"
        'Gets or sets the DataEncodingMode
        Public Shared Property DataEncodingMode() As DataEncodingMode
            Get
                Return m_dataEncodingMode
            End Get
            Set(ByVal value As DataEncodingMode)
                m_dataEncodingMode = value
            End Set
        End Property
#End Region 'Property : DataEncodingMode

#Region "Property : CompressionMethod"
        'Gets or sets the CompressionMethod
        Public Shared Property CompressionMethod() As CompressionMethod
            Get
                Return m_compressionMethod
            End Get
            Set(ByVal value As CompressionMethod)
                m_compressionMethod = value
            End Set
        End Property
#End Region 'Property : CompressionMethod

#Region "Property : CompressionLevel"
        'Gets or sets the CompressionLevel
        Public Shared Property CompressionLevel() As CompressionLevel
            Get
                Return m_compressionLevel
            End Get
            Set(ByVal value As CompressionLevel)
                m_compressionLevel = value
            End Set
        End Property
#End Region 'Property : CompressionLevel

        '#Region "Property : Page"
        '        'Gets or sets the page on which the UI is so that it can be updated from the other classes in the application
        '        Public Shared Property Page() As RootPage
        '            Get
        '                Return m_page
        '            End Get
        '            Set(ByVal value As RootPage)
        '                m_page = value
        '            End Set
        '        End Property
        '#End Region 'Property : Page

#Region "Method ObtainFiles()"
        'Obtains the file(s) to upload
        Public Shared Function ObtainFiles() As List(Of FileToUpload)
            'In Silverlight, this is the only means to get access to files on the client
            Dim fileDialog As New OpenFileDialog()
            'Set whether the user can select one or more files
            fileDialog.Multiselect = Common.Multiple
            fileDialog.Filter = Common.FileFilter

            'The list that will contain the files to upload
            Dim fileList As List(Of FileToUpload) = Nothing

            Dim result As Nullable(Of Boolean) = fileDialog.ShowDialog()
            If result.HasValue And result.Value Then
                Try
                    'FileDialogFileInfo is returned by the SelectedFiles collection
                    For Each file As FileInfo In fileDialog.Files
                        'The first time we loop, the list needs to be created
                        If fileList Is Nothing Then
                            fileList = New List(Of FileToUpload)()
                        End If
                        '              Add a new FileToUpload item, which is a class that provides information
                        '              needed by methods uploading files
                        fileList.Add(New FileToUpload(file.OpenRead(), If(Common.Multiple, "Multiple", "Single"), file.Name))
                    Next file
                Catch ex As Exception
                    'If an exception is thrown by the OpenFileDialog, set the list to null, and inform the user
                    fileList = Nothing
                    'Common.Page.eventLogList.Items.Add("An exception was thrown by the OpenFileDialog. Please try again.")
                    Throw ex
                End Try
            End If
            Return fileList
        End Function
#End Region 'Method ObtainFiles()

#Region "Method ReleaseFiles()"
        '    Need to release the resources associated with each file we have uploaded,
        '    that is, close the stream 
        Public Shared Sub ReleaseFiles(ByVal fileList As List(Of FileToUpload))
            If fileList IsNot Nothing Then
                For Each file As FileToUpload In fileList
                    file.Stream.Close()
                Next file
            End If
        End Sub
#End Region 'Method ReleaseFiles()

#Region "Method InspectResponse()"
        '    Provides a way to see what was returned by the server in the HttpWebResponse
        '    and inform the user of the result 
        Public Shared Sub InspectResponse(ByVal response As HttpWebResponse)
            'Everything worked fine
            If response.StatusCode = HttpStatusCode.OK Then
                If (Not Common.Multiple) Then
                    'Common.Page.eventLogList.Items.Add("Done! File uploaded to the server.")
                Else
                    'Common.Page.eventLogList.Items.Add("Done! Files uploaded to the server.")
                End If
                'Something went wrong
            Else
                If (Not Common.Multiple) Then
                    'Common.Page.eventLogList.Items.Add("A problem occurred on the server, so the file was not uploaded.")
                Else
                    'Common.Page.eventLogList.Items.Add("A problem occurred on the server, so some or all files were not uploaded.")
                End If

                'See if we have logged something on the server
                GetMoreInformationFromCookies()
            End If

#If DEBUG Then
            '      Inspect the StatusCode and the Response itself. This will work only when debugging in Visual Studio
            '        and will appear in the Output window 
            System.Diagnostics.Debug.WriteLine("Response status code: " & response.StatusCode.ToString())

            'Get the response stream
            Dim responseStream As Stream = response.GetResponseStream()
            'If we have a response to inspect
            If responseStream IsNot Nothing Then
                Dim buffer(32767) As Byte
                Dim bytesRead As Integer = 0
                Dim responseStreamContent As String = ""
                'Get what's in the response stream
                bytesRead = responseStream.Read(buffer, 0, buffer.Length)
                Do While bytesRead > 0
                    'Build the string from the stream
                    responseStreamContent &= System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead)
                    bytesRead = responseStream.Read(buffer, 0, buffer.Length)
                Loop
                'Display the result string
                System.Diagnostics.Debug.WriteLine(responseStreamContent)
                'If we have no response to inspect
            Else
                System.Diagnostics.Debug.WriteLine("The response stream is null, so it cannot be read")
            End If
#End If 'DEBUG
        End Sub
#End Region 'Method InspectResponse()

#Region "Method GetMoreInformationFormCookies()"
        'Get what we have logged via the cookies on the server side, and if the cookie is empty,
        'we take for granted an unhandled exception happened on the server
        Private Shared Sub GetMoreInformationFromCookies()
            Dim list() As String = HtmlPage.Document.Cookies.Split(";"c)
            'If the cookies were empty
            If list.Length = 1 AndAlso list(0) = String.Empty Then
                ' This means an unhandled exception occurred on the server, so inform the user of this
                'Common.Page.eventLogList.Items.Add("An unhandled exception occurred on the server.")
                'If we have cookies values
            Else
                'Forech key/value pairs
                For Each item As String In list
                    'If it is a key/value pair
                    If item <> String.Empty AndAlso item.Contains("=") Then
                        'Get the key and the value
                        Dim keyValueString() As String = item.Split("="c)
                        If keyValueString(0).Contains("Exception") Then
                            'Show the value to the user, which is the exception message we logged in the aspx pages
                            'Common.Page.eventLogList.Items.Add(keyValueString(1).Trim())
                        End If
                    End If
                Next item
            End If
        End Sub
#End Region 'Method GetMoreInformationFormCookies()

#Region "Private Fields"

        'The Page.xaml.cs UserControl
        Private Shared m_page As RootPage
        'The url to log onto
        Private Shared m_url As Uri
        'If the upload is aborted
        Private Shared m_aborted As Boolean

        'Options and Events fields
        Private Shared m_useOptions As Boolean
        Private Shared m_useEvents As Boolean
        Private Shared m_options As HttpOptions
        Private Shared m_events As HttpEvents

        'Single or Multiple upload
        Private Shared m_multiple As Boolean
        Private Shared m_FileFilter As String
        'Callback method to pass to the upload methods
        Private Shared m_callback As AsyncCallback
        'Static, Instance, Deferred upload
        Private Shared m_uploadType As UploadType

        'Option fields
        Private Shared m_dataEncodingMode As DataEncodingMode
        Private Shared m_compressionMethod As CompressionMethod
        Private Shared m_compressionLevel As CompressionLevel

#End Region 'Private Fields

    End Class

    'The type of upload to use, that is, static, instance, or deferred
    Public Enum UploadType

        [Static]

        Instance

        Deferred

    End Enum

End Namespace
