Public NotInheritable Class Main : Inherits Form

    Friend WithEvents RefreshTimer As New System.Windows.Forms.Timer
    Private ignoreChildWindowsFlag As Boolean

    Private Sub Main_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        ' Nothing to do here...
    End Sub

    Private Sub Main_Shown(ByVal sender As Object, ByVal e As EventArgs) _
    Handles MyBase.Shown

        With Me.RefreshTimer
            .Enabled = True
            .Start()
        End With

    End Sub

    Private Sub Main_FormClosing(sender As Object, e As FormClosingEventArgs) _
    Handles MyBase.FormClosing

        With Me.RefreshTimer
            .Stop()
            .Enabled = False
            .Dispose()
        End With

    End Sub

    Private Sub CheckBox_ChildWindows_CheckedChanged(ByVal sender As Object, ByVal e As EventArgs) _
    Handles CheckBox_ChildWindows.CheckedChanged

        Me.ignoreChildWindowsFlag = DirectCast(sender, CheckBox).Checked

    End Sub

    Private Sub NumericUpDown_Refresh_ValueChanged(ByVal sender As Object, ByVal e As EventArgs) _
    Handles NumericUpDown_Refresh.ValueChanged

        Me.RefreshTimer.Interval = CInt(DirectCast(sender, NumericUpDown).Value)

    End Sub

    Private Sub WindowTimer_Tick(ByVal sender As Object, ByVal e As EventArgs) _
    Handles RefreshTimer.Tick

        Dim hwnd As IntPtr = NativeMethods.WindowFromPoint(MousePosition)

        If (hwnd <> IntPtr.Zero) Then

            If (Me.ignoreChildWindowsFlag) Then

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

    Private Sub ShowInfo(ByVal hwnd As IntPtr)

        Dim pid As Integer
        NativeMethods.GetWindowThreadProcessId(hwnd, pid)

        Dim proc As Process = Process.GetProcessById(pid)

        Dim sb As New StringBuilder(256)
        NativeMethods.GetWindowText(hwnd, sb, sb.Capacity)

        Dim rc As NativeMethods.Rect
        NativeMethods.GetWindowRect(hwnd, rc)

        Dim pt As New Point(Control.MousePosition.X - rc.Left, Control.MousePosition.Y - rc.Top)

        Me.Label_ProcName_Value.Text = proc.ProcessName
        Me.Label_PID_Value.Text = pid.ToString
        Me.Label_Hwnd_Value.Text = hwnd.ToString
        Me.Label_Caption_Value.Text = sb.ToString
        Me.Label_CoordsRelative_Value.Text = String.Format("X={0}, Y={1}", pt.X, pt.Y)
        Me.Label_CoordsScreen_Value.Text = String.Format("X={0}, Y={1}", Control.MousePosition.X, Control.MousePosition.Y)

    End Sub

End Class
