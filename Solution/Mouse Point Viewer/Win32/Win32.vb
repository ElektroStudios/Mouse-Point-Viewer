#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices

#End Region

Namespace Win32

    Public Delegate Function EnumWindowsProc(hWnd As IntPtr, lParam As IntPtr) As Boolean

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure NativeRectangle

        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer

    End Structure

End Namespace
