Imports Regexator.Snippets
Imports Regexator.Text
Imports Regexator.Windows.Forms

Public Class SnippetToolStripItemFactory

    Public Sub New(editor As AppLiteralEditor)

        If editor Is Nothing Then Throw New ArgumentNullException("editor")
        _editor = editor

    End Sub

    Public Function CreateFavoriteSnippetsItem() As ToolStripMenuItem

        Return CreateItem(My.Resources.FavoriteSnippets, Function() CreateItems(Data.EnumerateFavoriteSnippets()))

    End Function

    Public Function CreateSubstitutionSnippetsItem() As ToolStripMenuItem

        Return CreateItem(My.Resources.Snippets, Function() CreateItems(Data.EnumerateSubstitutionSnippets().OrderBy(Function(f) f, New SubstitutionSnippetSorter()), SnippetOptions.None))

    End Function

    Public Function CreateGroupedSnippetsItem() As ToolStripMenuItem

        Return CreateItem(My.Resources.Snippets, Function() CreateItemsByCategory(Data.EnumerateSnippets().Where(Function(f) f.RegexCategory <> RegexCategory.Substitutions)).ToArray())

    End Function

    Public Function CreateRecentItem() As ToolStripMenuItem

        Return CreateRecentItem(My.Resources.RecentSnippets)

    End Function

    Public Function CreateRecentItem(text As String) As ToolStripMenuItem

        Return CreateItem(text, Function() CreateItems(_editor.EnumerateRecentSnippets().Take(My.Settings.RecentItemsMaxCount), SnippetOptions.None))

    End Function

    Private Shared Function CreateItem(text As String, factory As Func(Of IEnumerable(Of ToolStripMenuItem))) As ToolStripMenuItem

        Dim item = New ToolStripMenuItem(text, Nothing, New ToolStripMenuItem())
        AddHandler item.DropDownOpening,
            Sub()
                item.DropDownItems.LoadItems(factory())
                If item.DropDownItems.Count = 0 Then
                    item.DropDownItems.Add(New ToolStripMenuItem(My.Resources.NoItems) With {.Enabled = False})
                End If
            End Sub
        Return item

    End Function

    Private Iterator Function CreateItemsByCategory(snippets As IEnumerable(Of RegexSnippet)) As IEnumerable(Of ToolStripMenuItem)

        For Each item In snippets _
                .GroupBy(Function(f) f.Category) _
                .Select(Function(f) New With {.Group = f, .Text = TextUtility.SplitCamelCase(f.Key)}) _
                .OrderBy(Function(f) f.Text)
            Yield New ToolStripMenuItem(item.Text, Nothing,
                item.Group _
                .Select(Function(f) CreateItem(f)) _
                .OrderBy(Function(f) f.Text).ToArray())
        Next

    End Function

    Public Function CreateItems(snippets As IEnumerable(Of RegexSnippet)) As IEnumerable(Of ToolStripMenuItem)

        Return CreateItems(snippets, SnippetOptions.Sort)

    End Function

    Public Function CreateItems(snippets As IEnumerable(Of RegexSnippet), options As SnippetOptions) As IEnumerable(Of ToolStripMenuItem)

        Dim items = SnippetSense.Filter(snippets, options).Select(Function(f) CreateItem(f))
        If (options And SnippetOptions.Sort) = SnippetOptions.Sort Then
            items = items.OrderBy(Function(f) f.Text)
        End If
        Return items

    End Function

    Private Function CreateItem(snippet As RegexSnippet) As ToolStripMenuItem

        Dim item As ToolStripMenuItem = New ToolStripMenuItem(snippet.Title, If(snippet.IsExtensible, ExtensibleSnippetBitmap, SnippetBitmap))
        AddHandler item.Click, Sub() _editor.InsertSnippet(snippet, (Control.ModifierKeys And Keys.Shift) = Keys.Shift)
        AddHandler item.MouseUp,
            Sub(sender As Object, e As MouseEventArgs)
                If e.Button = MouseButtons.Right Then
                    _editor.InsertSnippet(snippet, True)
                    item.DropDown.GetBaseDropDown().Hide()
                End If
            End Sub
        Return item

    End Function

    Private Shared ReadOnly ExtensibleSnippetBitmap As Bitmap = My.Resources.IcoSnippetExtensible.ToBitmap()
    Private Shared ReadOnly SnippetBitmap As Bitmap = My.Resources.IcoSnippet.ToBitmap()

    Private ReadOnly _editor As AppLiteralEditor

End Class
