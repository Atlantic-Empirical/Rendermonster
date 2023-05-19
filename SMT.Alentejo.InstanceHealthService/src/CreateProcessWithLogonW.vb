Imports System
Imports System.Runtime.InteropServices

'2005.09.21
'translation in VB of the C# code of
'Chris Hand cj_hand@hotmail.com

Public Module CreateProcessWithLogonW

    Public Sub Main()

        'Dim MyPointer As IntPtr = Marshal.AllocHGlobal(4)
        'Marshal.WriteInt32(MyPointer, StdOutputHandle)
        'Dim MyErrorPointer As IntPtr = Marshal.AllocHGlobal(4)
        'Marshal.WriteInt32(MyErrorPointer, StdErrorHandle)
        Dim startupInfo As New StartupInfo
        startupInfo.reserved = Nothing
        startupInfo.flags = startupInfo.flags And Startf_UseStdHandles
        'startupInfo.stdOutput = MyPointer
        'startupInfo.stdError = MyErrorPointer

        'Dim exitCode As System.UInt32 = Convert.ToUInt32(123456)
        Dim processInfo As New ProcessInformation

        Dim command As String = "c:\windows\Notepad.exe"
        Dim user As String = "ThomasPurnellFisher"
        Dim domain As String = System.Environment.MachineName
        Dim password As String = "dante"
        Dim currentDirectory As String = System.IO.Directory.GetCurrentDirectory()

        Try
            CreateProcessWithLogonW(user, domain, password, Convert.ToUInt32(1), _
            command, command, Convert.ToUInt32(0), Convert.ToUInt32(0), _
            currentDirectory, startupInfo, processInfo)
        Catch e As Exception
            Console.WriteLine(e.ToString())
        End Try
    End Sub

    Private Infinite As System.UInt32 = Convert.ToUInt32(&HFFFFFFF)
    Private Startf_UseStdHandles As Int32 = &H100
    Private StdOutputHandle As Int32 = -11
    Private StdErrorHandle As Int32 = -12

    <StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
    Private Structure StartupInfo
        Public cb As Integer
        Public reserved As String
        Public desktop As String
        Public title As String
        Public x As Integer
        Public y As Integer
        Public xSize As Integer
        Public ySize As Integer
        Public xCountChars As Integer
        Public yCountChars As Integer
        Public fillAttribute As Integer
        Public flags As Integer
        Public showWindow As UInt16
        Public reserved2 As UInt16
        Public reserved3 As Byte
        Public stdInput As IntPtr
        Public stdOutput As IntPtr
        Public stdError As IntPtr
    End Structure 'StartupInfo

    Private Structure ProcessInformation
        Public process As IntPtr
        Public thread As IntPtr
        Public processId As Integer
        Public threadId As Integer
    End Structure 'ProcessInformation

    <DllImport("advapi32.dll", SetLastError:=True, CharSet:=CharSet.Unicode)> _
    Private Function CreateProcessWithLogonW(ByVal userName As String, ByVal domain As String, ByVal password As String, ByVal logonFlags As UInt32, ByVal applicationName As String, ByVal commandLine As String, ByVal creationFlags As UInt32, ByVal environment As UInt32, ByVal currentDirectory As String, ByRef startupInfo As StartupInfo, ByRef processInformation As ProcessInformation) As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)> _
    Private Function GetExitCodeProcess(ByVal process As IntPtr, ByRef exitCode As UInt32) As Boolean
    End Function

    <DllImport("Kernel32.dll", SetLastError:=True)> _
    Private Function WaitForSingleObject(ByVal handle As IntPtr, ByVal milliseconds As UInt32) As UInt32
    End Function

    <DllImport("Kernel32.dll", SetLastError:=True)> _
    Private Function GetStdHandle(ByVal handle As IntPtr) As IntPtr
    End Function

    <DllImport("Kernel32.dll", SetLastError:=True)> _
    Private Function CloseHandle(ByVal handle As IntPtr) As Boolean
    End Function

End Module
