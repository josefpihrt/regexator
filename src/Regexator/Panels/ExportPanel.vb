Imports Regexator.Text
Imports Regexator.Windows.Forms

Public Class ExportPanel
    Implements IDisposable

    Public Sub New()

        _rtb = New ExportRichTextBox()
        _pnlBox = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.None}
        _tsp = New AppToolStrip()

        LoadExportModes()

        _btnExport = New ToolStripButton(Nothing, My.Resources.IcoExport.ToBitmap(), AddressOf CopyToClipboard) With {.ToolTipText = My.Resources.Export}
        _btnMultiline = New ToolStripButton(Nothing, My.Resources.IcoMultilineLiteral.ToBitmap(), Sub() Multiline = _btnMultiline.Checked) With {.ToolTipText = My.Resources.Multiline, .CheckOnClick = True, .Enabled = False}

        _tsp.Items.AddRange(EnumerateItems().ToArray())
        _pnlBox.Controls.Add(_rtb)
        _pnl.Controls.AddRange({_pnlBox, _tsp})

        App.Formats.Text.Controls.Add(_rtb)

    End Sub

    Private Sub LoadExportModes()

        _cmbExportMode = New ToolStripComboBox() With {.FlatStyle = FlatStyle.Flat, .ToolTipText = My.Resources.ExportMode, .DropDownStyle = ComboBoxStyle.DropDownList, .AutoSize = False, .Width = 100}
        _cmbExportMode.ComboBox.ValueMember = "Value"
        _cmbExportMode.ComboBox.DisplayMember = "Title"
        _cmbExportMode.ComboBox.DataSource = [Enum].GetValues(GetType(ExportMode)) _
            .Cast(Of ExportMode) _
            .Select(Function(f) New With {.Value = f, .Title = Text.EnumHelper.GetDescription(f)}) _
            .OrderBy(Function(f) f.Title) _
            .ToList()
        AddHandler _cmbExportMode.ComboBox.SelectedValueChanged,
            Sub()
                Mode = DirectCast(_cmbExportMode.ComboBox.SelectedValue, ExportMode)
                _btnMultiline.Enabled = (Mode <> ExportMode.CSharp)
            End Sub

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _btnExport
        Yield New ToolStripSeparator()
        Yield _cmbExportMode
        Yield _btnMultiline

    End Function

    Public Sub LoadSettings()

        _suppress = True
        Mode = My.Settings.CodeExportMode
        Multiline = My.Settings.CodeExportMultiline
        _suppress = False
        Execute()

    End Sub

    Private Sub Execute()

        If _suppress = False Then
            If PatternText IsNot Nothing Then
                If Mode = Text.ExportMode.VisualBasic Then
                    _rtb.Text = VisualBasicBuilder.GetText(PatternText, Settings)
                Else
                    _rtb.Text = CSharpBuilder.GetText(PatternText, Settings)
                End If
            Else
                _rtb.Clear()
            End If
        End If

    End Sub

    Private Sub CopyToClipboard(sender As Object, e As EventArgs)

        If String.IsNullOrEmpty(_rtb.Text) = False Then
            AppUtility.SetClipboardText(_rtb.GetTextCrLf())
        End If

    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose

        Dispose(True)
        GC.SuppressFinalize(Me)

    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)

        If _disposed = False Then
            If disposing Then
                _rtb.Dispose()
                _rtb = Nothing
                _tsp.Dispose()
                _tsp = Nothing
                _btnExport.Dispose()
                _btnExport = Nothing
                _cmbExportMode.Dispose()
                _cmbExportMode = Nothing
                _btnMultiline.Dispose()
                _btnMultiline = Nothing
            End If
            _disposed = True
        End If

    End Sub

    Public Property PatternText As String
        Get
            Return _patternText
        End Get
        Set(value As String)
            _patternText = value
            Execute()
        End Set
    End Property

    Public Property Mode As ExportMode
        Get
            Return _mode
        End Get
        Set(value As ExportMode)
            If _mode <> value Then
                _mode = value
                _cmbExportMode.ComboBox.SelectedValue = _mode
                My.Settings.CodeExportMode = _mode
                Execute()
            End If
        End Set
    End Property

    Public Property Multiline As Boolean
        Get
            Return _multiline
        End Get
        Set(value As Boolean)
            If _multiline <> value Then
                _multiline = value
                _btnMultiline.Checked = _multiline
                My.Settings.CodeExportMultiline = _multiline
                Execute()
            End If
        End Set
    End Property

    Public ReadOnly Property Settings As CodeBuilderSettings
        Get
            Return New CodeBuilderSettings() With {
                .Verbatim = (Mode = ExportMode.CSharpVerbatim),
                .Multiline = Multiline AndAlso (Mode <> ExportMode.CSharp)
            }
        End Get
    End Property

    Friend _rtb As ExportRichTextBox
    Friend _pnlBox As Panel
    Friend _pnl As Panel
    Friend _tsp As AppToolStrip

    Friend _btnExport As ToolStripButton
    Friend _cmbExportMode As ToolStripComboBox
    Friend _btnMultiline As ToolStripButton

    Private _multiline As Boolean
    Private _mode As ExportMode
    Private _patternText As String
    Private _disposed As Boolean
    Private _suppress As Boolean

End Class
