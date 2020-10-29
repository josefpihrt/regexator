<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EulaForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me._rtb = New System.Windows.Forms.RichTextBox()
        Me._pnl = New System.Windows.Forms.Panel()
        Me._btnOk = New System.Windows.Forms.Button()
        Me._btnCancel = New System.Windows.Forms.Button()
        Me._pnl.SuspendLayout()
        Me.SuspendLayout()
        '
        '_rtb
        '
        Me._rtb.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me._rtb.Dock = System.Windows.Forms.DockStyle.Fill
        Me._rtb.Location = New System.Drawing.Point(0, 0)
        Me._rtb.Name = "_rtb"
        Me._rtb.ShowSelectionMargin = True
        Me._rtb.Size = New System.Drawing.Size(452, 287)
        Me._rtb.TabIndex = 0
        Me._rtb.Text = ""
        '
        '_pnl
        '
        Me._pnl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me._pnl.Controls.Add(Me._rtb)
        Me._pnl.Location = New System.Drawing.Point(15, 15)
        Me._pnl.Margin = New System.Windows.Forms.Padding(6)
        Me._pnl.Name = "_pnl"
        Me._pnl.Size = New System.Drawing.Size(454, 289)
        Me._pnl.TabIndex = 1
        '
        '_btnOk
        '
        Me._btnOk.Location = New System.Drawing.Point(257, 316)
        Me._btnOk.Margin = New System.Windows.Forms.Padding(6)
        Me._btnOk.Name = "_btnOk"
        Me._btnOk.Size = New System.Drawing.Size(100, 30)
        Me._btnOk.TabIndex = 1
        Me._btnOk.Text = "I Agree"
        Me._btnOk.UseVisualStyleBackColor = True
        '
        '_btnCancel
        '
        Me._btnCancel.Location = New System.Drawing.Point(369, 316)
        Me._btnCancel.Margin = New System.Windows.Forms.Padding(6)
        Me._btnCancel.Name = "_btnCancel"
        Me._btnCancel.Size = New System.Drawing.Size(100, 30)
        Me._btnCancel.TabIndex = 2
        Me._btnCancel.Text = "Cancel"
        Me._btnCancel.UseVisualStyleBackColor = True
        '
        'EulaForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(484, 361)
        Me.Controls.Add(Me._btnCancel)
        Me.Controls.Add(Me._btnOk)
        Me.Controls.Add(Me._pnl)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.Name = "EulaForm"
        Me.Text = "EULA"
        Me._pnl.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents _rtb As System.Windows.Forms.RichTextBox
    Friend WithEvents _pnl As System.Windows.Forms.Panel
    Friend WithEvents _btnOk As System.Windows.Forms.Button
 Friend WithEvents _btnCancel As System.Windows.Forms.Button
End Class
