<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        cmbBackend = New ComboBox()
        txtImagePath = New TextBox()
        btnBrowseImg = New Button()
        numSizeGB = New NumericUpDown()
        txtVolumeLabel = New TextBox()
        btnCreateImage = New Button()
        btnPrepareGma = New Button()
        btnMount = New Button()
        btnUnmount = New Button()
        btnSanity = New Button()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        Label4 = New Label()
        btnOpen = New Button()
        lblStatus = New Label()
        chkShowConsole = New CheckBox()
        CType(numSizeGB, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' cmbBackend
        ' 
        cmbBackend.FormattingEnabled = True
        cmbBackend.Location = New Point(97, 6)
        cmbBackend.Name = "cmbBackend"
        cmbBackend.Size = New Size(190, 23)
        cmbBackend.TabIndex = 0
        ' 
        ' txtImagePath
        ' 
        txtImagePath.Location = New Point(97, 33)
        txtImagePath.Name = "txtImagePath"
        txtImagePath.Size = New Size(190, 23)
        txtImagePath.TabIndex = 1
        txtImagePath.Text = "txtImagePath"
        ' 
        ' btnBrowseImg
        ' 
        btnBrowseImg.Location = New Point(293, 32)
        btnBrowseImg.Name = "btnBrowseImg"
        btnBrowseImg.Size = New Size(75, 23)
        btnBrowseImg.TabIndex = 2
        btnBrowseImg.Text = "Browse.."
        btnBrowseImg.UseVisualStyleBackColor = True
        ' 
        ' numSizeGB
        ' 
        numSizeGB.Location = New Point(97, 60)
        numSizeGB.Maximum = New Decimal(New Integer() {64, 0, 0, 0})
        numSizeGB.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        numSizeGB.Name = "numSizeGB"
        numSizeGB.Size = New Size(45, 23)
        numSizeGB.TabIndex = 3
        numSizeGB.Value = New Decimal(New Integer() {2, 0, 0, 0})
        ' 
        ' txtVolumeLabel
        ' 
        txtVolumeLabel.Location = New Point(232, 60)
        txtVolumeLabel.Name = "txtVolumeLabel"
        txtVolumeLabel.Size = New Size(100, 23)
        txtVolumeLabel.TabIndex = 4
        txtVolumeLabel.Text = "MA2USB"
        ' 
        ' btnCreateImage
        ' 
        btnCreateImage.Location = New Point(12, 104)
        btnCreateImage.Name = "btnCreateImage"
        btnCreateImage.Size = New Size(88, 23)
        btnCreateImage.TabIndex = 5
        btnCreateImage.Text = "Create Image"
        btnCreateImage.UseVisualStyleBackColor = True
        ' 
        ' btnPrepareGma
        ' 
        btnPrepareGma.Location = New Point(66, 175)
        btnPrepareGma.Name = "btnPrepareGma"
        btnPrepareGma.Size = New Size(111, 23)
        btnPrepareGma.TabIndex = 6
        btnPrepareGma.Text = "Prepare /gma2"
        btnPrepareGma.UseVisualStyleBackColor = True
        ' 
        ' btnMount
        ' 
        btnMount.Location = New Point(106, 104)
        btnMount.Name = "btnMount"
        btnMount.Size = New Size(75, 23)
        btnMount.TabIndex = 7
        btnMount.Text = "Mount"
        btnMount.UseVisualStyleBackColor = True
        ' 
        ' btnUnmount
        ' 
        btnUnmount.Location = New Point(187, 104)
        btnUnmount.Name = "btnUnmount"
        btnUnmount.Size = New Size(75, 23)
        btnUnmount.TabIndex = 8
        btnUnmount.Text = "Unmount"
        btnUnmount.UseVisualStyleBackColor = True
        ' 
        ' btnSanity
        ' 
        btnSanity.Location = New Point(356, 153)
        btnSanity.Name = "btnSanity"
        btnSanity.Size = New Size(81, 23)
        btnSanity.TabIndex = 9
        btnSanity.Text = "Sanity check"
        btnSanity.UseMnemonic = False
        btnSanity.UseVisualStyleBackColor = True
        ' 
        ' Label1
        ' 
        Label1.AutoSize = True
        Label1.Location = New Point(32, 62)
        Label1.Name = "Label1"
        Label1.Size = New Size(59, 15)
        Label1.TabIndex = 12
        Label1.Text = "Size ( GB):"
        ' 
        ' Label2
        ' 
        Label2.AutoSize = True
        Label2.Location = New Point(33, 9)
        Label2.Name = "Label2"
        Label2.Size = New Size(58, 15)
        Label2.TabIndex = 13
        Label2.Text = " Backend:"
        ' 
        ' Label3
        ' 
        Label3.AutoSize = True
        Label3.Location = New Point(21, 36)
        Label3.Name = "Label3"
        Label3.Size = New Size(70, 15)
        Label3.TabIndex = 14
        Label3.Text = "Image path:"
        ' 
        ' Label4
        ' 
        Label4.AutoSize = True
        Label4.Location = New Point(148, 62)
        Label4.Name = "Label4"
        Label4.Size = New Size(83, 15)
        Label4.TabIndex = 15
        Label4.Text = "Volume name:"
        ' 
        ' btnOpen
        ' 
        btnOpen.Location = New Point(268, 104)
        btnOpen.Name = "btnOpen"
        btnOpen.Size = New Size(75, 23)
        btnOpen.TabIndex = 16
        btnOpen.Text = "Open"
        btnOpen.UseVisualStyleBackColor = True
        ' 
        ' lblStatus
        ' 
        lblStatus.AutoSize = True
        lblStatus.Location = New Point(399, 12)
        lblStatus.Name = "lblStatus"
        lblStatus.Size = New Size(41, 15)
        lblStatus.TabIndex = 17
        lblStatus.Text = "Label5"
        ' 
        ' chkShowConsole
        ' 
        chkShowConsole.AutoSize = True
        chkShowConsole.Location = New Point(498, 105)
        chkShowConsole.Name = "chkShowConsole"
        chkShowConsole.Size = New Size(99, 19)
        chkShowConsole.TabIndex = 18
        chkShowConsole.Text = "Show console"
        chkShowConsole.UseVisualStyleBackColor = True
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(722, 447)
        Controls.Add(chkShowConsole)
        Controls.Add(lblStatus)
        Controls.Add(btnOpen)
        Controls.Add(Label4)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(btnSanity)
        Controls.Add(btnUnmount)
        Controls.Add(btnMount)
        Controls.Add(btnPrepareGma)
        Controls.Add(btnCreateImage)
        Controls.Add(txtVolumeLabel)
        Controls.Add(numSizeGB)
        Controls.Add(btnBrowseImg)
        Controls.Add(txtImagePath)
        Controls.Add(cmbBackend)
        Name = "Form1"
        Text = "Form1"
        CType(numSizeGB, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
        PerformLayout()
    End Sub

    Friend WithEvents cmbBackend As ComboBox
    Friend WithEvents txtImagePath As TextBox
    Friend WithEvents btnBrowseImg As Button
    Friend WithEvents numSizeGB As NumericUpDown
    Friend WithEvents txtVolumeLabel As TextBox
    Friend WithEvents btnCreateImage As Button
    Friend WithEvents btnPrepareGma As Button
    Friend WithEvents btnMount As Button
    Friend WithEvents btnUnmount As Button
    Friend WithEvents btnSanity As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents btnOpen As Button
    Friend WithEvents lblStatus As Label
    Friend WithEvents chkShowConsole As CheckBox

End Class
