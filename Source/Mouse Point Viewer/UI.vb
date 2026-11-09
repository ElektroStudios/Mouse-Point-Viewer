#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Globalization
Imports System.Runtime.InteropServices
Imports System.Text

Imports DevCase.Extensions.PropertyGridExtensions

Imports Win32

#End Region

''' <summary>
''' The main window of the application.
''' </summary>
Partial Friend NotInheritable Class Main : Inherits Form

#Region " Constants "

    ''' <summary>
    ''' The X position of the splitter control of the <see cref="PropertyGrid1"/>.
    ''' </summary>
    Private Const PropertyGridSplitterPosition As Integer = 140

    ''' <summary>
    ''' The height factor of the client area in expanded mode, relative to the compact mode.
    ''' <para></para>
    ''' It is enough to display all the top-level items of the <see cref="PropertyGrid1"/> expanded (one level).
    ''' </summary>
    Private Const ExpandedHeightFactor As Double = 2.25

    ''' <summary>
    ''' The URL of the GitHub repository of the application.
    ''' </summary>
    Private Const GitHubUrl As String = "https://github.com/ElektroStudios/Mouse-Point-Viewer"

    ''' <summary>
    ''' The modifier keys of all the global hotkeys.
    ''' </summary>
    Friend Const HotkeyModifierKeys As HotkeyModifiers = HotkeyModifiers.Control Or HotkeyModifiers.Alt

    ''' <summary>
    ''' The key of the global hotkey that copies the values to the clipboard (with <see cref="HotkeyModifierKeys"/>).
    ''' </summary>
    Friend Const CopyHotkeyKey As Keys = Keys.C

    ''' <summary>
    ''' The key of the global hotkey that shows or hides the magnifier (with <see cref="HotkeyModifierKeys"/>).
    ''' </summary>
    Friend Const MagnifierHotkeyKey As Keys = Keys.M

#End Region

#Region " Shared Fields "

    ''' <summary>
    ''' When <see langword="True"/>, only top-level windows are retrieved under the mouse cursor.
    ''' </summary>
    Friend Shared ignoreChildWindows As Boolean

    ''' <summary>
    ''' When <see langword="True"/>, windows with the <c>WS_EX_LAYERED</c> extended style are ignored.
    ''' </summary>
    Friend Shared ignoreLayeredWindows As Boolean

    ''' <summary>
    ''' When <see langword="True"/>, windows with the <c>WS_EX_TRANSPARENT</c> extended style,
    ''' and windows that are not visible at the pixel under the mouse cursor, are ignored.
    ''' </summary>
    Friend Shared ignoreTransparentWindows As Boolean

#End Region

#Region " Private Fields "

    ''' <summary>
    ''' The information displayed in the <see cref="PropertyGrid1"/>.
    ''' </summary>
    Private ReadOnly AllInfo As New AllInfoContainer()

    ''' <summary>
    ''' The current visual theme.
    ''' </summary>
    Private currentTheme As AppTheme

    ''' <summary>
    ''' Whether the expanded view mode is active (<see langword="False"/> for the compact view mode).
    ''' </summary>
    Private expandedMode As Boolean

    ''' <summary>
    ''' The size of the client area in compact mode.
    ''' </summary>
    Private compactClientSize As Size

    ''' <summary>
    ''' The font used for the glyphs of the icon buttons, or <see langword="Nothing"/> if the icon font is not installed.
    ''' </summary>
    Private ReadOnly iconFont As Font

    ''' <summary>
    ''' The GitHub logo for the light theme, resized to fit the GitHub button.
    ''' </summary>
    Private gitHubImageForLightTheme As Bitmap

    ''' <summary>
    ''' The GitHub logo for the dark theme, resized to fit the GitHub button.
    ''' </summary>
    Private gitHubImageForDarkTheme As Bitmap

    ''' <summary>
    ''' The global hotkeys manager, or <see langword="Nothing"/> if the hotkeys are not registered yet.
    ''' </summary>
    Private hotkeyManager As GlobalHotkeyManager

    ''' <summary>
    ''' The identifier of the global hotkey that copies the values to the clipboard, or 0 if it is not registered.
    ''' </summary>
    Private copyHotkeyId As Integer

    ''' <summary>
    ''' The identifier of the global hotkey that shows or hides the magnifier, or 0 if it is not registered.
    ''' </summary>
    Private magnifierHotkeyId As Integer

    ''' <summary>
    ''' The identifiers of the global hotkeys that zoom in the magnifier, registered only while the magnifier is visible.
    ''' </summary>
    Private ReadOnly zoomInHotkeyIds As New List(Of Integer)

    ''' <summary>
    ''' The identifiers of the global hotkeys that zoom out the magnifier, registered only while the magnifier is visible.
    ''' </summary>
    Private ReadOnly zoomOutHotkeyIds As New List(Of Integer)

    ''' <summary>
    ''' The identifier of the global hotkey (Escape) that closes the magnifier,
    ''' registered only while the magnifier is visible, or 0 if it is not registered.
    ''' </summary>
    Private closeMagnifierHotkeyId As Integer

    ''' <summary>
    ''' The magnifier window, or <see langword="Nothing"/> if it has not been shown yet.
    ''' </summary>
    Private magnifier As MagnifierWindow

    ''' <summary>
    ''' The hint text of the global hotkeys, shown in the <see cref="Label_Hotkeys"/>.
    ''' </summary>
    Private hotkeysHintText As String

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the current visual theme.
    ''' </summary>
    Friend ReadOnly Property Theme As AppTheme
        Get
            Return Me.currentTheme
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the expanded view mode is active.
    ''' </summary>
    Friend ReadOnly Property IsExpandedMode As Boolean
        Get
            Return Me.expandedMode
        End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the magnifier is visible.
    ''' </summary>
    Friend ReadOnly Property IsMagnifierVisible As Boolean
        Get
            Return Me.magnifier IsNot Nothing AndAlso Me.magnifier.Visible
        End Get
    End Property

    ''' <summary>
    ''' Gets the size of the client area in compact mode.
    ''' </summary>
    Friend ReadOnly Property CompactModeClientSize As Size
        Get
            Return Me.compactClientSize
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="Main"/> class,
    ''' applying the visual theme and the view mode stored in the user settings.
    ''' <para></para>
    ''' The constructor must be public for the default form instance (<c>My.Forms.Main</c>) used as the startup form.
    ''' </summary>
    Public Sub New()

        Me.InitializeComponent()

        Me.Label_Hotkeys.Tag = ThemeColorRole.Secondary
        Me.Label_Separator.Tag = ThemeColorRole.Separator
        Me.compactClientSize = Me.ClientSize
        Me.hotkeysHintText = Me.Label_Hotkeys.Text

        If ImageHelper.IsFontInstalled(ImageHelper.IconFontName) Then
            Me.iconFont = New Font(ImageHelper.IconFontName, 10.5!, FontStyle.Regular, GraphicsUnit.Point)
            For Each btn As ButtonBase In {DirectCast(Me.CheckBox_Magnifier, ButtonBase), Me.Button_ViewMode, Me.Button_Theme}
                btn.Font = Me.iconFont
            Next
        End If

        Me.currentTheme = ThemeManager.ResolveTheme(My.Settings.Theme, ThemeManager.IsSystemThemeDark())
        Me.expandedMode = My.Settings.ExpandedMode
        Me.ApplyThemeColors()
        Me.UpdateViewModeButton()
        Me.ToolTip1.SetToolTip(Me.Button_GitHub, "Open the GitHub repository")
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Changes the visual theme, without saving it in the user settings.
    ''' </summary>
    '''
    ''' <param name="theme">
    ''' The new visual theme.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub SetTheme(theme As AppTheme)

        ' Validates the value.
        Dim palette As ThemePalette = ThemePalette.FromTheme(theme)

        Me.currentTheme = theme
        Me.ApplyThemeColors()

        If Me.IsHandleCreated Then
            ThemeManager.SetDarkTitleBar(Me.Handle, theme = AppTheme.Dark)
            Me.ApplyNativeControlsTheme()
        End If

        If Me.magnifier IsNot Nothing Then
            Me.magnifier.Palette = palette
        End If

        Me.Invalidate(invalidateChildren:=True)
    End Sub

    ''' <summary>
    ''' Changes the view mode (compact or expanded), without saving it in the user settings.
    ''' <para></para>
    ''' The expanded mode makes the window taller and expands the top-level items of the property grid.
    ''' </summary>
    '''
    ''' <param name="expanded">
    ''' <see langword="True"/> for the expanded mode, <see langword="False"/> for the compact mode.
    ''' </param>
    <DebuggerStepThrough>
    Friend Sub SetViewMode(expanded As Boolean)

        Me.expandedMode = expanded
        Me.UpdateViewModeButton()
        Me.ApplyViewModeSize()

        If Me.PropertyGrid1.IsInitialized() Then
            Me.PropertyGrid1.SetTopLevelItemsExpanded(expanded)
        End If
    End Sub

    ''' <summary>
    ''' Shows the magnifier if it is hidden, or hides it if it is visible.
    ''' </summary>
    <DebuggerStepThrough>
    Friend Sub ToggleMagnifier()

        ' The CheckedChanged event handler shows or hides the magnifier.
        Me.CheckBox_Magnifier.Checked = Not Me.CheckBox_Magnifier.Checked
    End Sub

    ''' <summary>
    ''' Returns the size of the client area in expanded mode.
    ''' </summary>
    '''
    ''' <param name="compactSize">
    ''' The size of the client area in compact mode.
    ''' </param>
    '''
    ''' <returns>
    ''' The size of the client area in expanded mode.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function GetExpandedClientSize(compactSize As Size) As Size

        Return New Size(compactSize.Width, CInt(Math.Round(compactSize.Height * ExpandedHeightFactor)))
    End Function

    ''' <summary>
    ''' Tries to parse the refresh rate typed in the refresh rate control.
    ''' </summary>
    '''
    ''' <param name="text">
    ''' The typed text.
    ''' </param>
    '''
    ''' <param name="minimum">
    ''' The minimum allowed value.
    ''' </param>
    '''
    ''' <param name="maximum">
    ''' The maximum allowed value.
    ''' </param>
    '''
    ''' <param name="refInterval">
    ''' Receives the parsed interval, in milliseconds, if the text is valid.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the text is an integer within the allowed range, <see langword="False"/> otherwise.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function TryParseRefreshInterval(text As String, minimum As Decimal, maximum As Decimal, ByRef refInterval As Integer) As Boolean

        Dim value As Integer
        If Integer.TryParse(text, NumberStyles.Integer, CultureInfo.CurrentCulture, value) AndAlso
            value >= minimum AndAlso
            value <= maximum Then

            refInterval = value
            Return True
        End If

        Return False
    End Function

#End Region

#Region " Protected Methods "

    ''' <summary>
    ''' Raises the <see cref="Control.HandleCreated"/> event, applying the dark title bar when the dark theme is active.
    ''' <para></para>
    ''' The title bar attribute must be applied here: it has no effect when it is applied in the <see cref="Form.Load"/> event.
    ''' </summary>
    '''
    ''' <param name="e">The event data.</param>
    Protected Overrides Sub OnHandleCreated(e As EventArgs)

        MyBase.OnHandleCreated(e)
        ThemeManager.SetDarkTitleBar(Me.Handle, Me.currentTheme = AppTheme.Dark)
    End Sub

#End Region

#Region " Event Handlers "

    <DebuggerStepThrough>
    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load

#If DEBUG Then ' CREATE A LAYERED OR TRANSPARENT FORM TO TEST FUNCTIONALITIES.

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

        ' Add something visible inside to tell it apart.
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

        ' The user settings are stored per application version: migrate them from the previous version.
        If My.Settings.UpgradeRequired Then
            My.Settings.Upgrade()
            My.Settings.UpgradeRequired = False
            My.Settings.Save()

            Me.expandedMode = My.Settings.ExpandedMode
            Me.UpdateViewModeButton()
            Me.SetTheme(ThemeManager.ResolveTheme(My.Settings.Theme, ThemeManager.IsSystemThemeDark()))
        End If

        Me.Text = $"{My.Application.Info.Title} {My.Application.Info.Version.ToString(3)}"

        ' The client area may have been scaled (AutoScaleMode) after the constructor.
        Me.compactClientSize = Me.ClientSize
        Me.ApplyViewModeSize()

        Me.PropertyGrid1.SelectedObject = Me.AllInfo
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown

        Me.MinimumSize = Me.SizeFromClientSize(Me.compactClientSize)
        Me.PropertyGrid1.SetSplitterPosition(Main.PropertyGridSplitterPosition)
        Me.ApplyNativeControlsTheme()

        ' Populate the property grid immediately, instead of waiting for the first timer tick.
        Me.RefreshInfo()
        If Me.expandedMode Then
            Me.PropertyGrid1.SetTopLevelItemsExpanded(True)
        End If
        Me.RefreshTimer.Start()

        ' The hotkeys are registered once the information is populated, so the copy hotkey always has values to copy.
        Me.RegisterGlobalHotkeys()
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Me.RefreshTimer.Stop()
        Me.StatusTimer.Stop()
        Me.HideMagnifier()
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed

        Me.hotkeyManager?.Dispose()
        Me.hotkeyManager = Nothing
        Me.Button_GitHub.Image = Nothing
        Me.gitHubImageForLightTheme?.Dispose()
        Me.gitHubImageForDarkTheme?.Dispose()
        Me.iconFont?.Dispose()
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize

        If Me.PropertyGrid1.IsInitialized Then
            Me.PropertyGrid1.SetSplitterPosition(Main.PropertyGridSplitterPosition)
        End If
    End Sub

    <DebuggerStepThrough>
    Private Sub MainForm_KeyDown(sender As Object, e As KeyEventArgs) Handles Me.KeyDown

        '' Ctrl+C is kept as a local shortcut (when this window is focused) in addition to the global hotkey.
        'If e.Control AndAlso Not e.Alt AndAlso e.KeyCode = Keys.C Then
        '    e.Handled = True
        '    Me.CopyInfoToClipboard()
        'End If
    End Sub

    <DebuggerStepThrough>
    Private Sub HotkeyManager_HotkeyPressed(sender As Object, e As HotkeyPressedEventArgs)

        Select Case True
            Case e.Id = Me.copyHotkeyId
                Me.CopyInfoToClipboard()

            Case e.Id = Me.magnifierHotkeyId
                Me.ToggleMagnifier()

            Case Me.zoomInHotkeyIds.Contains(e.Id)
                Me.magnifier?.ZoomIn()

            Case Me.zoomOutHotkeyIds.Contains(e.Id)
                Me.magnifier?.ZoomOut()

            Case e.Id = Me.closeMagnifierHotkeyId
                Me.CheckBox_Magnifier.Checked = False

        End Select
    End Sub

    <DebuggerStepThrough>
    Private Sub Button_GitHub_Click(sender As Object, e As EventArgs) Handles Button_GitHub.Click

        Try
            Process.Start(Main.GitHubUrl)?.Dispose()

        Catch ex As Exception
            MessageBox.Show(Me, $"ERROR: {ex.Message}", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)

        End Try
    End Sub

    <DebuggerStepThrough>
    Private Sub Button_Theme_Click(sender As Object, e As EventArgs) Handles Button_Theme.Click

        Dim newTheme As AppTheme = If(Me.currentTheme = AppTheme.Dark, AppTheme.Light, AppTheme.Dark)
        Me.SetTheme(newTheme)

        My.Settings.Theme = ThemeManager.ToSettingValue(newTheme)
        My.Settings.Save()
    End Sub

    <DebuggerStepThrough>
    Private Sub Button_ViewMode_Click(sender As Object, e As EventArgs) Handles Button_ViewMode.Click

        Me.SetViewMode(Not Me.expandedMode)

        My.Settings.ExpandedMode = Me.expandedMode
        My.Settings.Save()
    End Sub

    <DebuggerStepThrough>
    Private Sub CheckBox_Magnifier_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox_Magnifier.CheckedChanged

        If Me.CheckBox_Magnifier.Checked Then
            Me.ShowMagnifier()
        Else
            Me.HideMagnifier()
        End If
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

        ' Note: the NumericUpDown clamps out of range values to its Minimum/Maximum when it validates the typed text,
        ' raising this event again with the clamped value.
        Dim nud As NumericUpDown = DirectCast(sender, NumericUpDown)

        Dim interval As Integer
        If Main.TryParseRefreshInterval(nud.Text, nud.Minimum, nud.Maximum, interval) Then
            Me.RefreshTimer.Interval = interval
        End If
    End Sub

    <DebuggerStepThrough>
    Private Sub RefreshTimer_Tick(sender As Object, e As EventArgs) Handles RefreshTimer.Tick

        Me.RefreshInfo()
    End Sub

    <DebuggerStepThrough>
    Private Sub StatusTimer_Tick(sender As Object, e As EventArgs) Handles StatusTimer.Tick

        Me.StatusTimer.Stop()
        Me.Label_Hotkeys.Text = Me.hotkeysHintText
        Me.Label_Hotkeys.ForeColor = ThemePalette.FromTheme(Me.currentTheme).SecondaryFore
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Applies the colors of the current theme to this window and its controls, and updates the theme-dependent icons.
    ''' </summary>
    Private Sub ApplyThemeColors()

        Dim palette As ThemePalette = ThemePalette.FromTheme(Me.currentTheme)
        ThemeManager.ApplyColors(Me, palette)

        If Me.StatusTimer.Enabled Then ' Keep the "copied" notification color.
            Me.Label_Hotkeys.ForeColor = palette.SuccessFore
        End If

        Me.UpdateThemeButton()
        Me.UpdateGitHubButton()
        Me.UpdateMagnifierButton()
    End Sub

    ''' <summary>
    ''' Applies the current theme to the native parts of the controls that are not themed through colors
    ''' (the scroll bars of the property grid). The control handles must be created.
    ''' </summary>
    Private Sub ApplyNativeControlsTheme()

        ThemeManager.SetDarkScrollBars(Me.PropertyGrid1.GetGridView().Handle, Me.currentTheme = AppTheme.Dark)
    End Sub

    ''' <summary>
    ''' Updates the glyph and the tool tip of the theme button.
    ''' </summary>
    Private Sub UpdateThemeButton()

        Dim isDark As Boolean = Me.currentTheme = AppTheme.Dark
        Me.Button_Theme.Text = If(Me.iconFont IsNot Nothing,
                                  If(isDark, ImageHelper.GlyphSun, ImageHelper.GlyphMoon),
                                  If(isDark, "L", "D"))
        Me.ToolTip1.SetToolTip(Me.Button_Theme, If(isDark, "Switch to the light theme", "Switch to the dark theme"))
    End Sub

    ''' <summary>
    ''' Updates the glyph and the tool tip of the view mode button.
    ''' </summary>
    Private Sub UpdateViewModeButton()

        Me.Button_ViewMode.Text = If(Me.iconFont IsNot Nothing,
                                     If(Me.expandedMode, ImageHelper.GlyphCollapse, ImageHelper.GlyphExpand),
                                     If(Me.expandedMode, "-", "+"))
        Me.ToolTip1.SetToolTip(Me.Button_ViewMode, If(Me.expandedMode, "Switch to compact mode", "Switch to expanded mode"))
    End Sub

    ''' <summary>
    ''' Updates the glyph and the tool tip of the magnifier button.
    ''' </summary>
    Private Sub UpdateMagnifierButton()

        Me.CheckBox_Magnifier.Text = If(Me.iconFont IsNot Nothing, ImageHelper.GlyphZoom, "Z")

        Dim tip As String = "Show or hide the magnifier"
        If Me.magnifierHotkeyId <> 0 Then
            tip &= $" ({GlobalHotkeyManager.FormatHotkey(HotkeyModifierKeys, MagnifierHotkeyKey)})"
        End If
        Me.ToolTip1.SetToolTip(Me.CheckBox_Magnifier, tip)
    End Sub

    ''' <summary>
    ''' Updates the image of the GitHub button according to the current theme.
    ''' </summary>
    Private Sub UpdateGitHubButton()

        If Me.gitHubImageForLightTheme Is Nothing Then
            Dim imageSize As Size = Me.LogicalToDeviceUnits(New Size(18, 18))
            Using black As Bitmap = My.Resources.GitHub_Invertocat_Black,
                  white As Bitmap = My.Resources.GitHub_Invertocat_White
                Me.gitHubImageForLightTheme = ImageHelper.ResizeImage(black, imageSize)
                Me.gitHubImageForDarkTheme = ImageHelper.ResizeImage(white, imageSize)
            End Using
        End If

        Me.Button_GitHub.Image = If(Me.currentTheme = AppTheme.Dark, Me.gitHubImageForDarkTheme, Me.gitHubImageForLightTheme)
    End Sub

    ''' <summary>
    ''' Resizes the window according to the current view mode, keeping it inside the working area of its screen.
    ''' </summary>
    Private Sub ApplyViewModeSize()

        Me.ClientSize = If(Me.expandedMode, Main.GetExpandedClientSize(Me.compactClientSize), Me.compactClientSize)

        If Me.IsHandleCreated Then
            Dim workingArea As Rectangle = Screen.FromControl(Me).WorkingArea
            If Me.Bottom > workingArea.Bottom Then
                Me.Top = Math.Max(workingArea.Top, workingArea.Bottom - Me.Height)
            End If
        End If
    End Sub

    ''' <summary>
    ''' Registers the global hotkeys, updates the hotkeys hint text,
    ''' and warns the user about the hotkeys that are already in use by another application.
    ''' </summary>
    Private Sub RegisterGlobalHotkeys()

        Me.hotkeyManager = New GlobalHotkeyManager()
        AddHandler Me.hotkeyManager.HotkeyPressed, AddressOf Me.HotkeyManager_HotkeyPressed

        Dim copyText As String = GlobalHotkeyManager.FormatHotkey(HotkeyModifierKeys, CopyHotkeyKey)
        Dim magnifierText As String = GlobalHotkeyManager.FormatHotkey(HotkeyModifierKeys, MagnifierHotkeyKey)
        Dim unavailable As New List(Of String)

        If Not Me.hotkeyManager.TryRegister(HotkeyModifierKeys Or HotkeyModifiers.NoRepeat, CopyHotkeyKey, Me.copyHotkeyId) Then
            unavailable.Add(copyText)
        End If
        If Not Me.hotkeyManager.TryRegister(HotkeyModifierKeys Or HotkeyModifiers.NoRepeat, MagnifierHotkeyKey, Me.magnifierHotkeyId) Then
            unavailable.Add(magnifierText)
        End If

        Me.hotkeysHintText = Main.BuildHotkeysHintText(Me.copyHotkeyId <> 0, Me.magnifierHotkeyId <> 0)
        If Not Me.StatusTimer.Enabled Then
            Me.Label_Hotkeys.Text = Me.hotkeysHintText
        End If
        Me.UpdateMagnifierButton()

        If unavailable.Count > 0 Then
            MessageBox.Show(Me,
                            $"The following global hotkeys could not be registered because they are in use by another application:{Environment.NewLine}{Environment.NewLine}" &
                            String.Join(Environment.NewLine, unavailable),
                            My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    ''' <summary>
    ''' Builds the hotkeys hint text shown in the <see cref="Label_Hotkeys"/>.
    ''' </summary>
    '''
    ''' <param name="copyHotkeyRegistered">
    ''' Whether the copy hotkey was registered.
    ''' </param>
    '''
    ''' <param name="magnifierHotkeyRegistered">
    ''' Whether the magnifier hotkey was registered.
    ''' </param>
    '''
    ''' <returns>
    ''' The hotkeys hint text (two lines).
    ''' </returns>
    Friend Shared Function BuildHotkeysHintText(copyHotkeyRegistered As Boolean, magnifierHotkeyRegistered As Boolean) As String

        Dim copyLine As String = If(copyHotkeyRegistered,
                                    $"{GlobalHotkeyManager.FormatHotkey(HotkeyModifierKeys, CopyHotkeyKey)}: copy the values to the clipboard.",
                                    "Ctrl+C (this window focused): copy the values.")

        Dim magnifierLine As String = If(magnifierHotkeyRegistered,
                                         $"{GlobalHotkeyManager.FormatHotkey(HotkeyModifierKeys, MagnifierHotkeyKey)}: show or hide the magnifier.",
                                         "Use the magnifier button to show or hide it.")

        Return $"{copyLine}{Environment.NewLine}{magnifierLine}"
    End Function

    ''' <summary>
    ''' Copies the current values to the clipboard, and notifies it in the <see cref="Label_Hotkeys"/> for a while.
    ''' </summary>
    Private Sub CopyInfoToClipboard()

        Try
            Clipboard.SetText(Me.AllInfo.ToString())

        Catch ex As ExternalException
            ' The clipboard is being used by another process.
            MessageBox.Show(Me, $"ERROR: {ex.Message}", My.Application.Info.Title, MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return

        End Try

        Me.Label_Hotkeys.Text = $"✔ Values copied to the clipboard.{Environment.NewLine}{Me.AllInfo.PixelColor} — {Me.AllInfo.Process}"
        Me.Label_Hotkeys.ForeColor = ThemePalette.FromTheme(Me.currentTheme).SuccessFore
        Me.StatusTimer.Stop()
        Me.StatusTimer.Start()
    End Sub

    ''' <summary>
    ''' Shows the magnifier, and registers its zoom and close hotkeys.
    ''' </summary>
    Private Sub ShowMagnifier()

        If Me.magnifier Is Nothing Then
            Me.magnifier = New MagnifierWindow()
            Me.components.Add(Me.magnifier) ' Disposed along with this window.
        End If
        Me.magnifier.Palette = ThemePalette.FromTheme(Me.currentTheme)
        Me.magnifier.HintLines = Me.RegisterMagnifierHotkeys()
        Me.magnifier.Show()
    End Sub

    ''' <summary>
    ''' Hides the magnifier, and unregisters its zoom and close hotkeys.
    ''' </summary>
    Private Sub HideMagnifier()

        Me.magnifier?.Hide()

        If Me.hotkeyManager Is Nothing Then
            Return
        End If

        Dim ids As New List(Of Integer)(Me.zoomInHotkeyIds)
        ids.AddRange(Me.zoomOutHotkeyIds)
        If Me.closeMagnifierHotkeyId <> 0 Then
            ids.Add(Me.closeMagnifierHotkeyId)
        End If

        For Each id As Integer In ids
            Me.hotkeyManager.Unregister(id)
        Next

        Me.zoomInHotkeyIds.Clear()
        Me.zoomOutHotkeyIds.Clear()
        Me.closeMagnifierHotkeyId = 0
    End Sub

    ''' <summary>
    ''' Registers the zoom and close hotkeys of the magnifier, which are only registered while the magnifier is visible
    ''' (so they do not steal those keys from other applications the rest of the time).
    ''' </summary>
    '''
    ''' <returns>
    ''' The hint lines for the hotkeys that could be registered.
    ''' </returns>
    Private Function RegisterMagnifierHotkeys() As String()

        Dim lines As New List(Of String)
        If Me.hotkeyManager Is Nothing Then
            Return lines.ToArray()
        End If

        Dim id As Integer
        For Each key As Keys In {Keys.Oemplus, Keys.Add}
            If Me.hotkeyManager.TryRegister(HotkeyModifierKeys, key, id) Then
                Me.zoomInHotkeyIds.Add(id)
            End If
        Next
        For Each key As Keys In {Keys.OemMinus, Keys.Subtract}
            If Me.hotkeyManager.TryRegister(HotkeyModifierKeys, key, id) Then
                Me.zoomOutHotkeyIds.Add(id)
            End If
        Next
        If Me.hotkeyManager.TryRegister(HotkeyModifiers.NoRepeat, Keys.Escape, id) Then
            Me.closeMagnifierHotkeyId = id
        End If

        If Me.zoomInHotkeyIds.Count > 0 AndAlso Me.zoomOutHotkeyIds.Count > 0 Then
            lines.Add($"{GlobalHotkeyManager.FormatModifiers(HotkeyModifierKeys)} + Plus/Minus: zoom")
        End If
        If Me.closeMagnifierHotkeyId <> 0 Then
            lines.Add("Esc: close")
        ElseIf Me.magnifierHotkeyId <> 0 Then
            lines.Add($"{GlobalHotkeyManager.FormatHotkey(HotkeyModifierKeys, MagnifierHotkeyKey)}: close")
        End If

        Return lines.ToArray()
    End Function

    ''' <summary>
    ''' Retrieves the window under the mouse cursor and updates the displayed information.
    ''' </summary>
    <DebuggerStepThrough>
    Private Sub RefreshInfo()

        Dim ptScreen As Point = Control.MousePosition
        Dim hWnd As IntPtr = WindowHelper.GetWindowFromPoint(ptScreen)

        Me.UpdateAllInfo(hWnd, ptScreen)
    End Sub

    ''' <summary>
    ''' Updates the displayed information with the specified window and mouse point.
    ''' </summary>
    '''
    ''' <param name="hWnd">The handle of the window under the mouse cursor.</param>
    ''' <param name="ptScreen">The mouse point, in screen coordinates.</param>
    <DebuggerStepThrough>
    Private Sub UpdateAllInfo(hWnd As IntPtr, ptScreen As Point)

        ' Window Handle
        Dim windowHandleInfo As New WindowHandlePropertyInfo(hWnd)
        Me.AllInfo.WindowHandle = windowHandleInfo

        ' Process
        Dim pid As Integer
        NativeMethods.GetWindowThreadProcessId(hWnd, pid)
        Me.AllInfo.Process = ProcessPropertyInfo.FromProcessId(pid)

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
        NativeMethods.RealGetWindowClass(hWnd, realClassNameBuffer, realClassNameBuffer.Capacity)
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

#End Region

End Class
