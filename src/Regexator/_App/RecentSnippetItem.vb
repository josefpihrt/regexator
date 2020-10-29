Imports Regexator.Snippets

Public Class RecentSnippetItem

    Public Sub New(snippet As RegexSnippet, code As String)

        If snippet Is Nothing Then Throw New ArgumentNullException("snippet")
        If code Is Nothing Then Throw New ArgumentNullException("code")
        _snippet = snippet
        _code = code

    End Sub

    Public ReadOnly Property Snippet As RegexSnippet
        Get
            Return _snippet
        End Get
    End Property

    Public ReadOnly Property Code As String
        Get
            Return _code
        End Get
    End Property

    Private ReadOnly _snippet As RegexSnippet
    Private ReadOnly _code As String

End Class
