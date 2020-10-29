Imports System.Text.RegularExpressions
Imports Regexator.Collections.Generic
Imports Regexator.Snippets
Imports Regexator.Text
Imports Regexator.Windows.Forms

Public Class AppLiteralEditor
    Inherits SnippetLiteralEditor

    Public Sub New(rtb As ExtendedRichTextBox)

        _rtb = rtb
        _items = New List(Of RecentSnippetItem)()
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

    Public Sub InsertSnippet(snippet As RegexSnippet)

        InsertSnippet(snippet, False)

    End Sub

    Public Sub InsertSnippet(snippet As RegexSnippet, useExtended As Boolean)

        If snippet Is Nothing Then Throw New ArgumentNullException("snippet")
        If useExtended AndAlso snippet.ExtendedSnippet IsNot Nothing Then
            snippet = snippet.ExtendedSnippet
        End If
        Dim text = TextUtility.Untabify(snippet.Code, _rtb.SpaceCount)
        If snippet.Literals.Any() Then
            Location = SnippetSense.GetLocation(_rtb, Width)
            App.MainForm.Controls.Add(Me)
            Show(snippet, text, _rtb.SelectedText)
            BringToFront()
        Else
            InsertSnippet(snippet, text)
        End If

    End Sub

    Protected Overrides Sub OnTerminate(e As EventArgs)

        _rtb.Select()
        If Success Then
            InsertSnippet(Snippet, SnippetText)
        End If
        MyBase.OnTerminate(e)

    End Sub

    Private Sub InsertSnippet(snippet As RegexSnippet, code As String)

        _rtb.Select()
        Dim enabled = Panels.Output.OutputEnabled
        Dim flg = My.Settings.SetOptionsIfRequiredBySnippet AndAlso snippet.Options <> RegexOptions.None
        If flg Then
            Panels.Output.OutputEnabled = False
        End If
        RegexSnippet.Insert(_rtb, code)
        _items.RemoveAll(Function(f) snippet.CodeKind = f.Snippet.CodeKind AndAlso String.Equals(snippet.FullName, f.Snippet.FullName))
        _items.Add(New RecentSnippetItem(snippet, code))
        If flg Then
            App.RegexOptionsManager.AddOptions(snippet.Options)
            Panels.Output.OutputEnabled = enabled
        End If

    End Sub

    Protected Overrides Sub OnLeave(e As EventArgs)

        MyBase.OnLeave(e)
        Hide()
        App.MainForm.Controls.Remove(Me)

    End Sub

    Public Function EnumerateRecentSnippets() As IEnumerable(Of RegexSnippet)

        Return EnumerateRecentItems().Select(Function(f) f.Snippet)

    End Function

    Public Function EnumerateRecentItems() As IEnumerable(Of RecentSnippetItem)

        Return _items.Reversed()

    End Function

    Public ReadOnly Property RecentCount As Integer
        Get
            Return _items.Count
        End Get
    End Property

    Private ReadOnly _rtb As ExtendedRichTextBox
    Private ReadOnly _items As List(Of RecentSnippetItem)

End Class