'http://odetocode.com/Blogs/scott/archive/2004/10/28/602.aspx
'http://msdn.microsoft.com/en-us/library/ms682429(VS.85).aspx
'http://support.microsoft.com/default.aspx?scid=kb;EN-US;889251
'https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=3223434&SiteID=1

Imports System.Runtime.InteropServices
Imports System.Security.Principal

Public Module StartProcessAsUser

#Region "SUCCESSFUL APPROACH"

    Public Sub RunProcess(ByVal ExecutablePath As String, ByVal DoLogonUser As Boolean, ByVal UserName As String, ByVal Password As String)
        Dim pi As PROCESS_INFORMATION
        Dim si As STARTUPINFO
        Dim sa As SECURITY_ATTRIBUTES
        Dim hDupedToken As IntPtr
        Dim res As Integer
        Dim hToken As IntPtr
        Dim SessionId As Integer
        Dim b As Boolean

        Try
            If DoLogonUser Then
                res = LogonUser(UserName, "", Password, LogonType.LOGON32_LOGON_INTERACTIVE, LogonProvider.LOGON32_PROVIDER_DEFAULT, hToken)
                If res < 0 Then Throw New Exception("LogonUser() failed.")
            Else
                SessionId = WTSGetActiveConsoleSessionId()
                res = WTSQueryUserToken(SessionId, hToken)
                If res < 0 Then Throw New Exception("WTSQueryUserToken() failed.")
            End If

            sa = New SECURITY_ATTRIBUTES
            sa.Length = Marshal.SizeOf(sa)

            res = DuplicateTokenEx(hToken, TOKEN_ALL_ACCESS, sa, CInt(SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation), CInt(TOKEN_TYPE.TokenPrimary), hDupedToken)
            If res < 0 Then Throw New Exception("DuplicateTokenEx() failed.")

            si = New STARTUPINFO
            si.cb = Marshal.SizeOf(si)
            si.lpDesktop = "winsta0\default"
            'si.lpDesktop = "Winsta0\Winlogon"
            'si.dwFlags = STARTF_USESHOWWINDOW
            'si.wShowWindow = SW_SHOWNORMAL
            si.cb = Marshal.SizeOf(si)

            pi = New PROCESS_INFORMATION

            b = CreateProcessAsUser(hDupedToken, ExecutablePath, [String].Empty, sa, sa, False, 0, IntPtr.Zero, "C:\", si, pi)
            If Not b Then
                Dim hr As Integer = Marshal.GetLastWin32Error()
                'Dim ex As Exception = Marshal.GetExceptionForHR(hr)
                Throw New Exception("CreateProcessAsUser() Failed. Message: " & "" & " Code: " & hr)
            End If

        Catch ex As Exception
            Throw New Exception("Problem with RunProcess(). Error: " & ex.Message, ex)
        Finally
            If pi.hProcess <> IntPtr.Zero Then
                CloseHandle(pi.hProcess)
            End If
            If pi.hThread <> IntPtr.Zero Then
                CloseHandle(pi.hThread)
            End If
            If hDupedToken <> IntPtr.Zero Then
                CloseHandle(hDupedToken)
            End If
        End Try
    End Sub

#Region "CONSTS"

    Private Const SW_SHOWNORMAL = 1
    Private Const STARTF_USESHOWWINDOW As UInt16 = 1

    Private Const STANDARD_RIGHTS_REQUIRED As UInt32 = &HF0000
    Private Const STANDARD_RIGHTS_READ As UInt32 = &H20000
    Private Const TOKEN_ASSIGN_PRIMARY As UInt32 = &H1
    Private Const TOKEN_DUPLICATE As UInt32 = &H2
    Private Const TOKEN_IMPERSONATE As UInt32 = &H4
    Private Const TOKEN_QUERY As UInt32 = &H8
    Private Const TOKEN_QUERY_SOURCE As UInt32 = &H10
    Private Const TOKEN_ADJUST_PRIVILEGES As UInt32 = &H20
    Private Const TOKEN_ADJUST_GROUPS As UInt32 = &H40
    Private Const TOKEN_ADJUST_DEFAULT As UInt32 = &H80
    Private Const TOKEN_ADJUST_SESSIONID As UInt32 = &H100
    Private Const TOKEN_READ As UInt32 = (STANDARD_RIGHTS_READ Or TOKEN_QUERY)
    Private Const TOKEN_ALL_ACCESS As UInt32 = (STANDARD_RIGHTS_REQUIRED Or TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or TOKEN_ADJUST_PRIVILEGES Or TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or TOKEN_ADJUST_SESSIONID)

#End Region 'CONSTS

#Region "STRUCTURES"

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure STARTUPINFO
        Public cb As Int32
        Public lpReserved As String
        Public lpDesktop As String
        Public lpTitle As String
        Public dwX As Int32
        Public dwY As Int32
        Public dwXSize As Int32
        Public dwXCountChars As Int32
        Public dwYCountChars As Int32
        Public dwFillAttribute As Int32
        Public dwFlags As Int32
        Public wShowWindow As Int16
        Public cbReserved2 As Int16
        Public lpReserved2 As IntPtr
        Public hStdInput As IntPtr
        Public hStdOutput As IntPtr
        Public hStdError As IntPtr
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure PROCESS_INFORMATION
        Public hProcess As IntPtr
        Public hThread As IntPtr
        Public dwProcessID As Int32
        Public dwThreadID As Int32
    End Structure

    <StructLayout(LayoutKind.Sequential)> _
    Private Structure SECURITY_ATTRIBUTES
        Public Length As Int32
        Public lpSecurityDescriptor As IntPtr
        Public bInheritHandle As Boolean
    End Structure

    Private Enum SECURITY_IMPERSONATION_LEVEL
        SecurityAnonymous
        SecurityIdentification
        SecurityImpersonation
        SecurityDelegation
    End Enum

    Private Enum TOKEN_TYPE
        TokenPrimary = 1
        TokenImpersonation
    End Enum

#End Region 'STRUCTURES

#Region "INTEROP"

    <DllImport("kernel32.dll", EntryPoint:="CloseHandle", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)> _
    Private Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function
    <DllImport("advapi32.dll", EntryPoint:="CreateProcessAsUser", SetLastError:=True, CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.StdCall)> _
    Private Function CreateProcessAsUser(ByVal hToken As IntPtr, _
                                               ByVal lpApplicationName As String, _
                                               ByVal lpCommandLine As String, _
                                               ByRef lpProcessAttributes As SECURITY_ATTRIBUTES, _
                                               ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, _
                                               ByVal bInheritHandle As Boolean, _
                                               ByVal dwCreationFlags As Int32, _
                                               ByVal lpEnvrionment As IntPtr, _
                                               ByVal lpCurrentDirectory As String, _
                                               ByRef lpStartupInfo As STARTUPINFO, _
                                               ByRef lpProcessInformation As PROCESS_INFORMATION) As Boolean
    End Function
    <DllImport("advapi32.dll", EntryPoint:="DuplicateTokenEx")> _
    Private Function DuplicateTokenEx(ByVal hExistingToken As IntPtr, ByVal dwDesiredAccess As Int32, ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, ByVal ImpersonationLevel As Int32, ByVal dwTokenType As Int32, ByRef phNewToken As IntPtr) As Integer
    End Function
    Private Declare Function WTSGetActiveConsoleSessionId Lib "Kernel32.dll" Alias "WTSGetActiveConsoleSessionId" () As Integer
    Private Declare Function WTSQueryUserToken Lib "wtsapi32.dll" (ByVal sessionId As UInt32, ByRef Token As IntPtr) As Integer
    Declare Auto Function LogonUser Lib "advapi32.dll" (ByVal lpszUsername As String, ByVal lpszDomain As String, ByVal lpszPassword As String, ByVal dwLogonType As LogonType, ByVal dwLogonProvider As LogonProvider, ByRef phToken As IntPtr) As Integer

    Public Enum LogonType As Integer
        'This logon type is intended for users who will be interactively using the computer, such as a user being logged on 
        'by a terminal server, remote shell, or similar process.
        'This logon type has the additional expense of caching logon information for disconnected operations; 
        'therefore, it is inappropriate for some client/server applications,
        'such as a mail server.
        LOGON32_LOGON_INTERACTIVE = 2

        'This logon type is intended for high performance servers to authenticate plaintext passwords.
        'The LogonUser function does not cache credentials for this logon type.
        LOGON32_LOGON_NETWORK = 3

        'This logon type is intended for batch servers, where processes may be executing on behalf of a user without 
        'their direct intervention. This type is also for higher performance servers that process many plaintext
        'authentication attempts at a time, such as mail or Web servers. 
        'The LogonUser function does not cache credentials for this logon type.
        LOGON32_LOGON_BATCH = 4

        'Indicates a service-type logon. The account provided must have the service privilege enabled. 
        LOGON32_LOGON_SERVICE = 5

        'This logon type is for GINA DLLs that log on users who will be interactively using the computer. 
        'This logon type can generate a unique audit record that shows when the workstation was unlocked. 
        LOGON32_LOGON_UNLOCK = 7

        'This logon type preserves the name and password in the authentication package, which allows the server to make 
        'connections to other network servers while impersonating the client. A server can accept plaintext credentials 
        'from a client, call LogonUser, verify that the user can access the system across the network, and still 
        'communicate with other servers.
        'NOTE: Windows NT:  This value is not supported. 
        LOGON32_LOGON_NETWORK_CLEARTEXT = 8

        'This logon type allows the caller to clone its current token and specify new credentials for outbound connections.
        'The new logon session has the same local identifier but uses different credentials for other network connections. 
        'NOTE: This logon type is supported only by the LOGON32_PROVIDER_WINNT50 logon provider.
        'NOTE: Windows NT:  This value is not supported. 
        LOGON32_LOGON_NEW_CREDENTIALS = 9
    End Enum

    Public Enum LogonProvider As Integer
        'Use the standard logon provider for the system. 
        'The default security provider is negotiate, unless you pass NULL for the domain name and the user name 
        'is not in UPN format. In this case, the default provider is NTLM. 
        'NOTE: Windows 2000/NT:   The default security provider is NTLM.
        LOGON32_PROVIDER_DEFAULT = 0
    End Enum

#End Region 'INTEROP

#End Region 'SUCCESSFUL APPROACH

#Region "OTHER EXPLORATION"

#Region "CONSTS"

    'Private Const SW_SHOWNORMAL = 1

    'Private Const STARTF_USESHOWWINDOW As UInt16 = 1
    'Private Const STARTF_USESIZE As UInt16 = 2
    'Private Const STARTF_USEPOSITION As UInt16 = 4
    'Private Const STARTF_USECOUNTCHARS As UInt16 = 8
    'Private Const STARTF_USEFILLATTRIBUTE As UInt16 = 10
    'Private Const STARTF_RUNFULLSCREEN As UInt16 = 20  ' ignored for non-x86 platforms
    'Private Const STARTF_FORCEONFEEDBACK As UInt16 = 40
    'Private Const STARTF_FORCEOFFFEEDBACK As UInt16 = 80
    'Private Const STARTF_USESTDHANDLES As UInt16 = 100

    'Private Const GENERIC_ALL_ACCESS As Integer = &H10000000

#End Region 'CONSTS

#Region "STRUCTURES"

#End Region 'STRUCTURES

#Region "INTEROP"

    <DllImport("advapi32.dll", SetLastError:=True)> _
    Private Function OpenProcessToken(ByVal ProcessHandle As IntPtr, _
                                        ByVal DesiredAccess As Integer, _
                                        ByRef TokenHandle As IntPtr) As Boolean
    End Function
    Private Declare Function GetCurrentProcess Lib "kernel32.dll" () As IntPtr
    Private Declare Function SetTokenInformation Lib "advapi32.dll" (ByVal TokenHandle As Integer, ByRef TokenInformationClass As Short, ByRef TokenInformation As Object, ByVal TokenInformationLength As Integer) As Integer

#End Region 'INTEROP

#End Region 'OTHER EXPLORATION

End Module


''http://odetocode.com/Blogs/scott/archive/2004/10/28/602.aspx
''http://msdn.microsoft.com/en-us/library/ms682429(VS.85).aspx
''http://support.microsoft.com/default.aspx?scid=kb;EN-US;889251
''https://forums.microsoft.com/MSDN/ShowPost.aspx?PostID=3223434&SiteID=1

'Imports System.Runtime.InteropServices
'Imports System.Security.Principal

'Public Module StartProcessAsUser

'#Region "SUCCESSFUL APPROACH"

'    Public Sub RunProcess(ByVal ExecutablePath As String)
'        Dim pi As PROCESS_INFORMATION
'        Dim si As STARTUPINFO
'        Dim sa As SECURITY_ATTRIBUTES
'        Dim hDupedToken As IntPtr
'        Dim res As Integer
'        Dim hToken As IntPtr
'        Dim SessionId As Integer
'        Dim b As Boolean

'        Try
'            SessionId = WTSGetActiveConsoleSessionId()
'            res = WTSQueryUserToken(SessionId, hToken)
'            If res < 0 Then Throw New Exception("WTSQueryUserToken() failed.")

'            sa = New SECURITY_ATTRIBUTES
'            sa.Length = Marshal.SizeOf(sa)

'            res = DuplicateTokenEx(hToken, TOKEN_ALL_ACCESS, sa, CInt(SECURITY_IMPERSONATION_LEVEL.SecurityImpersonation), CInt(TOKEN_TYPE.TokenPrimary), hDupedToken)
'            If res < 0 Then Throw New Exception("DuplicateTokenEx() failed.")

'            si = New STARTUPINFO
'            si.cb = Marshal.SizeOf(si)
'            si.lpDesktop = "winsta0\default"
'            'si.lpDesktop = "Winsta0\Winlogon"
'            'si.dwFlags = STARTF_USESHOWWINDOW
'            'si.wShowWindow = SW_SHOWNORMAL
'            si.cb = Marshal.SizeOf(si)

'            pi = New PROCESS_INFORMATION

'            b = CreateProcessAsUser(hDupedToken, ExecutablePath, [String].Empty, sa, sa, False, 0, IntPtr.Zero, "C:\", si, pi)
'            If Not b Then
'                Dim hr As Integer = Marshal.GetLastWin32Error()
'                'Dim ex As Exception = Marshal.GetExceptionForHR(hr)
'                Throw New Exception("CreateProcessAsUser() Failed. Message: " & "" & " Code: " & hr)
'            End If

'        Catch ex As Exception
'            Throw New Exception("Problem with NewCreateProcessAsUser(). Error: " & ex.Message, ex)
'        Finally
'            If pi.hProcess <> IntPtr.Zero Then
'                CloseHandle(pi.hProcess)
'            End If
'            If pi.hThread <> IntPtr.Zero Then
'                CloseHandle(pi.hThread)
'            End If
'            If hDupedToken <> IntPtr.Zero Then
'                CloseHandle(hDupedToken)
'            End If
'        End Try
'    End Sub

'#Region "CONSTS"

'    Private Const SW_SHOWNORMAL = 1
'    Private Const STARTF_USESHOWWINDOW As UInt16 = 1

'    Private Const STANDARD_RIGHTS_REQUIRED As UInt32 = &HF0000
'    Private Const STANDARD_RIGHTS_READ As UInt32 = &H20000
'    Private Const TOKEN_ASSIGN_PRIMARY As UInt32 = &H1
'    Private Const TOKEN_DUPLICATE As UInt32 = &H2
'    Private Const TOKEN_IMPERSONATE As UInt32 = &H4
'    Private Const TOKEN_QUERY As UInt32 = &H8
'    Private Const TOKEN_QUERY_SOURCE As UInt32 = &H10
'    Private Const TOKEN_ADJUST_PRIVILEGES As UInt32 = &H20
'    Private Const TOKEN_ADJUST_GROUPS As UInt32 = &H40
'    Private Const TOKEN_ADJUST_DEFAULT As UInt32 = &H80
'    Private Const TOKEN_ADJUST_SESSIONID As UInt32 = &H100
'    Private Const TOKEN_READ As UInt32 = (STANDARD_RIGHTS_READ Or TOKEN_QUERY)
'    Private Const TOKEN_ALL_ACCESS As UInt32 = (STANDARD_RIGHTS_REQUIRED Or TOKEN_ASSIGN_PRIMARY Or TOKEN_DUPLICATE Or TOKEN_IMPERSONATE Or TOKEN_QUERY Or TOKEN_QUERY_SOURCE Or TOKEN_ADJUST_PRIVILEGES Or TOKEN_ADJUST_GROUPS Or TOKEN_ADJUST_DEFAULT Or TOKEN_ADJUST_SESSIONID)

'#End Region 'CONSTS

'#Region "STRUCTURES"

'    <StructLayout(LayoutKind.Sequential)> _
'    Private Structure STARTUPINFO
'        Public cb As Int32
'        Public lpReserved As String
'        Public lpDesktop As String
'        Public lpTitle As String
'        Public dwX As Int32
'        Public dwY As Int32
'        Public dwXSize As Int32
'        Public dwXCountChars As Int32
'        Public dwYCountChars As Int32
'        Public dwFillAttribute As Int32
'        Public dwFlags As Int32
'        Public wShowWindow As Int16
'        Public cbReserved2 As Int16
'        Public lpReserved2 As IntPtr
'        Public hStdInput As IntPtr
'        Public hStdOutput As IntPtr
'        Public hStdError As IntPtr
'    End Structure

'    <StructLayout(LayoutKind.Sequential)> _
'    Private Structure PROCESS_INFORMATION
'        Public hProcess As IntPtr
'        Public hThread As IntPtr
'        Public dwProcessID As Int32
'        Public dwThreadID As Int32
'    End Structure

'    <StructLayout(LayoutKind.Sequential)> _
'    Private Structure SECURITY_ATTRIBUTES
'        Public Length As Int32
'        Public lpSecurityDescriptor As IntPtr
'        Public bInheritHandle As Boolean
'    End Structure

'    Private Enum SECURITY_IMPERSONATION_LEVEL
'        SecurityAnonymous
'        SecurityIdentification
'        SecurityImpersonation
'        SecurityDelegation
'    End Enum

'    Private Enum TOKEN_TYPE
'        TokenPrimary = 1
'        TokenImpersonation
'    End Enum

'#End Region 'STRUCTURES

'#Region "INTEROP"

'    <DllImport("kernel32.dll", EntryPoint:="CloseHandle", SetLastError:=True, CharSet:=CharSet.Auto, CallingConvention:=CallingConvention.StdCall)> _
'    Private Function CloseHandle(ByVal handle As IntPtr) As Boolean
'    End Function
'    <DllImport("advapi32.dll", EntryPoint:="CreateProcessAsUser", SetLastError:=True, CharSet:=CharSet.Ansi, CallingConvention:=CallingConvention.StdCall)> _
'    Private Function CreateProcessAsUser(ByVal hToken As IntPtr, _
'                                               ByVal lpApplicationName As String, _
'                                               ByVal lpCommandLine As String, _
'                                               ByRef lpProcessAttributes As SECURITY_ATTRIBUTES, _
'                                               ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, _
'                                               ByVal bInheritHandle As Boolean, _
'                                               ByVal dwCreationFlags As Int32, _
'                                               ByVal lpEnvrionment As IntPtr, _
'                                               ByVal lpCurrentDirectory As String, _
'                                               ByRef lpStartupInfo As STARTUPINFO, _
'                                               ByRef lpProcessInformation As PROCESS_INFORMATION) As Boolean
'    End Function
'    <DllImport("advapi32.dll", EntryPoint:="DuplicateTokenEx")> _
'    Private Function DuplicateTokenEx(ByVal hExistingToken As IntPtr, ByVal dwDesiredAccess As Int32, ByRef lpThreadAttributes As SECURITY_ATTRIBUTES, ByVal ImpersonationLevel As Int32, ByVal dwTokenType As Int32, ByRef phNewToken As IntPtr) As Integer
'    End Function
'    Private Declare Function WTSGetActiveConsoleSessionId Lib "Kernel32.dll" Alias "WTSGetActiveConsoleSessionId" () As Integer
'    Private Declare Function WTSQueryUserToken Lib "wtsapi32.dll" (ByVal sessionId As UInt32, ByRef Token As IntPtr) As Integer

'#End Region 'INTEROP

'#End Region 'SUCCESSFUL APPROACH

'#Region "OTHER EXPLORATION"

'#Region "CONSTS"

'    'Private Const SW_SHOWNORMAL = 1

'    'Private Const STARTF_USESHOWWINDOW As UInt16 = 1
'    'Private Const STARTF_USESIZE As UInt16 = 2
'    'Private Const STARTF_USEPOSITION As UInt16 = 4
'    'Private Const STARTF_USECOUNTCHARS As UInt16 = 8
'    'Private Const STARTF_USEFILLATTRIBUTE As UInt16 = 10
'    'Private Const STARTF_RUNFULLSCREEN As UInt16 = 20  ' ignored for non-x86 platforms
'    'Private Const STARTF_FORCEONFEEDBACK As UInt16 = 40
'    'Private Const STARTF_FORCEOFFFEEDBACK As UInt16 = 80
'    'Private Const STARTF_USESTDHANDLES As UInt16 = 100

'    'Private Const GENERIC_ALL_ACCESS As Integer = &H10000000

'#End Region 'CONSTS

'#Region "STRUCTURES"

'#End Region 'STRUCTURES

'#Region "INTEROP"

'    <DllImport("advapi32.dll", SetLastError:=True)> _
'    Private Function OpenProcessToken(ByVal ProcessHandle As IntPtr, _
'                                        ByVal DesiredAccess As Integer, _
'                                        ByRef TokenHandle As IntPtr) As Boolean
'    End Function
'    Private Declare Function GetCurrentProcess Lib "kernel32.dll" () As IntPtr
'    Private Declare Function SetTokenInformation Lib "advapi32.dll" (ByVal TokenHandle As Integer, ByRef TokenInformationClass As Short, ByRef TokenInformation As Object, ByVal TokenInformationLength As Integer) As Integer

'#End Region 'INTEROP

'#End Region 'OTHER EXPLORATION

'End Module
