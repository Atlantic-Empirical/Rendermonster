Imports SMT.Alentejo_SLA.AlentejoJobService

Public Module [GLOBAL]

#Region "FIELDS"

    Public LOGGEDIN As Boolean = False

    Public CurrentJobId As String
    Public CurrentSessionId As String
    Public IPAddress As String
    Public CurrentBalance As Double

    Public JobForResubmit As cSMT_ATJ_RenderJob_Base
    Public ReadOnly Property JobForResubmit_Maxwell() As cSMT_ATJ_RenderJob_Maxwell
        Get
            Return TryCast(JobForResubmit, cSMT_ATJ_RenderJob_Maxwell)
        End Get
    End Property

#End Region 'FIELDS

#Region "METHODS"

    Public Function HashPassword(ByVal nPassword As String) As String
        Return SimpleHash.ComputeHash(nPassword, "SHA1", New Byte() {0})
        'If Not SimpleHash.VerifyHash(nPassword, "SHA1", out) Then
        '    App.DebugWrite("Hash failed.")
        'End If
        'Return out
    End Function

#End Region 'METHODS

#Region "GREY-OUT CONTROL"

    Public Function GreyOutControl(ByRef FE As Panel) As Rectangle
        Dim r As New Rectangle
        r.Width = FE.ActualWidth
        r.Height = FE.ActualHeight
        r.Fill = New SolidColorBrush(Color.FromArgb(&H33, &HFF, &HFF, &HFF))
        Canvas.SetZIndex(r, 100)
        FE.Children.Add(r)
        Return r
    End Function

    Public Sub GrayOutClear(ByRef P As Panel, ByRef R As Rectangle)
        P.Children.Remove(R)
    End Sub

#End Region 'GREY-OUT CONTROL

End Module
