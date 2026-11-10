#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging

Imports Win32

#End Region

''' <summary>
''' A borderless, top-most window that follows the mouse cursor and shows a zoomed view of the screen around it,
''' highlighting the pixel under the mouse cursor.
''' <para></para>
''' The window never takes the focus, it is transparent to the mouse, it does not appear in the taskbar nor in Alt+Tab,
''' and it is always placed next to (never over) the captured screen area.
''' </summary>
Friend NotInheritable Class MagnifierWindow : Inherits FormUInteger ' NOSONAR

#Region " Constants "

    ''' <summary>
    ''' The logical size (at 96 DPI) of the magnifier window.
    ''' </summary>
    Friend Shared ReadOnly LogicalWindowSize As New Size(220, 220)

    ''' <summary>
    ''' The logical distance (at 96 DPI) between the captured screen area and the magnifier window.
    ''' </summary>
    Private Const LogicalGap As Integer = 16

    ''' <summary>
    ''' The refresh interval of the zoomed view, in milliseconds (about 30 frames per second).
    ''' </summary>
    Private Const FrameInterval As Integer = 33

#End Region

#Region " Private Fields "

    ''' <summary>
    ''' The timer that refreshes the zoomed view.
    ''' </summary>
    Private ReadOnly frameTimer As New Timer() With {.Interval = FrameInterval}

    ''' <summary>
    ''' The last captured screen area, or <see langword="Nothing"/> if nothing has been captured yet.
    ''' </summary>
    Private capturedFrame As Bitmap

    ''' <summary>
    ''' The current zoom factor.
    ''' </summary>
    Private zoomValue As Integer = MagnifierGeometry.DefaultZoom

    ''' <summary>
    ''' The current color palette.
    ''' </summary>
    Private paletteValue As ThemePalette = ThemePalette.Light

    ''' <summary>
    ''' The hint lines drawn at the bottom of the zoomed view.
    ''' </summary>
    Private hintLinesValue As String() = Array.Empty(Of String)()

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets or sets the zoom factor. The value is clamped to the range
    ''' <see cref="MagnifierGeometry.MinZoom"/> to <see cref="MagnifierGeometry.MaxZoom"/>.
    ''' </summary>
    Friend Property Zoom As Integer
        Get
            Return Me.zoomValue
        End Get
        Set(value As Integer)
            Dim clamped As Integer = MagnifierGeometry.ClampZoom(value)
            If clamped <> Me.zoomValue Then
                Me.zoomValue = clamped
                Me.RefreshFrame()
            End If
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the color palette used to draw the border and the overlay texts.
    ''' </summary>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when the value is <see langword="Nothing"/>.
    ''' </exception>
    Friend Property Palette As ThemePalette
        Get
            Return Me.paletteValue
        End Get
        Set(value As ThemePalette)
            If value Is Nothing Then
                Throw New ArgumentNullException(NameOf(value))
            End If
            Me.paletteValue = value
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets or sets the hint lines drawn at the bottom of the zoomed view (e.g. the zoom keyboard shortcuts).
    ''' </summary>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when the value is <see langword="Nothing"/>.
    ''' </exception>
    Friend Property HintLines As String()
        Get
            Return Me.hintLinesValue
        End Get
        Set(value As String())
            If value Is Nothing Then
                Throw New ArgumentNullException(NameOf(value))
            End If
            Me.hintLinesValue = value
            Me.Invalidate()
        End Set
    End Property

    ''' <summary>
    ''' Gets the color of the pixel under the mouse cursor in the last captured frame,
    ''' or <see cref="Color.Empty"/> if nothing has been captured yet.
    ''' </summary>
    Friend ReadOnly Property CenterPixelColor As Color
        Get
            If Me.capturedFrame Is Nothing Then
                Return Color.Empty
            End If
            Return Me.capturedFrame.GetPixel(Me.capturedFrame.Width \ 2, Me.capturedFrame.Height \ 2)
        End Get
    End Property

    ''' <summary>
    ''' Gets the screen area captured in the last frame, or <see cref="Rectangle.Empty"/> if nothing has been captured yet.
    ''' </summary>
    Friend ReadOnly Property CaptureRectangle As Rectangle

    ''' <summary>
    ''' Gets a value indicating whether the window is shown without being activated.
    ''' </summary>
    Protected Overrides ReadOnly Property ShowWithoutActivation As Boolean
        Get
            Return True
        End Get
    End Property

    ''' <summary>
    ''' Gets the parameters to create the window handle: a top-most, non-activable, mouse-transparent layered tool window.
    ''' </summary>
    Protected Overrides ReadOnly Property CreateParams As CreateParams
        Get
            Dim cp As CreateParams = MyBase.CreateParams
            cp.ExStyle = cp.ExStyle Or
                         Constants.WS_EX_TOOLWINDOW Or
                         Constants.WS_EX_TOPMOST Or
                         Constants.WS_EX_NOACTIVATE Or
                         Constants.WS_EX_LAYERED Or
                         Constants.WS_EX_TRANSPARENT
            Return cp
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="MagnifierWindow"/> class.
    ''' </summary>
    Friend Sub New()

        Me.FormBorderStyle = FormBorderStyle.None
        Me.StartPosition = FormStartPosition.Manual
        Me.ShowInTaskbar = False
        Me.TopMost = True
        Me.DoubleBuffered = True
        Me.Text = "Mouse Point Viewer - Magnifier"
        Me.Font = New Font("Segoe UI", 8.25!, FontStyle.Regular, GraphicsUnit.Point)
        Me.Size = Me.LogicalToDeviceUnits(LogicalWindowSize)

        AddHandler Me.frameTimer.Tick, AddressOf Me.FrameTimer_Tick
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Increases the zoom factor by one step.
    ''' </summary>
    Friend Sub ZoomIn()

        Me.Zoom += 1
    End Sub

    ''' <summary>
    ''' Decreases the zoom factor by one step.
    ''' </summary>
    Friend Sub ZoomOut()

        Me.Zoom -= 1
    End Sub

    ''' <summary>
    ''' Captures the screen area around the mouse cursor, moves the window next to it, and redraws the zoomed view.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Sub RefreshFrame()

        Me.RefreshFrame(Control.MousePosition)
    End Sub

    ''' <summary>
    ''' Captures the screen area around the specified point, moves the window next to it, and redraws the zoomed view.
    ''' </summary>
    '''
    ''' <param name="center">
    ''' The center point, in screen coordinates.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub RefreshFrame(center As Point)

        Dim viewSize As Size = Me.ClientSize
        If viewSize.Width <= 0 OrElse viewSize.Height <= 0 Then
            Return ' Minimized or not created yet: there is nothing to draw.
        End If

        Dim rect As Rectangle = MagnifierGeometry.GetCaptureRectangle(center, viewSize, Me.zoomValue)
        Me._CaptureRectangle = rect

        Dim screenBounds As Rectangle = Screen.FromPoint(center).Bounds
        Dim bounds As Rectangle = MagnifierGeometry.GetWindowBounds(rect, Me.Size, screenBounds, Me.LogicalToDeviceUnits(LogicalGap))
        If bounds <> Me.Bounds Then
            Me.Bounds = bounds
        End If

        Dim bmp As New Bitmap(rect.Width, rect.Height, PixelFormat.Format32bppArgb)
        Try
            Using g As Graphics = Graphics.FromImage(bmp)
                g.CopyFromScreen(rect.Location, Point.Empty, rect.Size, CopyPixelOperation.SourceCopy)
            End Using

        Catch ex As ComponentModel.Win32Exception
            ' The screen cannot be captured while the secure desktop is shown (e.g. UAC prompt, lock screen):
            ' keep the last captured frame.
            bmp.Dispose()
            Return

        End Try

        Me.capturedFrame?.Dispose()
        Me.capturedFrame = bmp
        Me.Invalidate()
    End Sub

#End Region

#Region " Event Handlers "

    ''' <summary>
    ''' Handles the <see cref="Timer.Tick"/> event of the <see cref="MagnifierWindow.frameTimer"/>.
    ''' </summary>
    '''
    ''' <param name="sender">The source of the event.</param>
    ''' <param name="e">The event data.</param>
    Private Sub FrameTimer_Tick(sender As Object, e As EventArgs)

        Me.RefreshFrame()
    End Sub

#End Region

#Region " Protected Methods "

    ''' <summary>
    ''' Raises the <see cref="Control.HandleCreated"/> event, making the layered window fully opaque.
    ''' </summary>
    '''
    ''' <param name="e">The event data.</param>
    Protected Overrides Sub OnHandleCreated(e As EventArgs)

        MyBase.OnHandleCreated(e)
        NativeMethods.SetLayeredWindowAttributes(Me.Handle, 0UI, 255, Constants.LWA_ALPHA)
    End Sub

    ''' <summary>
    ''' Raises the <see cref="Control.VisibleChanged"/> event, starting or stopping the refresh of the zoomed view.
    ''' </summary>
    '''
    ''' <param name="e">The event data.</param>
    Protected Overrides Sub OnVisibleChanged(e As EventArgs)

        MyBase.OnVisibleChanged(e)

        If Me.Visible Then
            Me.RefreshFrame()
            Me.frameTimer.Start()
        Else
            Me.frameTimer.Stop()
        End If
    End Sub

    ''' <summary>
    ''' Paints the zoomed view, the pixel grid, the center pixel highlight, the border and the overlay texts.
    ''' </summary>
    '''
    ''' <param name="e">The event data.</param>
    Protected Overrides Sub OnPaint(e As PaintEventArgs)

        Dim g As Graphics = e.Graphics
        Dim palette As ThemePalette = Me.paletteValue
        Dim viewSize As Size = Me.ClientSize
        g.Clear(palette.WindowBack)

        If Me.capturedFrame IsNot Nothing Then
            Dim zoom As Integer = Me.zoomValue
            Dim dest As Rectangle = MagnifierGeometry.GetDestinationRectangle(viewSize, zoom)

            ' Zoomed screen area (sharp pixels).
            g.InterpolationMode = InterpolationMode.NearestNeighbor
            g.PixelOffsetMode = PixelOffsetMode.Half
            g.DrawImage(Me.capturedFrame, dest)
            g.PixelOffsetMode = PixelOffsetMode.Default

            ' Pixel grid.
            If zoom >= MagnifierGeometry.PixelGridMinZoom Then
                Using gridPen As New Pen(Color.FromArgb(48, 128, 128, 128))
                    For x As Integer = dest.Left To dest.Right Step zoom
                        g.DrawLine(gridPen, x, 0, x, viewSize.Height)
                    Next
                    For y As Integer = dest.Top To dest.Bottom Step zoom
                        g.DrawLine(gridPen, 0, y, viewSize.Width, y)
                    Next
                End Using
            End If

            ' Center pixel highlight (black and white, visible over any color).
            Dim cell As Rectangle = MagnifierGeometry.GetCenterPixelRectangle(viewSize, zoom)
            Dim outer As Rectangle = Rectangle.Inflate(cell, 1, 1)
            g.DrawRectangle(Pens.Black, outer.X, outer.Y, outer.Width - 1, outer.Height - 1)
            g.DrawRectangle(Pens.White, cell.X, cell.Y, cell.Width - 1, cell.Height - 1)

            ' Overlay texts.
            Dim c As Color = Me.CenterPixelColor
            Dim lines As New List(Of String)(Me.hintLinesValue.Length + 1) From {
                $"{ColorTranslator.ToHtml(Color.FromArgb(c.R, c.G, c.B))}    {zoom}x"
            }
            lines.AddRange(Me.hintLinesValue)
            Me.DrawOverlay(g, viewSize, palette, lines)
        End If

        ' Border.
        Using borderPen As New Pen(palette.Border)
            g.DrawRectangle(borderPen, 0, 0, viewSize.Width - 1, viewSize.Height - 1)
        End Using
    End Sub

    ''' <summary>
    ''' Releases the resources used by the window.
    ''' </summary>
    '''
    ''' <param name="disposing">
    ''' <see langword="True"/> to release both managed and unmanaged resources.
    ''' </param>
    Protected Overrides Sub Dispose(disposing As Boolean)

        If disposing Then
            Me.frameTimer.Dispose()
            Me.capturedFrame?.Dispose()
            Me.capturedFrame = Nothing
        End If
        MyBase.Dispose(disposing)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Draws the overlay text lines over a semi-transparent strip at the bottom of the view.
    ''' </summary>
    '''
    ''' <param name="g">The graphics surface.</param>
    ''' <param name="viewSize">The size of the view.</param>
    ''' <param name="palette">The color palette.</param>
    ''' <param name="lines">The text lines.</param>
    Private Sub DrawOverlay(g As Graphics, viewSize As Size, palette As ThemePalette, lines As List(Of String))

        Dim lineHeight As Integer = TextRenderer.MeasureText(g, "Ag", Me.Font).Height
        Dim padding As Integer = Me.LogicalToDeviceUnits(4)
        Dim stripHeight As Integer = (lineHeight * lines.Count) + (padding * 2)
        Dim strip As New Rectangle(0, viewSize.Height - stripHeight, viewSize.Width, stripHeight)

        Using stripBrush As New SolidBrush(Color.FromArgb(200, palette.WindowBack))
            g.FillRectangle(stripBrush, strip)
        End Using

        For i As Integer = 0 To lines.Count - 1
            Dim lineRect As New Rectangle(padding, strip.Top + padding + (i * lineHeight), viewSize.Width - (padding * 2), lineHeight)
            Dim color As Color = If(i = 0, palette.WindowFore, palette.SecondaryFore)
            TextRenderer.DrawText(g, lines(i), Me.Font, lineRect, color,
                                  TextFormatFlags.HorizontalCenter Or TextFormatFlags.VerticalCenter Or
                                  TextFormatFlags.SingleLine Or TextFormatFlags.EndEllipsis Or TextFormatFlags.NoPadding)
        Next
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'MagnifierWindow
        '
        Me.ClientSize = New System.Drawing.Size(284, 261)
        Me.Name = "MagnifierWindow"
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
