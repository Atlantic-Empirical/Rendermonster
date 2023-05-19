Imports SMT.AWS.SDB
Imports SMT.AWS.Authorization
Imports SMT.Alentejo.Core.JobManagement.Classes
Imports Amazon.SimpleDB.Model
Imports SMT.Alentejo.Core.Consts
Imports SMT.AWS
Imports SMT.AWS.SQS

Namespace SessionManagement

    Public Module Session_Storage

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As SMT.AWS.SDB.cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

#End Region 'FIELDS & PROPERTIES

#Region "SESSION DOMAIN"

        Public Function SaveSession(ByVal Session As cSMT_ATJ_Session) As String
            Try
                If String.IsNullOrEmpty(Session.Id) Then Session.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(Session.ToSDB(), Session.Id, sdb_domain_sessions)
                Return Session.Id
            Catch ex As Exception
                Throw New Exception("Problem with SaveSession(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetSessionByAttribute(ByRef AttributeName As String, ByRef AttributeValue As String) As cSMT_ATJ_Session
            Try
                Dim out As cSMT_ATJ_Session = Nothing

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

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_jobs, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName

                    al = SDBObject.GetAttributes(item, sdb_domain_jobs)
                    out = cSMT_ATJ_Session.FromSDB(al)
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetSessionByAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function SessionCheck(ByVal SessionId As String, ByVal IpAddress As String) As Boolean
            If String.IsNullOrEmpty(SessionId) OrElse String.IsNullOrEmpty(IpAddress) Then Return False
            Dim s As cSMT_ATJ_Session = GetSessionByAttribute("Id", SessionId)
            If s Is Nothing Then Return False
            If Not s.IpAddress = IpAddress Then Return False
            If Not String.IsNullOrEmpty(s.LogoutUTCTicks) Then Return False
            Dim TS As New TimeSpan(DateTime.UtcNow.Ticks - s.LastActivityUTCTicks)
            If TS.TotalMinutes > 120 Then Return False

            'We're good
            SetSessionAttribute(SessionId, "LastActivityUTCTicks", DateTime.UtcNow.Ticks)
            Return True
        End Function

        Public Function GetSessionAttribute(ByRef SessionId As String, ByRef Name As String) As String
            Try
                If String.IsNullOrEmpty(SessionId) Then Throw New Exception("Invalid SessionId.")
                Return SDBObject.GetItemAttribute(sdb_domain_sessions, SessionId, Name)
            Catch ex As Exception
                Throw New Exception("Problem with GetSessionAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub SetSessionAttribute(ByVal SessionToken As String, ByVal Name As String, ByVal Value As String)
            Try
                If String.IsNullOrEmpty(SessionToken) Then Throw New Exception("Invalid SessionId.")
                SDBObject.SetItemAttribute(sdb_domain_sessions, SessionToken, Name, Value)
            Catch ex As Exception
                Throw New Exception("Problem with SetSessionAttribute(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetSessionCount(ByVal SIQ As cSMT_ATJ_SessionInfoQuery) As UInt32
            Try
                Dim q_str As String = ""

                If SIQ IsNot Nothing Then
                    Dim Q As New cSMT_AWS_SDB_Query
                    Dim P As cSMT_AWS_SDB_QueryPredicate
                    Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                    Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)
                    'use date filter
                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("LoginUTCTicks", SIQ.LoginStartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                    P.Add(C, eAndOr.and)
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("LoginUTCTicks", SIQ.LoginEndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                    Q.Predicates.Add(p_kvp)
                    q_str = Q.ToString
                End If
                Dim QR As QueryResult = SDBObject.Query(sdb_domain_sessions, q_str)
                Return QR.ItemName.Count
            Catch ex As Exception
                Throw New Exception("Problem with GetSessionCount(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetUserIdForSession(ByVal SessionToken As String) As String
            Try
                Return GetSessionAttribute(SessionToken, "UserId")
            Catch ex As Exception
                Return ""
            End Try
        End Function

#End Region 'SESSION DOMAIN

    End Module

End Namespace
