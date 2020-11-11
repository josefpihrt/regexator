Imports System.Globalization
Imports Regexator.FileSystem
Imports Regexator.Windows.Forms

Public NotInheritable Class MenuFactory

    Private Sub New()

    End Sub

    Private Shared Function SetDropDownItemsEnabled(item As ToolStripMenuItem, enabled As Boolean) As Boolean

        For Each tsi As ToolStripItem In item.DropDownItems
            tsi.Enabled = enabled
        Next
        Return enabled

    End Function

    Private Shared Function CreateItem(item As ToolStripMenuItem, itemsFactory As Func(Of ToolStripMenuItem, IEnumerable(Of ToolStripItem))) As ToolStripMenuItem

        Return CreateItem(item, itemsFactory, False, Function() True)

    End Function

    Private Shared Function CreateItem(item As ToolStripMenuItem, itemsFactory As Func(Of IEnumerable(Of ToolStripItem))) As ToolStripMenuItem

        Return CreateItem(item, itemsFactory, False)

    End Function

    Private Shared Function CreateItem(item As ToolStripMenuItem, itemsFactory As Func(Of IEnumerable(Of ToolStripItem)), reloadAlways As Boolean) As ToolStripMenuItem

        Return CreateItem(item, Function(parent As ToolStripMenuItem) itemsFactory(), reloadAlways, Function() True)

    End Function

    Private Shared Function CreateItem(item As ToolStripMenuItem, itemsFactory As Func(Of ToolStripMenuItem, IEnumerable(Of ToolStripItem)), reloadAlways As Boolean, enabledPredicate As Func(Of Boolean)) As ToolStripMenuItem

        Dim items = item.DropDownItems
        items.Add(New ToolStripSeparator())
        AddHandler item.DropDown.Opening,
            Sub()
                If reloadAlways OrElse (items.Count = 1 AndAlso TypeOf items(0) Is ToolStripSeparator) Then
                    items.LoadItems(itemsFactory(item))
                    If enabledPredicate() = False Then
                        SetDropDownItemsEnabled(item, False)
                    End If
                End If
            End Sub
        Return item

    End Function

    Public Shared Function CreateFileItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.FileAmp), AddressOf CreateFileItems)

    End Function

    Private Shared Iterator Function CreateFileItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.NewItem & My.Resources.EllipsisStr, My.Resources.IcoNewItem.ToBitmap(),
            Sub() Explorer.AddNewProjectOrFolder()) With {.ShortcutKeyDisplayString = My.Resources.CtrlN}
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.NewProject & My.Resources.EllipsisStr, My.Resources.IcoCodeNew.ToBitmap(),
            Sub() Explorer.LoadNewProject())
        Yield New ToolStripMenuItem(My.Resources.NewProjectWithInput & My.Resources.EllipsisStr, My.Resources.IcoNewProjectWithInput.ToBitmap(),
            Sub() Explorer.LoadNewProject(InputKind.Included))
        Yield New ToolStripMenuItem(My.Resources.NewProjectWithFileInput & My.Resources.EllipsisStr, My.Resources.IcoNewProjectWithFileInput.ToBitmap(),
            Sub() Explorer.LoadNewProject(InputKind.File))
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.OpenProject & My.Resources.EllipsisStr, My.Resources.IcoCodeOpen.ToBitmap(),
            Sub() Explorer.LoadProject()) With {.ShortcutKeyDisplayString = My.Resources.CtrlO}
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.SaveAll, My.Resources.IcoSaveAll.ToBitmap(),
            Sub() Explorer.SaveAll()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftS}
        Dim tsiSaveProject As New ToolStripMenuItem(My.Resources.SaveProject, Nothing,
            Sub() Explorer.SaveProject()) With {.Enabled = False}
        Yield tsiSaveProject
        Dim tsiSaveProjectAs As New ToolStripMenuItem(My.Resources.SaveProjectAs & My.Resources.EllipsisStr, Nothing,
            Sub() Explorer.SaveProjectAs()) With {.Enabled = False}
        Yield tsiSaveProjectAs
        Dim tsiSaveInput As New ToolStripMenuItem(My.Resources.SaveInput, Nothing,
            Sub() Explorer.SaveInput()) With {.Enabled = False}
        Yield tsiSaveInput
        Dim tsiSaveInputAs As New ToolStripMenuItem(My.Resources.SaveInputAs & My.Resources.EllipsisStr, Nothing,
            Sub() Explorer.SaveInputAs(InputKind.Included)) With {.Enabled = False}
        Yield tsiSaveInputAs
        Dim tsiSaveInputAsFile As New ToolStripMenuItem(My.Resources.SaveInputAsFile & My.Resources.EllipsisStr, Nothing,
            Sub() Explorer.SaveInputAs(InputKind.File)) With {.Enabled = False}
        Yield tsiSaveInputAsFile
        Yield New ToolStripSeparator()
        Dim tsiCloseProject As New ToolStripMenuItem(My.Resources.CloseProject, Nothing,
            Sub()
                Dim control = App.MainForm.FindFocusedControl()
                Explorer.CloseProject()
                If ReferenceEquals(control, App.MainForm.FindFocusedControl()) = False Then
                    Explorer.TreeView.Select()
                End If
            End Sub)
        Yield tsiCloseProject

        Dim tsiCloseInput As New ToolStripMenuItem(My.Resources.CloseInput, Nothing,
            Sub()
                Dim control = App.MainForm.FindFocusedControl()
                Explorer.CloseInput()
                If ReferenceEquals(control, App.MainForm.FindFocusedControl()) = False Then
                    Explorer.TreeView.Select()
                End If
            End Sub)
        Yield tsiCloseInput

        Yield New ToolStripSeparator()
        Dim tsiRecentProjects As New ToolStripMenuItem(My.Resources.RecentProjects)
        AddHandler tsiRecentProjects.DropDown.KeyDown, AddressOf RecentDropDown_KeyDown
        AddHandler tsiRecentProjects.DropDownOpening,
            Sub()
                If tsiRecentProjects.DropDownItems.Count = 1 AndAlso TypeOf tsiRecentProjects.DropDownItems(0) Is ToolStripSeparator Then
                    Explorer.LoadRecentProjectItems(tsiRecentProjects.DropDownItems)
                End If
            End Sub
        Yield tsiRecentProjects
        Dim tsiRecentDirectories As New ToolStripMenuItem(My.Resources.RecentFolders)
        AddHandler tsiRecentDirectories.DropDown.KeyDown, AddressOf RecentDropDown_KeyDown
        AddHandler tsiRecentDirectories.DropDownOpening,
            Sub()
                If tsiRecentDirectories.DropDownItems.Count = 1 AndAlso TypeOf tsiRecentDirectories.DropDownItems(0) Is ToolStripSeparator Then
                    Explorer.LoadRecentDirectoryItems(tsiRecentDirectories.DropDownItems)
                End If
            End Sub
        Yield tsiRecentDirectories
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.Exit_, My.Resources.IcoExit.ToBitmap(),
            Sub() Application.Exit()) With {.ShortcutKeyDisplayString = My.Resources.AltF4}

        Dim action = Sub()
                         tsiRecentProjects.DropDownItems.Clear()
                         If Explorer.RecentManager.Any(ItemKind.Project) Then
                             tsiRecentProjects.DropDownItems.Add(New ToolStripSeparator())
                         End If
                         tsiRecentDirectories.DropDownItems.Clear()
                         If Explorer.RecentManager.Any(ItemKind.Directory) Then
                             tsiRecentDirectories.DropDownItems.Add(New ToolStripSeparator())
                         End If
                         Dim fProject = Explorer.CurrentProjectNode IsNot Nothing
                         Dim fInput = Explorer.CurrentInputNode IsNot Nothing
                         tsiSaveProject.Enabled = fProject
                         tsiSaveProjectAs.Enabled = fProject
                         tsiSaveInput.Enabled = fInput
                         tsiSaveInputAsFile.Enabled = fInput
                         tsiSaveInputAs.Enabled = fInput
                         tsiCloseProject.Enabled = fProject
                         tsiCloseInput.Enabled = fInput
                     End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()

    End Function

    Public Shared Function CreateViewItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.ViewAmp), AddressOf CreateViewItems)

    End Function

    Private Shared Iterator Function CreateViewItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Dim tsiShowStatusBar As New ToolStripMenuItem(My.Resources.ShowStatusBar, Nothing,
            Sub()
                Dim flg = Not App.MainForm._stsMain.Visible
                App.MainForm._stsMain.Visible = flg
                My.Settings.MainFormStatusBarVisible = flg
            End Sub)
        Yield tsiShowStatusBar
        Yield New ToolStripSeparator()
        Dim tsiVertical = New ToolStripMenuItem(My.Resources.Vertical, Nothing, Sub() App.MainForm.ToggleInputOutputOrientation())
        Dim tsiHorizontal = New ToolStripMenuItem(My.Resources.Horizontal, Nothing, Sub() App.MainForm.ToggleInputOutputOrientation())
        Dim tsiLayout = New ToolStripMenuItem(My.Resources.Layout, Nothing, tsiVertical, tsiHorizontal)
        AddHandler tsiLayout.DropDownOpening,
            Sub()
                tsiVertical.Checked = App.MainForm._spcInputOutput.Orientation = Orientation.Vertical
                tsiHorizontal.Checked = App.MainForm._spcInputOutput.Orientation = Orientation.Horizontal
            End Sub
        Yield tsiLayout
        Yield New ToolStripSeparator()
        Dim tsiExplorerPosition As New ToolStripMenuItem(My.Resources.ProjectExplorerPosition, Nothing, New ToolStripMenuItem())
        AddHandler tsiExplorerPosition.DropDownOpening,
            Sub()
                Dim items = tsiExplorerPosition.DropDownItems
                items.Clear()
                items.Add(New ToolStripMenuItem(My.Resources.Left, Nothing, Sub() App.MainForm.SwapPanels()) With {.Checked = My.Settings.ProjectExplorerOnLeft})
                items.Add(New ToolStripMenuItem(My.Resources.Right, Nothing, Sub() App.MainForm.SwapPanels()) With {.Checked = Not My.Settings.ProjectExplorerOnLeft})
            End Sub
        Yield tsiExplorerPosition

        Dim action =
            Sub()
                tsiShowStatusBar.Checked = My.Settings.MainFormStatusBarVisible
            End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()

    End Function

    Public Shared Function CreatePatternItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.PatternAmp), AddressOf CreatePatternItems, False, Function() Explorer.CurrentProjectNode IsNot Nothing)

    End Function

    Private Shared Iterator Function CreatePatternItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Dim rtb = Panels.Pattern._rtb
        Yield New ToolStripMenuItem(My.Resources.Save, Nothing, Sub() Explorer.SavePattern()) With {.ShortcutKeyDisplayString = My.Resources.CtrlS}
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SavePatternAsTxt(), Sub() Commands.SavePatternAsRtf())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.ChangeEvaluationMode, Nothing, Sub() SelectNextMode()) With {.ShortcutKeyDisplayString = My.Resources.CtrlF2}
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.ListSnippets, My.Resources.IcoSnippet.ToBitmap(), {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.ShowTitleSnippetList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlQ},
            New ToolStripMenuItem(My.Resources.ByCode, Nothing, Sub() rtb.ShowCodeSnippetList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftQ}})
        Yield New ToolStripMenuItem(My.Resources.ListCharacters, Nothing, {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.ShowTitleCharacterList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlW},
            New ToolStripMenuItem(My.Resources.ByValue, Nothing, Sub() rtb.ShowCodeCharacterList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftW}})
        Dim tsiListRecent As New ToolStripMenuItem(My.Resources.ListRecentSnippets, Nothing, {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.Sense.ShowRecentTitles()) With {.ShortcutKeyDisplayString = My.Resources.CtrlE},
            New ToolStripMenuItem(My.Resources.ByValue, Nothing, Sub() rtb.Sense.ShowRecentValues()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftE}})
        Yield tsiListRecent
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateAdjustSelectionItem(rtb, True)
        Yield ToolStripItemFactory.CreateSelectedLinesItem(rtb, True)
        Yield New ToolStripSeparator()
        Dim tsiWordWrap = ToolStripItemFactory.CreatePatternWordWrapItem()
        Yield tsiWordWrap
        Yield New ToolStripSeparator()
        For Each item In ToolStripItemFactory.CreateCutCopyPasteItems(rtb)
            Yield item
        Next
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateSelectAllItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(rtb)
        Dim action = Sub()
                         If SetDropDownItemsEnabled(parent, Explorer.CurrentProjectNode IsNot Nothing) Then
                             tsiListRecent.Enabled = (rtb.Sense.RecentCount > 0)
                         End If
                         tsiWordWrap.Checked = rtb.WordWrap
                     End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()

    End Function

    Public Shared Function CreateReplacementItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.ReplacementAmp), AddressOf CreateReplacementItems, False, Function() Explorer.CurrentProjectNode IsNot Nothing)

    End Function

    Private Shared Iterator Function CreateReplacementItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Dim rtb = Panels.Replacement._rtb
        Yield New ToolStripMenuItem(My.Resources.Save, Nothing, Sub() Explorer.SaveReplacement()) With {.ShortcutKeyDisplayString = My.Resources.CtrlS}
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveReplacementAsTxt(), Sub() Commands.SaveReplacementAsRtf())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.ListSnippets, My.Resources.IcoSnippet.ToBitmap(), {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.ShowTitleSnippetList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlQ},
            New ToolStripMenuItem(My.Resources.ByCode, Nothing, Sub() rtb.ShowCodeSnippetList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftQ}})
        Yield New ToolStripMenuItem(My.Resources.ListCharacters, Nothing, {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.ShowTitleCharacterList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlW},
            New ToolStripMenuItem(My.Resources.ByValue, Nothing, Sub() rtb.ShowCodeCharacterList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftW}})
        Dim tsiListRecent As New ToolStripMenuItem(My.Resources.ListRecentSnippets, Nothing, {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.Sense.ShowRecentTitles()) With {.ShortcutKeyDisplayString = My.Resources.CtrlE},
            New ToolStripMenuItem(My.Resources.ByValue, Nothing, Sub() rtb.Sense.ShowRecentValues()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftE}})
        Yield tsiListRecent
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateAdjustSelectionItem(rtb)
        Yield ToolStripItemFactory.CreateSelectedLinesItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateNewLineModeItem(rtb)
        Yield New ToolStripSeparator()
        Dim tsiWordWrap = ToolStripItemFactory.CreateReplacementWordWrapItem()
        Yield tsiWordWrap
        Yield New ToolStripSeparator()
        For Each item In ToolStripItemFactory.CreateCutCopyPasteItems(rtb)
            Yield item
        Next
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateSelectAllItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(rtb)
        Dim action = Sub()
                         If SetDropDownItemsEnabled(parent, Explorer.CurrentProjectNode IsNot Nothing) Then
                             tsiListRecent.Enabled = (rtb.Sense.RecentCount > 0)
                         End If
                         tsiWordWrap.Checked = rtb.WordWrap
                     End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()

    End Function

    Public Shared Function CreateInputItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.InputAmp, Nothing), AddressOf CreateInputItems, False, Function() Explorer.CurrentInputNode IsNot Nothing)

    End Function

    Private Shared Iterator Function CreateInputItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Dim rtb = Panels.Input._rtb
        Dim tsiEncoding = ToolStripItemFactory.CreateInputEncodingItem()
        Yield New ToolStripMenuItem(My.Resources.Save, Nothing, Sub() Explorer.SaveInput()) With {.ShortcutKeyDisplayString = My.Resources.CtrlS}
        Yield New ToolStripMenuItem(My.Resources.SaveAs & My.Resources.EllipsisStr, Nothing, Sub() Explorer.SaveInputAs(InputKind.Included))
        Yield New ToolStripMenuItem(My.Resources.SaveAsFile & My.Resources.EllipsisStr, Nothing, Sub() Explorer.SaveInputAs(InputKind.File))
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.SaveText, Nothing, Sub() Explorer.SaveInputText())
        Yield New ToolStripMenuItem(My.Resources.SaveOutputText, Nothing, Sub() Commands.SaveOutputText())
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveInputAsTxt(), Sub() Commands.SaveInputAsRtf())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.ListCharacters, Nothing, {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.ShowTitleCharacterList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlW},
            New ToolStripMenuItem(My.Resources.ByValue, Nothing, Sub() rtb.ShowCodeCharacterList()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftW}})
        Dim tsiListRecent As New ToolStripMenuItem(My.Resources.ListRecentCharacters, Nothing, {
            New ToolStripMenuItem(My.Resources.ByTitle, Nothing, Sub() rtb.Sense.ShowRecentTitles()) With {.ShortcutKeyDisplayString = My.Resources.CtrlE},
            New ToolStripMenuItem(My.Resources.ByValue, Nothing, Sub() rtb.Sense.ShowRecentValues()) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftE}})
        Yield tsiListRecent
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateAdjustSelectionItem(rtb)
        Yield ToolStripItemFactory.CreateSelectedLinesItem(rtb)
        Yield New ToolStripSeparator()
        Yield tsiEncoding
        Yield ToolStripItemFactory.CreateNewLineModeItem(rtb)
        Yield New ToolStripSeparator()
        Dim tsiWordWrap = ToolStripItemFactory.CreateInputWordWrapItem()
        Yield tsiWordWrap
        Yield New ToolStripSeparator()
        For Each item In ToolStripItemFactory.CreateCutCopyPasteItems(rtb)
            Yield item
        Next
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateSelectAllItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(rtb)
        Dim action = Sub()
                         If SetDropDownItemsEnabled(parent, Explorer.CurrentInputNode IsNot Nothing) Then
                             tsiListRecent.Enabled = (rtb.Sense.RecentCount > 0)
                             tsiEncoding.Enabled = (Panels.Input.Kind = InputKind.File)
                         End If
                         tsiWordWrap.Checked = rtb.WordWrap
                     End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()

    End Function

    Public Shared Function CreateOutputItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.OutputAmp), AddressOf CreateOutputItems)

    End Function

    Private Shared Iterator Function CreateOutputItems() As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.SetOutputTextAsInputText, Nothing, Sub() Panels.Input.SetText(OutputText.Text))
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.Text, Nothing, CreateOutputTextItems().ToArray())
        Yield New ToolStripMenuItem(My.Resources.Table, Nothing, CreateOutputTableItems().ToArray())
        Yield New ToolStripMenuItem(My.Resources.Summary, Nothing, CreateOutputSummaryItems().ToArray())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.EnableAllSymbols, Nothing, Sub() Panels.Output.EnableAllSymbols())
        Yield New ToolStripMenuItem(My.Resources.DisableAllSymbols, Nothing, Sub() Panels.Output.DisableAllSymbols())

    End Function

    Private Shared Iterator Function CreateOutputTextItems() As IEnumerable(Of ToolStripItem)

        Dim rtb = OutputText._rtb
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveOutputAsTxt(), Sub() Commands.SaveOutputAsRtf())
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateCopyAllItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(rtb)

    End Function

    Private Shared Iterator Function CreateOutputTableItems() As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.CopyAll, Nothing, Sub() OutputTable.CopyTable())

    End Function

    Private Shared Iterator Function CreateOutputSummaryItems() As IEnumerable(Of ToolStripItem)

        Dim rtb = Summary._rtb
        Yield ToolStripItemFactory.CreateSaveItem(Sub() Commands.SaveSummaryAsTxt(), Sub() Commands.SaveSummaryAsRtf())
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateCopyAllItem(Sub() Summary.CopySummaryText())
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreateResetZoomItem(rtb)
        Yield New ToolStripSeparator()
        Yield ToolStripItemFactory.CreatePrintItem(rtb)

    End Function

    Public Shared Function CreateGroupsItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.GroupsAmp), AddressOf CreateGroupsItems, False, Function() App.Mode = EvaluationMode.Match)

    End Function

    Private Shared Iterator Function CreateGroupsItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Dim tsiUndo = New ToolStripMenuItem(My.Resources.Undo, My.Resources.IcoArrowLeft.ToBitmap(), Sub() Groups.History.Undo()) With {.ShortcutKeyDisplayString = My.Resources.CtrlZ}
        Dim tsiRedo = New ToolStripMenuItem(My.Resources.Redo, My.Resources.IcoArrowRight.ToBitmap(), Sub() Groups.History.Redo()) With {.ShortcutKeyDisplayString = My.Resources.CtrlY}
        Dim action = Sub()
                         If SetDropDownItemsEnabled(parent, App.Mode = EvaluationMode.Match) Then
                             tsiUndo.Enabled = Groups.History.CanUndo
                             tsiRedo.Enabled = Groups.History.CanRedo
                         End If
                     End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()
        Yield tsiUndo
        Yield tsiRedo
        Yield New ToolStripSeparator()
        For Each item In Groups.CreateToolStripItems(parent)
            Yield item
        Next

    End Function

    Public Shared Function CreateRegexOptionsItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.RegexOptionsAmp), AddressOf CreateRegexOptionsItems)

    End Function

    Public Shared Iterator Function CreateRegexOptionsItems(parent As ToolStripMenuItem) As IEnumerable(Of ToolStripItem)

        Dim tsiUndo = New ToolStripMenuItem(My.Resources.Undo, My.Resources.IcoArrowLeft.ToBitmap(), Sub() Options.History.Undo()) With {.ShortcutKeyDisplayString = My.Resources.CtrlZ}
        Dim tsiRedo = New ToolStripMenuItem(My.Resources.Redo, My.Resources.IcoArrowRight.ToBitmap(), Sub() Options.History.Redo()) With {.ShortcutKeyDisplayString = My.Resources.CtrlY}
        Dim action = Sub()
                         tsiUndo.Enabled = Options.History.CanUndo
                         tsiRedo.Enabled = Options.History.CanRedo
                     End Sub
        AddHandler parent.DropDownOpening, Sub() action()
        action()
        Yield tsiUndo
        Yield tsiRedo
        Yield New ToolStripSeparator()
        For Each item In Options.CreateToolStripItems(parent).ToArray()
            Yield item
        Next

    End Function

    Public Shared Function CreateToolsItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.ToolsAmp), AddressOf CreateToolsItems)

    End Function

    Private Shared Iterator Function CreateToolsItems() As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.RemoveInvalidRecentItems, Nothing, Sub() Explorer.RecentManager.RemoveInvalidItems())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.CharacterAnalyzer, Nothing, Sub() AppForms.ShowCharacterAnalyzerForm())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.Options & My.Resources.EllipsisStr, My.Resources.IcoSettings.ToBitmap(), Sub() AppForms.ShowOptionsForm())

    End Function

    Public Shared Function CreateHelpItem() As ToolStripMenuItem

        Return CreateItem(New ToolStripMenuItem(My.Resources.HelpAmp), AddressOf CreateHelpItems)

    End Function

    Private Shared Iterator Function CreateHelpItems() As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.Guide, Nothing, Sub() AppForms.ShowGuideForm()) With {.ShortcutKeyDisplayString = My.Resources.F1}
        Yield New ToolStripMenuItem(My.Resources.UserGuide, Nothing, Sub() AppForms.ShowUserGuideForm()) With {.ShortcutKeyDisplayString = My.Resources.ShiftF1}
        'Yield New ToolStripSeparator()
        'Yield New ToolStripMenuItem("Check for Updates", Nothing, Sub() AppUtility.CheckVersion())
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.About, Nothing, Sub() AppForms.ShowAboutForm())

    End Function

    Private Shared Sub RecentDropDown_KeyDown(sender As Object, e As KeyEventArgs)

        Dim dropDown = DirectCast(sender, ToolStripDropDown)
        If e.Modifiers = Keys.None AndAlso e.KeyCode = Keys.Delete Then
            Dim tsi = dropDown.Items.Cast(Of ToolStripItem).FirstOrDefault(Function(f) f.Selected)
            If tsi IsNot Nothing Then
                Dim item As RecentItem = DirectCast(tsi.Tag, RecentItem)
                If item IsNot Nothing Then
                    dropDown.GetBaseDropDown().Hide()
                    If MessageDialog.QuestionYesNo(String.Format(CultureInfo.CurrentCulture, My.Resources.RemovePathFromRecentMsg, item.FullName)) Then
                        Explorer.RecentManager.Remove(item.FullName)
                    End If
                End If
            End If
        End If

    End Sub

End Class