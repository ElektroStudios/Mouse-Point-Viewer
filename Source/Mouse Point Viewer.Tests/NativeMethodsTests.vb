#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Text
Imports System.Windows.Forms

Imports Microsoft.Win32.SafeHandles

Imports MousePointViewer.Tests.Helpers

Imports Win32

#End Region

#Region " NativeMethodsTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Tests for the P/Invoke declarations of <see cref="NativeMethods"/> and the Win32 structures and constants,
    ''' verifying that they are correctly declared (signatures, marshaling, values) against real windows and processes.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class NativeMethodsTests

#Region " Structures and Constants "

        ''' <summary>
        ''' Verifies that the <see cref="NativeRectangle"/> structure matches the native <c>RECT</c> layout (4 x LONG).
        ''' </summary>
        <TestMethod>
        Public Sub NativeRectangle_HasNativeRectLayout()

            Assert.AreEqual(16, Marshal.SizeOf(GetType(NativeRectangle)))
            Assert.AreEqual(0, Marshal.OffsetOf(GetType(NativeRectangle), NameOf(NativeRectangle.Left)).ToInt32())
            Assert.AreEqual(4, Marshal.OffsetOf(GetType(NativeRectangle), NameOf(NativeRectangle.Top)).ToInt32())
            Assert.AreEqual(8, Marshal.OffsetOf(GetType(NativeRectangle), NameOf(NativeRectangle.Right)).ToInt32())
            Assert.AreEqual(12, Marshal.OffsetOf(GetType(NativeRectangle), NameOf(NativeRectangle.Bottom)).ToInt32())
        End Sub

        ''' <summary>
        ''' Verifies the values of the Win32 constants against the Windows SDK headers.
        ''' </summary>
        <TestMethod>
        Public Sub Constants_MatchWindowsSdkValues()

            Assert.AreEqual(-20, Constants.GWL_EXSTYLE)
            Assert.AreEqual(&H80000, Constants.WS_EX_LAYERED)
            Assert.AreEqual(&H20, Constants.WS_EX_TRANSPARENT)
            Assert.AreEqual(2UI, Constants.GW_HWNDNEXT)
            Assert.AreEqual(5UI, Constants.GW_CHILD)
            Assert.AreEqual(14, Constants.DWMWA_CLOAKED)
            Assert.AreEqual(&HCC0020, Constants.SRCCOPY)
            Assert.AreEqual(&H1000, Constants.PROCESS_QUERY_LIMITED_INFORMATION)
        End Sub

#End Region

#Region " Process Functions "

        ''' <summary>
        ''' Verifies <c>OpenProcess</c> and <c>IsWow64Process</c> on the current process.
        ''' </summary>
        <TestMethod>
        Public Sub OpenProcess_IsWow64Process_CurrentProcess()

            Using current As Process = Process.GetCurrentProcess(),
                  hProcess As SafeProcessHandle = NativeMethods.OpenProcess(Constants.PROCESS_QUERY_LIMITED_INFORMATION, False, current.Id)

                Assert.IsFalse(hProcess.IsInvalid, $"OpenProcess failed: Win32 error {Marshal.GetLastWin32Error()}.")

                Dim isWow64 As Boolean
                Assert.IsTrue(NativeMethods.IsWow64Process(hProcess, isWow64))

                Dim expectedWow64 As Boolean = Environment.Is64BitOperatingSystem AndAlso Not Environment.Is64BitProcess
                Assert.AreEqual(expectedWow64, isWow64)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that <c>OpenProcess</c> returns an invalid handle for a non-existent process.
        ''' </summary>
        <TestMethod>
        Public Sub OpenProcess_NonExistentProcess_ReturnsInvalidHandle()

            Using hProcess As SafeProcessHandle = NativeMethods.OpenProcess(Constants.PROCESS_QUERY_LIMITED_INFORMATION, False, Integer.MaxValue)
                Assert.IsTrue(hProcess.IsInvalid)
            End Using
        End Sub

#End Region

#Region " Window Functions "

        ''' <summary>
        ''' Verifies <c>GetWindowThreadProcessId</c> on a window owned by the current process.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowThreadProcessId_OwnWindow_ReturnsCurrentProcessId()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), Color.White).ShowAndPaint(),
                  current As Process = Process.GetCurrentProcess()

                Dim pid As Integer
                Dim tid As Integer = NativeMethods.GetWindowThreadProcessId(window.Handle, pid)

                Assert.AreEqual(current.Id, pid)
                Assert.AreNotEqual(0, tid)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies <c>GetWindowText</c>, including non-ASCII characters (Unicode marshaling).
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowText_ReturnsCaption()

            Const caption As String = "Mouse Point Viewer ✓ Ñandú 日本"
            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), Color.White, caption).ShowAndPaint()

                Dim sb As New StringBuilder(1024)
                Dim length As Integer = NativeMethods.GetWindowText(window.Handle, sb, sb.Capacity)

                Assert.AreEqual(caption.Length, length)
                Assert.AreEqual(caption, sb.ToString())
            End Using
        End Sub

        ''' <summary>
        ''' Verifies <c>GetClassName</c> and <c>RealGetWindowClass</c>:
        ''' a WinForms button is a superclass of the system "Button" class.
        ''' </summary>
        <TestMethod>
        Public Sub GetClassName_RealGetWindowClass_WinFormsButton()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim button As New Button() With {.Text = "OK", .Location = New Point(10, 10)}
                window.Form.Controls.Add(button)
                window.ShowAndPaint()

                Dim className As New StringBuilder(256)
                Dim realClassName As New StringBuilder(256)
                NativeMethods.GetClassName(button.Handle, className, className.Capacity)
                NativeMethods.RealGetWindowClass(button.Handle, realClassName, realClassName.Capacity)

                StringAssert.StartsWith(className.ToString(), "WindowsForms10.", StringComparison.OrdinalIgnoreCase)
                Assert.AreEqual("Button", realClassName.ToString(), ignoreCase:=True)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies <c>GetWindowRect</c>, <c>GetClientRect</c>
        ''' and <c>ClientToScreen</c> on a borderless window, whose client area equals its bounds.
        ''' </summary>
        <TestMethod>
        Public Sub GetWindowRect_GetClientRect_ClientToScreen_BorderlessWindow()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), Color.White).ShowAndPaint()

                Dim expected As Rectangle = window.Bounds

                Dim windowRect As NativeRectangle
                Assert.IsTrue(NativeMethods.GetWindowRect(window.Handle, windowRect))
                Assert.AreEqual(expected.Left, windowRect.Left)
                Assert.AreEqual(expected.Top, windowRect.Top)
                Assert.AreEqual(expected.Right, windowRect.Right)
                Assert.AreEqual(expected.Bottom, windowRect.Bottom)

                Dim clientRect As NativeRectangle
                Assert.IsTrue(NativeMethods.GetClientRect(window.Handle, clientRect))
                Assert.AreEqual(0, clientRect.Left)
                Assert.AreEqual(0, clientRect.Top)
                Assert.AreEqual(expected.Width, clientRect.Right)
                Assert.AreEqual(expected.Height, clientRect.Bottom)

                Dim pt As New Point(0, 0)
                Assert.IsTrue(NativeMethods.ClientToScreen(window.Handle, pt))
                Assert.AreEqual(expected.Location, pt)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies <c>IsWindowVisible</c> on a shown and a hidden window.
        ''' </summary>
        <TestMethod>
        Public Sub IsWindowVisible_ShownAndHidden()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), Color.White).ShowAndPaint()

                Assert.IsTrue(NativeMethods.IsWindowVisible(window.Handle))

                window.Form.Hide()
                TestWindow.PumpMessages()

                Assert.IsFalse(NativeMethods.IsWindowVisible(window.Handle))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies <c>GetWindow</c> with <see cref="Constants.GW_CHILD"/> and
        ''' <see cref="Constants.GW_HWNDNEXT"/>: the children are enumerated in Z-order (top to bottom).
        ''' </summary>
        <TestMethod>
        Public Sub GetWindow_EnumeratesChildrenInZOrder()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim first As New Panel()
                Dim second As New Panel()
                window.Form.Controls.Add(first)
                window.Form.Controls.Add(second)
                second.BringToFront()
                window.ShowAndPaint()

                Dim child As IntPtr = NativeMethods.GetWindow(window.Handle, Constants.GW_CHILD)
                Dim nextChild As IntPtr = NativeMethods.GetWindow(child, Constants.GW_HWNDNEXT)
                Dim afterLast As IntPtr = NativeMethods.GetWindow(nextChild, Constants.GW_HWNDNEXT)

                Assert.AreEqual(second.Handle, child)
                Assert.AreEqual(first.Handle, nextChild)
                Assert.AreEqual(IntPtr.Zero, afterLast)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies <c>DwmGetWindowAttribute</c> with <see cref="Constants.DWMWA_CLOAKED"/>:
        ''' a regular shown window is not cloaked.
        ''' </summary>
        <TestMethod>
        Public Sub DwmGetWindowAttribute_Cloaked_RegularWindowIsNotCloaked()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), Color.White).ShowAndPaint()

                Dim cloaked As Integer = -1
                Dim hr As Integer = NativeMethods.DwmGetWindowAttribute(window.Handle, Constants.DWMWA_CLOAKED, cloaked, Marshal.SizeOf(GetType(Integer)))

                Assert.AreEqual(0, hr, $"HRESULT: 0x{hr:X8}")
                Assert.AreEqual(0, cloaked)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
