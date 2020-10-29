Imports System.ComponentModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Text
Imports Regexator.Text
Imports Regexator.UI
Imports Regexator.Windows.Forms

'P I R O hotkeys
'alt+left/right go to next/previous block

<SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")>
Public Class FindResultsPanel

    Friend _rtb As FindRichTextBox
    Friend _tsp As AppFindToolStrip
    Friend _pnlBox As Panel
    Friend _pnl As Panel
    Friend _tspSearch As SearchToolStrip
    Private _bgw As BackgroundWorker
    Private _isSearching As Boolean

    Private _results As List(Of SearchMatch)

    Public Sub New()

        _rtb = New FindRichTextBox()
        _tsp = New AppFindToolStrip()
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None, .BackColor = _rtb.BackColor}
        _tspSearch = New AppSearchToolStrip(_rtb, False)

        _pnlBox.Controls.Add(_rtb)
        _pnl.Controls.AddRange({_pnlBox, _tspSearch, _tsp})

        App.Formats.Text.Controls.Add(_rtb)
        App.Formats.Text.Controls.Add(_pnl)

        _results = New List(Of SearchMatch)()

        InitializeBackgroundWorker()

        _tsp.MatchCase = My.Settings.FindResultsMatchCase
        _tsp.MatchWholeWord = My.Settings.FindResultsMachWholeWord
        _tsp.SearchPattern = My.Settings.FindResultsSearchPattern
        _tsp.SearchInput = My.Settings.FindResultsSearchInput
        _tsp.SearchReplacement = My.Settings.FindResultsSearchReplacement
        _tsp.SearchOutput = My.Settings.FindResultsSearchOutput

        AddHandler _tsp.FindButton.Click, Sub() FindAll()
        AddHandler _tsp.StopFindButton.Click, Sub() _bgw.CancelAsync()

        LoadHistory()

        AddHandler _rtb.CurrentLineChanged,
            Sub()
                If Not _isSearching Then
                    Dim result As SearchMatch = GetCurrentResult()
                    If result IsNot Nothing Then
                        result.Box.Select(result.Index, result.Length)
                    End If
                End If
            End Sub

        AddHandler _rtb.KeyDown, AddressOf RichTextBox_KeyDown
        AddHandler _tsp.SearchBox.KeyDown, AddressOf SearchBox_KeyDown

    End Sub

    Private Sub InitializeBackgroundWorker()

        _bgw = New BackgroundWorker() With {.WorkerReportsProgress = True, .WorkerSupportsCancellation = True}

        AddHandler _bgw.DoWork,
            Sub(sender As Object, e As DoWorkEventArgs)
                e.Cancel = Not FindAll(DirectCast(e.Argument, List(Of SearchDefinition)))
            End Sub

        AddHandler _bgw.ProgressChanged,
            Sub(sender As Object, e As ProgressChangedEventArgs)

                Dim results = DirectCast(e.UserState, List(Of SearchMatch))
                Dim sb As New StringBuilder()
                For i = 0 To results.Count - 1
                    _results.Add(results(i))
                    sb.Append(vbLf)
                    sb.Append(results(i).GetText())
                Next
                _rtb.AppendText(sb.ToString())

            End Sub

        AddHandler _bgw.RunWorkerCompleted,
            Sub(sender As Object, e As RunWorkerCompletedEventArgs)
                Debug.Assert(e.Error Is Nothing, e.Error?.StackTrace)
                _rtb.AppendText(vbLf)
                _rtb.AppendText(If(e.Cancelled, My.Resources.FindWasStoppedInProgress, My.Resources.Matches & ": " & _results.Count))
                _rtb.SelectBeginning()
                _tsp.FindButton.Enabled = True
                _tsp.StopFindButton.Enabled = False
                _isSearching = False
            End Sub

    End Sub

    Private Sub LoadHistory()

        Dim dt = DateTime.MinValue
        For Each item As String In AppSettings.FindResultsSearchPhrases()
            dt = dt.AddSeconds(1)
            _tsp.History(item) = dt
        Next

    End Sub

    Private Sub SearchBox_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Enter Then
                FindAll()
            ElseIf e.KeyCode = Keys.Down Then
                If _tsp.SearchBox.DroppedDown = False Then
                    _rtb.Select()
                    _rtb.SelectBeginning()
                End If
            End If
        End If

    End Sub

    Private Sub RichTextBox_KeyDown(sender As Object, e As KeyEventArgs)

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Up Then
                If _rtb.CurrentLineIndex = 0 Then
                    _tsp.Select()
                    _tsp.SearchBox.Focus()
                    _tsp.SearchBox.SelectAll()
                End If
            ElseIf e.KeyCode = Keys.Enter Then
                Dim result As SearchMatch = GetCurrentResult()
                If result IsNot Nothing Then

                    _rtb.SelectCurrentLine()
                    result.Box.Select(result.Index, result.Length)

                    If result.Box.Visible = False Then
                        Dim tbp As TabPage = result.Box.FindParent(Of TabPage)()
                        If tbp IsNot Nothing Then
                            Dim tbc As TabControl = tbp.FindParent(Of TabControl)()
                            If tbc IsNot Nothing Then
                                tbc.SelectedTab = tbp
                            End If
                        End If
                    End If

                    result.Box.Select()

                End If
            End If
        End If

    End Sub

    Private Function GetCurrentResult() As SearchMatch

        If _results.Count > 0 Then

            Dim index = _rtb.CurrentLineIndex - 1
            If index >= 0 AndAlso index < _results.Count Then
                Return _results(index)
            End If

        End If

        Return Nothing

    End Function

    Private Sub FindAll()

        If _tsp.SearchPhrase.Length > 0 AndAlso (_tsp.SearchPattern OrElse _tsp.SearchInput OrElse _tsp.SearchReplacement OrElse _tsp.SearchOutput) Then
            _results = New List(Of SearchMatch)()
            _rtb.Text = GetFirstLine()
            _tsp.History(_tsp.SearchPhrase) = DateTime.UtcNow
            _tsp.FindButton.Enabled = False
            _tsp.StopFindButton.Enabled = True
            _isSearching = True
            _bgw.RunWorkerAsync(New List(Of SearchDefinition)(GetFindInfos()))
        End If

    End Sub

    Private Function FindAll(definitions As List(Of SearchDefinition)) As Boolean

        For i = 0 To definitions.Count - 1

            Dim results As New List(Of SearchMatch)
            For Each result As SearchMatch In definitions(i).FindAll()

                results.Add(result)

                If _bgw.CancellationPending Then
                    _bgw.ReportProgress(results.Count, results)
                    Return False
                End If

            Next

            _bgw.ReportProgress(results.Count, results)

        Next

        Return True

    End Function

    Private Iterator Function GetFindInfos() As IEnumerable(Of SearchDefinition)

        Yield New SearchDefinition(Panels.Pattern._rtb) With {.Condition = _tsp.SearchPattern, .SearchPhrase = _tsp.SearchPhrase, .SearchOptions = _tsp.Options, .Name = My.Resources.Pattern}
        Yield New SearchDefinition(Panels.Input._rtb) With {.Condition = _tsp.SearchInput, .SearchPhrase = _tsp.SearchPhrase, .SearchOptions = _tsp.Options, .Name = My.Resources.Input}
        Yield New SearchDefinition(Panels.Replacement._rtb) With {.Condition = _tsp.SearchReplacement, .SearchPhrase = _tsp.SearchPhrase, .SearchOptions = _tsp.Options, .Name = My.Resources.Replacement}

        Dim infoLength As Integer = If(Panels.Output.Builder IsNot Nothing, Panels.Output.Builder.InfoLength, 0)
        Yield New OutputSearchDefinition(OutputText._rtb, infoLength) With {.Condition = _tsp.SearchOutput, .SearchPhrase = _tsp.SearchPhrase, .SearchOptions = _tsp.Options, .Name = My.Resources.Output}

    End Function

    <SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")>
    Public Function GetFirstLine() As String

        Dim _sb = New StringBuilder()
        _sb.Append("Find all """)
        _sb.Append(_tsp.SearchPhrase)
        _sb.Append("""")
        If (_tsp.Options And SearchOptions.MatchCase) <> 0 Then
            _sb.Append(", Match case")
        End If
        If (_tsp.Options And SearchOptions.MatchWholeWord) <> 0 Then
            _sb.Append(", Whole word")
        End If
        If _tsp.SearchPattern Then
            _sb.Append(", ")
            _sb.Append(My.Resources.Pattern)
        End If
        If _tsp.SearchInput Then
            _sb.Append(", ")
            _sb.Append(My.Resources.Input)
        End If
        If _tsp.SearchReplacement Then
            _sb.Append(", ")
            _sb.Append(My.Resources.Replacement)
        End If
        If _tsp.SearchOutput Then
            _sb.Append(", ")
            _sb.Append(My.Resources.Output)
        End If
        Return _sb.ToString()

    End Function

End Class