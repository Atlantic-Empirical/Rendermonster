Imports System.Runtime.InteropServices
Imports System.Text

Public Module AttachConsole


    '<System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint:="AllocConsole", SetLastError:=True)> _
    'Public Function AllocConsole() As Boolean
    'End Function

    '<System.Runtime.InteropServices.DllImportAttribute("kernel32.dll", EntryPoint:="FreeConsole", SetLastError:=True)> _
    'Public Function FreeConsole() As Boolean
    'End Function

    '<DllImport("kernel32.dll")> _
    'Public Function ReadConsole(ByVal hConsoleInput As IntPtr, <Out()> ByVal lpBuffer As StringBuilder, ByVal nNumberOfCharsToRead As UInteger, ByRef lpNumberOfCharsRead As UInteger, ByVal lpReserved As IntPtr) As Boolean
    'End Function

    ''Private Sub ShowConsole_Click(ByVal sender As Object, ByVal e As EventArgs)
    ''    If Not AllocConsole() Then
    ''        Throw New System.ComponentModel.Win32Exception()
    ''    End If
    ''End Sub

    ''Private Sub HideConsole_Click(ByVal sender As Object, ByVal e As EventArgs)
    ''    If Not FreeConsole() Then
    ''        Throw New System.ComponentModel.Win32Exception()
    ''    End If
    ''End Sub

    ''Private Sub RunProgram_Click(ByVal sender As Object, ByVal e As EventArgs)
    ''    Dim psi As New ProcessStartInfo()
    ''    psi.FileName = "cmd.exe"
    ''    psi.UseShellExecute = False

    ''    Using p As Process = Process.Start(psi)
    ''        p.Dispose()
    ''    End Using
    ''End Sub


















    ''''' <summary> 
    ''''' allocates a new console for the calling process. 
    ''''' </summary> 
    ''''' <returns>If the function succeeds, the return value is nonzero. 
    ''''' If the function fails, the return value is zero. 
    ''''' To get extended error information, call Marshal.GetLastWin32Error.</returns> 
    ''<DllImport("kernel32", SetLastError:=True)> _
    ''Private Function AllocConsole() As Boolean
    ''End Function

    ''''' <summary> 
    ''''' Detaches the calling process from its console 
    ''''' </summary> 
    ''''' <returns>If the function succeeds, the return value is nonzero. 
    ''''' If the function fails, the return value is zero. 
    ''''' To get extended error information, call Marshal.GetLastWin32Error.</returns> 
    ''<DllImport("kernel32", SetLastError:=True)> _
    ''Private Function FreeConsole() As Boolean
    ''End Function

    '''' <summary> 
    '''' Attaches the calling process to the console of the specified process. 
    '''' </summary> 
    '''' <param name="dwProcessId">[in] Identifier of the process, usually will be ATTACH_PARENT_PROCESS</param> 
    '''' <returns>If the function succeeds, the return value is nonzero. 
    '''' If the function fails, the return value is zero. 
    '''' To get extended error information, call Marshal.GetLastWin32Error.</returns> 
    '<DllImport("kernel32.dll", SetLastError:=True)> _
    'Public Function AttachConsole(ByVal dwProcessId As UInteger) As Boolean
    'End Function

    ''''' <summary>Identifies the console of the parent of the current process as the console to be attached. 
    ''''' always pass this with AttachConsole in .NET for stability reasons and mainly because 
    ''''' I have NOT tested interprocess attaching in .NET so dont blame me if it doesnt work! </summary> 
    ''Private Const ATTACH_PARENT_PROCESS As UInt32 = &HFFFFFFFFUI

    ''''' <summary> 
    ''''' calling process is already attached to a console 
    ''''' </summary> 
    ''Private Const ERROR_ACCESS_DENIED As Integer = 5

    ''''' <summary> 
    ''''' Allocate a console if application started from within windows GUI. 
    ''''' Detects the presence of an existing console associated with the application and 
    ''''' attaches itself to it if available. 
    ''''' </summary> 
    ''Public Sub AllocateConsole(Optional ByVal Pid As UInt32 = ATTACH_PARENT_PROCESS)
    ''    ' 
    ''    ' the following should only be used in a non-console application type (C#) 
    ''    ' (since a console is allocated/attached already when you define a console app.. :) ) 
    ''    ' 
    ''    Dim b As Boolean = AttachConsole(Pid)
    ''    If Not b AndAlso Marshal.GetLastWin32Error() = ERROR_ACCESS_DENIED Then
    ''        ' A console was not allocated, so we need to make one. 
    ''        If Not AllocConsole() Then
    ''            Debug.WriteLine("A console could not be allocated, sorry!")
    ''            Throw New Exception("Console Allocation Failed")
    ''        Else
    ''            Console.WriteLine("Is Attached, press a key...")
    ''            Console.ReadKey(True)
    ''            ' you now may use the Console.xxx functions from .NET framework 
    ''            ' and they will work as normal 
    ''        End If

    ''    End If
    ''End Sub

End Module
