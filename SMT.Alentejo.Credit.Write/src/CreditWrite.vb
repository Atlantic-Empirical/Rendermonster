Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.Encryption
Imports SMT.Alentejo.Core.Credit.Read
Imports System.IO
Imports System.Reflection

Public Module CreditWrite

    Private ReadOnly Property PublicKey() As Asymmetric.PublicKey
        Get
            Dim keyStr As Stream = Assembly.GetAssembly(GetType(CreditWrite)).GetManifestResourceStream("SMT.Alentejo.Credit.Write.publickey.xml")
            Dim sr As New StreamReader(keyStr)
            Dim keyXml As String = sr.ReadToEnd
            Return New Asymmetric.PublicKey(keyXml)
        End Get
    End Property

    Public Sub SetUserBalance(ByVal UserId As String, ByVal NewBalance As Double)
        Try
            'encrypt the balance
            Using a As New Asymmetric()
                Dim EncryptedData As Data = a.Encrypt(New Data(CStr(NewBalance)), PublicKey)
                'now store it
                SetUserAttribute(UserId, "Balance_Encrypted", EncryptedData.Base64)
            End Using
        Catch ex As Exception
            Throw New Exception("Problem with SetUserBalance(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Sub ReduceUserBalance(ByVal UserId As String, ByVal ReduceBy As Double)
        Try
            Dim balance_current As String = GetUserBalance(UserId)

            'encrypt the balance
            Using a As New Asymmetric()
                Dim EncryptedData As Data = a.Encrypt(New Data(CStr(balance_current - ReduceBy)), PublicKey)
                'Dim EncryptedData As Data = a.Encrypt(New Data(balance_current - ReduceBy), PublicKey)

                'now store it
                SetUserAttribute(UserId, "Balance_Encrypted", EncryptedData.Base64)
            End Using
        Catch ex As Exception
            Throw New Exception("Problem with ReduceUserBalance(). Error: " & ex.Message, ex)
        End Try
    End Sub

    Public Function TestMethod_EncryptUserBalance(ByVal UserId As String, ByVal NewBalance As String) As String
        Try
            'encrypt the balance
            Using a As New Asymmetric()
                Dim EncryptedData As Data = a.Encrypt(New Data(NewBalance), PublicKey)
                Return EncryptedData.Base64
            End Using
        Catch ex As Exception
            Throw New Exception("Problem with TestMethod_EncryptUserBalance(). Error: " & ex.Message, ex)
        End Try
    End Function

End Module
