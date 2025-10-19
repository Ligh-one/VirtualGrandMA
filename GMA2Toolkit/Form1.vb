Imports System.IO
Imports System.Diagnostics
Imports System.Runtime.InteropServices

Public Class Form1
    ' --- Console allocation (Win32 API) ---
    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function AllocConsole() As Boolean
    End Function

    <DllImport("kernel32.dll", SetLastError:=True)>
    Private Shared Function FreeConsole() As Boolean
    End Function

    Private _backend As IUsbBackend
    Private _isMounted As Boolean = False
    Private _mountedRoot As String = Nothing

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' --- Console toggle from settings ---
        Try
            chkShowConsole.Checked = My.Settings.ShowConsole
        Catch
            chkShowConsole.Checked = False
        End Try

        If chkShowConsole.Checked Then
            SafeAllocConsole()
        End If

        ConsoleWriteLine("=== GMA2Toolkit Console Started ===")
        ConsoleWriteLine($"[{DateTime.Now:HH:mm:ss}] GMA2Toolkit initializing...")

        ' --- Populate backends (only the ones we use) ---
        cmbBackend.Items.Clear()
        cmbBackend.Items.Add("ImDisk USB Backend")
        cmbBackend.Items.Add("Dev VHD Backend (testing)")

        ' --- Restore saved settings ---
        txtImagePath.Text = If(My.Settings.LastImagePath, Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "gma2-usb.img"))
        txtVolumeLabel.Text = If(String.IsNullOrWhiteSpace(My.Settings.LastVolumeLabel), "MA2USB", My.Settings.LastVolumeLabel)

        ' --- Restore backend or auto-pick ---
        If Not String.IsNullOrWhiteSpace(My.Settings.LastBackend) AndAlso cmbBackend.Items.Contains(My.Settings.LastBackend) Then
            cmbBackend.SelectedItem = My.Settings.LastBackend
            If My.Settings.LastBackend.Contains("ImDisk") Then
                SetBackend(BackendKind.DriverBackend) ' ImDisk backend
            Else
                SetBackend(BackendKind.DevVhdBackend)
            End If
        Else
            AutoSelectBackendByPath()
        End If

        ' If ImDisk backend is selected at startup, ensure it's present
        If cmbBackend.SelectedItem?.ToString() = "ImDisk USB Backend" Then
            EnsureImDiskPresentOrInstall()
        End If

        Log("GMA2Toolkit ready.")
        SetUiState()
    End Sub

    ' --- Console checkbox toggle ---
    Private Sub chkShowConsole_CheckedChanged(sender As Object, e As EventArgs) Handles chkShowConsole.CheckedChanged
        Try
            If chkShowConsole.Checked Then
                SafeAllocConsole()
                ConsoleWriteLine("=== Console attached ===")
            Else
                ConsoleWriteLine("=== Console detached ===")
                SafeFreeConsole()
            End If
            My.Settings.ShowConsole = chkShowConsole.Checked
            My.Settings.Save()
        Catch
        End Try
    End Sub

    Private Sub SafeAllocConsole()
        Try
            AllocConsole()
            Console.Title = "GMA2Toolkit Console"

            ' Redirect standard output and error streams to the new console
            Dim stdOut As New StreamWriter(Console.OpenStandardOutput()) With {.AutoFlush = True}
            Console.SetOut(stdOut)
            Dim stdErr As New StreamWriter(Console.OpenStandardError()) With {.AutoFlush = True}
            Console.SetError(stdErr)

            Console.WriteLine("=== Console attached ===")
        Catch ex As Exception
            Debug.WriteLine("Console init failed: " & ex.Message)
        End Try
    End Sub

    Private Sub SafeFreeConsole()
        Try
            Console.WriteLine("=== Console detached ===")
            FreeConsole()
        Catch
        End Try
    End Sub

    Private Sub ConsoleWriteLine(line As String)
        Try
            Console.WriteLine(line)
        Catch
            ' no console attached; ignore
        End Try
    End Sub

    ' === ImDisk detection & install ===
    Private Function IsImDiskInstalled() As Boolean
        Dim sysPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "imdisk.exe")
        Return File.Exists(sysPath)
    End Function

    Private Sub EnsureImDiskPresentOrInstall()
        If IsImDiskInstalled() Then
            Log("ImDisk detected.")
            Return
        End If

        Dim choice = MessageBox.Show(
            "ImDisk Toolkit is required to mount virtual USB images (.img)." & Environment.NewLine &
            "Install ImDisk now?",
            "ImDisk Required",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question)

        If choice <> DialogResult.Yes Then
            Log("ImDisk not installed. ImDisk backend will not work.")
            Return
        End If

        Try
            Dim installerPath = Path.Combine(Application.StartupPath, "install.bat")
            If Not File.Exists(installerPath) Then
                MessageBox.Show("ImDisk installer (imdiskinst.exe) was not found next to the application." & Environment.NewLine &
                                "Please place it beside GMA2Toolkit.exe and try again.",
                                "Installer Missing", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Log("ImDisk installer missing.")
                Return
            End If

            Dim psi As New ProcessStartInfo("cmd.exe", "/c install.bat") With {
             .WorkingDirectory = Application.StartupPath
}
            Process.Start(psi)
            MessageBox.Show("Installing ImDisk... Please wait for the installer to finish, then restart GMA2Toolkit.",
                            "Installing", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Log("Started ImDisk silent installer. Exiting app.")
            Close()
        Catch ex As Exception
            MessageBox.Show("Failed to start ImDisk installer: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Log("Failed to start ImDisk installer: " & ex.Message)
        End Try
    End Sub
    ' === end ImDisk detection & install ===

    ' --- Automatically pick backend by file extension ---
    Private Sub AutoSelectBackendByPath()
        Dim p = txtImagePath.Text.Trim()
        If p.EndsWith(".img", StringComparison.OrdinalIgnoreCase) Then
            cmbBackend.SelectedItem = "ImDisk USB Backend"
            SetBackend(BackendKind.DriverBackend)
            EnsureImDiskPresentOrInstall()
        ElseIf p.EndsWith(".vhd", StringComparison.OrdinalIgnoreCase) Then
            cmbBackend.SelectedItem = "Dev VHD Backend (testing)"
            SetBackend(BackendKind.DevVhdBackend)
        End If
    End Sub

    Private Sub cmbBackend_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbBackend.SelectedIndexChanged
        Select Case cmbBackend.SelectedItem?.ToString()
            Case "ImDisk USB Backend"
                SetBackend(BackendKind.DriverBackend)
                EnsureImDiskPresentOrInstall()
            Case "Dev VHD Backend (testing)"
                SetBackend(BackendKind.DevVhdBackend)
        End Select
        My.Settings.LastBackend = cmbBackend.SelectedItem?.ToString()
        My.Settings.Save()
    End Sub

    Private Sub SetBackend(kind As BackendKind)
        Select Case kind
            Case BackendKind.DriverBackend
                _backend = New DriverBackend()
            Case BackendKind.DevVhdBackend
                _backend = New DevVhdBackend()
        End Select
        _backend.EnsureReady(AddressOf Log)
        Log($"Backend set: {_backend.Name}")
    End Sub

    Private Sub btnBrowseImg_Click(sender As Object, e As EventArgs) Handles btnBrowseImg.Click
        Using sfd As New SaveFileDialog()
            sfd.Filter = "Raw Image (*.img)|*.img|Virtual Disk (*.vhd)|*.vhd|All Files (*.*)|*.*"
            sfd.FileName = txtImagePath.Text
            If sfd.ShowDialog() = DialogResult.OK Then
                txtImagePath.Text = sfd.FileName
                My.Settings.LastImagePath = txtImagePath.Text
                My.Settings.Save()
                AutoSelectBackendByPath()
            End If
        End Using
    End Sub

    Private Sub btnCreateImage_Click(sender As Object, e As EventArgs) Handles btnCreateImage.Click
        Try
            Dim img = txtImagePath.Text
            Dim sizeBytes As Long = CLng(numSizeGB.Value) * 1024L * 1024L * 1024L
            If String.IsNullOrWhiteSpace(img) Then
                Log("Choose an image path.")
                Return
            End If

            If img.EndsWith(".img", StringComparison.OrdinalIgnoreCase) Then
                Using fs As New FileStream(img, FileMode.Create, FileAccess.Write, FileShare.None)
                    fs.SetLength(sizeBytes)
                End Using
                Log($"Created raw image: {img}")
            ElseIf img.EndsWith(".vhd", StringComparison.OrdinalIgnoreCase) Then
                Dim sizeMB As Long = CLng(numSizeGB.Value) * 1024L
                Dim script = $"create vdisk file=""{img}"" maximum={sizeMB} type=fixed{Environment.NewLine}exit"
                Dim tmp = Path.Combine(Path.GetTempPath(), "create_vhd.txt")
                File.WriteAllText(tmp, script)
                ProcessUtils.Run("diskpart.exe", $"/s ""{tmp}""", AddressOf Log, 60000)
                Log($"Created VHD: {img} ({numSizeGB.Value} GB)")
            Else
                Log("Unsupported file extension. Use .img or .vhd.")
                Return
            End If

            Log($"Created image: {img} ({numSizeGB.Value} GB).")
            Log("Note: Formatting FAT32 happens automatically when mounting.")
        Catch ex As Exception
            Log("Create image failed: " & ex.Message)
        End Try
    End Sub

    Private Sub btnMount_Click(sender As Object, e As EventArgs) Handles btnMount.Click
        If Not _backend.EnsureReady(AddressOf Log) Then
            Log("Backend not ready; see log above.")
            Return
        End If

        AutoSelectBackendByPath()
        Dim res = _backend.Mount(txtImagePath.Text, txtVolumeLabel.Text, AddressOf Log)

        If res.Success Then
            _mountedRoot = If(String.IsNullOrEmpty(res.DriveLetter), DetectLikelyMountedDriveRoot(), res.DriveLetter)
            If Not String.IsNullOrEmpty(_mountedRoot) AndAlso Not _mountedRoot.EndsWith("\") Then _mountedRoot &= "\"
            _isMounted = True
        Else
            _mountedRoot = Nothing
            _isMounted = False
        End If

        SetUiState()
        Log(If(res.Success, "Mounted.", $"Mount failed: {res.Message}"))
    End Sub

    Private Sub btnUnmount_Click(sender As Object, e As EventArgs) Handles btnUnmount.Click
        Dim res = _backend.Unmount(AddressOf Log)
        _isMounted = False
        _mountedRoot = Nothing
        SetUiState()
        Log(If(res.Success, "Unmounted.", $"Unmount failed: {res.Message}"))
    End Sub

    Private Sub btnPrepareGma_Click(sender As Object, e As EventArgs) Handles btnPrepareGma.Click
        If String.IsNullOrEmpty(_mountedRoot) Then
            Log("No mounted drive detected. Please mount your image first.")
            Return
        End If
        DriveUtils.EnsureGma2Structure(_mountedRoot)
        Log($"Prepared /gma2 structure on {_mountedRoot}")
    End Sub

    Private Sub btnSanity_Click(sender As Object, e As EventArgs) Handles btnSanity.Click
        Dim found As Boolean = False
        For Each d In DriveInfo.GetDrives()
            Try
                If d.IsReady AndAlso d.DriveType = DriveType.Removable Then
                    Dim fs = d.DriveFormat
                    Dim hasGma2 = Directory.Exists(Path.Combine(d.RootDirectory.FullName, "gma2"))
                    Log($"Drive {d.RootDirectory.FullName}: FS={fs}, Label={d.VolumeLabel}, gma2={(If(hasGma2, "Yes", "No"))}")
                    If String.Equals(fs, "FAT32", StringComparison.OrdinalIgnoreCase) Then found = True
                End If
            Catch ex As Exception
                Log($"Error reading drive: {ex.Message}")
            End Try
        Next
        If Not found Then
            Log("No removable FAT32 drives detected.")
        End If
    End Sub

    Private Sub btnOpen_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If _isMounted AndAlso Not String.IsNullOrEmpty(_mountedRoot) Then
            Process.Start("explorer.exe", _mountedRoot)
        Else
            Log("Nothing mounted to open.")
        End If
    End Sub

    Private Sub txtVolumeLabel_TextChanged(sender As Object, e As EventArgs) Handles txtVolumeLabel.TextChanged
        My.Settings.LastVolumeLabel = txtVolumeLabel.Text
        My.Settings.Save()
    End Sub

    ' --- Console-only logging ---
    Private Sub Log(msg As String)
        ConsoleWriteLine($"[{DateTime.Now:HH:mm:ss}] {msg}")
    End Sub

    Private Sub SetUiState()
        btnMount.Enabled = Not _isMounted
        btnUnmount.Enabled = _isMounted
        btnPrepareGma.Enabled = _isMounted
        btnSanity.Enabled = True
        btnOpen.Enabled = _isMounted
        lblStatus.Text = If(_isMounted, $"Mounted at: {_mountedRoot}", "Not mounted")
    End Sub

    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        Try
            If _isMounted AndAlso _backend IsNot Nothing Then
                Log("Auto-unmounting before exit…")
                _backend.Unmount(AddressOf Log)
            End If
        Catch
        End Try

        Try
            ' If you were showing console only when toggled, nothing to do here.
        Catch
        End Try

        MyBase.OnFormClosing(e)
    End Sub

    Private Function DetectLikelyMountedDriveRoot() As String
        Dim latest As DriveInfo = Nothing
        For Each d In DriveInfo.GetDrives()
            If d.IsReady AndAlso d.DriveType = DriveType.Removable Then
                latest = d
            End If
        Next
        Return If(latest Is Nothing, Nothing, latest.RootDirectory.FullName)
    End Function
End Class
