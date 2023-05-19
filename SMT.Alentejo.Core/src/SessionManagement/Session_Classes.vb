Imports System.IO
Imports SMT.AWS.SDB
Imports Amazon.SimpleDB.Model
Imports SMT.AWS

Namespace SessionManagement

    Public Class cSMT_ATJ_Session

        Public Id As String
        Public UserId As String
        Public IpAddress As String
        Public LoginUTCTicks As String
        Public LogoutUTCTicks As String
        Public LastActivityUTCTicks As String

#Region "SDB"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Attribute)) As cSMT_ATJ_Session
            Dim p As New cSMT_ATJ_Session
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_Session)(Attributes, p)
        End Function

#End Region 'SDB

    End Class

    Public Class cSMT_ATJ_SessionInfoQuery

        Public LoginDateRange As eSMT_ATJ_DateRange

        Public ReadOnly Property LoginStartDate() As DateTime
            Get
                Select Case LoginDateRange
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

        Public ReadOnly Property LoginEndDate() As DateTime
            Get
                Select Case LoginDateRange
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
