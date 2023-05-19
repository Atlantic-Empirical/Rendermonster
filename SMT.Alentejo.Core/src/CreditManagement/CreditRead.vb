Imports SMT.Alentejo.Core.UserManagement
Imports SMT.Alentejo.Core.Encryption
Imports System.IO
Imports System.Reflection

Namespace Credit.Read

    Public Module BalanceRead

        Private ReadOnly Property PrivateKey() As Asymmetric.PrivateKey
            Get
                Dim keyStr As Stream = Assembly.GetAssembly(GetType(BalanceRead)).GetManifestResourceStream("SMT.Alentejo.Core.privatekey.xml")
                Dim sr As New StreamReader(keyStr)
                Dim keyXml As String = sr.ReadToEnd
                Return New Asymmetric.PrivateKey(keyXml)
            End Get
        End Property

        Public Function GetUserBalance(ByVal UserId As String) As Double
            Try
                'look up encrypted user balance
                Dim Balance_Encrypted As String = GetUserAttribute(UserId, "Balance_Encrypted")

                'now decrypt it
                Using a As New Asymmetric()
                    Dim DecryptedData As Data = a.Decrypt(New Data(Convert.FromBase64String(Balance_Encrypted)), PrivateKey)
                    Return CDbl(DecryptedData.Text)
                End Using
            Catch ex As Exception
                Throw New Exception("Problem with GetUserBalance(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function DecodeBalance(ByRef Balance_Encrypted As String) As Double
            Try
                'now decrypt it
                Using a As New Asymmetric()
                    Dim DecryptedData As Data = a.Decrypt(New Data(Convert.FromBase64String(Balance_Encrypted)), PrivateKey)
                    Return Math.Round(CDbl(DecryptedData.Text), 2)
                End Using

                'Dim balBytes() As Byte = Convert.FromBase64String(Balance_Encrypted)

                ''now decrypt it
                'Dim a As New Asymmetric()
                'Debug.WriteLine("a.Decrypt()")
                'Dim DecryptedData As Data = a.Decrypt(New Data(balBytes), PrivateKey)
                'Return Math.Round(CDbl(DecryptedData.ToString), 2)
            Catch ex As Exception
                Throw New Exception("Problem with DecodeBalance(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function TestMethod_DecryptUserBalance(ByVal ForTesting As Byte()) As Double
            Try
                Using a As New Asymmetric()
                    Dim DecryptedData As Data = a.Decrypt(New Data(ForTesting), PrivateKey)
                    Return CDbl(DecryptedData.Text)
                End Using
            Catch ex As Exception
                Throw New Exception("Problem with TestMethod_DecryptUserBalance(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function TestMethod_DecryptUserBalance2(ByVal b64s As String) As Double
            Try
                Using a As New Asymmetric()
                    Dim DecryptedData As Data = a.Decrypt(New Data(Convert.FromBase64String(b64s)), PrivateKey)
                    Return CDbl(DecryptedData.Text)
                End Using
            Catch ex As Exception
                Throw New Exception("Problem with TestMethod_DecryptUserBalance(). Error: " & ex.Message, ex)
            End Try
        End Function

        Public Function Charges_FriendlyString(ByVal UnFriendlyString As String) As String
            If String.IsNullOrEmpty(UnFriendlyString) Then Return "0.00"
            If InStr(UnFriendlyString, ".") = 0 Then
                Return UnFriendlyString & ".00"
            Else
                Dim s() As String = Split(UnFriendlyString, ".")
                If s(1).Length < 2 Then
                    Return UnFriendlyString & "0"
                Else
                    Return UnFriendlyString
                End If
            End If
        End Function

    End Module

End Namespace
