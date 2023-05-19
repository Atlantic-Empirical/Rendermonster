Imports SMT.Alentejo.Core.JobManagement
Imports SMT.AWS.SDB
Imports SMT.AWS.Authorization
Imports Amazon.SimpleDB.Model
Imports SMT.Alentejo.Core.Consts
Imports SMT.AWS
Imports SMT.AWS.SQS

Namespace JobManagement

    Public Module Job_Management

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As SMT.AWS.SDB.cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

#End Region 'FIELDS & PROPERTIES

#Region "JOBS DOMAIN"

        ''' <summary>
        ''' Returns a List of jobs (any job derived from cSMT_ATJ_RenderJob_Base).
        ''' </summary>
        ''' <param name="User"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetJobs(ByVal User As String) As List(Of cSMT_ATJ_RenderJob_Maxwell)
            Try
                Dim out As New List(Of cSMT_ATJ_RenderJob_Maxwell)

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("UserName", User, eComparisonOperator.EqualTo)
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
                    out.Add(cSMT_ATJ_RenderJob_Maxwell.FromSDB(al))

                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetJobs(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetJobInfo_Lite(ByRef JIQ As cSMT_ATJ_JobInfoQuery) As List(Of cSMT_ATJ_LiteJobInfo)
            Try
                Dim out As New List(Of cSMT_ATJ_LiteJobInfo)

                Dim Q As New cSMT_AWS_SDB_Query
                Dim pos As Byte = 0
                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                'UserId
                If Not String.IsNullOrEmpty(JIQ.UserFilter) Then
                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("UserId", JIQ.UserFilter, eComparisonOperator.EqualTo)
                    P.Add(C, eAndOr.and)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    pos += 1
                    Q.Predicates.Add(p_kvp)
                    Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                    pos += 1
                End If

                'use date filter
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Submitted_Ticks", JIQ.StartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.and)
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Submitted_Ticks", JIQ.EndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)
                Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                pos += 1

                'use status filter
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("_Status", JIQ.StatusMin.ToString.PadLeft(6, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.and)
                C = New cSMT_AWS_SDB_QueryAttributeCompare("_Status", JIQ.StatusMax.ToString.PadLeft(6, "0"), eComparisonOperator.LessThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)
                'Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                'pos += 1

                'apply sort
                'If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                '    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                'End If

                Dim q_str As String = Q.ToString
                Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_jobs, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_jobs)
                    out.Add(cSMT_ATJ_LiteJobInfo.FromSDB(al))
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetJobInfo_Lite(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetJobInfo(ByRef JIQ As cSMT_ATJ_JobInfoQuery) As List(Of cSMT_ATJ_RenderJob_Maxwell)
            Try
                Dim out As New List(Of cSMT_ATJ_RenderJob_Maxwell)

                Dim Q As New cSMT_AWS_SDB_Query
                Dim pos As Byte = 0
                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                'UserId
                If Not String.IsNullOrEmpty(JIQ.UserFilter) Then
                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("UserId", JIQ.UserFilter, eComparisonOperator.EqualTo)
                    P.Add(C, eAndOr.and)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    pos += 1
                    Q.Predicates.Add(p_kvp)
                    Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                    pos += 1
                End If

                'use date filter
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Submitted_Ticks", JIQ.StartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.and)
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Submitted_Ticks", JIQ.EndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)
                Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                pos += 1

                'use status filter
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("_Status", JIQ.StatusMin.ToString.PadLeft(6, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.and)
                C = New cSMT_AWS_SDB_QueryAttributeCompare("_Status", JIQ.StatusMax.ToString.PadLeft(6, "0"), eComparisonOperator.LessThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)
                'Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                'pos += 1

                'apply sort
                'If Me.cbQU_SortOrder.SelectedItem.ToString <> "None" Then
                '    Q.Sort = New cSMT_AWS_SDB_QuerySort(txtQU_AttributeName.Text, cbQU_SortOrder.SelectedItem.ToString = "Descending")
                'End If

                Dim q_str As String = Q.ToString
                Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_jobs, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_jobs)
                    out.Add(cSMT_ATJ_RenderJob_Maxwell.FromSDB(al))
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetJobInfo(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function GetJob(ByVal JobId As String) As cSMT_ATJ_RenderJob_Maxwell
            Return GetJobByAttribute("Id", JobId)
        End Function

        Public Function GetLiteJobInfo(ByVal JobId As String) As cSMT_ATJ_LiteJobInfo
            Dim j As cSMT_ATJ_RenderJob_Maxwell = GetJobByAttribute("Id", JobId)
            Return New cSMT_ATJ_LiteJobInfo(j)
        End Function

        Public Function GetJobByAttribute(ByVal AttributeName As String, ByVal AttributeValue As String) As cSMT_ATJ_RenderJob_Maxwell
            Try
                Dim out As cSMT_ATJ_RenderJob_Maxwell = Nothing

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
                    out = cSMT_ATJ_RenderJob_Maxwell.FromSDB(al)
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetJobByAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        ''' <summary>
        ''' Saves the referenced job to SDB and returns the JobId.
        ''' If the job already exists it is overwritten.
        ''' </summary>
        ''' <param name="Job"></param>
        ''' <returns>The JobId.</returns>
        ''' <remarks></remarks>
        Public Function SaveJob(ByRef Job As cSMT_ATJ_RenderJob_Maxwell) As String
            Try
                If String.IsNullOrEmpty(Job.Id) Then Job.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(Job.ToSDB(), Job.Id, sdb_domain_jobs)
                Return Job.Id
            Catch ex As Exception
                Throw New Exception("Problem with SaveJob(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub SetJobStatus(ByVal JobId As String, ByVal Status As eSMT_ATJ_RenderJob_Status)
            SetJobAttribute(JobId, "_Status", Status + 100000)

            Select Case Status
                Case eSMT_ATJ_RenderJob_Status.ARCHIVED, eSMT_ATJ_RenderJob_Status.COMPLETED, eSMT_ATJ_RenderJob_Status.RENDERING, eSMT_ATJ_RenderJob_Status.PREPROCESSING, eSMT_ATJ_RenderJob_Status.POSTPROCESSING
                    AddJobProgressMessage(New cSMT_ATJ_JobProgressMessage(JobId, "Status = " & Status.ToString, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.Status_major))
                Case Else
                    AddJobProgressMessage(New cSMT_ATJ_JobProgressMessage(JobId, "Status = " & Status.ToString, cSMT_ATJ_JobProgressMessage.eJobProgressMessageType.Status_minor))
            End Select

            Select Case Status
                Case eSMT_ATJ_RenderJob_Status.COMPLETED
                    SetJobAttribute(JobId, "Completed_Ticks", DateTime.UtcNow.Ticks)
            End Select

            'if the user wants emails notifying them of progress and/or problems do it here

        End Sub

        Public Function GetJobStatus(ByVal JobId As String) As eSMT_ATJ_RenderJob_Status
            Return GetJobAttribute(JobId, "_Status") - 100000
        End Function

        Public Sub SetJobAttribute(ByVal JobId As String, ByVal Name As String, ByVal Value As String)
            Try
                If String.IsNullOrEmpty(JobId) Then Throw New Exception("Invalid JobId.")
                SDBObject.SetItemAttribute(sdb_domain_jobs, JobId, Name, Value)
            Catch ex As Exception
                Throw New Exception("Problem with SetJobAttribute(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Sub DeleteJobAttribute(ByVal JobId As String, ByVal Name As String)
            Try
                If String.IsNullOrEmpty(JobId) Then Throw New Exception("Invalid JobId.")
                Dim a As New Attribute
                a.Name = Name
                SDBObject.DeleteAttributes(JobId, sdb_domain_jobs, ObjectToList(a))
            Catch ex As Exception
                Throw New Exception("Problem with DeleteJobAttribute(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetJobAttribute(ByVal JobId As String, ByVal Name As String) As String
            Try
                If String.IsNullOrEmpty(JobId) Then Throw New Exception("Invalid JobId.")
                Return SDBObject.GetItemAttribute(sdb_domain_jobs, JobId, Name)
            Catch ex As Exception
                Throw New Exception("Problem with GetJobAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public ReadOnly Property JobExists(ByVal JobId As String) As Boolean
            Get
                Try
                    Dim Q As New cSMT_AWS_SDB_Query
                    Dim P As New cSMT_AWS_SDB_QueryPredicate()
                    Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("JobId", JobId, eComparisonOperator.EqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                    Q.Predicates.Add(p_kvp)
                    Dim QR As QueryResult = SDBObject.Query(sdb_domain_jobs, Q.ToString)
                    Return (QR.ItemName IsNot Nothing) AndAlso QR.ItemName.Count > 0
                Catch ex As Exception
                    Throw New Exception("Problem with JobIdExists(). Error: " & ex.Message, ex)
                End Try
            End Get
        End Property

        Public Sub SetS3ObjectKeyGuidsToJobFileNames(ByVal JobId As String, ByVal data As Dictionary(Of String, String))
            Try
                Dim j As cSMT_ATJ_RenderJob_Maxwell = GetJob(JobId)
                If j.JobFileNames Is Nothing OrElse j.JobFileNames.Length < 1 Then Exit Sub 'this should never happen but do we want an exception?
                ReDim j.JobFileGuids(j.JobFileNames.Length - 1)
                For b As Byte = 0 To UBound(j.JobFileNames)
                    j.JobFileGuids(b) = data.Item(j.JobFileNames(b))
                Next
                SaveJob(j)
            Catch ex As Exception
                Throw New Exception("Problem with SetS3ObjectKeyGuidsToJobFileNames(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Sub SetJobCharges(ByVal JobId As String, ByVal Charges As Double)
            SetJobAttribute(JobId, "Charges", CStr(Charges))
        End Sub

        Public Function GetActiveJobsCount(ByVal JIQ As cSMT_ATJ_JobInfoQuery) As UInt32
            Try
                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                'use date filter
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Submitted_Ticks", JIQ.StartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.and)
                C = New cSMT_AWS_SDB_QueryAttributeCompare("Submitted_Ticks", JIQ.EndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)
                Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                pos += 1

                'use status filter
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("_Status", JIQ.StatusMin.ToString.PadLeft(6, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.and)
                C = New cSMT_AWS_SDB_QueryAttributeCompare("_Status", JIQ.StatusMax.ToString.PadLeft(6, "0"), eComparisonOperator.LessThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)

                Dim q_str As String = Q.ToString
                Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_jobs, q_str)

                Return QR.ItemName.Count
            Catch ex As Exception
                Throw New Exception("Problem in GetActiveJobsCount(). Error: " & ex.Message, ex)
            End Try
        End Function

#Region "JOBS:HELPER"

        Public Function CreateSampleMaxwellJob() As cSMT_ATJ_RenderJob_Maxwell
            Try
                Dim out As New cSMT_ATJ_RenderJob_Maxwell
                With out
                    .AnimationFrames = 0
                    .ActiveCamera = ""
                    .CameraNames = ""
                    .Channels_Alpha = True
                    .Channels_Alpha_Opaque = True
                    .Channels_MaterialId = False
                    .Channels_Render = True
                    .Channels_Shadow = False
                    .Channels_ZBuffer = False
                    .Channels_ZBuffer_min = 0
                    .Channels_ZBuffer_max = 0
                    .CreateMultilight = True
                    .EmailSamples = True
                    .MasterInstanceId = 1
                    .RenderNodeInstanceIds = New String() {0, 0}
                    .Name = "SampleMaxwellJob"
                    .MaxCost = 500
                    .MaxDuration = 360
                    .RenderTextures = True
                    .SampleCount = 25
                    .UserId = "Sequoyan"
                    '.MXSFileName = "VaseDiamond.mxs"
                    '.PathFileNames = Nothing 'New String() {"hi one", "hi two", "hi three"}
                End With
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with CreateSampleJob(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'JOBS:HELPER

#End Region 'JOBS DOMAIN

#Region "JOB PROGRESS DOMAIN"

        Public Sub AddJobProgressMessage(ByRef Msg As cSMT_ATJ_JobProgressMessage)
            Try
                If String.IsNullOrEmpty(Msg.Id) Then Msg.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(Msg.ToSDB(), Msg.Id, sdb_domain_job_progress)
                If Msg.IsSampleLevelNotice Then
                    SetJobAttribute(Msg.JobId, "CurrentSampleLevel", Msg.SampleLevel)
                    SetJobAttribute(Msg.JobId, "CurrentBenchmark", Msg.Benchmark)
                End If
            Catch ex As Exception
                Throw New Exception("Problem with AddJobProgressMessage(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetJobProgress(ByRef JobId As String, Optional ByVal SinceTicks As String = "-1", Optional ByVal MessageType As cSMT_ATJ_JobProgressMessage.eJobProgressMessageType = 0, Optional ByVal Comparison As eComparisonOperator = eComparisonOperator.EqualTo) As List(Of cSMT_ATJ_JobProgressMessage)
            Try
                Dim out As New List(Of cSMT_ATJ_JobProgressMessage)

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As cSMT_AWS_SDB_QueryPredicate
                Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                'ADD JOBID TO QUERY
                P = New cSMT_AWS_SDB_QueryPredicate
                C = New cSMT_AWS_SDB_QueryAttributeCompare("JobId", JobId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                Q.Predicates.Add(p_kvp)
                pos += 1

                'Q.Predicates.Add(p_kvp)
                'Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                'pos += 1

                ''ONLY GET PROGRESS MESSAGES WITH TYPE GREATER THAN 0 (major messages)
                'P = New cSMT_AWS_SDB_QueryPredicate
                'C = New cSMT_AWS_SDB_QueryAttributeCompare("_Type", "0", eComparisonOperator.GreaterThan)
                'P.Add(C, eAndOr.NOT_INDICATED)
                'p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                'Q.Predicates.Add(p_kvp)
                'pos += 1

                'ADD SINCETICKS TO QUERY
                If SinceTicks <> "" AndAlso CLng(SinceTicks) <> -1 Then

                    Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                    pos += 1

                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("Timestamp", SinceTicks, eComparisonOperator.GreaterThanOrEqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    pos += 1
                    Q.Predicates.Add(p_kvp)

                End If

                If MessageType > -32778 Then

                    Q.SetOperators.Add(New KeyValuePair(Of Byte, eSetOperator)(pos, eSetOperator.intersection))
                    pos += 1

                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("_Type", MessageType, Comparison)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                    pos += 1
                    Q.Predicates.Add(p_kvp)

                End If

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_job_progress, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_job_progress)
                    out.Add(cSMT_ATJ_JobProgressMessage.FromSDB(al))
                Next

                out.Sort(New cSMT_ATJ_JobProgressMessage.cSMT_ATJ_JobProgressMessage_Sorter)

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetJobProgress(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'JOB PROGRESS DOMAIN

#Region "FILE TRANSFER QUEUE"

        Private ReadOnly Property SQSObject() As cSMT_AWS_SQS
            Get
                If _SQSObject Is Nothing Then _SQSObject = New cSMT_AWS_SQS(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SQSObject
            End Get
        End Property
        Private _SQSObject As cSMT_AWS_SQS

        Public Sub Enqueue_JobNeedingFileTransferToS3(ByVal JobId As String)
            Dim msgId As String = SQSObject.SendMessage(sqs_Q_copy_job_files_to_s3, JobId)
        End Sub

        Public Function Pop_JobNeedingFileTransferToS3() As String
            Dim l As List(Of Amazon.SQS.Model.Message) = SQSObject.ReceiveMessages(sqs_Q_copy_job_files_to_s3, 1, 10)
            If l IsNot Nothing AndAlso l.Count > 0 Then
                SQSObject.DeleteMessage(sqs_Q_copy_job_files_to_s3, l.Item(0).ReceiptHandle)
                Return cSMT_AWS_SQS.DecodeString(l.Item(0).Body)
            Else
                Return Nothing
            End If
        End Function

#End Region 'FILE TRANSFER QUEUE

    End Module

End Namespace
