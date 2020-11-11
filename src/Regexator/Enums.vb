Public Enum AppFile

    ApplicationLog
    FormatGroups
    FormatInfos
    UserGuideHtml

End Enum

Public Enum AppDirectory

    ExceptionLogs
    Help
    Snippets

End Enum

<Flags>
Public Enum SnippetOptions

    None = 0
    HideCategory = 1
    Sort = 2

End Enum

Public Enum OptionsFormTab

    Application
    DefaultValues
    Export
    ProjectExplorer
    Snippets
    RegexOptions

End Enum

Public Enum TableLayout

    Group
    Value

End Enum

Public Enum HighlightSource

    None
    Text
    Table

End Enum

Public Enum TextKind

    SelectionOrCurrent

End Enum

<Flags>
Friend Enum KeyStates

    None = 0
    Down = 1
    Toggled = 2

End Enum