Imports SMT.Alentejo.Credit.Write
Imports SMT.Alentejo.Core.Credit.Read

Public Class Form1

    Private Sub btnEncrypt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEncrypt.Click
        Dim t1 As Long = DateTime.UtcNow.Ticks
        txtCipher.Text = TestMethod_EncryptUserBalance("asdf", txtBalance.Text)
        Dim ts As New TimeSpan(DateTime.UtcNow.Ticks - t1)
        Debug.WriteLine("Encrypt took: " & ts.TotalMilliseconds & "ms")
        'txtCipher.Text = System.Text.Encoding.Unicode.GetString(encryptedData)
        txtBalance.Text = ""
    End Sub

    Private Sub btnDecrypt_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDecrypt.Click
        Dim t1 As Long = DateTime.UtcNow.Ticks
        txtBalance.Text = TestMethod_DecryptUserBalance2(txtCipher.Text)
        Dim ts As New TimeSpan(DateTime.UtcNow.Ticks - t1)
        Debug.WriteLine("Decrypt took: " & ts.TotalMilliseconds & "ms")
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim uid As String
        If txtUserId.Text = "" Then
            uid = "ba54fe10-a2f2-4d92-9631-a9a4f0d3da2e"
        Else
            uid = txtUserId.Text
        End If
        SetUserBalance(uid, txtBalance.Text)
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim uid As String
        If txtUserId.Text = "" Then
            uid = "ba54fe10-a2f2-4d92-9631-a9a4f0d3da2e"
        Else
            uid = txtUserId.Text
        End If
        txtBalance.Text = GetUserBalance(uid)
    End Sub

End Class
