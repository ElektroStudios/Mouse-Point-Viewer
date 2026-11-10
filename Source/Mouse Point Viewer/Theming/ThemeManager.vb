#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Reflection
Imports System.Windows.Forms.VisualStyles

Imports Microsoft.Win32

Imports Win32

#End Region

''' <summary>
''' Applies the visual themes (<see cref="AppTheme"/>) to the windows and controls of the application.
''' </summary>
Friend Module ThemeManager ' NOSONAR

#Region " Constants "

    ''' <summary>
    ''' The value stored in the settings for <see cref="AppTheme.Light"/>.
    ''' </summary>
    Friend Const LightSettingValue As String = "Light"

    ''' <summary>
    ''' The value stored in the settings for <see cref="AppTheme.Dark"/>.
    ''' </summary>
    Friend Const DarkSettingValue As String = "Dark"

    ''' <summary>
    ''' The registry key that contains the personalization settings of the current user.
    ''' </summary>
    Private Const PersonalizeRegistryKey As String = "Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"

    ''' <summary>
    ''' The registry value that indicates whether applications use the light theme (1) or the dark theme (0).
    ''' </summary>
    Private Const AppsUseLightThemeValueName As String = "AppsUseLightTheme"

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Determines whether the Windows applications theme of the current user is dark.
    ''' </summary>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the Windows applications theme is dark;
    ''' <see langword="False"/> if it is light, or the operating system does not support themes for applications.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function IsSystemThemeDark() As Boolean

        Using key As RegistryKey = Registry.CurrentUser.OpenSubKey(PersonalizeRegistryKey, writable:=False)
            If key Is Nothing Then
                Return False ' Older Windows versions without application themes.
            End If

            Dim value As Object = key.GetValue(AppsUseLightThemeValueName)
            Return TypeOf value Is Integer AndAlso DirectCast(value, Integer) = 0
        End Using
    End Function

    ''' <summary>
    ''' Resolves the theme stored in the settings.
    ''' </summary>
    '''
    ''' <param name="settingValue">
    ''' The value stored in the settings: <see cref="LightSettingValue"/>, <see cref="DarkSettingValue"/>,
    ''' or an empty string when the user has not chosen a theme yet.
    ''' </param>
    '''
    ''' <param name="systemThemeIsDark">
    ''' Whether the Windows applications theme is dark (see <see cref="IsSystemThemeDark"/>),
    ''' used when the user has not chosen a theme yet.
    ''' </param>
    '''
    ''' <returns>
    ''' The resolved theme.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="settingValue"/> is not a valid theme setting value.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Function ResolveTheme(settingValue As String, systemThemeIsDark As Boolean) As AppTheme

        If String.IsNullOrEmpty(settingValue) Then
            Return If(systemThemeIsDark, AppTheme.Dark, AppTheme.Light)
        End If

        If String.Equals(settingValue, LightSettingValue, StringComparison.OrdinalIgnoreCase) Then
            Return AppTheme.Light
        End If

        If String.Equals(settingValue, DarkSettingValue, StringComparison.OrdinalIgnoreCase) Then
            Return AppTheme.Dark
        End If

        Throw New ArgumentException($"Invalid theme setting value: '{settingValue}'.", NameOf(settingValue))
    End Function

    ''' <summary>
    ''' Returns the value stored in the settings for the specified theme.
    ''' </summary>
    '''
    ''' <param name="theme">
    ''' The theme.
    ''' </param>
    '''
    ''' <returns>
    ''' The setting value.
    ''' </returns>
    '''
    ''' <exception cref="ComponentModel.InvalidEnumArgumentException">
    ''' Thrown when <paramref name="theme"/> is not a defined <see cref="AppTheme"/> value.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Function ToSettingValue(theme As AppTheme) As String

        Select Case theme
            Case AppTheme.Light : Return LightSettingValue
            Case AppTheme.Dark : Return DarkSettingValue
            Case Else : Throw New ComponentModel.InvalidEnumArgumentException(NameOf(theme), theme, GetType(AppTheme))
        End Select
    End Function

    ''' <summary>
    ''' Applies the colors of the specified palette to a control and all its child controls.
    ''' <para></para>
    ''' The control tree is traversed iteratively.
    ''' Controls whose <see cref="Control.Tag"/> is <see cref="SkipThemingTag"/> are not modified (nor their children).
    ''' </summary>
    '''
    ''' <param name="root">
    ''' The root control (usually a <see cref="Form"/>).
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The palette.
    ''' </param>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="root"/> or <paramref name="palette"/> is <see langword="Nothing"/>.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Sub ApplyColors(root As Control, palette As ThemePalette)

        If root Is Nothing Then
            Throw New ArgumentNullException(NameOf(root))
        End If
        If palette Is Nothing Then
            Throw New ArgumentNullException(NameOf(palette))
        End If

        Dim pending As New Stack(Of Control)()
        pending.Push(root)

        While pending.Count > 0
            Dim ctrl As Control = pending.Pop()
            If SkipThemingTag.Equals(ctrl.Tag) Then
                Continue While
            End If

            ThemeManager.ApplyControlColors(ctrl, palette)

            ' PropertyGrid children (grid view, tool bar, help pane) are themed through the PropertyGrid properties.
            If TypeOf ctrl Is PropertyGrid OrElse TypeOf ctrl Is NumericUpDown Then
                Continue While
            End If

            For Each child As Control In ctrl.Controls
                pending.Push(child)
            Next
        End While
    End Sub

    ''' <summary>
    ''' Enables or disables the dark mode of the title bar (non-client area) of a window,
    ''' and forces the title bar to be redrawn.
    ''' <para></para>
    ''' This must be called once the window handle has been created (e.g. in <see cref="Form.OnHandleCreated"/>),
    ''' and it has no effect on Windows versions prior to Windows 10 version 1809.
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' The handle of the window.
    ''' </param>
    '''
    ''' <param name="dark">
    ''' <see langword="True"/> to enable the dark mode of the title bar, <see langword="False"/> to disable it.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the attribute was set, <see langword="False"/> if the operating system does not support it.
    ''' </returns>
    '''
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="hWnd"/> is <see cref="IntPtr.Zero"/>.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Function SetDarkTitleBar(hWnd As IntPtr, dark As Boolean) As Boolean

        If hWnd = IntPtr.Zero Then
            Throw New ArgumentException("The window handle cannot be zero.", NameOf(hWnd))
        End If

        Dim value As Integer = If(dark, 1, 0)
        Dim hr As Integer = NativeMethods.DwmSetWindowAttribute(hWnd, Constants.DWMWA_USE_IMMERSIVE_DARK_MODE, value, 4)
        If hr <> 0 Then
            hr = NativeMethods.DwmSetWindowAttribute(hWnd, Constants.DWMWA_USE_IMMERSIVE_DARK_MODE_BEFORE_20H1, value, 4)
        End If

        If hr <> 0 Then
            Return False
        End If

        ' When switching the theme at runtime, the title bar of a visible window is not repainted until it is
        ' (de)activated (Windows 10). Repaint only the caption by simulating a deactivation followed by its real state.
        ' Note: forcing a frame recalculation (SetWindowPos with SWP_FRAMECHANGED) desynchronizes the non-client area
        ' and DWM, producing caption glitches and invisible minimize/maximize/close buttons.
        If NativeMethods.IsWindowVisible(hWnd) Then
            Dim isActive As Boolean = NativeMethods.GetForegroundWindow() = hWnd
            NativeMethods.SendMessage(hWnd, Constants.WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero)
            NativeMethods.SendMessage(hWnd, Constants.WM_NCACTIVATE, New IntPtr(If(isActive, 1, 0)), IntPtr.Zero)
        End If
        Return True
    End Function

    ''' <summary>
    ''' Applies the dark or the default visual style to the scroll bars of a window
    ''' (e.g. the grid view of a <see cref="PropertyGrid"/>).
    ''' </summary>
    '''
    ''' <param name="hWnd">
    ''' The handle of the window.
    ''' </param>
    '''
    ''' <param name="dark">
    ''' <see langword="True"/> to apply the dark visual style, <see langword="False"/> to restore the default one.
    ''' </param>
    '''
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="hWnd"/> is <see cref="IntPtr.Zero"/>.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Sub SetDarkScrollBars(hWnd As IntPtr, dark As Boolean)

        If hWnd = IntPtr.Zero Then
            Throw New ArgumentException("The window handle cannot be zero.", NameOf(hWnd))
        End If

        ' The return value is ignored on purpose: the dark visual style does not exist prior to Windows 10 version 1809,
        ' in which case the default visual style is kept, which is the expected fallback.
        NativeMethods.SetWindowTheme(hWnd, If(dark, "DarkMode_Explorer", "Explorer"), Nothing)
    End Sub

    ''' <summary>
    ''' Switches the expand/collapse glyphs (chevrons) drawn by the <see cref="PropertyGrid"/> between the light
    ''' ("Explorer::TreeView") and the dark ("DarkMode_Explorer::TreeView") variants of the operating system visual style.
    ''' <para></para>
    ''' The <see cref="PropertyGrid"/> always draws the glyphs of the internal
    ''' <c>VisualStyleElement.ExplorerTreeView.Glyph.Closed</c> and <c>.Opened</c> elements, whose (black) light variant
    ''' is hardly visible over a dark background. This method replaces, through reflection, the cached elements
    ''' (the private static fields <c>closed</c> and <c>opened</c>) with the elements of the requested variant,
    ''' which have the same shape and size (so the label indentation does not change).
    ''' <para></para>
    ''' The change affects every <see cref="PropertyGrid"/> of the process. Invalidate the grids to repaint them.
    ''' </summary>
    '''
    ''' <param name="dark">
    ''' <see langword="True"/> to use the dark variant, <see langword="False"/> to restore the light (default) variant.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the glyphs were switched; <see langword="False"/> if the internal fields were not found
    ''' (a .NET Framework version with a different implementation), visual styles are disabled, or the dark variant
    ''' is not defined (Windows versions prior to Windows 10 version 1809). In that case the glyphs are not modified.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Function TrySetDarkTreeGlyphs(dark As Boolean) As Boolean

        Const flags As BindingFlags = BindingFlags.Public Or BindingFlags.NonPublic
        Dim explorerTreeViewType As Type = GetType(VisualStyleElement).GetNestedType("ExplorerTreeView", flags)
        Dim glyphType As Type = explorerTreeViewType?.GetNestedType("Glyph", flags)
        Dim closedField As FieldInfo = glyphType?.GetField("closed", BindingFlags.NonPublic Or BindingFlags.Static)
        Dim openedField As FieldInfo = glyphType?.GetField("opened", BindingFlags.NonPublic Or BindingFlags.Static)
        If closedField Is Nothing OrElse openedField Is Nothing OrElse
            closedField.FieldType IsNot GetType(VisualStyleElement) OrElse openedField.FieldType IsNot GetType(VisualStyleElement) Then
            Return False
        End If

        ' TVP_GLYPH (2), GLPS_CLOSED (1) / GLPS_OPENED (2).
        Dim className As String = If(dark, "DarkMode_Explorer::TreeView", "Explorer::TreeView")
        Dim closedElement As VisualStyleElement = VisualStyleElement.CreateElement(className, 2, 1)
        Dim openedElement As VisualStyleElement = VisualStyleElement.CreateElement(className, 2, 2)

        If dark AndAlso Not (VisualStyleRenderer.IsSupported AndAlso
                             VisualStyleRenderer.IsElementDefined(closedElement) AndAlso
                             VisualStyleRenderer.IsElementDefined(openedElement)) Then
            Return False
        End If

        closedField.SetValue(Nothing, closedElement)
        openedField.SetValue(Nothing, openedElement)
        Return True
    End Function

#End Region

#Region " Public Fields "

    ''' <summary>
    ''' The <see cref="Control.Tag"/> value that excludes a control (and its children) from <see cref="ApplyColors"/>.
    ''' </summary>
    Friend ReadOnly SkipThemingTag As New Object()

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Applies the colors of the specified palette to a single control, according to its type.
    ''' </summary>
    '''
    ''' <param name="ctrl">
    ''' The control.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The palette.
    ''' </param>
    Private Sub ApplyControlColors(ctrl As Control, palette As ThemePalette)

        Dim isDark As Boolean = palette.Theme = AppTheme.Dark

        Select Case True
            Case TypeOf ctrl Is Form
                ctrl.BackColor = palette.WindowBack
                ctrl.ForeColor = palette.WindowFore

            Case TypeOf ctrl Is PropertyGrid
                ThemeManager.ApplyPropertyGridColors(DirectCast(ctrl, PropertyGrid), palette)

            Case TypeOf ctrl Is ButtonBase AndAlso ThemeManager.IsButtonLike(DirectCast(ctrl, ButtonBase))
                Dim btn As ButtonBase = DirectCast(ctrl, ButtonBase)
                btn.FlatStyle = FlatStyle.Flat
                btn.UseVisualStyleBackColor = False
                btn.BackColor = palette.WindowBack
                btn.ForeColor = palette.ControlFore
                btn.FlatAppearance.BorderColor = palette.Border
                btn.FlatAppearance.BorderSize = 1
                btn.FlatAppearance.MouseOverBackColor = palette.ButtonHoverBack
                btn.FlatAppearance.MouseDownBackColor = palette.ButtonPressedBack
                btn.FlatAppearance.CheckedBackColor = palette.ButtonPressedBack

            Case TypeOf ctrl Is CheckBox
                Dim cb As CheckBox = DirectCast(ctrl, CheckBox)
                ' The system check box glyph is not readable over a dark background.
                cb.FlatStyle = If(isDark, FlatStyle.Flat, FlatStyle.Standard)
                cb.BackColor = palette.WindowBack
                cb.ForeColor = palette.WindowFore
                cb.FlatAppearance.BorderColor = palette.Border
                cb.FlatAppearance.CheckedBackColor = palette.ControlBack
                ' Must be set after BackColor: setting BackColor resets UseVisualStyleBackColor to False.
                cb.UseVisualStyleBackColor = Not isDark

            Case TypeOf ctrl Is NumericUpDown
                Dim nud As NumericUpDown = DirectCast(ctrl, NumericUpDown)
                nud.BackColor = palette.ControlBack
                nud.ForeColor = palette.ControlFore
                nud.BorderStyle = If(isDark, BorderStyle.FixedSingle, BorderStyle.Fixed3D)

            Case TypeOf ctrl Is Label
                Dim role As ThemeColorRole = If(TypeOf ctrl.Tag Is ThemeColorRole, DirectCast(ctrl.Tag, ThemeColorRole), ThemeColorRole.Primary)
                ' A separator is a thin label filled with the border color.
                ctrl.BackColor = If(role = ThemeColorRole.Separator, palette.Border, Color.Transparent)
                ' Labels may be dynamically recolored (e.g. the status/hint label); they use the window colors by default.
                ctrl.ForeColor = If(role = ThemeColorRole.Secondary, palette.SecondaryFore, palette.WindowFore)

            Case Else
                ctrl.BackColor = palette.WindowBack
                ctrl.ForeColor = palette.WindowFore
        End Select
    End Sub

    ''' <summary>
    ''' Determines whether a <see cref="ButtonBase"/> is rendered as a push button
    ''' (a <see cref="Button"/>, or a <see cref="CheckBox"/> with <see cref="Appearance.Button"/> appearance).
    ''' </summary>
    '''
    ''' <param name="btn">
    ''' The button.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the control is rendered as a push button, <see langword="False"/> otherwise.
    ''' </returns>
    Private Function IsButtonLike(btn As ButtonBase) As Boolean

        Return TypeOf btn Is Button OrElse
               (TypeOf btn Is CheckBox AndAlso DirectCast(btn, CheckBox).Appearance = Appearance.Button)
    End Function

    ''' <summary>
    ''' Applies the colors of the specified palette to a <see cref="PropertyGrid"/>.
    ''' </summary>
    '''
    ''' <param name="grid">
    ''' The <see cref="PropertyGrid"/>.
    ''' </param>
    '''
    ''' <param name="palette">
    ''' The palette.
    ''' </param>
    Private Sub ApplyPropertyGridColors(grid As PropertyGrid, palette As ThemePalette)

        grid.BackColor = palette.GridBack
        grid.ViewBackColor = palette.GridViewBack
        grid.ViewForeColor = palette.GridViewFore
        grid.LineColor = palette.GridLine
        grid.CategoryForeColor = palette.GridViewFore
        grid.CategorySplitterColor = palette.GridLine
        grid.HelpBackColor = palette.GridBack
        grid.HelpForeColor = palette.GridViewFore
        grid.CommandsBackColor = palette.GridBack
        grid.CommandsForeColor = palette.GridViewFore
        grid.SelectedItemWithFocusBackColor = palette.GridSelectedBack
        grid.SelectedItemWithFocusForeColor = palette.GridSelectedFore
        grid.DisabledItemForeColor = palette.GridReadOnlyFore
        grid.ViewBorderColor = palette.Border
        ' The visual style glyphs (the Explorer tree view chevrons drawn by the operating system) are kept in every theme:
        ' disabling them draws the legacy "+" / "-" boxes instead, which are narrower and shift the labels to the left.
        grid.CanShowVisualStyleGlyphs = True
        ' The result is ignored on purpose: when the dark glyphs are not available, the default glyphs are kept.
        ThemeManager.TrySetDarkTreeGlyphs(palette.Theme = AppTheme.Dark)
        grid.Invalidate(invalidateChildren:=True)
    End Sub

#End Region

End Module

''' <summary>
''' Specifies the color role of a <see cref="Label"/> when it is themed by <see cref="ThemeManager.ApplyColors"/>.
''' <para></para>
''' Assign the value to the <see cref="Control.Tag"/> property of the label.
''' </summary>
Friend Enum ThemeColorRole As Integer ' NOSONAR

    ''' <summary>
    ''' The label uses the primary text color (<see cref="ThemePalette.WindowFore"/>).
    ''' </summary>
    Primary = 0

    ''' <summary>
    ''' The label uses the secondary text color (<see cref="ThemePalette.SecondaryFore"/>).
    ''' </summary>
    Secondary = 1

    ''' <summary>
    ''' The label is a visual separator, filled with the border color (<see cref="ThemePalette.Border"/>).
    ''' </summary>
    Separator = 2

End Enum
