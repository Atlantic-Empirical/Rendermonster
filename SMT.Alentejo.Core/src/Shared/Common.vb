Public Module Common

    Public Enum eRenderEngine
        Maxwell
        VRay
        MentalRay
    End Enum

    Public Sub WriteEvent(ByVal sEntry As String, ByVal sAppName As String, ByVal eEventType As EventLogEntryType, ByVal Log As eSystemLog)
        Dim oEventLog As New EventLog
        Try
            'Register the Application as an Event Source
            If Not EventLog.SourceExists(sAppName) Then
                EventLog.CreateEventSource(sAppName, Log.ToString)
            End If

            'log the entry
            oEventLog.Source = sAppName
            oEventLog.WriteEntry(sEntry, eEventType)
        Catch Ex As Exception
            Debug.WriteLine("Failed WriteEvent().")
        End Try
    End Sub

    Public Enum eSystemLog
        System
        Application
    End Enum

    Public Enum eSMT_ATJ_DateRange
        All
        Past_7_Days
        Past_30_Days
        Past_90_Days
        Past_365_Days
        Last_Week
        Last_Month
        Last_Year
        This_Week
        This_Month
        This_Quarter
        This_Year
        Current 'exact meaning will be determined on a case by case basis.
    End Enum

End Module
