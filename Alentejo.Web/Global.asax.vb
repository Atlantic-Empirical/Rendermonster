Imports System.Web.SessionState
Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.AtjTrace

Public Class Global_asax
    Inherits System.Web.HttpApplication

    Sub Application_Start(ByVal sender As Object, ByVal e As EventArgs)
        'AddHandler System.Web.ApplicationServices.AuthenticationService.Authenticating, AddressOf AuthenticationService_Authenticating
    End Sub

    Sub AuthenticationService_Authenticating(ByVal sender As Object, ByVal e As System.Web.ApplicationServices.AuthenticatingEventArgs)
        'Dim t As New cSMT_ATJ_TRACE(System.Reflection.MethodBase.GetCurrentMethod.Module.Name)
        't.LogMessage("AuthenticationService_Authenticating " & e.UserName & "-" & e.Password, EventLogEntryType.Information)
        'e.Authenticated = LoginUser(e.UserName, e.Password)
        't.LogMessage("e.Authenticated=" & e.Authenticated.ToString, EventLogEntryType.Information)
        'e.AuthenticationIsComplete = True
    End Sub

    Sub Session_Start(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session is started
    End Sub

    Sub Application_BeginRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires at the beginning of each request
    End Sub

    Sub Application_AuthenticateRequest(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires upon attempting to authenticate the use
    End Sub

    Sub Application_Error(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when an error occurs
    End Sub

    Sub Session_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the session ends
    End Sub

    Sub Application_End(ByVal sender As Object, ByVal e As EventArgs)
        ' Fires when the application ends
    End Sub

End Class