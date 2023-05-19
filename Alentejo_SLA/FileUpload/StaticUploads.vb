Imports System
Imports System.IO
Imports System.Net
Imports System.Collections.Generic
Imports System.Windows.Controls

Imports Xceed.Http.Uploader

Namespace SilverlightUploader

    'This class demonstrates how to upload files using the static methods of HttpUploader
    Public Class StaticUploads

#Region "Method BeginStaticUploads()"
        ' Gets the list of files to upload and calls the right method depending on how many
        ' files there are to upload 
        Public Shared Function BeginStaticUploads() As IAsyncResult
            Dim result As IAsyncResult = Nothing

            'Get the file(s)
            m_fileList = Common.ObtainFiles()

            'If we have file(s) to upload
            If m_fileList IsNot Nothing Then
                'If only one file
                If (Not Common.Multiple) Then
                    result = StaticUploads.BeginSendFile(m_fileList)
                    'If more than one file
                Else
                    result = StaticUploads.BeginSendFiles(m_fileList)
                End If
            End If

            Return result
        End Function
#End Region 'Method BeginStaticUploads()

#Region "Method EndStaticUploads"
        'Needs to be called to finish the asynchronous upload 
        Public Shared Function EndStaticUploads(ByVal result As IAsyncResult) As HttpWebResponse
            Dim response As HttpWebResponse

            'Call the appropriate method depending on the number of files to upload
            If (Not Common.Multiple) Then
                response = HttpUploader.EndUploadFile(result, Nothing)
            Else
                response = HttpUploader.EndUploadFiles(result, Nothing)
            End If

            'Release the file resources
            Common.ReleaseFiles(m_fileList)

            Return response
        End Function
#End Region ' Method EndStaticUploads

#Region "Method BeginSendFile()"
        ' This is where the actual call to the component's methods are made when sending one file.
        ' Depending on whether options, events, and a callback are provided, we use different methods 
        Public Shared Function BeginSendFile(ByVal fileList As List(Of FileToUpload)) As IAsyncResult
            Dim result As IAsyncResult

            'No options, no events, with callback
            If (Not Common.UseOptions) AndAlso (Not Common.UseEvents) AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFile(Common.Url, fileList(0).Stream, fileList(0).Name, fileList(0).FileName, Common.Callback, Nothing)
                'Options, no events, with callback
            ElseIf Common.UseOptions AndAlso (Not Common.UseEvents) AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFile(Common.Url, fileList(0).Stream, fileList(0).Name, fileList(0).FileName, Common.Options, Nothing, Nothing, Common.Callback, Nothing)
                'No options, events, with callback
            ElseIf (Not Common.UseOptions) AndAlso Common.UseEvents AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFile(Common.Url, fileList(0).Stream, fileList(0).Name, fileList(0).FileName, Nothing, Common.Events, Nothing, Common.Callback, Nothing)
                'Options, events, with callback
            ElseIf Common.UseOptions AndAlso Common.UseEvents AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFile(Common.Url, fileList(0).Stream, fileList(0).Name, fileList(0).FileName, Common.Options, Common.Events, Nothing, Common.Callback, Nothing)
                'Every other possibility will result in calling this method
            Else
                result = HttpUploader.BeginUploadFile(Common.Url, fileList(0).Stream, fileList(0).Name, fileList(0).FileName)
            End If

            Return result
        End Function
#End Region 'Method BeginSendFile()

#Region "Method BeginSendFiles()"
        ' This is where the actual call to the component's methods are made when sending multiple files.
        ' Depending on whether options, events, and a callback are provided, we use different methods 
        Public Shared Function BeginSendFiles(ByVal fileList As List(Of FileToUpload)) As IAsyncResult
            Dim result As IAsyncResult

            'No options, no events, with callback
            If (Not Common.UseOptions) AndAlso (Not Common.UseEvents) AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFiles(Common.Url, fileList, Common.Callback, Nothing)
                'Options, no events, with callback
            ElseIf Common.UseOptions AndAlso (Not Common.UseEvents) AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFiles(Common.Url, fileList, Common.Options, Nothing, Nothing, Common.Callback, Nothing)
                'No options, events, with callback
            ElseIf (Not Common.UseOptions) AndAlso Common.UseEvents AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFiles(Common.Url, fileList, Nothing, Common.Events, Nothing, Common.Callback, Nothing)
                'Options, events, with callback
            ElseIf Common.UseOptions AndAlso Common.UseEvents AndAlso Common.Callback IsNot Nothing Then
                result = HttpUploader.BeginUploadFiles(Common.Url, fileList, Common.Options, Common.Events, Nothing, Common.Callback, Nothing)
                'Every other possibility will result in calling this method
            Else
                result = HttpUploader.BeginUploadFiles(Common.Url, fileList)
            End If

            Return result
        End Function
#End Region 'Method BeginSendFiles()

#Region "Private Fields"
        'The list of files to upload
        Private Shared m_fileList As List(Of FileToUpload)
#End Region 'Private Fields

    End Class

End Namespace
