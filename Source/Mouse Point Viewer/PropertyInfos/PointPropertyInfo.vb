#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Text

#End Region

<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class PointPropertyInfo

    Private ReadOnly pt As Point

    <DisplayName("X")>
    <[ReadOnly](True)>
    Public ReadOnly Property X As Integer
        Get
            Return pt.X
        End Get
    End Property

    <DisplayName("Y")>
    <[ReadOnly](True)>
    Public ReadOnly Property Y As Integer
        Get
            Return pt.Y
        End Get
    End Property

    Public Sub New(pt As Point)
        Me.pt = pt
    End Sub

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return $"{Me.X}, {Me.Y}"
    End Function

End Class
