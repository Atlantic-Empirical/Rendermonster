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
<System.Web.Services.WebService(Namespace:="http://runuh.com/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class UserAPI
    Inherits System.Web.Services.WebService

    <WebMethod()> _
    Public Function GetUserInfoLite(ByVal SessionToken As String, ByVal Username As String, ByRef outLiteUserInfo As cSMT_ATJ_User_Lite) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            Dim id As String = GetUserIdByUsername(Username)
            outLiteUserInfo = GetUserLite(id)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetUserCredit(ByVal SessionToken As String, ByVal UserId As String, ByRef outUserCredit As Double) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try
            Dim balance_encrypted As String = GetUserAttribute(UserId, "Balance_Encrypted")
            outUserCredit = DecodeBalance(balance_encrypted)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function GetAddCreditUrl(ByVal SessionToken As String, ByVal AmountToAdd As Double, ByRef outUrl As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function ModifyUserProperties(ByVal SessionToken As String, ByVal NewProperties As cSMT_ATJ_User) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function UsernameExists(ByVal Username As String, ByRef outExists As Boolean) As eAlentejoAPIResultCode
        Try
            Dim out As Boolean = SMT.Alentejo.Core.UserManagement.UserNameExists(Username)
            Dim atjTRACE As New cSMT_ATJ_TRACE("UserNameExists webmethod")
            atjTRACE.LogMessage(out.ToString, EventLogEntryType.Information)
            outExists = out
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function Signup(ByVal User As cSMT_ATJ_User, ByRef outNewUserId As String) As eAlentejoAPIResultCode
        Try
            'TODO: check the requesting IP address for permission to signup users.
            'That is how we will gate access to this method.

            outNewUserId = SMT.Alentejo.Core.UserManagement.SaveUser(User)
            'FOR NOW DO THIS HERE
            SetUserBalance(outNewUserId, 500)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

End Class
