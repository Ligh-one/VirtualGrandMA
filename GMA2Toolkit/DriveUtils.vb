' DriveUtils.vb
Imports System.IO

Public Module DriveUtils
    Public Sub EnsureGma2Structure(rootPath As String)
        Dim gmaRoot = Path.Combine(rootPath, "gma2")
        Dim dirs = {
            gmaRoot,
            Path.Combine(gmaRoot, "shows"),
            Path.Combine(gmaRoot, "shared"),
            Path.Combine(gmaRoot, "library"),
            Path.Combine(gmaRoot, "plugins"),
            Path.Combine(gmaRoot, "importexport")
        }
        For Each d In dirs
            If Not Directory.Exists(d) Then Directory.CreateDirectory(d)
        Next
    End Sub

    Public Function TryGetDriveFormat(driveRoot As String) As String
        Try
            Dim di As New DriveInfo(driveRoot)
            If di.IsReady Then Return di.DriveFormat
        Catch
        End Try
        Return String.Empty
    End Function

    Public Function IsRemovable(driveRoot As String) As Boolean
        Try
            Dim di As New DriveInfo(driveRoot)
            Return di.DriveType = DriveType.Removable
        Catch
            Return False
        End Try
    End Function
End Module
