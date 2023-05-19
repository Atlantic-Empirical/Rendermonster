Imports SMT.Alentejo.Core.FileManagement
Imports SMT.AWS.SDB
Imports Amazon.SimpleDB.Model

Namespace ApplicationManagement

    Public Class cSMT_ATJ_Application

        Public Id As String
        Public Name As String
        Public PublishDate As String        'ISO 6201 UTC
        Public AutoRun As String 'to be implemented
        'Public Files() As String

#Region "OVERRIDES"

        Public Function ToSDB() As System.Collections.Generic.List(Of ReplaceableAttribute)
            Return SMT_AWS_SDB_ObjectSerialization.Serialize(Me)
        End Function

        Public Shared Function FromSDB(ByVal Attributes As System.Collections.Generic.List(Of Amazon.SimpleDB.Model.Attribute)) As cSMT_ATJ_Application
            Dim p As New cSMT_ATJ_Application
            Return SMT_AWS_SDB_ObjectSerialization.Deserialize(Of cSMT_ATJ_Application)(Attributes, p)
        End Function

#End Region 'OVERRIDES

        Public Overrides Function ToString() As String
            Return Name & " - " & PublishDate & " - " & Id
        End Function

    End Class

    Public Enum eSMT_ATJ_Server
        Centraal = 0
        Render_MX = 1
        Merge_MX = 2
        Render_MR = 3
        Merge_MR = 4
        Render_VR = 5
        Merge_VR = 6
    End Enum

    Public Enum eSMT_ATJ_ServerApp
        JobFileTransferManager = 0
        InstanceDispatcher = 1
        RenderManager_MX = 2
        MergeManager_MX = 3
        LogReader = 4
    End Enum

End Namespace
