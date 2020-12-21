Imports System.ComponentModel
Imports System.Configuration
Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Regexator.Text.RegularExpressions
Imports Regexator.Windows.Forms

Public Module App

    <STAThread>
    Friend Sub Main(args As String())

#If CONFIG = "Release" Then
        System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException)
        AddHandler System.Windows.Forms.Application.ThreadException, Sub(sender As Object, e As System.Threading.ThreadExceptionEventArgs) ProcessException(e.Exception)
        AddHandler AppDomain.CurrentDomain.UnhandledException, Sub(sender As Object, e As System.UnhandledExceptionEventArgs) ProcessException(TryCast(e.ExceptionObject, Exception))
#End If
        Try
            AppDomain.CurrentDomain.SetData("REGEX_DEFAULT_MATCH_TIMEOUT", My.Settings.MatchTimeout.Duration())
        Catch ex As ConfigurationException
        End Try
        AppUtility.SetCulture()
        AppSettings.Initialize()
        CheckDirectoryExist()
        Trace.IndentSize = 2
        Debug.IndentSize = 2
        MessageDialog.MasterCaption = My.Application.Info.Title
        InitialFilePath = GetFilePathFromCommandLineArgs(args)
        RegexOptionsManager = New RegexOptionsManager()
        Formats = New Formats()
        _frm = New MainForm()
        Dim application As New RegexatorApplication(_frm)
        AddHandler application.StartupNextInstance, AddressOf Application_StartupNextInstance
        AddHandler Xml.Serialization.Projects.ProjectSerializer.Default.Serializing, Sub(sender As Object, e As Xml.Serialization.Projects.ProjectSerializationCancelEventArgs) e.ApplicationVersion = My.Application.Info.Version
        AddHandler My.Settings.PropertyChanged, AddressOf Settings_PropertyChanged
        DebugMethod()
        application.Run(args)

    End Sub

    <Conditional("DEBUG")>
    Private Sub DebugMethod()

    End Sub

    Private Sub Settings_PropertyChanged(sender As Object, e As PropertyChangedEventArgs)

        Select Case e.PropertyName
            Case "CodeExportMode"
                Export.Mode = My.Settings.CodeExportMode
        End Select

    End Sub

    Private Sub Application_StartupNextInstance(sender As Object, e As StartupNextInstanceEventArgs)

        e.BringToForeground = True
        If e.CommandLine IsNot Nothing Then
            Dim filePath As String = GetFilePathFromCommandLineArgs(e.CommandLine.ToArray())
            If filePath IsNot Nothing AndAlso File.Exists(filePath) Then
                If MainForm.WindowState = FormWindowState.Minimized Then
                    ShowWindow(MainForm.Handle, ShowWindowCommands.Restore)
                Else
                    MainForm.BringToFront()
                End If
                Explorer.LoadProject(filePath)
            End If
        End If

    End Sub

    Private Function GetFilePathFromCommandLineArgs(args As String()) As String

        If args IsNot Nothing Then
            Dim index = Array.IndexOf(args, "-p")
            If index <> -1 AndAlso (index + 1) < args.Length Then
                Return args(index + 1)
            End If
        End If
        Return Nothing

    End Function

#If CONFIG = "Release" Then
    Private Sub ProcessException(ex As Exception)

        CriticalException = True
        Try
            If ex IsNot Nothing Then
                AppUtility.LogException(ex)
            End If
            MessageDialog.Err(My.Resources.ErrorOccurredMsg)
        Finally
            Application.Exit()
        End Try

    End Sub
#End If

    Private Sub ChangeEvaluationMode(back As Boolean)

        Select Case Mode
            Case EvaluationMode.Match
                Mode = If(back, EvaluationMode.Split, EvaluationMode.Replace)
            Case EvaluationMode.Replace
                Mode = If(back, EvaluationMode.Match, EvaluationMode.Split)
            Case EvaluationMode.Split
                Mode = If(back, EvaluationMode.Replace, EvaluationMode.Match)
        End Select

    End Sub

    Public Sub SelectNextMode()

        ChangeEvaluationMode(False)

    End Sub

    Public Sub SelectPreviousMode()

        ChangeEvaluationMode(True)

    End Sub

    Public ReadOnly Property Data As DataManager
        Get
            Return DataManager.Instance
        End Get
    End Property

    Public ReadOnly Property ProjectContainer As ProjectContainer
        Get
            Dim value As New ProjectContainer() With {
                .Mode = Mode,
                .Attributes = Panels.Pattern.Attributes,
                .ProjectInfo = Info.ProjectInfo,
                .Pattern = Pattern,
                .Replacement = Panels.Replacement.Replacement,
                .OutputInfo = New OutputInfo(Panels.Output.Options, New GroupSettings(GroupSettings.DefaultSortProperty, GroupSettings.DefaultSortDirection, Groups.GetIgnoredGroups())),
                .FileSystemSearchInfo = FileSystemSearchResults.FileSystemSearchInfo}
            Return value
        End Get
    End Property

    Private ReadOnly Property Pattern As Pattern
        Get
            Return New Pattern() With {
                .Text = Panels.Pattern._rtb.GetTextCrLf(),
                .RegexOptions = RegexOptionsManager.Value,
                .PatternOptions = Panels.Pattern.PatternOptions,
                .CurrentLine = If(Panels.Pattern.CurrentLineOnly, Panels.Pattern.CurrentLine, 0)}
        End Get
    End Property

    Public ReadOnly Property DefaultProjectContainer As ProjectContainer
        Get
            Dim value As New ProjectContainer()
            value.Pattern.RegexOptions = My.Settings.DefaultRegexOptions
            value.Pattern.PatternOptions = My.Settings.DefaultPatternOptions
            value.OutputInfo.Options = My.Settings.DefaultOutputOptions
            value.Replacement.Options = My.Settings.DefaultReplacementOptions
            value.Replacement.NewLine = My.Settings.DefaultReplacementNewLine
            value.Replacement.Text = My.Settings.DefaultReplacementText
            Return value
        End Get
    End Property

    Public ReadOnly Property DefaultInput As Input
        Get
            Return New Input() With {
                .Options = My.Settings.DefaultInputOptions,
                .NewLine = My.Settings.DefaultInputNewLine,
                .Encoding = AppSettings.InputDefaultEncoding}
        End Get
    End Property

    Public Property Mode As EvaluationMode
        Get
            Return _mode
        End Get
        Set(value As EvaluationMode)
            If _mode <> value Then
                _mode = value
                Dim isMatch = (Mode = EvaluationMode.Match)
                _frm._tslMode.Text = EnumHelper.GetDescription(Mode)
                With _frm._tspMain
                    ._btnMatch.Checked = isMatch
                    ._btnReplace.Checked = (Mode = EvaluationMode.Replace)
                    ._btnSplit.Checked = (Mode = EvaluationMode.Split)
                End With
                With Groups
                    ._btnSetAll.Enabled = isMatch
                    ._btnUndo.Enabled = isMatch
                    ._btnRedo.Enabled = isMatch
                    ._dgv.Mode = Mode
                End With
                With Panels.Output._tsp
                    ._btnGroupLayout.Enabled = (Mode = EvaluationMode.Match)
                    ._btnValueLayout.Enabled = (Mode = EvaluationMode.Match)
                End With
                Panels.Output.LoadData()
            End If
        End Set
    End Property

    Public ReadOnly Property MainForm As MainForm
        Get
            Return _frm
        End Get
    End Property

    Public ReadOnly WebRoot As String = "http://pihrt.net/regexator"

    Private _frm As MainForm
    Private _mode As EvaluationMode
    Friend Property Formats As Formats
    Friend Property RegexOptionsManager As RegexOptionsManager
    Friend Property InitialFilePath As String

#If CONFIG = "Release" Then
    Friend Property CriticalException As Boolean
#End If

#If TATA Then
    Friend FormattedPanel As FormattedPanel
#End If

End Module