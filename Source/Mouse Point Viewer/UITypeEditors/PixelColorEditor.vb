#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing.Design

#End Region

Public Class PixelColorEditor : Inherits UITypeEditor

    Public Overrides Function GetEditStyle(context As ITypeDescriptorContext) As UITypeEditorEditStyle

        Return UITypeEditorEditStyle.None
    End Function

    Public Overrides Function GetPaintValueSupported(context As ITypeDescriptorContext) As Boolean

        Return True
    End Function

    Public Overrides Sub PaintValue(e As PaintValueEventArgs)

        Dim info As PixelColorPropertyInfo = TryCast(e.Value, PixelColorPropertyInfo)
        If info IsNot Nothing Then
            Dim c As Color = info.Color
            Using b As New SolidBrush(c)
                e.Graphics.FillRectangle(b, e.Bounds)
            End Using
        End If
    End Sub

End Class
