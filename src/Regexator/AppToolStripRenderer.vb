Public Class AppToolStripRenderer
    Inherits ToolStripProfessionalRenderer

    Protected Overrides Sub OnRenderButtonBackground(e As ToolStripItemRenderEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim item = TryCast(e.Item, ToolStripButton)
        If item IsNot Nothing AndAlso item.Selected = False AndAlso item.Checked Then
            Using brush As New SolidBrush(ColorTable.ButtonSelectedHighlight)
                e.Graphics.FillRectangle(brush, 0, 0, item.Size.Width, item.Size.Height)
            End Using
        End If
        MyBase.OnRenderButtonBackground(e)

    End Sub

End Class