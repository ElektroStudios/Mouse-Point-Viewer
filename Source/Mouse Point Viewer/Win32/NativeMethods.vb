#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Runtime.InteropServices
Imports System.Security

Imports Microsoft.Win32.SafeHandles
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

        ''' <summary>
        ''' Sets the value of a Desktop Window Manager (DWM) non-client rendering attribute for a window.
        ''' </summary>
        '''
        ''' <param name="hWnd">
        ''' The handle to the window for which the attribute value is to be set.
        ''' </param>
        '''
        ''' <param name="dwAttribute">
        ''' The attribute to set (e.g. <see cref="Constants.DWMWA_USE_IMMERSIVE_DARK_MODE"/>).
        ''' </param>
        '''
        ''' <param name="pvAttribute">
        ''' The attribute value.
        ''' </param>
        '''
        ''' <param name="cbAttribute">
        ''' The size, in bytes, of the attribute value.
        ''' </param>
        '''
        ''' <returns>
        ''' <c>S_OK</c> (0) if the function succeeds, otherwise an <c>HRESULT</c> error code.
        ''' </returns>
        <DllImport("dwmapi.dll", PreserveSig:=True)>
        Friend Function DwmSetWindowAttribute(hWnd As IntPtr,
                                              dwAttribute As Integer,
                                              ByRef pvAttribute As Integer,
                                              cbAttribute As Integer
        ) As Integer
        End Function

#End Region

#Region " uxtheme "

        ''' <summary>
        ''' Causes a window to use a different set of visual style information than its class normally uses
        ''' (e.g. "DarkMode_Explorer" for dark scroll bars).
        ''' </summary>
        '''
        ''' <param name="hWnd">
        ''' Handle to the window whose visual style information is to be changed.
        ''' </param>
        '''
        ''' <param name="pszSubAppName">
        ''' The application name to use in place of the calling application's name,
        ''' or <see langword="Nothing"/> to restore the default visual style.
        ''' </param>
        '''
        ''' <param name="pszSubIdList">
        ''' A semicolon-separated list of class identifier names, or <see langword="Nothing"/>.
        ''' </param>
        '''
        ''' <returns>
        ''' <c>S_OK</c> (0) if the function succeeds, otherwise an <c>HRESULT</c> error code.
        ''' </returns>
        <DllImport("uxtheme.dll", CharSet:=CharSet.Unicode, PreserveSig:=True)>
        Friend Function SetWindowTheme(hWnd As IntPtr,
                                       pszSubAppName As String,
                                       pszSubIdList As String
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

        ''' <summary>
        ''' Determines whether the specified process is running under WOW64.
        ''' </summary>
        '''
        ''' <param name="hProcess">
        ''' A handle to the process, opened with at least <see cref="Constants.PROCESS_QUERY_LIMITED_INFORMATION"/> access.
        ''' </param>
        '''
        ''' <param name="refWow64Process">
        ''' Receives <see langword="True"/> if the process is running under WOW64.
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds, <see langword="False"/> otherwise.
        ''' </returns>
        <DllImport("kernel32.dll", SetLastError:=True, CallingConvention:=CallingConvention.Winapi)>
        Friend Function IsWow64Process(hProcess As SafeProcessHandle,
                                       <MarshalAs(UnmanagedType.Bool)> ByRef refWow64Process As Boolean
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Opens an existing local process object.
        ''' </summary>
        '''
        ''' <param name="dwDesiredAccess">
        ''' The access to the process object.
        ''' </param>
        '''
        ''' <param name="bInheritHandle">
        ''' If <see langword="True"/>, processes created by this process will inherit the handle.
        ''' </param>
        '''
        ''' <param name="dwProcessId">
        ''' The identifier of the local process to be opened.
        ''' </param>
        '''
        ''' <returns>
        ''' An open handle to the specified process. If the function fails, the handle is invalid.
        ''' </returns>
        <DllImport("kernel32.dll", SetLastError:=True)>
        Friend Function OpenProcess(dwDesiredAccess As Integer,
                                    <MarshalAs(UnmanagedType.Bool)> bInheritHandle As Boolean,
                                    dwProcessId As Integer
        ) As SafeProcessHandle
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

        ''' <summary>
        ''' Defines a system-wide hot key.
        ''' <para></para>
        ''' When the hot key is pressed, the system posts a <see cref="Constants.WM_HOTKEY"/> message
        ''' to the specified window. No hook is installed, so it has no impact on the system input performance.
        ''' </summary>
        '''
        ''' <param name="hWnd">
        ''' A handle to the window that will receive the <see cref="Constants.WM_HOTKEY"/> messages.
        ''' </param>
        '''
        ''' <param name="id">
        ''' The identifier of the hot key.
        ''' </param>
        '''
        ''' <param name="fsModifiers">
        ''' The keys that must be pressed in combination with the key specified by <paramref name="vk"/>
        ''' (a combination of the <c>MOD_*</c> values, see <see cref="HotkeyModifiers"/>).
        ''' </param>
        '''
        ''' <param name="vk">
        ''' The virtual-key code of the hot key.
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds, <see langword="False"/> otherwise
        ''' (e.g. the hot key is already registered by another application).
        ''' </returns>
        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function RegisterHotKey(hWnd As IntPtr,
                                       id As Integer,
                                       fsModifiers As UInteger,
                                       vk As UInteger
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Frees a hot key previously registered by the calling thread.
        ''' </summary>
        '''
        ''' <param name="hWnd">
        ''' A handle to the window associated with the hot key to be freed.
        ''' </param>
        '''
        ''' <param name="id">
        ''' The identifier of the hot key to be freed.
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds, <see langword="False"/> otherwise.
        ''' </returns>
        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function UnregisterHotKey(hWnd As IntPtr,
                                         id As Integer
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Retrieves a handle to the foreground window (the window with which the user is currently working).
        ''' </summary>
        '''
        ''' <returns>
        ''' The handle of the foreground window, or <see cref="IntPtr.Zero"/> in certain circumstances
        ''' (e.g. when a window is losing activation).
        ''' </returns>
        <DllImport("user32.dll")>
        Friend Function GetForegroundWindow() As IntPtr
        End Function

        ''' <summary>
        ''' Sets the opacity and transparency color key of a layered window.
        ''' </summary>
        '''
        ''' <param name="hWnd">
        ''' A handle to the layered window.
        ''' </param>
        '''
        ''' <param name="crKey">
        ''' The transparency color key (<c>COLORREF</c>).
        ''' </param>
        '''
        ''' <param name="bAlpha">
        ''' The alpha value used to describe the opacity of the layered window.
        ''' </param>
        '''
        ''' <param name="dwFlags">
        ''' An action to be taken (e.g. <see cref="Constants.LWA_ALPHA"/>).
        ''' </param>
        '''
        ''' <returns>
        ''' <see langword="True"/> if the function succeeds, <see langword="False"/> otherwise.
        ''' </returns>
        <DllImport("user32.dll", SetLastError:=True)>
        Friend Function SetLayeredWindowAttributes(hWnd As IntPtr,
                                                   crKey As UInteger,
                                                   bAlpha As Byte,
                                                   dwFlags As UInteger
        ) As <MarshalAs(UnmanagedType.Bool)> Boolean
        End Function

        ''' <summary>
        ''' Sends the specified message to a window and does not return until the window procedure has processed the message.
        ''' </summary>
        '''
        ''' <param name="hWnd">
        ''' A handle to the window whose window procedure will receive the message.
        ''' </param>
        '''
        ''' <param name="msg">
        ''' The message to be sent.
        ''' </param>
        '''
        ''' <param name="wParam">
        ''' Additional message-specific information.
        ''' </param>
        '''
        ''' <param name="lParam">
        ''' Additional message-specific information.
        ''' </param>
        '''
        ''' <returns>
        ''' The result of the message processing.
        ''' </returns>
        <DllImport("user32.dll")>
        Friend Function SendMessage(hWnd As IntPtr,
                                    msg As Integer,
                                    wParam As IntPtr,
                                    lParam As IntPtr
        ) As IntPtr
        End Function

#End Region

    End Module

End Namespace