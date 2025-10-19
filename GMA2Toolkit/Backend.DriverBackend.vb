' DriverBackend.vb
Imports System.IO

Public Class DriverBackend
    Implements IUsbBackend

    Private _imdiskPath As String
    Private _lastMountLetter As String
    Private _lastImage As String

    Public ReadOnly Property Name As String Implements IUsbBackend.Name
        Get
            Return "ImDisk USB Backend"
        End Get
    End Property

    ' --- Ensure ImDisk CLI exists ---
    Public Function EnsureReady(log As Action(Of String)) As Boolean Implements IUsbBackend.EnsureReady
        Dim paths As String() = {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "imdisk.exe"),
            "C:\Program Files\ImDisk\imdisk.exe",
            "C:\Program Files (x86)\ImDisk\imdisk.exe"
        }

        For Each p In paths
            If File.Exists(p) Then
                _imdiskPath = p
                log($"ImDisk found at: {p}")
                Return True
            End If
        Next

        log("ImDisk not found. Please install ImDisk Toolkit (https://sourceforge.net/projects/imdisk-toolkit/).")
        Return False
    End Function

    ' --- Mount and auto-format FAT32 ---
    Public Function Mount(imagePath As String, volumeLabel As String, log As Action(Of String)) As MountResult Implements IUsbBackend.Mount
        If String.IsNullOrWhiteSpace(_imdiskPath) OrElse Not File.Exists(_imdiskPath) Then
            Return MountResult.Fail("ImDisk CLI not found. Run EnsureReady first.")
        End If
        If String.IsNullOrWhiteSpace(imagePath) OrElse Not File.Exists(imagePath) Then
            Return MountResult.Fail("Image file not found.")
        End If

        Try
            Dim letter As Char = DriveLetterUtils.GetFreeDriveLetter()

            ' FAT32 quick-format parameters passed directly to ImDisk
            Dim fmtParams = $"/fs:fat32 /q /y /v:{volumeLabel}"
            Dim args = $"-a -f ""{imagePath}"" -m {letter}: -o rem -p ""{fmtParams}"""

            log($"Executing: imdisk {args}")
            Dim result = ProcessUtils.Run(_imdiskPath, args, log, 90000)

            If result.ExitCode = 0 Then
                _lastMountLetter = $"{letter}:"
                _lastImage = imagePath
                log($"Mounted {_lastImage} as removable drive {_lastMountLetter} and formatted FAT32.")
                Return MountResult.Ok("Mounted and formatted.", _lastMountLetter)
            Else
                Return MountResult.Fail("ImDisk mount failed: " & result.Output)
            End If

        Catch ex As Exception
            Return MountResult.Fail("Mount error: " & ex.Message)
        End Try
    End Function

    ' --- Unmount ---
    Public Function Unmount(log As Action(Of String)) As MountResult Implements IUsbBackend.Unmount
        If String.IsNullOrEmpty(_lastMountLetter) Then
            Return MountResult.Fail("No mounted drive.")
        End If

        If String.IsNullOrWhiteSpace(_imdiskPath) OrElse Not File.Exists(_imdiskPath) Then
            Return MountResult.Fail("ImDisk CLI not found.")
        End If

        Try
            Dim args = $"-D -m {_lastMountLetter}"
            Dim result = ProcessUtils.Run(_imdiskPath, args, log, 20000)

            If result.ExitCode = 0 Then
                log($"Unmounted {_lastMountLetter}")
                _lastMountLetter = Nothing
                _lastImage = Nothing
                Return MountResult.Ok("Unmounted.")
            Else
                Return MountResult.Fail($"Unmount failed: {result.Output}")
            End If
        Catch ex As Exception
            Return MountResult.Fail("Unmount error: " & ex.Message)
        End Try
    End Function
End Class
