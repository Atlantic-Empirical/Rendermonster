Imports SMT.Alentejo_SLA.AlentejoJobService
'Imports SMT.Alentejo_SLA.AuthService
Imports SMT.Alentejo_SLA.AlentejoAuthService

Public Module WebServiceClients

    Public WithEvents JobServiceClient As AlentejoJobService.AlentejoJobServiceSoapClient
    'Public WithEvents AuthServiceClient As AuthService.AuthenticationServiceClient
    Public WithEvents AuthServiceClient As AlentejoAuthService.AlentejoAuthServiceSoapClient

    Public Sub SetupWebserviceClients()
        Dim binding As System.ServiceModel.BasicHttpBinding
        Dim address As System.ServiceModel.EndpointAddress

        'ASP.NET SERVICE
        ' SETUP AUTHENTICATION SERVICE CLIENT
        binding = New System.ServiceModel.BasicHttpBinding
        address = New System.ServiceModel.EndpointAddress(New Uri(Application.Current.Host.Source, "../Services/AlentejoAuthService.asmx"))
        AuthServiceClient = New AlentejoAuthServiceSoapClient(binding, address)

        'WCF SERVICE
        '' SETUP AUTHENTICATION SERVICE CLIENT
        'binding = New System.ServiceModel.BasicHttpBinding
        'address = New System.ServiceModel.EndpointAddress(New Uri(Application.Current.Host.Source, "../Services/Auth.svc"))
        'AuthServiceClient = New AuthenticationServiceClient(binding, address)

        ' SETUP JOB SERVICE CLIENT
        binding = New System.ServiceModel.BasicHttpBinding
        address = New System.ServiceModel.EndpointAddress(New Uri(Application.Current.Host.Source, "../Services/AlentejoJobService.asmx"))
        JobServiceClient = New AlentejoJobServiceSoapClient(binding, address)
#If DEBUG Then
        'JobServiceClient.InnerChannel.OperationTimeout = TimeSpan.FromMilliseconds(600000)
#End If

    End Sub

End Module
