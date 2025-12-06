
#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Text

Imports DevCase.Extensions.PropertyGridExtensions

Imports Win32

#End Region

Partial Friend NotInheritable Class Main : Inherits Form

    Private ReadOnly AllInfo As New AllInfoContainer()

    Private Const PropertyGridSplitterPosition As Integer = 140

    Friend Shared ignoreChildWindows As Boolean
    Friend Shared ignoreLayeredWindows As Boolean
    Friend Shared ignoreTransparentWindows As Boolean

    <DebuggerStepThrough>
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#If DEBUG Then ' CREATE A LAYERED OR TRNSPARENT FORM TO TEST FUNCTIONALITIES.

        Dim layeredForm As New Form() With {
            .FormBorderStyle = FormBorderStyle.None,
            .StartPosition = FormStartPosition.CenterScreen,
            .Width = 500,
            .Height = 500,
            .Text = "Debug Form",
            .TopMost = True,
            .AllowTransparency = True,
            .BackColor = Color.Lime,
            .TransparencyKey = Color.Lime, .Opacity = 0
        }

        ' Agregar algo visible dentro para diferenciar
        Dim lbl As New Label() With {
            .Text = "Layered Window",
            .ForeColor = Color.Black,
            .BackColor = Color.Transparent,
            .Font = New Font("Segoe UI", 96, FontStyle.Bold),
            .AutoSize = True,
            .Location = New Point(40, 80)
        }
        layeredForm.Controls.Add(lbl)
        layeredForm.Show()

        Dim exStyle As IntPtr = WindowHelper.GetWindowLongPtrSafe(layeredForm.Handle, Constants.GWL_EXSTYLE)
        Dim newStyle As IntPtr = CType(exStyle.ToInt64() Or CLng(Constants.WS_EX_LAYERED Or Constants.WS_EX_TRANSPARENT), IntPtr)

        WindowHelper.SetWindowLongPtrSafe(layeredForm.Handle, Constants.GWL_EXSTYLE, newStyle)
#End If

        Me.PropertyGrid1.SelectedObject = Me.AllInfo
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Me.MinimumSize = New Size(Me.Width, Me.Height)
        Me.PropertyGrid1.SetSplitterPosition(Main.PropertyGridSplitterPosition)
        Me.RefreshTimer.Start()
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Me.RefreshTimer.Stop()
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        If Me.PropertyGrid1.IsInitialized Then
            Me.PropertyGrid1.SetSplitterPosition(Main.PropertyGridSplitterPosition)
        End If
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        If e.Control AndAlso e.KeyCode = Keys.C Then
            Clipboard.SetText(Me.allInfo.ToString())
            e.Handled = True
        End If
    End Sub

    <DebuggerStepThrough>
    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked

        Try
            Process.Start("https://github.com/ElektroStudios/Mouse-Point-Viewer")

        Catch ex As Exception
            MessageBox.Show(Me, $"ERROR: {ex.Message}", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try
    End Sub

    <DebuggerStepThrough>
    Private Sub CheckBox_IgnoreChildWindows_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_IgnoreChildWindows.CheckedChanged

        Dim cb As CheckBox = DirectCast(sender, CheckBox)
        Main.ignoreChildWindows = cb.Checked
    End Sub

    <DebuggerStepThrough>
    Private Sub CheckBox_IgnoreLayeredWindows_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_IgnoreLayeredWindows.CheckedChanged

        Dim cb As CheckBox = DirectCast(sender, CheckBox)
        Main.ignoreLayeredWindows = cb.Checked
    End Sub

    <DebuggerStepThrough>
    Private Sub CheckBox_IgnoreTransparentWindows_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_IgnoreTransparentWindows.CheckedChanged

        Dim cb As CheckBox = DirectCast(sender, CheckBox)
        Main.ignoreTransparentWindows = cb.Checked
    End Sub

    <DebuggerStepThrough>
    Private Sub CheckBox_TopMost_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_TopMost.CheckedChanged

        Dim cb As CheckBox = DirectCast(sender, CheckBox)
        Me.TopMost = cb.Checked
    End Sub

    <DebuggerStepThrough>
    Private Sub NumericUpDown_Refresh_TextChanged(sender As Object, e As EventArgs) Handles NumericUpDown_Refresh.TextChanged

        Dim nud As NumericUpDown = DirectCast(sender, NumericUpDown)

        Dim value As Integer
        If Integer.TryParse(nud.Text, value) AndAlso
            value >= nud.Minimum AndAlso
            value <= nud.Maximum Then

            Me.RefreshTimer.Interval = value
        End If
    End Sub

    <DebuggerStepThrough>
    Private Sub RefreshTimer_Tick(sender As Object, e As EventArgs) Handles RefreshTimer.Tick

        Dim ptScreen As Point = Control.MousePosition
        Dim hWnd As IntPtr = WindowHelper.GetWindowFromPoint(ptScreen)

        Me.UpdateAllInfo(hWnd, ptScreen)
    End Sub

    <DebuggerStepThrough>
    Private Sub UpdateAllInfo(hWnd As IntPtr, ptScreen As Point)

        ' Window Handle
        Dim windowHandleInfo As New WindowHandlePropertyInfo(hWnd)
        Me.AllInfo.WindowHandle = windowHandleInfo

        ' Process
        Dim pid As Integer
        NativeMethods.GetWindowThreadProcessId(hWnd, pid)
        Dim pr As Process = Process.GetProcessById(pid)
        Me.AllInfo.Process = New ProcessPropertyInfo(pr)

        ' Window Caption
        Dim caption As New StringBuilder(1024)
        NativeMethods.GetWindowText(hWnd, caption, caption.Capacity)
        Me.AllInfo.Caption = New CaptionPropertyInfo With {
            .Text = caption.ToString()
        }

        ' Class Name
        Dim classNameBuffer As New StringBuilder(256)
        Dim realClassNameBuffer As New StringBuilder(256)
        NativeMethods.GetClassName(hWnd, classNameBuffer, classNameBuffer.Capacity)
        NativeMethods.RealGetWindowClass(hWnd, realClassNameBuffer, classNameBuffer.Capacity)
        Me.AllInfo.ClassName = New ClassNamePropertyInfo With {
            .Name = classNameBuffer.ToString(),
            .RealName = realClassNameBuffer.ToString()
        }

        ' Window Bounds
        '
        '   Rectangle
        Me.AllInfo.WindowBounds = New WindowBoundsPropertyContainer()
        Dim windowRect As NativeRectangle
        NativeMethods.GetWindowRect(hWnd, windowRect)
        Me.AllInfo.WindowBounds.RectangleInfo = New RectanglePropertyInfo(New Rectangle(
            windowRect.Left,
            windowRect.Top,
            windowRect.Right - windowRect.Left,
            windowRect.Bottom - windowRect.Top
        ))
        '   Client Rectangle
        Dim clientRect As NativeRectangle
        NativeMethods.GetClientRect(hWnd, clientRect)
        Dim ptClientTopLeft As New Point(0, 0)
        NativeMethods.ClientToScreen(hWnd, ptClientTopLeft)
        Me.AllInfo.WindowBounds.ClientRectangleInfo = New RectanglePropertyInfo(New Rectangle(
            ptClientTopLeft.X,
            ptClientTopLeft.Y,
            clientRect.Right - clientRect.Left,
            clientRect.Bottom - clientRect.Top
        ))

        ' Mouse Points
        Me.AllInfo.Point = New PointPropertyContainer With {
            .ScreenPointInfo = New PointPropertyInfo(ptScreen),
            .ClientPointInfo = New PointPropertyInfo(New Point(
                ptScreen.X - ptClientTopLeft.X,
                ptScreen.Y - ptClientTopLeft.Y
            ))
        }

        ' Pixel Color
        Dim pixelColor As Color = WindowHelper.GetPixelColorAtScreenPosition(ptScreen.X, ptScreen.Y)
        Me.AllInfo.PixelColor = New PixelColorPropertyInfo(pixelColor)

        Me.PropertyGrid1.Refresh()
    End Sub

End Class
