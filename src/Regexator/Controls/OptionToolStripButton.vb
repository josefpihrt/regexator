Imports System.ComponentModel

Public Class OptionToolStripButton
    Inherits ToolStripButton

    Friend Sub New(icon As Icon, item As RegexOptionsItem)

        MyBase.New(Nothing, icon.ToBitmap())
        If item Is Nothing Then Throw New ArgumentNullException("item")
        ToolTipText = item.Text
        CheckOnClick = True
        Checked = item.Enabled
        Visible = item.Visible OrElse item.Enabled
        AddHandler Click,
            Sub()
                If Checked Then
                    App.RegexOptionsManager.AddOptions(item.Value)
                Else
                    App.RegexOptionsManager.RemoveOptions(item.Value)
                End If
            End Sub
        AddHandler item.PropertyChanged, Sub(sender As Object, e As PropertyChangedEventArgs)
                                             If e.PropertyName = "Visible" OrElse e.PropertyName = "Enabled" Then
                                                 Checked = item.Enabled
                                                 Visible = item.Visible OrElse item.Enabled
                                             End If
                                         End Sub

    End Sub

End Class
