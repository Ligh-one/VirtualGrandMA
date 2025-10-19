' Backend.LinuxUsbIpBackend.vb
Public Class LinuxUsbIpBackend
    Implements IUsbBackend

    Public ReadOnly Property Name As String Implements IUsbBackend.Name
        Get
            Return "Linux USB/IP Backend"
        End Get
    End Property

    ' Paths to helper scripts you will add later (batch/ps1 on Windows; shell script on Linux VM)
    Private ReadOnly WinAttachScript As String = ""  ' e.g., "C:\GMA2Toolkit\usbip\attach.bat"
    Private ReadOnly WinDetachScript As String = ""  ' e.g., "C:\GMA2Toolkit\usbip\detach.bat"

    Public Function EnsureReady(log As Action(Of String)) As Boolean Implements IUsbBackend.EnsureReady
        If String.IsNullOrWhiteSpace(WinAttachScript) OrElse String.IsNullOrWhiteSpace(WinDetachScript) Then
            log("USB/IP backend not configured. Set WinAttachScript/WinDetachScript paths in LinuxUsbIpBackend.")
            Return False
        End If
        Dim ok = IO.File.Exists(WinAttachScript) AndAlso IO.File.Exists(WinDetachScript)
        log(If(ok, "USB/IP scripts found.", "USB/IP scripts missing."))
        Return ok
    End Function

    Public Function Mount(imagePath As String, volumeLabel As String, log As Action(Of String)) As MountResult Implements IUsbBackend.Mount
        If Not EnsureReady(log) Then Return MountResult.Fail("USB/IP not ready.")
        If Not IO.File.Exists(imagePath) Then Return MountResult.Fail("Image file not found.")

        ' Your attach script is expected to: ensure Linux gadget is running, then usbip attach from Windows.
        Dim r = ProcessUtils.Run(WinAttachScript, $"""{imagePath}"" ""{volumeLabel}""", log)
        If r.ExitCode = 0 Then
            ' Optional: parse attached drive letter from script output if you print one.
            Return MountResult.Ok("Mounted via USB/IP backend.")
        End If
        Return MountResult.Fail("Mount failed via USB/IP backend.")
    End Function

    Public Function Unmount(log As Action(Of String)) As MountResult Implements IUsbBackend.Unmount
        If Not EnsureReady(log) Then Return MountResult.Fail("USB/IP not ready.")
        Dim r = ProcessUtils.Run(WinDetachScript, "", log)
        If r.ExitCode = 0 Then Return MountResult.Ok("Unmounted.")
        Return MountResult.Fail("Unmount failed.")
    End Function
End Class
