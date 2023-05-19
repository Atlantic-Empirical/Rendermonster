Imports Amazon.SimpleDB.Model
Imports SMT.AWS.Authorization
Imports SMT.AWS.SDB
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.AWS.S3

Namespace FileManagement

    Public Module File_Management

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

#End Region 'FIELDS & PROPERTIES

#Region "STORAGE"

        Public Sub SaveAtjFile(ByVal F As cSMT_ATJ_File)
            Try
                If String.IsNullOrEmpty(F.Id) Then F.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(F.ToSDB(), F.Id, sdb_domain_files)
            Catch ex As Exception
                Throw New Exception("Problem with SaveFile(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetAtjFiles(ByVal JobId As String, Optional ByVal FileType_Floor As cSMT_ATJ_File.eAtjFileType = 0, Optional ByVal FileType_Ceiling As cSMT_ATJ_File.eAtjFileType = 0) As List(Of cSMT_ATJ_File)
            Try
                Dim out As New List(Of cSMT_ATJ_File)
                Dim Q As New cSMT_AWS_SDB_Query
                Dim pos As Byte = 0
                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                ' JOBID
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("JobId", JobId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                Q.Predicates.Add(p_kvp)
                pos += 1

                ' FILE TYPE
                If FileType_Floor > 0 Or FileType_Ceiling > 0 Then

                    Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                    pos += 1

                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("_Type", FileType_Floor, eComparisonOperator.GreaterThanOrEqualTo)
                    P.Add(C, eAndOr.and)
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("_Type", FileType_Ceiling, eComparisonOperator.LessThanOrEqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    pos += 1
                    Q.Predicates.Add(p_kvp)

                End If

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_files, q_str)

                Dim al As List(Of Attribute)
                Dim f As cSMT_ATJ_File
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_files)
                    f = cSMT_ATJ_File.FromSDB(al)
                    If f.Type = cSMT_ATJ_File.eAtjFileType.MXI Then
                        Dim S3 As New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey, False)
                        f.Url = S3.GetObjectUrl_Signed(s3_bucket_files_us, f.Id, 900)
                    End If
                    out.Add(f)
                Next

                out.Sort(New cSMT_ATJ_File.cSMT_ATJ_fILE_Sorter)

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetAtjFiles(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetAtjFile(ByVal FileId As String) As cSMT_ATJ_File
            Try
                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                ' FileId
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Id", FileId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                Q.Predicates.Add(p_kvp)
                pos += 1

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_files, q_str)
                If QR.ItemName.Count < 1 Then Return Nothing
                Dim al As List(Of Attribute) = SDBObject.GetAttributes(QR.ItemName(0), sdb_domain_files)
                Dim out As cSMT_ATJ_File = cSMT_ATJ_File.FromSDB(al)
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetAtjFile(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetImageFilesForJob(ByVal JobId As String) As List(Of cSMT_ATJ_File)
            Return GetAtjFiles(JobId, cSMT_ATJ_File.eAtjFileType.IntermediateOutputImage, cSMT_ATJ_File.eAtjFileType.FinalOutputImage)
        End Function

        Public Function GetImageGuidsForJob(ByVal JobId As String) As String()
            Try
                Dim files As List(Of cSMT_ATJ_File) = GetImageFilesForJob(JobId)
                Dim out As New List(Of String)
                For Each f As cSMT_ATJ_File In files
                    If Not String.IsNullOrEmpty(f.IsAvailable) AndAlso f.IsAvailable = "True" Then
                        out.Add(f.Id)
                    End If
                Next
                Return out.ToArray
            Catch ex As Exception
                Throw New Exception("Problem with GetImageGuidsForJob(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub SetFileAvailable(ByVal FileId As String)
            SetFileAttribute(FileId, "IsAvailable", "True")
        End Sub

        Public Sub SetFileAttribute(ByVal FileId As String, ByVal Name As String, ByVal Value As String)
            Try
                If String.IsNullOrEmpty(FileId) Then Throw New Exception("Invalid JobId.")
                SDBObject.SetItemAttribute(sdb_domain_files, FileId, Name, Value)
            Catch ex As Exception
                Throw New Exception("Problem with SetFileAttribute(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetJobLatestImageFile(ByVal JobId As String, Optional ByVal GetUrl As Boolean = False, Optional ByVal SignUrl As Boolean = True, Optional ByVal UrlDuration As Integer = 900, Optional ByVal Token As String = "") As cSMT_ATJ_File
            Try
                Dim FileId As String = GetJobAttribute(JobId, "LatestImageId")
                If String.IsNullOrEmpty(FileId) Then Return Nothing

                If Token = FileId Then Return Nothing
                Dim out As cSMT_ATJ_File = GetAtjFile(FileId)

                If GetUrl Then
                    Dim S3 As New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey, False)
                    If S3.ObjectExists(s3_bucket_files_us, FileId) Then
                        If SignUrl Then
                            out.Url = S3.GetObjectUrl_Signed(s3_bucket_files_us, FileId, UrlDuration)
                        Else
                            out.Url = S3.GetObjectUrl_Unsigned(s3_bucket_files_us, FileId)
                        End If
                    End If
                End If

                Return out

                ''get file ids for intermediate and final image
                ''whichever is latest that IsAvailable is our guy

                'Dim files As List(Of cSMT_ATJ_File) = GetImageFilesForJob(JobId)
                'For i As Integer = 0 To files.Count - 1
                '    If files(i).IsAvailable Then Return files(i).Id
                'Next
                'Return ""
            Catch ex As Exception
                Throw New Exception("Problem with GetJobLatestImageFile(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetFileCount() As UInt32
            Try
                'Dim Q As New cSMT_AWS_SDB_Query

                'Dim pos As Byte = 0

                'Dim P As cSMT_AWS_SDB_QueryPredicate
                'Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                'Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                'P = New cSMT_AWS_SDB_QueryPredicate
                'C = New cSMT_AWS_SDB_QueryAttributeCompare("JobId", JobId, eComparisonOperator.EqualTo)
                'P.Add(C, eAndOr.NOT_INDICATED)
                'p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                'Q.Predicates.Add(p_kvp)
                'pos += 1

                'Dim q_str As String = Q.ToString
                ''Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_files, "")
                Return QR.ItemName.Count
            Catch ex As Exception
                Throw New Exception("Problem with GetFileCount(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'STORAGE

    End Module

End Namespace
