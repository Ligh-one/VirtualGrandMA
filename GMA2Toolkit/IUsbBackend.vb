' IUsbBackend.vb
Public Interface IUsbBackend
    ReadOnly Property Name As String
    ' Create or validate prerequisites; install services/driver if needed.
    Function EnsureReady(log As Action(Of String)) As Boolean
    ' Mount the image as a *Removable USB Mass Storage* device.
    Function Mount(imagePath As String, volumeLabel As String, log As Action(Of String)) As MountResult
    ' Unmount / detach anything this backend mounted.
    Function Unmount(log As Action(Of String)) As MountResult
End Interface
