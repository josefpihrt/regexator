Imports Regexator.Output

Public Class OutputTablePanel

    Public Sub New()

        _dgv = New OutputDataGridView() With {.BorderStyle = BorderStyle.None}
        _pnl = New Panel() With {.Dock = DockStyle.Fill, .BorderStyle = BorderStyle.FixedSingle}

        AddHandler _dgv.SelectionChanged,
            Sub()
                If Panels.Output.Suppressed = False Then
                    Dim block = FindBlock()
                    If block IsNot Nothing Then
                        Panels.Output.Highlight(block, HighlightSource.Table)
                    End If
                End If
            End Sub
        _pnl.Controls.Add(_dgv)

    End Sub

    Private Function FindBlock() As RegexBlock

        Dim key As String = _dgv.SelectionKey
        If key IsNot Nothing Then
            Return If(Panels.Output.FindBlock(key), Panels.Output.FindBlock(key & "0"))
        End If
        Return Nothing

    End Function

    Public Sub ClearData()

        _dgv.ClearData()

    End Sub

    Public Sub Highlight(block As RegexBlock)

        _dgv.Highlight(block)

    End Sub

    Public Sub EnsureDisplayed()

        _dgv.EnsureDisplayed()

    End Sub

    Public Sub LoadTable()

        Dim result = Panels.Output.Builder
        If Panels.Output.OutputEnabled AndAlso result IsNot Nothing AndAlso _dgv.Loaded = False Then
            _dgv.Load(result)
        End If

    End Sub

    Public Sub CopyTable()

        LoadTable()
        _dgv.CopyAll()

    End Sub

    Public Property Options As OutputOptions
        Get
            Dim value = OutputOptions.None
            If _dgv.WrapText Then
                value = value Or OutputOptions.WrapText
            End If
            If _dgv.UseNoCaptureSymbol Then
                value = value Or OutputOptions.NoCaptureSymbol
            End If
            Return value
        End Get
        Set(value As OutputOptions)
            _dgv.WrapText = ((value And OutputOptions.WrapText) = OutputOptions.WrapText)
            _dgv.UseNoCaptureSymbol = ((value And OutputOptions.NoCaptureSymbol) = OutputOptions.NoCaptureSymbol)
        End Set
    End Property

    Friend _dgv As OutputDataGridView
    Friend _pnl As Panel

End Class
