Imports System.IO
Imports System.Xml.Serialization

Public Module Serialization

    Public Sub SerializeObject_XML(ByVal anObject As Object, ByVal aStream As Stream, ByVal CloseStream As Boolean)
        Dim xs As New XmlSerializer(anObject.GetType)
        xs.Serialize(aStream, anObject)
        If CloseStream Then aStream.Close()
    End Sub

    Public Function DeserializeObject_XML(ByVal aType As Type, ByVal aStream As Stream) As Object
        Dim xs As New XmlSerializer(aType)
        Dim out As Object = xs.Deserialize(aStream)
        aStream.Close()
        Return out
    End Function

End Module
