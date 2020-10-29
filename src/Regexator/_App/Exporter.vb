Imports System.Collections.ObjectModel
Imports System.Diagnostics.CodeAnalysis
Imports System.Text.RegularExpressions
Imports Regexator.Text
Imports Regexator.Windows.Forms

Public Class Exporter

    Public Sub New(rtb As RegexRichTextBox)

        _rtb = rtb

    End Sub

    Public Function Export(mode As ExportMode) As String

        Return Export(mode, DefaultKind)

    End Function

    Public Function Export(mode As ExportMode, kind As TextKind) As String

        Return CreateBuilder(mode).GetText(GetText(kind)).EnsureCarriageReturnLinefeed()

    End Function

    Public Sub ExportToClipboard()

        ExportToClipboard(DefaultKind)

    End Sub

    Public Sub ExportToClipboard(kind As TextKind)

        ExportToClipboard(DefaultMode, kind)

    End Sub

    Public Sub ExportToClipboard(mode As ExportMode)

        ExportToClipboard(mode, DefaultKind)

    End Sub

    Public Sub ExportToClipboard(mode As ExportMode, kind As TextKind)

        AppUtility.SetClipboardText(Export(mode, kind))

    End Sub

    Public Shared Function Import(value As String, mode As ExportMode) As String

        Return CodeImporter.Import(value, mode)

    End Function

    Public Sub ImportFromClipboard()

        ImportFromClipboard(DefaultMode)

    End Sub

    <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
    Public Sub ImportFromClipboard(mode As ExportMode)

        Dim s As String = AppUtility.GetClipboardText()
        If s IsNot Nothing Then
            Try
                _rtb.SelectedText = Import(s, mode)
            Catch ex As Exception
                Debug.WriteLine(ex.CreateLog())
                MessageDialog.Warning(My.Resources.UnableToImportCodeMsg)
            End Try
        End If

    End Sub

    Public Function Test(mode As ExportMode) As Boolean

        Dim exported = Export(mode)
        Dim imported = Import(exported, mode)
        Return GetText(TextKind.SelectionOrCurrent).EnsureCarriageReturnLinefeed() = imported

    End Function

    <SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")>
    Public Sub Test()

        Dim s As String = String.Empty
        For Each mode As ExportMode In Modes
            s &= Text.EnumHelper.GetDescription(mode) & My.Resources.ColonStr & " "
            Try
                s &= Test(mode)
            Catch ex As Exception
                s &= ex.GetBaseException().Message
            End Try
            s &= vbLf
        Next
        MessageDialog.Info(s)

    End Sub

    Public Shared Sub ExportRegexConstructor(mode As ExportMode)

        Dim builder = CreateBuilder(mode)
        builder.AppendRegexConstructor(Pattern, RegexOptions)
        AppUtility.SetClipboardText(builder.ToString().EnsureCarriageReturnLinefeed())

    End Sub

    Public Shared Sub ExportInstanceMethod(mode As ExportMode)

        Dim builder = CreateBuilder(mode)
        Select Case Exporter.Mode
            Case EvaluationMode.Match
                builder.AppendInstanceMatch(Input, Pattern, RegexOptions)
            Case EvaluationMode.Replace
                builder.AppendInstanceReplace(Input, Pattern, Replacement, Panels.Replacement.ReplacementMode, RegexOptions)
            Case EvaluationMode.Split
                builder.AppendInstanceSplit(Input, Pattern, RegexOptions)
        End Select
        AppUtility.SetClipboardText(builder.ToString().EnsureCarriageReturnLinefeed())

    End Sub

    Public Shared Sub ExportInstanceIsMatch(mode As ExportMode)

        Dim builder = CreateBuilder(mode)
        builder.AppendInstanceIsMatch(Input, Pattern, RegexOptions)
        AppUtility.SetClipboardText(builder.ToString().EnsureCarriageReturnLinefeed())

    End Sub

    Public Shared Sub ExportStaticMethod(mode As ExportMode)

        Dim builder = CreateBuilder(mode)
        Select Case Exporter.Mode
            Case EvaluationMode.Match
                builder.AppendStaticMatch(Input, Pattern, RegexOptions)
            Case EvaluationMode.Replace
                builder.AppendStaticReplace(Input, Pattern, Replacement, Panels.Replacement.ReplacementMode, RegexOptions)
            Case EvaluationMode.Split
                builder.AppendStaticSplit(Input, Pattern, RegexOptions)
        End Select
        AppUtility.SetClipboardText(builder.ToString().EnsureCarriageReturnLinefeed())

    End Sub

    Public Shared Sub ExportStaticIsMatch(mode As ExportMode)

        Dim builder = CreateBuilder(mode)
        builder.AppendStaticIsMatch(Input, Pattern, RegexOptions)
        AppUtility.SetClipboardText(builder.ToString().EnsureCarriageReturnLinefeed())

    End Sub

    Public Function CreateExportItems(kind As TextKind, parent As ToolStripMenuItem) As ToolStripItem()

        If parent Is Nothing Then Throw New ArgumentNullException("parent")
        Dim items = Modes.Select(Function(mode) CreateExportItem(mode, kind)).ToArray()
        AddHandler parent.DropDownOpening,
            Sub()
                For Each item In items
                    item.ShortcutKeyDisplayString = If(DirectCast(item.Tag, ExportMode) = DefaultMode, My.Resources.CtrlShiftC, String.Empty)
                Next
            End Sub
        Return items

    End Function

    Private Function CreateExportItem(mode As ExportMode, kind As TextKind) As ToolStripMenuItem

        Return New ToolStripMenuItem(Text.EnumHelper.GetDescription(mode), Nothing, Sub() ExportToClipboard(mode, kind)) With {.Tag = mode}

    End Function

    Public Function CreateImportItem(text As String) As ToolStripMenuItem

        Dim items = CreateImportItems().ToArray()
        Dim item As New ToolStripMenuItem(text, Nothing, items)
        AddHandler item.DropDownOpening,
            Sub()
                For Each tsi In items
                    Dim mode = DirectCast(tsi.Tag, ExportMode)
                    Dim mode2 = If(DefaultMode = ExportMode.CSharpVerbatim, ExportMode.CSharp, DefaultMode)
                    tsi.ShortcutKeyDisplayString = If(mode = mode2, My.Resources.CtrlShiftC, String.Empty)
                Next
            End Sub
        Return item

    End Function

    Public Iterator Function CreateImportItems() As IEnumerable(Of ToolStripMenuItem)

        Yield New ToolStripMenuItem(My.Resources.CSharpAbbr, Nothing, Sub() ImportFromClipboard(ExportMode.CSharp)) With {.Tag = ExportMode.CSharp}
        Yield New ToolStripMenuItem(My.Resources.VisualBasic, Nothing, Sub() ImportFromClipboard(ExportMode.VisualBasic)) With {.Tag = ExportMode.VisualBasic}

    End Function

#If TATA Then
    Public Function CreateTestItem() As ToolStripMenuItem

        Return New ToolStripMenuItem("Test Export/Import", Nothing, Sub() Test())

    End Function
#End If

    Public Function CreateDefaultItems() As IEnumerable(Of ToolStripItem)

        Return CreateItems(DefaultMode, True, True)

    End Function

    Public Shared Function CreateDefaultModeItem() As ToolStripMenuItem

        Dim item As New ToolStripMenuItem(My.Resources.Mode, Nothing, New ToolStripMenuItem())
        AddHandler item.DropDownOpening, Sub() item.DropDownItems.LoadItems(CreateDefaultModeItems())
        Return item

    End Function

    Public Shared Function CreateDefaultModeItems() As IEnumerable(Of ToolStripMenuItem)

        Return Modes.Select(Function(f) New ToolStripMenuItem(Text.EnumHelper.GetDescription(f), Nothing, Sub() DefaultMode = f) With {.Checked = (f = DefaultMode)})

    End Function

    Public Function CreateItems() As IEnumerable(Of ToolStripItem)

        Return Modes.Select(Function(f) New ToolStripMenuItem(Text.EnumHelper.GetDescription(f), Nothing, CreateItems(f).ToArray()))

    End Function

    Private Function CreateItems(mode As ExportMode) As IEnumerable(Of ToolStripItem)

        Return CreateItems(mode, False, False)

    End Function

    Private Iterator Function CreateItems(mode As ExportMode, addMode As Boolean, importAlways As Boolean) As IEnumerable(Of ToolStripItem)

        Yield New ToolStripMenuItem(My.Resources.Export, Nothing, Sub() ExportToClipboard(mode))
        If importAlways OrElse mode <> ExportMode.CSharpVerbatim Then
            Yield New ToolStripMenuItem(My.Resources.Import, Nothing, Sub() ImportFromClipboard(mode))
        End If
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(My.Resources.Constructor, Nothing, Sub() ExportRegexConstructor(mode))
        Yield New ToolStripSeparator()
        Yield New ToolStripMenuItem(EnumHelper.GetDescription(Exporter.Mode) & " " & My.Resources.Instance.AddParentheses(), Nothing, Sub() ExportInstanceMethod(mode))
        Yield New ToolStripMenuItem(EnumHelper.GetDescription(Exporter.Mode) & " " & My.Resources.Static_.AddParentheses(), Nothing, Sub() ExportStaticMethod(mode))
        If Exporter.Mode = EvaluationMode.Match Then
            Yield New ToolStripSeparator()
            Yield New ToolStripMenuItem(CodeBuilder.IsMatchMethod & " " & My.Resources.Instance.AddParentheses(), Nothing, Sub() ExportInstanceIsMatch(mode))
            Yield New ToolStripMenuItem(CodeBuilder.IsMatchMethod & " " & My.Resources.Static_.AddParentheses(), Nothing, Sub() ExportStaticIsMatch(mode))
        End If

    End Function

    Public Shared Function CreateBuilder(mode As ExportMode) As CodeBuilder

        If mode = ExportMode.CSharp OrElse mode = ExportMode.CSharpVerbatim Then
            Return New CSharpBuilder(GetSettings(mode))
        ElseIf mode = ExportMode.VisualBasic Then
            Return New VisualBasicBuilder(GetSettings(mode))
        End If
        Return Nothing

    End Function

    Private Shared Function GetSettings(mode As ExportMode) As CodeBuilderSettings

        Dim value = Panels.Export.Settings
        value.Verbatim = (mode = ExportMode.CSharpVerbatim)
        Return value

    End Function

    Private Shared ReadOnly Property Mode As EvaluationMode
        Get
            Return App.Mode
        End Get
    End Property

    Private ReadOnly Property GetText(kind As TextKind) As String
        Get
            Select Case kind
                Case TextKind.SelectionOrCurrent
                    Return GetSelectedOrCurrentText(_rtb)
                Case Else
                    Return _rtb.Text
            End Select
        End Get
    End Property

    Private Shared Function GetSelectedOrCurrentText(rtb As RegexRichTextBox) As String

        Return If(rtb.SelectionLength > 0, rtb.SelectedText, rtb.CurrentText(NewLineMode.Lf))

    End Function

    Private Shared ReadOnly Property Input As String
        Get
            Return GetSelectedOrCurrentText(Panels.Input._rtb)
        End Get
    End Property

    Private Shared ReadOnly Property Pattern As String
        Get
            Return GetSelectedOrCurrentText(Panels.Pattern._rtb)
        End Get
    End Property

    Private Shared ReadOnly Property Replacement As String
        Get
            Return GetSelectedOrCurrentText(Panels.Replacement._rtb)
        End Get
    End Property

    Private Shared ReadOnly Property RegexOptions As RegexOptions
        Get
            Return App.RegexOptionsManager.Value
        End Get
    End Property

    Private Shared Property DefaultMode As ExportMode
        Get
            Return My.Settings.CodeExportMode
        End Get
        Set(value As ExportMode)
            My.Settings.CodeExportMode = value
        End Set
    End Property

    Public Property IsPattern As Boolean

    Private ReadOnly _rtb As RegexRichTextBox

    Private Shared ReadOnly Modes As ReadOnlyCollection(Of ExportMode) = Array.AsReadOnly([Enum].GetValues(GetType(ExportMode)).Cast(Of ExportMode).ToArray())
    Private Shared ReadOnly DefaultKind As TextKind = TextKind.SelectionOrCurrent

End Class
