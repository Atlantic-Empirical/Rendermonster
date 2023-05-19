Imports Amazon.SimpleDB.Model
Imports SMT.AWS.Authorization
Imports SMT.AWS.SDB

Namespace SystemSettings

    Public Module System_Settings

#Region "FIELDS & PROPERTIES"

        Private ReadOnly Property SDBObject() As SMT.AWS.SDB.cSMT_AWS_SDB
            Get
                If _SDBObject Is Nothing Then _SDBObject = New cSMT_AWS_SDB(SMT_AWS_STATIC_AUTH.AccessKeyId, SMT_AWS_STATIC_AUTH.SecretAccessKey)
                Return _SDBObject
            End Get
        End Property
        Private _SDBObject As cSMT_AWS_SDB

#End Region 'FIELDS & PROPERTIES

        Private Const SettingItemName As String = "settings"

        Public Function GetSystemSetting(ByVal Name As String) As String
            Try
                If String.IsNullOrEmpty(Name) Then Throw New Exception("Invalid Setting Name.")
                Return SDBObject.GetItemAttribute(sdb_domain_system_settings, SettingItemName, Name)
            Catch ex As Exception
                Throw New Exception("Problem with GetSystemSetting(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Sub SetSystemSetting(ByVal Name As String, ByVal Value As String)
            Try
                If String.IsNullOrEmpty(Name) Then Throw New Exception("Invalid Name.")
                SDBObject.SetItemAttribute(sdb_domain_system_settings, SettingItemName, Name, Value)
            Catch ex As Exception
                Throw New Exception("Problem with SetSystemSetting(). Error: " & ex.Message, ex)
            End Try
        End Sub

    End Module

End Namespace
