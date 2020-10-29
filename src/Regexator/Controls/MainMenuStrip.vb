Public Class MainMenuStrip
    Inherits MenuStrip

    Public Sub New()

        Dock = DockStyle.Top
        BackColor = SystemColors.Control
        _tsiFile = MenuFactory.CreateFileItem()
        _tsiView = MenuFactory.CreateViewItem()
        _tsiPattern = MenuFactory.CreatePatternItem()
        _tsiReplacement = MenuFactory.CreateReplacementItem()
        _tsiInput = MenuFactory.CreateInputItem()
        _tsiOutput = MenuFactory.CreateOutputItem()
        _tsiGroups = MenuFactory.CreateGroupsItem()
        _tsiRegexOptions = MenuFactory.CreateRegexOptionsItem()
        _tsiTools = MenuFactory.CreateToolsItem()
        _tsiHelp = MenuFactory.CreateHelpItem()

    End Sub

    Public Sub LoadItems()

        Items.AddRange(EnumerateItems().ToArray())

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _tsiFile
        Yield _tsiView
        Yield _tsiPattern
        Yield _tsiInput
        Yield _tsiReplacement
        Yield _tsiOutput
        Yield _tsiGroups
        Yield _tsiRegexOptions
        Yield _tsiTools
        Yield _tsiHelp

    End Function

    Friend _tsiFile As ToolStripMenuItem
    Friend _tsiView As ToolStripMenuItem
    Friend _tsiPattern As ToolStripMenuItem
    Friend _tsiReplacement As ToolStripMenuItem
    Friend _tsiInput As ToolStripMenuItem
    Friend _tsiOutput As ToolStripMenuItem
    Friend _tsiGroups As ToolStripMenuItem
    Friend _tsiRegexOptions As ToolStripMenuItem
    Friend _tsiTools As ToolStripMenuItem
    Friend _tsiHelp As ToolStripMenuItem

End Class