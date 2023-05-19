Imports SMT.AWS
Imports SMT.Alentejo.Core.SystemSettings
Imports System.IO

Public Module Consts

#Region "LOCAL DEBUGGING"

    Public Const LOCAL_DEBUGGING As Boolean = False
    Public Const LOCAL_RENDERING As Boolean = False
    'Public Const LITE_RENDERING As Boolean = False

#End Region 'LOCAL DEBUGGING

#Region "SYSTEM"

    Public Const system_user_id As String = "9C778511-AFB4-4c2e-BDF3-E3CCA51F3D68"

#End Region 'SYSTEM

#Region "AWS"

    'EC2
    Public Property ec2_ami_render() As String
        Get
            'Return GetSystemSetting("ec2_ami_render")
            'Return _ec2_ami_render

#If DEBUG Then
            'Return _ec2_ami_render_lite
            Return GetSystemSetting("ec2_ami_render_debug")
#Else
            Return GetSystemSetting("ec2_ami_render")
            ''Return _ec2_ami_render
            'If LITE_RENDERING Then
            '    Return _ec2_ami_render_lite
            'Else
            '    Return _ec2_ami_render
            '    Return GetSystemSetting("ec2_ami_render")
            'End If
#End If
        End Get
        Set(ByVal value As String)
            'SetSystemSetting("ec2_ami_render", value)
#If DEBUG Then
            SetSystemSetting("ec2_ami_render_debug", value)
#Else
            SetSystemSetting("ec2_ami_render", value)
#End If
        End Set
    End Property
    'Private _ec2_ami_render As String = "ami-0580676c"
    'Private _ec2_ami_render_lite As String = "ami-5b866132"

    Public Property ec2_render_instance_size() As String
        Get
            'Return GetSystemSetting("ec2_render_instance_size")
            '            Return "m1.large"
#If DEBUG Then
            'Return "m1.small"
            Return GetSystemSetting("ec2_render_instance_size_debug")
#Else
            'Return "m1.large"
            'Return "c1.xlarge"
            'If LITE_RENDERING then
            '    Return "m1.small"
            'Else
            '    Return "c1.xlarge"
            'End if 
            Return GetSystemSetting("ec2_render_instance_size")
#End If
        End Get
        Set(ByVal value As String)
            'SetSystemSetting("ec2_render_instance_size", value)
#If DEBUG Then
            SetSystemSetting("ec2_render_instance_size_debug", value)
#Else
            SetSystemSetting("ec2_render_instance_size", value)
#End If
        End Set
    End Property
    Public Const ec2_user_data As String = ""
    Public Const ec2_security_group As String = "alentejo" 'smt_atj_render 'default
    Public Const ec2_key_name As String = "gsg-keypair"
    Public Property ec2_availability_zone() As String
        Get
            Return "us-east-1b"
            'Return GetSystemSetting("ec2_availability_zone")
        End Get
        Set(ByVal value As String)
            SetSystemSetting("ec2_availability_zone", value)
        End Set
    End Property

    'SQS
    Public Const sqs_Q_copy_job_files_to_s3 As String = "smt_atj_job_files_to_copy"
    Public Const sqs_Q_jobs_needing_instances As String = "smt_atj_jobs_needing_instances"
    Public Const sqs_Q_system_messages As String = "smt_atj_system_exceptions"

    'SDB
    Public Const sdb_domain_jobs As String = "smt_atj_jobs"
    Public Const sdb_domain_job_progress As String = "smt_atj_job_progress"
    Public Const sdb_domain_system_messages As String = "smt_atj_system_messages"
    Public Const sdb_domain_instances As String = "smt_atj_instances"
    Public Const sdb_domain_files As String = "smt_atj_files"
    Public Const sdb_domain_users As String = "smt_atj_users"
    Public Const sdb_domain_system_settings As String = "smt_atj_system_settings"
    Public Const sdb_domain_sessions As String = "smt_atj_sessions"
    Public Const sdb_domain_applications As String = "smt_atj_applications"
    Public Const sdb_domain_application_files As String = "smt_atj_application_files"

    'S3
    Public Const s3_bucket_files_us As String = "atj-files-us.seqmt.com"
    Public Const s3_bucket_files_eu As String = "atj-files-eu.seqmt.com"
    Public Const s3_bucket_appfiles_us As String = "atj-appfiles-us.seqmt.com"
    'Public Const s3_bucket_job_files As String = "smt_atj_job_files"
    'Public Const s3_bucket_render_output_inter As String = "smt_atj_render_inter_files"
    'Public Const s3_bucket_render_output_final As String = "smt_atj_render_final_files"

#End Region 'AWS

#Region "PATHS"

    Public ReadOnly Property ALENTEJO_SERVER_JOB_FILE_STORAGE() As String
        Get
            Dim out As String
            'Return GetSystemSetting("ALENTEJO_SERVER_JOB_FILE_STORAGE")
            Dim path As String = Environment.GetEnvironmentVariable("ALENTEJO_SERVER_JOB_FILE_STORAGE")
            If path Is Nothing Then
                Environment.SetEnvironmentVariable("ALENTEJO_SERVER_JOB_FILE_STORAGE", atj_server_job_file_storage_default_path)
                out = atj_server_job_file_storage_default_path
            Else
                out = path
            End If
            If Not Directory.Exists(out) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property
    Private atj_server_job_file_storage_default_path As String = "C:\ALENTEJO\ALENTEJO_SERVER_JOB_FILE_STORAGE\"

    Public ReadOnly Property ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE() As String
        Get
            Dim out As String
            'Return GetSystemSetting("ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE")
            Dim path As String = Environment.GetEnvironmentVariable("ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE")
            If path Is Nothing Then
                Environment.SetEnvironmentVariable("ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE", atj_server_sample_image_storage_default_path)
                out = atj_server_sample_image_storage_default_path
            Else
                out = path
            End If
            If Not Directory.Exists(out) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property
    Private atj_server_sample_image_storage_default_path As String = "C:\ALENTEJO\ALENTEJO_SERVER_SAMPLE_IMAGE_STORAGE\"

    '''' <summary>
    '''' This constant provides the desired length of time that job files are permitted
    '''' to stay in local storage on the Alentejo server before they are treated
    '''' as orphened and are deleted. Value is in HOURS.
    '''' </summary>
    '''' <remarks></remarks>
    'Public Property atj_local_job_file_storage_persistence() As Double
    '    Get
    '        Return GetSystemSetting("atj_local_job_file_storage_persistence")
    '    End Get
    '    Set(ByVal value As Double)
    '        SetSystemSetting("atj_local_job_file_storage_persistence", value)
    '    End Set
    'End Property
    Public Const atj_local_job_file_storage_persistence As Double = 0.99

    Public ReadOnly Property ALENTEJO_INSTANCE_JOB_FILE_STORAGE() As String
        Get
            Dim out As String
            'Return GetSystemSetting("ALENTEJO_INSTANCE_JOB_FILE_STORAGE")
            Dim path As String = Environment.GetEnvironmentVariable("ALENTEJO_INSTANCE_JOB_FILE_STORAGE")
            If path Is Nothing Then
                Environment.SetEnvironmentVariable("ALENTEJO_INSTANCE_JOB_FILE_STORAGE", atj_instance_job_file_storage_default_path)
                out = atj_instance_job_file_storage_default_path
            Else
                out = path
            End If
            If Not Directory.Exists(out) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property
    Private atj_instance_job_file_storage_default_path As String = "C:\ALENTEJO\ALENTEJO_INSTANCE_JOB_FILE_STORAGE\"

    Public ReadOnly Property ALENTEJO_RENDERMANAGER_EXE_PATH() As String
        Get
            Dim out As String
#If DEBUG Then
            out = "T:\SOURCE_CODE_WORKING\SMT\Solutions\ALENTEJO\SMT.Alentejo.RenderManager\bin\Debug\SMT.Alentejo.RenderManager.exe"
#Else
            out = "C:\Program Files\SMT\Alentejo\Render Manager\SMT.Alentejo.RenderManager.exe"
#End If
            If Not Directory.Exists(Path.GetDirectoryName(out)) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property

    Public ReadOnly Property ALENTEJO_INSTANCEDISPATCHER_EXE_PATH() As String
        Get
            Dim out As String
#If DEBUG Then
            out = "T:\CODE\SMT\Solutions\ALENTEJO\SMT.Alentejo.InstanceDispatcher\bin\Debug\SMT.Alentejo.InstanceDispatcher.exe"
#Else
            out = "C:\Program Files\SMT\Alentejo\Instance Dispatcher\SMT.Alentejo.InstanceDispatcher.exe"
#End If
            If Not Directory.Exists(Path.GetDirectoryName(out)) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property

    Public ReadOnly Property ALENTEJO_JOBFILETRANSFERMANAGER_EXE_PATH() As String
        Get
            Dim out As String
#If DEBUG Then
            out = "T:\CODE\SMT\Solutions\ALENTEJO\SMT.Alentejo.JobFileTransferManager\bin\Debug\SMT.Alentejo.JobFileTransferManager.exe"
#Else
            out = "C:\Program Files\SMT\Alentejo\Job File Transfer Manager\SMT.Alentejo.JobFileTransferManager.exe"
#End If
            If Not Directory.Exists(Path.GetDirectoryName(out)) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property

    Public ReadOnly Property ALENTEJO_RENDER_ENGINE_OUTPUT() As String
        Get
            Dim out As String
            'Return GetSystemSetting("ALENTEJO_RENDER_ENGINE_OUTPUT")
            Dim path As String = Environment.GetEnvironmentVariable("ALENTEJO_RENDER_ENGINE_OUTPUT")
            If path Is Nothing Then
                Environment.SetEnvironmentVariable("ALENTEJO_RENDER_ENGINE_OUTPUT", atj_render_engine_output_default_path)
                out = atj_render_engine_output_default_path
            Else
                out = path
            End If
            If Not Directory.Exists(out) Then Directory.CreateDirectory(out)
            Return out
        End Get
    End Property
    Private atj_render_engine_output_default_path As String = "C:\ALENTEJO\ALENTEJO_RENDER_ENGINE_OUTPUT\"

#End Region ' PATHS

#Region "CONSOLE"

    ''' <summary>
    ''' Returns a simple timestamp for use at the end of loglines as appropriate.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property TS() As String
        Get
            Return " @ " & DateTo_ISO8601()
        End Get
    End Property

#End Region 'CONSOLE

#Region "SERVER HEALTH MONITORS"


#End Region 'SERVER HEALTH MONITORS

End Module
