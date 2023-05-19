Imports Amazon.SimpleDB.Model
Imports Amazon.SQS.Model
Imports SMT.Alentejo.Core.Consts
Imports SMT.AWS.Authorization
Imports SMT.AWS.SDB
Imports SMT.AWS.SQS
Imports System.Reflection

Namespace SystemwideMessaging

    Public Module System_Message

#Region "PUBLIC EXCEPTION SUBMISSION"

        Public Sub SubmitSystemMessage(ByVal nMethodBase As MethodBase, ByVal nJobId As String, ByVal nMsg As String, ByVal nStackTrace As String)
            Try
                Dim ex As New cSMT_ATJ_SystemMessage(nMethodBase.Module.Name, nMethodBase.Name, nStackTrace, nJobId, nMsg)
                Dim ExId As String = StoreSystemException(ex)
                If String.IsNullOrEmpty(nJobId) Then
                    Enqueue_SystemException(ExId)
                End If
            Catch ex As Exception
                Debug.WriteLine("WARNING: Problem with SubmitError: " & ex.Message)
            End Try
        End Sub

#End Region 'PUBLIC EXCEPTION SUBMISSION

#Region "SYSTEM EXCEPTION QUEUE"

        Private ReadOnly Property SQSObject() As cSMT_AWS_SQS
            Get
                If _SQSObject Is Nothing Then _SQSObject = New cSMT_AWS_SQS(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SQSObject
            End Get
        End Property
        Private _SQSObject As cSMT_AWS_SQS

        ''' <summary>
        ''' Client code should use the SubmitError method above.
        ''' </summary>
        ''' <param name="ExceptionId"></param>
        ''' <remarks></remarks>
        Private Sub Enqueue_SystemException(ByVal ExceptionId As String)
            Dim msgId As String = SQSObject.SendMessage(sqs_Q_system_messages, ExceptionId)
        End Sub

        ''' <summary>
        ''' Gets ExceptionId.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Pop_SystemException() As String
            Dim l As List(Of Message) = SQSObject.ReceiveMessages(sqs_Q_system_messages, 1, 10)
            If l IsNot Nothing AndAlso l.Count > 0 Then
                SQSObject.DeleteMessage(sqs_Q_system_messages, l.Item(0).ReceiptHandle)
                Return cSMT_AWS_SQS.DecodeString(l.Item(0).Body)
            Else
                Return Nothing
            End If
        End Function

#End Region 'SYSTEM EXCEPTION QUEUE

#Region "SYSTEM EXCEPTION DOMAIN"

        Private ReadOnly Property SDBObject() As SMT.AWS.SDB.cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

        ''' <summary>
        ''' Client code should call SubmitError() above.
        ''' Returns the ExceptionId (guid) for this exception.
        ''' </summary>
        ''' <param name="Exc"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function StoreSystemException(ByVal Exc As cSMT_ATJ_SystemMessage) As String
            Try
                If String.IsNullOrEmpty(Exc.Id) Then Exc.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(Exc.ToSDB(), Exc.Id, sdb_domain_system_messages)
                Return Exc.Id
            Catch ex As Exception
                Throw New Exception("Problem with StoreSystemException(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetSystemException(ByVal ExceptionId As String) As cSMT_ATJ_SystemMessage
            Try
                'Dim out As cSMT_ATJ_SystemMessage = Nothing

                'Dim Q As New cSMT_AWS_SDB_Query

                'Dim pos As Byte = 0

                'Dim P As New cSMT_AWS_SDB_QueryPredicate()
                'Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Id", ExceptionId, eComparisonOperator.EqualTo)
                'P.Add(C, eAndOr.NOT_INDICATED)
                'Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                'pos += 1
                'Q.Predicates.Add(p_kvp)

                ''If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                ''    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                ''End If

                'Dim q_str As String = Q.ToString
                ''Debug.WriteLine(q_str)

                'Dim QR As QueryResult = SDBObject.Query(sdb_domain_jobs, q_str)

                'Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                'For Each item As String In QR.ItemName

                '    al = SDBObject.GetAttributes(item, sdb_domain_jobs)
                '    out = cSMT_ATJ_SystemMessage.FromSDB(al)
                'Next

                'Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetSystemException(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetSystemMessages(ByVal SMQ As cSMT_ATJ_SystemMessageQuery) As List(Of cSMT_ATJ_SystemMessage)
            Try
                Dim out As New List(Of cSMT_ATJ_SystemMessage)

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare

                If Not String.IsNullOrEmpty(SMQ.JobId) Then
                    P = New cSMT_AWS_SDB_QueryPredicate()
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("JobId", SMQ.JobId, eComparisonOperator.EqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    pos += 1
                    Q.Predicates.Add(p_kvp)
                End If

                'If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                '    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                'End If

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_system_messages, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_system_messages)
                    out.Add(cSMT_ATJ_SystemMessage.FromSDB(al))
                Next

                out.Sort(New cSMT_ATJ_SystemMessage.cSMT_ATJ_SystemMessage_Sorter)

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetSystemMessages(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'SYSTEM EXCEPTION DOMAIN

#Region "CLASSES"

        Public Class cSMT_ATJ_SystemMessage

            Public SCHEMA_VERSION As String = "001"

            Public Id As String

            Public JobId As String
            Public Msg As String
            Public SourceModule As String
            Public SourceMethod As String
            Public StackTrace As String
            Public WriteOnly Property TimeStamp_Ticks() As String
                Set(ByVal value As String)
                    _TimeStamp_Ticks = value.PadLeft("20", "0")
                End Set
            End Property
            ''' <summary>
            ''' Do not set directly. Use TimeStamp_Ticks property to set.
            ''' </summary>
            ''' <remarks></remarks>
            Public _TimeStamp_Ticks As String

            Public Sub New()
                TimeStamp_Ticks = DateTime.UtcNow.Ticks
            End Sub

            Public Sub New(ByVal nSourceModule As String, ByVal nSourceMethod As String, ByVal nStackTrace As String, ByVal nJobId As String, ByVal nMsg As String)
                SourceModule = nSourceModule
                SourceMethod = nSourceMethod
                StackTrace = nStackTrace
                JobId = nJobId
                Msg = nMsg
                TimeStamp_Ticks = DateTime.UtcNow.Ticks
            End Sub

#Region "OVERRIDES"

            Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
                Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
            End Function

            Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Amazon.SimpleDB.Model.Attribute)) As cSMT_ATJ_SystemMessage
                Dim p As New cSMT_ATJ_SystemMessage
                Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_SystemMessage)(Attributes, p)
            End Function

#End Region 'OVERRIDES

            Public Shadows Function ToString(Optional ByVal T As Char = "t") As String
                Select Case T
                    Case "m"c
                        Return Msg
                    Case "t"c
                        Return New DateTime(_TimeStamp_Ticks, DateTimeKind.Utc).ToString("r")
                    Case Else
                        'Return MyBase.ToString
                        Return SourceModule
                End Select
            End Function

            Public Class cSMT_ATJ_SystemMessage_Sorter
                Implements IComparer(Of cSMT_ATJ_SystemMessage)

                Public Function Compare(ByVal X As cSMT_ATJ_SystemMessage, ByVal Y As cSMT_ATJ_SystemMessage) As Integer Implements IComparer(Of SMT.Alentejo.Core.SystemwideMessaging.cSMT_ATJ_SystemMessage).Compare
                    Dim Msg1 As cSMT_ATJ_SystemMessage = DirectCast(X, cSMT_ATJ_SystemMessage)
                    Dim Msg2 As cSMT_ATJ_SystemMessage = DirectCast(Y, cSMT_ATJ_SystemMessage)
                    Dim l As Long = Msg1._TimeStamp_Ticks - Msg2._TimeStamp_Ticks
                    If l < 0 Then Return -1
                    If l = 0 Then Return 0
                    If l > 0 Then Return 1
                End Function

            End Class

        End Class

        Public Class cSMT_ATJ_SystemMessageQuery

            Public JobId As String
            Public JobName As String
            Public InstanceId As String
            Public BeginTicks As String
            Public EndTicks As String

            Public Sub New()
            End Sub

        End Class

#End Region 'CLASSES

    End Module

End Namespace
