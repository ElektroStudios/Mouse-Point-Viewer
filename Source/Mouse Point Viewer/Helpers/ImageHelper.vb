#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

#End Region

''' <summary>
''' Provides helper methods for images and icon glyphs.
''' </summary>
Friend Module ImageHelper

#Region " Constants "

    ''' <summary>
    ''' The name of the icon font of Windows 10 and later ("Segoe MDL2 Assets").
    ''' </summary>
    Friend Const IconFontName As String = "Segoe MDL2 Assets"

    ''' <summary>
    ''' "Zoom" glyph of the <see cref="IconFontName"/> font.
    ''' </summary>
    Friend Const GlyphZoom As String = ChrW(&HE71E)

    ''' <summary>
    ''' "Brightness" (sun) glyph of the <see cref="IconFontName"/> font.
    ''' </summary>
    Friend Const GlyphSun As String = ChrW(&HE706)

    ''' <summary>
    ''' "QuietHours" (moon) glyph of the <see cref="IconFontName"/> font.
    ''' </summary>
    Friend Const GlyphMoon As String = ChrW(&HE708)

    ''' <summary>
    ''' "FullScreen" (expand) glyph of the <see cref="IconFontName"/> font.
    ''' </summary>
    Friend Const GlyphExpand As String = ChrW(&HE740)

    ''' <summary>
    ''' "BackToWindow" (collapse) glyph of the <see cref="IconFontName"/> font.
    ''' </summary>
    Friend Const GlyphCollapse As String = ChrW(&HE73F)

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Returns a high quality resized copy of the specified image.
    ''' </summary>
    '''
    ''' <param name="source">
    ''' The source image.
    ''' </param>
    '''
    ''' <param name="size">
    ''' The size of the resulting image.
    ''' </param>
    '''
    ''' <returns>
    ''' The resized image. The caller is responsible for disposing it.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="source"/> is <see langword="Nothing"/>.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentOutOfRangeException">
    ''' Thrown when <paramref name="size"/> is not positive.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Function ResizeImage(source As Image, size As Size) As Bitmap

        If source Is Nothing Then
            Throw New ArgumentNullException(NameOf(source))
        End If
        If size.Width <= 0 OrElse size.Height <= 0 Then
            Throw New ArgumentOutOfRangeException(NameOf(size), size, "The size must be positive.")
        End If

        Dim result As New Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb)
        Using g As Graphics = Graphics.FromImage(result)
            g.CompositingQuality = CompositingQuality.HighQuality
            g.InterpolationMode = InterpolationMode.HighQualityBicubic
            g.PixelOffsetMode = PixelOffsetMode.HighQuality
            g.SmoothingMode = SmoothingMode.HighQuality

            ' Keep the aspect ratio, centering the image.
            Dim scale As Double = Math.Min(size.Width / source.Width, size.Height / source.Height)
            Dim w As Integer = Math.Max(1, CInt(Math.Round(source.Width * scale)))
            Dim h As Integer = Math.Max(1, CInt(Math.Round(source.Height * scale)))
            Using attributes As New ImageAttributes()
                attributes.SetWrapMode(WrapMode.TileFlipXY) ' Avoids semi-transparent edges.
                g.DrawImage(source, New Rectangle((size.Width - w) \ 2, (size.Height - h) \ 2, w, h),
                            0, 0, source.Width, source.Height, GraphicsUnit.Pixel, attributes)
            End Using
        End Using
        Return result
    End Function

    ''' <summary>
    ''' Determines whether the specified font family is installed.
    ''' </summary>
    '''
    ''' <param name="familyName">
    ''' The font family name.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the font family is installed, <see langword="False"/> otherwise.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function IsFontInstalled(familyName As String) As Boolean

        For Each family As FontFamily In FontFamily.Families
            If String.Equals(family.Name, familyName, StringComparison.OrdinalIgnoreCase) Then
                Return True
            End If
        Next
        Return False
    End Function

#End Region

End Module
