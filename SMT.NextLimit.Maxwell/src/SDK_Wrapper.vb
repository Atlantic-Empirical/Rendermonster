Imports System.IO
Imports System.Runtime.InteropServices

Public Module Maxwell_SDK

    Public Function GetCameraNamesForMXS(ByVal MXSPath As String) As String()
        Try
            'validate/complete the MXSPath
            If Path.HasExtension(MXSPath) Then
                If Path.GetExtension(MXSPath).ToLower = ".mxs" Then
                    If Not File.Exists(MXSPath) Then
                        Throw New Exception("MXS does not exist (" & MXSPath & ").")
                    End If
                Else
                    Throw New Exception("Invalid file type.")
                End If
            Else
                'its a directory
                Dim files() As String = Directory.GetFiles(MXSPath, "*.mxs")
                If files.Length > 1 Then Throw New Exception("More than one MXS at path (" & MXSPath & ").")
                MXSPath = files(0)
            End If
            Dim out As String()
            MXSPath = Replace(MXSPath, "\", "\\")
            GetCameraNames(MXSPath, out)
            Return out
        Catch ex As Exception
            Throw New Exception("Problem with GetCameraNamesForMXS(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Function CallTestMethod(ByVal MXSPath As String) As String()
        Try
            'validate/complete the MXSPath
            If Path.HasExtension(MXSPath) Then
                If Path.GetExtension(MXSPath).ToLower = ".mxs" Then
                    If Not File.Exists(MXSPath) Then
                        Throw New Exception("MXS does not exist (" & MXSPath & ").")
                    End If
                Else
                    Throw New Exception("Invalid file type.")
                End If
            Else
                'its a directory
                Dim files() As String = Directory.GetFiles(MXSPath, "*.mxs")
                If files.Length > 1 Then Throw New Exception("More than one MXS at path (" & MXSPath & ").")
                MXSPath = files(0)
            End If
            Dim out As String()
            MXSPath = Replace(MXSPath, "\", "\\")
            Test(MXSPath, out)
            Return out
        Catch ex As Exception
            Throw New Exception("Problem with CallTestMethod(). Error: " & ex.Message, ex)
        End Try
    End Function

    Private Declare Function GetCameraNames Lib "MaxwellSDKWrapper.dll" (<[In](), MarshalAs(UnmanagedType.LPStr)> ByVal MXSPath As String, <MarshalAs(UnmanagedType.SafeArray, SafeArraySubType:=VarEnum.VT_BSTR)> ByRef CameraNames As String()) As Integer
    Private Declare Function Test Lib "MaxwellSDKWrapper.dll" (<[In](), MarshalAs(UnmanagedType.LPStr)> ByVal MXSPath As String, <MarshalAs(UnmanagedType.SafeArray, SafeArraySubType:=VarEnum.VT_BSTR)> ByRef CameraNames As String()) As Integer

End Module
