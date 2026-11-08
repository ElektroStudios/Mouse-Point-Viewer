#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Imports Win32

#End Region

''' <summary>
''' Provides helper methods to find the window under a screen point and to sample screen pixels.
''' </summary>
Public NotInheritable Class WindowHelper

#Region " Constructors "

    ''' <summary>
    ''' Prevents a default instance of the <see cref="WindowHelper"/> class from being created.
    ''' </summary>
    <DebuggerNonUserCode>
    Private Sub New()
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Returns the window under the specified screen point, honoring the
    ''' <see cref="Main.ignoreChildWindows"/>, <see cref="Main.ignoreLayeredWindows"/>
    ''' and <see cref="Main.ignoreTransparentWindows"/> options.
    ''' </summary>
    '''
    ''' <param name="ptScreen">
    ''' The point, in screen coordinates.
    ''' </param>
    '''
    ''' <returns>
    ''' The handle of the found window, or <see cref="IntPtr.Zero"/> if no window was found.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function GetWindowFromPoint(ptScreen As Point) As IntPtr

        If Main.ignoreChildWindows Then
            Return WindowHelper.GetTopLevelFromPoint(ptScreen)
        End If

        ' Child mode: iterate through top-level windows in Z-order and search the deepest child under the point.
        Dim topList As List(Of IntPtr) = WindowHelper.EnumTopLevelWindowsInZOrder()
        For Each top As IntPtr In topList
            Dim found As IntPtr = WindowHelper.GetDeepestWindowFromPoint(top, ptScreen)
            If found <> IntPtr.Zero Then
                Return found
            End If
        Next
        Return IntPtr.Zero
    End Function

    ''' <summary>
    ''' Returns the color of the screen pixel at the specified screen coordinates.
    ''' </summary>
    '''
    ''' <param name="screenX">
    ''' The X screen coordinate.
    ''' </param>
    '''
    ''' <param name="screenY">
    ''' The Y screen coordinate.
    ''' </param>
    '''
    ''' <returns>
    ''' The pixel color.
    ''' </returns>
    '''
    ''' <exception cref="ComponentModel.Win32Exception">
    ''' Thrown when the device context of the screen cannot be retrieved.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Shared Function GetPixelColorAtScreenPosition(screenX As Integer, screenY As Integer) As Color

        Dim hdc As IntPtr = NativeMethods.GetDC(IntPtr.Zero)
        If hdc = IntPtr.Zero Then
            Throw New ComponentModel.Win32Exception("Cannot retrieve the device context of the screen.")
        End If

        Dim pixel As UInteger
        Try
            pixel = NativeMethods.GetPixel(hdc, screenX, screenY)
        Finally
            NativeMethods.ReleaseDC(IntPtr.Zero, hdc)
        End Try

        Return WindowHelper.ColorFromColorRef(pixel)
    End Function

    ''' <summary>
    ''' Converts a Win32 <c>COLORREF</c> value (0x00BBGGRR) to an opaque <see cref="Color"/>.
    ''' </summary>
    '''
    ''' <param name="colorRef">
    ''' The <c>COLORREF</c> value.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="Color"/>, with an alpha component of 255.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function ColorFromColorRef(colorRef As UInteger) As Color

        Dim r As Integer = CInt(colorRef And &HFFUI)
        Dim g As Integer = CInt((colorRef >> 8) And &HFFUI)
        Dim b As Integer = CInt((colorRef >> 16) And &HFFUI)

        Return Color.FromArgb(r, g, b)
    End Function

    ''' <summary>
    ''' Retrieves the value at the specified offset of the extra window memory,
    ''' calling <c>GetWindowLongPtr</c> on 64-bit processes and <c>GetWindowLong</c> on 32-bit processes.
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' A handle to the window.
    ''' </param>
    '''
    ''' <param name="nIndex">
    ''' The zero-based offset to the value to be retrieved (e.g. <see cref="Constants.GWL_EXSTYLE"/>).
    ''' </param>
    '''
    ''' <returns>
    ''' The requested value.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function GetWindowLongPtrSafe(hWnd As IntPtr, nIndex As Integer) As IntPtr

        Return If(IntPtr.Size = 8,
            NativeMethods.GetWindowLongPtr(hWnd, nIndex),
            New IntPtr(NativeMethods.GetWindowLong(hWnd, nIndex)))
    End Function

    ''' <summary>
    ''' Changes an attribute of the specified window,
    ''' calling <c>SetWindowLongPtr</c> on 64-bit processes and <c>SetWindowLong</c> on 32-bit processes.
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' A handle to the window.
    ''' </param>
    '''
    ''' <param name="nIndex">
    ''' The zero-based offset to the value to be set (e.g. <see cref="Constants.GWL_EXSTYLE"/>).
    ''' </param>
    '''
    ''' <param name="dwNewLong">
    ''' The replacement value.
    ''' </param>
    '''
    ''' <returns>
    ''' The previous value.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function SetWindowLongPtrSafe(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr

        Return If(IntPtr.Size = 8,
            NativeMethods.SetWindowLongPtr(hWnd, nIndex, dwNewLong),
            New IntPtr(NativeMethods.SetWindowLong(hWnd, nIndex, dwNewLong.ToInt32())))
    End Function

    ''' <summary>
    ''' Determines whether the specified point is inside the specified rectangle.
    ''' <para></para>
    ''' The left and top edges are inclusive; the right and bottom edges are exclusive (Win32 semantics).
    ''' </summary>
    '''
    ''' <param name="r">
    ''' The rectangle.
    ''' </param>
    '''
    ''' <param name="p">
    ''' The point.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the point is inside the rectangle, <see langword="False"/> otherwise.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function PointInRect(r As NativeRectangle, p As Point) As Boolean

        Return p.X >= r.Left AndAlso p.X < r.Right AndAlso p.Y >= r.Top AndAlso p.Y < r.Bottom
    End Function

    ''' <summary>
    ''' Determines whether a pixel sampled from the device context of a window is considered visible.
    ''' <para></para>
    ''' A pixel is visible if its alpha is high enough or its color is not pure black.
    ''' </summary>
    '''
    ''' <param name="px">
    ''' The sampled pixel.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the pixel is considered visible, <see langword="False"/> otherwise.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function IsWindowPixelVisible(px As Color) As Boolean

        Return px.A > 8 OrElse Not (px.R = 0 AndAlso px.G = 0 AndAlso px.B = 0)
    End Function

    ''' <summary>
    ''' Determines whether a pixel sampled from the screen is considered visible.
    ''' <para></para>
    ''' A pixel is visible if it is not fully transparent and its color is not pure black.
    ''' </summary>
    '''
    ''' <param name="px">
    ''' The sampled pixel.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the pixel is considered visible, <see langword="False"/> otherwise.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function IsScreenPixelVisible(px As Color) As Boolean

        Return Not (px.A = 0 OrElse (px.R = 0 AndAlso px.G = 0 AndAlso px.B = 0))
    End Function

    ''' <summary>
    ''' Enumerates the top-level windows in Z-order (top to bottom).
    ''' </summary>
    '''
    ''' <returns>
    ''' The handles of the top-level windows.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function EnumTopLevelWindowsInZOrder() As List(Of IntPtr)

        Dim list As New List(Of IntPtr)()
        Dim cb As EnumWindowsProc =
            Function(hWnd As IntPtr, lParam As IntPtr) As Boolean
                list.Add(hWnd)
                Return True ' continue
            End Function

        NativeMethods.EnumWindows(cb, IntPtr.Zero)
        GC.KeepAlive(cb)
        Return list
    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Returns the first top-level window in Z-order (top to bottom) that is a hit candidate for the specified point.
    ''' </summary>
    '''
    ''' <param name="ptScreen">
    ''' The point, in screen coordinates.
    ''' </param>
    '''
    ''' <returns>
    ''' The handle of the found window, or <see cref="IntPtr.Zero"/> if no window was found.
    ''' </returns>
    Private Shared Function GetTopLevelFromPoint(ptScreen As Point) As IntPtr

        ' Iterate through top-level windows (EnumWindows returns in Z-order: top -> bottom)
        Dim tops As List(Of IntPtr) = WindowHelper.EnumTopLevelWindowsInZOrder()

        For Each hWnd As IntPtr In tops
            Try
                If WindowHelper.IsHitCandidate(hWnd, ptScreen) Then
                    Return hWnd
                End If

            Catch ex As Exception
                ' Protect against errors in system windows / unusual processes.
                Continue For
            End Try
        Next

        Return IntPtr.Zero
    End Function

    ''' <summary>
    ''' Returns the deepest window under the specified point, starting from the specified window
    ''' and descending iteratively through its child windows in Z-order.
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' The window where the search starts (usually a top-level window).
    ''' </param>
    '''
    ''' <param name="ptScreen">
    ''' The point, in screen coordinates.
    ''' </param>
    '''
    ''' <returns>
    ''' The deepest window that is a hit candidate for the point,
    ''' <paramref name="hWnd"/> itself when none of its children is a hit candidate,
    ''' or <see cref="IntPtr.Zero"/> if <paramref name="hWnd"/> is not a hit candidate.
    ''' </returns>
    Private Shared Function GetDeepestWindowFromPoint(hWnd As IntPtr, ptScreen As Point) As IntPtr

        If Not WindowHelper.IsHitCandidate(hWnd, ptScreen) Then
            Return IntPtr.Zero
        End If

        Dim current As IntPtr = hWnd
        Do
            Dim nextLevel As IntPtr = IntPtr.Zero

            Dim child As IntPtr = NativeMethods.GetWindow(current, Constants.GW_CHILD)
            While child <> IntPtr.Zero
                If WindowHelper.IsHitCandidate(child, ptScreen) Then
                    nextLevel = child
                    Exit While
                End If
                child = NativeMethods.GetWindow(child, Constants.GW_HWNDNEXT)
            End While

            If nextLevel = IntPtr.Zero Then
                ' No child is under the point: the current window is the deepest one.
                Return current
            End If

            current = nextLevel
        Loop
    End Function

    ''' <summary>
    ''' Determines whether the specified window is a hit candidate for the specified point:
    ''' it is visible, not cloaked, contains the point, and is not excluded by the
    ''' <see cref="Main.ignoreLayeredWindows"/> and <see cref="Main.ignoreTransparentWindows"/> options
    ''' nor by the pixel hit test.
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' A handle to the window.
    ''' </param>
    '''
    ''' <param name="ptScreen">
    ''' The point, in screen coordinates.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the window is a hit candidate, <see langword="False"/> otherwise.
    ''' </returns>
    Private Shared Function IsHitCandidate(hWnd As IntPtr, ptScreen As Point) As Boolean

        If Not NativeMethods.IsWindowVisible(hWnd) OrElse WindowHelper.IsWindowCloaked(hWnd) Then
            Return False
        End If

        Dim rect As NativeRectangle
        If Not NativeMethods.GetWindowRect(hWnd, rect) OrElse Not WindowHelper.PointInRect(rect, ptScreen) Then
            Return False
        End If

        Dim exStylePtr As IntPtr = WindowHelper.GetWindowLongPtrSafe(hWnd, Constants.GWL_EXSTYLE)
        Dim exStyle As Long = exStylePtr.ToInt64()
        Dim isLayered As Boolean = (exStyle And Constants.WS_EX_LAYERED) <> 0
        Dim isExTransparent As Boolean = (exStyle And Constants.WS_EX_TRANSPARENT) <> 0

        ' If we ignore layered windows and the window is layered -> skip
        If Main.ignoreLayeredWindows AndAlso isLayered Then
            Return False
        End If

        ' If we ignore "transparent" windows by style -> skip
        If Main.ignoreTransparentWindows AndAlso isExTransparent Then
            Return False
        End If

        ' Pixel hit test:
        ' only perform when it makes sense (is layered, or we asked to ignore transparent windows)
        Dim needPixelTest As Boolean = isLayered OrElse Main.ignoreTransparentWindows
        If needPixelTest Then
            Dim visibleAtPixel As Boolean
            Try
                visibleAtPixel = WindowHelper.PixelHitTest(hWnd, ptScreen)
            Catch
                ' if PixelHitTest fails, consider it visible to avoid blocking interaction.
                visibleAtPixel = True
            End Try

            If Not visibleAtPixel Then
                Return False
            End If
        End If

        Return True
    End Function

    ''' <summary>
    ''' Samples the pixel of the window at the specified point to determine whether the window is visible at that point.
    ''' <para></para>
    ''' If the pixel cannot be copied from the window device context, the screen pixel is sampled instead.
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' A handle to the window.
    ''' </param>
    '''
    ''' <param name="ptScreen">
    ''' The point, in screen coordinates.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the window is considered visible at the point, <see langword="False"/> otherwise.
    ''' </returns>
    Private Shared Function PixelHitTest(hWnd As IntPtr, ptScreen As Point) As Boolean

        Dim rect As NativeRectangle
        If Not NativeMethods.GetWindowRect(hWnd, rect) Then
            Return True
        End If

        Dim width As Integer = rect.Right - rect.Left
        Dim height As Integer = rect.Bottom - rect.Top
        If width <= 0 OrElse height <= 0 Then
            Return False
        End If

        Dim clientPt As New Point(ptScreen.X - rect.Left, ptScreen.Y - rect.Top)
        If clientPt.X < 0 OrElse clientPt.X >= width OrElse clientPt.Y < 0 OrElse clientPt.Y >= height Then
            Return False
        End If

        Dim hdcSrc As IntPtr = IntPtr.Zero
        Try
            hdcSrc = NativeMethods.GetDC(hWnd)
            If hdcSrc = IntPtr.Zero Then
                ' Fallback: if we can't obtain the DC, return True to avoid blocking interaction
                Return True
            End If

            Using bmp As New Bitmap(1, 1, PixelFormat.Format32bppArgb)
                Dim copied As Boolean
                Using gDest As Graphics = Graphics.FromImage(bmp)
                    Dim hdcDest As IntPtr = gDest.GetHdc()
                    Try
                        copied = NativeMethods.BitBlt(hdcDest, 0, 0, 1, 1, hdcSrc, clientPt.X, clientPt.Y, Constants.SRCCOPY)
                    Finally
                        ' Note: the HDC must be released exactly once.
                        gDest.ReleaseHdc(hdcDest)
                    End Try
                End Using

                If Not copied Then
                    Return WindowHelper.PixelHitTest_ScreenSample(ptScreen)
                End If

                Return WindowHelper.IsWindowPixelVisible(bmp.GetPixel(0, 0))
            End Using

        Catch
            Return True

        Finally
            If hdcSrc <> IntPtr.Zero Then
                NativeMethods.ReleaseDC(hWnd, hdcSrc)
            End If

        End Try
    End Function

    ''' <summary>
    ''' Samples the screen pixel at the specified point to determine whether it is visible.
    ''' </summary>
    '''
    ''' <param name="ptScreen">
    ''' The point, in screen coordinates.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the screen pixel is considered visible, <see langword="False"/> otherwise.
    ''' </returns>
    Private Shared Function PixelHitTest_ScreenSample(ptScreen As Point) As Boolean

        Try
            Using bmp As New Bitmap(1, 1, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CopyFromScreen(ptScreen, Point.Empty, New Size(1, 1))
                End Using
                Return WindowHelper.IsScreenPixelVisible(bmp.GetPixel(0, 0))
            End Using
        Catch
            Return True
        End Try
    End Function

    ''' <summary>
    ''' Determines whether the specified window is cloaked by DWM
    ''' (e.g. it is on another virtual desktop, or it is a suspended UWP app).
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' A handle to the window.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the window is cloaked, <see langword="False"/> otherwise.
    ''' </returns>
    Private Shared Function IsWindowCloaked(hWnd As IntPtr) As Boolean

        Dim val As Integer = 0
        Dim hr As Integer = NativeMethods.DwmGetWindowAttribute(hWnd, Constants.DWMWA_CLOAKED, val, Marshal.SizeOf(GetType(Integer)))
        Return hr = 0 AndAlso val <> 0
    End Function

#End Region

End Class
