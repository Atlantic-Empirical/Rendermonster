'Imports System.Runtime.InteropServices
'Imports System.ComponentModel
'Imports System.Diagnostics
'Imports System.IO
'Imports System.Reflection
'Imports System.Security.Principal
'Imports System.Text

'Public Module Pipes

'    <DllImport("kernel32.dll", EntryPoint:="PeekNamedPipe", SetLastError:=True)> _
'    Public Function PeekNamedPipe(ByVal handle As IntPtr, ByVal buffer As Byte(), ByVal nBufferSize As UInteger, ByRef bytesRead As UInteger, ByRef bytesAvail As UInteger, ByRef BytesLeftThisMessage As UInteger) As Boolean
'    End Function

'    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
'    Public Function CreateNamedPipe(ByVal name As String, ByVal openMode As Integer, ByVal pipeMode As Integer, ByVal maxInstances As Integer, ByVal outBufSize As Integer, ByVal inBufSize As Integer, ByVal timeout As Integer, ByVal lpPipeAttributes As SecurityAttributes) As IntPtr
'    End Function

'    <DllImport("kernel32.dll", CharSet:=CharSet.Auto)> _
'    Public Function CreateFile(ByVal lpFileName As String, ByVal dwDesiredAccess As Integer, ByVal dwShareMode As Integer, ByVal lpSecurityAttributes As SecurityAttributes, ByVal dwCreationDisposition As Integer, ByVal dwFlagsAndAttributes As Integer, ByVal hTemplateFile As HandleRef) As IntPtr
'    End Function

'    Private INVALID_HANDLE_VALUE As New IntPtr(-1)
'    Public NullHandleRef As New HandleRef(Nothing, IntPtr.Zero)

'    Public Sub MyCreatePipe(ByVal PipeName As String, ByRef parentHandle As IntPtr, ByRef childHandle As IntPtr, ByVal parentInputs As Boolean)
'        'Dim pipename As String = "\\.\pipe\Global\" & Guid.NewGuid().ToString()

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
'        childHandle = CreateFile(pipename, num1, 3, attributes3, 3, &H40000080, NullHandleRef)
'        If childHandle = INVALID_HANDLE_VALUE Then
'            Throw New Win32Exception()
'        End If
'    End Sub

'    <StructLayout(LayoutKind.Sequential)> _
'    Public Class SecurityAttributes
'        Public nLength As Integer
'        Public lpSecurityDescriptor As IntPtr
'        Public bInheritHandle As Boolean
'        Public Sub New()
'            Me.nLength = Marshal.SizeOf(GetType(SecurityAttributes))
'        End Sub
'    End Class

'End Module
