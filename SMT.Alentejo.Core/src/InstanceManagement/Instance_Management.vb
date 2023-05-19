Imports Amazon.EC2.Model
Imports Amazon.SimpleDB.Model
Imports Amazon.SQS.Model
Imports SMT.AWS.Authorization
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.AWS.EC2
Imports SMT.AWS.SDB
Imports SMT.AWS.SQS
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.AWS
Imports SMT.Alentejo.Core.AtjTrace

Namespace InstanceManagement

    Public Module Instance_Management

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

        Private ReadOnly Property EC2Object() As cSMT_AWS_EC2
            Get
                If _EC2Object Is Nothing Then _EC2Object = New cSMT_AWS_EC2(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _EC2Object
            End Get
        End Property
        Private _EC2Object As cSMT_AWS_EC2

#End Region 'FIELDS & PROPERTIES

#Region "INSTANCE QUEUING"

#Region "JOBS-NEEDING-INSTANCES QUEUE"

        Private ReadOnly Property SQSObject() As cSMT_AWS_SQS
            Get
                If _SQSObject Is Nothing Then _SQSObject = New cSMT_AWS_SQS(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SQSObject
            End Get
        End Property
        Private _SQSObject As cSMT_AWS_SQS

        Public Sub Enqueue_JobNeedingInstance(ByVal JobId As String)
            Dim msgId As String = SQSObject.SendMessage(sqs_Q_jobs_needing_instances, JobId)
        End Sub

        ''' <summary>
        ''' Returns JobId
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Pop_JobNeedingInstance() As String
            Dim l As List(Of Message) = SQSObject.ReceiveMessages(sqs_Q_jobs_needing_instances, 1, 10)
            If l IsNot Nothing AndAlso l.Count > 0 Then
                SQSObject.DeleteMessage(sqs_Q_jobs_needing_instances, l.Item(0).ReceiptHandle)
                Return cSMT_AWS_SQS.DecodeString(l.Item(0).Body)
            Else
                Return Nothing
            End If
        End Function

#End Region 'JOBS-NEEDING-INSTANCES QUEUE

#End Region 'INSTANCE QUEUING

#Region "INSTANCE STORAGE"

        Public Function GetInstances(Optional ByVal StatusEqualToAndAbove As eSMT_ATJ_Instance_Status = eSMT_ATJ_Instance_Status.NotAvailable_DoesNotExist) As List(Of cSMT_ATJ_Instance)
            Try
                Dim out As New List(Of cSMT_ATJ_Instance)

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Status", StatusEqualToAndAbove, eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_instances, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName
                    al = SDBObject.GetAttributes(item, sdb_domain_instances)
                    out.Add(cSMT_ATJ_Instance.FromSDB(al))
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetInstances(). Error: " & ex.Message, ex)
            End Try
        End Function

        ''' <summary>
        ''' Returns a list of all the EC2 instances that are currently active in the Alentejo group.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetGroupRunningInstances() As List(Of String)
            Try
                Dim out As New List(Of String)
                Dim Rs As List(Of Reservation) = EC2Object.DescribeInstances()
                For Each r As Reservation In Rs
                    For Each s As String In r.GroupName
                        If s = ec2_security_group Then
                            For Each i As RunningInstance In r.RunningInstance
                                If i.InstanceState.Code < 32 Then
                                    out.Add(r.RunningInstance(0).InstanceId)
                                End If
                            Next
                        End If
                    Next
                Next
                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetGroupRunningInstances(). Error: " & ex.Message, ex)
            End Try
        End Function

        ''' <summary>
        ''' Returns an instance based on its Id.
        ''' </summary>
        ''' <param name="InstanceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetInstance(ByVal InstanceId As String) As cSMT_ATJ_Instance
            Try
                Dim out As cSMT_ATJ_Instance = Nothing

                Dim Q As New cSMT_AWS_SDB_Query

                Dim pos As Byte = 0

                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Id", InstanceId, eComparisonOperator.EqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(pos, P)
                pos += 1
                Q.Predicates.Add(p_kvp)

                Dim q_str As String = Q.ToString
                'Debug.WriteLine(q_str)

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_instances, q_str)

                Dim al As List(Of Amazon.SimpleDB.Model.Attribute)
                For Each item As String In QR.ItemName

                    al = SDBObject.GetAttributes(item, sdb_domain_instances)
                    out = cSMT_ATJ_Instance.FromSDB(al)
                Next

                Return out
            Catch ex As Exception
                Throw New Exception("Problem with GetInstance(). Error: " & ex.Message, ex)
            End Try
        End Function

        ''' <summary>
        ''' Saves the referenced instance to SDB and returns the InstanceId.
        ''' If the Instance already exists it is overwritten.
        ''' </summary>
        ''' <param name="Instance"></param>
        ''' <returns>The InstanceId.</returns>
        ''' <remarks></remarks>
        Public Function SaveInstance(ByRef Instance As cSMT_ATJ_Instance) As String
            Try
                If String.IsNullOrEmpty(Instance.Id) Then Instance.Id = Guid.NewGuid.ToString
                SDBObject.PutAttributes(Instance.ToSDB(), Instance.Id, sdb_domain_instances)
                Return Instance.Id
            Catch ex As Exception
                Throw New Exception("Problem with SaveInstance(). Error: " & ex.Message, ex)
            End Try
        End Function

        ''' <summary>
        ''' This method is meant to take the id of a running EC2 instance (render master or node)
        ''' that is not in the instances-domain and package it and add it.
        ''' Throwing it back in the mix essentially.
        ''' </summary>
        ''' <param name="InstanceId"></param>
        ''' <remarks></remarks>
        Public Sub SaveRunningInstanceToDomain(ByVal InstanceId As String)
            Try
                Dim r As List(Of Reservation) = EC2Object.DescribeInstances(InstanceId)
                If r Is Nothing Then Throw New Exception("Unexpected.")
                If r.Count = 0 Then Throw New Exception("Unexpected.")
                If r(0).RunningInstance.Count = 0 Then Throw New Exception("Unexpected.")
                If r(0).RunningInstance(0) Is Nothing Then Throw New Exception("Unexpected.")

                Dim tI As New cSMT_ATJ_Instance

                tI.AmiId = r(0).RunningInstance(0).ImageId
                tI.AvailabilityZone = r(0).RunningInstance(0).Placement.AvailabilityZone
                tI.PublicHostname = r(0).RunningInstance(0).PublicDnsName
                tI.Id = r(0).RunningInstance(0).InstanceId
                tI.InstanceType = r(0).RunningInstance(0).InstanceType
                tI.LaunchTime = r(0).RunningInstance(0).LaunchTime
                tI.LaunchTime_UTCTicks = DateFrom_ISO8601(tI.LaunchTime).Ticks.ToString.PadLeft(20, "0")
                tI.Status = eSMT_ATJ_Instance_Status.Available

                'tI.DnsName_Private = "" 'not available until the instance reaches running state.
                'tI.ExternalIp = "" 'TODO: assign an ElasticIp
                'tI.InternalIp = "" 'not available until the instance is running
                'tI.KernelId = ri.KernelId 'not using currently
                'tI.ProductCodes = ri.ProductCode.Count 'not using currently
                'tI.RamDiskId = ri.RamdiskId 'not using currently

                SaveInstance(tI)

            Catch ex As Exception
                Throw New Exception("Problem with SaveRunningInstanceToDomain(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Sub SetInstanceStatus(ByVal InstanceId As String, ByVal Status As eSMT_ATJ_Instance_Status)
            SetInstanceAttribute(InstanceId, "Status", Status)
        End Sub

        Public Sub SetInstanceAttribute(ByVal InstanceId As String, ByVal Name As String, ByVal Value As String)
            Try
                If String.IsNullOrEmpty(InstanceId) Then Throw New Exception("Invalid InstanceId.")
                If String.IsNullOrEmpty(Value) Then
                    Dim a As New Amazon.SimpleDB.Model.Attribute
                    a.Name = Name
                    SDBObject.DeleteAttributes(InstanceId, sdb_domain_instances, ObjectToList(a))
                Else
                    SDBObject.SetItemAttribute(sdb_domain_instances, InstanceId, Name, Value)
                End If
            Catch ex As Exception
                Throw New Exception("Problem with SetInstanceAttribute(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public Function GetInstanceAttribute(ByVal InstanceId As String, ByVal AttributeName As String) As String
            Try
                If String.IsNullOrEmpty(InstanceId) Then Throw New Exception("Invalid InstanceId.")
                Return SDBObject.GetItemAttribute(sdb_domain_instances, InstanceId, AttributeName)
            Catch ex As Exception
                Throw New Exception("Problem with GetInstanceAttribute(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public ReadOnly Property RenderInstancesAvailable() As Byte
            Get
                Try
                    Dim Q As New cSMT_AWS_SDB_Query
                    Dim P As New cSMT_AWS_SDB_QueryPredicate()
                    Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Status", eSMT_ATJ_Instance_Status.Available, eComparisonOperator.EqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                    Q.Predicates.Add(p_kvp)
                    Dim QR As QueryResult = SDBObject.Query(sdb_domain_instances, Q.ToString)
                    If QR Is Nothing OrElse QR.ItemName Is Nothing Then Return 0
                    Return QR.ItemName.Count
                Catch ex As Exception
                    SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
                End Try
            End Get
        End Property

        ''' <summary>
        ''' Assigns instances to a Job and returns the difference between the number requested and the number
        ''' successfully assigned.
        ''' </summary>
        ''' <param name="JobId"></param>
        ''' <param name="RequestedInstanceCount"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function AssignInstancesToJob(ByVal JobId As String, ByVal RequestedInstanceCount As Byte, ByVal SetFirstAsMaster As Boolean, ByRef outInstanceIds As List(Of String)) As Byte
            Try
                If String.IsNullOrEmpty(JobId) Then Throw New Exception("Invalid JobId.")

                Dim Q As New cSMT_AWS_SDB_Query
                Dim P As New cSMT_AWS_SDB_QueryPredicate()
                Dim C As New cSMT_AWS_SDB_QueryAttributeCompare("Status", eSMT_ATJ_Instance_Status.Available, eComparisonOperator.GreaterThanOrEqualTo)
                P.Add(C, eAndOr.NOT_INDICATED)
                Dim p_kvp As New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                Q.Predicates.Add(p_kvp)
                Dim QR As QueryResult = SDBObject.Query(sdb_domain_instances, Q.ToString)
                If QR Is Nothing OrElse QR.ItemName Is Nothing Then Return RequestedInstanceCount

                Dim a As ReplaceableAttribute
                Dim cntInstancesAssigned As Byte = 0
                Dim RenderNodeIds As New List(Of String)

                'NEW WAY
                Dim MasterAssigned As Boolean = False
                For Each InstanceId As String In QR.ItemName

                    ' VERIFY THAT THE INSTANCE IS ACTUALLY RUNNING!
                    If InstanceIsRunning(InstanceId) Then

                        outInstanceIds.Add(InstanceId)

                        a = New ReplaceableAttribute
                        a.Name = "AssignedToJobId"
                        a.Value = JobId
                        a.Replace = True
                        SDBObject.PutAttributes(ObjectToList(a), InstanceId, sdb_domain_instances)

                        a = New ReplaceableAttribute
                        a.Name = "Status"
                        a.Value = eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob
                        a.Replace = True
                        SDBObject.PutAttributes(ObjectToList(a), InstanceId, sdb_domain_instances)

                        If Not MasterAssigned And SetFirstAsMaster Then
                            a = New ReplaceableAttribute
                            a.Name = "IsMaster"
                            a.Value = "True"
                            a.Replace = True
                            SDBObject.PutAttributes(ObjectToList(a), InstanceId, sdb_domain_instances)
                            SetJobAttribute(JobId, "MasterInstanceId", InstanceId)
                            MasterAssigned = True
                        Else
                            a = New ReplaceableAttribute
                            a.Name = "IsMaster"
                            a.Value = "False"
                            a.Replace = True
                            SDBObject.PutAttributes(ObjectToList(a), InstanceId, sdb_domain_instances)
                            RenderNodeIds.Add(InstanceId)
                        End If
                        cntInstancesAssigned += 1

                        ' BAIL OUT IF WE'VE ASSIGNED ALL THAT WERE REQUESTED
                        If cntInstancesAssigned = RequestedInstanceCount Then Exit For

                    Else
                        SetInstanceStatus(InstanceId, eSMT_ATJ_Instance_Status.NotAvailable_Terminated)
                    End If

                Next

                'OLD WAY, WORKS
                'For i As Byte = 0 To Math.Min(QR.ItemName.Count - 1, RequestedInstanceCount - 1)

                '    ' VERIFY THAT THE INSTANCE IS ACTUALLY RUNNING!
                '    If InstanceIsRunning(QR.ItemName(i)) Then

                '        outInstanceIds.Add(QR.ItemName(i))

                '        a = New ReplaceableAttribute
                '        a.Name = "AssignedToJobId"
                '        a.Value = JobId
                '        a.Replace = True
                '        SDBObject.PutAttributes(ObjectToList(a), QR.ItemName(i), sdb_domain_instances)

                '        a = New ReplaceableAttribute
                '        a.Name = "Status"
                '        a.Value = eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob
                '        a.Replace = True
                '        SDBObject.PutAttributes(ObjectToList(a), QR.ItemName(i), sdb_domain_instances)

                '        If i = 0 And SetFirstAsMaster Then
                '            a = New ReplaceableAttribute
                '            a.Name = "IsMaster"
                '            a.Value = "True"
                '            a.Replace = True
                '            SDBObject.PutAttributes(ObjectToList(a), QR.ItemName(i), sdb_domain_instances)
                '            SetJobAttribute(JobId, "MasterInstanceId", QR.ItemName(i))
                '        Else
                '            a = New ReplaceableAttribute
                '            a.Name = "IsMaster"
                '            a.Value = "False"
                '            a.Replace = True
                '            SDBObject.PutAttributes(ObjectToList(a), QR.ItemName(i), sdb_domain_instances)
                '            RenderNodeIds.Add(QR.ItemName(i))
                '        End If
                '        cntInstancesAssigned += 1

                '    End If

                'Next

                'Store render node ids
                If RenderNodeIds.Count > 0 Then
                    Dim sb As New System.Text.StringBuilder
                    For b As Byte = 0 To RenderNodeIds.Count - 1
                        If b = 0 Then
                            sb.Append(RenderNodeIds(b))
                        Else
                            sb.Append("," & RenderNodeIds(b))
                        End If
                    Next
                    SetJobAttribute(JobId, "RenderNodeInstanceIds", sb.ToString)
                End If

                Return RequestedInstanceCount - cntInstancesAssigned
            Catch ex As Exception
                Throw New Exception("Problem with AssignInstanceToJob(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub RemoveInstance(ByVal InstanceId As String)
            Try
                SDBObject.DeleteAttributes(InstanceId, sdb_domain_instances)
            Catch ex As Exception
                Throw New Exception("Problem with RemoveInstance(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public ReadOnly Property InstanceIsDueForTermination(ByVal InstanceId As String) As Boolean
            Get
                Dim LaunchTime_UTCTicks As String = GetInstanceAttribute(InstanceId, "LaunchTime_UTCTicks")
                Return DoesTimeFail(DateTime.UtcNow, LaunchTime_UTCTicks)
                'Dim runningTicks As Long = DateTime.UtcNow.Ticks - LaunchTime_UTCTicks
                'Dim minutesSinceStartup As Long = runningTicks / TimeSpan.TicksPerMinute
                'If minutesSinceStartup < 50 Then Return False 'short cut
                'For l As Long = 50 To (48 * 60) Step 60
                '    If minutesSinceStartup > l And minutesSinceStartup < l + 10 Then Return True
                'Next
                'If (runningTicks / TimeSpan.TicksPerHour) >= 48 Then 'bailout after 48 hours
                '    Return True
                'Else
                '    Return False
                'End If
            End Get
        End Property

        Public Function GetNextInstanceTerminationTime(ByVal InstanceId As String) As DateTime
            Dim LaunchTime_UTCTicks As String = GetInstanceAttribute(InstanceId, "LaunchTime_UTCTicks")
            Dim DT As DateTime = DateTime.UtcNow
            Dim cnt As Integer = 0
            While True
                If DoesTimeFail(DT, LaunchTime_UTCTicks) Then Return DT
                DT = DT.AddMinutes(1)
                cnt += 1
                If cnt >= (48 * 60) Then Return Nothing 'bailout
            End While
        End Function

        Private Function DoesTimeFail(ByRef DT As DateTime, ByRef LaunchTime_Ticks As Long) As Boolean
            Dim runningTicks As Long = DT.Ticks - LaunchTime_Ticks
            Dim minutesSinceStartup As Long = runningTicks / TimeSpan.TicksPerMinute
            If minutesSinceStartup < 50 Then Return False 'short cut
            For l As Long = 50 To (48 * 60) Step 60
                If minutesSinceStartup > l And minutesSinceStartup < l + 10 Then Return True
            Next
            If (runningTicks / TimeSpan.TicksPerHour) >= 48 Then 'bailout after 48 hours
                Return True
            Else
                Return False
            End If
        End Function

        Public Function GetInstanceCount(ByVal IIQ As cSMT_ATJ_InstanceInfoQuery) As UInt32
            Try
                Dim q_str As String = ""

                If IIQ IsNot Nothing Then
                    Dim Q As New cSMT_AWS_SDB_Query
                    Dim P As cSMT_AWS_SDB_QueryPredicate
                    Dim C As cSMT_AWS_SDB_QueryAttributeCompare
                    Dim p_kvp As KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)

                    'P = New cSMT_AWS_SDB_QueryPredicate
                    'C = New cSMT_AWS_SDB_QueryAttributeCompare("Status", IIQ.StatusMin, eComparisonOperator.GreaterThanOrEqualTo)
                    'P.Add(C, eAndOr.and)
                    'C = New cSMT_AWS_SDB_QueryAttributeCompare("Status", IIQ.StatusMax, eComparisonOperator.LessThanOrEqualTo)
                    'P.Add(C, eAndOr.NOT_INDICATED)
                    'p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                    'Q.Predicates.Add(p_kvp)
                    'q_str = Q.ToString

                    P = New cSMT_AWS_SDB_QueryPredicate
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("LaunchTime_UTCTicks", IIQ.StartDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.GreaterThanOrEqualTo)
                    P.Add(C, eAndOr.and)
                    C = New cSMT_AWS_SDB_QueryAttributeCompare("LaunchTime_UTCTicks", IIQ.EndDate.Ticks.ToString.PadLeft(20, "0"), eComparisonOperator.LessThanOrEqualTo)
                    P.Add(C, eAndOr.NOT_INDICATED)
                    p_kvp = New KeyValuePair(Of Byte, cSMT_AWS_SDB_QueryPredicate)(0, P)
                    Q.Predicates.Add(p_kvp)
                    q_str = Q.ToString

                End If

                Dim QR As QueryResult = SDBObject.Query(sdb_domain_instances, q_str)
                Return QR.ItemName.Count
            Catch ex As Exception
                Throw New Exception("Problem with GetInstanceCount(). Error: " & ex.Message, ex)
            End Try
        End Function

#End Region 'INSTANCE STORAGE

#Region "INSTANCE RUN-TERMINATE"

        ''' <summary>
        ''' Starts render instances.
        ''' Returns the difference between the number of instances requested and the number started successfully.
        ''' </summary>
        ''' <param name="JobId"></param>
        ''' <param name="DesiredInstanceCount"></param>
        ''' <param name="SetFirstAsMaster"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function RunRenderInstances(ByVal JobId As String, ByVal DesiredInstanceCount As Byte, ByVal SetFirstAsMaster As Boolean, ByRef outInstanceIds As List(Of String)) As Byte
            Try
                'SAFETY
                If DesiredInstanceCount > 6 Then DesiredInstanceCount = 1
                'SAFETY

                Dim R As Reservation

                If LOCAL_RENDERING Then
                    'MAKE A FAKE RESERVATION FOR USE IN LOCAL DEBUGGING
                    Dim RIx As New RunningInstance
                    RIx.ImageId = ec2_ami_render
                    RIx.Placement = New Placement
                    RIx.Placement.AvailabilityZone = "us-east-1b"
                    RIx.PublicDnsName = _PublicHostname
                    RIx.InstanceId = _InstanceId
                    RIx.InstanceType = ec2_render_instance_size
                    RIx.LaunchTime = DateTo_ISO8601()
                    R = New Reservation
                    R.RunningInstance = ObjectToList(RIx)
                Else
                    'Dim atjTRACE As New cSMT_ATJ_TRACE("Instance Dispatcher")
                    'atjTRACE.LogMessage(ec2_ami_render & " - " & ec2_user_data & " - " & ec2_security_group & " - " & ec2_render_instance_size & " - " & ec2_key_name & " - " & DesiredInstanceCount & " - " & DesiredInstanceCount & " - " & ec2_availability_zone, EventLogEntryType.Information)
                    R = EC2Object.RunInstances(ec2_ami_render, ec2_user_data, ec2_security_group, ec2_render_instance_size, ec2_key_name, DesiredInstanceCount, DesiredInstanceCount, ec2_availability_zone)
                End If

                Dim tI As cSMT_ATJ_Instance

                Dim pos As Byte = 0
                Dim RenderNodeIds As New List(Of String)
                For Each ri As RunningInstance In R.RunningInstance

                    outInstanceIds.Add(ri.InstanceId)

                    tI = New cSMT_ATJ_Instance

                    tI.AmiId = ri.ImageId
                    tI.AssignedToJobId = JobId
                    tI.AvailabilityZone = ri.Placement.AvailabilityZone
                    tI.Id = ri.InstanceId
                    tI.InstanceType = ri.InstanceType

                    If (pos = 0) And SetFirstAsMaster Then
                        tI.IsMaster = "True"
                        SetJobAttribute(JobId, "MasterInstanceId", ri.InstanceId)
                    Else
                        tI.IsMaster = "False"
                        RenderNodeIds.Add(ri.InstanceId)
                    End If

                    tI.LaunchTime = ri.LaunchTime
                    tI.LaunchTime_UTCTicks = DateFrom_ISO8601(tI.LaunchTime).Ticks.ToString.PadLeft(20, "0")
                    tI.Status = eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob

                    'tI.DnsName_Private = "" 'not available until the instance reaches running state.
                    'tI.ExternalIp = "" 'TODO: assign an ElasticIp
                    'tI.InternalIp = "" 'not available until the instance is running
                    'tI.KernelId = ri.KernelId 'not using currently
                    'tI.ProductCodes = ri.ProductCode.Count 'not using currently
                    'tI.RamDiskId = ri.RamdiskId 'not using currently

                    SaveInstance(tI)
                    pos += 1
                Next

                'Store render node ids
                If RenderNodeIds.Count > 0 Then
                    Dim sb As New System.Text.StringBuilder
                    For b As Byte = 0 To RenderNodeIds.Count - 1
                        If b = 0 Then
                            sb.Append(RenderNodeIds(b))
                        Else
                            sb.Append("," & RenderNodeIds(b))
                        End If
                    Next
                    SetJobAttribute(JobId, "RenderNodeInstanceIds", sb.ToString)
                End If

                Return DesiredInstanceCount - R.RunningInstance.Count

            Catch ex As Exception
                Throw New Exception("Problem with RunRenderInstances(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public ReadOnly Property InstanceIsRunning(ByVal InstanceId As String) As Boolean
            Get
                Dim r As List(Of Reservation)
                Try
                    r = EC2Object.DescribeInstances(InstanceId)
                Catch ex As Exception
                    Return False
                End Try
                If r Is Nothing Then Return False
                If r.Count = 0 Then Return False
                If r(0).RunningInstance.Count = 0 Then Return False
                Dim state As String = r(0).RunningInstance(0).InstanceState.Name.ToLower
                Return (state = "running") Or (state = "pending")
            End Get
        End Property

        Public Sub TerminateRenderInstance(ByVal InstanceId As String)
            Try
                EC2Object.TerminateInstances(ObjectToList(InstanceId))
            Catch ex As Exception
                Throw New Exception("Problem with TerminateRenderInstance(). Error: " & ex.Message, ex)
            End Try
        End Sub

        Public ReadOnly Property WindowsIsReady(ByVal InstanceId As String) As Boolean
            Get
                Try
                    Dim EC2 As New cSMT_AWS_EC2(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                    Dim consoleoutput As String = EC2.GetConsoleOutput(InstanceId)
                    If String.IsNullOrEmpty(consoleoutput) Then Return False
                    Return InStr(consoleoutput.ToLower, "windows is ready")
                Catch ex As Exception
                    Throw New Exception("Problem with WindowsIsReady(). Error: " & ex.Message, ex)
                End Try
            End Get
        End Property

#End Region 'INSTANCE RUN-TERMINATE

#Region "INSTANCE HEALTH"

        ''' <summary>
        ''' Returns true if the instance is running, the render manager app is responsive
        ''' and it is either rendering or not rendering.
        ''' </summary>
        ''' <param name="InstanceId"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsInstanceHealthy(ByVal InstanceId As String) As Boolean
            Dim al As List(Of Amazon.SimpleDB.Model.Attribute) = SDBObject.GetAttributes(InstanceId, sdb_domain_instances, ObjectToList("HealthPingTimeStamp"))
            If al Is Nothing Then Return False
            If al.Count <> 1 Then Return False
            If al.Item(0) Is Nothing Then Return False
            Dim LastHealthStamp As DateTime = DateFrom_ISO8601(al.Item(0).Value)
            Return DateDiff(DateTime.UtcNow, LastHealthStamp).TotalMinutes < 10
        End Function

        ''' <summary>
        ''' Called periodically by code on a running render instance to indicate that it is healthy.
        ''' </summary>
        ''' <param name="InstanceId"></param>
        ''' <remarks></remarks>
        Public Sub HealthPing(ByVal InstanceId As String)
            SDBObject.SetItemAttribute(sdb_domain_instances, InstanceId, "HealthPingTimeStamp", DateTo_ISO8601())
            'TODO: something with the two arguments here
        End Sub

#End Region 'INSTANCE HEALTH

    End Module

End Namespace
