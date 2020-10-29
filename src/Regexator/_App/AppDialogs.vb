Public NotInheritable Class AppDialogs

    Private Sub New()

    End Sub

    Public Shared Function GetTextFilePath(fileName As String) As String

        Using dlg As New SaveFileDialog()
            dlg.FileName = fileName
            dlg.DefaultExt = "txt"
            dlg.Filter = "Text Files (*.txt)|*.txt"
            dlg.OverwritePrompt = True
            If dlg.ShowDialog() = DialogResult.OK Then
                Return dlg.FileName
            End If
        End Using
        Return Nothing

    End Function

    Public Shared Function GetRtfFilePath(fileName As String) As String

        Using dlg As New SaveFileDialog()
            dlg.FileName = fileName
            dlg.DefaultExt = "rtf"
            dlg.Filter = "Rich Text Format Files (*.rtf)|*.rtf"
            dlg.OverwritePrompt = True
            If dlg.ShowDialog() = DialogResult.OK Then
                Return dlg.FileName
            End If
        End Using
        Return Nothing

    End Function

End Class
