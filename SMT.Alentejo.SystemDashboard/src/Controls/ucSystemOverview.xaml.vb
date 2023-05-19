Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.InstanceManagement
Imports SMT.Alentejo.Core.Credit
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core
Imports SMT.Alentejo.Core.SessionManagement

Partial Public Class ucSystemOverview

    Private AlreadyLoaded As Boolean = False

#Region "ON LOAD"

    Private Sub ucSystemOverview_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        If AlreadyLoaded Then Exit Sub
        SetMeUp()
        AlreadyLoaded = True
    End Sub

    Private Sub SetMeUp()
        Try
            Cursor = Cursors.Wait
            SetupJobInfo()
            SetupUserInfo()
            SetupInstanceInfo()
            SetupFinanceInfo()
        Catch ex As Exception
            MsgBox("Problem with setup of overview. Error: " & ex.Message, MsgBoxStyle.OkOnly)
        Finally
            Cursor = Cursors.Arrow
        End Try
    End Sub

#End Region 'ON LOAD

#Region "JOBS"

    Private Sub SetupJobInfo()
        Dim JIQ As cSMT_ATJ_JobInfoQuery

        ' ALL JOBS
        JIQ = New cSMT_ATJ_JobInfoQuery()
        JIQ.StatusFilter = cSMT_ATJ_JobInfoQuery.eStatus.All
        tbJBS_JobsInSystem.Text = GetActiveJobsCount(JIQ)

        ' ACTIVE JOBS
        JIQ = New cSMT_ATJ_JobInfoQuery()
        JIQ.StatusFilter = cSMT_ATJ_JobInfoQuery.eStatus.Active
        tbJBS_JobsActive.Text = GetActiveJobsCount(JIQ)

        ' COMPLETED JOBS - 7 days
        JIQ = New cSMT_ATJ_JobInfoQuery()
        JIQ.StatusFilter = cSMT_ATJ_JobInfoQuery.eStatus.Completed
        JIQ.DateRange = eSMT_ATJ_DateRange.Past_7_Days
        tbJBS_JobsCompleted_7days.Text = GetActiveJobsCount(JIQ)

        ' COMPLETED JOBS - 30 days
        JIQ = New cSMT_ATJ_JobInfoQuery()
        JIQ.StatusFilter = cSMT_ATJ_JobInfoQuery.eStatus.Completed
        JIQ.DateRange = eSMT_ATJ_DateRange.Past_30_Days
        tbJBS_JobsCompleted_30days.Text = GetActiveJobsCount(JIQ)

        ' COMPLETED JOBS - Quarter
        JIQ = New cSMT_ATJ_JobInfoQuery()
        JIQ.StatusFilter = cSMT_ATJ_JobInfoQuery.eStatus.Completed
        JIQ.DateRange = eSMT_ATJ_DateRange.This_Quarter
        tbJBS_JobsCompleted_Quarter.Text = GetActiveJobsCount(JIQ)

        ' COMPLETED JOBS - Year
        JIQ = New cSMT_ATJ_JobInfoQuery()
        JIQ.StatusFilter = cSMT_ATJ_JobInfoQuery.eStatus.Completed
        JIQ.DateRange = eSMT_ATJ_DateRange.This_Year
        tbJBS_JobsCompleted_YTD.Text = GetActiveJobsCount(JIQ)

        ' FILES
        tbJBS_FilesInSystem.Text = GetFileCount()
    End Sub

#End Region 'JOBS

#Region "USERS"

    Private Sub SetupUserInfo()
        ' USERS REGISTERED
        tbUSR_UsersRegistered.Text = GetUsersCount(Nothing)

        ' USERS ACTIVE IN PAST 7 DAYS
        Dim UIQ As cSMT_ATJ_UserInfoQuery
        UIQ = New cSMT_ATJ_UserInfoQuery
        UIQ.ActiveDateRange = eSMT_ATJ_DateRange.Past_7_Days
        tbUSR_UsersActive_7days.Text = GetUsersCount(UIQ)

        ' SESSIONS - ACTIVE
        Dim SIQ As cSMT_ATJ_SessionInfoQuery
        SIQ = New cSMT_ATJ_SessionInfoQuery
        SIQ.LoginDateRange = eSMT_ATJ_DateRange.Current
        tbUSR_Sessions_Current.Text = GetSessionCount(SIQ)

        ' SESSIONS - 7 DAYS
        SIQ = New cSMT_ATJ_SessionInfoQuery
        SIQ.LoginDateRange = eSMT_ATJ_DateRange.Past_7_Days
        tbUSR_Sessions_7days.Text = GetSessionCount(SIQ)

        ' SESSIONS - 30 DAYS
        SIQ = New cSMT_ATJ_SessionInfoQuery
        SIQ.LoginDateRange = eSMT_ATJ_DateRange.Past_30_Days
        tbUSR_Sessions_30days.Text = GetSessionCount(SIQ)

        ' SESSIONS - QUARTER
        SIQ = New cSMT_ATJ_SessionInfoQuery
        SIQ.LoginDateRange = eSMT_ATJ_DateRange.This_Quarter
        tbUSR_Sessions_Quarter.Text = GetSessionCount(SIQ)

        ' SESSIONS - YTD
        SIQ = New cSMT_ATJ_SessionInfoQuery
        SIQ.LoginDateRange = eSMT_ATJ_DateRange.This_Year
        tbUSR_Sessions_YTD.Text = GetSessionCount(SIQ)
    End Sub

#End Region 'USERS

#Region "INSTANCES"

    Private Sub SetupInstanceInfo()
        Dim IIQ As cSMT_ATJ_InstanceInfoQuery

        ' INSTANCES ACTIVE - NOW
        IIQ = New cSMT_ATJ_InstanceInfoQuery
        IIQ.DateRange = eSMT_ATJ_DateRange.Current
        tbINS_InstancesActive_Now.Text = GetInstanceCount(IIQ)

        ' INSTANCES ACTIVE - 7 DAYS
        IIQ = New cSMT_ATJ_InstanceInfoQuery
        IIQ.DateRange = eSMT_ATJ_DateRange.Past_7_Days
        tbINS_InstancesActive_7days.Text = GetInstanceCount(IIQ)

        '' INSTANCES MAX - 7 DAYS
        'IIQ = New cSMT_ATJ_InstanceInfoQuery
        'IIQ.DateRange = eSMT_ATJ_DateRange.Past_7_Days
        'tbINS_InstancesMax_7days.Text = GetInstanceCount(IIQ)

        '' INSTANCES MAX - 30 DAYS
        'IIQ = New cSMT_ATJ_InstanceInfoQuery
        'IIQ.DateRange = eSMT_ATJ_DateRange.Past_30_Days
        'tbINS_InstancesMax_30days.Text = GetInstanceCount(IIQ)

        '' INSTANCES MAX - QUARTER
        'IIQ = New cSMT_ATJ_InstanceInfoQuery
        'IIQ.DateRange = eSMT_ATJ_DateRange.This_Quarter
        'tbINS_InstancesMax_Quarter.Text = GetInstanceCount(IIQ)

        '' INSTANCES MAX - YTD
        'IIQ = New cSMT_ATJ_InstanceInfoQuery
        'IIQ.DateRange = eSMT_ATJ_DateRange.This_Year
        '.Text = GetInstanceCount(IIQ)

    End Sub

#End Region 'INSTANCES

#Region "FINANCES"

    Private Sub SetupFinanceInfo()

    End Sub

#End Region 'FINANCES

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUpdate.Click
        SetMeUp()
    End Sub

End Class
