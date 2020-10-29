Imports Regexator

Public Class OutputToolStrip
    Inherits AppToolStrip

    Public Sub New()

        MyBase.New()

        _btnOutputEnabled = New ToolStripButton(Nothing, My.Resources.IcoMediaPlay.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.ToggleOutput & " " & My.Resources.AltF2.AddParentheses()}
        _btnLimitEnabled = New ToolStripButton(Nothing, My.Resources.IcoLimit.ToBitmap()) With {.Checked = My.Settings.OutputLimitEnabled, .CheckOnClick = True, .ToolTipText = My.Resources.ToggleLimitation}
        _btnToggleHighlighting = New ToolStripButton(Nothing, My.Resources.IcoPaintRoller.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.ToggleHighlighting}

        _tssHighlight = New ToolStripSplitButton(Nothing, My.Resources.IcoHighlightAll.ToBitmap()) With {.Enabled = False, .ToolTipText = My.Resources.Highlight & " " & My.Resources.CtrlG.AddParentheses()}
        Dim tsiHighlightAll As New ToolStripMenuItem(My.Resources.HighlightAll, Nothing, Sub() Panels.Output.Highlight(HighlightOptions.None)) With {.ShortcutKeyDisplayString = My.Resources.CtrlShiftG}
        _tssHighlight.DropDownItems.Add(tsiHighlightAll)

        _btnInfo = New ToolStripButton(Nothing, My.Resources.IcoInfo.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.ToggleInfo}
        _btnSymbols = New ToolStripButton(Nothing, My.Resources.IcoArrows.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.ToggleSymbols}
        _btnCarriageReturnSymbol = New ToolStripButton(Nothing, My.Resources.IcoArrowLeftSmall.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = EnumHelper.GetDescription(OutputOptions.CarriageReturnSymbol)}
        _btnLinefeedSymbol = New ToolStripButton(Nothing, My.Resources.IcoArrowDownSmall.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = EnumHelper.GetDescription(OutputOptions.LinefeedSymbol)}
        _btnTabSymbol = New ToolStripButton(Nothing, My.Resources.IcoArrowRightSmall.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = EnumHelper.GetDescription(OutputOptions.TabSymbol)}
        _btnSpaceSymbol = New ToolStripButton(Nothing, My.Resources.IcoSpaceSymbol.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = EnumHelper.GetDescription(OutputOptions.SpaceSymbol)}
        _btnWordWrap = New ToolStripButton(Nothing, My.Resources.IcoWordWrap.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.ToggleWordWrap}
        _lblInfo = New ToolStripLabel() With {.Alignment = ToolStripItemAlignment.Right}

        _btnGroupLayout = New ToolStripButton(Nothing, My.Resources.IcoGroupLayout.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.GroupLayout, .Visible = False}
        _btnValueLayout = New ToolStripButton(Nothing, My.Resources.IcoValueLayout.ToBitmap()) With {.CheckOnClick = True, .ToolTipText = My.Resources.ValueLayout, .Visible = False}

        Items.AddRange(EnumerateItems().ToArray())

        AddHandler _btnOutputEnabled.CheckedChanged, Sub() Panels.Output.OutputEnabled = _btnOutputEnabled.Checked
        AddHandler _btnLimitEnabled.CheckedChanged, Sub() LimitEnabled = _btnLimitEnabled.Checked
        AddHandler _btnToggleHighlighting.CheckedChanged, AddressOf OptionsButton_CheckedChanged
        AddHandler _tssHighlight.Click, Sub() Panels.Output.Highlight()
        AddHandler _btnInfo.CheckedChanged, AddressOf OptionsButton_CheckedChanged
        AddHandler _btnCarriageReturnSymbol.CheckedChanged, AddressOf OptionsButton_CheckedChanged
        AddHandler _btnLinefeedSymbol.CheckedChanged, AddressOf OptionsButton_CheckedChanged
        AddHandler _btnTabSymbol.CheckedChanged, AddressOf OptionsButton_CheckedChanged
        AddHandler _btnSpaceSymbol.CheckedChanged, AddressOf OptionsButton_CheckedChanged
        AddHandler _btnSymbols.Click, AddressOf Symbols_Click

    End Sub

    Private Iterator Function EnumerateItems() As IEnumerable(Of ToolStripItem)

        Yield _btnOutputEnabled
        Yield _btnLimitEnabled
        Yield New ToolStripSeparator()
        Yield _btnToggleHighlighting
        Yield _tssHighlight
        Yield _btnInfo
        Yield New ToolStripSeparator()
        Yield _btnSymbols
        Yield _btnCarriageReturnSymbol
        Yield _btnLinefeedSymbol
        Yield _btnTabSymbol
        Yield _btnSpaceSymbol
        Yield New ToolStripSeparator()
        Yield _btnWordWrap
        Yield _lblInfo
        Yield _btnGroupLayout
        Yield _btnValueLayout

    End Function

    Public Function HasOptions(options As OutputOptions) As Boolean

        Return ((Me.Options And options) = options)

    End Function

    Public Sub AddOptions(options As OutputOptions)

        Me.Options = Me.Options Or options

    End Sub

    Public Sub RemoveOptions(options As OutputOptions)

        Me.Options = Me.Options And (Not options)

    End Sub

    Public Sub EnableSymbols()

        AddOptions(Symbols)

    End Sub

    Public Sub DisableSymbols()

        RemoveOptions(Symbols)

    End Sub

    Public Sub ShowTableLayoutItems()

        _btnGroupLayout.Visible = True
        _btnValueLayout.Visible = True

    End Sub

    Public Sub HideTableLayoutItems()

        _btnGroupLayout.Visible = False
        _btnValueLayout.Visible = False

    End Sub

    Public Property Options As OutputOptions
        Get
            Return _options
        End Get
        Set(value As OutputOptions)
            value = value And (Not OutputOptions.NoCaptureSymbol)
            value = value And (Not OutputOptions.WrapText)
            If value <> _options Then
                _inSetValue = True
                _btnToggleHighlighting.Checked = ((value And OutputOptions.Highlight) = OutputOptions.Highlight)
                _tssHighlight.Enabled = ((value And OutputOptions.Highlight) = OutputOptions.Highlight)
                _btnInfo.Checked = ((value And OutputOptions.Info) = OutputOptions.Info)
                _btnCarriageReturnSymbol.Checked = ((value And OutputOptions.CarriageReturnSymbol) = OutputOptions.CarriageReturnSymbol)
                _btnLinefeedSymbol.Checked = ((value And OutputOptions.LinefeedSymbol) = OutputOptions.LinefeedSymbol)
                _btnTabSymbol.Checked = ((value And OutputOptions.TabSymbol) = OutputOptions.TabSymbol)
                _btnSpaceSymbol.Checked = ((value And OutputOptions.SpaceSymbol) = OutputOptions.SpaceSymbol)
                _options = value
                _btnSymbols.Checked = HasOptions(Symbols)
                _inSetValue = False
                Panels.Output.LoadData()
            End If
        End Set
    End Property

    Private Sub OptionsButton_CheckedChanged(sender As Object, e As EventArgs)

        If _inSetValue = False Then
            Dim value As OutputOptions = OutputOptions.None
            If _btnToggleHighlighting.Checked Then
                value = value Or OutputOptions.Highlight
            End If
            If _btnInfo.Checked Then
                value = value Or OutputOptions.Info
            End If
            If _btnCarriageReturnSymbol.Checked Then
                value = value Or OutputOptions.CarriageReturnSymbol
            End If
            If _btnLinefeedSymbol.Checked Then
                value = value Or OutputOptions.LinefeedSymbol
            End If
            If _btnTabSymbol.Checked Then
                value = value Or OutputOptions.TabSymbol
            End If
            If _btnSpaceSymbol.Checked Then
                value = value Or OutputOptions.SpaceSymbol
            End If
            Options = value
        End If

    End Sub

    Private Sub Symbols_Click(sender As Object, e As EventArgs)

        If HasOptions(Symbols) Then
            DisableSymbols()
        Else
            EnableSymbols()
        End If

    End Sub

    Public Property LimitEnabled As Boolean
        Get
            Return _limitEnabled
        End Get
        Set(value As Boolean)
            If _limitEnabled <> value Then
                _limitEnabled = value
                _btnLimitEnabled.Checked = value
                Panels.Output.LoadData()
            End If
        End Set
    End Property

    Private _options As OutputOptions
    Private _inSetValue As Boolean
    Private _limitEnabled As Boolean = My.Settings.OutputLimitEnabled

    Friend _btnOutputEnabled As ToolStripButton
    Friend _btnLimitEnabled As ToolStripButton
    Friend _btnToggleHighlighting As ToolStripButton
    Friend _tssHighlight As ToolStripSplitButton
    Friend _btnInfo As ToolStripButton
    Friend _btnSymbols As ToolStripButton
    Friend _btnCarriageReturnSymbol As ToolStripButton
    Friend _btnLinefeedSymbol As ToolStripButton
    Friend _btnTabSymbol As ToolStripButton
    Friend _btnSpaceSymbol As ToolStripButton
    Friend _lblInfo As ToolStripLabel
    Friend _btnWordWrap As ToolStripButton

    Friend _btnGroupLayout As ToolStripButton
    Friend _btnValueLayout As ToolStripButton

    Private Shared ReadOnly Symbols As OutputOptions = OutputOptions.CarriageReturnSymbol Or OutputOptions.LinefeedSymbol Or OutputOptions.TabSymbol

End Class
