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

Public NotInheritable Class WindowHelper

#Region " Public Methods "

    <DebuggerStepThrough>
    Friend Shared Function GetWindowFromPoint(ptScreen As Point) As IntPtr

        If Main.ignoreChildWindows Then
            Return WindowHelper.GetTopLevelFromPoint(ptScreen)
        Else
            ' Child mode: iterate through top-level windows in Z-order and search children recursively
            Dim topList As List(Of IntPtr) = WindowHelper.EnumTopLevelWindowsInZOrder()
            For Each top As IntPtr In topList
                Dim found As IntPtr = WindowHelper.GetWindowFromPointRecursive(top, ptScreen)
                If found <> IntPtr.Zero Then
                    Return found
                End If
            Next
            Return IntPtr.Zero
        End If
    End Function

    <DebuggerStepThrough>
    Friend Shared Function GetPixelColorAtScreenPosition(screenX As Integer, screenY As Integer) As Color

        Dim hdc As IntPtr = NativeMethods.GetDC(IntPtr.Zero)

        Dim pixel As UInteger = NativeMethods.GetPixel(hdc, screenX, screenY)

        NativeMethods.ReleaseDC(IntPtr.Zero, hdc)

        Dim r As Integer = CInt(pixel And &HFF)
        Dim g As Integer = CInt((pixel >> 8) And &HFF)
        Dim b As Integer = CInt((pixel >> 16) And &HFF)

        Return Color.FromArgb(r, g, b)
    End Function

    <DebuggerStepThrough>
    Friend Shared Function GetWindowLongPtrSafe(hWnd As IntPtr, nIndex As Integer) As IntPtr

        Return If(IntPtr.Size = 8,
            NativeMethods.GetWindowLongPtr(hWnd, nIndex),
            New IntPtr(NativeMethods.GetWindowLong(hWnd, nIndex)))
    End Function

    <DebuggerStepThrough>
    Friend Shared Function SetWindowLongPtrSafe(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr

        Return If(IntPtr.Size = 8,
            NativeMethods.SetWindowLongPtr(hWnd, nIndex, dwNewLong),
            New IntPtr(NativeMethods.SetWindowLong(hWnd, nIndex, dwNewLong.ToInt32())))
    End Function

#End Region

#Region " Private Methods "

    Private Shared Function GetTopLevelFromPoint(ptScreen As Point) As IntPtr

        ' Iterate through top-level windows (EnumWindows returns in Z-order: top -> bottom)
        Dim tops As List(Of IntPtr) = WindowHelper.EnumTopLevelWindowsInZOrder()

        For Each hWnd As IntPtr In tops
            Try
                If Not NativeMethods.IsWindowVisible(hWnd) OrElse WindowHelper.IsWindowCloaked(hWnd) Then
                    Continue For
                End If

                Dim rect As NativeRectangle
                If Not NativeMethods.GetWindowRect(hWnd, rect) Then
                    Continue For
                End If
                If Not WindowHelper.PointInRect(rect, ptScreen) Then
                    Continue For
                End If

                Dim exStylePtr As IntPtr = WindowHelper.GetWindowLongPtrSafe(hWnd, Constants.GWL_EXSTYLE)
                Dim exStyle As Long = exStylePtr.ToInt64()
                Dim isLayered As Boolean = (exStyle And Constants.WS_EX_LAYERED) <> 0
                Dim isExTransparent As Boolean = (exStyle And Constants.WS_EX_TRANSPARENT) <> 0

                ' If we ignore layered windows and the window is layered -> skip
                If Main.ignoreLayeredWindows AndAlso isLayered Then
                    Continue For
                End If

                ' If we ignore "transparent" windows by style -> skip
                If Main.ignoreTransparentWindows AndAlso isExTransparent Then
                    Continue For
                End If

                ' Pixel hit test:
                ' only perform when it makes sense (is layered,
                ' or we asked to ignore transparent,
                ' or when we want to apply the test for safety)
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
                        ' If we asked to ignore transparents -> skip; otherwise, original layered logic failing pixel -> skip
                        Continue For
                    End If
                End If

                ' If we reached here, this is the top-level window we want.
                Return hWnd

            Catch ex As Exception
                ' Protect against errors in system windows / unusual processes.
                Continue For
            End Try
        Next

        Return IntPtr.Zero
    End Function

    Private Shared Function GetWindowFromPointRecursive(hWnd As IntPtr, ptScreen As Point) As IntPtr

        ' Adaptation to respect ignoreLayeredWindows and ignoreTransparentWindows.
        If Not NativeMethods.IsWindowVisible(hWnd) OrElse WindowHelper.IsWindowCloaked(hWnd) Then
            Return IntPtr.Zero
        End If

        Dim rect As NativeRectangle
        If Not NativeMethods.GetWindowRect(hWnd, rect) OrElse Not PointInRect(rect, ptScreen) Then
            Return IntPtr.Zero
        End If

        Dim exStylePtr As IntPtr = WindowHelper.GetWindowLongPtrSafe(hWnd, Constants.GWL_EXSTYLE)
        Dim exStyle As Long = exStylePtr.ToInt64()
        Dim isLayered As Boolean = (exStyle And Constants.WS_EX_LAYERED) <> 0
        Dim isExTransparent As Boolean = (exStyle And Constants.WS_EX_TRANSPARENT) <> 0

        ' Respect ignoreLayeredWindows.
        If Main.ignoreLayeredWindows AndAlso isLayered Then
            Return IntPtr.Zero
        End If

        ' Respect ignoreTransparentWindows by style.
        If Main.ignoreTransparentWindows AndAlso isExTransparent Then
            Return IntPtr.Zero
        End If

        ' Pixel test if needed.
        Dim needPixelTest As Boolean = isLayered OrElse Main.ignoreTransparentWindows
        If needPixelTest Then
            Dim visibleAtPixel As Boolean
            Try
                visibleAtPixel = WindowHelper.PixelHitTest(hWnd, ptScreen)
            Catch
                visibleAtPixel = True
            End Try

            If Not visibleAtPixel Then
                Return IntPtr.Zero
            End If
        End If

        ' If we do not ignore children, dive deeper to find the deepest child.
        If Not Main.ignoreChildWindows Then
            Dim child As IntPtr = NativeMethods.GetWindow(hWnd, Constants.GW_CHILD)
            While child <> IntPtr.Zero
                Dim found As IntPtr = WindowHelper.GetWindowFromPointRecursive(child, ptScreen)
                If found <> IntPtr.Zero Then Return found
                child = NativeMethods.GetWindow(child, Constants.GW_HWNDNEXT)
            End While
        End If

        ' If no children are found, return this window (top-level or child depending on context).
        Return hWnd
    End Function

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

        Dim exStylePtr As IntPtr = WindowHelper.GetWindowLongPtrSafe(hWnd, Constants.GWL_EXSTYLE)
        Dim exStyle As Long = exStylePtr.ToInt64()
        Dim isLayered As Boolean = (exStyle And Constants.WS_EX_LAYERED) <> 0

        Dim hdcSrc As IntPtr = IntPtr.Zero
        Dim bmp As Bitmap = Nothing
        Try
            hdcSrc = NativeMethods.GetDC(hWnd)
            If hdcSrc = IntPtr.Zero Then
                ' Fallback: if we can't obtain the DC, return True to avoid blocking interaction
                Return True
            End If

            bmp = New Bitmap(1, 1, PixelFormat.Format32bppArgb)
            Using gDest As Graphics = Graphics.FromImage(bmp)
                Dim hdcDest As IntPtr = gDest.GetHdc()
                Try
                    Dim ptClient As Point = clientPt
                    Dim ok As Boolean = NativeMethods.BitBlt(hdcDest, 0, 0, 1, 1, hdcSrc, ptClient.X, ptClient.Y, Constants.SRCCOPY)
                    If Not ok Then
                        gDest.ReleaseHdc(hdcDest)
                        Return WindowHelper.PixelHitTest_ScreenSample(ptScreen)
                    End If
                Finally
                    gDest.ReleaseHdc(hdcDest)
                End Try
            End Using

            Dim px As Color = bmp.GetPixel(0, 0)
            ' Threshold: we consider it visible if the alpha is high enough or the color is not pure black.
            Return px.A > 8 OrElse Not (px.R = 0 AndAlso px.G = 0 AndAlso px.B = 0)
        Catch
            Return True

        Finally
            bmp?.Dispose()
            If hdcSrc <> IntPtr.Zero Then
                NativeMethods.ReleaseDC(hWnd, hdcSrc)
            End If

        End Try
    End Function

    Private Shared Function PixelHitTest_ScreenSample(ptScreen As Point) As Boolean

        Try
            Using bmp As New Bitmap(1, 1, PixelFormat.Format32bppArgb)
                Using g As Graphics = Graphics.FromImage(bmp)
                    g.CopyFromScreen(ptScreen, Point.Empty, New Size(1, 1))
                End Using
                Dim c As Color = bmp.GetPixel(0, 0)
                Return Not (c.A = 0 Or (c.R = 0 And c.G = 0 And c.B = 0))
            End Using
        Catch
            Return True
        End Try
    End Function

    Private Shared Function IsWindowCloaked(hWnd As IntPtr) As Boolean

        Dim val As Integer = 0
        Dim hr As Integer = NativeMethods.DwmGetWindowAttribute(hWnd, Constants.DWMWA_CLOAKED, val, Marshal.SizeOf(GetType(Integer)))
        Return hr = 0 AndAlso val <> 0
    End Function

    Private Shared Function EnumTopLevelWindowsInZOrder() As List(Of IntPtr)

        Dim list As New List(Of IntPtr)()
        Dim cb As EnumWindowsProc =
            Function(hWnd, lParam)
                list.Add(hWnd)
                Return True ' continue
            End Function

        NativeMethods.EnumWindows(cb, IntPtr.Zero)
        Return list
    End Function

    Private Shared Function PointInRect(r As NativeRectangle, p As Point) As Boolean
        Return (p.X >= r.Left AndAlso p.X < r.Right AndAlso p.Y >= r.Top AndAlso p.Y < r.Bottom)
    End Function

#End Region

End Class
