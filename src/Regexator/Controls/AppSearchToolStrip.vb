Imports Regexator.UI

Public Class AppSearchToolStrip
    Inherits SearchToolStrip

    Public Sub New(rtb As RichTextBox)

        MyBase.New(rtb)
        AppUtility.SetToolStripProperties(Me)

    End Sub

    Public Sub New(rtb As RichTextBox, replaceEnabled As Boolean)

        MyBase.New(rtb, replaceEnabled)
        AppUtility.SetToolStripProperties(Me)

    End Sub

    Protected Overrides Function ProcessCtrlTab() As Boolean

        Switcher.Instance.Show()
        Return True

    End Function

    Protected Overrides Function ProcessCtrlShiftTab() As Boolean

        Switcher.Instance.Show()
        Return True

    End Function

End Class