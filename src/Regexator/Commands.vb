Imports System.IO
Imports System.Text
Imports Regexator.FileSystem
Imports Regexator.Windows.Forms

Public NotInheritable Class Commands

    Private Sub New()

    End Sub

    Public Shared Sub SaveOutputText()

        Dim s As String = OutputText.Text
        Using Panels.Output.OutputEnabledRestorer()
            Panels.Input.SetText(s)
            Explorer.SaveInputText()
        End Using

    End Sub

    Public Shared Sub SaveInputAsTxt()

        SaveAsTxt(Panels.Input._rtb, Explorer.CurrentInputFileNameWithoutExtension, Panels.Input.Encoding)

    End Sub

    Public Shared Sub SaveInputAsRtf()

        SaveAsRtf(Panels.Input._rtb, Explorer.CurrentInputFileNameWithoutExtension)

    End Sub

    Public Shared Sub SavePatternAsTxt()

        SaveAsTxt(Panels.Pattern._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SavePatternAsRtf()

        SaveAsRtf(Panels.Pattern._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveReplacementAsTxt()

        SaveAsTxt(Panels.Replacement._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveReplacementAsRtf()

        SaveAsRtf(Panels.Replacement._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveFindResultsAsTxt()

        SaveAsTxt(FindResults._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveFindResultsAsRtf()

        SaveAsRtf(FindResults._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveOutputAsTxt()

        SaveAsTxt(OutputText._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveOutputAsRtf()

        SaveAsRtf(OutputText._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveSummaryAsTxt()

        Summary.LoadSummary()
        SaveAsTxt(Summary._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Public Shared Sub SaveSummaryAsRtf()

        Summary.LoadSummary()
        SaveAsRtf(Summary._rtb, Explorer.CurrentProjectFileNameWithoutExtension)

    End Sub

    Private Shared Sub SaveAsTxt(rtb As RichTextBox, initialFileName As String)

        SaveAsTxt(rtb, initialFileName, DefaultEncoding)

    End Sub

    Private Shared Sub SaveAsTxt(rtb As RichTextBox, initialFileName As String, encoding As Encoding)

        Dim filePath = AppDialogs.GetTextFilePath(initialFileName)
        If filePath IsNot Nothing Then
            Executor.Execute(Sub() File.WriteAllText(filePath, rtb.GetTextCrLf(), encoding))
        End If

    End Sub

    Private Shared Sub SaveAsRtf(rtb As RichTextBox, initialFileName As String)

        Dim filePath = AppDialogs.GetRtfFilePath(initialFileName)
        If filePath IsNot Nothing Then
            Executor.Execute(Sub() rtb.SaveFile(filePath))
        End If

    End Sub

    Private Shared ReadOnly DefaultEncoding As Encoding = Encoding.UTF8

End Class
