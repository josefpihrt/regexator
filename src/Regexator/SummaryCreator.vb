Imports System.Collections.ObjectModel
Imports System.Diagnostics.CodeAnalysis
Imports Regexator.Output
Imports Regexator.Text
Imports Regexator.Text.RegularExpressions
Imports Regexator.Windows.Forms

Public Class SummaryCreator

    Public Sub New(blocks As ReadOnlyCollection(Of RegexBlock))

        _blocks = blocks

    End Sub

    Public Sub CreateSummary()

        _info = GetSummaryInfo()
        _inputProcessor = New InputTextProcessor(_info.Input, Panels.Output.OutputSettings, GetInputSelections())
        _info.Input = _inputProcessor.Text

        Using New RedrawDisabler(_rtb)
            _builder = New TextBoxSummaryBuilder()
            _builder.Settings.NewLine = vbLf
            _builder.CreateSummary(_info, _rtb)
            Highlight()
            _rtb.SelectBeginning()
        End Using

    End Sub

    Private Sub Highlight()

        App.Formats.SummaryHeading.Highlight(_rtb, _builder.EnumerateHeadingSpans())
        HighlightValue()
        If Panels.Output.HasOptions(OutputOptions.Highlight) Then
            HighlightInput()
            HighlightOutput()
        End If

    End Sub

    Private Sub HighlightValue()

        For Each element In {SummaryElements.Pattern, SummaryElements.Input, SummaryElements.Replacement, SummaryElements.Output}.Select(Function(f) _builder.GetElement(f)).Where(Function(f) f IsNot Nothing)
            If element.ValueIsEmptyCaption Then
                _rtb.Select(element.ValueSpan)
                _rtb.SelectionFont = New Font(If(_rtb.SelectionFont, _rtb.Font), FontStyle.Italic)
            Else
                App.Formats.SummaryValue.Highlight(_rtb, GetValueSelections(element))
            End If
        Next

    End Sub

    Private Iterator Function GetValueSelections(element As SummaryElement) As IEnumerable(Of TextSpan)

        If element.Element = SummaryElements.Output AndAlso Mode <> EvaluationMode.Replace Then
            For Each item In _blocks.SelectMany(Function(f) f.TextSpans).Offset(OutputElement.ValueIndex)
                Yield item
            Next
        Else
            Yield element.ValueSpan
        End If

    End Function

    Private Sub HighlightInput()

        App.Formats.Capture.Highlight(_rtb, _inputProcessor.TextSpans.Offset(InputElement.ValueIndex))
        App.Formats.Symbol.Highlight(_rtb, _inputProcessor.SymbolSpans.Offset(InputElement.ValueIndex))

    End Sub

    Private Sub HighlightOutput()

        If App.Mode = EvaluationMode.Replace Then
            Dim builder = Panels.Output.Builder
            If builder IsNot Nothing Then
                App.Formats.Capture.Highlight(_rtb, builder.Blocks.SelectMany(Function(f) f.TextSpans).Offset(OutputElement.ValueIndex))
                App.Formats.Symbol.Highlight(_rtb, builder.Blocks.SelectMany(Function(f) f.SymbolTextSpans).Offset(OutputElement.ValueIndex))
            End If
        End If

    End Sub

    Private Function GetInputSelections() As IEnumerable(Of TextSpan)

        If _info.Mode = EvaluationMode.Match AndAlso _info.GroupSettings.IsZeroIgnored = False Then
            Return _blocks.Cast(Of CaptureBlock).Where(Function(f) f.IsDefaultCapture).Select(Function(f) f.ValueSpan)
        Else
            Return _blocks.Select(Function(f) f.ValueSpan)
        End If

    End Function

    Private Shared Function GetSummaryInfo() As SummaryInfo

        Dim info As SummaryInfo = Nothing
        If Panels.Pattern.RegexInfo IsNot Nothing Then
            info = New SummaryInfo(Panels.Pattern.RegexInfo, Groups.GroupSettings) With {
                .ProjectInfo = Panels.Info.ProjectInfo,
                .Input = Panels.Input.CurrentText,
                .Mode = Mode,
                .Output = OutputText.Text,
                .Replacement = If(Mode = EvaluationMode.Replace, TextProcessor.ProcessSymbols(Panels.Replacement.CurrentText, Panels.Output.OutputSettings), String.Empty),
                .ReplacementMode = If(Mode = EvaluationMode.Replace, Panels.Replacement.ReplacementMode, ReplacementMode.None)}
            For Each item In GetUnsuccessGroups()
                info.UnsuccessGroups.Add(item.Name)
            Next
        End If
        Return info

    End Function

    <SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")>
    Public Shared Iterator Function GetUnsuccessGroups() As IEnumerable(Of GroupInfo)

        Dim matchBuilder = TryCast(Panels.Output.Builder, MatchBuilder)
        If matchBuilder IsNot Nothing Then
            For Each groupInfo As GroupInfo In matchBuilder.Data.UnsuccessGroups
                Yield groupInfo
            Next
        Else
            Dim splitBuilder = TryCast(Panels.Output.Builder, SplitBuilder)
            If splitBuilder IsNot Nothing Then
                For Each groupInfo As GroupInfo In splitBuilder.Data.UnsuccessGroups
                    Yield groupInfo
                Next
            End If

        End If

    End Function

    Private ReadOnly Property InputElement As SummaryElement
        Get
            Return _builder.GetElement(SummaryElements.Input)
        End Get
    End Property

    Private ReadOnly Property OutputElement As SummaryElement
        Get
            Return _builder.GetElement(SummaryElements.Output)
        End Get
    End Property

    Public ReadOnly Property Searcher As Int32BinarySearcher
        Get
            Return New Int32BinarySearcher(_builder.EnumerateHeadingIndexes(True))
        End Get
    End Property

    Private ReadOnly _rtb As SummaryRichTextBox = Summary._rtb
    Private ReadOnly _blocks As ReadOnlyCollection(Of RegexBlock)
    Private _info As SummaryInfo
    Private _builder As TextBoxSummaryBuilder
    Private _inputProcessor As InputTextProcessor

End Class
