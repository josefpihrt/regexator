
#If TATA Then

Public Class FormattedPatternRichTextBox
    Inherits ExtendedRichTextBox

    Public Sub New()

        Me.Dock = DockStyle.Fill
        Me.ReadOnly = True
        Me.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.DetectUrls = False
        Me.ContextMenuStrip = New ContextMenuStrip()
        Me.ContextMenuStrip.Items.AddRange({
            New ToolStripMenuItem(My.Resources.Cut, My.Resources.IcoCut.ToBitmap(), Sub() Me.Cut()) With {.ShortcutKeyDisplayString = My.Resources.CtrlX},
            New ToolStripMenuItem(My.Resources.Copy, My.Resources.IcoCopy.ToBitmap(), Sub() Me.Copy()) With {.ShortcutKeyDisplayString = My.Resources.CtrlC},
            New ToolStripSeparator(),
            New ToolStripMenuItem(My.Resources.Print & My.Resources.EllipsisStr, My.Resources.IcoPrint.ToBitmap(), Sub() Me.PrintText()) With {.ShortcutKeyDisplayString = My.Resources.CtrlP}})

    End Sub

End Class

#End If
