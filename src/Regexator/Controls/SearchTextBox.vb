Imports Regexator.Windows.Forms

Public Class SearchTextBox
    Inherits ExtendedTextBox

    Public Sub New()

        BorderStyle = BorderStyle.None
        Dock = DockStyle.Top
        SetDefaultText()

    End Sub

    Private Sub SetDefaultText()

        Text = DefaultText
        ForeColor = DefaultTextForeColor

    End Sub

    Private Sub RemoveDefaultText()

        Clear()
        ForeColor = DefaultForeColor

    End Sub

    Protected Overrides Sub OnEnter(e As EventArgs)

        If String.Equals(Text, DefaultText, StringComparison.CurrentCulture) Then
            RemoveDefaultText()
        End If
        MyBase.OnEnter(e)

    End Sub

    Protected Overrides Sub OnLeave(e As EventArgs)

        If IsValidSearchPhrase = False Then
            SetDefaultText()
        End If
        MyBase.OnLeave(e)

    End Sub

    Public ReadOnly Property DefaultText As String
        Get
            Return _defaultText
        End Get
    End Property

    Public ReadOnly Property DefaultTextForeColor As Color
        Get
            Return _defaultTextForeColor
        End Get
    End Property

    Public ReadOnly Property IsValidSearchPhrase As Boolean
        Get
            Return Not String.IsNullOrWhiteSpace(Text)
        End Get
    End Property

    Private ReadOnly _defaultText As String = My.Resources.SearchProjectExplorerCtrlF
    Private ReadOnly _defaultTextForeColor As Color = Color.DimGray

End Class
