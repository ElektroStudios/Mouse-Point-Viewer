#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Text

#End Region

Friend NotInheritable Class MouseCoordinatesInfo

    <DisplayName("Caption")>
    Public Property Caption As String

    <DisplayName("Process Name")>
    Public Property ProcessName As String

    <DisplayName("Process ID")>
    Public Property ProcessId As Integer

    <DisplayName("Window Handle")>
    Public Property WindowHandle As String

    <DisplayName("Screen Coordinates")>
    Public Property ScreenCoordinates As String

    <DisplayName("Relative Coordinates")>
    Public Property RelativeCoordinates As String

    <DisplayName("Color Under Cursor")>
    <TypeConverter(GetType(WebColorConverter))>
    Public Property Color As Color

    Public Overrides Function ToString() As String
        Dim sb As New StringBuilder()

        sb.AppendLine($"Caption: {Me.Caption}")
        sb.AppendLine($"Process Name: {Me.ProcessName}")
        sb.AppendLine($"Process ID: {Me.ProcessId}")
        sb.AppendLine($"Window Handle: {Me.WindowHandle}")
        sb.AppendLine($"Screen Coordinates: {Me.ScreenCoordinates}")
        sb.AppendLine($"Relative Coordinates: {Me.RelativeCoordinates}")

        Dim webColorConverter As New WebColorConverter()
        Dim colorString As String = DirectCast(webColorConverter.ConvertTo(Me.Color, GetType(String)), String)
        sb.AppendLine($"Color Under Cursor: {colorString}")

        Return sb.ToString()
    End Function

End Class
