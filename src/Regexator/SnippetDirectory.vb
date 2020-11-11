Imports System.IO
Imports System.ComponentModel

Public Class SnippetDirectory

    Public Sub New()

    End Sub

    Public Sub New(info As DirectoryInfo)

        Me.Info = info

    End Sub

    Public Property Path As String
        Get
            Return _path
        End Get
        Set(value As String)
            _path = If(value, String.Empty)
            _info = Nothing
            _info = New DirectoryInfo(value)
        End Set
    End Property

    <Browsable(False)>
    Public Property Info As DirectoryInfo
        Get
            Return _info
        End Get
        Set(value As DirectoryInfo)
            _info = value
            _path = If(value IsNot Nothing, value.FullName, String.Empty)
        End Set
    End Property

    Private _path As String = String.Empty
    Private _info As DirectoryInfo

End Class