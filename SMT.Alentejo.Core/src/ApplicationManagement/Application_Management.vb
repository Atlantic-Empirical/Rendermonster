Imports Amazon.SimpleDB.Model
Imports SMT.AWS.Authorization
Imports SMT.AWS.SDB
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.AWS.S3
Imports SMT.AWS

Namespace ApplicationManagement

    Public Module Application_Management

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

        Private ReadOnly Property S3Object() As cSMT_AWS_S3
            Get
                If _S3 Is Nothing Then _S3 = New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _S3
            End Get
        End Property
        Private _S3 As cSMT_AWS_S3

#End Region 'FIELDS & PROPERTIES

#Region "SERVER-TO-APPLICATION MAPPING"

        Public Function GetAppListForServer(ByRef Server As eSMT_ATJ_Server) As List(Of cSMT_ATJ_Application)
            Try
                'first get all the server-to-application maps for this server
                Dim ServerAppMaps As List(Of Attribute) = SDBObject.GetAttributes("ServerAppMaps", sdb_domain_system_settings, ObjectToList(CInt(Server).ToString))
                If ServerAppMaps.Count = 0 Then Return Nothing

                'now, get all the application-to-appId maps
                Dim appIds As New List(Of String)
                Dim appMaps As New List(Of Attribute)
                For Each sam As Attribute In ServerAppMaps
                    appMaps = SDBObject.GetAttributes("ApplicationMaps", sdb_domain_system_settings, ObjectToList(sam.Value))
                    For Each am As Attribute In appMaps
                        appIds.Add(am.Value)
                    Next
                Next

                Dim out As New List(Of cSMT_ATJ_Application)
                For Each aid As String In appIds
                    out.Add(GetApplication(aid))
                Next
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetAppListForServer(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub SetApplicationMap(ByVal App As eSMT_ATJ_ServerApp, ByVal AppId As String)
            Try
                Dim ra As New ReplaceableAttribute
                ra.Name = App
                ra.Value = AppId
                ra.Replace = True
                SDBObject.PutAttributes(ObjectToList(ra), "ApplicationMaps", sdb_domain_system_settings)
            Catch ex As Exception
                Throw New Exception("Problem with SetApplicationMap(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Sub SetServerMap(ByVal Server As eSMT_ATJ_Server, ByVal App As eSMT_ATJ_ServerApp)
            Try
                Dim ra As New ReplaceableAttribute
                ra.Name = Server
                ra.Value = App
                ra.Replace = False
                SDBObject.PutAttributes(ObjectToList(ra), "ServerAppMaps", sdb_domain_system_settings)
            Catch ex As Exception
                Throw New Exception("Problem with SetServerMap(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Sub DeleteServerMap(ByVal Server As eSMT_ATJ_Server, ByVal App As eSMT_ATJ_ServerApp)
            Try
                Dim a As New Attribute
                a.Name = Server
                a.Value = App
                SDBObject.DeleteAttributes("ServerAppMaps", sdb_domain_system_settings, ObjectToList(a))
            Catch ex As Exception
                Throw New Exception("Problem with DeleteServerMap(). Error: " & ex.Message, ex)
            End Try
        End Sub

#End Region 'SERVER-TO-APPLICATION MAPPING

#Region "APPLICATIONS"

        ''' <summary>
        ''' Returns App.Id
        ''' </summary>
        ''' <param name="App"></param>
        ''' <param name="Files"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveApplication(ByRef App As cSMT_ATJ_Application, ByVal Files() As cSMT_ATJ_File) As String
            Try
                If String.IsNullOrEmpty(App.Id) Then App.Id = Guid.NewGuid.ToString
                For Each f As cSMT_ATJ_File In Files
                    f.AppId = App.Id
                    SaveApplicationFile(f)
                Next
                SDBObject.PutAttributes(App.ToSDB(), App.Id, sdb_domain_applications)
                Return App.Id
            Catch ex As Exception
                Throw New Exception("Problem with SaveApplication(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetApplication(ByVal AppId As String) As cSMT_ATJ_Application
            Try
                Dim Q As New cSMT_AWS_SDB_Query
                Dim pos As Byte = 0
                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Id", AppId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                Q.Predicates.Add(p_kvp)
                pos += 1

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_applications, q_str)
                Dim al As List(Of Attribute)
                al = SDBObject.GetAttributes(QR.ItemName(0), sdb_domain_applications)
                Return cSMT_ATJ_Application.FromSDB(al)
            Catch ex As Exception
                Throw New Exception("Problem with GetApplicationFiles(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetApplicationList() As List(Of cSMT_ATJ_Application)
            Try
                Dim QR As QueryResult = SDBObject.Query(sdb_domain_applications, "")
                Dim al As List(Of Attribute)
                Dim out As New List(Of cSMT_ATJ_Application)
                For Each i As String In QR.ItemName
                    al = SDBObject.GetAttributes(i, sdb_domain_applications)
                    out.Add(cSMT_ATJ_Application.FromSDB(al))
                Next
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetApplicationList(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub DeleteApplication(ByVal ServerApp As eSMT_ATJ_ServerApp)
            Try
                Dim AppId As String = GetApplicationId(ServerApp.ToString)
                If String.IsNullOrEmpty(AppId) Then Exit Sub
                Dim App As cSMT_ATJ_Application = GetApplication(AppId)
                SDBObject.DeleteAttributes(AppId, sdb_domain_applications)

                Dim files As List(Of cSMT_ATJ_File) = GetApplicationFiles(AppId)
                For Each f As cSMT_ATJ_File In files
                    SDBObject.DeleteAttributes(f.Id, sdb_domain_application_files)
                    S3Object.DeleteObject(s3_bucket_appfiles_us, f.Id)
                Next
            Catch ex As Exception
                Throw New Exception("Problem with DeleteApplication(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetApplicationId(ByVal ApplicationName As String) As String
            Try
                Dim Q As New cSMT_AWS_SDB_Query
                Dim pos As Byte = 0
                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                ' JOBID
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Name", ApplicationName, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                Q.Predicates.Add(p_kvp)
                pos += 1

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_applications, q_str)
                If QR.ItemName.Count > 1 Then
                    'TODO: this should never be. add a system message
                End If
                If QR.ItemName.Count = 0 Then Return ""
                Dim al As List(Of Attribute)
                al = SDBObject.GetAttributes(QR.ItemName(0), sdb_domain_applications)
                Return cSMT_ATJ_Application.FromSDB(al).Id
            Catch ex As Exception
                Throw New Exception("Problem with GetApplicationId(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'APPLICATIONS

#Region "APPLICATION FILES"

        Public Function SaveApplicationFile(ByRef F As cSMT_ATJ_File) As String
            Try
                If String.IsNullOrEmpty(F.Id) Then F.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(F.ToSDB(), F.Id, sdb_domain_application_files)
                Return F.Id
            Catch ex As Exception
                Throw New Exception("Problem with SaveApplicationFile(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetApplicationFiles(ByVal AppId As String) As List(Of cSMT_ATJ_File)
            Try
                Dim out As New List(Of cSMT_ATJ_File)
                Dim Q As New cSMT_AWS_SDB_Query
                Dim pos As Byte = 0
                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                ' JOBID
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("AppId", AppId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                Q.Predicates.Add(p_kvp)
                pos += 1

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_application_files, q_str)

                Dim al As List(Of Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_application_files)
                    out.Add(cSMT_ATJ_File.FromSDB(al))
                Next

                'out.Sort(New cSMT_ATJ_File.cSMT_ATJ_fILE_Sorter)

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetApplicationFiles(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'APPLICATION FILES

    End Module

End Namespace
