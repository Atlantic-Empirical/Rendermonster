Imports Amazon.SimpleDB.Model
Imports SMT.AWS.SDB
Imports SMT.AWS

Namespace InstanceManagement

    Public Class cSMT_ATJ_Instance

        Public SCHEMA_VERSION As String = "001"

        Public Id As String 'EC2 Instance Id
        Public Status As String 'eSMT_ATJ_Instance_Status

        Public AmiId As String 'EC2 AMI Id
        Public InstanceType As String 'EC2 Instance type eInstanceType
        Public LaunchTime As String 'ISO 8601 DateTime
        Public LaunchTime_UTCTicks As String
        Public ScheduledTerminationTime As String 'ISO 8601 DateTime
        Public AssignedToJobId As String 'guid
        Public IsMaster As String 'bool

        'CORE EC2 FIELDS
        Public LocalIp As String
        Public PublicIp As String 'we're not going to set ElasticIP to render instances generally. Only for troubleshooting.
        Public LocalHostname As String
        Public PublicHostname As String 'store for use in troubleshooting 
        Public KernelId As String
        Public AvailabilityZone As String
        Public ProductCodes As String 'not going to use for now
        Public RamDiskId As String 'not going to use for now

        'HEALTH MONITORING
        Public HealthPingTimeStamp As String

        'RENDERING PROPERTIES
        Public Benchmark As String

        'PROPERTIES
        Public ReadOnly Property MinutesActive() As UInt16
            Get
                'TODO: calculate minutes active
            End Get
        End Property

        Public ReadOnly Property MinutesUntilTermination() As Byte
            Get

            End Get
        End Property

        Public Sub New()
            HealthPingTimeStamp = DateTo_ISO8601()
        End Sub

#Region "OVERRIDES"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Amazon.SimpleDB.Model.Attribute)) As cSMT_ATJ_Instance
            Dim p As New cSMT_ATJ_Instance
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_Instance)(Attributes, p)
        End Function

#End Region 'OVERRIDES

        Public Overrides Function ToString() As String
            Return Id & " - " & Status.ToString & " - " & InstanceType
        End Function

    End Class

    Public Enum eSMT_ATJ_Instance_Status As Short
        NotAvailable_DoesNotExist = -5
        NotAvailable_Terminated = -4
        NotAvailable_ShuttingDown = -3
        NotAvailable_PendingPeerNodeExit = -2
        NotAvailable_AssignedToJob = -1
        NOT_INDICATED = 0
        Available = 1
    End Enum

    Public Class cSMT_ATJ_InstanceInfoQuery

        Public ReadOnly Property StartDate() As DateTime
            Get
                Select Case DateRange
                    Case eSMT_ATJ_DateRange.Current
                        Return DateTime.UtcNow.Subtract(New TimeSpan(3, 0, 0))
                    Case eSMT_ATJ_DateRange.All
                        Return DateTime.MinValue
                    Case eSMT_ATJ_DateRange.Last_Month
                        Dim d As DateTime = DateTime.UtcNow.AddMonths(-1)
                        While d.Month = DateTime.UtcNow.Month
                            d = d.AddDays(-1)
                        End While
                        Return d.AddDays(1) 'this will be the first day of the previous month

                    Case eSMT_ATJ_DateRange.Last_Week
                        Dim d As DateTime = DateTime.UtcNow.AddDays(-7)
                        While d.DayOfWeek <> DayOfWeek.Monday
                            d = d.AddDays(-1)
                        End While
                        Return d 'this will be monday of the previous week

                    Case eSMT_ATJ_DateRange.Last_Year
                        Return New DateTime(DateTime.UtcNow.Year - 1, 1, 1)

                    Case eSMT_ATJ_DateRange.Past_30_Days
                        Return DateTime.UtcNow.AddDays(-30)

                    Case eSMT_ATJ_DateRange.Past_365_Days
                        Return DateTime.UtcNow.AddDays(-365)

                    Case eSMT_ATJ_DateRange.Past_7_Days
                        Return DateTime.UtcNow.AddDays(-7)

                    Case eSMT_ATJ_DateRange.Past_90_Days
                        Return DateTime.UtcNow.AddDays(-90)

                    Case eSMT_ATJ_DateRange.This_Month
                        Dim d As DateTime = DateTime.UtcNow
                        While d.Month = DateTime.UtcNow.Month
                            d = d.AddDays(-1)
                        End While
                        Return d.AddDays(1) 'this will be the first day of the this month

                    Case eSMT_ATJ_DateRange.This_Week
                        Dim d As DateTime = DateTime.UtcNow
                        While d.DayOfWeek <> DayOfWeek.Monday
                            d = d.AddDays(-1)
                        End While
                        Return d 'this will be monday of this week

                    Case eSMT_ATJ_DateRange.This_Quarter
                        Select Case DateTime.UtcNow.Month
                            Case 1, 2, 3
                                Return New DateTime(DateTime.UtcNow.Year, 1, 1)
                            Case 4, 5, 6
                                Return New DateTime(DateTime.UtcNow.Year, 4, 1)
                            Case 7, 8, 9
                                Return New DateTime(DateTime.UtcNow.Year, 7, 1)
                            Case 10, 11, 12
                                Return New DateTime(DateTime.UtcNow.Year, 10, 1)
                        End Select

                    Case eSMT_ATJ_DateRange.This_Year
                        Return New DateTime(DateTime.UtcNow.Year, 1, 1)

                End Select
            End Get
        End Property

        Public ReadOnly Property EndDate() As DateTime
            Get
                Select Case DateRange

                    Case eSMT_ATJ_DateRange.All
                        Return DateTime.MaxValue
                    Case eSMT_ATJ_DateRange.Last_Month
                        Dim d As DateTime = DateTime.UtcNow
                        While d.Month = DateTime.UtcNow.Month
                            d = d.AddDays(-1)
                        End While
                        Return d 'this will be the last day of the previous month

                    Case eSMT_ATJ_DateRange.Last_Week
                        Dim d As DateTime = DateTime.UtcNow
                        While d.DayOfWeek <> DayOfWeek.Sunday
                            d = d.AddDays(-1)
                        End While
                        Return d 'this will be the previous sunday

                    Case eSMT_ATJ_DateRange.Last_Year
                        Return New DateTime(DateTime.UtcNow.Year - 1, 12, 31)

                    Case Else 'eDateRange.Past_30_Days, eDateRange.Past_365_Days, eDateRange.Past_7_Days, eDateRange.Past_90_Days, eDateRange.This_Month, eDateRange.This_Week, eDateRange.This_Year
                        Return DateTime.UtcNow
                End Select
            End Get
        End Property

        Public ReadOnly Property StatusMin() As String
            Get
                Select Case StatusFilter
                    Case eStatus.Active
                        Return "-2"
                    Case Else
                        Return "-100000"
                End Select
            End Get
        End Property

        Public ReadOnly Property StatusMax() As UInt32
            Get
                Return "100000"
            End Get
        End Property

        Public DateRange As eSMT_ATJ_DateRange
        Public UserFilter As String
        Public StatusFilter As eStatus

        Public Enum eStatus
            All
            Active
            Completed
            Archived
        End Enum

    End Class

End Namespace
