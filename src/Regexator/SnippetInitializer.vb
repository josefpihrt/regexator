Imports System.IO
Imports Regexator.Snippets
Imports Regexator.Xml.Serialization.Snippets

Public Class SnippetManagerFactory

    Private Sub LoadCategories()

        For Each category In AppSettings.HiddenCategories
            _manager.HiddenCategories.Add(category)
        Next

    End Sub

    Private Sub LoadSnippets()

        _log.Clear()
        For Each item In CreateSystemSnippets().Concat(CreateUnicodeSnippets).Concat(CreateUserSnippets)
            If item.Success Then
                Try
                    _manager.Add(item.Snippet)
                Catch ex As ArgumentException
                    _log.Add(New SnippetInfo(New SnippetErrorInfo(item.FilePath, ex, item.Snippet.FullName)))
                End Try
            Else
                _log.Add(item)
            End If
        Next

    End Sub

    Private Function CreateSystemSnippets() As IEnumerable(Of SnippetInfo)

        Dim di = New DirectoryInfo(GetDirectoryPath(AppDirectory.Snippets))
        Return CreateSnippets(di, SnippetOrigin.System)

    End Function

    Private Function CreateUserSnippets() As IEnumerable(Of SnippetInfo)

        Return AppSettings.SnippetDirectories.SelectMany(Function(f) CreateSnippets(f, SnippetOrigin.User))

    End Function

    Private Function CreateUnicodeSnippets() As IEnumerable(Of SnippetInfo)

        Return CreateUnicodeSnippets(My.Resources.XmlGeneralCategories).Concat(CreateUnicodeSnippets(My.Resources.XmlNamedBlocks))

    End Function

    Private Iterator Function CreateUnicodeSnippets(value As String) As IEnumerable(Of SnippetInfo)

        For Each item In SnippetSerializer.DeserializeString(value)
            If item.Snippet IsNot Nothing Then
                item.Snippet.Origin = SnippetOrigin.System
            End If
            Yield item
        Next

    End Function

    Private Iterator Function CreateSnippets(di As DirectoryInfo, origin As SnippetOrigin) As IEnumerable(Of SnippetInfo)

        For Each item In SnippetSerializer.Deserialize(di, SearchOption.AllDirectories).AsParallel()
            If item.Snippet IsNot Nothing Then
                item.Snippet.Origin = origin
            End If
            Yield item
        Next

    End Function

    Private Sub SetFavorite()

        If My.Settings.FavoriteSnippets IsNot Nothing Then
            For Each item In _manager.EnumerateSnippets() _
                    .Join(My.Settings.FavoriteSnippets.Cast(Of String), Function(f) f.FullName, Function(g) g, Function(f, g) f)
                item.Favorite = True
            Next
        End If

    End Sub

    Public ReadOnly Property SnippetManager As SnippetManager
        Get
            _manager = New SnippetManager()
            LoadCategories()
            LoadSnippets()
            SetFavorite()
            Return _manager
        End Get
    End Property

    Public ReadOnly Property ErrorLog As IEnumerable(Of SnippetInfo)
        Get
            Return _log.Select(Function(f) f)
        End Get
    End Property

    Private _manager As SnippetManager
    Private ReadOnly _log As New List(Of SnippetInfo)

End Class