Imports Amazon.SimpleDB.Model
Imports SMT.AWS.SDB

Namespace FileManagement

    Public Class cSMT_ATJ_File

#Region "FIELDS"

        Public SCHEMA_VERSION As String = "001"

        Public OwnerId As String
        Public Id As String
        Public FileName As String
        Public JobId As String
        Public Property Type() As eAtjFileType
            Get
                Return _Type
            End Get
            Set(ByVal value As eAtjFileType)
                _Type = value
            End Set
        End Property
        Public _Type As String
        Public Property Size() As String
            Get
                Return CInt(_Size).ToString
            End Get
            Set(ByVal value As String)
                _Size = value.PadLeft(9, "0")
            End Set
        End Property
        Public _Size As String
        Public SampleLevel As String
        Public Timestamp As String
        ''' <summary>
        ''' wXh
        ''' </summary>
        ''' <remarks></remarks>
        Public Resolution As String

        ''' <summary>
        ''' Indicates that the file has been fully uploaded to S3.
        ''' </summary>
        ''' <remarks></remarks>
        Public IsAvailable As String

        Public Property _IsPrimaryExecutable() As Boolean
            Get
                If String.IsNullOrEmpty(_IsPrimaryExecutable) Then
                    Return False
                Else
                    If IsPrimaryExecutable = "True" Then
                        Return True
                    Else
                        Return False
                    End If
                End If
            End Get
            Set(ByVal value As Boolean)
                IsPrimaryExecutable = value.ToString
            End Set
        End Property
        Public IsPrimaryExecutable As String 'Boolean

        Public AppId As String

        Public Url As String

#End Region 'FIELDS

#Region "CONSTRUCTORS"

        Public Sub New()
        End Sub

        Public Sub New(ByVal nId As String, ByVal nFileName As String, ByVal nJobId As String, ByVal nType As eAtjFileType, ByVal nSize As Integer, ByVal nOwnerId As String)
            Id = nId
            FileName = nFileName
            JobId = nJobId
            Type = nType
            Size = nSize
            Timestamp = DateTime.UtcNow.Ticks
            OwnerId = nOwnerId
        End Sub

#End Region 'CONSTRUCTORS

#Region "OVERRIDES"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Amazon.SimpleDB.Model.Attribute)) As cSMT_ATJ_File
            Dim p As New cSMT_ATJ_File
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_File)(Attributes, p)
        End Function

#End Region 'OVERRIDES

        Public Enum eAtjFileType As Byte
            NOT_SPECIFIED = 0
            Model
            Texture
            IntermediateOutputImage
            FinalOutputImage
            MXI
            SystemApplicationFile
        End Enum

        Public Class cSMT_ATJ_File_Sorter
            Implements IComparer(Of cSMT_ATJ_File)

            Public Function Compare(ByVal X As cSMT_ATJ_File, ByVal Y As cSMT_ATJ_File) As Integer Implements IComparer(Of SMT.Alentejo.Core.FileManagement.cSMT_ATJ_File).Compare
                Dim Msg1 As cSMT_ATJ_File = DirectCast(X, cSMT_ATJ_File)
                Dim Msg2 As cSMT_ATJ_File = DirectCast(Y, cSMT_ATJ_File)
                Dim l As Long = Msg1.Timestamp - Msg2.Timestamp
                If l < 0 Then Return -1
                If l = 0 Then Return 0
                If l > 0 Then Return 1
            End Function

        End Class

    End Class

End Namespace
