#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Drawing
Imports System.Windows.Forms

Imports MousePointViewer.Tests.Helpers

Imports Win32

#End Region

#Region " WindowHelperIntegrationTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Integration tests for the <see cref="WindowHelper"/> class, using real top-level windows shown on the desktop.
    ''' <para></para>
    ''' These tests require an interactive desktop session.
    ''' <para></para>
    ''' Note: the window search options that are not the subject of a test are set to ignore layered and
    ''' "transparent" windows, so that invisible overlay windows of third-party software
    ''' (e.g. GPU or recording overlays covering the whole screen) do not interfere with the tests.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class WindowHelperIntegrationTests

#Region " Colors "

        ''' <summary>
        ''' The background color of the bottom test window.
        ''' </summary>
        Private Shared ReadOnly BottomColor As Color = Color.FromArgb(32, 160, 96)

        ''' <summary>
        ''' The background color of the top test window.
        ''' </summary>
        Private Shared ReadOnly TopColor As Color = Color.FromArgb(200, 64, 32)

        ''' <summary>
        ''' The background color of child controls.
        ''' </summary>
        Private Shared ReadOnly ChildColor As Color = Color.FromArgb(40, 80, 220)

#End Region

#Region " GetWindowFromPoint (top-level mode) "

        ''' <summary>
        ''' Verifies that the top-level window under the point is found, and that it is the top-level window
        ''' (not its child) when child windows are ignored.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_IgnoreChildWindows_ReturnsTopLevelWindow()

            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=True, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True),
                  window As New TestWindow(TestWindow.GetCenteredBounds(), BottomColor)

                Dim child As New Panel() With {.Dock = DockStyle.Fill, .BackColor = ChildColor}
                window.Form.Controls.Add(child)
                window.ShowAndPaint()

                Dim found As IntPtr = WindowHelper.GetWindowFromPoint(window.Center)

                Assert.AreEqual(window.Handle, found)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the top-most window in Z-order wins when two windows overlap.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_OverlappingWindows_ReturnsTopWindowInZOrder()

            Dim bounds As Rectangle = TestWindow.GetCenteredBounds()
            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=True, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True),
                  bottom As TestWindow = New TestWindow(bounds, BottomColor).ShowAndPaint(),
                  top As TestWindow = New TestWindow(bounds, TopColor).ShowAndPaint()

                Assert.AreEqual(top.Handle, WindowHelper.GetWindowFromPoint(top.Center))

                ' Once the top window is hidden, the bottom one is found.
                top.Form.Hide()
                TestWindow.PumpMessages()

                Assert.AreEqual(bottom.Handle, WindowHelper.GetWindowFromPoint(bottom.Center))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a point just outside the right/bottom edges of the window does not hit the window
        ''' (Win32 exclusive edges), while the last pixel inside does.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_WindowEdges_AreExclusive()

            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=True, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True),
                  window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), BottomColor).ShowAndPaint()

                Dim b As Rectangle = window.Bounds

                Assert.AreEqual(window.Handle, WindowHelper.GetWindowFromPoint(New Point(b.Left, b.Top)))
                Assert.AreEqual(window.Handle, WindowHelper.GetWindowFromPoint(New Point(b.Right - 1, b.Bottom - 1)))
                Assert.AreNotEqual(window.Handle, WindowHelper.GetWindowFromPoint(New Point(b.Right, b.Bottom)))
            End Using
        End Sub

#End Region

#Region " GetWindowFromPoint (child mode) "

        ''' <summary>
        ''' Verifies that the deepest child window under the point is found, at any nesting level,
        ''' and that the top-level window itself is returned where there is no child.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_ChildMode_ReturnsDeepestChild()

            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=False, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True),
                  window As New TestWindow(TestWindow.GetCenteredBounds(), BottomColor)

                ' Left half: Panel > Panel (nested). Right half: no children.
                Dim outer As New Panel() With {.Dock = DockStyle.Left, .Width = window.Form.ClientSize.Width \ 2, .BackColor = ChildColor}
                Dim inner As New Panel() With {.Location = New Point(10, 10), .Size = New Size(60, 60), .BackColor = TopColor}
                outer.Controls.Add(inner)
                window.Form.Controls.Add(outer)
                window.ShowAndPaint()

                Dim innerCenter As Point = inner.PointToScreen(New Point(inner.Width \ 2, inner.Height \ 2))
                Dim outerOnly As Point = outer.PointToScreen(New Point(outer.Width - 5, outer.Height - 5))
                Dim formOnly As Point = window.Form.PointToScreen(New Point(window.Form.ClientSize.Width - 5, window.Form.ClientSize.Height \ 2))

                Assert.AreEqual(inner.Handle, WindowHelper.GetWindowFromPoint(innerCenter), "Nested child not found.")
                Assert.AreEqual(outer.Handle, WindowHelper.GetWindowFromPoint(outerOnly), "Direct child not found.")
                Assert.AreEqual(window.Handle, WindowHelper.GetWindowFromPoint(formOnly), "Top-level window not found.")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that hidden child windows are skipped in child mode.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_ChildMode_SkipsHiddenChildren()

            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=False, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True),
                  window As New TestWindow(TestWindow.GetCenteredBounds(), BottomColor)

                Dim child As New Panel() With {.Dock = DockStyle.Fill, .BackColor = ChildColor}
                window.Form.Controls.Add(child)
                window.ShowAndPaint()

                Assert.AreEqual(child.Handle, WindowHelper.GetWindowFromPoint(window.Center))

                child.Visible = False
                TestWindow.PumpMessages()

                Assert.AreEqual(window.Handle, WindowHelper.GetWindowFromPoint(window.Center))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that, among overlapping sibling child windows, the one at the top of the Z-order wins.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_ChildMode_OverlappingSiblings_ReturnsTopSibling()

            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=False, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True),
                  window As New TestWindow(TestWindow.GetCenteredBounds(), BottomColor)

                Dim first As New Panel() With {.Dock = DockStyle.Fill, .BackColor = ChildColor}
                Dim second As New Panel() With {.Dock = DockStyle.Fill, .BackColor = TopColor}
                window.Form.Controls.Add(first)
                window.Form.Controls.Add(second)
                first.BringToFront()
                window.ShowAndPaint()

                Assert.AreEqual(first.Handle, WindowHelper.GetWindowFromPoint(window.Center))

                second.BringToFront()
                TestWindow.PumpMessages()

                Assert.AreEqual(second.Handle, WindowHelper.GetWindowFromPoint(window.Center))
            End Using
        End Sub

#End Region

#Region " GetWindowFromPoint (layered / transparent windows) "

        ''' <summary>
        ''' Verifies that a layered window on top is found when layered windows are not ignored,
        ''' and skipped (finding the window below) when they are.
        ''' </summary>
        '''
        ''' <param name="ignoreChildWindows">
        ''' The value for <see cref="Main.ignoreChildWindows"/>, to test both search modes.
        ''' </param>
        <TestMethod>
        <DataRow(True)>
        <DataRow(False)>
        Public Sub GetWindowFromPoint_LayeredWindow_HonorsIgnoreLayeredWindows(ignoreChildWindows As Boolean)

            Dim bounds As Rectangle = TestWindow.GetCenteredBounds()
            Using bottom As TestWindow = New TestWindow(bounds, BottomColor).ShowAndPaint(),
                  top As New TestWindow(bounds, TopColor)

                top.Form.Opacity = 0.9 ' WinForms makes the window layered (WS_EX_LAYERED).
                top.ShowAndPaint()

                Dim exStyle As Long = WindowHelper.GetWindowLongPtrSafe(top.Handle, Constants.GWL_EXSTYLE).ToInt64()
                Assert.AreNotEqual(0L, exStyle And Constants.WS_EX_LAYERED, "The top window is not layered.")

                Using options As New WindowSearchOptionsScope(ignoreChildWindows, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True)
                    Assert.AreEqual(bottom.Handle, WindowHelper.GetWindowFromPoint(top.Center))
                End Using

                Using options As New WindowSearchOptionsScope(ignoreChildWindows, ignoreLayeredWindows:=False, ignoreTransparentWindows:=False)
                    Assert.AreEqual(top.Handle, WindowHelper.GetWindowFromPoint(top.Center))
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a window with the <c>WS_EX_TRANSPARENT</c> extended style on top is found
        ''' when transparent windows are not ignored, and skipped (finding the window below) when they are.
        ''' </summary>
        '''
        ''' <param name="ignoreChildWindows">
        ''' The value for <see cref="Main.ignoreChildWindows"/>, to test both search modes.
        ''' </param>
        <TestMethod>
        <DataRow(True)>
        <DataRow(False)>
        Public Sub GetWindowFromPoint_TransparentWindow_HonorsIgnoreTransparentWindows(ignoreChildWindows As Boolean)

            Dim bounds As Rectangle = TestWindow.GetCenteredBounds()
            Using bottom As TestWindow = New TestWindow(bounds, BottomColor).ShowAndPaint(),
                  top As TestWindow = New TestWindow(bounds, TopColor).ShowAndPaint()

                top.AddExtendedStyles(Constants.WS_EX_TRANSPARENT)

                Using options As New WindowSearchOptionsScope(ignoreChildWindows, ignoreLayeredWindows:=True, ignoreTransparentWindows:=True)
                    Assert.AreEqual(bottom.Handle, WindowHelper.GetWindowFromPoint(top.Center))
                End Using

                Using options As New WindowSearchOptionsScope(ignoreChildWindows, ignoreLayeredWindows:=True, ignoreTransparentWindows:=False)
                    Assert.AreEqual(top.Handle, WindowHelper.GetWindowFromPoint(top.Center))
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a visible (non-black) pixel of a regular window passes the pixel hit test
        ''' performed when transparent windows are ignored.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowFromPoint_IgnoreTransparent_VisiblePixel_ReturnsWindow()

            Using options As New WindowSearchOptionsScope(ignoreChildWindows:=True, ignoreLayeredWindows:=False, ignoreTransparentWindows:=True),
                  window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), BottomColor).ShowAndPaint()

                Assert.AreEqual(window.Handle, WindowHelper.GetWindowFromPoint(window.Center))
            End Using
        End Sub

#End Region

#Region " EnumTopLevelWindowsInZOrder "

        ''' <summary>
        ''' Verifies that shown top-level windows are enumerated in Z-order (the last shown top-most window first),
        ''' and that child windows are not enumerated.
        ''' </summary>
        <TestMethod>
        Public Sub EnumTopLevelWindowsInZOrder_ReturnsTopLevelWindowsInZOrder()

            Dim bounds As Rectangle = TestWindow.GetCenteredBounds()
            Using bottom As TestWindow = New TestWindow(bounds, BottomColor).ShowAndPaint(),
                  top As New TestWindow(bounds, TopColor)

                Dim child As New Panel() With {.Dock = DockStyle.Fill}
                top.Form.Controls.Add(child)
                top.ShowAndPaint()

                Dim windows As List(Of IntPtr) = WindowHelper.EnumTopLevelWindowsInZOrder()

                Dim topIndex As Integer = windows.IndexOf(top.Handle)
                Dim bottomIndex As Integer = windows.IndexOf(bottom.Handle)
                Assert.IsTrue(topIndex >= 0, "The top window was not enumerated.")
                Assert.IsTrue(bottomIndex >= 0, "The bottom window was not enumerated.")
                Assert.IsTrue(topIndex < bottomIndex, "The windows are not in Z-order.")
                Assert.IsFalse(windows.Contains(child.Handle), "A child window was enumerated.")
            End Using
        End Sub

#End Region

#Region " Window Long Ptr "

        ''' <summary>
        ''' Verifies that <see cref="WindowHelper.SetWindowLongPtrSafe"/> returns the previous value,
        ''' and that <see cref="WindowHelper.GetWindowLongPtrSafe"/> reads the new value,
        ''' in both 32-bit and 64-bit processes.
        ''' </summary>
        <TestMethod>
        Public Sub SetAndGetWindowLongPtrSafe_ExtendedStyle_RoundTrips()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), BottomColor).ShowAndPaint()

                Dim original As IntPtr = WindowHelper.GetWindowLongPtrSafe(window.Handle, Constants.GWL_EXSTYLE)
                Assert.AreEqual(0L, original.ToInt64() And Constants.WS_EX_TRANSPARENT, "Unexpected initial style.")

                Dim newStyle As New IntPtr(original.ToInt64() Or Constants.WS_EX_TRANSPARENT)
                Dim previous As IntPtr = WindowHelper.SetWindowLongPtrSafe(window.Handle, Constants.GWL_EXSTYLE, newStyle)
                Dim current As IntPtr = WindowHelper.GetWindowLongPtrSafe(window.Handle, Constants.GWL_EXSTYLE)

                Assert.AreEqual(original, previous)
                Assert.AreNotEqual(0L, current.ToInt64() And Constants.WS_EX_TRANSPARENT)
            End Using
        End Sub

#End Region

#Region " GetPixelColorAtScreenPosition "

        ''' <summary>
        ''' Verifies that the color of the screen pixel at the center of a solid-color window is retrieved.
        ''' </summary>
        <TestMethod>
        Public Sub GetPixelColorAtScreenPosition_SolidWindow_ReturnsBackColor()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), TopColor).ShowAndPaint()

                Dim pt As Point = window.Center
                Dim actual As Color = Color.Empty

                ' The desktop composition (DWM) updates the screen asynchronously.
                Dim matched As Boolean = TestWindow.WaitUntil(
                    Function() As Boolean
                        actual = WindowHelper.GetPixelColorAtScreenPosition(pt.X, pt.Y)
                        Return actual.ToArgb() = TopColor.ToArgb()
                    End Function)

                Assert.IsTrue(matched, $"Expected {TopColor}, got {actual}.")
                Assert.AreEqual(CByte(255), actual.A)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
