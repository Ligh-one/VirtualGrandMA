' Models.vb
Public Enum BackendKind
    DriverBackend      ' ImDisk
    DevVhdBackend
End Enum

Public Class MountResult
    Public Property Success As Boolean
    Public Property Message As String
    Public Property DriveLetter As String ' e.g., "E:\"
    Public Shared Function Ok(msg As String, Optional driveLetter As String = Nothing) As MountResult
        Return New MountResult With {.Success = True, .Message = msg, .DriveLetter = driveLetter}
    End Function
    Public Shared Function Fail(msg As String) As MountResult
        Return New MountResult With {.Success = False, .Message = msg}
    End Function
End Class
