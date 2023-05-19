Imports SMT.Alentejo_SLA.AlentejoJobService
Imports SMT.Alentejo_SLA.AlentejoAuthService

Partial Public Class ucSignup
    Inherits UserControl

    Private NewUser As cSMT_ATJ_User

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub ucSignup_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
        'PopulateWithDummyData()
        Setup()
    End Sub

    Private Sub Setup()

        'COUNTRIES
        Dim c As New cCountries
        cbCountries.Items.Clear()
        For Each cn As cCountry In c.Countries
            cbCountries.Items.Add(cn.Name)
        Next

        For i As Integer = 0 To cbCountries.Items.Count - 1
            If cbCountries.Items(i) = "NETHERLANDS" Then
                'DebugWrite("***FOUND IT***")
                cbCountries.SelectedIndex = i
                Exit For
            End If
        Next

        'TIMEZONE
        Dim ts As TimeSpan = TimeZoneInfo.Local.GetUtcOffset(DateTime.Now)
        Dim str As String = ts.Hours & ":" & ts.Minutes.ToString.PadLeft(2, "0")
        If ts.Hours > 0 Then str = "+" & str
        str = "UTC" & str
        For i As Integer = 0 To cbTimezone.Items.Count - 1
            If cbTimezone.Items.Item(i).ToString = str Then
                cbTimezone.SelectedIndex = i
                Exit For
            End If
        Next

    End Sub

    Private Sub btnSignup_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles btnSignup.Click
        Try
            If Not PageIsValid Then
                MessageBox.Show("Please correct invalid data before submitting.")
                Exit Sub
            End If
            NewUser = New cSMT_ATJ_User
            With NewUser

                .Username = txtUserName.Text
                .PasswordHash = HashPassword(Me.txtPassword.Password)

                .Address1 = Me.txtAddress.Text
                .City = txtCity.Text
                .Company = txtCompany.Text
                .Country = cbCountries.SelectedItem.ToString
                .EmailAddress = txtEmail.Text
                .FirstName = txtFirstName.Text
                .LastLoginIpAddress = IPAddress
                .LastName = txtLastName.Text
                .PayPalAccountEmailAddress = txtEmailForPaypalInvoicing.Text
                .PhoneMobile = txtPhoneM.Text
                .PhoneOffice = txtPhoneO.Text
                .PostalCode = txtPostalCode.Text
                .RenderEngine = cbRenderEngine.SelectedItem.ToString
                .Timezone = cbTimezone.SelectedItem.ToString
                .Website = txtWebsite.Text
                .Created_UTCTicks = DateTime.UtcNow.Ticks
                .LastLogin_UTCTicks = .Created_UTCTicks

                ' PREFERENCES
                .MaxCost = "0"
                .MaxRenderingTime = "360"
                .NumberOfCores = "8"
                .OutputType = ".jpg"
                .RenderLevels = "25"

                'figure out
                .AccountStatus = ""
                .AccountType = ""
                .Address2 = ""
                .Address3 = ""
                .BillingType = ""
                .EmailNotificationLevel = ""
                .State = ""
            End With
            AddHandler AuthServiceClient.SaveUserCompleted, AddressOf SaveUser_Callback
            AuthServiceClient.SaveUserAsync("", NewUser)
        Catch ex As Exception
            MessageBox.Show("Problem submitting user. Error: " & ex.Message)
        End Try
    End Sub

    Private Sub SaveUser_Callback(ByVal sender As Object, ByVal e As SaveUserCompletedEventArgs)
        RemoveHandler AuthServiceClient.SaveUserCompleted, AddressOf SaveUser_Callback
        If Not String.IsNullOrEmpty(e.Result) Then
            User = New cSMT_ATJ_ClientSideUserProfile(NewUser.Username, NewUser.PasswordHash, e.Result, NewUser.FirstName, NewUser.LastName)
            OpenDetail(Me, ePages.USER_Home)
        Else
            MessageBox.Show("problem in creation of user account. contact smt at info@seqmt.com")
        End If
    End Sub

    Private Sub PopulateWithDummyData()
        txtUserName.Text = "sequoyan_" & DateTime.Now.Second
        txtPassword.Password = "dante"
        txtPasswordAgain.Password = "dante"
        txtFirstName.Text = "Thomas"
        txtLastName.Text = "Purnell-Fisher"
        txtCompany.Text = "SMT"
        txtWebsite.Text = "www.seqmt.com"
        txtEmail.Text = "thesequoyan@gmail.com"
        txtPhoneM.Text = "+31 (0)6 11 955 840"
        txtPhoneO.Text = "+1 818 574 8784"
        txtAddress.Text = "Overtoom 51-I"
        txtCity.Text = "Amsterdam"
        txtStateProvince.Text = "Noord-Holland"
        txtPostalCode.Text = "1054 HB"
        For i As Integer = 0 To cbCountries.Items.Count - 1
            If cbCountries.Items(i).ToString = "NETHERLANDS" Then
                cbCountries.SelectedIndex = i
            End If
        Next
        txtEmailForPaypalInvoicing.Text = "thesequoyan@gmail.com"
        txtRendersPerMonth.Text = "10-20"
        cbRenderEngine.SelectedItem = "Maxwell"
    End Sub

#Region "VALIDATION"

#Region "VALIDATION:TRIGGERS"

    Private Sub txtUserName_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtUserName.LostFocus
        AddHandler AuthServiceClient.UsernameExistsCompleted, AddressOf UsernameExists_Callback
        AuthServiceClient.UsernameExistsAsync(txtUserName.Text)
    End Sub

    Private Sub UsernameExists_Callback(ByVal sender As Object, ByVal e As AlentejoAuthService.UsernameExistsCompletedEventArgs)
        RemoveHandler AuthServiceClient.UsernameExistsCompleted, AddressOf UsernameExists_Callback
        If e.Result = True Then
            txtUserName.BorderBrush = New SolidColorBrush(Colors.Red)
            txtUserName.BorderThickness = New Thickness(CDbl(2))
            _UsernameIsValid = False
        Else
            txtUserName.BorderBrush = New SolidColorBrush(Colors.Green)
            txtUserName.BorderThickness = New Thickness(CDbl(2))
            _UsernameIsValid = True
        End If
    End Sub

    Private Sub txtPasswordAgain_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtPasswordAgain.LostFocus
        Validate_Password()
    End Sub

    Private Sub txtAddress_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtAddress.LostFocus
        Validate_Address()
    End Sub

    Private Sub txtCity_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtCity.LostFocus
        Validate_City()
    End Sub

    Private Sub txtCompany_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtCompany.LostFocus
        Validate_Company()
    End Sub

    Private Sub txtEmail_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtEmail.LostFocus
        Validate_Email()
    End Sub

    Private Sub txtEmailForPaypalInvoicing_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtEmailForPaypalInvoicing.LostFocus
        Validate_PayPalEmail()
    End Sub

    Private Sub txtFirstName_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtFirstName.LostFocus
        Validate_FirstName()
    End Sub

    Private Sub txtLastName_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtLastName.LostFocus
        Validate_LastName()
    End Sub

    Private Sub txtPhoneM_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtPhoneM.LostFocus
        Validate_PhoneM()
    End Sub

    Private Sub txtPhoneO_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtPhoneO.LostFocus
        Validate_PhoneO()
    End Sub

    Private Sub txtPostalCode_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtPostalCode.LostFocus
        Validate_PostalCode()
    End Sub

    Private Sub txtWebsite_LostFocus(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles txtWebsite.LostFocus
        Validate_Website()
    End Sub

#End Region 'VALIDATION:TRIGGERS

#Region "VALIDATION:CORE"

    Private ReadOnly Property PageIsValid() As Boolean
        Get
            Dim out As Integer = 0
            If Not Validate_Username() Then out += 1
            If Not Validate_Password() Then out += 1
            If Not Validate_FirstName() Then out += 1
            If Not Validate_LastName() Then out += 1
            If Not Validate_Company() Then out += 1
            If Not Validate_Website() Then out += 1
            If Not Validate_Email() Then out += 1
            If Not Validate_PhoneM() Then out += 1
            If Not Validate_PhoneO() Then out += 1
            If Not Validate_Address() Then out += 1
            If Not Validate_City() Then out += 1
            If Not Validate_PostalCode() Then out += 1
            If Not Validate_PayPalEmail() Then out += 1
            Return out = 0
        End Get
    End Property

    Private Function Validate_Username() As Boolean
        Return _UsernameIsValid
    End Function
    Private _UsernameIsValid As Boolean = False

    Private Function Validate_Password() As Boolean
        If Me.txtPassword.Password <> "" AndAlso (Me.txtPassword.Password = Me.txtPasswordAgain.Password) Then
            SetControlValidationState(txtPasswordAgain, True)
            Return True
        Else
            SetControlValidationState(txtPasswordAgain, False)
            Return False
        End If
    End Function

    Private Function Validate_FirstName() As Boolean
        If Me.txtFirstName.Text <> "" Then
            SetControlValidationState(txtFirstName, True)
            Return True
        Else
            SetControlValidationState(txtFirstName, False)
            Return False
        End If
    End Function

    Private Function Validate_LastName() As Boolean
        If txtLastName.Text <> "" Then
            SetControlValidationState(txtLastName, True)
            Return True
        Else
            SetControlValidationState(txtLastName, False)
            Return False
        End If
    End Function

    Private Function Validate_Company() As Boolean
        If txtCompany.Text <> "" Then
            SetControlValidationState(txtCompany, True)
            Return True
        Else
            SetControlValidationState(txtCompany, False)
            Return False
        End If
    End Function

    Private Function Validate_Website() As Boolean
        If txtWebsite.Text <> "" Then
            SetControlValidationState(txtWebsite, True)
            Return True
        Else
            SetControlValidationState(txtWebsite, False)
            Return False
        End If
    End Function

    Private Function Validate_Email() As Boolean
        If EmailIsValid(txtEmail.Text) Then
            SetControlValidationState(txtEmail, True)
            Return True
        Else
            SetControlValidationState(txtEmail, False)
            Return False
        End If
    End Function

    Private Function Validate_PhoneM() As Boolean
        If txtPhoneM.Text <> "" Then
            SetControlValidationState(txtPhoneM, True)
            Return True
        Else
            SetControlValidationState(txtPhoneM, False)
            Return False
        End If
    End Function

    Private Function Validate_PhoneO() As Boolean
        If txtPhoneO.Text <> "" Then
            SetControlValidationState(txtPhoneO, True)
            Return True
        Else
            SetControlValidationState(txtPhoneO, False)
            Return False
        End If
    End Function

    Private Function Validate_Address() As Boolean
        If txtAddress.Text <> "" Then
            SetControlValidationState(txtAddress, True)
            Return True
        Else
            SetControlValidationState(txtAddress, False)
            Return False
        End If
    End Function

    Private Function Validate_City() As Boolean
        If txtCity.Text <> "" Then
            SetControlValidationState(txtCity, True)
            Return True
        Else
            SetControlValidationState(txtCity, False)
            Return False
        End If
    End Function

    Private Function Validate_PostalCode() As Boolean
        If txtPostalCode.Text <> "" Then
            SetControlValidationState(txtPostalCode, True)
            Return True
        Else
            SetControlValidationState(txtPostalCode, False)
            Return False
        End If
    End Function

    Private Function Validate_PayPalEmail() As Boolean
        If EmailIsValid(txtEmailForPaypalInvoicing.Text) Then
            SetControlValidationState(txtEmailForPaypalInvoicing, True)
            Return True
        Else
            SetControlValidationState(txtEmailForPaypalInvoicing, False)
            Return False
        End If
    End Function

    'Private Function Validate_() As Boolean
    '    If 1 = 1 Then
    '        SetControlValidationState(, True)
    '        Return True
    '    Else
    '        SetControlValidationState(, True)
    '        Return False
    '    End If
    'End Function

#End Region 'CORE

#Region "VALIDATION:REGEX"

    Private Function EmailIsValid(ByVal Address As String) As Boolean
        '\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b
        Dim Expression As New System.Text.RegularExpressions.Regex("\S+@\S+\.\S+")
        Return Expression.IsMatch(txtEmail.Text)
    End Function

#End Region 'VALIDATION:REGEX

#Region "VALIDATION:CONTOL"

    Private Sub SetControlValidationState(ByVal c As Control, ByVal Valid As Boolean)
        If _borderBrush Is Nothing And Not Valid Then
            _borderBrush = c.BorderBrush
            _borderThickness = c.BorderThickness
        End If

        If Valid Then
            c.BorderBrush = _borderBrush
            c.BorderThickness = _borderThickness
        Else
            c.BorderBrush = New SolidColorBrush(Colors.Red)
            c.BorderThickness = New Thickness(CDbl(2))
        End If
    End Sub
    Private _borderBrush As Brush = Nothing
    Private _borderThickness As Thickness = Nothing

#End Region 'VALIDATION:CONTROL

#Region "VALIDATION:OLD"

    Private Sub HandleValidationError(ByVal sender As Object, ByVal e As ValidationErrorEventArgs) Handles Me.BindingValidationError
        Dim control As Control = DirectCast(e.OriginalSource, Control)

        If _borderBrush Is Nothing Then
            _borderBrush = control.BorderBrush
        End If

        Select Case e.Action
            Case ValidationErrorEventAction.Added
                control.BorderBrush = New SolidColorBrush(Colors.Red)
                control.BorderThickness = New Thickness(CDbl(2))
                Exit Select
            Case ValidationErrorEventAction.Removed
                control.BorderBrush = _borderBrush
                control.BorderThickness = New Thickness(CDbl(1))
                Exit Select
            Case Else
                Exit Select
        End Select
    End Sub

#End Region 'VALIDATION:OLD

#End Region 'VALIDATION

End Class

#Region "VALIDATION:PROPERTIES"

Public Class cSMT_ATJ_Validator

    Public Property UserName() As String
        Get
            Return _UserName
        End Get
        Set(ByVal value As String)
            If 1 <> 2 Then
                Throw New Exception("Username is already taken.")
                Exit Property
            End If
            _UserName = value
        End Set
    End Property
    Private _UserName As String = "fewa"



End Class

#End Region 'VALIDATION:PROPERTIES
