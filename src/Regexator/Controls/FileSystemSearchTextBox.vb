Imports Regexator.IO

Public NotInheritable Class FileSystemSearchTextBox
    Inherits ToolStripTextBox

    Friend _dgv As FileSystemSearchDataGridView = New FileSystemSearchDataGridView()

    Private _searchPhrase As String = String.Empty
    Private _hideOnLostFocus As Boolean = True

    Private Shared ReadOnly DefaultText As String = My.Resources.SearchFiles & My.Resources.EllipsisStr
    Private Shared ReadOnly DefaultTextForeColor As Color = Color.Gray

    Friend Sub New()

        MyBase.New()
        Text = DefaultText
        ForeColor = DefaultTextForeColor

        AddHandler _dgv.DataSourceChanged,
            Sub()
                If _dgv.DataSource IsNot Nothing Then
                    Dim tsp As ToolStrip = Me.Parent
                    Dim pt As Point = tsp.Parent.PointToScreen(tsp.Location)
                    pt.Offset(0, tsp.Height)
                    _dgv.ShowGrid(App.MainForm.PointToClient(pt))
                Else
                    _dgv.HideGrid()
                End If
            End Sub

        _dgv.Size = New Size(450, 250)

        AddHandler _dgv.KeyDown,
            Sub(sender As Object, e As KeyEventArgs)
                If e.Modifiers = Keys.None Then
                    If e.KeyCode = Keys.Up Then
                        If _dgv.SelectedRows.Count = 0 OrElse _dgv.SelectedRows(0).Index = 0 Then
                            _dgv.HideOnLostFocus = False
                            Me.Parent.Select()
                            Me.Focus()
                            _dgv.HideOnLostFocus = True
                        End If
                    ElseIf e.KeyCode = Keys.Escape Then
                        _dgv.HideOnLostFocus = False
                        Me.Parent.Select()
                        Me.Focus()
                        _dgv.HideOnLostFocus = True
                    ElseIf e.KeyCode = Keys.Enter Then
                        LoadSelectedItem()
                    End If
                End If
            End Sub

        AddHandler _dgv.MouseDoubleClick,
            Sub(sender As Object, e As MouseEventArgs)
                If (e.Button And MouseButtons.Left) <> 0 Then
                    Me.LoadSelectedItem()
                End If
            End Sub

    End Sub

    Public Sub LoadSelectedItem()

        If _dgv.SelectedRows.Count = 1 Then
            Explorer.Load(DirectCast(_dgv.SelectedRows(0).DataBoundItem, SearchResult))
            Clear()
        End If

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")

        If e.Modifiers = Keys.None Then
            If e.KeyCode = Keys.Enter Then
                If SearchPhrase <> _dgv.SearchPhrase Then
                    _dgv.SearchPhrase = SearchPhrase
                Else
                    LoadSelectedItem()
                End If
            ElseIf e.KeyCode = Keys.Down Then
                HideOnLostFocus = False
                _dgv.Select()
                If _dgv.SelectedRows.Count = 0 AndAlso _dgv.Rows.Count > 0 Then
                    _dgv.Rows(0).Selected = True
                End If
                HideOnLostFocus = True
            End If
        End If

        MyBase.OnKeyDown(e)

    End Sub

    Protected Overrides Function ProcessCmdKey(ByRef m As Message, keyData As Keys) As Boolean

        Dim modifiers = keyData And Keys.Modifiers
        Dim keyCode = keyData And Keys.KeyCode
        If modifiers = Keys.None Then
            If keyCode = Keys.Escape Then
                If TextLength > 0 Then
                    Clear()
                    Return True
                End If
            End If
        End If

        Return MyBase.ProcessCmdKey(m, keyData)

    End Function

    Protected Overrides Sub OnTextChanged(e As EventArgs)

        SearchPhrase = Text.Trim()

        If SearchPhrase = String.Empty AndAlso Me.Focused = False Then
            ForeColor = DefaultTextForeColor
            Text = DefaultText
        End If

        MyBase.OnTextChanged(e)

    End Sub

    Protected Overrides Sub OnLostFocus(e As EventArgs)

        If HideOnLostFocus AndAlso IsGridClickedWithLeftMouseButton() = False Then
            _dgv.HideGrid()
        End If

        MyBase.OnLostFocus(e)

    End Sub

    Private Function IsGridClickedWithLeftMouseButton() As Boolean

        If (Form.MouseButtons And MouseButtons.Left) = 0 Then
            Return False
        End If

        If _dgv.Visible = False Then
            Return False
        End If

        If _dgv.Parent Is Nothing Then
            Return False
        End If

        Dim gridRectangle = New Rectangle(_dgv.Parent.PointToScreen(_dgv.Location), _dgv.Size)

        Return gridRectangle.Contains(Control.MousePosition)

    End Function

    Protected Overrides Sub OnEnter(e As EventArgs)

        If ForeColor = DefaultTextForeColor Then
            Text = String.Empty
        End If

        ForeColor = Control.DefaultForeColor

        MyBase.OnEnter(e)

    End Sub

    Protected Overrides Sub OnLeave(e As EventArgs)

        If SearchPhrase = String.Empty Then
            ForeColor = DefaultTextForeColor
            Text = DefaultText
        Else
            ForeColor = Control.DefaultForeColor
        End If

        MyBase.OnLeave(e)

    End Sub

    Public Property SearchPhrase As String
        Get
            Return _searchPhrase
        End Get
        Private Set(value As String)
            If _searchPhrase <> value Then
                _searchPhrase = value
            End If
        End Set
    End Property

    Public Property HideOnLostFocus() As Boolean
        Get
            Return _hideOnLostFocus
        End Get
        Set(ByVal value As Boolean)
            _hideOnLostFocus = value
        End Set
    End Property

End Class
