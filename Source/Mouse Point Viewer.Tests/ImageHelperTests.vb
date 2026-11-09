#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing

#End Region

#Region " ImageHelperTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Tests for the <see cref="ImageHelper"/> module and the embedded image resources.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ImageHelperTests

#Region " ResizeImage "

        ''' <summary>
        ''' Verifies that the resized image has the requested size and keeps the content.
        ''' </summary>
        <TestMethod>
        Public Sub ResizeImage_SquareSource_FillsRequestedSize()

            Using source As New Bitmap(100, 100),
                  g As Graphics = Graphics.FromImage(source)

                g.Clear(Color.Red)

                Using result As Bitmap = ImageHelper.ResizeImage(source, New Size(18, 18))
                    Assert.AreEqual(New Size(18, 18), result.Size)
                    Assert.AreEqual(Color.Red.ToArgb(), result.GetPixel(9, 9).ToArgb())
                    Assert.AreEqual(Color.Red.ToArgb(), result.GetPixel(0, 0).ToArgb(), "The edges are blended.")
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the aspect ratio is kept, centering the image and leaving transparent margins.
        ''' </summary>
        <TestMethod>
        Public Sub ResizeImage_WideSource_KeepsAspectRatio()

            Using source As New Bitmap(200, 100),
                  g As Graphics = Graphics.FromImage(source)

                g.Clear(Color.Blue)

                Using result As Bitmap = ImageHelper.ResizeImage(source, New Size(20, 20))
                    Assert.AreEqual(0, result.GetPixel(10, 1).A, "The top margin is not transparent.")
                    Assert.AreEqual(0, result.GetPixel(10, 18).A, "The bottom margin is not transparent.")
                    Assert.AreEqual(Color.Blue.ToArgb(), result.GetPixel(10, 10).ToArgb())
                End Using
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that invalid arguments throw.
        ''' </summary>
        <TestMethod>
        Public Sub ResizeImage_InvalidArguments_Throw()

            Using source As New Bitmap(10, 10)
                Assert.ThrowsExactly(Of ArgumentNullException)(Function() ImageHelper.ResizeImage(Nothing, New Size(5, 5)))
                Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() ImageHelper.ResizeImage(source, Size.Empty))
                Assert.ThrowsExactly(Of ArgumentOutOfRangeException)(Function() ImageHelper.ResizeImage(source, New Size(5, -1)))
            End Using
        End Sub

#End Region

#Region " Fonts and Glyphs "

        ''' <summary>
        ''' Verifies the detection of installed fonts.
        ''' </summary>
        <TestMethod>
        Public Sub IsFontInstalled_DetectsInstalledFonts()

            Assert.IsTrue(ImageHelper.IsFontInstalled("Segoe UI"))
            Assert.IsTrue(ImageHelper.IsFontInstalled("segoe ui"), "The comparison is case-sensitive.")
            Assert.IsFalse(ImageHelper.IsFontInstalled("No Such Font 1234567890"))
        End Sub

        ''' <summary>
        ''' Verifies that the icon glyphs are single characters of the Unicode private use area (as the icon font requires),
        ''' and that they are all different.
        ''' </summary>
        <TestMethod>
        Public Sub Glyphs_AreDistinctPrivateUseCharacters()

            Dim glyphs As String() = {ImageHelper.GlyphZoom, ImageHelper.GlyphSun, ImageHelper.GlyphMoon,
                                      ImageHelper.GlyphExpand, ImageHelper.GlyphCollapse}

            For Each glyph As String In glyphs
                Assert.AreEqual(1, glyph.Length)
                Assert.IsTrue(AscW(glyph(0)) >= &HE000 AndAlso AscW(glyph(0)) <= &HF8FF, $"U+{AscW(glyph(0)):X4} is not a private use character.")
            Next
            CollectionAssert.AllItemsAreUnique(glyphs)
        End Sub

#End Region

#Region " Resources "

        ''' <summary>
        ''' Verifies that the GitHub logos are embedded, have the same size, and are black and white respectively.
        ''' </summary>
        <TestMethod>
        Public Sub GitHubLogos_AreEmbedded()

            Using black As Bitmap = My.Resources.GitHub_Invertocat_Black,
                  white As Bitmap = My.Resources.GitHub_Invertocat_White

                Assert.IsNotNull(black)
                Assert.IsNotNull(white)
                Assert.AreEqual(black.Size, white.Size)

                Dim ring As New Point(black.Width \ 2, CInt(black.Height * 0.05)) ' A point on the outer ring of the logo.
                Assert.IsTrue(black.GetPixel(ring.X, ring.Y).GetBrightness() < 0.2, "The black logo is not black.")
                Assert.IsTrue(white.GetPixel(ring.X, ring.Y).GetBrightness() > 0.8, "The white logo is not white.")
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
