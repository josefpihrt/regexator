Public Class ExplorerStatusStripRenderer
    Inherits ToolStripProfessionalRenderer

    Protected Overrides Sub OnRenderItemText(e As ToolStripItemTextRenderEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        Dim label = TryCast(e.Item, ToolStripStatusLabel)
        If label IsNot Nothing Then
            TextRenderer.DrawText(e.Graphics, e.Text, label.Font, e.TextRectangle, label.ForeColor, TextFormatFlags.PathEllipsis)
        Else
            MyBase.OnRenderItemText(e)
        End If

    End Sub

End Class
