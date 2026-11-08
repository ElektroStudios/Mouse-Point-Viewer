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

    End Module

End Namespace
