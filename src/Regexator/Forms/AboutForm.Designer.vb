<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AboutForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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
        Me._lblName = New System.Windows.Forms.Label()
        Me._lblCopyright = New System.Windows.Forms.Label()
        Me._lblRights = New System.Windows.Forms.Label()
        Me._btnOk = New System.Windows.Forms.Button()
        Me._pbxLogo = New System.Windows.Forms.PictureBox()
        Me._llbWeb = New System.Windows.Forms.LinkLabel()
        Me._lblVersion = New System.Windows.Forms.Label()
        CType(Me._pbxLogo, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        '_lblName
        '
        Me._lblName.AutoSize = True
        Me._lblName.Location = New System.Drawing.Point(79, 16)
        Me._lblName.Margin = New System.Windows.Forms.Padding(5)
        Me._lblName.Name = "_lblName"
        Me._lblName.Size = New System.Drawing.Size(59, 15)
        Me._lblName.TabIndex = 0
        Me._lblName.Text = "Regexator"
        '
        '_lblCopyright
        '
        Me._lblCopyright.AutoSize = True
        Me._lblCopyright.Location = New System.Drawing.Point(79, 66)
        Me._lblCopyright.Margin = New System.Windows.Forms.Padding(5)
        Me._lblCopyright.Name = "_lblCopyright"
        Me._lblCopyright.Size = New System.Drawing.Size(134, 15)
        Me._lblCopyright.TabIndex = 2
        Me._lblCopyright.Text = "© 2012-2014 Josef Pihrt."
        '
        '_lblRights
        '
        Me._lblRights.AutoSize = True
        Me._lblRights.Location = New System.Drawing.Point(79, 91)
        Me._lblRights.Margin = New System.Windows.Forms.Padding(5)
        Me._lblRights.Name = "_lblRights"
        Me._lblRights.Size = New System.Drawing.Size(104, 15)
        Me._lblRights.TabIndex = 3
        Me._lblRights.Text = "All rights reserved."
        '
        '_btnOk
        '
        Me._btnOk.Location = New System.Drawing.Point(268, 165)
        Me._btnOk.Margin = New System.Windows.Forms.Padding(6)
        Me._btnOk.Name = "_btnOk"
        Me._btnOk.Size = New System.Drawing.Size(100, 30)
        Me._btnOk.TabIndex = 5
        Me._btnOk.Text = "OK"
        Me._btnOk.UseVisualStyleBackColor = True
        '
        '_pbxLogo
        '
        Me._pbxLogo.Image = Global.Regexator.My.Resources.Resources.PngRegexator
        Me._pbxLogo.Location = New System.Drawing.Point(16, 16)
        Me._pbxLogo.Margin = New System.Windows.Forms.Padding(10)
        Me._pbxLogo.Name = "_pbxLogo"
        Me._pbxLogo.Size = New System.Drawing.Size(48, 48)
        Me._pbxLogo.TabIndex = 10
        Me._pbxLogo.TabStop = False
        '
        '_llbWeb
        '
        Me._llbWeb.AutoSize = True
        Me._llbWeb.Location = New System.Drawing.Point(79, 116)
        Me._llbWeb.Margin = New System.Windows.Forms.Padding(5)
        Me._llbWeb.Name = "_llbWeb"
        Me._llbWeb.Size = New System.Drawing.Size(111, 15)
        Me._llbWeb.TabIndex = 4
        Me._llbWeb.TabStop = True
        Me._llbWeb.Text = "http://pihrt.net/Regexator"
        '
        '_lblVersion
        '
        Me._lblVersion.AutoSize = True
        Me._lblVersion.Location = New System.Drawing.Point(79, 41)
        Me._lblVersion.Margin = New System.Windows.Forms.Padding(5)
        Me._lblVersion.Name = "_lblVersion"
        Me._lblVersion.Size = New System.Drawing.Size(46, 15)
        Me._lblVersion.TabIndex = 1
        Me._lblVersion.Text = "Version"
        '
        'AboutForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(384, 211)
        Me.Controls.Add(Me._lblVersion)
        Me.Controls.Add(Me._llbWeb)
        Me.Controls.Add(Me._pbxLogo)
        Me.Controls.Add(Me._btnOk)
        Me.Controls.Add(Me._lblRights)
        Me.Controls.Add(Me._lblCopyright)
        Me.Controls.Add(Me._lblName)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.Name = "AboutForm"
        Me.Padding = New System.Windows.Forms.Padding(10)
        Me.Text = "About Regexator"
        CType(Me._pbxLogo, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents _lblName As System.Windows.Forms.Label
    Friend WithEvents _lblCopyright As System.Windows.Forms.Label
 Friend WithEvents _lblRights As System.Windows.Forms.Label
 Friend WithEvents _btnOk As System.Windows.Forms.Button
 Friend WithEvents _pbxLogo As System.Windows.Forms.PictureBox
 Friend WithEvents _llbWeb As System.Windows.Forms.LinkLabel
 Friend WithEvents _lblVersion As System.Windows.Forms.Label
End Class
