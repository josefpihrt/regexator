Imports Microsoft.VisualBasic.ApplicationServices

Public Class RegexatorApplication
    Inherits WindowsFormsApplicationBase

    Public Sub New(mainForm As MainForm)

#If DEBUG Then
        IsSingleInstance = False
#Else
        Me.IsSingleInstance = True
#End If
        EnableVisualStyles = True
        Me.MainForm = mainForm
        SaveMySettingsOnExit = False

    End Sub

End Class
