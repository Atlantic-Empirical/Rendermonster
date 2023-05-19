Imports SMT.AWS.SDB
Imports Amazon.SimpleDB.Model
Imports SMT.AWS
Imports SMT.Alentejo.Core.Credit.Read

Namespace UserManagement

    Public Class cSMT_ATJ_User

#Region "FIELDS"

        Public SCHEMA_VERSION As String = "001"

        Public Id As String
        Public Created_UTCTicks As String
        Public LastLogin_UTCTicks As String

        'FINANCE
        Public Balance_Encrypted As String
        Public ReadOnly Property Balance_Decrypted() As Double
            Get
                Return DecodeBalance(Balance_Encrypted)
            End Get
        End Property

        ' CREDENTIALS
        Public Username As String
        Public PasswordHash As String

        ' LOCALITY
        ''' <summary>
        ''' Offset from UTC
        ''' </summary>
        ''' <remarks></remarks>
        Public Timezone As String
        Public LastLoginIpAddress As String

        ' IDENTITY
        Public FirstName As String
        Public LastName As String
        Public Company As String

        ' ADDRESS
        Public Address1 As String
        Public Address2 As String
        Public Address3 As String
        Public City As String
        Public State As String
        Public PostalCode As String
        Public Country As String

        ' CONTACT
        Public EmailAddress As String
        Public PhoneOffice As String
        Public PhoneMobile As String
        Public Website As String

        ' PAYMENT
        Public PayPalAccountEmailAddress As String

        ' PREFERENCES
        Public EmailNotificationLevel As String
        Public NumberOfCores As String
        Public RenderLevels As String
        Public MaxRenderingTime As String
        Public MaxCost As String
        Public OutputType As String
        ''' <summary>
        ''' Maps to eRenderEngine
        ''' </summary>
        ''' <remarks></remarks>
        Public RenderEngine As String

        ' ACCOUNT
        ''' <summary>
        ''' Maps to eAtjAccountStatus
        ''' </summary>
        ''' <remarks></remarks>
        Public AccountStatus As String
        ''' <summary>
        ''' Maps to eAtjAccountType
        ''' </summary>
        ''' <remarks></remarks>
        Public AccountType As String
        ''' <summary>
        ''' Maps to eAtjBillingType
        ''' </summary>
        ''' <remarks></remarks>
        Public BillingType As String

#End Region 'FIELDS

#Region "SDB SERIALIZATION"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Attribute)) As cSMT_ATJ_User
            Dim p As New cSMT_ATJ_User
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_User)(Attributes, p)
        End Function

#End Region 'SDB SERIALIZATION

    End Class

    Public Class cSMT_ATJ_User_Lite

        Public Id As String
        Public Username As String
        Public FirstName As String
        Public LastName As String
        Public Company As String

#Region "SDB SERIALIZATION"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Attribute)) As cSMT_ATJ_User_Lite
            Dim p As New cSMT_ATJ_User_Lite
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_User_Lite)(Attributes, p)
        End Function

#End Region 'SDB SERIALIZATION

    End Class

    Public Enum eAtjAccountStatus
        UNDEFINED = 0

        BARRED = 100
        UnVerified = 101
        BillingProblem_Barred = 103

        GOODSTANDING = 200
        GoodStanding_Limited = 201
        GoodStanding_Unlimited = 202

    End Enum

    Public Enum eAtjAccountType
        UNDEFINED = 0
        Trial = 1
        LiteUser = 2
        ProUser = 3
        IndustrialUser = 4
        Corporate = 5
        Admin = 100
    End Enum

    Public Enum eAtjBillingType
        UNDEFINED = 0

        Depository = 100

        INVOICED = 200
        Invoiced_Weekly = 201
        Invoiced_Biweekly = 202
        Invoiced_Monthly = 204
        Invoiced_Quarterly = 212

    End Enum

    Public Class cSMT_ATJ_UserInfoQuery

        Public Keyword As String
        Public ActiveDateRange As eSMT_ATJ_DateRange

        Public ReadOnly Property ActiveStartDate() As DateTime
            Get
                Select Case ActiveDateRange
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

        Public ReadOnly Property ActiveEndDate() As DateTime
            Get
                Select Case ActiveDateRange
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

    End Class

End Namespace
