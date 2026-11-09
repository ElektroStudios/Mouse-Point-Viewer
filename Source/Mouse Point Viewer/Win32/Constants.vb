#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

Namespace Win32

    ''' <summary>
    ''' Provides the Win32 constants used by the application.
    ''' </summary>
    Friend Module Constants

        ''' <summary>
        ''' Index used with <c>GetWindowLong</c>/<c>GetWindowLongPtr</c> to retrieve the extended window styles.
        ''' </summary>
        Friend Const GWL_EXSTYLE As Integer = -20

        ''' <summary>
        ''' Extended window style: the window is a layered window.
        ''' </summary>
        Friend Const WS_EX_LAYERED As Integer = &H80000

        ''' <summary>
        ''' Extended window style: the window is transparent to mouse hit-testing.
        ''' </summary>
        Friend Const WS_EX_TRANSPARENT As Integer = &H20

        ''' <summary>
        ''' <c>GetWindow</c> command: retrieves the window below the specified window in the Z-order.
        ''' </summary>
        Friend Const GW_HWNDNEXT As UInteger = 2

        ''' <summary>
        ''' <c>GetWindow</c> command: retrieves the child window at the top of the Z-order.
        ''' </summary>
        Friend Const GW_CHILD As UInteger = 5

        ''' <summary>
        ''' <c>DwmGetWindowAttribute</c> attribute: retrieves the cloaked state of the window.
        ''' </summary>
        Friend Const DWMWA_CLOAKED As Integer = 14

        ''' <summary>
        ''' Raster-operation code that copies the source rectangle directly to the destination rectangle.
        ''' </summary>
        Friend Const SRCCOPY As Integer = &HCC0020

        ''' <summary>
        ''' Process access right required to retrieve certain limited information about a process
        ''' (e.g. <c>IsWow64Process</c>), even for elevated or protected processes.
        ''' </summary>
        Friend Const PROCESS_QUERY_LIMITED_INFORMATION As Integer = &H1000

        ''' <summary>
        ''' Extended window style: the window is a tool window (it does not appear in the taskbar nor in Alt+Tab).
        ''' </summary>
        Friend Const WS_EX_TOOLWINDOW As Integer = &H80

        ''' <summary>
        ''' Extended window style: the window is placed above all non-topmost windows.
        ''' </summary>
        Friend Const WS_EX_TOPMOST As Integer = &H8

        ''' <summary>
        ''' Extended window style: the window does not become the foreground window when the user clicks it.
        ''' </summary>
        Friend Const WS_EX_NOACTIVATE As Integer = &H8000000

        ''' <summary>
        ''' <c>SetLayeredWindowAttributes</c> flag: use the alpha value to determine the opacity of the layered window.
        ''' </summary>
        Friend Const LWA_ALPHA As UInteger = &H2UI

        ''' <summary>
        ''' Message posted when the user presses a hot key registered by <c>RegisterHotKey</c>.
        ''' </summary>
        Friend Const WM_HOTKEY As Integer = &H312

        ''' <summary>
        ''' Message sent to a window when its non-client area needs to be changed to indicate an active or inactive state.
        ''' </summary>
        Friend Const WM_NCACTIVATE As Integer = &H86

        ''' <summary>
        ''' Parent window handle used to create a message-only window.
        ''' </summary>
        Friend ReadOnly HWND_MESSAGE As New IntPtr(-3)

        ''' <summary>
        ''' <c>DwmSetWindowAttribute</c> attribute: allows the window frame to be drawn in dark mode colors
        ''' (Windows 10 version 20H1 and later, and Windows 11).
        ''' </summary>
        Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE As Integer = 20

        ''' <summary>
        ''' Undocumented value of the <see cref="DWMWA_USE_IMMERSIVE_DARK_MODE"/> attribute
        ''' in Windows 10 versions prior to 20H1.
        ''' </summary>
        Friend Const DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1 As Integer = 19

    End Module

End Namespace
