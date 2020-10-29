Public Class FileSystemSearchBox

    Public Sub New()

        InitializeComponent()
        Dock = DockStyle.Top
        BorderStyle = BorderStyle.None
        Padding = New Padding(2, 1, 2, 1)
        ResizeRedraw = True
        _tbx = New SearchTextBox()
        Controls.Add(_tbx)

    End Sub

    Public Sub Clear()

        _tbx.Clear()

    End Sub

    Public Sub SelectAll()

        _tbx.SelectAll()

    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        If e Is Nothing Then Throw New ArgumentNullException("e")
        MyBase.OnPaint(e)
        Dim c = Color.DarkGray
        Dim s = ButtonBorderStyle.Solid
        Dim w = 1
        ControlPaint.DrawBorder(e.Graphics, ClientRectangle,
            c, w, s,
            c, w, s,
            c, w, s,
            Color.Empty, 0, ButtonBorderStyle.None)

    End Sub

    Public Overrides Property Text As String
        Get
            Return _tbx.Text
        End Get
        Set(value As String)
            _tbx.Text = value
        End Set
    End Property

    Public ReadOnly Property TextBox As TextBoxBase
        Get
            Return _tbx
        End Get
    End Property

End Class
