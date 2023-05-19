
Namespace AWS.S3

    Public Class cTransferJob

        Public JobId As String
        Public UserName As String
        Public JobName As String
        Public TargetBucket As String
        Public Files As cTransferFiles
        Public DirectionUp As Boolean

        Public Sub New(ByVal nJobId As String, ByVal nUserName As String, ByVal nJobName As String, ByVal nFiles() As String, ByVal nTargetBucket As String, ByVal nDirectionUp As Boolean)
            JobId = nJobId
            UserName = nUserName
            JobName = nJobName
            Files = New cTransferFiles(nFiles)
            TargetBucket = nTargetBucket
            DirectionUp = nDirectionUp
        End Sub

        Public Sub New(ByVal nJobId As String, ByVal nUserName As String, ByVal nJobName As String, ByVal nFiles() As String, ByVal nGuids() As String, ByVal nTargetBucket As String, ByVal nDirectionUp As Boolean)
            JobId = nJobId
            UserName = nUserName
            JobName = nJobName
            TargetBucket = nTargetBucket
            DirectionUp = nDirectionUp
            Files = New cTransferFiles(nFiles, nGuids)
        End Sub

        Public ReadOnly Property IsComplete() As Boolean
            Get
                If Files Is Nothing Then Return False
                For Each s As String In Files.Status
                    If String.IsNullOrEmpty(s) Then Return False
                Next
                Return True
            End Get
        End Property

        ''' <summary>
        ''' Only should be called after calling IsComplete becauase this will return False 
        ''' while uploads are still running.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property IsSuccessful() As Boolean
            Get
                If Files Is Nothing Then Return False
                For Each s As String In Files.Status
                    If String.IsNullOrEmpty(s) Then Return False
                    If s.ToLower <> "true" Then Return False
                Next
                Return True
            End Get
        End Property

        Public Class cTransferFiles

            Public Files() As String
            Public Guids() As String
            Public Status() As String

            Public Sub New(ByVal nFiles() As String)
                Files = nFiles
                ReDim Guids(UBound(Files))
                ReDim Status(UBound(Files))
            End Sub

            Public Sub New(ByVal nFiles() As String, ByVal nGuids() As String)
                Files = nFiles
                Guids = nGuids
                ReDim Status(UBound(Files))
            End Sub

        End Class

    End Class

End Namespace
