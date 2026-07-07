#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices
Imports System.Security
Imports System.Text

#End Region

Namespace Win32

    <SuppressUnmanagedCodeSecurity>
    Friend Module NativeMethods

#Region " dwmapi "

        <DllImport("dwmapi.dll", PreserveSig:=True)>
        Friend Function DwmGetWindowAttribute(hWnd As IntPtr,
                                              dwAttribute As Integer,
                                              ByRef refPvAttribute As Integer,
                                              cbAttribute As Integer
        ) As Integer
        End Function

#End Region

#Region " gdi32 "

        <DllImport("gdi32.dll", SetLastError:=True)>
        Friend Function BitBlt(hdcDest As IntPtr,
                               nXDest As Integer,
                               nYDest As Integer,
                               nWidth As Integer,
                               nHeight As Integer,
                               hdcSrc As IntPtr,
                               nXSrc As Integer,
                               nYSrc As Integer,
                               dwRop As Integer
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("gdi32.dll")>
        Friend Function GetPixel(hDC As IntPtr,
                                 x As Integer,
                                 y As Integer
        ) As UInteger
        End Function

#End Region

#Region " kernel32 "

        <DllImport("kernel32.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi)>
        Friend Function IsWow64Process(hProcess As IntPtr,
                                 ByRef refWow64Process As Boolean
        ) As Boolean
        End Function

#End Region

#Region " user32 "

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function EnumWindows(enumFunc As EnumWindowsProc, lParam As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function IsWindowVisible(hWnd As IntPtr) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function GetWindow(hWnd As IntPtr, uCmd As UInteger) As IntPtr
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function GetWindowThreadProcessId(hWnd As IntPtr,
                                           ByRef refPid As Integer
        ) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function GetWindowRect(hWnd As IntPtr,
                                ByRef refRect As NativeRectangle
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function GetClientRect(hWnd As IntPtr,
                                ByRef refRect As NativeRectangle
        ) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function ClientToScreen(hWnd As IntPtr,
                                 ByRef refPoint As Point
        ) As Boolean
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto, BestFitMapping:=False, ThrowOnUnmappableChar:=True)>
        Friend Function GetWindowText(hWnd As IntPtr,
                                      buffer As StringBuilder,
                                      capacity As Integer
        ) As Integer
        End Function

        <DllImport("user32.dll")>
        Friend Function GetDC(hWnd As IntPtr) As IntPtr
        End Function

        <DllImport("user32.dll")>
        Friend Function ReleaseDC(hWnd As IntPtr,
                                  hDC As IntPtr
        ) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto, BestFitMapping:=False, ThrowOnUnmappableChar:=True)>
        Friend Function GetClassName(hWnd As IntPtr,
                                     className As StringBuilder,
                                     classNameMax As Integer
        ) As Integer
        End Function

        <DllImport("user32.dll", SetLastError:=True, CharSet:=CharSet.Auto, BestFitMapping:=False, ThrowOnUnmappableChar:=True)>
        Friend Function RealGetWindowClass(hwnd As IntPtr,
                                           className As StringBuilder,
                                           classNameMax As Integer
            ) As UInteger
        End Function

        <DllImport("user32.dll", EntryPoint:="GetWindowLong")>
        Friend Function GetWindowLong(hWnd As IntPtr, nIndex As Integer) As Integer
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWindowLong")>
        Friend Function SetWindowLong(hWnd As IntPtr, nIndex As Integer, dwNewLong As Integer) As Integer
        End Function

        <DllImport("user32.dll", EntryPoint:="GetWindowLongPtr")>
        Friend Function GetWindowLongPtr(hWnd As IntPtr, nIndex As Integer) As IntPtr
        End Function

        <DllImport("user32.dll", EntryPoint:="SetWindowLongPtr")>
        Friend Function SetWindowLongPtr(hWnd As IntPtr, nIndex As Integer, dwNewLong As IntPtr) As IntPtr
        End Function

#End Region

    End Module

End Namespace