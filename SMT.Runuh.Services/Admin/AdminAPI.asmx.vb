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
Public Class AdminAPI
    Inherits System.Web.Services.WebService

#Region "USERS"

    <WebMethod()> _
    Public Function Users_SaveUser(ByVal SessionToken As String, ByVal User As cSMT_ATJ_User, ByRef outUserId As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken, True) Then Throw New Exception("Invalid session token.")
        Try
            outUserId = SMT.Alentejo.Core.UserManagement.SaveUser(User)
            'FOR NOW DO THIS HERE
            SetUserBalance(outUserId, 500)
            Return eAlentejoAPIResultCode.Success
        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function Users_ListUsers(ByVal SessionToken As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken, True) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

#End Region 'USERS

#Region "JOBS"

    <WebMethod()> _
    Public Function Jobs_ListJobs(ByVal SessionToken As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken, True) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

#End Region 'JOBS

#Region "FINANCE"

    <WebMethod()> _
    Public Function Finance_GetSystemBalance(ByVal SessionToken As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken, True) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

    <WebMethod()> _
    Public Function Finance_GetSystemIncome(ByVal SessionToken As String) As eAlentejoAPIResultCode
        If Not CheckSessionToken(SessionToken, True) Then Throw New Exception("Invalid session token.")
        Try

        Catch ex As Exception
            SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, "", ex.Message, ex.StackTrace)
            Return eAlentejoAPIResultCode.GenericFailure
        End Try
    End Function

#End Region 'FINANCE

End Class