Imports SMT.Alentejo.Core.JobManagement
Imports SMT.Alentejo.Core.SystemwideMessaging

Namespace AtjTrace

    Public Class cSMT_ATJ_TRACE

        Private WithEvents AlentejoInstanceHealthEventLog As System.Diagnostics.EventLog
        Private log_name As String = "SMT Alentejo"
        Private log_source As String = "Undefined"

        Public Sub New(ByVal AppName As String)
#If TRACE Then
            log_source = AppName
            AlentejoInstanceHealthEventLog = New System.Diagnostics.EventLog(log_name, ".", log_source)
            'AlentejoInstanceHealthEventLog.SynchronizingObject = Me
            If Not EventLog.Exists(log_name) Or Not EventLog.SourceExists(log_source) Then
                EventLog.CreateEventSource(log_source, log_name)
            End If
#End If
        End Sub

        Public Sub LogMessage(ByVal Msg As String, ByVal EntryType As EventLogEntryType, Optional ByVal SendToAlentejoSystemMessageQueue As Boolean = False, Optional ByVal the_job As cSMT_ATJ_RenderJob_Base = Nothing, Optional ByVal InstanceId As String = Nothing)
#If TRACE Then
            Try
                'WRITE TO LOCAL SYSTEM LOG
                AlentejoInstanceHealthEventLog.WriteEntry(Msg, EntryType)
            Catch ex As Exception
                Try
                    'SEND TO THE ALENTEJO SYSTEM EXCEPTION QUEUE
                    If the_job IsNot Nothing Then
                        SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, the_job.Id, ex.Message, ex.StackTrace)
                    ElseIf InstanceId IsNot Nothing Then
                        SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, InstanceId, ex.Message, ex.StackTrace)
                    End If
                Catch ex1 As Exception
                    'eat it
                End Try
            End Try
            If SendToAlentejoSystemMessageQueue Then
                Try
                    'SEND TO THE ALENTEJO SYSTEM EXCEPTION QUEUE
                    If the_job IsNot Nothing Then
                        SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, the_job.Id, Msg, EntryType.ToString)
                    ElseIf InstanceId IsNot Nothing Then
                        SubmitSystemMessage(System.Reflection.MethodBase.GetCurrentMethod, InstanceId, Msg, EntryType.ToString)
                    End If
                Catch ex As Exception
                    'eat it
                End Try
            End If
#End If
        End Sub

    End Class

End Namespace
