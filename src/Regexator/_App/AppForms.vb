Imports System.IO
Imports System.Globalization
Imports Regexator.UI

Public NotInheritable Class AppForms

    Private Sub New()

    End Sub

    Public Shared Sub ShowGuideForm()

        ShowModelessForm(Of GuideForm)()

    End Sub

    Public Shared Sub ShowUserGuideForm()

        Dim filePath As String = GetFilePath(AppFile.UserGuideHtml)
        If File.Exists(filePath) Then
            ShowModelessForm(Of UserGuideForm)()
        Else
            MessageDialog.Warning(String.Format(CultureInfo.CurrentCulture, My.Resources.FileWasNotFound, filePath))
        End If

    End Sub

    Public Shared Sub ShowCharacterAnalyzerForm()

        ShowModelessForm(Of CharacterAnalyzerForm)(App.MainForm)

    End Sub

    Private Shared Sub ShowModelessForm(Of T As {New, Form})(Optional owner As IWin32Window = Nothing)

        Dim frm As Form = Application.OpenForms.Cast(Of Form)().FirstOrDefault(Function(f) TypeOf f Is T)
        If frm IsNot Nothing Then
            If frm.WindowState = FormWindowState.Minimized Then
                ShowWindow(frm.Handle, ShowWindowCommands.Restore)
            Else
                frm.BringToFront()
            End If
        Else
            frm = New T()
            If owner IsNot Nothing Then
                frm.Show(owner)
            Else
                frm.Show()
            End If
        End If

    End Sub

    Public Shared Sub ShowAboutForm()

        Using frm As New AboutForm()
            frm.ShowDialog()
        End Using

    End Sub

    Public Shared Sub ShowOptionsForm()

        ShowOptionsForm(OptionsFormTab.Application)

    End Sub

    Public Shared Sub ShowOptionsForm(initialTab As OptionsFormTab)

        Using frm As New OptionsForm(initialTab)
            My.Settings.FileSystemUseRecycleBin = Explorer.UseRecycleBin
            My.Settings.ConfirmFileInputRemoval = Explorer.ConfirmFileInputRemoval
            My.Settings.FileSystemShowHidden = Explorer.ShowHidden
            My.Settings.RegexOptionsHotkeyNumberVisible = Options.HotkeyNumberColumnVisible
            My.Settings.RegexOptionsDescriptionVisible = Options.DescriptionColumnVisible
            If frm.ShowDialog() = DialogResult.OK Then
                App.MainForm._stsMain.Visible = My.Settings.MainFormStatusBarVisible
                App.Formats.Save()
                Explorer.UseRecycleBin = My.Settings.FileSystemUseRecycleBin
                Explorer.ConfirmFileInputRemoval = My.Settings.ConfirmFileInputRemoval
                Explorer.ShowHidden = My.Settings.FileSystemShowHidden
                Options.HotkeyNumberColumnVisible = My.Settings.RegexOptionsHotkeyNumberVisible
                Options.DescriptionColumnVisible = My.Settings.RegexOptionsDescriptionVisible
                Options.LoadItems()
                Export.LoadSettings()
                Panels.Output.LoadData()
            End If
        End Using

    End Sub

End Class