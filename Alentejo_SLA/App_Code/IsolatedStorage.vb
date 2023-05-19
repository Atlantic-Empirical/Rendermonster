Imports System.IO.IsolatedStorage
Imports System.IO

Public Module Iso

    Public ReadOnly Property Storage() As IsolatedStorageFile
        Get
            Return IsolatedStorageFile.GetUserStoreForApplication
        End Get
    End Property

    Public ReadOnly Property FileNames(Optional ByVal Pattern As String = "*") As String()
        Get
            Return Storage.GetFileNames(Pattern)
        End Get
    End Property

    Public ReadOnly Property DirectoryNames(Optional ByVal Pattern As String = "*") As String()
        Get
            Return Storage.GetDirectoryNames(Pattern)
        End Get
    End Property

    Public ReadOnly Property File(ByVal Path As String) As Stream
        Get
            Return Storage.OpenFile(Path, FileMode.OpenOrCreate, FileAccess.ReadWrite)
        End Get
    End Property

    Public ReadOnly Property FileExists(ByVal Path As String) As Boolean
        Get
            Return Storage.FileExists(Path)
        End Get
    End Property

    Public Sub DeleteFile(ByVal Path As String)
        If Storage.FileExists(Path) Then
            Try
                Storage.DeleteFile(Path)
            Catch isex As IsolatedStorageException
                DebugWrite("IsolatedStorage problem in DeleteFile(). Error: " & isex.Message)
                Storage.Remove()
            Catch ex As Exception
                DebugWrite("Problem in DeleteFile(). Error: " & ex.Message)
            End Try
        End If
    End Sub

End Module
