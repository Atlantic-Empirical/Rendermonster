Imports System.IO
Imports SMT.AWS.SDB
Imports Amazon.SimpleDB.Model
Imports SMT.AWS

Namespace JobManagement

    Public MustInherit Class cSMT_ATJ_RenderJob_Base

        Public SCHEMA_VERSION As String = "001"

        Public Id As String                                 'GUID

        Public UserId As String                             'GUID

        Public Name As String                               'String

        Public JobFileNames() As String                     'String

        Public JobFileGuids() As String                     'GUID

        ''' <summary>
        ''' Guid for looking up the final output image in S3.
        ''' </summary>
        ''' <remarks></remarks>
        Public FinalImageId As String                       'GUID

        ''' <summary>
        ''' DO NOT SET DIRECTLY.
        ''' Maps to eSMT_ATJ_RenderJob_Status
        ''' </summary>
        ''' <remarks></remarks>
        Public _Status As String                           'Int16

        ''' <summary>
        ''' Accessor that applys a negative number offset to this value for storage in SDB.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status() As eSMT_ATJ_RenderJob_Status
            Get
                Return _Status - 100000
            End Get
            Set(ByVal value As eSMT_ATJ_RenderJob_Status)
                _Status = value + 100000
            End Set
        End Property                          'Int16

        Public ReadOnly Property Status_Public() As String
            Get
                Select Case CType(_Status, eSMT_ATJ_RenderJob_Status)
                    Case Is < 0 'FAILED
                        Return "FAILED"
                    Case eSMT_ATJ_RenderJob_Status.ARCHIVED
                        Return "ARCHIVED"
                    Case eSMT_ATJ_RenderJob_Status.AWAITING_TERMINATION
                        Return "TERMINATING"
                    Case eSMT_ATJ_RenderJob_Status.COMPLETED
                        Return "COMPLETED"
                    Case 100 To 199 'PRE PROCESSING
                        Return "PRE-PROCESSING"
                    Case eSMT_ATJ_RenderJob_Status.RENDERING
                        Return "RENDERING"
                    Case 300 To 399 'POST PROCESSING
                        Return "POST-PROCESSING"
                    Case Else
                        Return "NOT_INDICATED"
                End Select
            End Get
        End Property

        ''' <summary>
        ''' Stored as ticks, maps to UTC DateTime
        ''' </summary>
        ''' <remarks></remarks>
        Public Submitted_Ticks As String                    'UInt64

        ''' <summary>
        ''' Stored as ticks, maps to UTC DateTime
        ''' </summary>
        ''' <remarks></remarks>
        Public StartedRendering_Ticks As String             'UInt64

        ''' <summary>
        ''' Stored as ticks, maps to UTC DateTime
        ''' </summary>
        ''' <remarks></remarks>
        Public Completed_Ticks As String                    'UInt64

        Public CoresRequested As String                     'UInt16

        Public MasterInstanceId As String                   'GUID

        Public RenderNodeInstanceIds() As String            'GUID

        Public MaxCost As String                            'UInt16

        Public EmailSamples As String                       'Boolean

        Public Width As String                              'Uint16

        Public Height As String                             'UInt16

        Public OutputImageFormat As String = ".jpg"         'String - Maps to a file extension for a supported image file type.

        ''' <summary>
        ''' Set when user wants to terminate the job.
        ''' Cleared on attempted termination.
        ''' </summary>
        ''' <remarks></remarks>
        Public TerminateFlag As String                      'Boolean

        Public Charges As String                            'Double

        Public MustOverride Function ToSDB() As List(Of ReplaceableAttribute)

        'Public Sub AddJobFile(ByVal FileName As String)
        '    Dim l As List(Of String)
        '    If Not JobFileNames Is Nothing Then
        '        l = JobFileNames.ToList()
        '    Else
        '        l = New List(Of String)
        '    End If
        '    l.Add(FileName)
        'End Sub

    End Class

    Public Class cSMT_ATJ_RenderJob_Maxwell
        Inherits cSMT_ATJ_RenderJob_Base

#Region "FIELDS"

        Public MaxDuration As String                'UInt16
        Public SampleCount As String                'Byte
        Public SampleCount_PerNode As String        'Byte
        Public ActiveCamera As String               'String
        Public CameraNames As String                'String
        Public AnimationFrames As String            'String
        Public CreateMultilight As String           'Boolean
        Public RenderTextures As String             'Boolean
        Public Channels_Render As String            'Boolean
        Public Channels_Shadow As String            'Boolean
        Public Channels_Alpha As String             'Boolean
        Public Channels_Alpha_Opaque As String      'Boolean
        Public Channels_MaterialId As String        'Boolean
        Public Channels_ObjectId As String          'Boolean
        Public Channels_ZBuffer As String           'Boolean
        Public Channels_ZBuffer_min As String       'UInt64
        Public Channels_ZBuffer_max As String       'UInt64
        Public Vignetting As String                 'UInt16
        Public ScatteringLens As String             'UInt16
        Public UsePreviewEngine As String           'Boolean

        ''' <summary>
        ''' Guid for locating the output MXI file in S3.
        ''' </summary>
        ''' <remarks></remarks>
        Public MXIFileId As String               'GUID
        Public Option_MergeCooperativeLevels As String 'Boolean

#End Region 'FIELDS

#Region "CONSTRUCTOR"

        Public Sub New()
            Submitted_Ticks = DateTime.UtcNow.Ticks.ToString.PadLeft(20, "0")
        End Sub

#End Region 'CONSTRUCTOR

#Region "OVERRIDES"

        Public Overrides Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Attribute)) As cSMT_ATJ_RenderJob_Maxwell
            Dim p As New cSMT_ATJ_RenderJob_Maxwell
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_RenderJob_Maxwell)(Attributes, p)
        End Function

#End Region 'OVERRIDES

        Public Overrides Function ToString() As String
            Return Name & " - " & JobFileNames(0) & " - " & New DateTime(Submitted_Ticks, DateTimeKind.Utc).ToString("ddd, dd MMM yyyy HH:mm 'UTC'")
        End Function

    End Class

    Public Enum eSMT_ATJ_RenderJob_Status As Short

        Failure_RenderJobFinalization = -7
        Failure_RunMXCL = -6
        Failure_RenderNodeCoordination = -5
        Failure_FileTransferFromS3 = -4
        Failure_FileTransferToS3 = -3
        Failure_Generic = -2
        JOB_FAILED = -1

        NOT_INDICATED = 0

        PREPROCESSING = 100
        Preprocessing_HaveJobFiles_WaitingForProfile = 101
        Preprocessing_WaitingForJobFileTransferToS3 = 102
        Preprocessing_TransferringJobFilesToS3 = 103
        Preprocessing_WaitingForInstanceStartLogic = 104
        Preprocessing_InInstanceStartLogic = 105
        Preprocessing_WaitingForMasterRenderInstanceToComeOnline = 106
        Preprocessing_RetrievingJobFiles = 107
        Preprocessing_CheckingForRenderNodes = 108
        Preprocessing_WaitingForRenderNodesToComeOnline = 109
        Preprocessing_FinalJobPrepPreRendering = 110

        RENDERING = 200

        POSTPROCESSING = 300
        Postprocessing_InitialActivities = 301
        Postprocessing_TransferringRenderFilesToS3 = 302

        COMPLETED = 400

        ARCHIVED = 500

        AWAITING_TERMINATION = 900
    End Enum

    Public Class cSMT_ATJ_JobProgressMessage

        'REQUIRED VALUES
        Public ReadOnly Property Type() As eJobProgressMessageType
            Get
                Return CType(_Type, eJobProgressMessageType)
            End Get
        End Property
        Public _Type As String
        Public Id As String 'Maps to guid
        Public JobId As String 'Maps to guid
        Public Message As String
        Public Timestamp As String 'Maps to Int64

        'OPTIONAL VALUES
        Public ReadOnly Property IsRenderedImage() As Boolean
            Get
                Return Type = eJobProgressMessageType.NewRenderedImage
            End Get
        End Property
        Public RenderedImageGuid As String 'Maps to Guid

        Public ReadOnly Property IsSampleLevelNotice() As Boolean
            Get
                Return Type = eJobProgressMessageType.NewSampleLevel
            End Get
        End Property
        Public SampleLevel As String
        Public Benchmark As String

        Public Sub New()
        End Sub

        Public Sub New(ByRef nJobId As String, ByRef nMessage As String, ByRef nType As eJobProgressMessageType)
            JobId = nJobId
            Message = nMessage
            Timestamp = DateTime.UtcNow.Ticks
            _Type = nType
        End Sub

#Region "OVERRIDES"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Attribute)) As cSMT_ATJ_JobProgressMessage
            Dim p As New cSMT_ATJ_JobProgressMessage
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_JobProgressMessage)(Attributes, p)
        End Function

#End Region 'OVERRIDES

        Public Class cSMT_ATJ_JobProgressMessage_Sorter
            Implements IComparer(Of cSMT_ATJ_JobProgressMessage)

            Public Function Compare(ByVal X As cSMT_ATJ_JobProgressMessage, ByVal Y As cSMT_ATJ_JobProgressMessage) As Integer Implements IComparer(Of SMT.Alentejo.Core.JobManagement.cSMT_ATJ_JobProgressMessage).Compare
                Dim Msg1 As cSMT_ATJ_JobProgressMessage = DirectCast(X, cSMT_ATJ_JobProgressMessage)
                Dim Msg2 As cSMT_ATJ_JobProgressMessage = DirectCast(Y, cSMT_ATJ_JobProgressMessage)
                Dim l As Long = Msg1.Timestamp - Msg2.Timestamp
                If l < 0 Then Return -1
                If l = 0 Then Return 0
                If l > 0 Then Return 1
            End Function

        End Class

        Public Overrides Function ToString() As String
            Return Message
        End Function

        Public Enum eJobProgressMessageType
            NewRenderedImage = -2
            Status_minor = -1
            Status_major = 1
            NewSampleLevel = 2
            RenderEngineMessage = 3
        End Enum

    End Class

    Public Class cSMT_ATJ_LiteJobInfo

        Public Id As String
        Public Name As String
        Public _Status As String                           'Int16
        Public Status_Public As String
        Public Submitted_Ticks As String
        Public Completed_Ticks As String
        Public Charged As String

        ''' <summary>
        ''' Accessor that applys a negative number offset to this value for storage in SDB.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Property Status() As eSMT_ATJ_RenderJob_Status
            Get
                Return _Status - 100000
            End Get
            Set(ByVal value As eSMT_ATJ_RenderJob_Status)
                _Status = value + 100000
                Status_Public = GetPublicStatus(_Status)
            End Set
        End Property

        ''' <summary>
        ''' DO NOT SET DIRECTLY.
        ''' Maps to eSMT_ATJ_RenderJob_Status
        ''' </summary>
        ''' <remarks></remarks>
        Private Shared Function GetPublicStatus(ByVal _Status As Int16) As String
            Select Case CType(_Status, eSMT_ATJ_RenderJob_Status)
                Case Is < 0 'FAILED
                    Return "FAILED"
                Case eSMT_ATJ_RenderJob_Status.ARCHIVED
                    Return "ARCHIVED"
                Case eSMT_ATJ_RenderJob_Status.AWAITING_TERMINATION
                    Return "TERMINATING"
                Case eSMT_ATJ_RenderJob_Status.COMPLETED
                    Return "COMPLETED"
                Case 100 To 199 'PRE PROCESSING
                    Return "PRE-PROCESSING"
                Case eSMT_ATJ_RenderJob_Status.RENDERING
                    Return "RENDERING"
                Case 300 To 399 'POST PROCESSING
                    Return "POST-PROCESSING"
                Case Else
                    Return "NOT_INDICATED"
            End Select
        End Function

        Public Sub New()
        End Sub

        Public Sub New(ByVal nId As String, ByVal nName As String, ByVal nStatus As String, ByVal nCreated As DateTime, ByVal nCompleted As DateTime)
            Id = nId
            Name = nName
            Status = nStatus
            Submitted_Ticks = nCreated
            Completed_Ticks = nCompleted
        End Sub

        Public Sub New(ByRef J As cSMT_ATJ_RenderJob_Base)
            Id = J.Id
            Name = J.Name
            Status = J.Status
            Submitted_Ticks = J.Submitted_Ticks
            Completed_Ticks = J.Completed_Ticks
        End Sub

#Region "OVERRIDES"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Attribute)) As cSMT_ATJ_LiteJobInfo
            Dim p As New cSMT_ATJ_LiteJobInfo
            Dim out As cSMT_ATJ_LiteJobInfo = SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_LiteJobInfo)(Attributes, p)
            out.Status_Public = GetPublicStatus(out.Status)
            Return out
        End Function

#End Region 'OVERRIDES

    End Class

    Public Class cSMT_ATJ_JobInfoQuery

        Public Sub New()
        End Sub

        Public ReadOnly Property StartDate() As DateTime
            Get
                Select Case DateRange
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

        Public ReadOnly Property StatusMin() As UInt32
            Get
                Select Case StatusFilter
                    Case eStatus.All
                        Return Short.MinValue + 100000
                    Case eStatus.Active
                        Return 1 + 100000
                    Case eStatus.Completed
                        Return eSMT_ATJ_RenderJob_Status.COMPLETED + 100000
                    Case eStatus.Archived
                        Return eSMT_ATJ_RenderJob_Status.ARCHIVED + 100000
                End Select
            End Get
        End Property

        Public ReadOnly Property StatusMax() As UInt32
            Get
                Select Case StatusFilter
                    Case eStatus.All
                        Return Short.MaxValue + 100000
                    Case eStatus.Active
                        Return eSMT_ATJ_RenderJob_Status.COMPLETED - 1 + 100000
                    Case eStatus.Completed
                        Return eSMT_ATJ_RenderJob_Status.COMPLETED + 100000
                    Case eStatus.Archived
                        Return eSMT_ATJ_RenderJob_Status.ARCHIVED + 100000
                End Select
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
