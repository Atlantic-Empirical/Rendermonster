
Public Module SessionShared

    Public Function CheckSessionToken(ByVal SessionToken As String, Optional ByVal RequestAdminPermission As Boolean = False) As Boolean
        Try
            'TODO: 
            Return True
        Catch ex As Exception
            Debug.WriteLine("Problem in CheckSessionToken(). Error: " & ex.Message)
            Return False
        End Try
    End Function

End Module
