Imports SMT.Jellyfish
Imports SMT.Alentejo.Core
Imports SMT.AWS.S3
Imports SMT.AWS.Authorization
Imports System.IO
Imports System.Drawing

Public Class cSMT_ATJ_GenerateDeepZoom

#Region "PROPERTIES"

    Private JobId As String
    Private FileGuid As String
    Private ReadOnly Property FileName() As String
        Get
            Return FileGuid & ".jpg"
        End Get
    End Property

    Private PhysicalPath_ServerRoot As String
    Private ReadOnly Property PhysicalPath_ClientBin() As String
        Get
            If String.IsNullOrEmpty(PhysicalPath_ServerRoot) Then Return ""
            Return PhysicalPath_ServerRoot & "ClientBin\"
        End Get
    End Property
    Private DeepZoomScratchFolder As String = "DeepZoomScratch"

    Private ReadOnly Property S3() As cSMT_AWS_S3
        Get
            If _S3 Is Nothing Then _S3 = New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
            Return _S3
        End Get
    End Property
    Private _S3 As cSMT_AWS_S3

    Private ReadOnly Property SampleImageInLocalStoragePath() As String
        Get
            If Not Directory.Exists(ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE & JobId) Then Directory.CreateDirectory(ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE & JobId)
            Return ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE & JobId & "\" & FileName
        End Get
    End Property

    Private ReadOnly Property XMLRelativePath() As String
        Get
            Return DeepZoomScratchFolder & "/" & JobId & "/" & FileGuid & "/collection_images/" & FileGuid & ".xml"
        End Get
    End Property

    Public ReadOnly Property OutputDirectory() As String
        Get
            If String.IsNullOrEmpty(PhysicalPath_ClientBin) Then
                Return Path.GetDirectoryName(SampleImageInLocalStoragePath) & "/Output"
            Else
                Return PhysicalPath_ClientBin & DeepZoomScratchFolder & "/" & JobId & "/" & FileGuid
            End If
        End Get
    End Property

#End Region 'PROPERTIES

#Region "EVENTS"

    Public Event evXMLPathAvailable(ByRef XMLPath As String)

#End Region 'EVENTS

#Region "CONSTRUCTOR"

    Public Sub New(ByRef nJobId As String, ByRef nFileGuid As String, Optional ByRef nPhysicalPathToServerRoot As String = "")
        JobId = nJobId
        FileGuid = nFileGuid
        PhysicalPath_ServerRoot = nPhysicalPathToServerRoot
    End Sub

#End Region 'CONSTRUCTOR

#Region "API"

    Public Function GetXMLPath() As String
        If String.IsNullOrEmpty(JobId) Or String.IsNullOrEmpty(FileGuid) Then Return ""
        If File.Exists(PhysicalPath_ClientBin & XMLRelativePath) Then Return XMLRelativePath

        If File.Exists(SampleImageInLocalStoragePath()) Then
            Return EntryPoint()
        Else
            RetrieveSampleImageFromS3()
            Return "downloading"
        End If
    End Function

#End Region 'API

#Region "S3"

    Private Sub RetrieveSampleImageFromS3()
        Try
            Dim Download1 As New cSMT_AWS_S3_FileTransfer(S3.S3)
            Download1.DownloadFile(s3_bucket_files_us, FileGuid, SampleImageInLocalStoragePath(), S3.GetObjectSize(s3_bucket_files_us, FileGuid), cSMT_AWS_S3_FileTransfer.eBucketLocation.US)
            AddHandler Download1.evTransferSucceeded, AddressOf TransferCompleted_Callback
        Catch ex As Exception
            Throw New Exception("Problem with RetrieveSampleImageFromS3(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Private Sub TransferCompleted_Callback(ByVal Direction As SMT.AWS.S3.cSMT_AWS_S3_FileTransfer.eDirection, ByVal FileName As String, ByVal AS3_ObjectKey As String)
        EntryPoint()
    End Sub

#End Region

    Private Function EntryPoint() As String
        Return ConvertToJpgIfNeeded()
    End Function

#Region "CONVERT IMAGE TO JPG"

    Private Function ConvertToJpgIfNeeded() As String
        Try
            If Not IsJpeg(SampleImageInLocalStoragePath) Then
                Dim bm As New Bitmap(SampleImageInLocalStoragePath)
                Dim newPath As String = Path.GetDirectoryName(SampleImageInLocalStoragePath) & "\" & Path.GetFileNameWithoutExtension(SampleImageInLocalStoragePath) & "_new.jpg"
                bm.Save(newPath, System.Drawing.Imaging.ImageFormat.Jpeg)
                File.Delete(SampleImageInLocalStoragePath)
                File.Copy(newPath, SampleImageInLocalStoragePath & ".jpg")
            Else
                If Not Path.GetExtension(SampleImageInLocalStoragePath).ToLower = ".jpg" Then
                    File.Copy(SampleImageInLocalStoragePath, SampleImageInLocalStoragePath & ".jpg")
                    File.Delete(SampleImageInLocalStoragePath)
                End If
            End If
        Catch ex As Exception
            Return "Failure in ConvertToJpgIfNeeded(). Error: " & ex.Message
        End Try

        Return CreateDeepZoomCollection()

    End Function

    Private Function IsJpeg(ByRef FilePath As String) As Boolean
        'FF D8 FF E1 FF FE
        Dim fs As New FileStream(FilePath, FileMode.Open)
        Dim b(7) As Byte
        fs.Read(b, 0, 6)
        fs.Close()
        fs.Dispose()
        Return BitConverter.ToUInt64(b, 0) = &H1000E0FFD8FF
    End Function

#End Region 'CONVERT IMAGE TO JPG

#Region "JELLYFISH"

    Private Function CreateDeepZoomCollection() As String
        If Not API.CreateDeepZoomCollectionForImage(SampleImageInLocalStoragePath, OutputDirectory, 256, 90, 5, 5, 1) Then Return ""
        Return DeepZoomScratchFolder & "/" & JobId & "/" & FileGuid & "/collection_images/" & Path.GetFileNameWithoutExtension(SampleImageInLocalStoragePath) & ".xml"
    End Function

#End Region 'JELLYFISH

End Class
