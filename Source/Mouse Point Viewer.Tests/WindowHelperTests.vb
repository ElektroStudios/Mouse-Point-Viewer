#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

Imports Win32

#End Region

#Region " WindowHelperTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the pure (non-Win32) helper methods of the <see cref="WindowHelper"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class WindowHelperTests

#Region " ColorFromColorRef "

        ''' <summary>
        ''' Verifies that a <c>COLORREF</c> (0x00BBGGRR) is decoded into the right RGB components,
        ''' with an opaque alpha.
        ''' </summary>
        '''
        ''' <param name="colorRef">
        ''' The <c>COLORREF</c> value.
        ''' </param>
        '''
        ''' <param name="r">
        ''' The expected red component.
        ''' </param>
        '''
        ''' <param name="g">
        ''' The expected green component.
        ''' </param>
        '''
        ''' <param name="b">
        ''' The expected blue component.
        ''' </param>
        <TestMethod>
        <DataRow(&H0UI, 0, 0, 0)>
        <DataRow(&HFFUI, 255, 0, 0)>
        <DataRow(&HFF00UI, 0, 255, 0)>
        <DataRow(&HFF0000UI, 0, 0, 255)>
        <DataRow(&H332211UI, &H11, &H22, &H33)>
        <DataRow(&HFFFFFFUI, 255, 255, 255)>
        Public Sub ColorFromColorRef_DecodesComponents(colorRef As UInteger, r As Integer, g As Integer, b As Integer)

            Dim c As Color = WindowHelper.ColorFromColorRef(colorRef)

            Assert.AreEqual(CByte(255), c.A)
            Assert.AreEqual(CByte(r), c.R)
            Assert.AreEqual(CByte(g), c.G)
            Assert.AreEqual(CByte(b), c.B)
        End Sub

        ''' <summary>
        ''' Verifies that the high-order byte of the <c>COLORREF</c> is ignored
        ''' (e.g. <c>CLR_INVALID</c>, 0xFFFFFFFF, is decoded as white instead of throwing).
        ''' </summary>
        <TestMethod>
        Public Sub ColorFromColorRef_IgnoresHighOrderByte()

            Assert.AreEqual(Color.FromArgb(&H11, &H22, &H33).ToArgb(), WindowHelper.ColorFromColorRef(&HAB332211UI).ToArgb())
            Assert.AreEqual(Color.FromArgb(255, 255, 255).ToArgb(), WindowHelper.ColorFromColorRef(&HFFFFFFFFUI).ToArgb())
        End Sub

        ''' <summary>
        ''' Verifies that <see cref="ColorTranslator.ToWin32"/> and <see cref="WindowHelper.ColorFromColorRef"/>
        ''' are inverse operations for every value of each component.
        ''' </summary>
        <TestMethod>
        Public Sub ColorFromColorRef_IsInverseOfColorTranslatorToWin32()

            For v As Integer = 0 To 255
                Dim colors As Color() = {
                    Color.FromArgb(v, 0, 0), Color.FromArgb(0, v, 0), Color.FromArgb(0, 0, v), Color.FromArgb(v, 255 - v, v \ 2)
                }
                For Each c As Color In colors
                    Dim colorRef As UInteger = CUInt(ColorTranslator.ToWin32(c))
                    Assert.AreEqual(c.ToArgb(), WindowHelper.ColorFromColorRef(colorRef).ToArgb())
                Next
            Next
        End Sub

#End Region

#Region " PointInRect "

        ''' <summary>
        ''' Verifies the Win32 rectangle semantics: left and top edges inclusive, right and bottom edges exclusive.
        ''' </summary>
        '''
        ''' <param name="x">
        ''' The X coordinate of the point.
        ''' </param>
        '''
        ''' <param name="y">
        ''' The Y coordinate of the point.
        ''' </param>
        '''
        ''' <param name="expected">
        ''' The expected result.
        ''' </param>
        <TestMethod>
        <DataRow(10, 20, True)>   ' top-left corner
        <DataRow(29, 39, True)>   ' last pixel inside
        <DataRow(20, 30, True)>   ' center
        <DataRow(30, 30, False)>  ' right edge (exclusive)
        <DataRow(20, 40, False)>  ' bottom edge (exclusive)
        <DataRow(9, 30, False)>   ' left of the left edge
        <DataRow(20, 19, False)>  ' above the top edge
        <DataRow(30, 40, False)>  ' bottom-right corner (exclusive)
        Public Sub PointInRect_EdgeSemantics(x As Integer, y As Integer, expected As Boolean)

            Dim r As New NativeRectangle With {.Left = 10, .Top = 20, .Right = 30, .Bottom = 40}

            Assert.AreEqual(expected, WindowHelper.PointInRect(r, New Point(x, y)))
        End Sub

        ''' <summary>
        ''' Verifies that rectangles with negative coordinates (monitors left of or above the primary monitor) work.
        ''' </summary>
        <TestMethod>
        Public Sub PointInRect_NegativeCoordinates()

            Dim r As New NativeRectangle With {.Left = -1920, .Top = -200, .Right = 0, .Bottom = 880}

            Assert.IsTrue(WindowHelper.PointInRect(r, New Point(-1920, -200)))
            Assert.IsTrue(WindowHelper.PointInRect(r, New Point(-1, 879)))
            Assert.IsFalse(WindowHelper.PointInRect(r, New Point(0, 0)))
        End Sub

        ''' <summary>
        ''' Verifies that an empty rectangle never contains any point.
        ''' </summary>
        <TestMethod>
        Public Sub PointInRect_EmptyRectangle_ReturnsFalse()

            Dim r As New NativeRectangle With {.Left = 5, .Top = 5, .Right = 5, .Bottom = 5}

            Assert.IsFalse(WindowHelper.PointInRect(r, New Point(5, 5)))
        End Sub

#End Region

#Region " Pixel Visibility "

        ''' <summary>
        ''' Verifies the visibility threshold of a pixel sampled from a window device context:
        ''' visible when its alpha is greater than 8 or its color is not pure black.
        ''' </summary>
        '''
        ''' <param name="a">
        ''' The alpha component.
        ''' </param>
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
        ''' The expected result.
        ''' </param>
        <TestMethod>
        <DataRow(0, 0, 0, 0, False)>
        <DataRow(8, 0, 0, 0, False)>
        <DataRow(9, 0, 0, 0, True)>
        <DataRow(255, 0, 0, 0, True)>
        <DataRow(0, 1, 0, 0, True)>
        <DataRow(0, 0, 1, 0, True)>
        <DataRow(0, 0, 0, 1, True)>
        Public Sub IsWindowPixelVisible_Threshold(a As Integer, r As Integer, g As Integer, b As Integer, expected As Boolean)

            Assert.AreEqual(expected, WindowHelper.IsWindowPixelVisible(Color.FromArgb(a, r, g, b)))
        End Sub

        ''' <summary>
        ''' Verifies the visibility of a pixel sampled from the screen:
        ''' visible when it is not fully transparent and its color is not pure black.
        ''' </summary>
        '''
        ''' <param name="a">
        ''' The alpha component.
        ''' </param>
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
        ''' The expected result.
        ''' </param>
        <TestMethod>
        <DataRow(255, 0, 0, 0, False)>
        <DataRow(0, 255, 255, 255, False)>
        <DataRow(0, 0, 0, 0, False)>
        <DataRow(255, 1, 0, 0, True)>
        <DataRow(255, 255, 255, 255, True)>
        <DataRow(1, 0, 0, 1, True)>
        Public Sub IsScreenPixelVisible_Threshold(a As Integer, r As Integer, g As Integer, b As Integer, expected As Boolean)

            Assert.AreEqual(expected, WindowHelper.IsScreenPixelVisible(Color.FromArgb(a, r, g, b)))
        End Sub

#End Region

    End Class

End Namespace

#End Region
