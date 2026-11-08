#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " GeometryPropertyInfoTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the geometry-related property info classes:
    ''' <see cref="PointPropertyInfo"/>, <see cref="PointPropertyContainer"/>,
    ''' <see cref="RectanglePropertyInfo"/> and <see cref="WindowBoundsPropertyContainer"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class GeometryPropertyInfoTests

#Region " PointPropertyInfo "

        ''' <summary>
        ''' Verifies the coordinates and the string representation of a <see cref="PointPropertyInfo"/>,
        ''' including negative coordinates (e.g. monitors placed left of or above the primary monitor).
        ''' </summary>
        '''
        ''' <param name="x">
        ''' The X coordinate.
        ''' </param>
        '''
        ''' <param name="y">
        ''' The Y coordinate.
        ''' </param>
        '''
        ''' <param name="expected">
        ''' The expected string.
        ''' </param>
        <TestMethod>
        <DataRow(0, 0, "0, 0")>
        <DataRow(1920, 1080, "1920, 1080")>
        <DataRow(-1280, -24, "-1280, -24")>
        Public Sub PointPropertyInfo_ValuesAndToString(x As Integer, y As Integer, expected As String)

            Dim info As New PointPropertyInfo(New Point(x, y))

            Assert.AreEqual(x, info.X)
            Assert.AreEqual(y, info.Y)
            Assert.AreEqual(expected, info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies the string representation of a <see cref="PointPropertyContainer"/>.
        ''' </summary>
        <TestMethod>
        Public Sub PointPropertyContainer_ToString_JoinsScreenAndClient()

            Dim container As New PointPropertyContainer With {
                .ScreenPointInfo = New PointPropertyInfo(New Point(500, 400)),
                .ClientPointInfo = New PointPropertyInfo(New Point(-8, 31))
            }

            Assert.AreEqual("500, 400 | -8, 31", container.ToString())
        End Sub

#End Region

#Region " RectanglePropertyInfo "

        ''' <summary>
        ''' Verifies the values and the string representation of a <see cref="RectanglePropertyInfo"/>.
        ''' </summary>
        <TestMethod>
        Public Sub RectanglePropertyInfo_ValuesAndToString()

            Dim rect As New Rectangle(-7, 10, 800, 600)
            Dim info As New RectanglePropertyInfo(rect)

            Assert.AreEqual(-7, info.X)
            Assert.AreEqual(10, info.Y)
            Assert.AreEqual(800, info.Width)
            Assert.AreEqual(600, info.Height)
            Assert.AreEqual(rect.Location, info.Location)
            Assert.AreEqual(rect.Size, info.Size)
            Assert.AreEqual("-7, 10; 800, 600", info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies the string representation of an empty <see cref="RectanglePropertyInfo"/>
        ''' (e.g. the bounds retrieved for <see cref="IntPtr.Zero"/>).
        ''' </summary>
        <TestMethod>
        Public Sub RectanglePropertyInfo_Empty_ToString()

            Assert.AreEqual("0, 0; 0, 0", New RectanglePropertyInfo(Rectangle.Empty).ToString())
        End Sub

        ''' <summary>
        ''' Verifies the string representation of a <see cref="WindowBoundsPropertyContainer"/>.
        ''' </summary>
        <TestMethod>
        Public Sub WindowBoundsPropertyContainer_ToString_JoinsRectangleAndClientRectangle()

            Dim container As New WindowBoundsPropertyContainer With {
                .RectangleInfo = New RectanglePropertyInfo(New Rectangle(100, 50, 640, 480)),
                .ClientRectangleInfo = New RectanglePropertyInfo(New Rectangle(108, 81, 624, 441))
            }

            Assert.AreEqual("100, 50; 640, 480 | 108, 81; 624, 441", container.ToString())
        End Sub

#End Region

    End Class

End Namespace

#End Region
