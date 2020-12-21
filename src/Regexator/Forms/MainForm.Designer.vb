<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainForm
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
        Me._spc = New Regexator.Windows.Forms.ExtendedSplitContainer()
        Me._pnlRegex = New System.Windows.Forms.Panel()
        Me._spcRegex = New Regexator.Windows.Forms.ExtendedSplitContainer()
        Me._spcPattern = New Regexator.Windows.Forms.ExtendedSplitContainer()
        Me._spcInputOutput = New Regexator.Windows.Forms.ExtendedSplitContainer()
        Me._stsMain = New System.Windows.Forms.StatusStrip()
        Me._tslMode = New System.Windows.Forms.ToolStripStatusLabel()
        Me._tslGroups = New System.Windows.Forms.ToolStripStatusLabel()
        Me._tslEncoding = New System.Windows.Forms.ToolStripStatusLabel()
        Me._tslEmpty = New System.Windows.Forms.ToolStripStatusLabel()
        Me._mnsMain = New Regexator.MainMenuStrip()
        Me._tspMain = New Regexator.MainToolStrip()
        CType(Me._spc, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._spc.Panel2.SuspendLayout()
        Me._spc.SuspendLayout()
        Me._pnlRegex.SuspendLayout()
        CType(Me._spcRegex, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._spcRegex.Panel1.SuspendLayout()
        Me._spcRegex.Panel2.SuspendLayout()
        Me._spcRegex.SuspendLayout()
        CType(Me._spcPattern, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._spcPattern.SuspendLayout()
        CType(Me._spcInputOutput, System.ComponentModel.ISupportInitialize).BeginInit()
        Me._spcInputOutput.SuspendLayout()
        Me._stsMain.SuspendLayout()
        Me.SuspendLayout()
        '
        '_spc
        '
        Me._spc.Dock = System.Windows.Forms.DockStyle.Fill
        Me._spc.Location = New System.Drawing.Point(1, 49)
        Me._spc.Name = "_spc"
        '
        '_spc.Panel2
        '
        Me._spc.Panel2.Controls.Add(Me._pnlRegex)
        Me._spc.Size = New System.Drawing.Size(782, 512)
        Me._spc.SplitterDistance = 157
        Me._spc.SplitterWidth = 5
        Me._spc.TabIndex = 2
        Me._spc.TabStop = False
        '
        '_pnlRegex
        '
        Me._pnlRegex.Controls.Add(Me._spcRegex)
        Me._pnlRegex.Controls.Add(Me._stsMain)
        Me._pnlRegex.Dock = System.Windows.Forms.DockStyle.Fill
        Me._pnlRegex.Location = New System.Drawing.Point(0, 0)
        Me._pnlRegex.Name = "_pnlRegex"
        Me._pnlRegex.Size = New System.Drawing.Size(620, 512)
        Me._pnlRegex.TabIndex = 0
        '
        '_spcRegex
        '
        Me._spcRegex.Dock = System.Windows.Forms.DockStyle.Fill
        Me._spcRegex.Location = New System.Drawing.Point(0, 0)
        Me._spcRegex.Name = "_spcRegex"
        Me._spcRegex.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        '_spcRegex.Panel1
        '
        Me._spcRegex.Panel1.Controls.Add(Me._spcPattern)
        '
        '_spcRegex.Panel2
        '
        Me._spcRegex.Panel2.Controls.Add(Me._spcInputOutput)
        Me._spcRegex.Size = New System.Drawing.Size(620, 490)
        Me._spcRegex.SplitterDistance = 197
        Me._spcRegex.SplitterWidth = 5
        Me._spcRegex.TabIndex = 5
        Me._spcRegex.TabStop = False
        '
        '_spcPattern
        '
        Me._spcPattern.Dock = System.Windows.Forms.DockStyle.Fill
        Me._spcPattern.Location = New System.Drawing.Point(0, 0)
        Me._spcPattern.Name = "_spcPattern"
        Me._spcPattern.Size = New System.Drawing.Size(620, 197)
        Me._spcPattern.SplitterDistance = 358
        Me._spcPattern.TabIndex = 2
        Me._spcPattern.TabStop = False
        '
        '_spcInputOutput
        '
        Me._spcInputOutput.Dock = System.Windows.Forms.DockStyle.Fill
        Me._spcInputOutput.Location = New System.Drawing.Point(0, 0)
        Me._spcInputOutput.Name = "_spcInputOutput"
        Me._spcInputOutput.Size = New System.Drawing.Size(620, 288)
        Me._spcInputOutput.SplitterDistance = 288
        Me._spcInputOutput.SplitterWidth = 5
        Me._spcInputOutput.TabIndex = 1
        Me._spcInputOutput.TabStop = False
        '
        '_stsMain
        '
        Me._stsMain.BackColor = System.Drawing.SystemColors.Control
        Me._stsMain.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me._tslMode, Me._tslGroups, Me._tslEncoding, Me._tslEmpty})
        Me._stsMain.Location = New System.Drawing.Point(0, 490)
        Me._stsMain.Name = "_stsMain"
        Me._stsMain.ShowItemToolTips = True
        Me._stsMain.Size = New System.Drawing.Size(620, 22)
        Me._stsMain.SizingGrip = False
        Me._stsMain.TabIndex = 4
        Me._stsMain.Text = "StatusStrip1"
        '
        '_tslMode
        '
        Me._tslMode.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me._tslMode.Name = "_tslMode"
        Me._tslMode.Size = New System.Drawing.Size(4, 17)
        Me._tslMode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        '_tslGroups
        '
        Me._tslGroups.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right
        Me._tslGroups.Name = "_tslGroups"
        Me._tslGroups.Size = New System.Drawing.Size(4, 17)
        Me._tslGroups.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        '_tslEncoding
        '
        Me._tslEncoding.Name = "_tslEncoding"
        Me._tslEncoding.Size = New System.Drawing.Size(0, 17)
        Me._tslEncoding.Visible = False
        '
        '_tslEmpty
        '
        Me._tslEmpty.Name = "_tslEmpty"
        Me._tslEmpty.Size = New System.Drawing.Size(566, 17)
        Me._tslEmpty.Spring = True
        '
        '_mnsMain
        '
        Me._mnsMain.BackColor = System.Drawing.SystemColors.Control
        Me._mnsMain.Location = New System.Drawing.Point(1, 0)
        Me._mnsMain.Name = "_mnsMain"
        Me._mnsMain.Size = New System.Drawing.Size(782, 24)
        Me._mnsMain.TabIndex = 0
        '
        '_tspMain
        '
        Me._tspMain.AutoSize = False
        Me._tspMain.BackColor = System.Drawing.SystemColors.Control
        Me._tspMain.ButtonBackColor = System.Drawing.Color.Empty
        Me._tspMain.ButtonForeColor = System.Drawing.Color.Empty
        Me._tspMain.ButtonWidth = 23
        Me._tspMain.DropDownButtonWidth = 30
        Me._tspMain.FirstItemHasLeftMargin = False
        Me._tspMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me._tspMain.ItemAutoSize = False
        Me._tspMain.ItemHeight = 23
        Me._tspMain.ItemMargin = New System.Windows.Forms.Padding(3, 0, 0, 0)
        Me._tspMain.LeftMargin = 3
        Me._tspMain.Location = New System.Drawing.Point(1, 24)
        Me._tspMain.Name = "_tspMain"
        Me._tspMain.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me._tspMain.Size = New System.Drawing.Size(782, 25)
        Me._tspMain.SplitButtonWidth = 35
        Me._tspMain.TabIndex = 1
        Me._tspMain.UseCustomMargin = True
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me._spc)
        Me.Controls.Add(Me._tspMain)
        Me.Controls.Add(Me._mnsMain)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(238, Byte))
        Me.KeyPreview = True
        Me.MainMenuStrip = Me._mnsMain
        Me.Name = "MainForm"
        Me.Padding = New System.Windows.Forms.Padding(1, 0, 1, 1)
        Me._spc.Panel2.ResumeLayout(False)
        CType(Me._spc, System.ComponentModel.ISupportInitialize).EndInit()
        Me._spc.ResumeLayout(False)
        Me._pnlRegex.ResumeLayout(False)
        Me._pnlRegex.PerformLayout()
        Me._spcRegex.Panel1.ResumeLayout(False)
        Me._spcRegex.Panel2.ResumeLayout(False)
        CType(Me._spcRegex, System.ComponentModel.ISupportInitialize).EndInit()
        Me._spcRegex.ResumeLayout(False)
        CType(Me._spcPattern, System.ComponentModel.ISupportInitialize).EndInit()
        Me._spcPattern.ResumeLayout(False)
        CType(Me._spcInputOutput, System.ComponentModel.ISupportInitialize).EndInit()
        Me._spcInputOutput.ResumeLayout(False)
        Me._stsMain.ResumeLayout(False)
        Me._stsMain.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents _spc As Regexator.Windows.Forms.ExtendedSplitContainer
    Friend WithEvents _mnsMain As Regexator.MainMenuStrip
    Friend WithEvents _tspMain As Regexator.MainToolStrip
    Friend WithEvents _pnlRegex As System.Windows.Forms.Panel
    Friend WithEvents _spcRegex As Regexator.Windows.Forms.ExtendedSplitContainer
    Friend WithEvents _spcPattern As Regexator.Windows.Forms.ExtendedSplitContainer
    Friend WithEvents _spcInputOutput As Regexator.Windows.Forms.ExtendedSplitContainer
    Friend WithEvents _stsMain As System.Windows.Forms.StatusStrip
    Friend WithEvents _tslMode As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents _tslGroups As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents _tslEncoding As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents _tslEmpty As System.Windows.Forms.ToolStripStatusLabel

End Class
