#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Windows.Forms

#End Region

#Region " PixelColorPropertyInfoTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the <see cref="PixelColorPropertyInfo"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class PixelColorPropertyInfoTests

#Region " Values "

        ''' <summary>
        ''' Verifies all the displayed values of a color that has a known web name.
        ''' </summary>
        <TestMethod>
        Public Sub Values_KnownColor_AreCorrect()

            Dim info As New PixelColorPropertyInfo(Color.FromArgb(255, 0, 0))

            Assert.AreEqual("255, 0, 0", info.RGB)
            Assert.AreEqual(&HFF, info.Win32)
            Assert.AreEqual("#FF0000", info.Web)
            Assert.AreEqual("Red", info.WebName)
            Assert.AreEqual("#FF0000 (Red)", info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies all the displayed values of a color without a known web name.
        ''' </summary>
        <TestMethod>
        Public Sub Values_UnknownColor_AreCorrect()

            Dim info As New PixelColorPropertyInfo(Color.FromArgb(1, 2, 3))

            Assert.AreEqual("1, 2, 3", info.RGB)
            Assert.AreEqual(&H30201, info.Win32) ' COLORREF: 0x00BBGGRR
            Assert.AreEqual("#010203", info.Web)
            Assert.IsNull(info.WebName)
            Assert.AreEqual("#010203", info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies that <see cref="PixelColorPropertyInfo.Win32"/> is the <c>COLORREF</c> (0x00BBGGRR) representation,
        ''' and that it round-trips through <see cref="WindowHelper.ColorFromColorRef"/>.
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
        <DataRow(0, 0, 0)>
        <DataRow(255, 255, 255)>
        <DataRow(18, 52, 86)>
        <DataRow(200, 100, 50)>
        Public Sub Win32_IsColorRef_AndRoundTrips(r As Integer, g As Integer, b As Integer)

            Dim c As Color = Color.FromArgb(r, g, b)
            Dim info As New PixelColorPropertyInfo(c)

            Assert.AreEqual(r Or (g << 8) Or (b << 16), info.Win32)
            Assert.AreEqual(c.ToArgb(), WindowHelper.ColorFromColorRef(CUInt(info.Win32)).ToArgb())
        End Sub

        ''' <summary>
        ''' Verifies that the source color is preserved as-is.
        ''' </summary>
        <TestMethod>
        Public Sub Color_IsPreserved()

            Dim c As Color = Color.FromArgb(12, 34, 56)

            Assert.AreEqual(c, New PixelColorPropertyInfo(c).Color)
        End Sub

#End Region

#Region " Attributes "

        ''' <summary>
        ''' Verifies that the <see cref="PixelColorEditor"/> is the type editor used by the <see cref="PropertyGrid"/>,
        ''' which paints the color preview box.
        ''' </summary>
        <TestMethod>
        Public Sub Editor_IsPixelColorEditor()

            Dim editor As Object = TypeDescriptor.GetEditor(GetType(PixelColorPropertyInfo), GetType(UITypeEditor))

            Assert.IsInstanceOfType(Of PixelColorEditor)(editor)
        End Sub

        ''' <summary>
        ''' Verifies that the raw <see cref="PixelColorPropertyInfo.Color"/> property is not displayed,
        ''' and that the displayed properties have the expected display names.
        ''' </summary>
        <TestMethod>
        Public Sub BrowsableProperties_HaveExpectedDisplayNames()

            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(PixelColorPropertyInfo), {BrowsableAttribute.Yes})
            Dim names As New List(Of String)()
            For Each pd As PropertyDescriptor In props
                names.Add(pd.DisplayName)
            Next
            names.Sort(StringComparer.Ordinal)

            CollectionAssert.AreEqual({"RGB", "Web", "Web Name", "Win32"}, names)
        End Sub

#End Region

    End Class

End Namespace

#End Region
