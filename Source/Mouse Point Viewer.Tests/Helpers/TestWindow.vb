#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Drawing
Imports System.Threading
Imports System.Windows.Forms

Imports Win32

#End Region

#Region " TestWindow "

Namespace MousePointViewer.Tests.Helpers

    ''' <summary>
    ''' A real, borderless, top-most window shown on the desktop for integration tests.
    ''' <para></para>
    ''' The window is closed and disposed when this object is disposed.
    ''' </summary>
    Friend NotInheritable Class TestWindow : Implements IDisposable

#Region " Constants "

        ''' <summary>
        ''' The default size of a test window.
        ''' </summary>
        Friend Shared ReadOnly DefaultSize As New Size(240, 180)

        ''' <summary>
        ''' The maximum time to wait for a condition in <see cref="TestWindow.WaitUntil"/>.
        ''' </summary>
        Private Shared ReadOnly WaitTimeout As TimeSpan = TimeSpan.FromSeconds(3)

#End Region

#Region " Properties "

        ''' <summary>
        ''' Gets the underlying <see cref="System.Windows.Forms.Form"/>.
        ''' </summary>
        Friend ReadOnly Property Form As Form

        ''' <summary>
        ''' Gets the handle of the underlying <see cref="System.Windows.Forms.Form"/>.
        ''' </summary>
        Friend ReadOnly Property Handle As IntPtr
            Get
                Return Me.Form.Handle
            End Get
        End Property

        ''' <summary>
        ''' Gets the bounds of the window, in screen coordinates.
        ''' </summary>
        Friend ReadOnly Property Bounds As Rectangle
            Get
                Return Me.Form.Bounds
            End Get
        End Property

        ''' <summary>
        ''' Gets the center point of the window, in screen coordinates.
        ''' </summary>
        Friend ReadOnly Property Center As Point
            Get
                Dim b As Rectangle = Me.Form.Bounds
                Return New Point(b.Left + (b.Width \ 2), b.Top + (b.Height \ 2))
            End Get
        End Property

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="TestWindow"/> class, and shows the window.
        ''' </summary>
        '''
        ''' <param name="bounds">
        ''' The bounds of the window, in screen coordinates.
        ''' </param>
        '''
        ''' <param name="backColor">
        ''' The background color of the window.
        ''' </param>
        '''
        ''' <param name="caption">
        ''' The window caption.
        ''' </param>
        Friend Sub New(bounds As Rectangle, backColor As Color, Optional caption As String = "Mouse Point Viewer Test Window")

            Me.Form = New Form() With {
                .FormBorderStyle = FormBorderStyle.None,
                .StartPosition = FormStartPosition.Manual,
                .ShowInTaskbar = False,
                .TopMost = True,
                .BackColor = backColor,
                .Text = caption
            }
            Me.Form.Bounds = bounds
        End Sub

#End Region

#Region " Public Methods "

        ''' <summary>
        ''' Returns the bounds of a test window centered in the working area of the primary screen.
        ''' </summary>
        '''
        ''' <returns>
        ''' The bounds, in screen coordinates.
        ''' </returns>
        Friend Shared Function GetCenteredBounds() As Rectangle

            Dim area As Rectangle = Screen.PrimaryScreen.WorkingArea
            Return New Rectangle(area.Left + ((area.Width - DefaultSize.Width) \ 2),
                                 area.Top + ((area.Height - DefaultSize.Height) \ 2),
                                 DefaultSize.Width, DefaultSize.Height)
        End Function

        ''' <summary>
        ''' Shows the window, forces it to paint, and pumps the pending window messages.
        ''' </summary>
        '''
        ''' <returns>
        ''' This same instance.
        ''' </returns>
        Friend Function ShowAndPaint() As TestWindow

            Me.Form.Show()
            Me.Form.Refresh()
            TestWindow.PumpMessages()
            Return Me
        End Function

        ''' <summary>
        ''' Adds the specified extended window styles to the window.
        ''' </summary>
        '''
        ''' <param name="exStyles">
        ''' The extended window styles to add (e.g. <see cref="Constants.WS_EX_TRANSPARENT"/>).
        ''' </param>
        Friend Sub AddExtendedStyles(exStyles As Integer)

            Dim current As IntPtr = WindowHelper.GetWindowLongPtrSafe(Me.Handle, Constants.GWL_EXSTYLE)
            Dim newStyle As New IntPtr(current.ToInt64() Or CLng(exStyles))
            WindowHelper.SetWindowLongPtrSafe(Me.Handle, Constants.GWL_EXSTYLE, newStyle)
            TestWindow.PumpMessages()
        End Sub

        ''' <summary>
        ''' Processes all the window messages currently in the message queue.
        ''' </summary>
        Friend Shared Sub PumpMessages()

            For i As Integer = 1 To 5
                Application.DoEvents()
                Thread.Sleep(10)
            Next
        End Sub

        ''' <summary>
        ''' Repeatedly pumps the window messages until the specified condition is met or a timeout elapses.
        ''' <para></para>
        ''' This is needed for conditions that depend on the desktop composition (DWM),
        ''' which updates the screen asynchronously.
        ''' </summary>
        '''
        ''' <param name="condition">
        ''' The condition to wait for.
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the condition was met, <see langword="False"/> if the timeout elapsed.
        ''' </returns>
        '''
        ''' <exception cref="ArgumentNullException">
        ''' Thrown when <paramref name="condition"/> is <see langword="Nothing"/>.
        ''' </exception>
        Friend Shared Function WaitUntil(condition As Func(Of Boolean)) As Boolean

            If condition Is Nothing Then
                Throw New ArgumentNullException(NameOf(condition))
            End If

            Dim sw As Stopwatch = Stopwatch.StartNew()
            Do
                If condition() Then
                    Return True
                End If
                TestWindow.PumpMessages()
            Loop While sw.Elapsed < WaitTimeout

            Return condition()
        End Function

#End Region

#Region " IDisposable Implementation "

        ''' <summary>
        ''' Closes and disposes the window.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If Not Me.Form.IsDisposed Then
                Me.Form.Close()
                Me.Form.Dispose()
                TestWindow.PumpMessages()
            End If
        End Sub

#End Region

    End Class

End Namespace

#End Region
