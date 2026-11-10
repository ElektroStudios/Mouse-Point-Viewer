#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Windows.Forms

Imports MousePointViewer.Tests.Helpers

Imports Win32

#End Region

#Region " Magnifier Tests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the <see cref="MagnifierGeometry"/> module.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class MagnifierGeometryTests

#Region " Constants "

        ''' <summary>
        ''' The size of the view used in the tests (the default magnifier size at 96 DPI).
        ''' </summary>
        Private Shared ReadOnly ViewSize As New Size(220, 220)

        ''' <summary>
        ''' A primary 1920x1080 screen.
        ''' </summary>
        Private Shared ReadOnly PrimaryScreen As New Rectangle(0, 0, 1920, 1080)

        ''' <summary>
        ''' A secondary 1280x1024 screen placed to the left of the primary screen (negative coordinates).
        ''' </summary>
        Private Shared ReadOnly LeftScreen As New Rectangle(-1280, -100, 1280, 1024)

#End Region

#Region " Zoom "

        ''' <summary>
        ''' Verifies that the zoom is clamped to its range, and that the default zoom is within the range.
        ''' </summary>
        <TestMethod>
        Public Sub ClampZoom_ClampsToRange()

            Assert.AreEqual(MagnifierGeometry.MinZoom, MagnifierGeometry.ClampZoom(Integer.MinValue))
            Assert.AreEqual(MagnifierGeometry.MinZoom, MagnifierGeometry.ClampZoom(MagnifierGeometry.MinZoom - 1))
            Assert.AreEqual(MagnifierGeometry.MaxZoom, MagnifierGeometry.ClampZoom(MagnifierGeometry.MaxZoom + 1))
            Assert.AreEqual(MagnifierGeometry.MaxZoom, MagnifierGeometry.ClampZoom(Integer.MaxValue))
            Assert.AreEqual(MagnifierGeometry.DefaultZoom, MagnifierGeometry.ClampZoom(MagnifierGeometry.DefaultZoom))
        End Sub

#End Region

#Region " Capture "

        ''' <summary>
        ''' Verifies that the captured area always has odd dimensions (so there is a center pixel)
        ''' and that, once zoomed, it covers the whole view, for every zoom and several view sizes.
        ''' </summary>
        <TestMethod>
        Public Sub GetCaptureSize_IsOddAndCoversView()

            For Each view As Size In {ViewSize, New Size(221, 199), New Size(330, 330), New Size(1, 1)}
                For zoom As Integer = MagnifierGeometry.MinZoom To MagnifierGeometry.MaxZoom
                    Dim size As Size = MagnifierGeometry.GetCaptureSize(view, zoom)

                    Assert.AreEqual(1, size.Width Mod 2, $"Even width for {view} at {zoom}x.")
                    Assert.AreEqual(1, size.Height Mod 2, $"Even height for {view} at {zoom}x.")
                    Assert.IsTrue(size.Width * zoom >= view.Width, $"The width does not cover {view} at {zoom}x.")
                    Assert.IsTrue(size.Height * zoom >= view.Height, $"The height does not cover {view} at {zoom}x.")
                    Assert.IsTrue((size.Width - 2) * zoom < view.Width, $"The width is too large for {view} at {zoom}x.")
                Next
            Next
        End Sub

        ''' <summary>
        ''' Verifies that the center pixel of the captured area is the specified point (the pixel under the mouse cursor).
        ''' </summary>
        <TestMethod>
        Public Sub GetCaptureRectangle_IsCenteredAtPoint()

            For Each center As Point In {New Point(500, 400), New Point(0, 0), New Point(-1000, -50)}
                For zoom As Integer = MagnifierGeometry.MinZoom To MagnifierGeometry.MaxZoom
                    Dim rect As Rectangle = MagnifierGeometry.GetCaptureRectangle(center, ViewSize, zoom)

                    Assert.AreEqual(center.X, rect.X + (rect.Width \ 2), $"X at {zoom}x.")
                    Assert.AreEqual(center.Y, rect.Y + (rect.Height \ 2), $"Y at {zoom}x.")
                    Assert.IsTrue(rect.Contains(center))
                Next
            Next
        End Sub

        ''' <summary>
        ''' Verifies that the zoomed destination covers the whole view, and that the center pixel is drawn at the center of the view.
        ''' </summary>
        <TestMethod>
        Public Sub GetDestinationAndCenterPixel_AreCentered()

            For zoom As Integer = MagnifierGeometry.MinZoom To MagnifierGeometry.MaxZoom
                Dim dest As Rectangle = MagnifierGeometry.GetDestinationRectangle(ViewSize, zoom)
                Dim cell As Rectangle = MagnifierGeometry.GetCenterPixelRectangle(ViewSize, zoom)

                Assert.IsTrue(dest.Contains(New Rectangle(Point.Empty, ViewSize)), $"The destination does not cover the view at {zoom}x.")
                Assert.AreEqual(zoom, cell.Width)
                Assert.AreEqual(zoom, cell.Height)

                Dim viewCenter As New Point(ViewSize.Width \ 2, ViewSize.Height \ 2)
                Assert.IsTrue(Math.Abs((cell.X + (cell.Width / 2.0)) - viewCenter.X) <= 1, $"The center pixel is not centered horizontally at {zoom}x.")
                Assert.IsTrue(Math.Abs((cell.Y + (cell.Height / 2.0)) - viewCenter.Y) <= 1, $"The center pixel is not centered vertically at {zoom}x.")
            Next
        End Sub

        ''' <summary>
        ''' Verifies that invalid view sizes and zoom factors throw <see cref="ArgumentOutOfRangeException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub GetCaptureSize_InvalidArguments_ThrowArgumentOutOfRangeException()

            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() MagnifierGeometry.GetCaptureSize(Size.Empty, MagnifierGeometry.DefaultZoom))
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() MagnifierGeometry.GetCaptureSize(New Size(-1, 10), MagnifierGeometry.DefaultZoom))
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() MagnifierGeometry.GetCaptureSize(ViewSize, MagnifierGeometry.MinZoom - 1))
            Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() MagnifierGeometry.GetCaptureSize(ViewSize, MagnifierGeometry.MaxZoom + 1))
        End Sub

#End Region

#Region " Window Bounds "

        ''' <summary>
        ''' Verifies that the magnifier window is placed at the bottom-right of the captured area by default.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowBounds_Default_IsBottomRight()

            Dim capture As Rectangle = MagnifierGeometry.GetCaptureRectangle(New Point(500, 400), ViewSize, MagnifierGeometry.DefaultZoom)

            Dim bounds As Rectangle = MagnifierGeometry.GetWindowBounds(capture, ViewSize, PrimaryScreen, 16)

            Assert.AreEqual(capture.Right + 16, bounds.Left)
            Assert.AreEqual(capture.Bottom + 16, bounds.Top)
            Assert.AreEqual(ViewSize, bounds.Size)
        End Sub

        ''' <summary>
        ''' Verifies that the magnifier window is placed on the left side near the right edge of the screen,
        ''' and above near the bottom edge of the screen.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowBounds_NearEdges_FlipsToOppositeSide()

            Dim capture As Rectangle = MagnifierGeometry.GetCaptureRectangle(New Point(1900, 1070), ViewSize, MagnifierGeometry.DefaultZoom)

            Dim bounds As Rectangle = MagnifierGeometry.GetWindowBounds(capture, ViewSize, PrimaryScreen, 16)

            Assert.AreEqual(capture.Left - 16 - ViewSize.Width, bounds.Left)
            Assert.AreEqual(capture.Top - 16 - ViewSize.Height, bounds.Top)
        End Sub

        ''' <summary>
        ''' Verifies, for mouse positions all over two screens (including corners and edges) and for every zoom,
        ''' that the magnifier window is always inside the screen and never covers the captured area
        ''' (nor, therefore, the pixel under the mouse cursor).
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowBounds_AnyPosition_InsideScreenAndNeverOverCapture()

            For Each screenBounds As Rectangle In {PrimaryScreen, LeftScreen}
                For x As Integer = screenBounds.Left To screenBounds.Right - 1 Step 37
                    For y As Integer = screenBounds.Top To screenBounds.Bottom - 1 Step 41
                        For Each pt As Point In {New Point(x, y), New Point(screenBounds.Right - 1, y), New Point(x, screenBounds.Bottom - 1)}
                            For zoom As Integer = MagnifierGeometry.MinZoom To MagnifierGeometry.MaxZoom Step 2
                                Dim capture As Rectangle = MagnifierGeometry.GetCaptureRectangle(pt, ViewSize, zoom)
                                Dim bounds As Rectangle = MagnifierGeometry.GetWindowBounds(capture, ViewSize, screenBounds, 16)

                                If Not screenBounds.Contains(bounds) Then
                                    Assert.Fail($"Outside the screen {screenBounds}: {bounds} (cursor {pt}, {zoom}x).")
                                End If
                                If bounds.IntersectsWith(capture) Then
                                    Assert.Fail($"Over the captured area {capture}: {bounds} (cursor {pt}, {zoom}x).")
                                End If
                            Next
                        Next
                    Next
                Next
            Next
        End Sub

#End Region

    End Class

    ''' <summary>
    ''' Tests for the <see cref="MagnifierWindow"/> class, using real windows.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class MagnifierWindowTests

        ''' <summary>
        ''' Verifies the default zoom, and that zooming in and out is clamped to the zoom range.
        ''' </summary>
        <TestMethod>
        Public Sub Zoom_DefaultAndClamped()

            Using magnifier As New MagnifierWindow()
                Assert.AreEqual(MagnifierGeometry.DefaultZoom, magnifier.Zoom)

                For i As Integer = 1 To 100
                    magnifier.ZoomIn()
                Next
                Assert.AreEqual(MagnifierGeometry.MaxZoom, magnifier.Zoom)

                For i As Integer = 1 To 100
                    magnifier.ZoomOut()
                Next
                Assert.AreEqual(MagnifierGeometry.MinZoom, magnifier.Zoom)

                magnifier.Zoom = 1000
                Assert.AreEqual(MagnifierGeometry.MaxZoom, magnifier.Zoom)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that null palettes and hint lines throw <see cref="ArgumentNullException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub NullProperties_ThrowArgumentNullException()

            Using magnifier As New MagnifierWindow()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() magnifier.Palette = Nothing)
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() magnifier.HintLines = Nothing)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the magnifier window is a non-activable, mouse-transparent, top-most tool window
        ''' (it never steals the focus nor the mouse input, and it does not appear in the taskbar nor in Alt+Tab).
        ''' </summary>
        <TestMethod>
        Public Sub Window_HasNonIntrusiveExtendedStyles()

            Using magnifier As New MagnifierWindow()
                Dim exStyle As Long = WindowHelper.GetWindowLongPtrSafe(magnifier.Handle, Constants.GWL_EXSTYLE).ToInt64()

                For Each style As Integer In {Constants.WS_EX_TOOLWINDOW, Constants.WS_EX_NOACTIVATE, Constants.WS_EX_TOPMOST,
                                              Constants.WS_EX_LAYERED, Constants.WS_EX_TRANSPARENT}
                    Assert.AreNotEqual(0L, exStyle And style, $"Missing extended style 0x{style:X}.")
                Next
                Assert.IsFalse(magnifier.ShowInTaskbar)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the magnifier captures the screen around the specified point, that the center pixel is the
        ''' pixel at that point, and that the magnifier window is placed next to (never over) the captured area.
        ''' </summary>
        <TestMethod>
        Public Sub RefreshFrame_CapturesAroundPointAndStaysAside()

            Dim target As Color = Color.FromArgb(200, 64, 32)
            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), target).ShowAndPaint(),
                  magnifier As New MagnifierWindow()

                ' The magnifier is not shown, so its refresh timer does not move it to the real mouse position.
                Dim pt As Point = window.Center

                Dim matched As Boolean = TestWindow.WaitUntil(
                    Function() As Boolean
                        magnifier.RefreshFrame(pt)
                        Return magnifier.CenterPixelColor.ToArgb() = target.ToArgb()
                    End Function)

                Assert.IsTrue(matched, $"Expected {target}, got {magnifier.CenterPixelColor}.")
                Assert.AreEqual(MagnifierGeometry.GetCaptureRectangle(pt, magnifier.ClientSize, magnifier.Zoom), magnifier.CaptureRectangle)
                Assert.IsFalse(magnifier.Bounds.IntersectsWith(magnifier.CaptureRectangle), "The magnifier covers the captured area.")
                Assert.IsFalse(magnifier.Bounds.Contains(pt), "The magnifier covers the pixel under the cursor.")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the magnifier renders (with every theme and zoom) without throwing.
        ''' </summary>
        <TestMethod>
        Public Sub Render_AllThemesAndZooms_DoesNotThrow()

            Using magnifier As New MagnifierWindow() With {.HintLines = {"Line 1", "Line 2"}}
                magnifier.Show()
                For Each palette As ThemePalette In {ThemePalette.Light, ThemePalette.Dark}
                    magnifier.Palette = palette
                    For zoom As Integer = MagnifierGeometry.MinZoom To MagnifierGeometry.MaxZoom
                        magnifier.Zoom = zoom
                        magnifier.RefreshFrame(New Point(100, 100))
                        magnifier.Update()
                    Next
                Next
                magnifier.Hide()
            End Using
        End Sub

    End Class

End Namespace

#End Region
