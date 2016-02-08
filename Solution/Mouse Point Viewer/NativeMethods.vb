Imports System.Security

Friend NotInheritable Class NativeMethods

    <StructLayout(LayoutKind.Sequential)>
    Friend Structure Rect
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    <SuppressUnmanagedCodeSecurity>
    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function GetWindowRect(
            ByVal hwnd As IntPtr,
            ByRef lpRect As Rect
    ) As <MarshalAs(UnmanagedType.Bool)> Boolean
    End Function

    <SuppressUnmanagedCodeSecurity>
    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function GetWindowText(
            ByVal hwnd As IntPtr,
            ByVal buffer As StringBuilder,
            ByVal capacity As Integer
    ) As Integer
    End Function

    <SuppressUnmanagedCodeSecurity>
    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function WindowFromPoint(
            ByVal p As Point
    ) As IntPtr
    End Function

    <SuppressUnmanagedCodeSecurity>
    <DllImport("user32.dll", SetLastError:=True)>
    Friend Shared Function GetWindowThreadProcessId(
            ByVal hwnd As IntPtr,
            ByRef pid As Integer
    ) As Integer
    End Function

    <SuppressUnmanagedCodeSecurity>
    <DllImport("user32.dll", SetLastError:=False)>
    Friend Shared Function GetParent(
            ByVal hWnd As IntPtr
    ) As IntPtr
    End Function

    ''' <summary>
    ''' Prevents a default instance of the <see cref="NativeMethods"/> class from being created.
    ''' </summary>
    Private Sub New()
    End Sub

End Class
