Imports System.IO

Public Class cCountries

    Public Countries(-1) As cCountry
    Public CountriesCSVPath As String

    Public Sub New()
        Dim Assm As System.Reflection.Assembly = Me.GetType.Assembly
        Dim CountriesCSV As Stream = Assm.GetManifestResourceStream("SMT.Alentejo_SLA.Countries.csv")
        PopulateCountriesFromStream(CountriesCSV)
    End Sub

    Public Sub New(ByVal PathToCountriesCSV As String)
        Me.CountriesCSVPath = PathToCountriesCSV
        PopulateCountriesFromFile()
    End Sub

    Public Sub New(ByRef CountriesCSV As Stream)
        PopulateCountriesFromStream(CountriesCSV)
    End Sub

    Public Function GetCountryByName(ByVal CountryName As String) As cCountry
        CountryName = Replace(CountryName, "_", " ")
        For Each C As cCountry In Countries
            If C.Name = CountryName Then Return C
        Next
    End Function

    Public Function GetCountryAlpha2(ByVal CountryName As String) As String
        CountryName = Replace(CountryName, "_", " ")
        For Each C As cCountry In Countries
            If C.Name = CountryName Then Return C.Alpha2
        Next
    End Function

    Public Function GetCountryFromAlpha(ByVal A2 As String) As String
        For Each c As cCountry In Countries
            If c.Alpha2 = A2 Then Return c.Name
        Next
    End Function

    Public Function GetCountryNameAlpha2Pairs() As cCountryNameAlpha2Pair()
        Try
            Dim Out(-1) As cCountryNameAlpha2Pair
            Dim CA As cCountryNameAlpha2Pair
            For Each C As cCountry In Countries
                If C.Alpha2 <> "" And C.Alpha2 <> "ISO 3166" Then
                    ReDim Preserve Out(UBound(Out) + 1)
                    CA = New cCountryNameAlpha2Pair
                    CA.Name = C.Name
                    CA.Alpha2 = C.Alpha2
                    Out(UBound(Out)) = CA
                End If
            Next
            Return Out
        Catch ex As Exception
            Throw New Exception("Problem with GetCountryNameAlpha2Pairs(). Error: " & ex.Message)
        End Try
    End Function

    Private Sub AddCountry(ByVal C As cCountry)
        ReDim Preserve Countries(UBound(Countries) + 1)
        Countries(UBound(Countries)) = C
    End Sub

    Private Sub PopulateCountriesFromStream(ByRef CSVStream As Stream)
        Try
            ReDim Countries(-1)
            Dim SR As New StreamReader(CSVStream)
            Dim line As String = SR.ReadLine()
            While Not line Is Nothing
                AddCountry(LineToCountry(line))
                line = SR.ReadLine()
            End While
            SR.Close()
            CSVStream.Close()
        Catch ex As Exception
            Throw New Exception("Problem with PopulateCountriesFromStream(). Error: " & ex.Message)
        End Try
    End Sub

    Private Sub PopulateCountriesFromFile()
        Try
            ReDim Countries(-1)
            Dim FS As New FileStream(CountriesCSVPath, FileMode.Open)
            Dim SR As New StreamReader(FS)
            Dim line As String = SR.ReadLine()
            While Not line Is Nothing
                AddCountry(LineToCountry(line))
                line = SR.ReadLine()
            End While
            SR.Close()
            FS.Close()
        Catch ex As Exception
            Throw New Exception("Problem with PopulateCountriesFromFile(). Error: " & ex.Message)
        End Try
    End Sub

    Private Function LineToCountry(ByVal Line As String) As cCountry
        Try
            Dim C As New cCountry
            Dim L() As String = Split(Line, ",", -1, CompareMethod.Text)
            If L.Length < 8 Then Throw New Exception("hi")
            With C
                .Name = L(0)
                .Alpha2 = L(1)
                .Alpha3 = L(2)
                .Numeric3 = L(3)
                .WindowsCountryRegion = L(4)
                .WindowsCode = L(5)
                .MacName = L(6)
                .MacCode = L(7)
            End With
            Return C
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

End Class

Public Class cCountry

    Public Name As String
    Public Alpha2 As String 'ISO 3166
    Public Alpha3 As String 'ISO 
    Public Numeric3 As String 'UN
    Public WindowsCountryRegion As String
    Public WindowsCode As String
    Public MacName As String
    Public MacCode As String

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

End Class

Public Class cCountryNameAlpha2Pair

    Public Name As String
    Public Alpha2 As String

    Public Sub New()
        Name = ""
        Alpha2 = ""
    End Sub

    Public Overloads Overrides Function ToString() As String
        Return Name
    End Function

End Class
