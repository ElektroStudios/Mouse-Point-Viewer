
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Text

Imports DevCase.Extensions.PropertyGridExtensions

#End Region

Partial Friend NotInheritable Class Main : Inherits Form

    Private ReadOnly mouseInfo As New MouseCoordinatesInfo()
    Private ignoreChildWindows As Boolean

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.PropertyGrid1.SelectedObject = Me.mouseInfo
    End Sub

    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Me.MinimumSize = New Size(Me.Width, Me.Height)
        Me.MaximumSize = New Size(Integer.MaxValue, Me.Height)
        Me.PropertyGrid1.SetSplitterPosition(150)
        Me.RefreshTimer.Start()
    End Sub

    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Me.RefreshTimer.Stop()
    End Sub

    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        If Me.PropertyGrid1.IsInitialized Then
            Me.PropertyGrid1.SetSplitterPosition(150)
        End If
    End Sub

    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.Control AndAlso e.KeyCode = Keys.C Then
            Clipboard.SetText(Me.mouseInfo.ToString())
            e.Handled = True
        End If
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        Process.Start("https://github.com/ElektroStudios/Mouse-Point-Viewer")
    End Sub

    Private Sub CheckBox_ChildWindows_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_ChildWindows.CheckedChanged

        Dim cb As CheckBox = DirectCast(sender, CheckBox)
        Me.ignoreChildWindows = cb.Checked
    End Sub

    Private Sub CheckBox_TopMost_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_TopMost.CheckedChanged

        Dim cb As CheckBox = DirectCast(sender, CheckBox)
        Me.TopMost = cb.Checked
    End Sub

    Private Sub NumericUpDown_Refresh_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDown_Refresh.TextChanged

        Dim nud As NumericUpDown = DirectCast(sender, NumericUpDown)

        Dim value As Integer
        If Integer.TryParse(nud.Text, value) AndAlso
            value >= nud.Minimum AndAlso
            value <= nud.Maximum Then

            Me.RefreshTimer.Interval = value
        End If
    End Sub

    Private Sub RefreshTimer_Tick(sender As Object, e As EventArgs) Handles RefreshTimer.Tick

        Dim hwnd As IntPtr = NativeMethods.WindowFromPoint(MousePosition)

        If (hwnd <> IntPtr.Zero) Then
            If (Me.ignoreChildWindows) Then
                Dim parentHwnd As IntPtr

                Do While True
                    parentHwnd = NativeMethods.GetParent(hwnd)

                    If (parentHwnd <> IntPtr.Zero) Then
                        hwnd = parentHwnd
                    Else
                        Exit Do
                    End If
                Loop
            End If

            Me.ShowInfo(hwnd)
        End If
    End Sub

    Private Sub ShowInfo(hWnd As IntPtr)

        Dim pid As Integer
        NativeMethods.GetWindowThreadProcessId(hWnd, pid)

        Dim pr As Process = Process.GetProcessById(pid)

        Dim sb As New StringBuilder(1024)
        NativeMethods.GetWindowText(hWnd, sb, sb.Capacity)

        Dim rc As NativeMethods.Rect
        NativeMethods.GetWindowRect(hWnd, rc)

        Dim ptRelative As New Point(Control.MousePosition.X - rc.Left, Control.MousePosition.Y - rc.Top)
        Dim ptGlobal As New Point(ptRelative.X + rc.Left, ptRelative.Y + rc.Top)

        Me.mouseInfo.Caption = sb.ToString()
        Me.mouseInfo.ProcessName = pr.ProcessName
        Me.mouseInfo.ProcessId = pid
        Me.mouseInfo.WindowHandle = $"{hWnd} (0x{hWnd.ToInt64():X})"
        Me.mouseInfo.ScreenCoordinates = String.Format("X={0}, Y={1}", ptGlobal.X, ptGlobal.Y)
        Me.mouseInfo.RelativeCoordinates = String.Format("X={0}, Y={1}", ptRelative.X, ptRelative.Y)
        Me.mouseInfo.Color = Me.GetColorAtScreenPosition(ptGlobal.X, ptGlobal.Y)

        Me.PropertyGrid1.Refresh()
    End Sub

    Private Function GetColorAtScreenPosition(screenX As Integer, screenY As Integer) As Color

        Dim hdc As IntPtr = NativeMethods.GetDC(IntPtr.Zero)

        Dim pixel As UInteger = NativeMethods.GetPixel(hdc, screenX, screenY)

        NativeMethods.ReleaseDC(IntPtr.Zero, hdc)

        Dim r As Integer = CInt(pixel And &HFF)
        Dim g As Integer = CInt((pixel >> 8) And &HFF)
        Dim b As Integer = CInt((pixel >> 16) And &HFF)

        Return Color.FromArgb(r, g, b)
    End Function

End Class
