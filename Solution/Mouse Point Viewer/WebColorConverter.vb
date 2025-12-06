#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Globalization

#End Region

Friend NotInheritable Class WebColorConverter : Inherits ColorConverter

    Friend Shared ReadOnly KnownColorsDict As Dictionary(Of Integer, String)

    Public Overrides Function ConvertTo(context As ITypeDescriptorContext,
                                        culture As CultureInfo,
                                        value As Object,
                                        destinationType As Type) As Object

        If destinationType Is GetType(String) AndAlso TypeOf value Is Color Then
            Dim c As Color = DirectCast(value, Color)

            Dim html As String = ColorTranslator.ToHtml(c)

            Dim name As String = Nothing
            For Each kc As KnownColor In [Enum].GetValues(GetType(KnownColor))
                Dim kColor As Color = Color.FromKnownColor(kc)
                If Not kColor.IsSystemColor AndAlso
                    kColor.R = c.R AndAlso
                    kColor.G = c.G AndAlso
                    kColor.B = c.B AndAlso
                    kColor.A = 255 Then ' 255 for "White" instead of "Transparent" (0)

                    name = kColor.Name
                    Exit For
                End If
            Next

            Return If(name IsNot Nothing, $"{html} ({name})", html)
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function


    Public Overrides Function ConvertFrom(context As ITypeDescriptorContext,
                                          culture As Globalization.CultureInfo,
                                          value As Object) As Object

        Dim s As String = TryCast(value, String)
        If s IsNot Nothing Then
            Dim idx As Integer = s.IndexOf("("c)
            If idx >= 0 Then
                s = s.Substring(0, idx).Trim()
            End If

            Return ColorTranslator.FromHtml(s)
        End If

        Return MyBase.ConvertFrom(context, culture, value)
    End Function

End Class
