Imports System.IO

Public Module AppPaths

    Public Sub CheckDirectoryExist()

        If Directory.Exists(AppDataDirectoryPath) = False Then
            Directory.CreateDirectory(AppDataDirectoryPath)
        End If
        If Directory.Exists(LocalAppDataDirectoryPath) = False Then
            Directory.CreateDirectory(LocalAppDataDirectoryPath)
        End If

    End Sub

    Public Function GetFilePath(value As AppFile) As String

        Select Case value
            Case AppFile.ApplicationLog
                Return Path.Combine(AppDataDirectoryPath, "app.log")
            Case AppFile.FormatGroups
                Return Path.Combine(AppDataDirectoryPath, "format_groups.xml")
            Case AppFile.FormatInfos
                Return Path.Combine(AppDataDirectoryPath, "format_infos.xml")
            Case AppFile.UserGuideHtml
                Return Path.Combine(Path.Combine(GetDirectoryPath(AppDirectory.Help), "user_guide.mht"))
        End Select
        Return Nothing

    End Function

    Public Function GetDirectoryPath(value As AppDirectory) As String

        Select Case value
            Case AppDirectory.ExceptionLogs
                Return Path.Combine(AppDataDirectoryPath, "log")
            Case AppDirectory.Help
                Return Path.Combine(BaseDirectory, "Docs")
            Case AppDirectory.Snippets
                Return Path.Combine(BaseDirectory, "Snippets")
        End Select
        Return Nothing

    End Function

    Public ReadOnly Property BaseDirectory As String
        Get
            Return AppDomain.CurrentDomain.BaseDirectory
        End Get
    End Property

#If DEBUG Then
    Public ReadOnly AppDataDirectoryPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.CompanyName, My.Application.Info.ProductName, "Debug")
#Else
    Public ReadOnly AppDataDirectoryPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), My.Application.Info.CompanyName, My.Application.Info.ProductName)
#End If

#If DEBUG Then
    Public ReadOnly LocalAppDataDirectoryPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), My.Application.Info.CompanyName, My.Application.Info.ProductName, "Debug")
#Else
    Public ReadOnly LocalAppDataDirectoryPath As String = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), My.Application.Info.CompanyName, My.Application.Info.ProductName)
#End If

End Module