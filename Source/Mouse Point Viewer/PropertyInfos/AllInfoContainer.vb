#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Text

#End Region

Friend NotInheritable Class AllInfoContainer

    <DisplayName("Process")>
    <[ReadOnly](True)>
    Public Property Process As ProcessPropertyInfo

    <DisplayName("Window Caption")>
    <[ReadOnly](True)>
    Public Property Caption As CaptionPropertyInfo

    <DisplayName("Window Class")>
    <[ReadOnly](True)>
    Public Property ClassName As ClassNamePropertyInfo

    <DisplayName("Window Handle")>
    <[ReadOnly](True)>
    Public Property WindowHandle As WindowHandlePropertyInfo

    <DisplayName("Window Bounds")>
    <[ReadOnly](True)>
    Public Property WindowBounds As WindowBoundsPropertyContainer

    <DisplayName("Mouse Point")>
    <[ReadOnly](True)>
    Public Property Point As PointPropertyContainer

    <DisplayName("Pixel Color")>
    <[ReadOnly](True)>
    <TypeConverter(GetType(ExpandableObjectConverter))>
    Public Property PixelColor As PixelColorPropertyInfo

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Dim sb As New StringBuilder()

        sb.AppendLine("Process")
        sb.AppendLine($"{vbTab}Name: {Me.Process.Name}.exe")
        sb.AppendLine($"{vbTab}ID: {Me.Process.Id}")
        sb.AppendLine($"{vbTab}Architecture: {Me.Process.Architecture}")
        sb.AppendLine()
        sb.AppendLine("Window Caption")
        sb.AppendLine($"{vbTab}Text: {Me.Caption.Text}")
        sb.AppendLine($"{vbTab}Length: {Me.Caption.Length}")
        sb.AppendLine()
        sb.AppendLine("Window Class")
        sb.AppendLine($"{vbTab}Name: {Me.ClassName.Name}")
        sb.AppendLine($"{vbTab}Real Name: {Me.ClassName.RealName}")
        sb.AppendLine()
        sb.AppendLine("Window Handle")
        sb.AppendLine($"{vbTab}Decimal: {Me.WindowHandle.Decimal}")
        sb.AppendLine($"{vbTab}Hexadecimal: {Me.WindowHandle.Hexadecimal}")
        sb.AppendLine()
        sb.AppendLine("Window Bounds")
        sb.AppendLine($"{vbTab}Rectangle")
        sb.AppendLine($"{vbTab}{vbTab}X: {Me.WindowBounds.RectangleInfo.X}")
        sb.AppendLine($"{vbTab}{vbTab}Y: {Me.WindowBounds.RectangleInfo.Y}")
        sb.AppendLine($"{vbTab}{vbTab}Width: {Me.WindowBounds.RectangleInfo.Width}")
        sb.AppendLine($"{vbTab}{vbTab}Height: {Me.WindowBounds.RectangleInfo.Height}")
        sb.AppendLine($"{vbTab}Client Rectangle")
        sb.AppendLine($"{vbTab}{vbTab}X: {Me.WindowBounds.ClientRectangleInfo.X}")
        sb.AppendLine($"{vbTab}{vbTab}Y: {Me.WindowBounds.ClientRectangleInfo.Y}")
        sb.AppendLine($"{vbTab}{vbTab}Width: {Me.WindowBounds.ClientRectangleInfo.Width}")
        sb.AppendLine($"{vbTab}{vbTab}Height: {Me.WindowBounds.ClientRectangleInfo.Height}")
        sb.AppendLine()
        sb.AppendLine("Mouse Point")
        sb.AppendLine($"{vbTab}Screen")
        sb.AppendLine($"{vbTab}{vbTab}X: {Me.Point.ScreenPointInfo.X}")
        sb.AppendLine($"{vbTab}{vbTab}Y: {Me.Point.ScreenPointInfo.Y}")
        sb.AppendLine($"{vbTab}Client Window")
        sb.AppendLine($"{vbTab}{vbTab}X: {Me.Point.ClientPointInfo.X}")
        sb.AppendLine($"{vbTab}{vbTab}Y: {Me.Point.ClientPointInfo.Y}")
        sb.AppendLine()
        sb.AppendLine("Pixel Color")
        sb.AppendLine($"{vbTab}RGB: {Me.PixelColor.RGB}")
        sb.AppendLine($"{vbTab}Win32: {Me.PixelColor.Win32}")
        sb.AppendLine($"{vbTab}Web: {Me.PixelColor.Web}")
        sb.AppendLine($"{vbTab}Web Name: {Me.PixelColor.WebName}")

        Return sb.ToString()
    End Function

End Class
