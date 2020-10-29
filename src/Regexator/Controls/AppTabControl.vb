Imports Regexator.Windows.Forms

Public Class AppTabControl
    Inherits ExtendedTabControl

    Public Sub New()

        AllowDrop = True
        Dock = DockStyle.Fill
        Appearance = TabAppearance.FlatButtons

    End Sub

End Class
