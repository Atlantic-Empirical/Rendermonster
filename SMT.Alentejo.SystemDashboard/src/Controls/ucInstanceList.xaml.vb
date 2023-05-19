Imports SMT.Alentejo.Core.InstanceManagement
Imports System.Collections.ObjectModel
Imports SMT.AWS
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.UserManagement

Partial Public Class ucInstanceList

    Private Sub ucInstanceList_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
    End Sub

    Public Sub Sort_User(ByVal sender As Object, ByVal e As ContextMenuEventArgs)
        MsgBox("hi")
    End Sub

    Private Sub UpdateInstanceList()
        Try
            Dim SDBInstances As List(Of cSMT_ATJ_Instance) = GetInstances(eSMT_ATJ_Instance_Status.NotAvailable_AssignedToJob)
            Dim EC2Instances As List(Of String) = GetGroupRunningInstances()
            tbInstancesInEC2.Text = EC2Instances.Count
            tbInstancesInSDB.Text = SDBInstances.Count

            If EC2Instances.Count > SDBInstances.Count Then
                MsgBox("leaked instance is running.", MsgBoxStyle.Exclamation)
            End If

            lvInstances.ItemsSource = New cInstanceArray(SDBInstances.ToArray)
            lvInstances.View = GetGridView()

        Catch ex As Exception
            MsgBox("Problem with UpdateInstanceList(). Error: " & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub btnUpdateInstanceList_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUpdateInstanceList.Click
        UpdateInstanceList()
    End Sub

    Public Class cInstanceArray
        Inherits ObservableCollection(Of cSMT_ATJ_InstanceView)
        Public Sub New(ByRef Instances() As cSMT_ATJ_Instance)
            For Each i As cSMT_ATJ_Instance In Instances
                Add(New cSMT_ATJ_InstanceView(i))
            Next
        End Sub
    End Class

    Private Function GetGridView() As GridView
        Try
            Dim myGridView As New GridView()
            myGridView.AllowsColumnReorder = True
            myGridView.ColumnHeaderToolTip = "Employee Information"

            Dim tgvc As GridViewColumn

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Status")
            tgvc.Header = "Status"
            tgvc.Width = 40
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("Id")
            tgvc.Header = "Instance"
            tgvc.Width = 80
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("AmiId")
            tgvc.Header = "AMI"
            tgvc.Width = 80
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("InstanceType")
            tgvc.Header = "Type"
            tgvc.Width = 60
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("LaunchTime")
            tgvc.Header = "Launch"
            tgvc.Width = 110
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("ScheduledTermination")
            tgvc.Header = "Terminate"
            tgvc.Width = 110
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("ActiveMinutes")
            tgvc.Header = "Act."
            tgvc.Width = 40
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("DiesInMinutes")
            tgvc.Header = "Dies"
            tgvc.Width = 40
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("JobId")
            tgvc.Header = "Job"
            tgvc.Width = 60
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("UserName")
            tgvc.Header = "User"
            tgvc.Width = 60
            myGridView.Columns.Add(tgvc)

            tgvc = New GridViewColumn()
            tgvc.DisplayMemberBinding = New Binding("PublicHostname")
            tgvc.Header = "DNS"
            tgvc.Width = 200
            myGridView.Columns.Add(tgvc)

            Return myGridView
        Catch ex As Exception
            Throw New Exception("Problem with GetGridView(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Class cSMT_ATJ_InstanceView

        Private Instance As cSMT_ATJ_Instance

        Public Sub New(ByRef i As cSMT_ATJ_Instance)
            Instance = i
        End Sub

        Public ReadOnly Property Status() As String
            Get
                Return Instance.Status
            End Get
        End Property

        Public ReadOnly Property Id() As String
            Get
                Return Instance.Id
            End Get
        End Property

        Public ReadOnly Property AmiId() As String
            Get
                Return Instance.AmiId
            End Get
        End Property

        Public ReadOnly Property InstanceType() As String
            Get
                Return Instance.InstanceType
            End Get
        End Property

        Public ReadOnly Property LaunchTime() As String
            Get
                Dim d As DateTime = DateFrom_ISO8601(Instance.LaunchTime)
                Return d.ToString("ddd, dd MMM HH:mm")
            End Get
        End Property

        Public ReadOnly Property ScheduledTermination() As String
            Get
                If Instance.Status < -1 Then Return ""
                Dim dt As DateTime = GetNextInstanceTerminationTime(Instance.Id)
                If dt = Nothing Then Return ""
                Return dt.ToString("ddd, dd MMM HH:mm")
            End Get
        End Property

        Public ReadOnly Property ActiveMinutes() As String
            Get
                Dim tm As Integer = New TimeSpan(DateTime.Now.Ticks - Instance.LaunchTime_UTCTicks).TotalMinutes
                Return tm
            End Get
        End Property

        Public ReadOnly Property DiesInMinutes() As String
            Get
                If Instance.Status < -1 Then Return ""
                Dim dt As DateTime = GetNextInstanceTerminationTime(Instance.Id)
                If dt = Nothing Then Return ""
                Return New TimeSpan(dt.Ticks - DateTime.UtcNow.Ticks).TotalMinutes
            End Get
        End Property

        Public ReadOnly Property JobId() As String
            Get
                Return Instance.AssignedToJobId
            End Get
        End Property

        Public ReadOnly Property UserName() As String
            Get
                If String.IsNullOrEmpty(JobId) Then Return ""
                Dim Uid As String = GetJobAttribute(JobId, "UserId")
                Dim un As String = GetUserAttribute(Uid, "Username")
                Return un
            End Get
        End Property

        Public ReadOnly Property PublicHostname() As String
            Get
                Return Instance.PublicHostname
            End Get
        End Property

    End Class

End Class
