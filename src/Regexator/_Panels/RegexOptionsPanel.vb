Imports Regexator.UI

Public Class AppRegexOptionsPanel
    Inherits RegexOptionsPanel

    Public Sub New(manager As RegexOptionsManager)

        MyBase.New(manager)
        DataGridView.HotkeyNumberColumnVisible = My.Settings.RegexOptionsHotkeyNumberVisible
        DataGridView.DescriptionColumnVisible = My.Settings.RegexOptionsDescriptionVisible

        _dgv = DataGridView
        _dgv.BorderStyle = BorderStyle.None
        _tsp = New AppToolStrip()
        _pnlGrid = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}

        _btnUndo = New ToolStripButton(Nothing, My.Resources.IcoArrowLeft.ToBitmap(), Sub() History.Undo()) With {.Enabled = False, .ToolTipText = My.Resources.Undo & " " & My.Resources.CtrlZ.AddParentheses()}
        _btnRedo = New ToolStripButton(Nothing, My.Resources.IcoArrowRight.ToBitmap(), Sub() History.Redo()) With {.Enabled = False, .ToolTipText = My.Resources.Redo & " " & My.Resources.CtrlY.AddParentheses()}
        _btnEnableAll = New ToolStripButton(Nothing, My.Resources.IcoCheckAll.ToBitmap(), Sub() SetAll()) With {.ToolTipText = My.Resources.SetAll}
        _btnEnableNone = New ToolStripButton(Nothing, My.Resources.IcoUncheckAll.ToBitmap(), Sub() manager.SetNone()) With {.ToolTipText = My.Resources.SetNone}

        AddHandler History.CanUndoChanged, Sub() _btnUndo.Enabled = History.CanUndo
        AddHandler History.CanRedoChanged, Sub() _btnRedo.Enabled = History.CanRedo

        _tsp.Items.AddRange({_btnUndo, _btnRedo, New ToolStripSeparator(), _btnEnableAll, _btnEnableNone})
        _pnlGrid.Controls.Add(_dgv)
        _pnl.Controls.AddRange({_pnlGrid, _tsp})

    End Sub

    Public Property HotkeyNumberColumnVisible As Boolean
        Get
            Return _dgv.HotkeyNumberColumnVisible
        End Get
        Set(value As Boolean)
            _dgv.HotkeyNumberColumnVisible = value
        End Set
    End Property

    Public Property DescriptionColumnVisible As Boolean
        Get
            Return _dgv.DescriptionColumnVisible
        End Get
        Set(value As Boolean)
            _dgv.DescriptionColumnVisible = value
        End Set
    End Property

    Friend _dgv As RegexOptionsDataGridView
    Friend _tsp As AppToolStrip
    Friend _pnlGrid As Panel
    Friend _pnl As Panel
    Friend _btnUndo As ToolStripButton
    Friend _btnRedo As ToolStripButton
    Friend _btnEnableAll As ToolStripButton
    Friend _btnEnableNone As ToolStripButton

End Class
