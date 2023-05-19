Imports SMT.Alentejo.Core
Imports SMT.AWS
Imports SMT.AWS.Authorization
Imports SMT.AWS.S3
Imports System.IO
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.ApplicationManagement
Imports System.Text
Imports System.Runtime.InteropServices

Public Class AppPublisher_Main

#Region "PROPERTIES"

#Region "PROPERTIES:PATHS"

    Private ReadOnly Property code_root() As String
        Get
            Return txtSMTCodePathRoot.Text
        End Get
    End Property
    Private ReadOnly Property jftm_path() As String
        Get
            Return code_root & "JobFileTransferManager\bin\" & If(rbRelease.Checked, "Release", "Debug") & "\"
        End Get
    End Property
    Private ReadOnly Property id_path() As String
        Get
            Return code_root & "InstanceDispatcher\bin\" & If(rbRelease.Checked, "Release", "Debug") & "\"
        End Get
    End Property
    Private ReadOnly Property lr_path() As String
        Get
            Return code_root & "LogReader\bin\" & If(rbRelease.Checked, "Release", "Debug") & "\"
        End Get
    End Property
    Private ReadOnly Property rm_mx_path() As String
        Get
            Return code_root & "RenderManager\bin\" & If(rbRelease.Checked, "Release", "Debug") & "\"
        End Get
    End Property
    Private ReadOnly Property merge_mx_path() As String
        Get
            Return code_root & ""
        End Get
    End Property

#End Region 'PROPERTIES:PATHS

    Private ReadOnly Property S3Object() As cSMT_AWS_S3
        Get
            'Return New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
            If _S3 Is Nothing Then _S3 = New cSMT_AWS_S3(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
            Return _S3
        End Get
    End Property
    Private _S3 As cSMT_AWS_S3

#End Region 'PROPERTIES

#Region "FORM"

    Private Sub AppPublisher_Main_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _HandleTransferStatusEvent_UIThread_Delegate = New HandleTransferStatusEvent_UIThread_Delegate(AddressOf HandleTransferStatusEvent_UIThread)

        Dim s() As String = [Enum].GetNames(GetType(eSMT_ATJ_Server))
        For b As Byte = 0 To UBound(s)
            cbServers.Items.Add(s(b))
        Next

        s = [Enum].GetNames(GetType(eSMT_ATJ_ServerApp))
        For b As Byte = 0 To UBound(s)
            cbApps.Items.Add(s(b))
        Next
    End Sub

#End Region 'FORM

#Region "PUBLISHING"

#Region "PUBLISHING:UI"

    Private Sub btnPublish_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPublish.Click
        Publish()
    End Sub

#End Region 'PUBLISHING:UI

    Private Sub Publish()
        Try
            Cursor = Cursors.WaitCursor
            If cbJobFileTransferManager.Checked Then
                PublishApp(eSMT_ATJ_ServerApp.JobFileTransferManager, jftm_path)
            End If
            If cbInstanceDispatcher.Checked Then
                PublishApp(eSMT_ATJ_ServerApp.InstanceDispatcher, id_path)
            End If
            If cbRenderManager_MX.Checked Then
                PublishApp(eSMT_ATJ_ServerApp.RenderManager_MX, rm_mx_path)
            End If
            If cbPUB_LogReader.Checked Then
                PublishApp(eSMT_ATJ_ServerApp.LogReader, lr_path)
            End If

        Catch ex As Exception
            MsgBox("Problem with Publish(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub PublishApp(ByVal AppName As eSMT_ATJ_ServerApp, ByVal AppDirectory As String)
        Try
            AddConsoleLine("Publishing " & AppName.ToString)

            'Delete old jftm from server
            DeleteApplication(AppName)

            Dim TM As New cSMT_AWS_S3_TransferManager(S3Object)
            TM.Name = AppName.ToString
            AddHandler TM.evTransferStatus, AddressOf HandleTransferSucceeded_TransferThread

            Dim di As New DirectoryInfo(AppDirectory)
            Dim file As cSMT_ATJ_File
            Dim AtjFiles As New List(Of cSMT_ATJ_File)
            Dim FilesToUpload As New List(Of FileInfo)
            FilesToUpload.AddRange(di.GetFiles("*.exe"))
            FilesToUpload.AddRange(di.GetFiles("*.dll"))

            Dim app As New cSMT_ATJ_Application
            app.Name = AppName.ToString
            app.PublishDate = DateTo_ISO8601()
            Dim g As String
            For Each fi As FileInfo In FilesToUpload
                If InStr(fi.Name.ToLower, "vshost") = 0 Then
                    'Store file in SDB
                    g = Guid.NewGuid.ToString
                    file = New cSMT_ATJ_File(g, fi.Name, "", cSMT_ATJ_File.eAtjFileType.SystemApplicationFile, fi.Length, system_user_id)
                    If fi.Extension.ToLower = ".exe" Then file._IsPrimaryExecutable = True
                    AtjFiles.Add(file)

                    'Put file in S3
                    TM.UploadFile(fi.FullName, cSMT_AWS_S3_FileTransfer.eBucketLocation.US, s3_bucket_appfiles_us, g)
                End If
            Next

            Dim appId As String = SaveApplication(app, AtjFiles.ToArray)
            SetApplicationMap(AppName, appId)
            AddConsoleLine(AppName.ToString & " publishing completed (files still transferring maybe).")
        Catch ex As Exception
            Throw New Exception("Problem with PublishApp(). Error: " & ex.Message)
        End Try
    End Sub

#Region "PUBLISHING:TRANSFER EVENTS"

    Private Sub HandleTransferSucceeded_TransferThread(ByVal sender As Object, ByVal e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
        Me.Invoke(_HandleTransferStatusEvent_UIThread_Delegate, New Object() {sender, e})
    End Sub

    Private Sub HandleTransferStatusEvent_UIThread(ByRef sender As cSMT_AWS_S3_TransferManager, ByRef e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
        Try
            Select Case e.EventType
                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Success
                    AddConsoleLine(sender.Name & " - " & e.FileName)

                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Progress
                    'AddConsoleLine(sender.Name & " - " & e.FileName & " " & e.BytesRemaining & "B" & " " & e.ElapsedSeconds & "s")

                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Failure
                    AddConsoleLine(sender.Name.ToUpper & " - " & e.ToString.ToUpper)

            End Select

            Dim notComplete As Byte = 0
            For Each u As cSMT_AWS_S3_FileTransfer In sender.Uploads
                If Not u.IsComplete Then notComplete += 1
            Next
            If notComplete = 0 Then
                AddConsoleLine("File upload completed for " & sender.Name)
            End If
        Catch ex As Exception
            AddConsoleLine("Problem in HandleTransferSucceedFailEvent_UIThread(). Error: " & ex.Message)
        End Try
    End Sub
    Private Delegate Sub HandleTransferStatusEvent_UIThread_Delegate(ByRef sender As cSMT_AWS_S3_TransferManager, ByRef e As cSMT_AWS_S3_TransferManager.cTransferStatusEvent)
    Private _HandleTransferStatusEvent_UIThread_Delegate As HandleTransferStatusEvent_UIThread_Delegate

#End Region 'PUBLISHING:TRANSFER EVENTS

#End Region 'PUBLISHING

#Region "SERVER APP MAPS"

    Private Sub btnSetServerMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSetServerMap.Click
        Try
            If cbServers.SelectedIndex = -1 Or cbApps.SelectedIndex = -1 Then Exit Sub
            SetServerMap(cbServers.SelectedIndex, cbApps.SelectedIndex)
            AddConsoleLine(cbServers.SelectedItem.ToString & " mapped to " & cbApps.SelectedItem.ToString)
        Catch ex As Exception
            MsgBox("Problem with SetServerMap(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnDeleteServerMap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteServerMap.Click
        Try
            If cbServers.SelectedIndex = -1 Or cbApps.SelectedIndex = -1 Then Exit Sub
            DeleteServerMap(cbServers.SelectedIndex, cbApps.SelectedIndex)
            AddConsoleLine(cbServers.SelectedItem.ToString & " unmapped from " & cbApps.SelectedItem.ToString)
        Catch ex As Exception
            MsgBox("Problem with DeleteServerMap(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub btnPrintServerMaps_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPrintServerMaps.Click
        PrintServerAppMaps()
    End Sub

    Private Sub PrintServerAppMaps()
        Try
            Cursor = Cursors.WaitCursor
            Dim s() As Integer = [Enum].GetValues(GetType(eSMT_ATJ_Server))
            Dim al As List(Of cSMT_ATJ_Application)
            For Each server As Integer In s
                al = GetAppListForServer(server)
                If al IsNot Nothing Then
                    For Each a As cSMT_ATJ_Application In al
                        AddConsoleLine([Enum].GetName(GetType(eSMT_ATJ_Server), server) & " - " & a.Name)
                    Next
                End If
            Next
        Catch ex As Exception
            MsgBox("Problem with PrintServerAppMaps(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region 'SERVER APP MAPS

#Region "CONSOLE"

    Private Sub AddConsoleLine(ByVal msg As String)
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _AppendConsole_Delegate Is Nothing Then _AppendConsole_Delegate = New AppendConsole_Delegate(AddressOf int_AppendConsole)
            Me.Invoke(_AppendConsole_Delegate, New Object() {msg})
        Else
            int_AppendConsole(msg)
        End If
    End Sub

    Private Sub int_AppendConsole(ByVal Msg As String)
        If rtbQueuePollingConsole.Lines.Count > 1000 Then
            Dim tStr(499) As String
            Array.Copy(rtbQueuePollingConsole.Lines, 500, tStr, 0, 500)
            rtbQueuePollingConsole.Lines = tStr
            Me.rtbQueuePollingConsole.AppendText(vbNewLine)
        End If
        Me.rtbQueuePollingConsole.AppendText(Msg & vbNewLine)
        Me.rtbQueuePollingConsole.ScrollToCaret()
    End Sub
    Private Delegate Sub AppendConsole_Delegate(ByVal Msg As String)
    Private _AppendConsole_Delegate As AppendConsole_Delegate

    Private Sub ClearConsole()
        If Me.rtbQueuePollingConsole.InvokeRequired Then
            If _ClearConsole_Delegate Is Nothing Then _ClearConsole_Delegate = New ClearConsole_Delegate(AddressOf int_ClearConsole)
            Me.Invoke(_ClearConsole_Delegate)
        Else
            int_ClearConsole()
        End If
    End Sub

    Private Sub int_ClearConsole()
        rtbQueuePollingConsole.Clear()
    End Sub
    Private Delegate Sub ClearConsole_Delegate()
    Private _ClearConsole_Delegate As ClearConsole_Delegate

    Private Sub rtbQueuePollingConsole_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles rtbQueuePollingConsole.DoubleClick
        Me.rtbQueuePollingConsole.Clear()
    End Sub

#End Region 'CONSOLE

#Region "RETRIEVE"

    Private Downloads As List(Of cSMT_AWS_S3_FileTransfer)

    Private Sub btnRetrieve_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRetrieve.Click
        RetrieveApps()
    End Sub

    Private Sub RetrieveApps()
        Try
            Cursor = Cursors.WaitCursor

            If Not Downloads Is Nothing Then
                Downloads.Clear()
                Downloads = Nothing
            End If
            If cbRET_JobFileTransferManager.Checked Then
                RetrieveApp(eSMT_ATJ_ServerApp.JobFileTransferManager, txtRET_Directory.Text & eSMT_ATJ_ServerApp.JobFileTransferManager.ToString)
            End If
            If cbRET_InstanceDispatcher.Checked Then
                RetrieveApp(eSMT_ATJ_ServerApp.InstanceDispatcher, txtRET_Directory.Text & eSMT_ATJ_ServerApp.InstanceDispatcher.ToString)
            End If
            If cbRET_RenderManagerMX.Checked Then
                RetrieveApp(eSMT_ATJ_ServerApp.RenderManager_MX, txtRET_Directory.Text & eSMT_ATJ_ServerApp.RenderManager_MX.ToString)
            End If
            If cbRET_LogReader.Checked Then
                RetrieveApp(eSMT_ATJ_ServerApp.LogReader, txtRET_Directory.Text & eSMT_ATJ_ServerApp.LogReader.ToString)
            End If
        Catch ex As Exception
            MsgBox("Problem with RetrieveApps(). Error: " & ex.Message)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Returns the path to the primary executable.
    ''' </summary>
    ''' <param name="App"></param>
    ''' <param name="TargetPath"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function RetrieveApp(ByVal App As eSMT_ATJ_ServerApp, ByVal TargetPath As String) As String
        Try
            AddConsoleLine(System.Reflection.MethodBase.GetCurrentMethod.Name & " - " & App.ToString)

            If Directory.Exists(TargetPath) Then
                Dim di As New DirectoryInfo(TargetPath)
                For Each fsi As FileSystemInfo In di.GetFileSystemInfos
                    fsi.Delete()
                Next
            Else
                Directory.CreateDirectory(TargetPath)
            End If

            Dim out As String = ""
            Dim appId As String = GetApplicationId(App.ToString)
            Dim fs As List(Of cSMT_ATJ_File) = GetApplicationFiles(appId)
            For Each f As cSMT_ATJ_File In fs
                S3Object.GetFileObject(s3_bucket_appfiles_us, f.Id, TargetPath & "\" & f.FileName)
                If f._IsPrimaryExecutable Then
                    out = TargetPath & "\" & f.FileName
                End If
            Next
            Return out
        Catch ex As Exception
            'TODO: something drastic!
            AddConsoleLine("Problem with RetrieveExecutables(). Error: " & ex.Message)
            Return ""
        End Try
    End Function

    '#Region "EXECUTABLE RETRIEVAL:TRANSFER EVENTS"

    '    Private Sub AddTransferHandlers(ByRef nTransfer As cSMT_AWS_S3_FileTransfer)
    '        Try
    '            AddHandler nTransfer.evTransferFailed, AddressOf Me.HandleTransferFailedEvent
    '            AddHandler nTransfer.evTransferProgress, AddressOf Me.HandleTransferProgressEvent
    '            AddHandler nTransfer.evTransferSucceeded, AddressOf Me.HandleTransferSuccessEvent
    '        Catch ex As Exception
    '            Throw New Exception("Problem with AddTransferHandlers(). Error: " & ex.Message, ex)
    '        End Try
    '    End Sub

    '    Private Sub HandleTransferProgressEvent(ByVal Direction As SMT.AWS.S3.cSMT_AWS_S3_FileTransfer.eDirection, ByVal FileName As String, ByVal BytesTransferred As Long, ByVal BytesRemaining As Long, ByVal ElapsedSeconds As Long, ByVal ObjectKey As String)
    '        Try
    '            'Debug.WriteLine("TRANSFER STATUS: " & Direction.ToString & " " & FileName & " " & BytesTransferred & " " & BytesRemaining & " " & ElapsedSeconds)
    '            Dim e As New cTransferStatusEvent
    '            e.BytesRemaining = BytesRemaining
    '            e.BytesTransferred = BytesTransferred
    '            e.IsUpload = Direction = cSMT_AWS_S3_FileTransfer.eDirection.Up
    '            e.FileName = FileName
    '            e.ObjectKey = ObjectKey
    '            e.ElapsedSeconds = ElapsedSeconds
    '            e.EventType = cTransferStatusEvent.eTransferStatusEventType.Progress
    '            RaiseEvent evTransferStatus(Nothing, e)
    '        Catch ex As Exception
    '            Throw New Exception("Problem with HandleTransferProgressEvent(). Error: " & ex.Message, ex)
    '        End Try
    '    End Sub

    '    Private Sub HandleTransferSuccessEvent(ByVal Direction As SMT.AWS.S3.cSMT_AWS_S3_FileTransfer.eDirection, ByVal FileName As String, ByVal AS3_ObjectKey As String)
    '        Try
    '            'Debug.WriteLine("SUCCESS: " & Direction.ToString & " " & FileName)
    '            'RaiseEvent evFileTransferSuccess(Direction = cSMT_AWS_S3_FileTransfer.eDirection.Up, FileName, AS3_ObjectKey)
    '            Dim e As New cTransferStatusEvent
    '            e.IsUpload = Direction = cSMT_AWS_S3_FileTransfer.eDirection.Up
    '            e.FileName = FileName
    '            e.ObjectKey = AS3_ObjectKey
    '            e.EventType = cTransferStatusEvent.eTransferStatusEventType.Success
    '            RaiseEvent evTransferStatus(Nothing, e)
    '        Catch ex As Exception
    '            Throw New Exception("Problem with HandleTransferSuccessEvent(). Error: " & ex.Message, ex)
    '        End Try
    '    End Sub

    '    Private Sub HandleTransferFailedEvent(ByVal Direction As SMT.AWS.S3.cSMT_AWS_S3_FileTransfer.eDirection, ByVal FileName As String, ByVal Msg As String, ByVal AS3_ObjectKey As String)
    '        Try
    '            'Debug.WriteLine("FAILURE: " & Direction.ToString & " " & FileName)
    '            'RaiseEvent evFileTransferFailed(Direction = cSMT_AWS_S3_FileTransfer.eDirection.Up, FileName, Msg, AS3_ObjectKey)
    '            Dim e As New cTransferStatusEvent
    '            e.IsUpload = Direction = cSMT_AWS_S3_FileTransfer.eDirection.Up
    '            e.FileName = FileName
    '            e.ObjectKey = AS3_ObjectKey
    '            e.Message = Msg
    '            e.EventType = cTransferStatusEvent.eTransferStatusEventType.Failure
    '            RaiseEvent evTransferStatus(Nothing, e)
    '        Catch ex As Exception
    '            Throw New Exception("Problem with HandleTransferFailedEvent(). Error: " & ex.Message, ex)
    '        End Try
    '    End Sub

    '    Public Event evTransferStatus As EventHandler(Of cTransferStatusEvent)

    '    Public Class cTransferStatusEvent
    '        Inherits System.EventArgs

    '        Public EventType As eTransferStatusEventType

    '        Public IsUpload As Boolean
    '        Public FileName As String
    '        Public ObjectKey As String
    '        Public Message As String

    '        Public BytesTransferred As Long
    '        Public BytesRemaining As Long
    '        Public ElapsedSeconds As Long

    '        Public Enum eTransferStatusEventType
    '            Progress
    '            Success
    '            Failure
    '        End Enum

    '        Public Overrides Function ToString() As String
    '            Dim sb As New StringBuilder
    '            sb.Append(System.IO.Path.GetFileName(FileName))
    '            sb.Append(If(IsUpload, " Upload", " Download"))
    '            sb.Append(" " & ObjectKey)
    '            Select Case EventType
    '                Case eTransferStatusEventType.Progress
    '                    sb.Append(" " & BytesTransferred & "/" & BytesRemaining & " " & ElapsedSeconds & "s")
    '                Case eTransferStatusEventType.Success
    '                    sb.Append(" Success")
    '                Case eTransferStatusEventType.Failure
    '                    sb.Append(" Failure - " & Message)
    '            End Select
    '            Return sb.ToString
    '        End Function

    '    End Class

    '    Private Sub HandleTransferEvent(ByVal sender As cSMT_AWS_S3_TransferManager, ByVal e As cTransferStatusEvent) Handles Me.evTransferStatus
    '        Try
    '            aBeep(1000, 2)
    '            Select Case e.EventType
    '                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Success
    '                    AddConsoleLine("Transfer succeeded: " & e.FileName)

    '                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Progress
    '                    'AddConsoleLine(e.FileName & " " & e.BytesRemaining & "B" & " " & e.ElapsedSeconds & "s")

    '                Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Failure
    '                    AddConsoleLine("Transfer failed: " & e.ToString.ToUpper)
    '                    'TODO: something drastic here. IMPORTANT.

    '            End Select

    '            Dim notComplete As Byte = 0
    '            'AddConsoleLine("")
    '            For Each d As cSMT_AWS_S3_FileTransfer In Downloads
    '                If Not d.IsComplete Then
    '                    notComplete += 1
    '                    'AddConsoleLine(d.FileName & " is not complete.")
    '                Else
    '                    'AddConsoleLine(d.FileName & " is complete.")
    '                End If
    '            Next
    '            If notComplete = 0 Then
    '                AddConsoleLine("File transfer completed.")
    '            End If
    '        Catch ex As Exception
    '            AddConsoleLine("Problem in HandleTransferSucceedFailEvent_UIThread(). Error: " & ex.Message)
    '            'TODO: something drastic here. IMPORTANT.
    '        End Try
    '    End Sub

    '    'put this code just below the class level

    '    <DllImport("KERNEL32.DLL", EntryPoint:="Beep", SetLastError:=True, CharSet:=CharSet.Unicode, ExactSpelling:=True, CallingConvention:=CallingConvention.StdCall)> _
    '    Public Shared Function aBeep(ByVal dwFreq As Integer, ByVal dwDuration As Integer) As Boolean
    '    End Function

    '    'Private Sub HandleTransferEvent(ByVal sender As cSMT_AWS_S3_TransferManager, ByVal e As cTransferStatusEvent) Handles Me.evTransferStatus
    '    '    Try
    '    '        Select Case e.EventType
    '    '            Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Success
    '    '                AddConsoleLine("Transfer succeeded: " & sender.Name & " - " & e.FileName)

    '    '            Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Progress
    '    '                AddConsoleLine(sender.Name & " - " & e.FileName & " " & e.BytesRemaining & "B" & " " & e.ElapsedSeconds & "s")

    '    '            Case cSMT_AWS_S3_TransferManager.cTransferStatusEvent.eTransferStatusEventType.Failure
    '    '                AddConsoleLine(sender.Name.ToUpper & " - " & e.ToString.ToUpper)
    '    '                'TODO: something drastic here. IMPORTANT.
    '    '                AddConsoleLine("Transfer failed: " & sender.Name & " - " & e.FileName)

    '    '        End Select

    '    '        Dim notComplete As Byte = 0
    '    '        For Each d As cSMT_AWS_S3_FileTransfer In Downloads
    '    '            If Not d.IsComplete Then
    '    '                notComplete += 1
    '    '                AddConsoleLine(d.FileName & " is not complete.")
    '    '            End If
    '    '        Next
    '    '        If notComplete = 0 Then
    '    '            AddConsoleLine("File transfer completed for " & sender.Name)
    '    '        End If

    '    '        'Dim notComplete As Byte = 0
    '    '        'For Each d As cSMT_AWS_S3_FileTransfer In sender.Downloads
    '    '        '    If Not d.IsComplete Then
    '    '        '        notComplete += 1
    '    '        '        AddConsoleLine(d.FileName & " is not complete.")
    '    '        '    End If
    '    '        'Next
    '    '        'If notComplete = 0 Then
    '    '        '    AddConsoleLine("File transfer completed for " & sender.Name)
    '    '        'End If
    '    '    Catch ex As Exception
    '    '        AddConsoleLine("Problem in HandleTransferSucceedFailEvent_UIThread(). Error: " & ex.Message)
    '    '        'TODO: something drastic here. IMPORTANT.
    '    '    End Try
    '    'End Sub

    '#End Region 'EXECUTABLE RETRIEVAL:TRANSFER EVENTS

#End Region 'RETRIEVE

End Class
