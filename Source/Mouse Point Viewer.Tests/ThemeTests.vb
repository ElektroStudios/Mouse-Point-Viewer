#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Reflection
Imports System.Windows.Forms
Imports System.Windows.Forms.VisualStyles

Imports MousePointViewer.Tests.Helpers

#End Region

#Region " ThemeTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Tests for the visual themes: <see cref="ThemePalette"/> and <see cref="ThemeManager"/>.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class ThemeTests

#Region " Settings Values "

        ''' <summary>
        ''' Verifies how the theme stored in the settings is resolved:
        ''' an empty value (no theme chosen yet) follows the Windows applications theme.
        ''' </summary>
        '''
        ''' <param name="settingValue">The value stored in the settings.</param>
        ''' <param name="systemIsDark">Whether the Windows applications theme is dark.</param>
        ''' <param name="expected">The expected theme (<see cref="AppTheme"/> value).</param>
        <TestMethod>
        <DataRow("", False, CInt(AppTheme.Light))>
        <DataRow("", True, CInt(AppTheme.Dark))>
        <DataRow(Nothing, True, CInt(AppTheme.Dark))>
        <DataRow("Light", True, CInt(AppTheme.Light))>
        <DataRow("Dark", False, CInt(AppTheme.Dark))>
        <DataRow("dark", False, CInt(AppTheme.Dark))>
        <DataRow("LIGHT", True, CInt(AppTheme.Light))>
        Public Sub ResolveTheme_ReturnsExpectedTheme(settingValue As String, systemIsDark As Boolean, expected As Integer)

            Assert.AreEqual(CType(expected, AppTheme), ThemeManager.ResolveTheme(settingValue, systemIsDark))
        End Sub

        ''' <summary>
        ''' Verifies that an invalid setting value throws <see cref="ArgumentException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub ResolveTheme_InvalidValue_ThrowsArgumentException()

            Assert.ThrowsExactly(Of ArgumentException)(Function() ThemeManager.ResolveTheme("Blue", False))
        End Sub

        ''' <summary>
        ''' Verifies that every theme round-trips through its setting value.
        ''' </summary>
        <TestMethod>
        Public Sub ToSettingValue_RoundTrips()

            For Each theme As AppTheme In [Enum].GetValues(GetType(AppTheme))
                Dim value As String = ThemeManager.ToSettingValue(theme)
                Assert.AreEqual(theme, ThemeManager.ResolveTheme(value, systemThemeIsDark:=theme = AppTheme.Light))
            Next
        End Sub

        ''' <summary>
        ''' Verifies that undefined theme values throw <see cref="InvalidEnumArgumentException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub UndefinedTheme_ThrowsInvalidEnumArgumentException()

            Dim undefined As AppTheme = CType(99, AppTheme)
            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Function() ThemeManager.ToSettingValue(undefined))
            Assert.ThrowsExactly(Of InvalidEnumArgumentException)(Function() ThemePalette.FromTheme(undefined))
        End Sub

        ''' <summary>
        ''' Verifies that reading the Windows applications theme does not throw.
        ''' </summary>
        <TestMethod>
        Public Sub IsSystemThemeDark_DoesNotThrow()

            Dim unused As Boolean = ThemeManager.IsSystemThemeDark()
        End Sub

#End Region

#Region " Palettes "

        ''' <summary>
        ''' Verifies that each palette represents its theme.
        ''' </summary>
        <TestMethod>
        Public Sub FromTheme_ReturnsMatchingPalette()

            Assert.AreSame(ThemePalette.Light, ThemePalette.FromTheme(AppTheme.Light))
            Assert.AreSame(ThemePalette.Dark, ThemePalette.FromTheme(AppTheme.Dark))
            Assert.AreEqual(AppTheme.Light, ThemePalette.Light.Theme)
            Assert.AreEqual(AppTheme.Dark, ThemePalette.Dark.Theme)
        End Sub

        ''' <summary>
        ''' Verifies that the texts of the dark palette are readable (WCAG 2.x contrast ratios:
        ''' at least 4.5:1 for every text, and 7:1 for the primary texts).
        ''' </summary>
        <TestMethod>
        Public Sub DarkPalette_TextContrastIsAccessible()

            Dim p As ThemePalette = ThemePalette.Dark

            ThemeTests.AssertContrast(p.WindowFore, p.WindowBack, 7.0, "Window text")
            ThemeTests.AssertContrast(p.SecondaryFore, p.WindowBack, 4.5, "Secondary text")
            ThemeTests.AssertContrast(p.SuccessFore, p.WindowBack, 4.5, "Success text")
            ThemeTests.AssertContrast(p.ControlFore, p.ControlBack, 7.0, "Control text")
            ThemeTests.AssertContrast(p.GridViewFore, p.GridViewBack, 7.0, "Grid text")
            ThemeTests.AssertContrast(p.GridReadOnlyFore, p.GridViewBack, 4.5, "Grid read-only text")
            ThemeTests.AssertContrast(p.GridSelectedFore, p.GridSelectedBack, 4.5, "Grid selected text")
        End Sub

        ''' <summary>
        ''' Verifies that the texts of the light palette are readable with the default (non high contrast) system colors.
        ''' </summary>
        <TestMethod>
        Public Sub LightPalette_TextContrastIsAccessible()

            If SystemInformation.HighContrast Then
                Assert.Inconclusive("The light palette uses the high contrast system colors.")
            End If

            Dim p As ThemePalette = ThemePalette.Light

            ThemeTests.AssertContrast(p.WindowFore, p.WindowBack, 7.0, "Window text")
            ThemeTests.AssertContrast(p.SecondaryFore, p.WindowBack, 4.5, "Secondary text")
            ThemeTests.AssertContrast(p.SuccessFore, p.WindowBack, 4.5, "Success text")
            ThemeTests.AssertContrast(p.GridReadOnlyFore, p.GridViewBack, 4.5, "Grid read-only text")
        End Sub

        ''' <summary>
        ''' Verifies that the WCAG contrast ratio helper used by these tests is correct (black on white is 21:1).
        ''' </summary>
        <TestMethod>
        Public Sub ContrastRatio_BlackOnWhite_Is21()

            Assert.AreEqual(21.0, ThemeTests.GetContrastRatio(Color.Black, Color.White), 0.01)
            Assert.AreEqual(1.0, ThemeTests.GetContrastRatio(Color.Gray, Color.Gray), 0.01)
        End Sub

#End Region

#Region " ApplyColors "

        ''' <summary>
        ''' Verifies that the dark palette is applied to every kind of control in the control tree.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyColors_Dark_AppliesToAllControlTypes()

            Using form As New Form()
                Dim controls As ThemedControls = ThemeTests.AddControls(form)
                Dim p As ThemePalette = ThemePalette.Dark

                ThemeManager.ApplyColors(form, p)

                Assert.AreEqual(p.WindowBack, form.BackColor)
                Assert.AreEqual(p.WindowFore, form.ForeColor)
                Assert.AreEqual(p.WindowFore, controls.PrimaryLabel.ForeColor)
                Assert.AreEqual(p.SecondaryFore, controls.SecondaryLabel.ForeColor)
                Assert.AreEqual(FlatStyle.Flat, controls.Button.FlatStyle)
                Assert.AreEqual(p.ButtonHoverBack, controls.Button.FlatAppearance.MouseOverBackColor)
                Assert.AreEqual(FlatStyle.Flat, controls.ToggleButton.FlatStyle)
                Assert.AreEqual(p.ButtonPressedBack, controls.ToggleButton.FlatAppearance.CheckedBackColor)
                Assert.AreEqual(FlatStyle.Flat, controls.CheckBox.FlatStyle)
                Assert.AreEqual(p.WindowFore, controls.CheckBox.ForeColor)
                Assert.AreEqual(p.ControlBack, controls.NumericUpDown.BackColor)
                Assert.AreEqual(BorderStyle.FixedSingle, controls.NumericUpDown.BorderStyle)
                Assert.AreEqual(p.GridViewBack, controls.Grid.ViewBackColor)
                Assert.AreEqual(p.GridReadOnlyFore, controls.Grid.DisabledItemForeColor)
                Assert.AreEqual(p.GridSelectedBack, controls.Grid.SelectedItemWithFocusBackColor)
                Assert.IsTrue(controls.Grid.CanShowVisualStyleGlyphs, "The glyphs (and the label indentation) must be the same as in the light theme.")
                Assert.AreEqual(p.Border, controls.Separator.BackColor)
                Assert.AreEqual(p.WindowFore, controls.NestedLabel.ForeColor, "Nested controls are not themed.")
                Assert.AreEqual(p.WindowBack, controls.Panel.BackColor)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that applying the light palette after the dark one restores the original (system) look.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyColors_LightAfterDark_RestoresSystemLook()

            Using form As New Form()
                Dim controls As ThemedControls = ThemeTests.AddControls(form)

                ThemeManager.ApplyColors(form, ThemePalette.Dark)
                ThemeManager.ApplyColors(form, ThemePalette.Light)

                Assert.AreEqual(SystemColors.Window, form.BackColor)
                Assert.AreEqual(FlatStyle.Standard, controls.CheckBox.FlatStyle)
                Assert.IsTrue(controls.CheckBox.UseVisualStyleBackColor)
                Assert.AreEqual(BorderStyle.Fixed3D, controls.NumericUpDown.BorderStyle)
                Assert.AreEqual(SystemColors.Window, controls.Grid.ViewBackColor)
                Assert.AreEqual(SystemColors.WindowText, controls.Grid.ViewForeColor)
                Assert.IsTrue(controls.Grid.CanShowVisualStyleGlyphs)
                Assert.AreEqual(SystemColors.GrayText, controls.SecondaryLabel.ForeColor)
                Assert.AreEqual(SystemColors.ControlDark, controls.Separator.BackColor)
                Assert.AreEqual(Color.Transparent, controls.PrimaryLabel.BackColor)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that controls tagged with <see cref="ThemeManager.SkipThemingTag"/> (and their children) are not modified.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyColors_SkipThemingTag_IsRespected()

            Using form As New Form()
                Dim skipped As New Panel() With {.BackColor = Color.Magenta, .Tag = ThemeManager.SkipThemingTag}
                Dim child As New Label() With {.ForeColor = Color.Lime}
                skipped.Controls.Add(child)
                form.Controls.Add(skipped)

                ThemeManager.ApplyColors(form, ThemePalette.Dark)

                Assert.AreEqual(Color.Magenta, skipped.BackColor)
                Assert.AreEqual(Color.Lime, child.ForeColor)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that null arguments throw <see cref="ArgumentNullException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub ApplyColors_NullArguments_ThrowArgumentNullException()

            Using form As New Form()
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyColors(Nothing, ThemePalette.Dark))
                Assert.ThrowsExactly(Of ArgumentNullException)(Sub() ThemeManager.ApplyColors(form, Nothing))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a deep control tree is themed without stack overflows (the traversal is iterative).
        ''' </summary>
        <TestMethod>
        Public Sub ApplyColors_DeepControlTree_IsFullyThemed()

            Using form As New Form()
                Dim parent As Control = form
                For i As Integer = 1 To 200
                    Dim panel As New Panel()
                    parent.Controls.Add(panel)
                    parent = panel
                Next

                ThemeManager.ApplyColors(form, ThemePalette.Dark)

                Assert.AreEqual(ThemePalette.Dark.WindowBack, parent.BackColor)
            End Using
        End Sub

#End Region

#Region " Native Theme "

        ''' <summary>
        ''' Verifies that the dark title bar and dark scroll bars can be enabled and disabled on a real window.
        ''' </summary>
        <TestMethod>
        Public Sub SetDarkTitleBarAndScrollBars_RealWindow_DoNotThrow()

            Using window As TestWindow = New TestWindow(TestWindow.GetCenteredBounds(), Color.White).ShowAndPaint()
                window.Form.FormBorderStyle = FormBorderStyle.Sizable
                TestWindow.PumpMessages()

                Dim darkResult As Boolean = ThemeManager.SetDarkTitleBar(window.Handle, True)
                Dim lightResult As Boolean = ThemeManager.SetDarkTitleBar(window.Handle, False)
                Assert.AreEqual(darkResult, lightResult, "The attribute is supported for one value but not for the other.")

                ThemeManager.SetDarkScrollBars(window.Handle, True)
                ThemeManager.SetDarkScrollBars(window.Handle, False)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a zero window handle throws <see cref="ArgumentException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub NativeTheme_ZeroHandle_ThrowsArgumentException()

            Assert.ThrowsExactly(Of ArgumentException)(Function() ThemeManager.SetDarkTitleBar(IntPtr.Zero, True))
            Assert.ThrowsExactly(Of ArgumentException)(Sub() ThemeManager.SetDarkScrollBars(IntPtr.Zero, True))
        End Sub

#End Region

#Region " Dark Tree Glyphs "

        ''' <summary>
        ''' Restores the default (light) property grid glyphs after each test, as the glyphs are shared by the whole process.
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()

            ThemeManager.TrySetDarkTreeGlyphs(False)
        End Sub

        ''' <summary>
        ''' Verifies that the internal fields that cache the property grid glyphs are found through reflection,
        ''' and that the light glyphs are always restored.
        ''' </summary>
        <TestMethod>
        Public Sub TrySetDarkTreeGlyphs_Light_FindsInternalFieldsAndRestoresDefaultGlyphs()

            Assert.IsTrue(ThemeManager.TrySetDarkTreeGlyphs(False), "The internal fields of VisualStyleElement.ExplorerTreeView.Glyph were not found.")

            Assert.AreEqual("Explorer::TreeView", ThemeTests.GetTreeGlyph("Closed").ClassName)
            Assert.AreEqual("Explorer::TreeView", ThemeTests.GetTreeGlyph("Opened").ClassName)
        End Sub

        ''' <summary>
        ''' Verifies that the dark glyphs (same part and states, dark visual style class) are applied and then restored.
        ''' </summary>
        <TestMethod>
        Public Sub TrySetDarkTreeGlyphs_Dark_SwitchesToDarkVariantAndBack()

            If Not VisualStyleRenderer.IsSupported OrElse
                Not VisualStyleRenderer.IsElementDefined(VisualStyleElement.CreateElement("DarkMode_Explorer::TreeView", 2, 1)) Then
                Assert.Inconclusive("The dark visual style is not available (visual styles disabled, or Windows prior to 10 1809).")
            End If

            Assert.IsTrue(ThemeManager.TrySetDarkTreeGlyphs(True))
            Dim closed As VisualStyleElement = ThemeTests.GetTreeGlyph("Closed")
            Dim opened As VisualStyleElement = ThemeTests.GetTreeGlyph("Opened")
            Assert.AreEqual("DarkMode_Explorer::TreeView", closed.ClassName)
            Assert.AreEqual("DarkMode_Explorer::TreeView", opened.ClassName)
            Assert.AreEqual(2, closed.Part)
            Assert.AreEqual(1, closed.State)
            Assert.AreEqual(2, opened.State)

            Assert.IsTrue(ThemeManager.TrySetDarkTreeGlyphs(False))
            Assert.AreEqual("Explorer::TreeView", ThemeTests.GetTreeGlyph("Closed").ClassName)
        End Sub

        ''' <summary>
        ''' Verifies that a property grid with expanded items paints with the dark glyphs without throwing.
        ''' </summary>
        <TestMethod>
        Public Sub DarkGlyphs_PropertyGridPaints()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim grid As New PropertyGrid() With {.Dock = DockStyle.Fill, .SelectedObject = New Button()}
                window.Form.Controls.Add(grid)
                window.ShowAndPaint()

                ThemeManager.ApplyColors(window.Form, ThemePalette.Dark)
                grid.ExpandAllGridItems()
                grid.Refresh()
                ThemeManager.ApplyColors(window.Form, ThemePalette.Light)
                grid.Refresh()
            End Using
        End Sub

        ''' <summary>
        ''' Returns the current value of the internal <c>VisualStyleElement.ExplorerTreeView.Glyph</c> property with the specified name.
        ''' </summary>
        '''
        ''' <param name="propertyName">"Closed" or "Opened".</param>
        '''
        ''' <returns>The visual style element.</returns>
        Private Shared Function GetTreeGlyph(propertyName As String) As VisualStyleElement

            Const flags As BindingFlags = BindingFlags.Public Or BindingFlags.NonPublic
            Dim glyphType As Type = GetType(VisualStyleElement).GetNestedType("ExplorerTreeView", flags).GetNestedType("Glyph", flags)
            Return DirectCast(glyphType.GetProperty(propertyName, flags Or BindingFlags.Static).GetValue(Nothing), VisualStyleElement)
        End Function

#End Region

#Region " Private Members "

        ''' <summary>
        ''' The controls added to a form by <see cref="ThemeTests.AddControls"/>.
        ''' </summary>
        Private NotInheritable Class ThemedControls
            Friend Property PrimaryLabel As Label
            Friend Property SecondaryLabel As Label
            Friend Property Button As Button
            Friend Property ToggleButton As CheckBox
            Friend Property CheckBox As CheckBox
            Friend Property NumericUpDown As NumericUpDown
            Friend Property Grid As PropertyGrid
            Friend Property Panel As Panel
            Friend Property NestedLabel As Label
            Friend Property Separator As Label
        End Class

        ''' <summary>
        ''' Adds one control of each themed type to the specified form.
        ''' </summary>
        '''
        ''' <param name="form">The form.</param>
        '''
        ''' <returns>The added controls.</returns>
        Private Shared Function AddControls(form As Form) As ThemedControls

            Dim controls As New ThemedControls With {
                .PrimaryLabel = New Label(),
                .SecondaryLabel = New Label() With {.Tag = ThemeColorRole.Secondary},
                .Button = New Button(),
                .ToggleButton = New CheckBox() With {.Appearance = Appearance.Button},
                .CheckBox = New CheckBox(),
                .NumericUpDown = New NumericUpDown(),
                .Grid = New PropertyGrid(),
                .Panel = New Panel(),
                .NestedLabel = New Label(),
                .Separator = New Label() With {.Tag = ThemeColorRole.Separator}
            }
            controls.Panel.Controls.Add(controls.NestedLabel)
            form.Controls.AddRange({controls.PrimaryLabel, controls.SecondaryLabel, controls.Button, controls.ToggleButton,
                                    controls.CheckBox, controls.NumericUpDown, controls.Grid, controls.Panel, controls.Separator})
            Return controls
        End Function

        ''' <summary>
        ''' Asserts that the contrast ratio between two colors is at least the specified minimum.
        ''' </summary>
        '''
        ''' <param name="fore">The text color.</param>
        ''' <param name="back">The background color.</param>
        ''' <param name="minimum">The minimum contrast ratio.</param>
        ''' <param name="description">The description of the color pair.</param>
        Private Shared Sub AssertContrast(fore As Color, back As Color, minimum As Double, description As String)

            Dim ratio As Double = ThemeTests.GetContrastRatio(fore, back)
            Assert.IsTrue(ratio >= minimum, $"{description}: contrast {ratio:0.00}:1 is below {minimum}:1 ({fore} on {back}).")
        End Sub

        ''' <summary>
        ''' Returns the WCAG 2.x contrast ratio between two colors.
        ''' </summary>
        '''
        ''' <param name="a">The first color.</param>
        ''' <param name="b">The second color.</param>
        '''
        ''' <returns>The contrast ratio (1 to 21).</returns>
        Private Shared Function GetContrastRatio(a As Color, b As Color) As Double

            Dim la As Double = ThemeTests.GetRelativeLuminance(a)
            Dim lb As Double = ThemeTests.GetRelativeLuminance(b)
            Return (Math.Max(la, lb) + 0.05) / (Math.Min(la, lb) + 0.05)
        End Function

        ''' <summary>
        ''' Returns the WCAG 2.x relative luminance of a color.
        ''' </summary>
        '''
        ''' <param name="c">The color.</param>
        '''
        ''' <returns>The relative luminance (0 to 1).</returns>
        Private Shared Function GetRelativeLuminance(c As Color) As Double

            Dim channel As Func(Of Byte, Double) =
                Function(v As Byte) As Double
                    Dim s As Double = v / 255.0
                    Return If(s <= 0.03928, s / 12.92, Math.Pow((s + 0.055) / 1.055, 2.4))
                End Function

            Return (0.2126 * channel(c.R)) + (0.7152 * channel(c.G)) + (0.0722 * channel(c.B))
        End Function

#End Region

    End Class

End Namespace

#End Region
