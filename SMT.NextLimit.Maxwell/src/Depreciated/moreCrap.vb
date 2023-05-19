'Imports System
'Imports System.ComponentModel
'Imports System.Diagnostics
'Imports System.IO
'Imports System.Runtime.InteropServices
'Imports System.Reflection
'Imports System.Security.Principal
'Imports System.Text

'''' 
'''' UserSpecificProcess extends the standard Process object to 
'''' create new processes under a different user than the calling parent process. 
'''' Also, the standard output of the child process redirected back to the parent process. 
'''' This is class is designed to operate inside an ASP.NET web application. 
'''' The assumption is that the calling thread is operating with an impersonated security token. 
'''' The this class will change the imperonated security token to a primary token, 
'''' and call CreateProcessAsUser. 
'''' A System.Diagnostics.Process object will be returned with appropriate properties set. 
'''' To use this function, the following security priviliges need to be set for the ASPNET account 
'''' using the local security policy MMC snap-in. CreateProcessAsUser requirement. 
'''' "Replace a process level token"/SE_ASSIGNPRIMARYTOKEN_NAME/SeAssignPrimaryTokenPrivilege 
'''' "Adjust memory quotas for a process"/SE_INCREASE_QUOTA_NAME/SeIncreaseQuotaPrivilege 
'''' 
'''' This class was designed for .NET 1.1 for operating systems W2k an higher. 
'''' Any other features or platform support can be implemented by using the .NET reflector and 
'''' investigating the Process class. 
'''' 
'Public Class UserSpecificProcess
'    Inherits Process

'    <StructLayout(LayoutKind.Sequential)> _
'    Public Class CreateProcessStartupInfo
'        Public cb As Integer
'        Public lpReserved As String
'        Public lpDesktop As String
'        Public lpTitle As String
'        Public dwX As Integer
'        Public dwY As Integer
'        Public dwXSize As Integer
'        Public dwYSize As Integer
'        Public dwXCountChars As Integer
'        Public dwYCountChars As Integer
'        Public dwFillAttribute As Integer
'        Public dwFlags As Integer
'        Public wShowWindow As Short
'        Public cbReserved2 As Short
'        Public lpReserved2 As IntPtr
'        Public hStdInput As IntPtr
'        Public hStdOutput As IntPtr
'        Public hStdError As IntPtr
'        Public Sub New()
'            Me.cb = Marshal.SizeOf(GetType(CreateProcessStartupInfo))
'            Me.lpReserved = Nothing
'            Me.lpDesktop = Nothing
'            Me.lpTitle = Nothing
'            Me.dwX = 0
'            Me.dwY = 0
'            Me.dwXSize = 0
'            Me.dwYSize = 0
'            Me.dwXCountChars = 0
'            Me.dwYCountChars = 0
'            Me.dwFillAttribute = 0
'            Me.dwFlags = 0
'            Me.wShowWindow = 0
'            Me.cbReserved2 = 0
'            Me.lpReserved2 = IntPtr.Zero
'            Me.hStdInput = IntPtr.Zero
'            Me.hStdOutput = IntPtr.Zero
'            Me.hStdError = IntPtr.Zero
'        End Sub
'    End Class

'    <StructLayout(LayoutKind.Sequential)> _
'    Public Class CreateProcessProcessInformation
'        Public hProcess As IntPtr
'        Public hThread As IntPtr
'        Public dwProcessId As Integer
'        Public dwThreadId As Integer
'        Public Sub New()
'            Me.hProcess = IntPtr.Zero
'            Me.hThread = IntPtr.Zero
'            Me.dwProcessId = 0
'            Me.dwThreadId = 0
'        End Sub
'    End Class

'    <StructLayout(LayoutKind.Sequential)> _
'    Public Class SecurityAttributes
'        Public nLength As Integer
'        Public lpSecurityDescriptor As IntPtr
'        Public bInheritHandle As Boolean
'        Public Sub New()
'            Me.nLength = Marshal.SizeOf(GetType(SecurityAttributes))
'        End Sub
'    End Class

'    Public Declare Auto Function CloseHandle Lib "kernel32.dll" (ByVal handle As HandleRef) As Boolean

'    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
'    Public Shared Function CreateProcess(<MarshalAs(UnmanagedType.LPTStr)> ByVal lpApplicationName As String, ByVal lpCommandLine As StringBuilder, ByVal lpProcessAttributes As SecurityAttributes, ByVal lpThreadAttributes As SecurityAttributes, ByVal bInheritHandles As Boolean, ByVal dwCreationFlags As Integer, _
'    ByVal lpEnvironment As IntPtr, <MarshalAs(UnmanagedType.LPTStr)> ByVal lpCurrentDirectory As String, ByVal lpStartupInfo As CreateProcessStartupInfo, ByVal lpProcessInformation As CreateProcessProcessInformation) As Boolean
'    End Function

'    <DllImport("advapi32.dll", CharSet:=CharSet.Unicode, SetLastError:=True)> _
'    Public Shared Function CreateProcessAsUserW(ByVal token As IntPtr, <MarshalAs(UnmanagedType.LPTStr)> ByVal lpApplicationName As String, <MarshalAs(UnmanagedType.LPTStr)> ByVal lpCommandLine As String, ByVal lpProcessAttributes As SecurityAttributes, ByVal lpThreadAttributes As SecurityAttributes, ByVal bInheritHandles As Boolean, _
'    ByVal dwCreationFlags As Integer, ByVal lpEnvironment As IntPtr, <MarshalAs(UnmanagedType.LPTStr)> ByVal lpCurrentDirectory As String, ByVal lpStartupInfo As CreateProcessStartupInfo, ByVal lpProcessInformation As CreateProcessProcessInformation) As Boolean
'    End Function

'    <DllImport("kernel32.dll", CharSet:=CharSet.Ansi, SetLastError:=True)> _
'    Public Shared Function GetStdHandle(ByVal whichHandle As Integer) As IntPtr
'    End Function

'    <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> _
'    Public Shared Function CreateFile(ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, ByVal lpSecurityAttributes As SecurityAttributes, ByVal dwCreationDisposition As Integer, ByVal dwFlagsAndAttributes As Integer, _
'    ByVal hTemplateFile As HandleRef) As IntPtr
'    End Function

'    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
'    Public Shared Function CreateNamedPipe(ByVal name As String, ByVal openMode As Integer, ByVal pipeMode As Integer, ByVal maxInstances As Integer, ByVal outBufSize As Integer, ByVal inBufSize As Integer, ByVal timeout As Integer, ByVal lpPipeAttributes As SecurityAttributes) As IntPtr
'    End Function

'    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
'    Public Shared Function GetConsoleOutputCP() As Integer
'    End Function

'    <DllImport("advapi32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
'    Public Shared Function DuplicateTokenEx(ByVal hToken As HandleRef, ByVal access As Integer, ByVal tokenAttributes As SecurityAttributes, ByVal impersonationLevel As Integer, ByVal tokenType As Integer, ByRef hNewToken As IntPtr) As Boolean
'    End Function

'    ' WinNT.h ACCESS TYPES 
'    Const GENERIC_ALL As Integer = &H10000000

'    ' WinNT.h enum SECURITY_IMPERSONATION_LEVEL 
'    Const SECURITY_IMPERSONATION As Integer = 2

'    ' WinNT.h TOKEN TYPE 
'    Const TOKEN_PRIMARY As Integer = 1

'    ' WinBase.h 
'    Const STD_INPUT_HANDLE As Integer = -10
'    Const STD_ERROR_HANDLE As Integer = -12

'    ' WinBase.h STARTUPINFO 
'    Const STARTF_USESTDHANDLES As Integer = &H100

'    ' Microsoft.Win23.NativeMethods 
'    Shared INVALID_HANDLE_VALUE As New IntPtr(-1)
'    Public Shared NullHandleRef As New HandleRef(Nothing, IntPtr.Zero)

'    ''' 
'    ''' Starts the process with the security token of the calling thread. 
'    ''' If the security token has a token type of TokenImpersonation, 
'    ''' the token will be duplicated to a primary token before calling 
'    ''' CreateProcessAsUser. 
'    ''' 
'    ''' The process to start. 
'    Public Sub StartAsUser()
'        StartAsUser(WindowsIdentity.GetCurrent().Token)
'    End Sub

'    ''' 
'    ''' Starts the process with the given security token. 
'    ''' If the security token has a token type of TokenImpersonation, 
'    ''' the token will be duplicated to a primary token before calling 
'    ''' CreateProcessAsUser. 
'    ''' 
'    ''' 
'    Public Sub StartAsUser(ByVal userToken As IntPtr)
'        If StartInfo.UseShellExecute Then
'            Throw New InvalidOperationException("can't call this with shell execute")
'        End If

'        ' just assume that the securityToken is of TokenImpersonation and create a primary. 
'        Dim primayUserToken As IntPtr = CreatePrimaryToken(userToken)

'        Dim startupInfo As New CreateProcessStartupInfo()
'        Dim processInformation As New CreateProcessProcessInformation()

'        Dim stdinHandle As IntPtr
'        Dim stdoutReadHandle As IntPtr
'        Dim stdoutWriteHandle As IntPtr = IntPtr.Zero
'        Dim stderrHandle As IntPtr
'        Try
'            stdinHandle = GetStdHandle(STD_INPUT_HANDLE)
'            MyCreatePipe(stdoutReadHandle, stdoutWriteHandle, False)
'            stderrHandle = GetStdHandle(STD_ERROR_HANDLE)

'            'assign handles to startup info 
'            startupInfo.dwFlags = STARTF_USESTDHANDLES
'            startupInfo.hStdInput = stdinHandle
'            startupInfo.hStdOutput = stdoutWriteHandle
'            startupInfo.hStdError = stderrHandle

'            Dim commandLine As String = GetCommandLine()
'            Dim creationFlags As Integer = 0
'            Dim environment As IntPtr = IntPtr.Zero
'            Dim workingDirectory As String = GetWorkingDirectory()

'            ' create the process or fail trying. 
'            If Not CreateProcessAsUserW(primayUserToken, Nothing, commandLine, Nothing, Nothing, True, _
'            creationFlags, environment, workingDirectory, startupInfo, processInformation) Then
'                Throw New Win32Exception()
'            End If
'        Finally
'            ' close thread handle 
'            If processInformation.hThread <> INVALID_HANDLE_VALUE Then
'                CloseHandle(New HandleRef(Me, processInformation.hThread))
'            End If

'            ' close client stdout handle 
'            CloseHandle(New HandleRef(Me, stdoutWriteHandle))
'        End Try

'        ' get reader for standard output from the child 
'        Dim encoding__1 As Encoding = Encoding.GetEncoding(GetConsoleOutputCP())
'        Dim standardOutput As New StreamReader(New FileStream(stdoutReadHandle, FileAccess.Read, True, &H1000, True), encoding__1)

'        ' set this on the object accordingly. 
'        GetType(Process).InvokeMember("standardOutput", BindingFlags.SetField Or BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, Me, New Object() {standardOutput})

'        ' scream if a process wasn't started instead of returning false. 
'        If processInformation.hProcess = IntPtr.Zero Then
'            Throw New Exception("failed to create process")
'        End If

'        ' configure the properties of the Process object correctly 
'        GetType(Process).InvokeMember("SetProcessHandle", BindingFlags.InvokeMethod Or BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, Me, New Object() {processInformation.hProcess})
'        GetType(Process).InvokeMember("SetProcessId", BindingFlags.InvokeMethod Or BindingFlags.NonPublic Or BindingFlags.Instance, Nothing, Me, New Object() {processInformation.dwProcessId})
'    End Sub

'    ''' 
'    ''' Creates a primayToken out of an existing token. 
'    ''' 
'    ''' 
'    Private Function CreatePrimaryToken(ByVal userToken As IntPtr) As IntPtr
'        Dim securityAttributes As New SecurityAttributes()
'        Dim primaryUserToken As IntPtr = IntPtr.Zero
'        If Not DuplicateTokenEx(New HandleRef(Me, userToken), GENERIC_ALL, securityAttributes, SECURITY_IMPERSONATION, TOKEN_PRIMARY, primaryUserToken) Then
'            Throw New Win32Exception()
'        End If
'        Return primaryUserToken
'    End Function

'    ''' 
'    ''' Gets the appropriate commandLine from the process. 
'    ''' 
'    ''' 
'    ''' 
'    Private Function GetCommandLine() As String
'        Dim builder1 As New StringBuilder()
'        Dim text1 As String = StartInfo.FileName.Trim()
'        Dim text2 As String = StartInfo.Arguments
'        Dim flag1 As Boolean = text1.StartsWith("""") AndAlso text1.EndsWith("""")
'        If Not flag1 Then
'            builder1.Append("""")
'        End If
'        builder1.Append(text1)
'        If Not flag1 Then
'            builder1.Append("""")
'        End If
'        If (text2 IsNot Nothing) AndAlso (text2.Length > 0) Then
'            builder1.Append(" ")
'            builder1.Append(text2)
'        End If
'        Return builder1.ToString()
'    End Function

'    ''' 
'    ''' Gets the working directory or returns null if an empty string was found. 
'    ''' 
'    ''' 
'    Private Function GetWorkingDirectory() As String
'        Return If((StartInfo.WorkingDirectory <> String.Empty), StartInfo.WorkingDirectory, Nothing)
'    End Function

'    ''' 
'    ''' A clone of Process.CreatePipe. This is only implemented because reflection with 
'    ''' out parameters are a pain. 
'    ''' Note: This is only finished for w2k and higher machines. 
'    ''' 
'    ''' 
'    ''' 
'    ''' Specifies whether the parent will be performing the writes. 
'    Public Shared Sub MyCreatePipe(ByRef parentHandle As IntPtr, ByRef childHandle As IntPtr, ByVal parentInputs As Boolean)
'        Dim pipename As String = "\\.\pipe\Global\" & Guid.NewGuid().ToString()

'        Dim attributes2 As New SecurityAttributes()
'        attributes2.bInheritHandle = False

'        parentHandle = CreateNamedPipe(pipename, &H40000003, 0, &HFF, &H1000, &H1000, 0, attributes2)
'        If parentHandle = INVALID_HANDLE_VALUE Then
'            Throw New Win32Exception()
'        End If

'        Dim attributes3 As New SecurityAttributes()
'        attributes3.bInheritHandle = True
'        Dim num1 As Integer = &H40000000
'        If parentInputs Then
'            num1 = -2147483648
'        End If
'        childHandle = CreateFile(pipename, num1, 3, attributes3, 3, &H40000080, _
'        NullHandleRef)
'        If childHandle = INVALID_HANDLE_VALUE Then
'            Throw New Win32Exception()
'        End If
'    End Sub

'End Class
