Public Module GENERAL

    Public Function ComputeCombinedSampleLevel(ByRef MXI_SampleLevels() As Double) As Double
        Try
            Dim inter_total As Double = 0
            For Each sl As Double In MXI_SampleLevels
                inter_total += Math.Exp(sl * Math.Log(1.5)) - 1
            Next
            Dim out As Double = Math.Log(1 + inter_total) / Math.Log(1.5)
            Return Math.Round(out, 3)
        Catch ex As Exception
            Throw New Exception("Problem with ComputeCombineSampleLevel(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Function ComputeCombinedSampleLevel(ByRef MXIPaths() As String) As Double
        Try
            Dim MXI_SampleLevels As New List(Of Double)
            Dim tMXII As cMXIInfo
            For Each p As String In MXIPaths
                tMXII = MXIMERGE.GetMXIInfo(p)
                MXI_SampleLevels.Add(tMXII.SampleLevel)
            Next

            Dim inter_total As Double = 0
            For Each sl As Double In MXI_SampleLevels
                inter_total += Math.Exp(sl * Math.Log(1.5)) - 1
            Next
            Dim out As Double = Math.Log(1 + inter_total) / Math.Log(1.5)
            Return Math.Round(out, 3)
        Catch ex As Exception
            Throw New Exception("Problem with ComputeCombineSampleLevel(). Error: " & ex.Message, ex)
        End Try
    End Function

    ''' <summary>
    ''' Use this method to determine the sample level to assign to each node in a cooperative render, in order to reach an ultimate target value.
    ''' </summary>
    ''' <param name="Nodes"></param>
    ''' <param name="TargetLevel"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ComputeNodeLevelTarget(ByVal Nodes As Byte, ByVal TargetLevel As Double) As Double
        Try
            Dim d1 As Double = Math.Exp(Math.Log(1.5) * TargetLevel) - 1
            Dim d2 As Double = d1 / Nodes
            Dim d3 As Double = Math.Log(d2 + 1) / Math.Log(1.5)
            Return d3

            'OLD - didn't get this approach to work
            'Dim d1 As Double = Math.Exp(TargetLevel * Math.Log(1.5)) - 1
            'Dim d2 As Double = (d1 * Nodes) + 1
            'Dim d3 As Double = Math.Log(d2) / Math.Log(1.5)
            'Return d3
        Catch ex As Exception
            Throw New Exception("Problem with ComputeNodeLevelTarget(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Function ComputeSampleLevelEstimateForNodes(ByVal OneNodeSampleLevel As Double, ByVal NodeCount As Byte) As Double
        Try
            Dim inter_total As Double = (Math.Exp(OneNodeSampleLevel * Math.Log(1.5)) - 1) * NodeCount
            Dim out As Double = Math.Log(1 + inter_total) / Math.Log(1.5)
            Return Math.Round(out, 2)
        Catch ex As Exception
            Throw New Exception("Problem with ComputeSampleLevelEstimateForNodes(). Error: " & ex.Message, ex)
        End Try
    End Function

    Public Function WrapStringInQuotes(ByVal inString As String) As String
        Return Chr(34) & inString & Chr(34)
    End Function

End Module
