Imports SMT.AWS.SDB
Imports Amazon.SimpleDB.Model
Imports SMT.AWS.Authorization
Imports SMT.Alentejo.Core.AtjTrace

Namespace UserManagement

    Public Module User_Management

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As SMT.AWS.SDB.cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

#End Region 'FIELDS & PROPERTIES

#Region "USERS DOMAIN"

        Public Function GetUsers() As List(Of cSMT_ATJ_USER)
            Try
                Dim out As New List(Of cSMT_ATJ_USER)
                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, "")
                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_users)
                    out.Add(cSMT_ATJ_USER.FromSDB(al))
                Next
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetUsers(). Error: " & ex.Message, ex)
            End Try
        End Function

        ''' <summary>
        ''' Returns User.Id
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SaveUser(ByRef User As cSMT_ATJ_USER) As String
            Try
                If String.IsNullOrEmpty(User.Id) Then User.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(User.ToSDB(), User.Id, sdb_domain_users)
                Return User.Id
            Catch ex As Exception
                Throw New Exception("Problem with SaveUser(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUser(ByVal UserId As String) As cSMT_ATJ_USER
            Try
                Dim out As cSMT_ATJ_USER = Nothing

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Id", UserId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)

                'If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                '    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                'End If

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName

                    al = SDBObject.GetAttributes(item, sdb_domain_users)
                    out = cSMT_ATJ_USER.FromSDB(al)
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetUser(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUserLite(ByVal UserId As String) As cSMT_ATJ_User_Lite
            Try
                Dim out As cSMT_ATJ_User_Lite = Nothing

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Id", UserId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)

                'If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                '    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                'End If

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName

                    al = SDBObject.GetAttributes(item, sdb_domain_users)
                    out = cSMT_ATJ_User_Lite.FromSDB(al)
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetUserLite(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUsersLite(ByVal UIQ As cSMT_ATJ_UserInfoQuery) As List(Of cSMT_ATJ_User_Lite)
            Try
                Dim Q As New cSMT_AWS_SDB_Query
                Dim q_str As String = ""

                If UIQ IsNot Nothing Then

                    Dim pos As Byte = 0
                    Dim P As cSMT_AWS_SDB_QueryPredicate
                    Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                    Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                    'UserId
                    If Not String.IsNullOrEmpty(UIQ.Keyword) Then

                        P = New cSMT_AWS_SDB_QueryPredicate
                        C = New cSMT_AWS_SDB_QueryAttributeCompare("Username", UIQ.Keyword, eComparisonOperator.StartsWith)
                        P.Add(C, eAndOr.NOT_INDICATED)
                        p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                        Q.Predicates.Add(p_kvp)
                        pos += 1

                        Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.union))
                        pos += 1

                        P = New cSMT_AWS_SDB_QueryPredicate
                        C = New cSMT_AWS_SDB_QueryAttributeCompare("FirstName", UIQ.Keyword, eComparisonOperator.StartsWith)
                        P.Add(C, eAndOr.NOT_INDICATED)
                        p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                        Q.Predicates.Add(p_kvp)
                        pos += 1

                        Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.union))
                        pos += 1

                        P = New cSMT_AWS_SDB_QueryPredicate
                        C = New cSMT_AWS_SDB_QueryAttributeCompare("LastName", UIQ.Keyword, eComparisonOperator.StartsWith)
                        P.Add(C, eAndOr.NOT_INDICATED)
                        p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                        Q.Predicates.Add(p_kvp)
                        pos += 1

                        Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.union))
                        pos += 1

                        P = New cSMT_AWS_SDB_QueryPredicate
                        C = New cSMT_AWS_SDB_QueryAttributeCompare("Company", UIQ.Keyword, eComparisonOperator.StartsWith)
                        P.Add(C, eAndOr.NOT_INDICATED)
                        p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                        Q.Predicates.Add(p_kvp)
                        pos += 1

                        Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.NOT_INDICATED))
                        pos += 1

                    End If

                    'P = New cSMT_AWS_SDB_QueryPredicate
                    'C = New cSMT_AWS_SDB_QueryAttributeCompare("LastLogin_UTCTicks", UIQ.ActiveStartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                    'P.Add(C, eAndOr.and)
                    'C = New cSMT_AWS_SDB_QueryAttributeCompare("LastLogin_UTCTicks", UIQ.ActiveEndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                    'P.Add(C, eAndOr.NOT_INDICATED)
                    'p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    'Q.Predicates.Add(p_kvp)
                    'pos += 1

                    q_str = Q.ToString
                End If


                'Dim P As New cSMT_AWS_SDB_QueryPredicate()
                'Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Id", UserId, eComparisonOperator.EqualTo)
                'P.Add(C, eAndOr.NOT_INDICATED)
                'Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                'pos += 1
                'Q.Predicates.Add(p_kvp)


                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, q_str)
                Dim out As New List(Of cSMT_ATJ_User_Lite)
                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_users)
                    out.Add(cSMT_ATJ_User_Lite.FromSDB(al))
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetUsersLite(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUserByAttribute(ByVal AttributeName As String, ByVal AttributeValue As String) As cSMT_ATJ_User
            Try
                Dim out As cSMT_ATJ_User = Nothing

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare(AttributeName, AttributeValue, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)

                'If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                '    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                'End If

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName

                    al = SDBObject.GetAttributes(item, sdb_domain_users)
                    out = cSMT_ATJ_User.FromSDB(al)
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetUserByAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUserIdByUsername(ByVal Username As String) As String
            Dim user As cSMT_ATJ_User = GetUserByAttribute("Username", Username)
            If user Is Nothing Then Return ""
            Return user.Id
        End Function

        Public Function GetUserAttribute(ByVal UserId As String, ByVal AttributeName As String) As String
            Try
                If String.IsNullOrEmpty(UserId) Then Throw New Exception("Invalid JobId.")
                Return SDBObject.GetItemAttribute(sdb_domain_users, UserId, AttributeName)
            Catch ex As Exception
                Throw New Exception("Problem with GetUserAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub UpdateUserLoginInfo(ByVal UserId As String, ByVal ClientIp As String)
            SetUserAttribute(UserId, "LastLogin_UTCTicks", DateTime.UtcNow.Ticks.ToString.PadLeft(20, "0"))
            SetUserAttribute(UserId, "LastLoginIpAddress", ClientIp)
        End Sub

        Public Sub SetUserAttribute(ByVal UserId As String, ByVal Name As String, ByVal Value As String)
            Try
                If String.IsNullOrEmpty(UserId) Then Throw New Exception("Invalid UserId.")
                SDBObject.SetItemAttribute(sdb_domain_users, UserId, Name, Value)
            Catch ex As Exception
                Throw New Exception("Problem with SetUserAttribute(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function UserNameExists(ByVal Username As String) As Boolean
            Try
                Dim Q As New cSMT_AWS_SDB_Query
                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Username", Username.ToLower, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                Q.Predicates.Add(p_kvp)
                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, Q.ToString)

                Dim atjTRACE As New cSMT_ATJ_TRACE("UserNameExists")
                atjTRACE.LogMessage(Q.ToString & " - " & Username & " - " & QR.ItemName.Count, EventLogEntryType.Information)
                Dim out As Boolean = QR.ItemName.Count > 0
                atjTRACE.LogMessage(out.ToString, EventLogEntryType.Information)
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with UserNameExists(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUsersCount(ByVal UIQ As cSMT_ATJ_UserInfoQuery) As UInt32
            Try
                Dim q_str As String = ""

                If UIQ IsNot Nothing Then
                    Dim Q As New cSMT_AWS_SDB_Query
                    'Dim pos As Byte = 0
                    Dim P As cSMT_AWS_SDB_QueryPredicate
                    Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                    Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)
                    'use date filter
                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("LastLogin_UTCTicks", UIQ.ActiveStartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                    P.Add(C, eAndOr.and)
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("LastLogin_UTCTicks", UIQ.ActiveEndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                    'pos += 1
                    Q.Predicates.Add(p_kvp)
                    'Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                    'pos += 1
                    q_str = Q.ToString
                End If

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_users, q_str)
                Return QR.ItemName.Count
            Catch ex As Exception
                Throw New Exception("Problem with GetUsersRegisteredCount(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'JOBS DOMAIN

    End Module

End Namespace
