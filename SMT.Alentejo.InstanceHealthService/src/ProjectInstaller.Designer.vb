<System.ComponentModel.RunInstaller(True)> Partial Class ProjectInstaller
    Inherits System.Configuration.Install.Installer

    'Installer overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Component Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Component Designer
    'It can be modified using the Component Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.ServiceProcessInstaller_AlentejoInstanceHealthService = New System.ServiceProcess.ServiceProcessInstaller
        Me.ServiceInstaller_AlentejoInstanceHealthService = New System.ServiceProcess.ServiceInstaller
        '
        'ServiceProcessInstaller_AlentejoInstanceHealthService
        '
        Me.ServiceProcessInstaller_AlentejoInstanceHealthService.Account = System.ServiceProcess.ServiceAccount.LocalSystem
        Me.ServiceProcessInstaller_AlentejoInstanceHealthService.Password = Nothing
        Me.ServiceProcessInstaller_AlentejoInstanceHealthService.Username = Nothing
        '
        'ServiceInstaller_AlentejoInstanceHealthService
        '
        Me.ServiceInstaller_AlentejoInstanceHealthService.Description = "Monitors and maintains the health of an EC2 instance for use by the Alentejo syst" & _
            "em."
        Me.ServiceInstaller_AlentejoInstanceHealthService.DisplayName = "SMT Alentejo Instance Health Service"
        Me.ServiceInstaller_AlentejoInstanceHealthService.ServiceName = "SMT Alentejo Instance Health Service"
        Me.ServiceInstaller_AlentejoInstanceHealthService.StartType = System.ServiceProcess.ServiceStartMode.Automatic
        '
        'ProjectInstaller
        '
        Me.Installers.AddRange(New System.Configuration.Install.Installer() {Me.ServiceProcessInstaller_AlentejoInstanceHealthService, Me.ServiceInstaller_AlentejoInstanceHealthService})

    End Sub
    Friend WithEvents ServiceProcessInstaller_AlentejoInstanceHealthService As System.ServiceProcess.ServiceProcessInstaller
    Friend WithEvents ServiceInstaller_AlentejoInstanceHealthService As System.ServiceProcess.ServiceInstaller

End Class
