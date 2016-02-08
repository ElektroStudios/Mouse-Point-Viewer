<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.Label_Hwnd = New System.Windows.Forms.Label()
        Me.Label_Hwnd_Value = New System.Windows.Forms.Label()
        Me.Label_CoordsRelative = New System.Windows.Forms.Label()
        Me.Label_CoordsRelative_Value = New System.Windows.Forms.Label()
        Me.Label_CoordsScreen_Value = New System.Windows.Forms.Label()
        Me.Label_CoordsScreen = New System.Windows.Forms.Label()
        Me.Label_Caption_Value = New System.Windows.Forms.Label()
        Me.Label_Caption = New System.Windows.Forms.Label()
        Me.CheckBox_ChildWindows = New System.Windows.Forms.CheckBox()
        Me.Label_PID_Value = New System.Windows.Forms.Label()
        Me.Label_PID = New System.Windows.Forms.Label()
        Me.Label_ProcName_Value = New System.Windows.Forms.Label()
        Me.Label_ProcName = New System.Windows.Forms.Label()
        Me.NumericUpDown_Refresh = New System.Windows.Forms.NumericUpDown()
        Me.Label_refresh = New System.Windows.Forms.Label()
        Me.Label_refresh_ms = New System.Windows.Forms.Label()
        CType(Me.NumericUpDown_Refresh, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label_Hwnd
        '
        Me.Label_Hwnd.AutoSize = True
        Me.Label_Hwnd.Location = New System.Drawing.Point(12, 56)
        Me.Label_Hwnd.Name = "Label_Hwnd"
        Me.Label_Hwnd.Size = New System.Drawing.Size(86, 13)
        Me.Label_Hwnd.TabIndex = 0
        Me.Label_Hwnd.Text = "Window Handle:"
        '
        'Label_Hwnd_Value
        '
        Me.Label_Hwnd_Value.AutoSize = True
        Me.Label_Hwnd_Value.Location = New System.Drawing.Point(104, 56)
        Me.Label_Hwnd_Value.Name = "Label_Hwnd_Value"
        Me.Label_Hwnd_Value.Size = New System.Drawing.Size(16, 13)
        Me.Label_Hwnd_Value.TabIndex = 1
        Me.Label_Hwnd_Value.Text = "..."
        '
        'Label_CoordsRelative
        '
        Me.Label_CoordsRelative.AutoSize = True
        Me.Label_CoordsRelative.Location = New System.Drawing.Point(12, 108)
        Me.Label_CoordsRelative.Name = "Label_CoordsRelative"
        Me.Label_CoordsRelative.Size = New System.Drawing.Size(88, 13)
        Me.Label_CoordsRelative.TabIndex = 2
        Me.Label_CoordsRelative.Text = "Relative Coords.:"
        '
        'Label_CoordsRelative_Value
        '
        Me.Label_CoordsRelative_Value.AutoSize = True
        Me.Label_CoordsRelative_Value.Location = New System.Drawing.Point(103, 108)
        Me.Label_CoordsRelative_Value.Name = "Label_CoordsRelative_Value"
        Me.Label_CoordsRelative_Value.Size = New System.Drawing.Size(16, 13)
        Me.Label_CoordsRelative_Value.TabIndex = 3
        Me.Label_CoordsRelative_Value.Text = "..."
        '
        'Label_CoordsScreen_Value
        '
        Me.Label_CoordsScreen_Value.AutoSize = True
        Me.Label_CoordsScreen_Value.Location = New System.Drawing.Point(103, 134)
        Me.Label_CoordsScreen_Value.Name = "Label_CoordsScreen_Value"
        Me.Label_CoordsScreen_Value.Size = New System.Drawing.Size(16, 13)
        Me.Label_CoordsScreen_Value.TabIndex = 5
        Me.Label_CoordsScreen_Value.Text = "..."
        '
        'Label_CoordsScreen
        '
        Me.Label_CoordsScreen.AutoSize = True
        Me.Label_CoordsScreen.Location = New System.Drawing.Point(12, 134)
        Me.Label_CoordsScreen.Name = "Label_CoordsScreen"
        Me.Label_CoordsScreen.Size = New System.Drawing.Size(83, 13)
        Me.Label_CoordsScreen.TabIndex = 4
        Me.Label_CoordsScreen.Text = "Screen Coords.:"
        '
        'Label_Caption_Value
        '
        Me.Label_Caption_Value.AutoSize = True
        Me.Label_Caption_Value.Location = New System.Drawing.Point(103, 82)
        Me.Label_Caption_Value.Name = "Label_Caption_Value"
        Me.Label_Caption_Value.Size = New System.Drawing.Size(16, 13)
        Me.Label_Caption_Value.TabIndex = 7
        Me.Label_Caption_Value.Text = "..."
        '
        'Label_Caption
        '
        Me.Label_Caption.AutoSize = True
        Me.Label_Caption.Location = New System.Drawing.Point(12, 82)
        Me.Label_Caption.Name = "Label_Caption"
        Me.Label_Caption.Size = New System.Drawing.Size(88, 13)
        Me.Label_Caption.TabIndex = 6
        Me.Label_Caption.Text = "Window Caption:"
        '
        'CheckBox_ChildWindows
        '
        Me.CheckBox_ChildWindows.AutoSize = True
        Me.CheckBox_ChildWindows.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_ChildWindows.Location = New System.Drawing.Point(15, 161)
        Me.CheckBox_ChildWindows.Name = "CheckBox_ChildWindows"
        Me.CheckBox_ChildWindows.Size = New System.Drawing.Size(129, 17)
        Me.CheckBox_ChildWindows.TabIndex = 8
        Me.CheckBox_ChildWindows.Text = "Ignore Child Windows"
        Me.CheckBox_ChildWindows.UseVisualStyleBackColor = True
        '
        'Label_PID_Value
        '
        Me.Label_PID_Value.AutoSize = True
        Me.Label_PID_Value.Location = New System.Drawing.Point(104, 32)
        Me.Label_PID_Value.Name = "Label_PID_Value"
        Me.Label_PID_Value.Size = New System.Drawing.Size(16, 13)
        Me.Label_PID_Value.TabIndex = 10
        Me.Label_PID_Value.Text = "..."
        '
        'Label_PID
        '
        Me.Label_PID.AutoSize = True
        Me.Label_PID.Location = New System.Drawing.Point(12, 32)
        Me.Label_PID.Name = "Label_PID"
        Me.Label_PID.Size = New System.Drawing.Size(63, 13)
        Me.Label_PID.TabIndex = 9
        Me.Label_PID.Text = "Process Id.:"
        '
        'Label_ProcName_Value
        '
        Me.Label_ProcName_Value.AutoSize = True
        Me.Label_ProcName_Value.Location = New System.Drawing.Point(104, 9)
        Me.Label_ProcName_Value.Name = "Label_ProcName_Value"
        Me.Label_ProcName_Value.Size = New System.Drawing.Size(16, 13)
        Me.Label_ProcName_Value.TabIndex = 12
        Me.Label_ProcName_Value.Text = "..."
        '
        'Label_ProcName
        '
        Me.Label_ProcName.AutoSize = True
        Me.Label_ProcName.Location = New System.Drawing.Point(12, 9)
        Me.Label_ProcName.Name = "Label_ProcName"
        Me.Label_ProcName.Size = New System.Drawing.Size(79, 13)
        Me.Label_ProcName.TabIndex = 11
        Me.Label_ProcName.Text = "Process Name:"
        '
        'NumericUpDown_Refresh
        '
        Me.NumericUpDown_Refresh.Location = New System.Drawing.Point(106, 185)
        Me.NumericUpDown_Refresh.Maximum = New Decimal(New Integer() {1000, 0, 0, 0})
        Me.NumericUpDown_Refresh.Minimum = New Decimal(New Integer() {20, 0, 0, 0})
        Me.NumericUpDown_Refresh.Name = "NumericUpDown_Refresh"
        Me.NumericUpDown_Refresh.Size = New System.Drawing.Size(56, 20)
        Me.NumericUpDown_Refresh.TabIndex = 13
        Me.NumericUpDown_Refresh.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'Label_refresh
        '
        Me.Label_refresh.AutoSize = True
        Me.Label_refresh.Location = New System.Drawing.Point(15, 189)
        Me.Label_refresh.Name = "Label_refresh"
        Me.Label_refresh.Size = New System.Drawing.Size(85, 13)
        Me.Label_refresh.TabIndex = 14
        Me.Label_refresh.Text = "Refresh Interval:"
        '
        'Label_refresh_ms
        '
        Me.Label_refresh_ms.AutoSize = True
        Me.Label_refresh_ms.Location = New System.Drawing.Point(168, 189)
        Me.Label_refresh_ms.Name = "Label_refresh_ms"
        Me.Label_refresh_ms.Size = New System.Drawing.Size(23, 13)
        Me.Label_refresh_ms.TabIndex = 15
        Me.Label_refresh_ms.Text = "ms."
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(404, 214)
        Me.Controls.Add(Me.Label_refresh_ms)
        Me.Controls.Add(Me.Label_refresh)
        Me.Controls.Add(Me.NumericUpDown_Refresh)
        Me.Controls.Add(Me.Label_ProcName_Value)
        Me.Controls.Add(Me.Label_ProcName)
        Me.Controls.Add(Me.Label_PID_Value)
        Me.Controls.Add(Me.Label_PID)
        Me.Controls.Add(Me.CheckBox_ChildWindows)
        Me.Controls.Add(Me.Label_Caption_Value)
        Me.Controls.Add(Me.Label_Caption)
        Me.Controls.Add(Me.Label_CoordsScreen_Value)
        Me.Controls.Add(Me.Label_CoordsScreen)
        Me.Controls.Add(Me.Label_CoordsRelative_Value)
        Me.Controls.Add(Me.Label_CoordsRelative)
        Me.Controls.Add(Me.Label_Hwnd_Value)
        Me.Controls.Add(Me.Label_Hwnd)
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Mouse Point Viewer - By Elektro"
        CType(Me.NumericUpDown_Refresh, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label_Hwnd As System.Windows.Forms.Label
    Friend WithEvents Label_Hwnd_Value As System.Windows.Forms.Label
    Friend WithEvents Label_CoordsRelative As System.Windows.Forms.Label
    Friend WithEvents Label_CoordsRelative_Value As System.Windows.Forms.Label
    Friend WithEvents Label_CoordsScreen_Value As System.Windows.Forms.Label
    Friend WithEvents Label_CoordsScreen As System.Windows.Forms.Label
    Friend WithEvents Label_Caption_Value As System.Windows.Forms.Label
    Friend WithEvents Label_Caption As System.Windows.Forms.Label
    Friend WithEvents CheckBox_ChildWindows As System.Windows.Forms.CheckBox
    Friend WithEvents Label_PID_Value As System.Windows.Forms.Label
    Friend WithEvents Label_PID As System.Windows.Forms.Label
    Friend WithEvents Label_ProcName_Value As System.Windows.Forms.Label
    Friend WithEvents Label_ProcName As System.Windows.Forms.Label
    Friend WithEvents NumericUpDown_Refresh As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label_refresh As System.Windows.Forms.Label
    Friend WithEvents Label_refresh_ms As System.Windows.Forms.Label

End Class