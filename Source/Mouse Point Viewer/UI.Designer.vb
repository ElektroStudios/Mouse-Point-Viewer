<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Main
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Main))
        Me.Label_refresh_ms = New System.Windows.Forms.Label()
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.RefreshTimer = New System.Windows.Forms.Timer(Me.components)
        Me.Label_Hotkeys = New System.Windows.Forms.Label()
        Me.CheckBox_IgnoreChildWindows = New System.Windows.Forms.CheckBox()
        Me.NumericUpDown_Refresh = New System.Windows.Forms.NumericUpDown()
        Me.Label_refresh = New System.Windows.Forms.Label()
        Me.CheckBox_TopMost = New System.Windows.Forms.CheckBox()
        Me.CheckBox_IgnoreLayeredWindows = New System.Windows.Forms.CheckBox()
        Me.CheckBox_IgnoreTransparentWindows = New System.Windows.Forms.CheckBox()
        Me.CheckBox_Magnifier = New System.Windows.Forms.CheckBox()
        Me.Button_ViewMode = New System.Windows.Forms.Button()
        Me.Button_Theme = New System.Windows.Forms.Button()
        Me.Button_GitHub = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.StatusTimer = New System.Windows.Forms.Timer(Me.components)
        CType(Me.NumericUpDown_Refresh, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label_refresh_ms
        '
        Me.Label_refresh_ms.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label_refresh_ms.AutoSize = True
        Me.Label_refresh_ms.Location = New System.Drawing.Point(369, 195)
        Me.Label_refresh_ms.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label_refresh_ms.Name = "Label_refresh_ms"
        Me.Label_refresh_ms.Size = New System.Drawing.Size(23, 15)
        Me.Label_refresh_ms.TabIndex = 11
        Me.Label_refresh_ms.Text = "ms"
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PropertyGrid1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PropertyGrid1.HelpVisible = False
        Me.PropertyGrid1.Location = New System.Drawing.Point(13, 48)
        Me.PropertyGrid1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGrid1.Size = New System.Drawing.Size(379, 139)
        Me.PropertyGrid1.TabIndex = 5
        Me.PropertyGrid1.TabStop = False
        Me.PropertyGrid1.ToolbarVisible = False
        '
        'Label_Hotkeys
        '
        Me.Label_Hotkeys.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label_Hotkeys.AutoEllipsis = True
        Me.Label_Hotkeys.Font = New System.Drawing.Font("Segoe UI", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Hotkeys.Location = New System.Drawing.Point(12, 9)
        Me.Label_Hotkeys.Name = "Label_Hotkeys"
        Me.Label_Hotkeys.Size = New System.Drawing.Size(256, 34)
        Me.Label_Hotkeys.TabIndex = 0
        Me.Label_Hotkeys.Text = "Ctrl+Alt+C: copy the values to the clipboard." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Ctrl+Alt+M: show or hide the magnifier."
        Me.Label_Hotkeys.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CheckBox_IgnoreChildWindows
        '
        Me.CheckBox_IgnoreChildWindows.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckBox_IgnoreChildWindows.AutoSize = True
        Me.CheckBox_IgnoreChildWindows.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_IgnoreChildWindows.Location = New System.Drawing.Point(13, 194)
        Me.CheckBox_IgnoreChildWindows.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox_IgnoreChildWindows.Name = "CheckBox_IgnoreChildWindows"
        Me.CheckBox_IgnoreChildWindows.Size = New System.Drawing.Size(139, 19)
        Me.CheckBox_IgnoreChildWindows.TabIndex = 6
        Me.CheckBox_IgnoreChildWindows.Text = "Top-level windows only"
        Me.CheckBox_IgnoreChildWindows.UseVisualStyleBackColor = True
        '
        'NumericUpDown_Refresh
        '
        Me.NumericUpDown_Refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.NumericUpDown_Refresh.Location = New System.Drawing.Point(311, 192)
        Me.NumericUpDown_Refresh.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.NumericUpDown_Refresh.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.NumericUpDown_Refresh.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown_Refresh.Name = "NumericUpDown_Refresh"
        Me.NumericUpDown_Refresh.Size = New System.Drawing.Size(53, 23)
        Me.NumericUpDown_Refresh.TabIndex = 10
        Me.NumericUpDown_Refresh.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'Label_refresh
        '
        Me.Label_refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label_refresh.AutoSize = True
        Me.Label_refresh.Location = New System.Drawing.Point(232, 196)
        Me.Label_refresh.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label_refresh.Name = "Label_refresh"
        Me.Label_refresh.Size = New System.Drawing.Size(75, 15)
        Me.Label_refresh.TabIndex = 9
        Me.Label_refresh.Text = "Refresh Rate:"
        '
        'CheckBox_TopMost
        '
        Me.CheckBox_TopMost.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckBox_TopMost.AutoSize = True
        Me.CheckBox_TopMost.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_TopMost.Location = New System.Drawing.Point(317, 236)
        Me.CheckBox_TopMost.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox_TopMost.Name = "CheckBox_TopMost"
        Me.CheckBox_TopMost.Size = New System.Drawing.Size(75, 19)
        Me.CheckBox_TopMost.TabIndex = 12
        Me.CheckBox_TopMost.Text = "Always on top"
        Me.CheckBox_TopMost.UseVisualStyleBackColor = True
        '
        'CheckBox_IgnoreLayeredWindows
        '
        Me.CheckBox_IgnoreLayeredWindows.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckBox_IgnoreLayeredWindows.AutoSize = True
        Me.CheckBox_IgnoreLayeredWindows.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_IgnoreLayeredWindows.Location = New System.Drawing.Point(13, 215)
        Me.CheckBox_IgnoreLayeredWindows.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox_IgnoreLayeredWindows.Name = "CheckBox_IgnoreLayeredWindows"
        Me.CheckBox_IgnoreLayeredWindows.Size = New System.Drawing.Size(151, 19)
        Me.CheckBox_IgnoreLayeredWindows.TabIndex = 7
        Me.CheckBox_IgnoreLayeredWindows.Text = "Ignore layered windows"
        Me.CheckBox_IgnoreLayeredWindows.UseVisualStyleBackColor = True
        '
        'CheckBox_IgnoreTransparentWindows
        '
        Me.CheckBox_IgnoreTransparentWindows.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.CheckBox_IgnoreTransparentWindows.AutoSize = True
        Me.CheckBox_IgnoreTransparentWindows.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_IgnoreTransparentWindows.Location = New System.Drawing.Point(13, 236)
        Me.CheckBox_IgnoreTransparentWindows.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox_IgnoreTransparentWindows.Name = "CheckBox_IgnoreTransparentWindows"
        Me.CheckBox_IgnoreTransparentWindows.Size = New System.Drawing.Size(173, 19)
        Me.CheckBox_IgnoreTransparentWindows.TabIndex = 8
        Me.CheckBox_IgnoreTransparentWindows.Text = "Ignore invisible windows"
        Me.CheckBox_IgnoreTransparentWindows.UseVisualStyleBackColor = True
        '
        'CheckBox_Magnifier
        '
        Me.CheckBox_Magnifier.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.CheckBox_Magnifier.Appearance = System.Windows.Forms.Appearance.Button
        Me.CheckBox_Magnifier.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.CheckBox_Magnifier.Location = New System.Drawing.Point(274, 12)
        Me.CheckBox_Magnifier.Margin = New System.Windows.Forms.Padding(0)
        Me.CheckBox_Magnifier.Name = "CheckBox_Magnifier"
        Me.CheckBox_Magnifier.Size = New System.Drawing.Size(28, 28)
        Me.CheckBox_Magnifier.TabIndex = 1
        Me.CheckBox_Magnifier.TabStop = False
        Me.CheckBox_Magnifier.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.CheckBox_Magnifier.UseVisualStyleBackColor = False
        '
        'Button_ViewMode
        '
        Me.Button_ViewMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button_ViewMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_ViewMode.Location = New System.Drawing.Point(304, 12)
        Me.Button_ViewMode.Margin = New System.Windows.Forms.Padding(0)
        Me.Button_ViewMode.Name = "Button_ViewMode"
        Me.Button_ViewMode.Size = New System.Drawing.Size(28, 28)
        Me.Button_ViewMode.TabIndex = 2
        Me.Button_ViewMode.TabStop = False
        Me.Button_ViewMode.UseVisualStyleBackColor = False
        '
        'Button_Theme
        '
        Me.Button_Theme.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button_Theme.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_Theme.Location = New System.Drawing.Point(334, 12)
        Me.Button_Theme.Margin = New System.Windows.Forms.Padding(0)
        Me.Button_Theme.Name = "Button_Theme"
        Me.Button_Theme.Size = New System.Drawing.Size(28, 28)
        Me.Button_Theme.TabIndex = 3
        Me.Button_Theme.TabStop = False
        Me.Button_Theme.UseVisualStyleBackColor = False
        '
        'Button_GitHub
        '
        Me.Button_GitHub.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button_GitHub.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Button_GitHub.Location = New System.Drawing.Point(364, 12)
        Me.Button_GitHub.Margin = New System.Windows.Forms.Padding(0)
        Me.Button_GitHub.Name = "Button_GitHub"
        Me.Button_GitHub.Size = New System.Drawing.Size(28, 28)
        Me.Button_GitHub.TabIndex = 4
        Me.Button_GitHub.TabStop = False
        Me.Button_GitHub.UseVisualStyleBackColor = False
        '
        'StatusTimer
        '
        Me.StatusTimer.Interval = 2000
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(404, 261)
        Me.Controls.Add(Me.Button_GitHub)
        Me.Controls.Add(Me.Button_Theme)
        Me.Controls.Add(Me.Button_ViewMode)
        Me.Controls.Add(Me.CheckBox_Magnifier)
        Me.Controls.Add(Me.CheckBox_IgnoreTransparentWindows)
        Me.Controls.Add(Me.CheckBox_IgnoreLayeredWindows)
        Me.Controls.Add(Me.CheckBox_TopMost)
        Me.Controls.Add(Me.Label_Hotkeys)
        Me.Controls.Add(Me.PropertyGrid1)
        Me.Controls.Add(Me.Label_refresh_ms)
        Me.Controls.Add(Me.Label_refresh)
        Me.Controls.Add(Me.NumericUpDown_Refresh)
        Me.Controls.Add(Me.CheckBox_IgnoreChildWindows)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ForeColor = System.Drawing.SystemColors.ControlText
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.MaximizeBox = False
        Me.Name = "Main"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Mouse Point Viewer - By ElektroStudios"
        CType(Me.NumericUpDown_Refresh, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label_refresh_ms As System.Windows.Forms.Label
    Friend WithEvents PropertyGrid1 As PropertyGrid
    Friend WithEvents RefreshTimer As Windows.Forms.Timer
    Friend WithEvents Label_Hotkeys As Label
    Friend WithEvents CheckBox_IgnoreChildWindows As CheckBox
    Friend WithEvents NumericUpDown_Refresh As NumericUpDown
    Friend WithEvents Label_refresh As Label
    Friend WithEvents CheckBox_TopMost As CheckBox
    Friend WithEvents CheckBox_IgnoreLayeredWindows As CheckBox
    Friend WithEvents CheckBox_IgnoreTransparentWindows As CheckBox
    Friend WithEvents CheckBox_Magnifier As CheckBox
    Friend WithEvents Button_ViewMode As Button
    Friend WithEvents Button_Theme As Button
    Friend WithEvents Button_GitHub As Button
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents StatusTimer As Windows.Forms.Timer
End Class
