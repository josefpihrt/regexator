Imports System.Text.RegularExpressions
Imports System.Globalization
Imports Regexator
Imports Regexator.FileSystem
Imports System.Diagnostics.CodeAnalysis
Imports Regexator.Windows.Forms

Public Class MainToolStrip
    Inherits AppToolStrip

    Public Sub New()

        MyBase.New()
        CreateItems()

    End Sub

    Public Sub LoadItems()

        Items.AddRange(EnumerateItems().ToArray())

    End Sub

    <SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")>
    Private Sub CreateItems()

        _tssUndo = New ToolStripSplitButton(Nothing, My.Resources.IcoArrowLeftSmallInverse.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.NavigateBackward & " " & My.Resources.CtrlMinus.AddParentheses()}
        _tssRedo = New ToolStripSplitButton(Nothing, My.Resources.IcoArrowRightSmallInverse.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.NavigateForward & " " & My.Resources.CtrlShiftMinus.AddParentheses()}
        _ddbNewProject = New ToolStripDropDownButton(Nothing, My.Resources.IcoCodeNew.ToBitmap(),
            New ToolStripMenuItem(My.Resources.NewProject & My.Resources.EllipsisStr, My.Resources.IcoCodeNew.ToBitmap(), Sub() Explorer.LoadNewProject()),
            New ToolStripMenuItem(My.Resources.NewProjectWithInput & My.Resources.EllipsisStr, My.Resources.IcoNewProjectWithInput.ToBitmap(), Sub() Explorer.LoadNewProject(InputKind.Included)),
            New ToolStripMenuItem(My.Resources.NewProjectWithFileInput & My.Resources.EllipsisStr, My.Resources.IcoNewProjectWithFileInput.ToBitmap(), Sub() Explorer.LoadNewProject(InputKind.File))) With {.ToolTipText = My.Resources.NewProject}

        _btnOpenProject = New ToolStripButton(Nothing, My.Resources.IcoCodeOpen.ToBitmap(), Sub() Explorer.LoadProject()) With {.ToolTipText = My.Resources.OpenProject}
        _ddbFolders = New ToolStripDropDownButton(Nothing, My.Resources.IcoFolder.ToBitmap()) With {.ToolTipText = My.Resources.RecentFolders}
        _ddbProjects = New ToolStripDropDownButton(Nothing, My.Resources.IcoCode.ToBitmap()) With {.ToolTipText = My.Resources.RecentProjects}
        _cmbSearch = New FileSystemSearchTextBox() With {.AutoSize = False, .Width = 125}

        _tssSaveProject = New ToolStripSplitButton(Nothing, My.Resources.IcoSave.ToBitmap()) With {.ToolTipText = My.Resources.SaveProject}
        Dim tsiSaveProjectAs As New ToolStripMenuItem(My.Resources.SaveProjectAs & My.Resources.EllipsisStr, Nothing, Sub() Explorer.SaveProjectAs()) With {.Enabled = False}
        _tssSaveProject.DropDownItems.Add(tsiSaveProjectAs)
        AddHandler _tssSaveProject.DropDownOpening, Sub() tsiSaveProjectAs.Enabled = (Explorer.CurrentProjectNode IsNot Nothing)

        _btnSaveAll = New ToolStripButton(Nothing, My.Resources.IcoSaveAll.ToBitmap(), Sub() Explorer.SaveAll()) With {.ToolTipText = My.Resources.SaveAll & " " & My.Resources.CtrlShiftS.AddParentheses()}
        _btnMatch = New ToolStripButton(Nothing, My.Resources.IcoM.ToBitmap(), Sub() Mode = EvaluationMode.Match) With {.Checked = True, .ToolTipText = My.Resources.Match}
        _btnReplace = New ToolStripButton(Nothing, My.Resources.IcoR.ToBitmap(), Sub() Mode = EvaluationMode.Replace) With {.ToolTipText = My.Resources.Replace}
        _btnSplit = New ToolStripButton(Nothing, My.Resources.IcoS.ToBitmap(), Sub() Mode = EvaluationMode.Split) With {.ToolTipText = My.Resources.Split}

        CreateLayoutButtons()

        AddHandler _tssUndo.DropDownOpening, Sub() _tssUndo.DropDownItems.LoadItems(Explorer.History.CreateUndoToolStripMenuItems(My.Settings.UndoItemsMaxCount))
        AddHandler _tssUndo.ButtonClick, Sub() Explorer.History.Undo()
        AddHandler _tssRedo.DropDownOpening, Sub() _tssRedo.DropDownItems.LoadItems(Explorer.History.CreateRedoToolStripMenuItems(My.Settings.UndoItemsMaxCount))
        AddHandler _tssRedo.ButtonClick, Sub() Explorer.History.Redo()
        AddHandler _ddbFolders.DropDownOpening, Sub() Explorer.LoadRecentDirectoryItems(_ddbFolders.DropDownItems)
        AddHandler _ddbFolders.DropDown.KeyDown, AddressOf RecentDropDown_KeyDown
        AddHandler _ddbProjects.DropDownOpening, Sub() Explorer.LoadRecentProjectItems(_ddbProjects.DropDownItems)
        AddHandler _ddbProjects.DropDown.KeyDown, AddressOf RecentDropDown_KeyDown
        AddHandler _tssSaveProject.ButtonClick, Sub() Explorer.SaveProject()

    End Sub

    Private Sub CreateLayoutButtons()

        _tssLayout = New ToolStripSplitButton(Nothing, My.Resources.IcoLayout.ToBitmap()) With {.ToolTipText = My.Resources.ToggleLayout}
        Dim tsiVerticalLayout As New ToolStripMenuItem(My.Resources.VerticalLayout)
        AddHandler tsiVerticalLayout.Click,
            Sub()
                If App.MainForm._spcInputOutput.Orientation <> Orientation.Vertical Then
                    App.MainForm.ToggleInputOutputOrientation()
                End If
            End Sub
        Dim tsiHorizontalLayout As New ToolStripMenuItem(My.Resources.HorizontalLayout)
        AddHandler tsiHorizontalLayout.Click,
            Sub()
                If App.MainForm._spcInputOutput.Orientation <> Orientation.Horizontal Then
                    App.MainForm.ToggleInputOutputOrientation()
                End If
            End Sub
        AddHandler _tssLayout.ButtonClick, Sub() App.MainForm.ToggleInputOutputOrientation()
        AddHandler _tssLayout.DropDownOpening, Sub()
                                                   tsiVerticalLayout.Checked = My.Settings.InputOutputOrientation = System.Windows.Forms.Orientation.Vertical
                                                   tsiHorizontalLayout.Checked = My.Settings.InputOutputOrientation = System.Windows.Forms.Orientation.Horizontal
                                               End Sub
        _tssLayout.DropDownItems.AddRange({tsiVerticalLayout, tsiHorizontalLayout})

    End Sub

    Public Sub CreateOptionsButtons()

        Dim items = App.RegexOptionsManager.Items

        _btnCompiled = New OptionToolStripButton(My.Resources.IcoCompiled(), items(RegexOptions.Compiled))
        _btnCultureInvariant = New OptionToolStripButton(My.Resources.IcoCultureInvariant(), items(RegexOptions.CultureInvariant))
        _btnECMAScript = New OptionToolStripButton(My.Resources.IcoECMAScript(), items(RegexOptions.ECMAScript))
        _btnExplicitCapture = New OptionToolStripButton(My.Resources.IcoExplicitCapture(), items(RegexOptions.ExplicitCapture))
        _btnIgnoreCase = New OptionToolStripButton(My.Resources.IcoIgnoreCase(), items(RegexOptions.IgnoreCase))
        _btnIgnorePatternWhitespace = New OptionToolStripButton(My.Resources.IcoIgnorePatternWhitespace(), items(RegexOptions.IgnorePatternWhitespace))
        _btnMultiline = New OptionToolStripButton(My.Resources.IcoMultiline(), items(RegexOptions.Multiline))
        _btnRightToLeft = New OptionToolStripButton(My.Resources.IcoRightToLeft(), items(RegexOptions.RightToLeft))
        _btnSingleline = New OptionToolStripButton(My.Resources.IcoSingleline(), items(RegexOptions.Singleline))

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _cmbSearch
        Yield New ToolStripSeparator()
        Yield _tssUndo
        Yield _tssRedo
        Yield New ToolStripSeparator()
        Yield _ddbNewProject
        Yield _btnOpenProject
        Yield New ToolStripSeparator()
        Yield _ddbFolders
        Yield _ddbProjects
        Yield New ToolStripSeparator()
        Yield _tssSaveProject
        Yield _btnSaveAll
        Yield New ToolStripSeparator()
        Yield _btnMatch
        Yield _btnReplace
        Yield _btnSplit
        Yield New ToolStripSeparator()
        Yield _btnCompiled
        Yield _btnCultureInvariant
        Yield _btnECMAScript
        Yield _btnExplicitCapture
        Yield _btnIgnoreCase
        Yield _btnIgnorePatternWhitespace
        Yield _btnMultiline
        Yield _btnRightToLeft
        Yield _btnSingleline
        Yield New ToolStripSeparator()
        Yield _tssLayout

    End Function

    Private Sub RecentDropDown_KeyDown(sender As Object, e As KeyEventArgs)

        Dim dropDown = DirectCast(sender, ToolStripDropDown)
        If e.Modifiers = Keys.None AndAlso e.KeyCode = Keys.Delete Then
            Dim tsi = dropDown.Items.Cast(Of ToolStripItem).FirstOrDefault(Function(f) f.Selected)
            If tsi IsNot Nothing Then
                Dim item As RecentItem = DirectCast(tsi.Tag, RecentItem)
                If item IsNot Nothing Then
                    dropDown.Hide()
                    If MessageDialog.Question(String.Format(CultureInfo.CurrentCulture, My.Resources.RemovePathFromRecentMsg, item.FullName)) = DialogResult.Yes Then
                        Explorer.RecentManager.Remove(item.FullName)
                    End If
                End If
            End If
        End If

    End Sub

    Friend _tssUndo As ToolStripSplitButton
    Friend _tssRedo As ToolStripSplitButton
    Friend _ddbNewProject As ToolStripDropDownButton
    Friend _btnOpenProject As ToolStripButton
    Friend _ddbFolders As ToolStripDropDownButton
    Friend _ddbProjects As ToolStripDropDownButton
    Friend _cmbSearch As FileSystemSearchTextBox

    Friend _tssSaveProject As ToolStripSplitButton
    Friend _btnSaveAll As ToolStripButton

    Friend _btnMatch As ToolStripButton
    Friend _btnReplace As ToolStripButton
    Friend _btnSplit As ToolStripButton

    Friend _btnCompiled As ToolStripButton
    Friend _btnCultureInvariant As ToolStripButton
    Friend _btnECMAScript As ToolStripButton
    Friend _btnExplicitCapture As ToolStripButton
    Friend _btnIgnoreCase As ToolStripButton
    Friend _btnIgnorePatternWhitespace As ToolStripButton
    Friend _btnMultiline As ToolStripButton
    Friend _btnRightToLeft As ToolStripButton
    Friend _btnSingleline As ToolStripButton
    Friend _tssLayout As ToolStripSplitButton

End Class