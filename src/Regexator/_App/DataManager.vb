Imports System.Collections.ObjectModel
Imports System.Globalization
Imports Regexator
Imports Regexator.Collections.Generic
Imports Regexator.Snippets
Imports Regexator.Text
Imports Regexator.Xml.Serialization

Public Class DataManager

    Private Sub New()

        ResetSnippetManager()

    End Sub

    Private Function SnippetManagerFactory() As SnippetManager

#If DEBUG Then
        Dim manager = _factory.SnippetManager
        UI.ErrorInfoForm.ShowErrors(SnippetErrorLog.ToArray(), My.Resources.FollowingSnippetsCouldNotBeLoadedMsg)
        Return manager
#Else
        Return _factory.SnippetManager
#End If

    End Function

    Private Function GuideItemsFactory() As ReadOnlyCollection(Of GuideItem)

        Return XmlSerializationManager.DeserializeText(Of Xml.Serialization.GuideItem())(My.Resources.XmlGuideItems) _
            .Select(Function(f) Xml.Serialization.GuideItem.FromSerializable(f)) _
            .ToReadOnly()

    End Function

    Private Iterator Function CreateCharacterSnippets() As IEnumerable(Of RegexSnippet)

        Dim i As Integer = 0
        Dim favorites As New SortedSet(Of String)(AppSettings.FavoriteCharacters)
        Dim basicLatin = RegexCategory.BasicLatin.ToString()
        Dim latin1Supplement = RegexCategory.Latin1Supplement.ToString()
        For Each name In My.Resources.BasicLatinCharacters.EnumerateLines().Concat(My.Resources.Latin1SupplementCharacters.EnumerateLines())
            If name.StartsWith(" ") = False Then
                Dim ch As Char = Convert.ToChar(i)
                Dim s As New RegexSnippet(If(i < 128, basicLatin, latin1Supplement), name, ch.ToString(), Nothing)
                s.Title = TextUtility.SplitCamelCase(s.Name)
                If favorites.Contains(s.FullName) Then
                    s.Favorite = True
                End If
                Yield s
            End If
            i += 1
        Next

    End Function

    Private Iterator Function CreatePatternCharacterSnippets() As IEnumerable(Of RegexSnippet)

        Dim i As Integer = 0
        Dim favorites As New SortedSet(Of String)(AppSettings.FavoritePatternCharacters)
        Dim basicLatin = RegexCategory.BasicLatin.ToString()
        Dim latin1Supplement = RegexCategory.Latin1Supplement.ToString()
        For Each name In My.Resources.BasicLatinCharacters.EnumerateLines().Concat(My.Resources.Latin1SupplementCharacters.EnumerateLines()).Select(Function(f) f.TrimStart())
            Dim ch As Char = Convert.ToChar(i)
            Dim code As String = If(Char.GetUnicodeCategory(ch) = UnicodeCategory.Control, "\u" + i.ToString("X4"), ch.ToString())
            Dim s As New RegexSnippet(If(i < 128, basicLatin, latin1Supplement), name, code, Nothing)
            s.Title = TextUtility.SplitCamelCase(s.Name)
            If favorites.Contains(s.FullName) Then
                s.Favorite = True
            End If
            Yield s
            i += 1
        Next

    End Function

    Public Sub ResetSnippetManager()

        _snippetManagerLazy = New Lazy(Of SnippetManager)(AddressOf SnippetManagerFactory)

    End Sub

    Public ReadOnly Property SnippetManager As SnippetManager
        Get
            Return _snippetManagerLazy.Value
        End Get
    End Property

    Public ReadOnly Property GuideItems As ReadOnlyCollection(Of GuideItem)
        Get
            Return _guideItemsLazy.Value
        End Get
    End Property

    Public ReadOnly Property RegexCategoryInfos As ReadOnlyCollection(Of RegexCategoryInfo)
        Get
            Return _regexCategoryInfosLazy.Value
        End Get
    End Property

    Private Function RegexCategoryInfosFactory() As ReadOnlyCollection(Of RegexCategoryInfo)

        Return XmlSerializationManager.DeserializeText(Of Xml.Serialization.RegexCategoryInfo())(My.Resources.XmlRegexCategoryInfos) _
            .Select(Function(f) Xml.Serialization.RegexCategoryInfo.FromSerializable(f)) _
            .ToReadOnly()

    End Function

    Public ReadOnly Property SnippetManagerInitialized As Boolean
        Get
            Return _snippetManagerLazy.IsValueCreated
        End Get
    End Property

    Public ReadOnly Property CharactersInitialized As Boolean
        Get
            Return _charactersLazy.IsValueCreated
        End Get
    End Property

    Public ReadOnly Property PatternCharactersInitialized As Boolean
        Get
            Return _patternCharactersLazy.IsValueCreated
        End Get
    End Property

    Public ReadOnly Property Characters As IEnumerable(Of RegexSnippet)
        Get
            Return _charactersLazy.Value
        End Get
    End Property

    Public ReadOnly Property PatternCharacters As IEnumerable(Of RegexSnippet)
        Get
            Return _patternCharactersLazy.Value
        End Get
    End Property

    Public Shared ReadOnly Property Instance As DataManager
        Get
            Return _instance
        End Get
    End Property

    Public ReadOnly Property SnippetErrorLog As IEnumerable(Of SnippetErrorInfo)
        Get
            Return _factory.ErrorLog.Select(Function(f) f.ErrorInfo)
        End Get
    End Property

    Public Function EnumerateSnippets() As IEnumerable(Of RegexSnippet)

        Return SnippetManager.EnumerateSnippets()

    End Function

    Public Function EnumerateFavoriteSnippets() As IEnumerable(Of RegexSnippet)

        Return EnumerateSnippets().Where(Function(f) f.Favorite AndAlso f.RegexCategory <> RegexCategory.Substitutions)

    End Function

    Public Function EnumerateSubstitutionSnippets() As IEnumerable(Of RegexSnippet)

        Return EnumerateSnippets().Where(Function(f) f.RegexCategory = RegexCategory.Substitutions)

    End Function

    Private _snippetManagerLazy As Lazy(Of SnippetManager)
    Private ReadOnly _guideItemsLazy As New Lazy(Of ReadOnlyCollection(Of GuideItem))(AddressOf GuideItemsFactory)
    Private ReadOnly _regexCategoryInfosLazy As New Lazy(Of ReadOnlyCollection(Of RegexCategoryInfo))(AddressOf RegexCategoryInfosFactory)
    Private ReadOnly _charactersLazy As New Lazy(Of ReadOnlyCollection(Of RegexSnippet))(Function() CreateCharacterSnippets.ToReadOnly())
    Private ReadOnly _patternCharactersLazy As New Lazy(Of ReadOnlyCollection(Of RegexSnippet))(Function() CreatePatternCharacterSnippets.ToReadOnly())
    Private ReadOnly _factory As New SnippetManagerFactory()

    Private Shared ReadOnly _instance As New DataManager()

End Class
