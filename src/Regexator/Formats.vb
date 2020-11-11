Imports System.IO
Imports System.Collections.ObjectModel
Imports Regexator
Imports Regexator.Xml.Serialization
Imports Regexator.Collections.Generic

Public NotInheritable Class Formats

    Public Sub New()

        Dim groups = DeserializeDefaultGroups()
        Dim infos = DeserializeDefaultInfos(groups)
        _defaultGroups = groups.ToClones().ToReadOnly()
        _defaultInfos = infos.ToClones().ToReadOnly()
        _groups = Array.AsReadOnly(groups)
        _infos = Array.AsReadOnly(infos)
        SetValues(groups)
        SetValues(infos)
        AssignInfos()
        Debug.Assert({_text, _currentLineText, _matchText, _symbol, _info, _beforeAfterResult, _summaryText, _summaryHeading,
            _summaryValue, _descriptionText, _tableText, _tableSelectedText, _snippetListText}.All(Function(f) f IsNot Nothing))

    End Sub

    Private Sub AssignInfos()

        For Each item In _infos
            Select Case item.Name
                Case "PlainText"
                    _text = item
                Case "CurrentLineText"
                    _currentLineText = item
                Case "MatchText"
                    _matchText = item
                Case "CharacterSymbol"
                    _symbol = item
                Case "InfoText"
                    _info = item
                Case "BeforeAfterResult"
                    _beforeAfterResult = item
                Case "SummaryText"
                    _summaryText = item
                Case "SummaryHeadingText"
                    _summaryHeading = item
                Case "SummaryValueText"
                    _summaryValue = item
                Case "DescriptionText"
                    _descriptionText = item
                Case "TableText"
                    _tableText = item
                Case "TableSelectedText"
                    _tableSelectedText = item
                Case "SnippetListText"
                    _snippetListText = item
            End Select
        Next

    End Sub

    Private Shared Sub SetValues(groups As FormatGroup())

        For Each item In groups.Join(
                DeserializeGroups(),
                Function(f) f.Name,
                Function(g) g.Name,
                Function(f, g) New With {.Group = f, .Group2 = g})
            item.Group.LoadValues(item.Group2)
        Next

    End Sub

    Private Shared Sub SetValues(infos As FormatInfo())

        For Each item In infos.Join(
                DeserializeInfos(),
                Function(f) f.Name,
                Function(g) g.Name,
                Function(f, g) New With {.Info = f, .Info2 = g})
            item.Info.LoadEnabledValues(item.Info2)
        Next

    End Sub

    Private Shared Function DeserializeGroups() As IEnumerable(Of FormatGroup)

        Dim filePath = GetFilePath(AppFile.FormatGroups)
        If File.Exists(filePath) Then
            Try
                Return XmlSerializationManager.Deserialize(Of Xml.Serialization.FormatGroup())(filePath).Select(Function(f) Xml.Serialization.FormatGroup.FromSerializable(f))
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.CreateLog())
                Debug.Assert(False, "deserialize format groups")
            End Try
        End If
        Return Enumerable.Empty(Of FormatGroup)()

    End Function

    Private Shared Function DeserializeInfos() As IEnumerable(Of FormatInfo)

        Dim filePath = GetFilePath(AppFile.FormatInfos)
        If File.Exists(filePath) Then
            Try
                Return XmlSerializationManager.Deserialize(Of Xml.Serialization.FormatInfo())(filePath).Select(Function(f) Xml.Serialization.FormatInfo.FromSerializable(f))
            Catch ex As InvalidOperationException
                Debug.WriteLine(ex.CreateLog())
                Debug.Assert(False, "deserialize format infos")
            End Try
        End If
        Return Enumerable.Empty(Of FormatInfo)()

    End Function

    Private Shared Function DeserializeDefaultGroups() As FormatGroup()

        Return XmlSerializationManager.DeserializeText(Of Xml.Serialization.FormatGroup())(My.Resources.XmlFormatGroups) _
            .Select(Function(f) Xml.Serialization.FormatGroup.FromSerializable(f)) _
            .ToArray()

    End Function

    Private Shared Function DeserializeDefaultInfos(groups As FormatGroup()) As FormatInfo()

        Dim infos = XmlSerializationManager.DeserializeText(Of Xml.Serialization.FormatInfo())(My.Resources.XmlFormatInfos)
        Dim lst As New List(Of FormatInfo)(infos.Length)
        For Each grp In groups.GroupJoin(
                infos,
                Function(g) g.Name,
                Function(i) i.Group,
                Function(g, i) New With {.Group = g, .Infos = i})
            For Each item In grp.Infos
                Dim info = Xml.Serialization.FormatInfo.FromSerializable(item)
                info.Group = grp.Group
                lst.Add(info)
            Next
        Next
        Return lst.ToArray()

    End Function

    Public Sub Save()

        XmlSerializationManager.Serialize(Of Xml.Serialization.FormatGroup())(
            GetFilePath(AppFile.FormatGroups),
            Groups.Select(Function(f) Xml.Serialization.FormatGroup.ToSerializable(f)).ToArray())
        XmlSerializationManager.Serialize(Of Xml.Serialization.FormatInfo())(
            GetFilePath(AppFile.FormatInfos),
            Infos.Select(Function(f) Xml.Serialization.FormatInfo.ToSerializable(f)).ToArray())

    End Sub

    Public Sub SetDefaults(groups As IEnumerable(Of FormatGroup))

        If groups Is Nothing Then Throw New ArgumentNullException("groups")
        For Each item In groups
            SetDefaults(item)
        Next

    End Sub

    Public Sub SetDefaults(group As FormatGroup)

        Dim group2 = DefaultGroups.FirstOrDefault(Function(f) f.Name = group.Name)
        If group2 IsNot Nothing Then
            group.LoadValues(group2)
        End If

    End Sub

    Public Sub SetDefaults(infos As IEnumerable(Of FormatInfo))

        If infos Is Nothing Then Throw New ArgumentNullException("infos")
        For Each item In infos
            SetDefaults(item)
        Next

    End Sub

    Public Sub SetDefaults(info As FormatInfo)

        Dim info2 = DefaultInfos.FirstOrDefault(Function(f) f.Name = info.Name)
        If info2 IsNot Nothing Then
            info.LoadValues(info2)
        End If

    End Sub

    Public ReadOnly Property Groups As ReadOnlyCollection(Of FormatGroup)
        Get
            Return _groups
        End Get
    End Property

    Public ReadOnly Property Infos As ReadOnlyCollection(Of FormatInfo)
        Get
            Return _infos
        End Get
    End Property

    Public ReadOnly Property DefaultGroups As ReadOnlyCollection(Of FormatGroup)
        Get
            Return _defaultGroups
        End Get
    End Property

    Public ReadOnly Property DefaultInfos As ReadOnlyCollection(Of FormatInfo)
        Get
            Return _defaultInfos
        End Get
    End Property

    Public ReadOnly Property Text As FormatInfo
        Get
            Return _text
        End Get
    End Property

    Public ReadOnly Property CurrentLineText As FormatInfo
        Get
            Return _currentLineText
        End Get
    End Property

    Public ReadOnly Property Capture As FormatInfo
        Get
            Return _matchText
        End Get
    End Property

    Public ReadOnly Property Symbol As FormatInfo
        Get
            Return _symbol
        End Get
    End Property

    Public ReadOnly Property Info As FormatInfo
        Get
            Return _info
        End Get
    End Property

    Public ReadOnly Property BeforeAfterResult As FormatInfo
        Get
            Return _beforeAfterResult
        End Get
    End Property

    Public ReadOnly Property SummaryText As FormatInfo
        Get
            Return _summaryText
        End Get
    End Property

    Public ReadOnly Property SummaryHeading As FormatInfo
        Get
            Return _summaryHeading
        End Get
    End Property

    Public ReadOnly Property SummaryValue As FormatInfo
        Get
            Return _summaryValue
        End Get
    End Property

    Public ReadOnly Property DescriptionText As FormatInfo
        Get
            Return _descriptionText
        End Get
    End Property

    Public ReadOnly Property TableText As FormatInfo
        Get
            Return _tableText
        End Get
    End Property

    Public ReadOnly Property TableSelectedText As FormatInfo
        Get
            Return _tableSelectedText
        End Get
    End Property

    Public ReadOnly Property DropDownText As FormatInfo
        Get
            Return _snippetListText
        End Get
    End Property

    Private ReadOnly _groups As ReadOnlyCollection(Of FormatGroup)
    Private ReadOnly _infos As ReadOnlyCollection(Of FormatInfo)
    Private ReadOnly _defaultGroups As ReadOnlyCollection(Of FormatGroup)
    Private ReadOnly _defaultInfos As ReadOnlyCollection(Of FormatInfo)
    Private _text As FormatInfo
    Private _currentLineText As FormatInfo
    Private _matchText As FormatInfo
    Private _symbol As FormatInfo
    Private _info As FormatInfo
    Private _beforeAfterResult As FormatInfo
    Private _summaryText As FormatInfo
    Private _summaryHeading As FormatInfo
    Private _summaryValue As FormatInfo
    Private _descriptionText As FormatInfo
    Private _tableText As FormatInfo
    Private _tableSelectedText As FormatInfo
    Private _snippetListText As FormatInfo

End Class