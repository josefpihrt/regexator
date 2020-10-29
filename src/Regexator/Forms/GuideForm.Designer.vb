<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GuideForm
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
        Me._spcMain = New Regexator.Windows.Forms.ExtendedSplitContainer()
        CType(Me._spcMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._spcMain.SuspendLayout()
        Me.SuspendLayout()
        '
        '_spcMain
        '
        Me._spcMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me._spcMain.Location = New System.Drawing.Point(0, 0)
        Me._spcMain.Name = "_spcMain"
        Me._spcMain.Size = New System.Drawing.Size(784, 562)
        Me._spcMain.SplitterDistance = 120
        Me._spcMain.SplitterWidth = 5
        Me._spcMain.TabIndex = 0
        Me._spcMain.TabStop = False
        '
        'GuideForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me._spcMain)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.Name = "GuideForm"
        CType(Me._spcMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me._spcMain.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents _spcMain As Regexator.Windows.Forms.ExtendedSplitContainer

End Class
