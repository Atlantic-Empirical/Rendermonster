Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.SessionManagement

Public Module Session_State

    Public Function AuthenticateWebServiceCall(ByVal SessionId As String) As Boolean
        'check sessionId and Ip against those stored in session domain
        'HttpContext.Current.Request.UserHostAddress
        'Return True
        Return SessionCheck(SessionId, HttpContext.Current.Request.UserHostAddress)
    End Function

    'Public Function LoginUser(ByRef Username As String, ByRef PasswordHash As String) As String
    '    ' CHECK PASSWORD
    '    Dim Id As String = GetUserIdByUsername(Username)
    '    If String.IsNullOrEmpty(Id) Then Return False
    '    Dim PWH As String = GetUserAttribute(Id, "PasswordHash")
    '    If String.IsNullOrEmpty(PWH) Then Return False
    '    If PWH <> PasswordHash Then Return False

    '    ' STORE TIME AND IP
    '    UpdateUserLoginInfo(Id, HttpContext.Current.Request.UserHostAddress)

    '    ' ADD TO ACTIVE USERS DOMAIN (?)

    '    Return True
    'End Function

    'Public Sub AuthenticateWebServiceCall()
    '    'if using authentication services
    '    'If Not HttpContext.Current.User.Identity.IsAuthenticated Then Throw New Exception("Session has expired")
    'End Sub

End Module
