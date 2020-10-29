Imports Regexator.Windows.Forms

Public Class ExportRichTextBox
    Inherits ExtendedRichTextBox

    Public Sub New()

        Dock = DockStyle.Fill
        BorderStyle = BorderStyle.None
        DetectUrls = False
        HideSelection = False
        Me.ReadOnly = True
        Dim cms = New ContextMenuStrip()
        cms.Items.Add(ToolStripItemFactory.CreateCopyItem(Me))
        cms.Items.AddSeparator()
        cms.Items.Add(ToolStripItemFactory.CreateResetZoomItem(Me))
        cms.Items.AddSeparator()
        cms.Items.Add(ToolStripItemFactory.CreatePrintItem(Me))
        ContextMenuStrip = cms

    End Sub

    Public Overrides Sub PrintText()

        Drawing.PrintUtility.Print(Me, My.Resources.Export)

    End Sub

End Class