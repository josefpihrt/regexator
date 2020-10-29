Partial Public Class AboutForm

    Public Sub New()

        InitializeComponent()
        StartPosition = FormStartPosition.CenterParent
        MaximizeBox = False
        MinimizeBox = False
        FormBorderStyle = FormBorderStyle.FixedSingle
        ShowInTaskbar = False
        Icon = My.Resources.IcoRegexator
        Text = My.Resources.About & " " & My.Application.Info.Title
        CancelButton = _btnOk
        _btnOk.Text = My.Resources.OK
        _lblName.Text = My.Application.Info.Title
        _lblVersion.Text = My.Resources.Version & " " & My.Application.Info.Version.ToString(3)
        _lblCopyright.Text = My.Application.Info.Copyright
        _lblRights.Text = My.Resources.AllRightsReserved
        _llbWeb.Text = WebRoot
        AddHandler _btnOk.Click, Sub() Close()
        AddHandler _llbWeb.LinkClicked, Sub() Process.Start(_llbWeb.Text)

    End Sub

End Class