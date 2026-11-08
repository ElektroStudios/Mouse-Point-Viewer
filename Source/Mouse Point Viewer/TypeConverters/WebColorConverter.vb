#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Globalization

#End Region

''' <summary>
''' A <see cref="ColorConverter"/> that converts a <see cref="Color"/> to its HTML representation,
''' followed by its known web color name in parentheses, when it has one (e.g. "#FF0000 (Red)").
''' </summary>
Friend NotInheritable Class WebColorConverter : Inherits ColorConverter

#Region " Public Methods "

    ''' <summary>
    ''' Returns the name of the first non-system <see cref="KnownColor"/> whose RGB components
    ''' match the RGB components of the specified color (the alpha component of <paramref name="c"/> is ignored).
    ''' </summary>
    '''
    ''' <param name="c">
    ''' The source color.
    ''' </param>
    '''
    ''' <returns>
    ''' The known color name (e.g. "Red"), or <see langword="Nothing"/> if there is no matching known color.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function GetKnownColorName(c As Color) As String

        For Each kc As KnownColor In [Enum].GetValues(GetType(KnownColor))
            Dim kColor As Color = Color.FromKnownColor(kc)
            If Not kColor.IsSystemColor AndAlso
                kColor.R = c.R AndAlso
                kColor.G = c.G AndAlso
                kColor.B = c.B AndAlso
                kColor.A = 255 Then ' 255 for "White" instead of "Transparent" (0)

                Return kColor.Name
            End If
        Next

        Return Nothing
    End Function

    ''' <summary>
    ''' Converts the given value object to the specified type.
    ''' <para></para>
    ''' A <see cref="Color"/> converted to <see cref="String"/> results in its HTML representation,
    ''' followed by its known web color name in parentheses, when it has one (e.g. "#FF0000 (Red)").
    ''' </summary>
    '''
    ''' <param name="context">
    ''' An <see cref="ITypeDescriptorContext"/> that provides a format context.
    ''' </param>
    '''
    ''' <param name="culture">
    ''' The culture info.
    ''' </param>
    '''
    ''' <param name="value">
    ''' The value to convert.
    ''' </param>
    '''
    ''' <param name="destinationType">
    ''' The type to convert the value to.
    ''' </param>
    '''
    ''' <returns>
    ''' The converted value.
    ''' </returns>
    <DebuggerStepThrough>
    Public Overrides Function ConvertTo(context As ITypeDescriptorContext,
                                        culture As CultureInfo,
                                        value As Object,
                                        destinationType As Type) As Object

        If destinationType Is GetType(String) AndAlso TypeOf value Is Color Then
            Dim c As Color = DirectCast(value, Color)

            Dim html As String = ColorTranslator.ToHtml(c)
            Dim name As String = WebColorConverter.GetKnownColorName(c)

            Return If(name IsNot Nothing, $"{html} ({name})", html)
        End If

        Return MyBase.ConvertTo(context, culture, value, destinationType)
    End Function

    ''' <summary>
    ''' Converts the given object to a <see cref="Color"/>.
    ''' <para></para>
    ''' Accepts the format produced by <see cref="WebColorConverter.ConvertTo"/> (e.g. "#FF0000 (Red)"):
    ''' the text in parentheses is discarded and the remaining text is parsed as an HTML color.
    ''' </summary>
    '''
    ''' <param name="context">
    ''' An <see cref="ITypeDescriptorContext"/> that provides a format context.
    ''' </param>
    '''
    ''' <param name="culture">
    ''' The culture info.
    ''' </param>
    '''
    ''' <param name="value">
    ''' The value to convert.
    ''' </param>
    '''
    ''' <returns>
    ''' The converted value.
    ''' </returns>
    <DebuggerStepThrough>
    Public Overrides Function ConvertFrom(context As ITypeDescriptorContext,
                                          culture As CultureInfo,
                                          value As Object) As Object

        Dim s As String = TryCast(value, String)
        If s IsNot Nothing Then
            Dim idx As Integer = s.IndexOf("("c)
            If idx >= 0 Then
                s = s.Substring(0, idx)
            End If

            Return ColorTranslator.FromHtml(s.Trim())
        End If

        Return MyBase.ConvertFrom(context, culture, value)
    End Function

#End Region

End Class
