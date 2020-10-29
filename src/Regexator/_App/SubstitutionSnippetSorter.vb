Imports Regexator.Snippets

Public Class SubstitutionSnippetSorter
    Implements IComparer(Of RegexSnippet)

    Public Function Compare(x As RegexSnippet, y As RegexSnippet) As Integer Implements IComparer(Of RegexSnippet).Compare

        If ReferenceEquals(x, y) Then
            Return 0
        End If
        If ReferenceEquals(x, Nothing) Then
            Return -1
        End If
        If ReferenceEquals(y, Nothing) Then
            Return 1
        End If
        If x.Favorite Then
            If y.Favorite Then
                Return String.Compare(x.Title, y.Title, StringComparison.CurrentCulture)
            Else
                Return -1
            End If
        Else
            If y.Favorite Then
                Return 1
            Else
                Return String.Compare(x.Title, y.Title, StringComparison.CurrentCulture)
            End If
        End If

    End Function

End Class
