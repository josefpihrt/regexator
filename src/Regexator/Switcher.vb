Imports Regexator.UI
Imports System.Diagnostics.CodeAnalysis

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public NotInheritable Class Switcher

    Private Sub New()

        _dic = CreateSwitcherItems().ToDictionary(Function(f) f.Element, Function(f) f)

        _dicTextOutput.Add(UIElement.OutputText, _frm._tbpText)
        _dicTextOutput.Add(UIElement.OutputTable, _frm._tbpTable)
        _dicTextOutput.Add(UIElement.OutputSummary, _frm._tbpSummary)

        _dicFileOutput.Add(UIElement.FileSystemSearchResults, _frm._tbpFileSystemSearch)

        _dicPattern.Add(UIElement.RegexOptions, _frm._tbpOptions)
        _dicPattern.Add(UIElement.Groups, _frm._tbpGroups)
        _dicPattern.Add(UIElement.ProjectInfo, _frm._tbpInfo)
        _dicPattern.Add(UIElement.FindResults, _frm._tbpFindResults)

    End Sub

    Public Sub Show()

        _cms.LoadItems(EnumerateItems())
        Dim pt = _frm.Location
        pt.Offset(CInt(_frm.Width / 2), CInt(_frm.Height / 2))
        pt.Offset(-CInt(_cms.Width / 2), -CInt(_cms.Height / 2))
        _cms.Show(pt)

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        _dic(UIElement.Pattern).Enabled = Panels.Pattern._pnl.Enabled
        _dic(UIElement.Input).Enabled = Panels.Input._pnl.Enabled
        _dic(UIElement.Replacement).Enabled = Panels.Pattern._pnl.Enabled
        _dic(UIElement.ProjectInfo).Enabled = Panels.Pattern._pnl.Enabled

        Yield _dic(UIElement.Pattern).ToolStripMenuItem
        Yield _dic(UIElement.Input).ToolStripMenuItem
        Yield _dic(UIElement.Replacement).ToolStripMenuItem

        Yield New ToolStripSeparator()

        For Each item In EnumerateItems(_dic, _dicTextOutput, _frm._tbcTextOutput)
            Yield item.ToolStripMenuItem
        Next

        Yield New ToolStripSeparator()
        Yield _dic(UIElement.FileSystemSearchResults).ToolStripMenuItem

        Yield New ToolStripSeparator()

        For Each item In EnumerateItems(_dic, _dicPattern, _frm._tbcOther)
            Yield item.ToolStripMenuItem
        Next

        Yield New ToolStripSeparator()

        Yield _dic(UIElement.ProjectExplorer).ToolStripMenuItem

    End Function

    Private Shared Function EnumerateItems(items As Dictionary(Of UIElement, SwitcherItem), tabs As Dictionary(Of UIElement, TabPage), tbc As TabControl) As IEnumerable(Of SwitcherItem)

        Return items.Join(tabs, Function(f) f.Key, Function(g) g.Key, Function(f, g) New With {.Item = f.Value, .TabPage = g.Value}) _
            .OrderBy(Function(f) tbc.TabPages.IndexOf(f.TabPage)) _
            .Select(Function(f) f.Item)

    End Function

    Private Iterator Function CreateSwitcherItems() As IEnumerable(Of SwitcherItem)

        Yield New SwitcherItem(UIElement.Pattern, Panels.Pattern._rtb, Panels.Pattern._pnl)
        Yield New SwitcherItem(UIElement.Input, Panels.Input._rtb, Panels.Input._pnl)

        Yield New SwitcherItem(UIElement.OutputText, OutputText._rtb)
        Yield New SwitcherItem(UIElement.OutputTable, OutputTable._dgv)
        Yield New SwitcherItem(UIElement.OutputSummary, Summary._rtb)

        Yield New SwitcherItem(UIElement.FileSystemSearchResults, FileSystemSearchResults._dgv)

        Yield New SwitcherItem(UIElement.Replacement, Panels.Replacement._rtb)
        Yield New SwitcherItem(UIElement.RegexOptions, Options._dgv)
        Yield New SwitcherItem(UIElement.Groups, Groups._dgv)
        Yield New SwitcherItem(UIElement.ProjectInfo, Info._pgd)
        Yield New SwitcherItem(UIElement.FindResults, FindResults._rtb)
        Yield New SwitcherItem(UIElement.ProjectExplorer, Explorer._trv, Explorer._pnl)

    End Function

    Public Shared ReadOnly Property Instance As Switcher
        Get
            Return _instance
        End Get
    End Property

    Private ReadOnly _frm As MainForm = App.MainForm
    Private ReadOnly _cms As New SwitcherContextMenuStrip()

    Private ReadOnly _dic As Dictionary(Of UIElement, SwitcherItem)
    Private ReadOnly _dicPattern As New Dictionary(Of UIElement, TabPage)()
    Private ReadOnly _dicTextOutput As New Dictionary(Of UIElement, TabPage)()
    Private ReadOnly _dicFileOutput As New Dictionary(Of UIElement, TabPage)()

    Private Shared ReadOnly _instance As Switcher = New Switcher()

End Class