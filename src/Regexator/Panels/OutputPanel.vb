Imports System.Diagnostics.CodeAnalysis
Imports System.Globalization
Imports Regexator
Imports Regexator.Output
Imports Regexator.Text
Imports Regexator.Text.RegularExpressions

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class OutputPanel

    Public Sub New()

        _tsp = New OutputToolStrip()

    End Sub

    Public Sub LoadData()

        LoadData(False)

    End Sub

    Public Sub LoadData(multiReplace As Boolean)

        ClearData()

        If Panels.Pattern.Exception IsNot Nothing Then
            Dim content = If(TypeOf Panels.Pattern.Exception Is ArgumentException, Panels.Pattern.Exception.Message, String.Empty)
            OutputText.LoadMessage(My.Resources.InvalidPattern, content, Color.Red)
            Return
        End If

        If OutputEnabled = False Then
            OutputText.LoadMessage(My.Resources.Paused)
            Return
        End If

        If Panels.Pattern.RegexInfo IsNot Nothing AndAlso Panels.Pattern.RegexInfo.IsEmpty Then
            OutputText.LoadMessage("No Pattern")
            Return
        End If

        _suppressed = True
        Try
            _builder = Me.GetBuilder()

            If _builder IsNot Nothing Then
                LoadData(_builder)
                If _builder.Mode = EvaluationMode.Match AndAlso Groups.IsAnyEnabled = False Then
                    OutputText.LoadMessage(My.Resources.NoGroupChecked)
                ElseIf _builder.Mode <> EvaluationMode.Split AndAlso _builder.Count = 0 Then
                    OutputText.LoadMessage(My.Resources.NoMatch)
                End If
            End If

            If OutputText.Text.Length = 0 Then
                OutputText.LoadMessage("Output is empty")
            End If

        Catch ex As TimeoutException
            OutputText.LoadMessage(My.Resources.EngineHasTimedOutMsg, ex.Message, Color.Red)
        Catch ex As IndexOutOfRangeException
            Debug.WriteLine(ex.CreateLog())
            OutputText.LoadMessage(My.Resources.EngineFailedWhileProcessingInput, String.Empty, Color.Red)
        End Try
        _suppressed = False

        If _builder IsNot Nothing Then
            Dim block = If(_selectedBlock IsNot Nothing, _builder.Blocks.FirstOrDefault(Function(f) f.Key = _selectedBlock.Key), Nothing)
            Highlight(block, HighlightSource.Text)
        End If

    End Sub

    Private Sub LoadData(builder As RegexBuilder)

        If Panels.Input.NewLineMode = NewLineMode.CrLf Then
            CarriageReturnProcessor.ProcessBlocks(builder)
        End If

        OutputText.Text = builder.GetText()

        If App.MainForm.IsTableTabSelected Then
            OutputTable.LoadTable()
        End If

        If App.MainForm.IsSummaryTabSelected Then
            Summary.LoadSummary()
        End If

        Panels.Input.Searcher = IndexSearcher.CreateInputSearcher(builder.Blocks)
        OutputText.Searcher = IndexSearcher.CreateOutputSearcher(builder.Blocks)
        _tsp._lblInfo.Text = String.Concat(GetInfoText(builder))
        _tsp._lblInfo.ToolTipText = String.Concat(GetInfoToolTipText(builder))

    End Sub

    Private Shared Iterator Function GetInfoText(builder As RegexBuilder) As IEnumerable(Of String)

        Yield builder.Count.ToString(CultureInfo.CurrentCulture)
        If builder.LimitState = LimitState.Limited Then
            Yield My.Resources.PlusStr
        End If
        Select Case builder.Mode
            Case EvaluationMode.Match
                If builder.Count <> builder.CaptureCount Then
                    Yield " " & builder.CaptureCount.ToString(CultureInfo.CurrentCulture).AddParentheses()
                End If
            Case EvaluationMode.Split
                Dim splitBuilder = DirectCast(builder, SplitBuilder)
                If splitBuilder.Data.GroupSplitItemCount > 0 Then
                    Yield " " & splitBuilder.Data.GroupSplitItemCount.ToString(CultureInfo.CurrentCulture).AddParentheses()
                End If
        End Select


    End Function

    Private Shared Iterator Function GetInfoToolTipText(builder As RegexBuilder) As IEnumerable(Of String)

        Yield builder.Count.ToString(CultureInfo.CurrentCulture)
        If builder.LimitState = LimitState.Limited Then
            Yield My.Resources.PlusStr
        End If
        Yield " "
        Select Case builder.Mode
            Case EvaluationMode.Match
                Yield If(builder.Count > 1, My.Resources.Matches.ToLowerInvariant(), My.Resources.Match.ToLowerInvariant())
                If builder.Count <> builder.CaptureCount Then
                    Yield " ("
                    Yield builder.CaptureCount.ToString(CultureInfo.CurrentCulture)
                    Yield " "
                    Yield If(builder.CaptureCount > 1, My.Resources.Captures.ToLowerInvariant(), My.Resources.Capture.ToLowerInvariant())
                    Yield ")"
                End If
            Case EvaluationMode.Replace
                Yield If(builder.Count > 1, My.Resources.Replacements().ToLowerInvariant(), My.Resources.Replacement().ToLowerInvariant())
            Case EvaluationMode.Split
                Yield If(builder.Count > 1, My.Resources.Splits.ToLowerInvariant(), My.Resources.Split.ToLowerInvariant())
                Dim splitBuilder = DirectCast(builder, SplitBuilder)
                If splitBuilder.Data.GroupSplitItemCount > 0 Then
                    Yield " ("
                    Yield splitBuilder.Data.GroupSplitItemCount.ToString(CultureInfo.CurrentCulture)
                    Yield " "
                    Yield If(splitBuilder.Data.GroupSplitItemCount > 1, My.Resources.Groups.ToLowerInvariant(), My.Resources.Group.ToLowerInvariant())
                    Yield ")"
                End If
        End Select

    End Function

    Public Sub ClearData()

        _suppressed = True
        _builder = Nothing
        _blocks = Nothing

        Panels.Input.Searcher = IndexSearcher.Empty
        OutputText.Searcher = IndexSearcher.Empty
        OutputText.ClearAll(App.MainForm.IsTextTabSelected)
        Summary.ClearSummary(App.MainForm.IsSummaryTabSelected)
        OutputTable.ClearData()
        FileSystemSearchResults.ClearData()

        _tsp._lblInfo.Text = String.Empty
        _suppressed = False

    End Sub

    Public Sub Highlight(block As RegexBlock, source As HighlightSource)

        If _suppressed = False Then
            _suppressed = True
            OutputText.Highlight(block, source)
            _selectedBlock = block
            If source <> HighlightSource.Table Then
                OutputTable.Highlight(block)
                If App.MainForm.IsTableTabSelected() Then
                    OutputTable.EnsureDisplayed()
                End If
            End If
            If block IsNot Nothing Then
                Panels.Input.Highlight(block)
            End If
            _suppressed = False
        End If

    End Sub

    Public Sub Highlight()

        Highlight(HighlightOptions.DisplayRectangle)

    End Sub

    Public Sub Highlight(options As HighlightOptions)

        If _builder IsNot Nothing Then
            _suppressed = True
            OutputText.Highlight(_builder, options)
            _selectedBlock = Nothing
            _suppressed = False
        End If

    End Sub

    Public Function FindBlock(key As String) As RegexBlock

        Debug.Assert(key IsNot Nothing)
        Dim block As RegexBlock = Nothing
        If _blocks Is Nothing AndAlso _builder IsNot Nothing Then
            _blocks = _builder.Blocks.ToDictionary(Function(f) f.Key, Function(f) f)
        End If
        If _blocks IsNot Nothing Then
            _blocks.TryGetValue(key, block)
        End If
        Return block

    End Function

    Public Sub ResetBlock()

        _selectedBlock = Nothing

    End Sub

    Public Function HasOptions(options As OutputOptions) As Boolean

        Return _tsp.HasOptions(options)

    End Function

    Public Sub EnableAllSymbols()

        _tsp.EnableSymbols()

    End Sub

    Public Sub DisableAllSymbols()

        _tsp.DisableSymbols()

    End Sub

    Public Function OutputEnabledRestorer() As ValueRestorer(Of Boolean)

        Dim value = OutputEnabled
        OutputEnabled = False
        Return New ValueRestorer(Of Boolean)(value, Sub(f) OutputEnabled = f)

    End Function

    Public ReadOnly Property OutputSettings As OutputSettings
        Get
            Dim settings As New OutputSettings() With {
                .NumberAlignment = My.Settings.OutputNumberAlignment,
                .OmitRepeatedInfo = My.Settings.OutputOmitRepeatedInfo,
                .NewLine = vbLf
            }

            settings.UseCarriageReturnSymbol = ((Options And OutputOptions.CarriageReturnSymbol) = OutputOptions.CarriageReturnSymbol)
            settings.UseLinefeedSymbol = ((Options And OutputOptions.LinefeedSymbol) = OutputOptions.LinefeedSymbol)
            settings.UseTabSymbol = ((Options And OutputOptions.TabSymbol) = OutputOptions.TabSymbol)
            settings.UseSpaceSymbol = ((Options And OutputOptions.SpaceSymbol) = OutputOptions.SpaceSymbol)
            settings.AddInfo = ((Options And OutputOptions.Info) = OutputOptions.Info)

            If String.IsNullOrEmpty(My.Settings.CarriageReturnSymbol) = False Then
                settings.Symbols.CarriageReturn = My.Settings.CarriageReturnSymbol
            End If
            If String.IsNullOrEmpty(My.Settings.LinefeedSymbol) = False Then
                settings.Symbols.Linefeed = My.Settings.LinefeedSymbol
            End If
            If String.IsNullOrEmpty(My.Settings.NoCaptureSymbol) = False Then
                settings.Symbols.NoCapture = My.Settings.NoCaptureSymbol
            End If
            If String.IsNullOrEmpty(My.Settings.TabSymbol) = False Then
                settings.Symbols.Tab = My.Settings.TabSymbol
            End If
            If String.IsNullOrEmpty(My.Settings.SpaceSymbol) = False Then
                settings.Symbols.Space = My.Settings.SpaceSymbol
            End If
            Return settings
        End Get
    End Property

    Public Property Options As OutputOptions
        Get
            Return _tsp.Options Or OutputTable.Options
        End Get
        Set(value As OutputOptions)
            OutputTable.Options = value
            _tsp.Options = value
        End Set
    End Property

    Public Property OutputEnabled As Boolean
        Get
            Return _outputEnabled
        End Get
        Set(value As Boolean)
            If _outputEnabled <> value Then
                _outputEnabled = value
                _tsp._btnOutputEnabled.Checked = value
                LoadData()
            End If
        End Set
    End Property

    Public ReadOnly Property LimitEnabled As Boolean
        Get
            Return _tsp.LimitEnabled
        End Get
    End Property

    Private ReadOnly Property LimitCount As Integer
        Get
            Return If(LimitEnabled, My.Settings.OutputLimit, MatchData.InfiniteLimit)
        End Get
    End Property

    Private ReadOnly Property ReplacementSettings As ReplacementSettings
        Get
            Return New ReplacementSettings(Panels.Replacement.ReplacementMode, LimitCount)
        End Get
    End Property

    Private Function GetBuilder() As RegexBuilder

        Select Case Mode
            Case EvaluationMode.Match
                Return GetMatchBuilder()
            Case EvaluationMode.Replace
                Return GetReplaceBuilder()
            Case EvaluationMode.Split
                Return GetSplitBuilder()
        End Select

        Return Nothing

    End Function

    Private Function GetMatchBuilder() As MatchBuilder

        Dim info = Panels.Pattern.RegexInfo
        If info IsNot Nothing Then
            Dim data As New MatchData(info.Regex, Panels.Input.CurrentText, LimitCount)
            Return New MatchBuilder(data, OutputSettings, Groups.GroupSettings)
        End If
        Return Nothing

    End Function

    Private Function GetReplaceBuilder() As ReplaceBuilder

        Dim info = Panels.Pattern.RegexInfo
        If info IsNot Nothing Then
            Dim data As New ReplaceData(info.Regex, Panels.Input.CurrentText, Panels.Replacement.CurrentText, ReplacementSettings)
            Return New ReplaceBuilder(data, OutputSettings)
        End If

        Return Nothing

    End Function

    Private Function GetSplitBuilder() As SplitBuilder

        Dim info = Panels.Pattern.RegexInfo
        If info IsNot Nothing Then
            Dim data As New SplitData(info.Regex, Panels.Input.CurrentText, LimitCount)
            Return New SplitBuilder(data, OutputSettings)
        End If
        Return Nothing

    End Function

    Public ReadOnly Property Suppressed As Boolean
        Get
            Return _suppressed
        End Get
    End Property

    Public ReadOnly Property Builder As RegexBuilder
        Get
            Return _builder
        End Get
    End Property

    Friend _tsp As OutputToolStrip
    Private _builder As RegexBuilder
    Private _outputEnabled As Boolean
    Private _suppressed As Boolean
    Private _selectedBlock As RegexBlock
    Private _blocks As Dictionary(Of String, RegexBlock)

End Class