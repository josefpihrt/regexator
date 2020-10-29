
Public Class FileSystemSearchResultsContextMenuStrip
    Inherits ContextMenuStrip

    Public Property OpenItem As ToolStripMenuItem
    Public Property OpenContainingFolderItem As ToolStripMenuItem
    Public Property CopyItem As ToolStripMenuItem
    Public Property DeleteItem As ToolStripMenuItem
    Public Property RenameItem As ToolStripMenuItem

    Public Sub New()

        OpenItem = New ToolStripMenuItem(My.Resources.Open, My.Resources.IcoOpen.ToBitmap()) With {.ShortcutKeyDisplayString = My.Resources.F4}
        OpenContainingFolderItem = New ToolStripMenuItem(My.Resources.OpenContainingFolder)
        CopyItem = New ToolStripMenuItem(My.Resources.Copy, My.Resources.IcoCopy.ToBitmap()) With {.ShortcutKeyDisplayString = My.Resources.CtrlC}
        DeleteItem = New ToolStripMenuItem(My.Resources.Delete, My.Resources.IcoDelete.ToBitmap()) With {.ShortcutKeyDisplayString = My.Resources.Del}
        RenameItem = New ToolStripMenuItem(My.Resources.Rename) With {.ShortcutKeyDisplayString = My.Resources.F2}

        Items.Add(OpenItem)
        Items.Add(OpenContainingFolderItem)
        Items.Add(New ToolStripSeparator())
        Items.Add(CopyItem)
        Items.Add(DeleteItem)
        Items.Add(RenameItem)

    End Sub

End Class
