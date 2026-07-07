#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

#End Region

<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class WindowBoundsPropertyContainer

    <DisplayName("Rectangle")>
    <[ReadOnly](True)>
    Public Property RectangleInfo As RectanglePropertyInfo

    <DisplayName("Client Rectangle")>
    <[ReadOnly](True)>
    Public Property ClientRectangleInfo As RectanglePropertyInfo

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return $"{Me.RectangleInfo.X}, {Me.RectangleInfo.Y}; {Me.RectangleInfo.Width}, {Me.RectangleInfo.Height} | {Me.ClientRectangleInfo.X}, {Me.ClientRectangleInfo.Y}; {Me.ClientRectangleInfo.Width}, {Me.ClientRectangleInfo.Height}"
    End Function

End Class
