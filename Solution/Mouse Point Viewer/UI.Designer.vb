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
        Me.CheckBox_ChildWindows = New System.Windows.Forms.CheckBox()
        Me.NumericUpDown_Refresh = New System.Windows.Forms.NumericUpDown()
        Me.Label_refresh = New System.Windows.Forms.Label()
        Me.Label_refresh_ms = New System.Windows.Forms.Label()
        Me.PropertyGrid1 = New System.Windows.Forms.PropertyGrid()
        Me.RefreshTimer = New System.Windows.Forms.Timer(Me.components)
        Me.Label1 = New System.Windows.Forms.Label()
        Me.LinkLabel1 = New System.Windows.Forms.LinkLabel()
        Me.CheckBox_TopMost = New System.Windows.Forms.CheckBox()
        CType(Me.NumericUpDown_Refresh, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'CheckBox_ChildWindows
        '
        Me.CheckBox_ChildWindows.AutoSize = True
        Me.CheckBox_ChildWindows.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_ChildWindows.Location = New System.Drawing.Point(13, 200)
        Me.CheckBox_ChildWindows.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox_ChildWindows.Name = "CheckBox_ChildWindows"
        Me.CheckBox_ChildWindows.Size = New System.Drawing.Size(143, 19)
        Me.CheckBox_ChildWindows.TabIndex = 8
        Me.CheckBox_ChildWindows.Text = "Ignore Child Windows"
        Me.CheckBox_ChildWindows.UseVisualStyleBackColor = True
        '
        'NumericUpDown_Refresh
        '
        Me.NumericUpDown_Refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.NumericUpDown_Refresh.Location = New System.Drawing.Point(308, 199)
        Me.NumericUpDown_Refresh.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.NumericUpDown_Refresh.Maximum = New Decimal(New Integer() {5000, 0, 0, 0})
        Me.NumericUpDown_Refresh.Minimum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.NumericUpDown_Refresh.Name = "NumericUpDown_Refresh"
        Me.NumericUpDown_Refresh.Size = New System.Drawing.Size(53, 23)
        Me.NumericUpDown_Refresh.TabIndex = 13
        Me.NumericUpDown_Refresh.Value = New Decimal(New Integer() {100, 0, 0, 0})
        '
        'Label_refresh
        '
        Me.Label_refresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label_refresh.AutoSize = True
        Me.Label_refresh.Location = New System.Drawing.Point(225, 204)
        Me.Label_refresh.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label_refresh.Name = "Label_refresh"
        Me.Label_refresh.Size = New System.Drawing.Size(75, 15)
        Me.Label_refresh.TabIndex = 14
        Me.Label_refresh.Text = "Refresh Rate:"
        '
        'Label_refresh_ms
        '
        Me.Label_refresh_ms.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label_refresh_ms.AutoSize = True
        Me.Label_refresh_ms.Location = New System.Drawing.Point(369, 204)
        Me.Label_refresh_ms.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label_refresh_ms.Name = "Label_refresh_ms"
        Me.Label_refresh_ms.Size = New System.Drawing.Size(23, 15)
        Me.Label_refresh_ms.TabIndex = 15
        Me.Label_refresh_ms.Text = "ms"
        '
        'PropertyGrid1
        '
        Me.PropertyGrid1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PropertyGrid1.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.PropertyGrid1.HelpVisible = False
        Me.PropertyGrid1.Location = New System.Drawing.Point(13, 48)
        Me.PropertyGrid1.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.PropertyGrid1.Name = "PropertyGrid1"
        Me.PropertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort
        Me.PropertyGrid1.Size = New System.Drawing.Size(379, 146)
        Me.PropertyGrid1.TabIndex = 16
        Me.PropertyGrid1.ToolbarVisible = False
        '
        'RefreshTimer
        '
        Me.RefreshTimer.Enabled = True
        '
        'Label1
        '
        Me.Label1.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, CType((System.Drawing.FontStyle.Bold Or System.Drawing.FontStyle.Italic), System.Drawing.FontStyle), System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(12, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(266, 34)
        Me.Label1.TabIndex = 17
        Me.Label1.Text = "Press Ctrl+C while this window is focused to copy the values to the Windows clipb" &
    "oard."
        '
        'LinkLabel1
        '
        Me.LinkLabel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LinkLabel1.AutoSize = True
        Me.LinkLabel1.Location = New System.Drawing.Point(332, 28)
        Me.LinkLabel1.Name = "LinkLabel1"
        Me.LinkLabel1.Size = New System.Drawing.Size(60, 15)
        Me.LinkLabel1.TabIndex = 18
        Me.LinkLabel1.TabStop = True
        Me.LinkLabel1.Text = "🌐 GitHub"
        '
        'CheckBox_TopMost
        '
        Me.CheckBox_TopMost.AutoSize = True
        Me.CheckBox_TopMost.ForeColor = System.Drawing.SystemColors.ControlText
        Me.CheckBox_TopMost.Location = New System.Drawing.Point(13, 225)
        Me.CheckBox_TopMost.Margin = New System.Windows.Forms.Padding(4, 3, 4, 3)
        Me.CheckBox_TopMost.Name = "CheckBox_TopMost"
        Me.CheckBox_TopMost.Size = New System.Drawing.Size(75, 19)
        Me.CheckBox_TopMost.TabIndex = 19
        Me.CheckBox_TopMost.Text = "Top Most"
        Me.CheckBox_TopMost.UseVisualStyleBackColor = True
        '
        'Main
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Window
        Me.ClientSize = New System.Drawing.Size(404, 249)
        Me.Controls.Add(Me.CheckBox_TopMost)
        Me.Controls.Add(Me.LinkLabel1)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.PropertyGrid1)
        Me.Controls.Add(Me.Label_refresh_ms)
        Me.Controls.Add(Me.Label_refresh)
        Me.Controls.Add(Me.NumericUpDown_Refresh)
        Me.Controls.Add(Me.CheckBox_ChildWindows)
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
    Friend WithEvents CheckBox_ChildWindows As System.Windows.Forms.CheckBox
    Friend WithEvents NumericUpDown_Refresh As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label_refresh As System.Windows.Forms.Label
    Friend WithEvents Label_refresh_ms As System.Windows.Forms.Label
    Friend WithEvents PropertyGrid1 As PropertyGrid
    Friend WithEvents RefreshTimer As Windows.Forms.Timer
    Friend WithEvents Label1 As Label
    Friend WithEvents LinkLabel1 As LinkLabel
    Friend WithEvents CheckBox_TopMost As CheckBox
End Class