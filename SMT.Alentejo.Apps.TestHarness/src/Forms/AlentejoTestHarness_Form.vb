#Region "IMPORTS"

'Imports SMT.NextLimit.Maxwell
'Imports SMT.AWS.Authorization
'Imports SMT.Alentejo.Credit.Write
Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.Consts
Imports SMT.Alentejo.Core.InstanceManagement
Imports SMT.AWS.EC2
Imports SMT.Alentejo.Core
Imports SMT.Alentejo.Core.SystemwideMessaging
Imports SMT.AWS.S3
Imports System.IO
Imports SMT.Alentejo.Core.FileManagement
Imports SMT.Alentejo.Core.AtjTrace
Imports System.Text
Imports System.Net

#End Region 'IMPORTS

Public Class AlentejoTestHarness_Form

#Region "JOB SUBMISSION"

    Private Sub btnCreateAndStoreJob_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnCreateAndStoreJob.Click
        Try
            Dim j As cSMT_ATJ_RenderJob_Maxwell = CreateSampleMaxwellJob()
            Dim res As String = SaveJob(j)
            Debug.WriteLine(res)
        Catch ex As Exception
            MsgBox("Problem with CreateAndStoreJob(). Error: " & ex.Message)
        End Try
    End Sub

#End Region 'JOB SUBMISSION

    Private Sub btnRetrieveJob_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRetrieveJob.Click
        Try
            Dim Js As List(Of cSMT_ATJ_RenderJob_Maxwell) = GetJobs("Sequoyan")
            Debug.WriteLine("hi")
        Catch ex As Exception
            MsgBox("Problem with RetireveJob(). Error: " & ex.Message, MsgBoxStyle.Exclamation)
        End Try
    End Sub

    Private Sub btnScrap_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScrap.Click
        Try
            'MsgBox(ec2_ami_render)
            RunSmallInstance()
            ComputeCooperativeBenchmark()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub RunSmallInstance()
        Try
            Dim iIds As New List(Of String)
            RunRenderInstances("testJob", 1, True, iIds)
            MsgBox(iIds(0))
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Function ComputeCooperativeBenchmark() As String
        Try
            Dim out As Integer = 0
            out += 93
            Dim tBM As String
            Dim RenderNodes As New List(Of cSMT_ATJ_RenderNodeStatus)
            Dim rn1 As New cSMT_ATJ_RenderNodeStatus("i-93f078fa")
            RenderNodes.Add(rn1)
            For Each rn As cSMT_ATJ_RenderNodeStatus In RenderNodes
                tBM = GetInstanceAttribute(rn.Id, "Benchmark")
                If Not String.IsNullOrEmpty(tBM) AndAlso IsNumeric(tBM) Then
                    out += tBM
                Else
                    'it is "N/A" or hasn't been set yet by render manager on the other instance
                End If
            Next
            Return out
        Catch ex As Exception
            MsgBox("Problem with ComputeCooperativeBenchmark(). Error: " & ex.Message)
            Return "N/A"
        End Try
    End Function

    Private Class cSMT_ATJ_RenderNodeStatus
        Public Id As String
        Public Complete As Boolean
        Public LocalIp As String
        Public MxiPath As String
        Public Sub New(ByVal nId As String)
            Id = nId
            Complete = False
        End Sub
    End Class

    Private Sub btnInstancesAvailable_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInstancesAvailable.Click
        MsgBox(RenderInstancesAvailable)
    End Sub

    Private Sub btnPutFileTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnPutFileTest.Click
        Dim pth As String = "c:\temp\test.txt"
        Dim fs As New FileStream(pth, FileMode.Open, FileAccess.Read)

        Try

            Dim uri As New Uri("http://localhost:51606/uploadMe.ul")
            Dim request As HttpWebRequest = HttpWebRequest.Create(uri)
            request.CookieContainer = New CookieContainer()
            request.Method = WebRequestMethods.Http.Post
            request.ContentLength = fs.Length
            request.ContentType = "text/plain"
            request.Headers.Add("Bucket", "us.seqmt.com")

            Dim rs As Stream = request.GetRequestStream()
            Dim buffer As Byte() = New Byte(1023) {}
            Dim bytesRead As Integer = 0
            While True
                bytesRead = fs.Read(buffer, 0, buffer.Length)
                If bytesRead = 0 Then
                    Exit While
                End If
                rs.Write(buffer, 0, bytesRead)
            End While
            rs.Close()

            Dim res As HttpWebResponse = request.GetResponse()
            Dim ObjectKey As String = res.Cookies("ObjectKey").Value
            res.Close()

            MsgBox(res.StatusDescription & vbNewLine & ObjectKey)


            'Dim pth As String = "c:\temp\test.txt"
            'Dim fs As New FileStream(pth, FileMode.Open, FileAccess.Read)

            'Dim strUrl As String = "http://localhost:51606/FileTransfer/PutFile.ashx"
            'Dim myHttpWebRequest As HttpWebRequest = CType(WebRequest.Create(strUrl), HttpWebRequest)
            'myHttpWebRequest.ContentType = "test"
            'myHttpWebRequest.Method = "POST"
            'myHttpWebRequest.ContentLength = fs.Length

            'Dim rs As Stream = myHttpWebRequest.GetRequestStream()

            'Dim buffer As Byte() = New Byte(1023) {}
            'Dim bytesRead As Integer = 0
            'While True
            '    bytesRead = fs.Read(buffer, 0, buffer.Length)
            '    If bytesRead = 0 Then
            '        Exit While
            '    End If
            '    rs.Write(buffer, 0, bytesRead)
            'End While

            'rs.Close()
            'fs.Close()

            'Dim res As HttpWebResponse = myHttpWebRequest.GetResponse()

            'MsgBox("done.")

        Catch ex As Exception
            MsgBox(ex.Message)
        Finally
            fs.Close()
        End Try
    End Sub



End Class
