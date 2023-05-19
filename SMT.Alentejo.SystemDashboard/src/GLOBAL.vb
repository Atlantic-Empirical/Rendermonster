Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.JobManagement
Imports System.Threading
Imports SMT.Alentejo.Core
Imports Amazon.SimpleDB
Imports Amazon.SimpleDB.Model
Imports SMT.AWS.SDB
Imports SMT.AWS.Authorization
Imports System.ComponentModel

Public Module [GLOBAL]

    Public Sub DownloadFile(ByVal FileId As String)
        MsgBox("inop")
    End Sub

    Public Sub RunRDP(ByVal Hostname As String)
        Process.Start("C:\windows\system32\mstsc.exe", "/v:" & Hostname & " /admin")
    End Sub

    Public Property CurrentUser() As cSMT_ATJ_User_Lite
        Get
            Return _CurrentUser
        End Get
        Set(ByVal value As cSMT_ATJ_User_Lite)
            _CurrentUser = value
        End Set
    End Property
    Private _CurrentUser As cSMT_ATJ_User_Lite

    Public Property CurrentJob() As cSMT_ATJ_RenderJob_Maxwell
        Get
            Return _CurrentJob
        End Get
        Set(ByVal value As cSMT_ATJ_RenderJob_Maxwell)
            _CurrentJob = value
        End Set
    End Property
    Private _CurrentJob As cSMT_ATJ_RenderJob_Maxwell

    Public Sub OpenJobViewer()
        Dim currentDispatcher As System.Windows.Threading.Dispatcher = System.Windows.Application.Current.Dispatcher
        currentDispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Normal, New OpenWindowDelegate(AddressOf _OpenJobViewer))

        'Dim currentDispatcher As System.Windows.Threading.Dispatcher = System.Windows.Application.Current.Dispatcher
        'currentDispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, New OpenWindowDelegate(AddressOf _OpenJobViewer))

        ''Dim TS As New ThreadStart(Function() currentDispatcher.Invoke(OpenWindowDelegate, Nothing))
        'Dim TS As New ThreadStart(System.Windows.Threading.DispatcherPriority.Normal, New OpenWindowDelegate(AddressOf _OpenJobViewer))

        'Dim T As New Thread(AddressOf _OpenJobViewer)
        'T.Start()
    End Sub

    Private Sub _OpenJobViewer()
        Dim jv As New JobViewer(_CurrentJob)
        'jv.Show()
        jv.Show()
    End Sub
    Delegate Sub OpenWindowDelegate()

    'Private Sub OpenWindow(ByVal bringFocus As Boolean)
    '    If Not bringFocus Then
    '        Dim jv As New JobViewer(CurrentJob)
    '        'jv.Show()
    '        jv.ShowDialog()
    '    Else
    '    End If
    'End Sub

    Public ReadOnly Property SDB() As cSMT_AWS_SDB
        Get
            If _SDB Is Nothing Then _SDB = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
            Return _SDB
        End Get
    End Property
    Private _SDB As cSMT_AWS_SDB

End Module
