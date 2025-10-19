' ProcessUtils.vb
Imports System.Diagnostics
Imports System.Text

Public Module ProcessUtils

    Public Structure ProcResult
        Public ExitCode As Integer
        Public Output As String
    End Structure

    Public Function Run(cmd As String,
                        args As String,
                        log As Action(Of String),
                        Optional timeoutMs As Integer = 120000) As ProcResult

        Dim psi As New ProcessStartInfo(cmd, args) With {
            .UseShellExecute = False,
            .RedirectStandardOutput = True,
            .RedirectStandardError = True,
            .CreateNoWindow = True
        }

        Dim sb As New StringBuilder()

        Using p As New Process()
            p.StartInfo = psi

            AddHandler p.OutputDataReceived,
                Sub(senderObj As Object, ev As DataReceivedEventArgs)
                    If ev IsNot Nothing AndAlso ev.Data IsNot Nothing Then
                        sb.AppendLine(ev.Data)
                        log(ev.Data)
                    End If
                End Sub

            AddHandler p.ErrorDataReceived,
                Sub(senderObj As Object, ev As DataReceivedEventArgs)
                    If ev IsNot Nothing AndAlso ev.Data IsNot Nothing Then
                        sb.AppendLine(ev.Data)
                        log(ev.Data)
                    End If
                End Sub

            p.Start()
            p.BeginOutputReadLine()
            p.BeginErrorReadLine()

            If Not p.WaitForExit(timeoutMs) Then
                Try
                    p.Kill()
                Catch
                End Try
                Return New ProcResult With {.ExitCode = -1, .Output = "Timed out"}
            End If

            Return New ProcResult With {.ExitCode = p.ExitCode, .Output = sb.ToString()}
        End Using
    End Function

End Module
