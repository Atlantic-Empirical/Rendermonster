Imports System.Xml.Serialization
Imports System.IO

Public Class cSMT_ATJ_ClientSideUserProfile

    <XmlAttribute()> _
    Public Username As String

    <XmlAttribute()> _
    Public UserId As String

    <XmlAttribute()> _
    Public PasswordHash As String

    <XmlAttribute()> _
    Public FirstName As String

    <XmlAttribute()> _
    Public LastName As String

    Public Sub New()
    End Sub

    Public Sub New(ByVal nUsername As String, ByVal nPasswordHash As String, ByVal nUserId As String, ByVal nFirstName As String, ByVal nLastName As String)
        Username = nUsername
        PasswordHash = nPasswordHash
        UserId = nUserId
        FirstName = nFirstName
        LastName = nLastName
    End Sub

End Class
