Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.Encryption
Imports System.IO
Imports System.Reflection

Public Module CreditRead

    Private ReadOnly Property PrivateKey() As Asymmetric.PrivateKey
        Get
            Dim keyStr As Stream = Assembly.GetAssembly(GetType(CreditRead)).GetManifestResourceStream("SMT.Alentejo.Credit.Read.privatekey.xml")
            Dim sr As New StreamReader(keyStr)
            Dim keyXml As String = sr.ReadToEnd
            Return New Asymmetric.PrivateKey(keyXml)
        End Get
    End Property

    Public Function GetUserBalance(ByVal UserId As String) As Double
        Try
            'look up encrypted user balance
            Dim balString As String = GetUserAttribute(UserId, "Balance_Encrypted")
            Dim balBytes() As Byte = Convert.FromBase64String(balString)

            'now decrypt it
            Dim a As New Asymmetric(2048)
            Dim DecryptedData As Data = a.Decrypt(New Data(balBytes), PrivateKey)
            Return CDbl(DecryptedData.Text)
        Catch ex As Exception
            Throw New Exception("Problem with GetUserBalance(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Function TestMethod_GetUserBalance(ByVal ForTesting As Byte()) As Double
        Try
            Dim a As New Asymmetric(2048)
            Dim DecryptedData As Data = a.Decrypt(New Data(ForTesting), PrivateKey)
            Return CDbl(DecryptedData.Text)
        Catch ex As Exception
            Throw New Exception("Problem with GetUserBalance(). Error: " & ex.Message, ex)
        End Try
    End Function

End Module
