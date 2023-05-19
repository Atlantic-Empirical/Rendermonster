Imports SMT.Alentejo.Core.ApplicationManagement

Partial Public Class ucDynamicDeploy

    Private apps As List(Of cSMT_ATJ_Application)

    Private Sub btnUpdate_Click(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnUpdate.Click
        Cursor = Cursors.Wait
        ListApplications()
        ListApplicationMaps()
        Cursor = Cursors.Arrow
    End Sub

    Private Sub ListApplications()
        Try
            Me.lbDynamicApps.Items.Clear()
            apps = GetApplicationList()
            For Each a As cSMT_ATJ_Application In apps
                Me.lbDynamicApps.Items.Add(a)
            Next
        Catch ex As Exception
            MsgBox("Problem with ListApplications(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub ListApplicationMaps()
        Try
            Me.lbAppMapping.Items.Clear()
            Dim s() As Integer = [Enum].GetValues(GetType(eSMT_ATJ_Server))
            Dim al As List(Of cSMT_ATJ_Application)
            For Each server As Integer In s
                al = GetAppListForServer(server)
                If al IsNot Nothing Then
                    For Each a As cSMT_ATJ_Application In al
                        Me.lbAppMapping.Items.Add(([Enum].GetName(GetType(eSMT_ATJ_Server), server) & " - " & a.Name))
                    Next
                End If
            Next
        Catch ex As Exception
            MsgBox("Problem with PrintServerAppMaps(). Error: " & ex.Message)
        End Try
    End Sub

End Class
