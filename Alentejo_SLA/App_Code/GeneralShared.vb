Imports System.Windows.Browser

Public Module GeneralShared

    Public Sub OpenDetail(ByVal FE As FrameworkElement, ByVal DetailPage As ePages)
        If FE.Parent.ToString = "SMT.Alentejo_SLA.RootPage" Then
            CType(FE.Parent, RootPage).LoadDetail(DetailPage)
        Else
            OpenDetail(FE.Parent, DetailPage)
        End If
    End Sub

    Public Sub CloseDetail(ByRef FE As FrameworkElement)
        If FE.Parent.ToString = "SMT.Alentejo_SLA.RootPage" Then
            CType(FE.Parent, RootPage).CloseDetail()
        Else
            CloseDetail(FE.Parent)
        End If
    End Sub

    Public Sub ShowBigWait()
        Dim rp As RootPage = TryCast(App.Current.RootVisual, RootPage)
        If rp Is Nothing Then Exit Sub
        rp.ShowBigWait()
    End Sub

    Public Sub HideBigWait()
        Dim rp As RootPage = TryCast(App.Current.RootVisual, RootPage)
        If rp Is Nothing Then Exit Sub
        rp.HideBigWait()
    End Sub

    'Public Sub SetCursor(ByRef FE As FrameworkElement, ByRef C As System.Windows.Input.Cursor)
    '    If FE.Parent Is Nothing Then
    '        FE.Cursor = C
    '        Exit Sub
    '    End If
    '    If FE.Parent.ToString = "SMT.Alentejo_SLA.RootPage" Then
    '        CType(FE.Parent, RootPage).LayoutRoot.Cursor = C
    '    Else
    '        'SetCursor(FE.Parent, C)
    '    End If
    'End Sub

    Public Sub DebugWrite(ByVal Msg As String)
#If DEBUG Then
        System.Diagnostics.Debug.WriteLine(Msg)
#End If
    End Sub

    Public Enum ePages
        Home
        LOGIN
        Signup
        USER_Home
        USER_JobBrowser
        USER_JobViewer
        ACCOUNT_Main
        ACCOUNT_Billing
        RENDER_SubmitJob
        SUPPORT_Main
    End Enum

    Public Property User() As cSMT_ATJ_ClientSideUserProfile
        Get
            If _User IsNot Nothing Then Return _User
            If Not Iso.FileExists(_ProfileFileName) Then Return Nothing
            Try
                _User = DeserializeObject_XML(GetType(cSMT_ATJ_ClientSideUserProfile), Iso.File(_ProfileFileName))
            Catch ex As Exception
                'deserialization failed. delete the user and return nothing
                User = Nothing
            End Try
            Return _User
        End Get
        Set(ByVal value As cSMT_ATJ_ClientSideUserProfile)
            _User = value
            Iso.DeleteFile(_ProfileFileName)
            If _User IsNot Nothing Then
                SerializeObject_XML(_User, Iso.File(_ProfileFileName), True)
            End If
        End Set
    End Property
    Private _User As cSMT_ATJ_ClientSideUserProfile
    Private _ProfileFileName As String = "user_profile.xml"

    Public Function IsNumeric(ByVal obj As Object) As Boolean
        Return Microsoft.VisualBasic.CompilerServices.Versioned.IsNumeric(obj)
    End Function

    Public Sub DownloadFile(ByVal FileId As String)
        Dim ifDownloader As HtmlElement = HtmlPage.Document.GetElementById("ifDownloader")
        ifDownloader.RemoveAttribute("src")
        ifDownloader.SetAttribute("src", New Uri(HtmlPage.Document.DocumentUri, FileId & ".atjdl?SessionHash=" & GetSessionHash()).AbsoluteUri)
    End Sub

    Public Sub DownloadFile_DirectFromS3(ByVal FileUrl As String)
        Dim ifDownloader As HtmlElement = HtmlPage.Document.GetElementById("ifDownloader")
        ifDownloader.RemoveAttribute("src")
        ifDownloader.SetAttribute("src", FileUrl)
    End Sub

    Public Function GetSessionHash() As String
        Return "123"
    End Function

    Public Function Charges_FriendlyString(ByVal UnFriendlyString As String) As String
        If String.IsNullOrEmpty(UnFriendlyString) Then Return "0.00"
        If InStr(UnFriendlyString, ".") = 0 Then
            Return UnFriendlyString & ".00"
        Else
            Dim s() As String = Split(UnFriendlyString, ".")
            If s(1).Length < 2 Then
                Return UnFriendlyString & "0"
            Else
                Return UnFriendlyString
            End If
        End If
    End Function

End Module

Module MessageBox
    Public Sub Show(ByVal message As String)
        System.Windows.Browser.HtmlPage.Window.Alert(message)
    End Sub
    'Private WithEvents MsgBox As ucMessageBox
    'Public Function Show(ByVal Message As String, ByVal Parent As FrameworkElement, Optional ByVal Title As String = "Render Monster", Optional ByVal Buttons As eMessageBoxButtons = eMessageBoxButtons.OkCancel) As String
    '    MsgBox = New ucMessageBox


    'End Function

    'Private Sub MsgBox_Close() Handles MsgBox.evClosed

    'End Sub

    'Public Enum eMessageBoxButtons
    '    OkCancel
    '    YesNo
    '    AbortRetryFail
    'End Enum
End Module
