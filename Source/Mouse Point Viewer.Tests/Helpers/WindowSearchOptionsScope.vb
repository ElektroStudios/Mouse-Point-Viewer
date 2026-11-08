#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " WindowSearchOptionsScope "

Namespace MousePointViewer.Tests.Helpers

    ''' <summary>
    ''' Sets the window search options of the <see cref="Main"/> form
    ''' (<see cref="Main.ignoreChildWindows"/>, <see cref="Main.ignoreLayeredWindows"/>
    ''' and <see cref="Main.ignoreTransparentWindows"/>) for the lifetime of this object,
    ''' and restores their previous values when it is disposed.
    ''' </summary>
    Friend NotInheritable Class WindowSearchOptionsScope : Implements IDisposable

#Region " Private Fields "

        ''' <summary>
        ''' The previous value of <see cref="Main.ignoreChildWindows"/>.
        ''' </summary>
        Private ReadOnly previousIgnoreChildWindows As Boolean

        ''' <summary>
        ''' The previous value of <see cref="Main.ignoreLayeredWindows"/>.
        ''' </summary>
        Private ReadOnly previousIgnoreLayeredWindows As Boolean

        ''' <summary>
        ''' The previous value of <see cref="Main.ignoreTransparentWindows"/>.
        ''' </summary>
        Private ReadOnly previousIgnoreTransparentWindows As Boolean

        ''' <summary>
        ''' Flag to detect redundant calls to <see cref="WindowSearchOptionsScope.Dispose"/>.
        ''' </summary>
        Private isDisposed As Boolean

#End Region

#Region " Constructors "

        ''' <summary>
        ''' Initializes a new instance of the <see cref="WindowSearchOptionsScope"/> class,
        ''' applying the specified window search options.
        ''' </summary>
        '''
        ''' <param name="ignoreChildWindows">
        ''' The value for <see cref="Main.ignoreChildWindows"/>.
        ''' </param>
        '''
        ''' <param name="ignoreLayeredWindows">
        ''' The value for <see cref="Main.ignoreLayeredWindows"/>.
        ''' </param>
        '''
        ''' <param name="ignoreTransparentWindows">
        ''' The value for <see cref="Main.ignoreTransparentWindows"/>.
        ''' </param>
        Friend Sub New(ignoreChildWindows As Boolean, ignoreLayeredWindows As Boolean, ignoreTransparentWindows As Boolean)

            Me.previousIgnoreChildWindows = Main.ignoreChildWindows
            Me.previousIgnoreLayeredWindows = Main.ignoreLayeredWindows
            Me.previousIgnoreTransparentWindows = Main.ignoreTransparentWindows

            Main.ignoreChildWindows = ignoreChildWindows
            Main.ignoreLayeredWindows = ignoreLayeredWindows
            Main.ignoreTransparentWindows = ignoreTransparentWindows
        End Sub

#End Region

#Region " IDisposable Implementation "

        ''' <summary>
        ''' Restores the previous window search options.
        ''' </summary>
        Public Sub Dispose() Implements IDisposable.Dispose

            If Me.isDisposed Then
                Return
            End If

            Main.ignoreChildWindows = Me.previousIgnoreChildWindows
            Main.ignoreLayeredWindows = Me.previousIgnoreLayeredWindows
            Main.ignoreTransparentWindows = Me.previousIgnoreTransparentWindows

            Me.isDisposed = True
        End Sub

#End Region

    End Class

End Namespace

#End Region
