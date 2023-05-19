Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.Alentejo.Core.AtjTrace
Imports SMT.Alentejo.Core.SessionManagement
Imports SMT.Alentejo.Credit.Write
Imports SMT.Alentejo.Core.Credit.Read

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://rm.seqmt.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class AlentejoAuthService
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Authenticates credentials, if good, returns sessionId.
    ''' </summary>
    ''' <param name="Username"></param>
    ''' <param name="PasswordHash"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Function Login(ByVal Username As String, ByVal PasswordHash As String) As String

        ' CHECK PASSWORD
        Dim Id As String = GetUserIdByUsername(Username)
        If String.IsNullOrEmpty(Id) Then Return ""
        Dim PWH As String = GetUserAttribute(Id, "PasswordHash")
        If String.IsNullOrEmpty(PWH) Then Return ""
        If PWH <> PasswordHash Then Return ""

        ' STORE TIME AND IP
        UpdateUserLoginInfo(Id, HttpContext.Current.Request.UserHostAddress)

        ' STORE NEW SESSION FOR USER
        Dim S As New cSMT_ATJ_Session
        S.IpAddress = HttpContext.Current.Request.UserHostAddress
        S.LoginUTCTicks = DateTime.UtcNow.Ticks.ToString.PadLeft(20, "0")
        S.UserId = Id
        Return SaveSession(S)

    End Function

    <WebMethod()> _
    Sub Logout(ByVal SessionId As String)
        SetSessionAttribute(SessionId, "LogoutUTCTicks", DateTime.UtcNow.Ticks)
    End Sub

    <WebMethod()> _
    Public Function UsernameExists(ByVal Username As String) As Boolean
        Try
            Dim out As Boolean = SMT.Alentejo.Core.UserManagement.UserNameExists(Username)
            Dim atjTRACE As New cSMT_ATJ_TRACE("UserNameExists webmethod")
            atjTRACE.LogMessage(out.ToString, EventLogEntryType.Information)
            Return out
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return False
        End Try
    End Function

    <WebMethod()> _
    Public Function SaveUser(ByVal SessionToken As String, ByVal User As cSMT_ATJ_User) As String
        Try
            Dim uid As String = SMT.Alentejo.Core.UserManagement.SaveUser(User)
            'FOR NOW DO THIS HERE
            SetUserBalance(uid, 500)
            Return uid
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

    <WebMethod()> _
    Public Function GetClientIP() As String
        Try
            Return HttpContext.Current.Request.UserHostAddress
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return ""
        End Try
    End Function

    <WebMethod()> _
    Public Function GetUserInfoLite(ByVal Username As String) As cSMT_ATJ_User_Lite
        Try
            Dim id As String = GetUserIdByUsername(Username)
            Return GetUserLite(id)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return Nothing
        End Try
    End Function

    <WebMethod()> _
    Public Function GetUserBalance(ByVal SessionToken As String, ByVal UserId As String) As Double
        Try
            Dim balance_encrypted As String = GetUserAttribute(UserId, "Balance_Encrypted")
            Return DecodeBalance(balance_encrypted)
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return 0
        End Try
    End Function

End Class