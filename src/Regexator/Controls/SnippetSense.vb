Imports Regexator.Snippets
Imports Regexator.Windows.Forms

Public Class SnippetSense
    Inherits CodeSense(Of SnippetSenseItem)

    Public Sub New(rtb As ExtendedRichTextBox)

        _rtb = rtb
        _editor = New AppLiteralEditor(_rtb)
        _factory = New SnippetToolStripItemFactory(_editor)
        App.Formats.DropDownText.Controls.Add(Me)
        AddHandler _rtb.ContextMenuStrip.Opening, AddressOf HideIfVisible
        AddHandler _rtb.VScroll, AddressOf HideIfVisible
        AddHandler _rtb.HScroll, AddressOf HideIfVisible
        AddHandler _rtb.SelectionChanged, AddressOf HideIfVisible
        AddHandler App.MainForm.Move, AddressOf HideIfVisible
        AddHandler App.MainForm.Resize, AddressOf HideIfVisible
        AddHandler App.MainForm.Deactivate, AddressOf HideIfVisible

    End Sub

    Private Sub HideIfVisible(sender As Object, e As EventArgs)

        If Visible Then
            _rtb.Select()
        End If

    End Sub

    Public Shared Function GetLocation(rtb As RichTextBox, width As Integer) As Point

        If rtb Is Nothing Then Throw New ArgumentNullException("rtb")
        Dim pt = App.MainForm.PointToClient(rtb.PointToScreen(rtb.GetSelectionEndLocation(1)))
        If Screen.GetWorkingArea(pt).Contains(New Point(pt.X + width, pt.Y)) = False Then
            pt.Offset(-width, 0)
        End If
        Return pt

    End Function

    Public Shared Function Filter(snippets As IEnumerable(Of RegexSnippet), options As SnippetOptions) As IEnumerable(Of RegexSnippet)

        If (options And SnippetOptions.HideCategory) = SnippetOptions.HideCategory Then
            snippets = snippets.Where(Function(f) Data.SnippetManager.HiddenCategories.Contains(f.RegexCategory) = False)
        End If
        Return snippets

    End Function

    Public Overloads Sub Show(snippets As IEnumerable(Of RegexSnippet))

        Show(snippets, Function(f As RegexSnippet) New SnippetTitleSenseItem(f))

    End Sub

    Public Overloads Sub Show(snippets As IEnumerable(Of RegexSnippet), creator As Func(Of RegexSnippet, SnippetSenseItem))

        Show(snippets, creator, SnippetOptions.None)

    End Sub

    Public Overloads Sub Show(snippets As IEnumerable(Of RegexSnippet), creator As Func(Of RegexSnippet, SnippetSenseItem), options As SnippetOptions)

        Show(snippets, creator, options, New SenseItemSorter(), True)

    End Sub

    Public Overloads Sub Show(snippets As IEnumerable(Of RegexSnippet), creator As Func(Of RegexSnippet, SnippetSenseItem), options As SnippetOptions, sorter As IComparer(Of SenseItem), orderByText As Boolean)

        If snippets Is Nothing Then Throw New ArgumentNullException("snippets")
        Dim qry = Filter(snippets, options).Select(Function(f) creator(f))
        If orderByText Then
            qry = qry.OrderBy(Function(f) f.Text)
        End If
        Dim items = qry.ToArray()
        If items.Length > 0 Then
            If _rtb.IsCaretVisible() = False Then
                _rtb.Reselect()
            End If
            Me.Sorter = sorter
            Location = GetLocation(_rtb, Width)
            App.MainForm.Controls.Add(Me)
            Show(items)
            BringToFront()
        End If

    End Sub

    Public Sub ShowRecentTitles()

        Show(Editor.EnumerateRecentSnippets(), Function(f) New SnippetTitleSenseItem(f), SnippetOptions.None, Nothing, False)

    End Sub

    Public Sub ShowRecentValues()

        Show(Editor.EnumerateRecentSnippets(), Function(f) New SnippetCodeSenseItem(f), SnippetOptions.None, Nothing, False)

    End Sub

    Public Sub InsertLastRecent()

        Dim snippet = Editor.EnumerateRecentSnippets().FirstOrDefault()
        If snippet IsNot Nothing Then
            Editor.InsertSnippet(snippet)
        End If

    End Sub

    Public Sub InsertLastRecentCode()

        Dim item = Editor.EnumerateRecentItems().FirstOrDefault()
        If item IsNot Nothing Then
            _rtb.Select()
            RegexSnippet.Insert(_rtb, item.Code)
        End If

    End Sub

    Protected Overrides Sub OnTerminate(e As EventArgs)

        _rtb.Select()
        If Success AndAlso SelectedItem IsNot Nothing Then
            _editor.InsertSnippet(SelectedItem.Snippet, SelectedItem.UseExtended)
        End If
        MyBase.OnTerminate(e)

    End Sub

    Protected Overrides Sub OnLeave(e As EventArgs)

        MyBase.OnLeave(e)
        Hide()
        App.MainForm.Controls.Remove(Me)

    End Sub

    Public ReadOnly Property RecentCount As Integer
        Get
            Return _editor.RecentCount
        End Get
    End Property

    Public ReadOnly Property Editor As AppLiteralEditor
        Get
            Return _editor
        End Get
    End Property

    Public ReadOnly Property Factory As SnippetToolStripItemFactory
        Get
            Return _factory
        End Get
    End Property

    Private ReadOnly _rtb As ExtendedRichTextBox
    Private ReadOnly _editor As AppLiteralEditor
    Private ReadOnly _factory As SnippetToolStripItemFactory

End Class
