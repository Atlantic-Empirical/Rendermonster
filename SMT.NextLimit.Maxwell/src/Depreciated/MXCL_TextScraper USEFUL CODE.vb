'Imports System.Runtime.InteropServices
'Imports System.Windows.Forms
'Imports System.Drawing

'Public Module MXCL_TextScraper

'    Public Function ScrapeConsole() As String
'        Try
'            ' FIND THE MAXWELL WINDOW
'            Dim hWnd As IntPtr = FindWindowByCaption(IntPtr.Zero, "Maxwell")

'            ' SHOW IT
'            ShowWindow(hWnd, 1)

'            ' FIND OUT WHERE IT IS
'            Dim r As Rectangle = GetWindowRect(hWnd)

'            ' GO INSIDE THE WINDOW
'            SetCursorPos(r.X + 100, r.Y + 100)

'            ' SIMULATE MOUSE RIGHT CLICK
'            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0)
'            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0)
'            'note: context menu is now showing

'            ' MOVE CURSOR OVER "Select All"
'            SetCursorPos(r.X + 120, r.Y + 144)

'            System.Threading.Thread.Sleep(250)

'            ' LEFT CLICK ON "Select All"
'            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
'            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
'            'note: context menu has now closed

'            ' SIMULATE MOUSE RIGHT CLICK AGAIN
'            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, 0)
'            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0)
'            'note: context menu is now showing

'            ' MOVE CURSOR OVER "Copy"
'            SetCursorPos(r.X + 140, r.Y + 160)

'            System.Threading.Thread.Sleep(250)

'            ' LEFT CLICK ON "Copy"
'            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
'            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)
'            'note: context menu has now closed

'            ' LEFT CLICK AGAIN TO UNSLECT ALL
'            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0)
'            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0)

'            System.Threading.Thread.Sleep(2000)
'            If Clipboard.ContainsText Then
'                Return Clipboard.GetText
'            Else
'                Return ""
'            End If

'        Catch ex As Exception
'            Throw New Exception("Problem with ScrapeConsole(). Error: " & ex.Message, ex)
'        End Try
'    End Function

'    Public Declare Auto Function SetCursorPos Lib "User32.dll" (ByVal X As Integer, ByVal Y As Integer) As Integer
'    Public Declare Sub mouse_event Lib "user32" Alias "mouse_event" (ByVal dwFlags As Integer, ByVal dx As Integer, ByVal dy As Integer, ByVal cButtons As Integer, ByVal dwExtraInfo As Integer)
'    Public Const MOUSEEVENTF_LEFTDOWN = &H2 ' left button down
'    Public Const MOUSEEVENTF_LEFTUP = &H4 ' left button up
'    Public Const MOUSEEVENTF_RIGHTDOWN = &H8 ' right button down
'    Public Const MOUSEEVENTF_RIGHTUP = &H10 ' right button up

'    <DllImport("user32.dll")> _
'    Private Function ShowWindow(ByVal hWnd As IntPtr, ByVal nCmdShow As Integer) As Boolean
'    End Function

'    <DllImport("user32.dll", EntryPoint:="FindWindow", SetLastError:=True, CharSet:=CharSet.Auto)> _
'    Private Function FindWindowByCaption( _
'     ByVal zero As IntPtr, _
'     ByVal lpWindowName As String) As IntPtr
'    End Function

'    Public Declare Function GetWindowRect Lib "User32" Alias "GetWindowRect" (ByVal hWnd As IntPtr, ByRef lpRect As RECT) As Int32
'    Public Function GetWindowRect(ByVal hWnd As IntPtr) As System.Drawing.Rectangle
'        Dim r As New RECT
'        GetWindowRect(hWnd, r)
'        Return New System.Drawing.Rectangle(r.Left, r.Top, r.Right, r.Bottom)
'    End Function

'    <StructLayout(LayoutKind.Sequential)> _
'    Public Structure RECT
'        Public Left As Integer
'        Public Top As Integer
'        Public Right As Integer
'        Public Bottom As Integer
'    End Structure

'End Module
