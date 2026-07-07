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
Friend NotInheritable Class RectanglePropertyInfo

    Private ReadOnly rect As Rectangle

    <[ReadOnly](True)>
    Public ReadOnly Property X As Integer
        Get
            Return Me.rect.X
        End Get
    End Property

    <[ReadOnly](True)>
    Public ReadOnly Property Y As Integer
        Get
            Return Me.rect.Y
        End Get
    End Property

    <[ReadOnly](True)>
    Public ReadOnly Property Width As Integer
        Get
            Return Me.rect.Width
        End Get
    End Property

    <[ReadOnly](True)>
    Public ReadOnly Property Height As Integer
        Get
            Return Me.rect.Height
        End Get
    End Property

    <Browsable(False)>
    <[ReadOnly](True)>
    Public ReadOnly Property Location As Point
        Get
            Return Me.rect.Location
        End Get
    End Property

    <Browsable(False)>
    <[ReadOnly](True)>
    Public ReadOnly Property Size As Size
        Get
            Return Me.rect.Size
        End Get
    End Property

    Public Sub New(rect As Rectangle)
        Me.rect = rect
    End Sub

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return $"{Me.X}, {Me.Y}; {Me.Width}, {Me.Height}"
    End Function

End Class
