Imports Regexator.UI
Imports Regexator.Windows.Forms

Public Class SwitcherContextMenuStrip
    Inherits ContextMenuStrip

    Public Sub New()

        DirectCast(Me, ToolStripDropDownMenu).ShowImageMargin = False

    End Sub

    Public Sub LoadItems(items As IEnumerable(Of ToolStripItem))

        If items Is Nothing Then Throw New ArgumentNullException("items")

        Me.Items.LoadItems(items)

        Dim item = items.OfType(Of ToolStripMenuItem)().FirstOrDefault(Function(f) CheckFocus(DirectCast(f.Tag, SwitcherItem)))
        Dim index = If(item IsNot Nothing, Me.Items.IndexOf(item), -1)

        If (Control.ModifierKeys And Keys.Shift) = Keys.Shift Then
            SelectPreviousItem(index)
        Else
            SelectNextItem(index)
        End If

    End Sub

    Private Shared Function CheckFocus(item As SwitcherItem) As Boolean

        If item.Control.ContainsFocus Then
            Return True
        End If

        Dim control = If(item.ParentControl, item.Control.FindParent(Of TabPage))
        If control IsNot Nothing AndAlso control.ContainsFocus Then
            Return True
        End If

        Return False

    End Function

    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")

        If e.KeyCode = Keys.ControlKey Then

            Dim item = Items _
                .OfType(Of ToolStripMenuItem)() _
                .Where(Function(f) f.Enabled AndAlso f.Selected) _
                .Select(Function(f) DirectCast(f.Tag, SwitcherItem)) _
                .FirstOrDefault()

            Hide()

            If item IsNot Nothing AndAlso CheckFocus(item) = False Then
                item.SelectControl()
            End If

        End If

        MyBase.OnKeyUp(e)

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        If (e.Modifiers And Keys.Shift) = Keys.Shift Then
            If e.KeyCode = Keys.Tab Then
                SelectPreviousItem()
            End If
        Else
            If e.KeyCode = Keys.Tab Then
                SelectNextItem()
            End If
        End If
        MyBase.OnKeyDown(e)

    End Sub

    Private Sub SelectPreviousItem()

        SelectPreviousItem(SelectedIndex)

    End Sub

    Private Sub SelectPreviousItem(index As Integer)

        Dim item As ToolStripMenuItem = GetPreviousItemToSelect(index)
        If item IsNot Nothing Then
            item.Select()
        End If

    End Sub

    Private Function GetPreviousItemToSelect(index As Integer) As ToolStripMenuItem

        If index = -1 OrElse index = 0 Then
            Return Items.OfType(Of ToolStripMenuItem).LastOrDefault()
        Else
            For i As Integer = index - 1 To 0 Step -1
                Dim item = TryCast(Items(i), ToolStripMenuItem)
                If item IsNot Nothing Then
                    Return item
                End If
            Next
        End If
        Return Nothing

    End Function

    Private Sub SelectNextItem()

        SelectNextItem(SelectedIndex)

    End Sub

    Private Sub SelectNextItem(index As Integer)

        Dim item As ToolStripMenuItem = GetNextItemToSelect(index)
        If item IsNot Nothing Then
            item.Select()
        End If

    End Sub

    Private Function GetNextItemToSelect(index As Integer) As ToolStripMenuItem

        If index = -1 OrElse index = Items.Count - 1 Then
            Return Items.OfType(Of ToolStripMenuItem).FirstOrDefault()
        Else
            For i As Integer = index + 1 To Items.Count - 1
                Dim item = TryCast(Items(i), ToolStripMenuItem)
                If item IsNot Nothing Then
                    Return item
                End If
            Next
        End If
        Return Nothing

    End Function

    Private ReadOnly Property SelectedIndex As Integer
        Get
            Return Items.OfType(Of ToolStripMenuItem).Where(Function(f) f.Selected).Select(Function(f) Items.IndexOf(f)).DefaultIfEmpty(-1).First()
        End Get
    End Property

End Class
