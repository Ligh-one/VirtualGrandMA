' Backend.DevVhdBackend.vb
Imports System.IO

Public Class DevVhdBackend
    Implements IUsbBackend

    Private _lastImagePath As String
    Private _lastDriveRoot As String

    Public ReadOnly Property Name As String Implements IUsbBackend.Name
        Get
            Return "Dev VHD Backend (testing)"
        End Get
    End Property

    Public Function EnsureReady(log As Action(Of String)) As Boolean Implements IUsbBackend.EnsureReady
        Dim ok = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "diskpart.exe"))
        log(If(ok, "DiskPart found.", "DiskPart not found."))
        Return ok
    End Function

    Public Function Mount(imagePath As String, volumeLabel As String, log As Action(Of String)) As MountResult Implements IUsbBackend.Mount
        If String.IsNullOrWhiteSpace(imagePath) OrElse Not File.Exists(imagePath) Then
            Return MountResult.Fail("Image file not found.")
        End If
        If Not imagePath.EndsWith(".vhd", StringComparison.OrdinalIgnoreCase) Then
            log("Dev VHD Backend requires a .vhd file (not .img / .vhdx).")
            Return MountResult.Fail("Unsupported image type for DEV backend.")
        End If

        ' 1) Snapshot disks BEFORE attach
        Dim preScript = "list disk" & Environment.NewLine & "exit"
        Dim prePath = Path.Combine(Path.GetTempPath(), "pre_listdisk.txt")
        File.WriteAllText(prePath, preScript)
        Dim preRes = ProcessUtils.Run("diskpart.exe", $"/s ""{prePath}""", log, 30000)
        Dim preSet = ParseDiskNumbers(preRes.Output)

        ' 2) Attach the VHD
        Dim attachScript =
$"select vdisk file=""{imagePath}""
attach vdisk noerr
exit"
        Dim attachPath = Path.Combine(Path.GetTempPath(), "attach_vhd.txt")
        File.WriteAllText(attachPath, attachScript)
        Dim attachRes = ProcessUtils.Run("diskpart.exe", $"/s ""{attachPath}""", log, 60000)
        If attachRes.ExitCode <> 0 Then
            Return MountResult.Fail("Attach failed.")
        End If

        ' 3) Snapshot disks AFTER attach
        Dim postScript = "list disk" & Environment.NewLine & "exit"
        Dim postPath = Path.Combine(Path.GetTempPath(), "post_listdisk.txt")
        File.WriteAllText(postPath, postScript)
        Dim postRes = ProcessUtils.Run("diskpart.exe", $"/s ""{postPath}""", log, 30000)
        Dim postSet = ParseDiskNumbers(postRes.Output)

        ' 4) Find the new disk number
        Dim newDisk As Integer = -1
        For Each n In postSet
            If Not preSet.Contains(n) Then
                newDisk = n
                Exit For
            End If
        Next
        If newDisk = -1 Then
            ' Fallback: pick the smallest disk number not 0 with small size (heuristic)
            ' but safer to fail loudly:
            Return MountResult.Fail("Could not identify attached VHD disk number.")
        End If

        ' 5) Create partition + assign a free letter on the NEW disk
        Dim letter As Char = DriveLetterUtils.GetFreeDriveLetter()
        Dim partScript =
$"select disk {newDisk}
online disk noerr
attributes disk clear readonly noerr
convert mbr noerr
create partition primary noerr
format fs=fat32 label=""{volumeLabel}"" quick noerr
assign letter={letter} noerr
exit"

        Dim partPath = Path.Combine(Path.GetTempPath(), "partition_assign.txt")
        File.WriteAllText(partPath, partScript)
        Dim partRes = ProcessUtils.Run("diskpart.exe", $"/s ""{partPath}""", log, 60000)
        If partRes.ExitCode <> 0 Then
            Return MountResult.Fail("Partition/assign letter failed.")
        End If

        _lastImagePath = imagePath
        _lastDriveRoot = $"{letter}:\"
        log($"Mounted {imagePath} -> {_lastDriveRoot} (DEV VHD via DiskPart)")
        Return MountResult.Ok("Mounted DEV VHD.", _lastDriveRoot)
    End Function


    Public Function Unmount(log As Action(Of String)) As MountResult Implements IUsbBackend.Unmount
        If String.IsNullOrWhiteSpace(_lastImagePath) Then
            Return MountResult.Fail("No DEV VHD mounted in this session.")
        End If

        Dim scriptPath = Path.Combine(Path.GetTempPath(), "detachvhd.txt")
        Dim script As String =
$"select vdisk file=""{_lastImagePath}""
detach vdisk
exit"
        File.WriteAllText(scriptPath, script)

        Dim r = ProcessUtils.Run("diskpart.exe", $"/s ""{scriptPath}""", log, 120000)
        If r.ExitCode = 0 Then
            Dim p = _lastImagePath
            _lastImagePath = Nothing
            _lastDriveRoot = Nothing
            Return MountResult.Ok($"Unmounted {p}.")
        End If

        Return MountResult.Fail("DEV VHD unmount failed (try running as admin).")
    End Function

    Private Function ParseDiskNumbers(output As String) As HashSet(Of Integer)
        Dim setNums As New HashSet(Of Integer)
        If String.IsNullOrEmpty(output) Then Return setNums
        ' Parse lines like: "  Disk 1    Online   2048 MB ..."
        For Each line In output.Split({vbCrLf, vbLf}, StringSplitOptions.RemoveEmptyEntries)
            Dim t = line.Trim()
            If t.StartsWith("Disk ", StringComparison.OrdinalIgnoreCase) OrElse t.StartsWith("* Disk ", StringComparison.OrdinalIgnoreCase) Then
                ' Remove optional leading asterisk
                t = t.TrimStart("*"c, " "c)
                ' Now expect "Disk <n>"
                Dim parts = t.Split({" "c}, StringSplitOptions.RemoveEmptyEntries)
                If parts.Length >= 2 AndAlso parts(0).Equals("Disk", StringComparison.OrdinalIgnoreCase) Then
                    Dim n As Integer
                    If Integer.TryParse(parts(1), n) Then setNums.Add(n)
                End If
            End If
        Next
        Return setNums
    End Function

End Class
