﻿Imports Regexator.Windows.Forms

Public Class AppToolStrip
    Inherits ExtendedToolStrip

    Public Sub New()

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
