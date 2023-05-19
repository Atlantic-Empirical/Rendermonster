Imports SMT.Runuh.Services.TestHarness.UserAPI


Public Module WebServiceClients

    Private WebserviceRootUrl As System.Uri = New System.Uri("http://localhost:51606/")

    Public WithEvents wscUserAPI As UserAPISoapClient

    Public Sub SetupWebserviceClients()
        Dim binding As System.ServiceModel.BasicHttpBinding
        Dim address As System.ServiceModel.EndpointAddress

        'ASP.NET SERVICE
        ' SETUP AUTHENTICATION SERVICE CLIENT
        binding = New System.ServiceModel.BasicHttpBinding
        address = New System.ServiceModel.EndpointAddress(New Uri(WebserviceRootUrl, "../User/UserAPI.asmx"))
        wscUserAPI = New UserAPISoapClient(binding, address)

        'WCF SERVICE
        '' SETUP AUTHENTICATION SERVICE CLIENT
        'binding = New System.ServiceModel.BasicHttpBinding
        'address = New System.ServiceModel.EndpointAddress(New Uri(Application.Current.Host.Source, "../Services/Auth.svc"))
        'AuthServiceClient = New AuthenticationServiceClient(binding, address)

        '' SETUP JOB SERVICE CLIENT
        'binding = New System.ServiceModel.BasicHttpBinding
        'address = New System.ServiceModel.EndpointAddress(New Uri(Application.Current.Host.Source, "../Services/AlentejoJobService.asmx"))
        'JobServiceClient = New AlentejoJobServiceSoapClient(binding, address)

#If DEBUG Then
        'JobServiceClient.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(600000)
#End If

    End Sub

End Module
