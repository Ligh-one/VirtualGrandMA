' DriveLetterUtils.vb
Imports System.IO

Public Module DriveLetterUtils
    Public Function GetFreeDriveLetter(Optional startLetter As Char = "U"c) As Char
        Dim used = DriveInfo.GetDrives().Select(Function(d) Char.ToUpperInvariant(d.Name(0))).ToHashSet()

        ' Try letters starting from U to Z first
        For c As Integer = AscW(startLetter) To AscW("Z"c)
            Dim ch As Char = ChrW(c)
            If Not used.Contains(ch) Then Return ch
        Next

        ' Fallback sweep from D to T
        For c As Integer = AscW("D"c) To AscW("T"c)
            Dim ch As Char = ChrW(c)
            If Not used.Contains(ch) Then Return ch
        Next

        ' If somehow everything's taken, return X
        Return "X"c
    End Function
End Module
