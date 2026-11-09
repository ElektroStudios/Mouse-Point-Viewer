#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

#End Region

''' <summary>
''' Provides the geometry calculations of the <see cref="MagnifierWindow"/>.
''' </summary>
Friend Module MagnifierGeometry

#Region " Constants "

    ''' <summary>
    ''' The minimum zoom factor.
    ''' </summary>
    Friend Const MinZoom As Integer = 2

    ''' <summary>
    ''' The maximum zoom factor.
    ''' </summary>
    Friend Const MaxZoom As Integer = 10

    ''' <summary>
    ''' The default zoom factor.
    ''' </summary>
    Friend Const DefaultZoom As Integer = 5

    ''' <summary>
    ''' The minimum zoom factor at which the pixel grid is drawn.
    ''' </summary>
    Friend Const PixelGridMinZoom As Integer = 6

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Clamps a zoom factor to the range <see cref="MinZoom"/> to <see cref="MaxZoom"/>.
    ''' </summary>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <returns>
    ''' The clamped zoom factor.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function ClampZoom(zoom As Integer) As Integer

        Return Math.Max(MinZoom, Math.Min(MaxZoom, zoom))
    End Function

    ''' <summary>
    ''' Returns the size, in screen pixels, of the screen area captured to fill the specified view at the specified zoom.
    ''' <para></para>
    ''' The width and height are always odd, so there is always a center pixel (the pixel under the mouse cursor).
    ''' </summary>
    '''
    ''' <param name="viewSize">
    ''' The size of the view (the client area of the magnifier).
    ''' </param>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <returns>
    ''' The size of the captured area.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when <paramref name="viewSize"/> is empty, or <paramref name="zoom"/> is out of range.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Function GetCaptureSize(viewSize As Size, zoom As Integer) As Size

        MagnifierGeometry.ValidateArguments(viewSize, zoom)

        Return New Size(MagnifierGeometry.ToOddCeiling(viewSize.Width, zoom),
                        MagnifierGeometry.ToOddCeiling(viewSize.Height, zoom))
    End Function

    ''' <summary>
    ''' Returns the screen area captured to fill the specified view at the specified zoom,
    ''' centered at the specified point (the pixel under the mouse cursor).
    ''' </summary>
    '''
    ''' <param name="center">
    ''' The center point, in screen coordinates.
    ''' </param>
    '''
    ''' <param name="viewSize">
    ''' The size of the view (the client area of the magnifier).
    ''' </param>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <returns>
    ''' The captured area, in screen coordinates.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function GetCaptureRectangle(center As Point, viewSize As Size, zoom As Integer) As Rectangle

        Dim size As Size = MagnifierGeometry.GetCaptureSize(viewSize, zoom)
        Return New Rectangle(center.X - (size.Width \ 2), center.Y - (size.Height \ 2), size.Width, size.Height)
    End Function

    ''' <summary>
    ''' Returns the rectangle of the view where the captured area is drawn (scaled by the zoom factor),
    ''' so that the center pixel is drawn at the center of the view.
    ''' <para></para>
    ''' The rectangle is equal to or larger than the view; the outer pixels may be partially visible.
    ''' </summary>
    '''
    ''' <param name="viewSize">
    ''' The size of the view.
    ''' </param>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <returns>
    ''' The destination rectangle, in view coordinates.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function GetDestinationRectangle(viewSize As Size, zoom As Integer) As Rectangle

        Dim captureSize As Size = MagnifierGeometry.GetCaptureSize(viewSize, zoom)
        Dim width As Integer = captureSize.Width * zoom
        Dim height As Integer = captureSize.Height * zoom
        Return New Rectangle((viewSize.Width - width) \ 2, (viewSize.Height - height) \ 2, width, height)
    End Function

    ''' <summary>
    ''' Returns the rectangle of the view where the center pixel (the pixel under the mouse cursor) is drawn.
    ''' </summary>
    '''
    ''' <param name="viewSize">
    ''' The size of the view.
    ''' </param>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <returns>
    ''' The rectangle of the center pixel, in view coordinates.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function GetCenterPixelRectangle(viewSize As Size, zoom As Integer) As Rectangle

        Dim captureSize As Size = MagnifierGeometry.GetCaptureSize(viewSize, zoom)
        Dim dest As Rectangle = MagnifierGeometry.GetDestinationRectangle(viewSize, zoom)
        Return New Rectangle(dest.X + ((captureSize.Width \ 2) * zoom),
                             dest.Y + ((captureSize.Height \ 2) * zoom),
                             zoom, zoom)
    End Function

    ''' <summary>
    ''' Returns the bounds of the magnifier window near the mouse cursor.
    ''' <para></para>
    ''' The window is placed diagonally (bottom-right by default) next to the captured area, so it never covers
    ''' the captured area nor the pixel under the mouse cursor. When there is no room on a side of the screen,
    ''' the window is placed on the opposite side.
    ''' </summary>
    '''
    ''' <param name="captureRectangle">
    ''' The captured area, in screen coordinates (see <see cref="GetCaptureRectangle"/>).
    ''' </param>
    '''
    ''' <param name="windowSize">
    ''' The size of the magnifier window.
    ''' </param>
    '''
    ''' <param name="screenBounds">
    ''' The bounds of the screen that contains the mouse cursor.
    ''' </param>
    '''
    ''' <param name="gap">
    ''' The distance between the captured area and the window.
    ''' </param>
    '''
    ''' <returns>
    ''' The bounds of the magnifier window, in screen coordinates.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function GetWindowBounds(captureRectangle As Rectangle, windowSize As Size, screenBounds As Rectangle, gap As Integer) As Rectangle

        Dim x As Integer = captureRectangle.Right + gap
        If x + windowSize.Width > screenBounds.Right Then
            x = captureRectangle.Left - gap - windowSize.Width
        End If

        Dim y As Integer = captureRectangle.Bottom + gap
        If y + windowSize.Height > screenBounds.Bottom Then
            y = captureRectangle.Top - gap - windowSize.Height
        End If

        ' Keep the window inside the screen.
        x = Math.Max(screenBounds.Left, Math.Min(x, screenBounds.Right - windowSize.Width))
        y = Math.Max(screenBounds.Top, Math.Min(y, screenBounds.Bottom - windowSize.Height))

        Return New Rectangle(New Point(x, y), windowSize)
    End Function

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Returns the smallest odd integer that is equal to or greater than <paramref name="length"/> / <paramref name="zoom"/>.
    ''' </summary>
    '''
    ''' <param name="length">
    ''' The length of the view.
    ''' </param>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <returns>
    ''' The odd ceiling.
    ''' </returns>
    Private Function ToOddCeiling(length As Integer, zoom As Integer) As Integer

        Dim value As Integer = (length + zoom - 1) \ zoom
        Return If(value Mod 2 = 0, value + 1, value)
    End Function

    ''' <summary>
    ''' Validates the view size and zoom arguments.
    ''' </summary>
    '''
    ''' <param name="viewSize">
    ''' The size of the view.
    ''' </param>
    '''
    ''' <param name="zoom">
    ''' The zoom factor.
    ''' </param>
    '''
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when <paramref name="viewSize"/> is empty, or <paramref name="zoom"/> is out of range.
    ''' </exception>
    Private Sub ValidateArguments(viewSize As Size, zoom As Integer)

        If viewSize.Width <= 0 OrElse viewSize.Height <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(viewSize), viewSize, "The view size must be positive.")
        End If

        If zoom < MinZoom OrElse zoom > MaxZoom Then
            Throw New ArgumentOutOfRangeException(NameOf(zoom), zoom, $"The zoom must be in the range {MinZoom} to {MaxZoom}.")
        End If
    End Sub

#End Region

End Module
