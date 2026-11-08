#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Globalization

#End Region

#Region " WebColorConverterTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the <see cref="WebColorConverter"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class WebColorConverterTests

#Region " GetKnownColorName "

        ''' <summary>
        ''' Verifies that an RGB value that matches a known web color returns its name.
        ''' <para></para>
        ''' When several known colors share the same RGB value (e.g. Aqua/Cyan, Fuchsia/Magenta),
        ''' the first one in <see cref="KnownColor"/> enumeration order wins.
        ''' </summary>
        '''
        ''' <param name="r">
        ''' The red component.
        ''' </param>
        '''
        ''' <param name="g">
        ''' The green component.
        ''' </param>
        '''
        ''' <param name="b">
        ''' The blue component.
        ''' </param>
        '''
        ''' <param name="expectedName">
        ''' The expected known color name.
        ''' </param>
        <TestMethod>
        <DataRow(255, 0, 0, "Red")>
        <DataRow(0, 128, 0, "Green")>
        <DataRow(0, 0, 255, "Blue")>
        <DataRow(0, 0, 0, "Black")>
        <DataRow(255, 255, 255, "White")>
        <DataRow(0, 255, 255, "Aqua")>
        <DataRow(255, 0, 255, "Fuchsia")>
        <DataRow(240, 248, 255, "AliceBlue")>
        <DataRow(211, 211, 211, "LightGray")>
        Public Sub GetKnownColorName_MatchingRgb_ReturnsName(r As Integer, g As Integer, b As Integer, expectedName As String)

            Dim actual As String = WebColorConverter.GetKnownColorName(Color.FromArgb(r, g, b))

            Assert.AreEqual(expectedName, actual)
        End Sub

        ''' <summary>
        ''' Verifies that an RGB value without a matching known web color returns <see langword="Nothing"/>.
        ''' </summary>
        '''
        ''' <param name="r">
        ''' The red component.
        ''' </param>
        '''
        ''' <param name="g">
        ''' The green component.
        ''' </param>
        '''
        ''' <param name="b">
        ''' The blue component.
        ''' </param>
        <TestMethod>
        <DataRow(1, 2, 3)>
        <DataRow(254, 0, 0)>
        <DataRow(18, 52, 86)>
        Public Sub GetKnownColorName_NonMatchingRgb_ReturnsNothing(r As Integer, g As Integer, b As Integer)

            Assert.IsNull(WebColorConverter.GetKnownColorName(Color.FromArgb(r, g, b)))
        End Sub

        ''' <summary>
        ''' Verifies that system colors (e.g. <see cref="KnownColor.Window"/>) are never returned,
        ''' and that the fully transparent "Transparent" known color is not returned for white.
        ''' </summary>
        <TestMethod>
        Public Sub GetKnownColorName_NeverReturnsSystemOrTransparentColors()

            For Each kc As KnownColor In [Enum].GetValues(GetType(KnownColor))
                Dim kColor As Color = Color.FromKnownColor(kc)
                Dim name As String = WebColorConverter.GetKnownColorName(Color.FromArgb(kColor.R, kColor.G, kColor.B))

                If name IsNot Nothing Then
                    Dim found As Color = Color.FromName(name)
                    Assert.IsFalse(found.IsSystemColor, $"System color returned for {kc}: {name}")
                    Assert.AreNotEqual("Transparent", name, $"Transparent returned for {kc}")
                End If
            Next
        End Sub

        ''' <summary>
        ''' Verifies that the alpha component of the source color is ignored.
        ''' </summary>
        <TestMethod>
        Public Sub GetKnownColorName_IgnoresSourceAlpha()

            Assert.AreEqual("Red", WebColorConverter.GetKnownColorName(Color.FromArgb(10, 255, 0, 0)))
        End Sub

#End Region

#Region " ConvertTo "

        ''' <summary>
        ''' Verifies the string representation of colors with and without a known web name.
        ''' </summary>
        '''
        ''' <param name="r">
        ''' The red component.
        ''' </param>
        '''
        ''' <param name="g">
        ''' The green component.
        ''' </param>
        '''
        ''' <param name="b">
        ''' The blue component.
        ''' </param>
        '''
        ''' <param name="expected">
        ''' The expected string.
        ''' </param>
        <TestMethod>
        <DataRow(255, 0, 0, "#FF0000 (Red)")>
        <DataRow(0, 0, 0, "#000000 (Black)")>
        <DataRow(255, 255, 255, "#FFFFFF (White)")>
        <DataRow(1, 2, 3, "#010203")>
        <DataRow(171, 205, 239, "#ABCDEF")>
        Public Sub ConvertTo_String_ReturnsHtmlAndOptionalName(r As Integer, g As Integer, b As Integer, expected As String)

            Dim converter As New WebColorConverter()

            Dim actual As Object = converter.ConvertTo(Color.FromArgb(r, g, b), GetType(String))

            Assert.AreEqual(expected, actual)
        End Sub

        ''' <summary>
        ''' Verifies that the conversion result does not depend on the current culture.
        ''' </summary>
        <TestMethod>
        Public Sub ConvertTo_String_IsCultureInvariant()

            Dim converter As New WebColorConverter()
            Dim c As Color = Color.FromArgb(255, 0, 0)

            Dim invariant As Object = converter.ConvertTo(Nothing, CultureInfo.InvariantCulture, c, GetType(String))
            Dim turkish As Object = converter.ConvertTo(Nothing, New CultureInfo("tr-TR"), c, GetType(String))

            Assert.AreEqual("#FF0000 (Red)", invariant)
            Assert.AreEqual(invariant, turkish)
        End Sub

#End Region

#Region " ConvertFrom "

        ''' <summary>
        ''' Verifies that strings in the format produced by <see cref="WebColorConverter.ConvertTo"/>,
        ''' plain HTML colors and color names are parsed.
        ''' </summary>
        '''
        ''' <param name="value">
        ''' The string to parse.
        ''' </param>
        '''
        ''' <param name="expectedArgb">
        ''' The expected ARGB value (as returned by <see cref="Color.ToArgb"/>).
        ''' </param>
        <TestMethod>
        <DataRow("#FF0000 (Red)", &HFFFF0000)>
        <DataRow("#FF0000", &HFFFF0000)>
        <DataRow("#010203", &HFF010203)>
        <DataRow("Red", &HFFFF0000)>
        <DataRow("  #00FF00  ", &HFF00FF00)>
        <DataRow("  #0000FF (Blue)", &HFF0000FF)>
        Public Sub ConvertFrom_ValidString_ReturnsColor(value As String, expectedArgb As Integer)

            Dim converter As New WebColorConverter()

            Dim actual As Color = DirectCast(converter.ConvertFrom(value), Color)

            Assert.AreEqual(expectedArgb, actual.ToArgb())
        End Sub

        ''' <summary>
        ''' Verifies that converting a color to string and back yields the same RGB value, for every gray level
        ''' and for a deterministic set of pseudo-random colors.
        ''' </summary>
        <TestMethod>
        Public Sub ConvertTo_ThenConvertFrom_RoundTrips()

            Dim converter As New WebColorConverter()
            Dim rnd As New Random(1234)

            For i As Integer = 0 To 255
                Dim colors As Color() = {
                    Color.FromArgb(i, i, i),
                    Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256))
                }

                For Each c As Color In colors
                    Dim s As String = DirectCast(converter.ConvertTo(c, GetType(String)), String)
                    Dim back As Color = DirectCast(converter.ConvertFrom(s), Color)
                    Assert.AreEqual(c.ToArgb(), back.ToArgb(), $"Round-trip failed for '{s}'.")
                Next
            Next
        End Sub

#End Region

    End Class

End Namespace

#End Region
