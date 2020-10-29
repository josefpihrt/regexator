Imports System.Collections.Specialized
Imports System.Configuration
Imports System.Diagnostics.CodeAnalysis
Imports System.IO
Imports System.Text
Imports Regexator
Imports Regexator.FileSystem
Imports Regexator.Collections.Generic

Public NotInheritable Class AppSettings

    Private Sub New()

    End Sub

    <SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")>
    Public Shared Sub Initialize()

        Dim upgrade As Boolean = False
        Try
            upgrade = My.Settings.SettingsUpgradeRequired
        Catch ex As ConfigurationException
            Dim filePath As String = GetConfigFilePath(ex)
            If File.Exists(filePath) AndAlso MessageBox.Show(My.Resources.SettingsInitializationErrorMsg, My.Application.Info.Title, MessageBoxButtons.YesNo, MessageBoxIcon.Error) = DialogResult.Yes Then
                File.Delete(filePath)
            End If
            AppUtility.LogException(ex)
            Environment.Exit(1)
        End Try
        If upgrade Then
#If NO_INSTALL Then
            Using frm As New EulaForm()
                If frm.ShowDialog() = DialogResult.OK Then
                    Process.Start(App.WebRoot & "/thanks.ashx")
                Else
                    Environment.Exit(1)
                End If
            End Using
#End If
            My.Settings.Upgrade()
            My.Settings.SettingsUpgradeRequired = False
            My.Settings.CheckAppVersionLastDate = Date.Now
        End If

    End Sub

    Private Shared Function GetConfigFilePath(ex As ConfigurationException) As String

        Do While ex.Filename Is Nothing
            Dim ex2 = TryCast(ex.InnerException, ConfigurationException)
            If ex2 IsNot Nothing Then
                ex = ex2
            Else
                Exit Do
            End If
        Loop
        Return ex.Filename

    End Function

    Public Shared Sub SaveHiddenCategories()

        If Data.SnippetManagerInitialized Then
            My.Settings.HiddenCategories = Data.SnippetManager.HiddenCategories _
                .ToStringCollection(Function(f) f.ToString())
        End If

    End Sub

    Private Shared Sub SaveFavoriteSnippets()

        If Data.SnippetManagerInitialized Then
            My.Settings.FavoriteSnippets = Data.SnippetManager.EnumerateSnippets _
                .Where(Function(f) f.Favorite) _
                .ToStringCollection(Function(f) f.FullName)
        End If

    End Sub

    Private Shared Sub SaveFavoriteCharacters()

        If Data.CharactersInitialized Then
            My.Settings.FavoriteCharacters = Data.Characters _
                .Where(Function(f) f.Favorite) _
                .ToStringCollection(Function(f) f.FullName)
        End If
        If Data.PatternCharactersInitialized Then
            My.Settings.FavoritePatternCharacters = Data.PatternCharacters _
                .Where(Function(f) f.Favorite) _
                .ToStringCollection(Function(f) f.FullName)
        End If

    End Sub

    Public Shared Sub SetDefaultFavoriteSnippets()

        My.Settings.FavoriteSnippets = DefaultFavoriteSnippets.ToStringCollection()

    End Sub

    <SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")>
    Public Shared Sub Save()

        With App.MainForm
            My.Settings.OtherTabNames = ._tbcOther.TabPages.Cast(Of TabPage).ToStringCollection(Function(f) f.Name)
            My.Settings.TextOutputTabNames = ._tbcTextOutput.TabPages.Cast(Of TabPage).ToStringCollection(Function(f) f.Name)
            My.Settings.OutputTabNames = ._tbcOutput.TabPages.Cast(Of TabPage).ToStringCollection(Function(f) f.Name)
            My.Settings.FileSystemSearchTabNames = ._tbcFileSystemSearch.TabPages.Cast(Of TabPage).ToStringCollection(Function(f) f.Name)
            My.Settings.MainFormLocation = .Location
            My.Settings.MainFormSize = .Size
            My.Settings.MainFormWindowState = .WindowState
            My.Settings.MainFormSplitterDistanceMain = ._spc.SplitterDistance
            My.Settings.MainFormSplitterDistancePattern = ._spcPattern.SplitterDistance
            If ._spcInputOutput.Orientation = Orientation.Horizontal Then
                My.Settings.MainFormSplitterDistanceInputOutputHorizontal = ._spcInputOutput.SplitterDistance
                My.Settings.MainFormSplitterDistanceRegexHorizontal = ._spcRegex.SplitterDistance
            Else
                My.Settings.MainFormSplitterDistanceInputOutputVertical = ._spcInputOutput.SplitterDistance
                My.Settings.MainFormSplitterDistanceRegexVertical = ._spcRegex.SplitterDistance
            End If
            My.Settings.InputOutputOrientation = ._spcInputOutput.Orientation
        End With
        My.Settings.RegexOptionsHotkeyNumberVisible = Options.HotkeyNumberColumnVisible
        My.Settings.RegexOptionsDescriptionVisible = Options.DescriptionColumnVisible
        My.Settings.LastProjectPath = Explorer.CurrentProjectPath
        My.Settings.LastDirectoryPath = If(Explorer.CurrentProjectPath IsNot Nothing, Nothing, Explorer.SelectedRootPath)
        My.Settings.FileSystemShowHidden = Explorer.ShowHidden
        SaveHiddenCategories()
        SaveFavoriteSnippets()
        SaveFavoriteCharacters()
        SaveRecentProjects()
        SaveRecentDirectories()
        My.Settings.VisibleRegexOptions = App.RegexOptionsManager.VisibleOptions
        My.Settings.OutputEnabled = Panels.Output.OutputEnabled
        My.Settings.OutputLimitEnabled = Panels.Output.LimitEnabled
        My.Settings.OutputMatchTableByGroups = (Panels.OutputTable._dgv.MatchModeLayout = TableLayout.Group)
        My.Settings.InputWordWrap = Panels.Input._rtb.WordWrap
        My.Settings.PatternWordWrap = Panels.Pattern._rtb.WordWrap
        My.Settings.ReplacementWordWrap = Panels.Replacement._rtb.WordWrap
        My.Settings.FindResultsMatchCase = FindResults._tsp.MatchCase
        My.Settings.FindResultsMachWholeWord = FindResults._tsp.MatchWholeWord
        My.Settings.FindResultsSearchPattern = FindResults._tsp.SearchPattern
        My.Settings.FindResultsSearchInput = FindResults._tsp.SearchInput
        My.Settings.FindResultsSearchReplacement = FindResults._tsp.SearchReplacement
        My.Settings.FindResultsSearchOutput = FindResults._tsp.SearchOutput
        My.Settings.FindResultsSearchPhraseHistory = FindResults._tsp.EnumerateSearchPhrases().ToStringCollection()
        My.Settings.OutputTextWordWrap = OutputText._rtb.WordWrap
        My.Settings.OutputSummaryWordWrap = Summary._rtb.WordWrap
        My.Settings.AppExitSuccess = True
        My.Settings.Save()

    End Sub

    Public Shared Iterator Function FindResultsSearchPhrases() As IEnumerable(Of String)

        Dim items As StringCollection = My.Settings.FindResultsSearchPhraseHistory
        If items IsNot Nothing Then
            For i = items.Count - 1 To 0 Step -1
                Yield DirectCast(items(i), String)
            Next
        End If

    End Function

    Public Shared ReadOnly Iterator Property FavoriteCharacters As IEnumerable(Of String)
        Get
            If My.Settings.FavoriteCharacters IsNot Nothing Then
                For Each item In My.Settings.FavoriteCharacters.Cast(Of String)()
                    Yield item
                Next
            End If
        End Get
    End Property

    Public Shared ReadOnly Iterator Property FavoritePatternCharacters As IEnumerable(Of String)
        Get
            If My.Settings.FavoritePatternCharacters IsNot Nothing Then
                For Each item In My.Settings.FavoritePatternCharacters.Cast(Of String)()
                    Yield item
                Next
            End If
        End Get
    End Property

    Public Shared ReadOnly Iterator Property RecentProjects As IEnumerable(Of RecentItem)
        Get
            If My.Settings.RecentProjects IsNot Nothing Then
                For Each item In My.Settings.RecentProjects _
                        .Cast(Of String) _
                        .Select(Function(f) Executor.CreateFileInfo(f)) _
                        .Where(Function(f) f IsNot Nothing) _
                        .Select(Function(f) New RecentProject(f))
                    Yield item
                Next
            End If
        End Get
    End Property

    Public Shared Sub SaveRecentProjects()

        My.Settings.RecentProjects = Explorer.RecentManager.EnumerateItems(ItemKind.Project) _
            .Select(Function(f) f.FullName) _
            .Take(100) _
            .Reverse() _
            .ToStringCollection()

    End Sub

    Public Shared ReadOnly Iterator Property RecentDirectories As IEnumerable(Of RecentItem)
        Get
            If My.Settings.RecentDirectories IsNot Nothing Then
                For Each item In My.Settings.RecentDirectories _
                        .Cast(Of String) _
                        .Select(Function(f) Executor.CreateDirectoryInfo(f)) _
                        .Where(Function(f) f IsNot Nothing) _
                        .Select(Function(f) New RecentDirectory(f))
                    Yield item
                Next
            End If
        End Get
    End Property

    Public Shared Sub SaveRecentDirectories()

        My.Settings.RecentDirectories = Explorer.RecentManager.EnumerateItems(ItemKind.Directory) _
            .Select(Function(f) f.FullName) _
            .Take(100) _
            .Reverse() _
            .ToStringCollection()

    End Sub

    Public Shared Iterator Property SnippetDirectories As IEnumerable(Of DirectoryInfo)
        Get
            If My.Settings.SnippetDirectories IsNot Nothing Then
                For Each item In My.Settings.SnippetDirectories _
                        .Cast(Of String)() _
                        .Select(Function(f) Executor.CreateDirectoryInfo(f)) _
                        .Where(Function(f) f IsNot Nothing)
                    Yield item
                Next
            End If
        End Get
        Set(value As IEnumerable(Of DirectoryInfo))
            If value Is Nothing Then Throw New ArgumentNullException("value")
            My.Settings.SnippetDirectories = value.ToStringCollection(Function(f) f.FullName)
        End Set
    End Property

    Public Shared ReadOnly Iterator Property HiddenCategories As IEnumerable(Of RegexCategory)
        Get
            If My.Settings.HiddenCategories IsNot Nothing Then
                For Each item In My.Settings.HiddenCategories
                    Dim category As RegexCategory
                    If [Enum].TryParse(item, category) Then
                        Yield category
                    End If
                Next
            End If
        End Get
    End Property

    Public Shared ReadOnly Property InputDefaultEncoding As Encoding
        Get
            If String.IsNullOrEmpty(My.Settings.DefaultInputEncoding) = False Then
                Try
                    Return Encoding.GetEncoding(My.Settings.DefaultInputEncoding)
                Catch ex As ArgumentException
                    My.Settings.DefaultInputEncoding = Input.DefaultEncoding.WebName
                End Try
            End If
            Return Input.DefaultEncoding
        End Get
    End Property

    Public Shared ReadOnly Iterator Property DefaultFavoriteSnippets As IEnumerable(Of String)
        Get
            If My.Settings.DefaultFavoriteSnippets IsNot Nothing Then
                For Each item As String In My.Settings.DefaultFavoriteSnippets
                    Yield item
                Next
            End If
        End Get
    End Property

End Class