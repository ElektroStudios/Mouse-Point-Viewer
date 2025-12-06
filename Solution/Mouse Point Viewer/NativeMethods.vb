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

<SuppressUnmanagedCodeSecurity>
Friend Module NativeMethods

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure Rect
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Function GetWindowRect(hWnd As IntPtr,
                            ByRef refRect As Rect
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Function GetWindowText(hWnd As IntPtr,
                                  buffer As StringBuilder,
                                  capacity As Integer
    ) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Function WindowFromPoint(pt As Point
    ) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Friend Function GetWindowThreadProcessId(hWnd As IntPtr,
                                       ByRef refPid As Integer
    ) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=False)>
    Friend Function GetParent(hWnd As IntPtr
    ) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Friend Function GetDC(hWnd As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")>
    Friend Function ReleaseDC(hWnd As IntPtr,
                              hDC As IntPtr
    ) As Integer
    End Function

    <DllImport("gdi32.dll")>
    Friend Function GetPixel(hDC As IntPtr,
                             x As Integer,
                             y As Integer
    ) As UInteger
    End Function

End Module
