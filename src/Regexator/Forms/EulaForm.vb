Public Class EulaForm

    Public Sub New()

        InitializeComponent()
        StartPosition = FormStartPosition.CenterScreen
        MaximizeBox = False
        MinimizeBox = False
        FormBorderStyle = FormBorderStyle.FixedSingle
        ShowInTaskbar = False
        Icon = My.Resources.IcoRegexator
        Text = My.Application.Info.Title
        _btnOk.Text = My.Resources.IAgree
        _btnCancel.Text = My.Resources.Cancel
        CancelButton = _btnCancel
        AddHandler _btnOk.Click, Sub(sender As Object, e As EventArgs)
                                     DialogResult = DialogResult.OK
                                     Close()
                                 End Sub

    End Sub

    Protected Overrides Sub OnLoad(e As EventArgs)

        Dim color = _rtb.BackColor
        _rtb.ReadOnly = True
        _rtb.BackColor = color
        _rtb.Rtf = My.Resources.RtfEula
        MyBase.OnLoad(e)

    End Sub

End Class