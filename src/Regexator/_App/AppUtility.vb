Imports System.IO
Imports System.Net
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Collections.Specialized
Imports System.Threading
Imports System.Globalization
Imports Regexator.FileSystem
Imports System.Diagnostics.CodeAnalysis
Imports Regexator.Windows.Forms

Friend Class AppUtility

    Private Sub New()

    End Sub

    Public Shared Sub AddTabPages(tbc As TabControl, tabPages As TabPage(), tabNames As StringCollection)

        If tabNames IsNot Nothing Then
            Dim tabs = New List(Of TabPage)(tabPages)
            For Each name In tabNames
                Dim tab = tabs.Find(Function(f) f.Name = name)
                If tab IsNot Nothing Then
                    tabs.Remove(tab)
                    tbc.TabPages.Add(tab)
                End If
            Next
            tbc.TabPages.AddRange(tabs.ToArray())
        Else
            tbc.TabPages.AddRange(tabPages)
        End If

    End Sub

    Public Shared Sub LogException(ex As Exception)

        If ex Is Nothing Then Throw New ArgumentNullException("ex")
        Dim fileName As String = Nothing
        Dim dirPath = GetDirectoryPath(AppDirectory.ExceptionLogs)
        If Directory.Exists(dirPath) OrElse Executor.Execute(Sub() Directory.CreateDirectory(dirPath), False) Then
            fileName = Date.Now.ToString("o", CultureInfo.CurrentCulture)
            fileName = Regex.Replace(fileName, "[^\d]", "_")
            fileName = Path.ChangeExtension(fileName, "log")
            fileName = Path.Combine(dirPath, fileName)
            Executor.Execute(Sub() File.WriteAllText(fileName, ex.CreateLog(), Encoding.UTF8), False)
        End If

    End Sub

    <SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")>
    Public Shared Sub CheckVersionAsync()

        If My.Settings.CheckAppVersionQuestionDisplayed = False Then
            My.Settings.CheckAppVersion = MessageDialog.Question(My.Resources.DoYouWantToCheckForUpdatesMsg) = DialogResult.Yes
            My.Settings.CheckAppVersionQuestionDisplayed = True
        End If
        If My.Settings.CheckAppVersion Then
            Dim span = Date.Now - My.Settings.CheckAppVersionLastDate
            If span > My.Settings.CheckAppVersionTimeSpan Then
                Dim client As New WebClient()
                AddHandler client.DownloadStringCompleted,
                    Sub(sender As Object, e As DownloadStringCompletedEventArgs)
                        If e.Error Is Nothing Then
                            My.Settings.CheckAppVersionLastDate = Date.Now
                            Dim version As Version = Nothing
                            If version.TryParse(e.Result, version) Then
                                If version > My.Application.Info.Version Then
                                    If MessageBox.Show(
                                            String.Format(CultureInfo.CurrentCulture, My.Resources.NewVersionAvailableMsg, version.ToString()),
                                            My.Resources.NewVersion,
                                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information) = DialogResult.Yes Then
                                        Process.Start(WebRoot & "/Download")
                                    End If
                                End If
                            End If
                        End If
                    End Sub
                client.DownloadStringAsync(New Uri(WebRoot & "/version.ashx"))
            End If
        End If

    End Sub

    <SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")>
    Public Shared Sub CheckVersion()

        Dim s As String = Nothing

        Using client As New WebClient()
            Try
                s = client.DownloadString(New Uri(WebRoot & "/version.ashx"))
            Catch ex As WebException
            End Try
        End Using

        If s IsNot Nothing Then
            Dim version As Version = Nothing
            If version.TryParse(s, version) Then
                If version > My.Application.Info.Version Then
                    If MessageBox.Show(
                            String.Format(CultureInfo.CurrentCulture, My.Resources.NewVersionAvailableMsg, version.ToString()),
                            My.Resources.NewVersion,
                            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information) = DialogResult.Yes Then
                        Process.Start(WebRoot & "/Download")
                    End If
                Else
                    MessageDialog.Info(My.Resources.YouAreUsingNewestVersion)
                End If
            End If
        Else
            MessageDialog.Info(My.Resources.UnableToCheckForUpdates)
        End If

    End Sub

    Public Shared Sub SetClipboardText(text As String)

        SetClipboardText(text, True)

    End Sub

    Public Shared Sub SetClipboardText(text As String, showMessage As Boolean)

        If String.IsNullOrEmpty(text) = False Then
            Try
                Clipboard.SetText(text)
            Catch ex As Exception
                If TypeOf ex Is System.Runtime.InteropServices.ExternalException OrElse TypeOf ex Is System.Threading.ThreadStateException Then
                    If showMessage Then
                        MessageDialog.Warning(ex.Message)
                    End If
                Else
                    Throw
                End If
            End Try
        End If

    End Sub

    Public Shared Function GetClipboardText() As String

        Dim s As String = Nothing
        Try
            If Clipboard.ContainsText() Then
                s = Clipboard.GetText()
            End If
        Catch ex As Exception
            If TypeOf ex Is System.Runtime.InteropServices.ExternalException OrElse TypeOf ex Is System.Threading.ThreadStateException Then
                MessageDialog.Warning(ex.Message)
            Else
                Throw
            End If
        End Try
        Return s

    End Function

    Public Shared Sub SetCulture()

        Try
            Thread.CurrentThread.CurrentUICulture = New CultureInfo("en")
        Catch ex As CultureNotFoundException
        End Try

    End Sub

    Public Shared Sub SetToolStripProperties(tsp As ExtendedToolStrip)

        tsp.Dock = DockStyle.Top
        tsp.ButtonBackColor = Color.Empty
        tsp.ItemAutoSize = False
        tsp.FirstItemHasLeftMargin = False
        tsp.RenderMode = ToolStripRenderMode.Professional
        tsp.Renderer = New AppToolStripRenderer()

    End Sub

End Class
