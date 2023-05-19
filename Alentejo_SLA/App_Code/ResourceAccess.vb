Imports System
Imports System.Windows.Resources
Imports System.IO
Imports System.Windows.Media.Imaging


Public Class ResourceHelper

    Public Shared ReadOnly Property ExecutingAssemblyName() As String
        Get
            Dim name As String = Reflection.Assembly.GetExecutingAssembly.FullName
            Return name.Substring(0, name.IndexOf(","c))
        End Get
    End Property

    Public Shared Function GetStream(ByVal relativeUri As String, ByVal assemblyName As String) As Stream
        Dim res As StreamResourceInfo = Application.GetResourceStream(New Uri(assemblyName & ";component/" & relativeUri, UriKind.Relative))
        If res Is Nothing Then
            res = Application.GetResourceStream(New Uri(relativeUri, UriKind.Relative))
        End If
        If res IsNot Nothing Then
            Return res.Stream
        Else
            Return Nothing
        End If
    End Function

    Public Shared Function GetStream(ByVal relativeUri As String) As Stream
        Return GetStream(relativeUri, ExecutingAssemblyName)
    End Function

    Public Shared Function GetBitmap(ByVal relativeUri As String, ByVal assemblyName As String) As BitmapImage
        Dim s As Stream = GetStream(relativeUri, assemblyName)
        If s Is Nothing Then Return Nothing
        Using s
            Dim bmp As New BitmapImage
            bmp.SetSource(s)
            Return bmp
        End Using
    End Function

    Public Shared Function GetBitmap(ByVal relativeUri As String) As BitmapImage
        Return GetBitmap(relativeUri, ExecutingAssemblyName)
    End Function

    Public Shared Function GetFontSource(ByVal relativeUri As String, ByVal assemblyName As String) As FontSource
        Dim s As Stream = GetStream(relativeUri, assemblyName)
        If s Is Nothing Then Return Nothing
        Using s
            Return New FontSource(s)
        End Using
    End Function

    Public Shared Function GetFontSource(ByVal relativeUri As String) As FontSource
        Dim out As FontSource = GetFontSource(relativeUri, ExecutingAssemblyName)
        Return out
    End Function

    Public Shared Function GetString(ByVal relativeUri As String, ByVal assemblyName As String) As String
        Dim s As Stream = GetStream(relativeUri, assemblyName)
        If s Is Nothing Then Return Nothing
        Using reader As New IO.StreamReader(s)
            Return reader.ReadToEnd
        End Using
    End Function

    Public Shared Function GetString(ByVal relativeUri As String) As String
        Return GetString(relativeUri, ExecutingAssemblyName)
    End Function

    Public Shared Function GetXamlObject(ByVal relativeUri As String, ByVal assemblyName As String) As Object
        Dim str As String = GetString(relativeUri, assemblyName)
        If str = Nothing Then Return Nothing
        Dim obj As Object = System.Windows.Markup.XamlReader.Load(str)
        Return obj
    End Function

    Public Shared Function GetXamlObject(ByVal relativeUri As String) As Object
        Return GetXamlObject(relativeUri, ExecutingAssemblyName)
    End Function

End Class
