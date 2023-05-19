Imports System.Web.Services
Imports System.Web.Services.Protocols
Imports System.ComponentModel
Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.Alentejo.Core.AtjTrace
Imports SMT.Alentejo.Core.SessionManagement

' To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line.
' <System.Web.Script.Services.ScriptService()> _
<System.Web.Services.WebService(Namespace:="http://tempuri.org/")> _
<System.Web.Services.WebServiceBinding(ConformsTo:=WsiProfiles.BasicProfile1_1)> _
<ToolboxItem(False)> _
Public Class SessionAPI
    Inherits System.Web.Services.WebService

    ''' <summary>
    ''' Authenticates credentials, if good, returns sessionId.
    ''' </summary>
    ''' <param name="Username"></param>
    ''' <param name="PasswordHash"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <WebMethod()> _
    Function GetSessionToken(ByVal Username As String, ByVal PasswordHash As String, ByRef outSessionTokey As String) As eAlentejoAPIResultCode
        Try
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
            outSessionTokey = SaveSession(S)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function InvalidateSessionToken(ByVal SessionToken As String) As eAlentejoAPIResultCode
        Try
            SetSessionAttribute(SessionToken, "LogoutUTCTicks", DateTime.UtcNow.Ticks)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function RenewSessionToken(ByVal SessionToken As String) As eAlentejoAPIResultCode
        Try

        Catch ex As Exception
            Return eAlentejoAPIResultCode.GenericFailure
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

End Class