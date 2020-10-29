Imports Regexator
Imports Regexator.Snippets

Public Class SnippetManager

    Public Sub New()

        _dic = New Dictionary(Of String, RegexSnippet)()
        _hiddenCategories = New HashSet(Of RegexCategory)()

    End Sub

    Public Sub Add(item As RegexSnippet)

        If item Is Nothing Then Throw New ArgumentNullException("item")
        _dic.Add(item.FullName, item)

    End Sub

    Public Function EnumerateSnippets() As IEnumerable(Of RegexSnippet)

        Return _dic.Select(Function(f) f.Value)

    End Function

    Public Function EnumerateFavoriteSnippets() As IEnumerable(Of RegexSnippet)

        Return EnumerateSnippets().Where(Function(f) f.Favorite)

    End Function

    Public Function EnumerateCategories() As IEnumerable(Of RegexCategory)

        Return EnumerateSnippets() _
            .GroupBy(Function(f) f.RegexCategory) _
            .Select(Function(f) f.Key) _
            .OrderBy(Function(f) f.ToString())

    End Function

    Public Sub SetFavoriteSnippets(fullNames As String())

        If fullNames Is Nothing Then Throw New ArgumentNullException("fullNames")
        For Each item In EnumerateSnippets()
            item.Favorite = fullNames.Contains(item.FullName)
        Next

    End Sub

    Public ReadOnly Property HiddenCategories As HashSet(Of RegexCategory)
        Get
            Return _hiddenCategories
        End Get
    End Property

    Private ReadOnly _dic As Dictionary(Of String, RegexSnippet)
    Private ReadOnly _hiddenCategories As HashSet(Of RegexCategory)

End Class
