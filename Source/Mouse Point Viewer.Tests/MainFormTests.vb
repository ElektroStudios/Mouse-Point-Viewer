#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Drawing
Imports System.Windows.Forms

Imports MousePointViewer.Tests.Helpers

#End Region

#Region " MainFormTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Tests for the <see cref="Main"/> form.
    ''' <para></para>
    ''' The form is never shown in these tests (so the user settings are never saved and no global hotkey is registered):
    ''' the tested behavior is driven through its controls and its internal methods.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class MainFormTests

#Region " Test Lifecycle "

        ''' <summary>
        ''' The window search options, saved before each test and restored after it.
        ''' </summary>
        Private optionsScope As WindowSearchOptionsScope

        ''' <summary>
        ''' Resets the window search options before each test.
        ''' </summary>
        <TestInitialize>
        Public Sub TestInitialize()

            Me.optionsScope = New WindowSearchOptionsScope(ignoreChildWindows:=False, ignoreLayeredWindows:=False, ignoreTransparentWindows:=False)
        End Sub

        ''' <summary>
        ''' Restores the window search options after each test.
        ''' </summary>
        <TestCleanup>
        Public Sub TestCleanup()

            Me.optionsScope.Dispose()
        End Sub

#End Region

#Region " Default State "

        ''' <summary>
        ''' Verifies the default state of the controls of the main form.
        ''' <para></para>
        ''' This failed in the first v1.4 test run: the designer enabled the refresh timer,
        ''' so it started ticking as soon as the form was created, before it was shown.
        ''' </summary>
        <TestMethod>
        Public Sub DefaultState()

            Using form As New Main()
                Assert.IsFalse(form.CheckBox_IgnoreChildWindows.Checked)
                Assert.IsFalse(form.CheckBox_IgnoreLayeredWindows.Checked)
                Assert.IsFalse(form.CheckBox_IgnoreTransparentWindows.Checked)
                Assert.IsFalse(form.CheckBox_TopMost.Checked)
                Assert.IsFalse(form.CheckBox_Magnifier.Checked)
                Assert.AreEqual(Appearance.Button, form.CheckBox_Magnifier.Appearance, "The magnifier check box must look like a toggle button.")
                Assert.AreEqual(10D, form.NumericUpDown_Refresh.Minimum)
                Assert.AreEqual(5000D, form.NumericUpDown_Refresh.Maximum)
                Assert.AreEqual(100D, form.NumericUpDown_Refresh.Value)
                Assert.AreEqual(100, form.RefreshTimer.Interval)
                Assert.IsFalse(form.RefreshTimer.Enabled, "The timer must not start before the form is shown.")
                Assert.IsFalse(form.StatusTimer.Enabled)
                Assert.IsTrue(form.KeyPreview, "KeyPreview is required for the local Ctrl+C shortcut.")
                Assert.IsNotNull(form.Icon)
                Assert.IsNotNull(form.Button_GitHub.Image, "The GitHub button has no image.")
                Assert.IsFalse(form.IsMagnifierVisible)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the icon buttons have a tool tip, so their meaning is discoverable.
        ''' </summary>
        <TestMethod>
        Public Sub IconButtons_HaveToolTips()

            Using form As New Main()
                For Each ctrl As Control In {DirectCast(form.CheckBox_Magnifier, Control), form.Button_ViewMode, form.Button_Theme, form.Button_GitHub}
                    Assert.IsFalse(String.IsNullOrWhiteSpace(form.ToolTip1.GetToolTip(ctrl)), $"'{ctrl.Name}' has no tool tip.")
                    If ctrl IsNot form.Button_GitHub Then
                        Assert.IsFalse(String.IsNullOrEmpty(ctrl.Text), $"'{ctrl.Name}' has no glyph.")
                    End If
                Next
            End Using
        End Sub

#End Region

#Region " Options "

        ''' <summary>
        ''' Verifies that each "Ignore ..." check box updates its corresponding window search option.
        ''' </summary>
        <TestMethod>
        Public Sub IgnoreCheckBoxes_UpdateWindowSearchOptions()

            Using form As New Main()
                form.CheckBox_IgnoreChildWindows.Checked = True
                Assert.IsTrue(Main.ignoreChildWindows)
                Assert.IsFalse(Main.ignoreLayeredWindows)
                Assert.IsFalse(Main.ignoreTransparentWindows)

                form.CheckBox_IgnoreLayeredWindows.Checked = True
                Assert.IsTrue(Main.ignoreLayeredWindows)

                form.CheckBox_IgnoreTransparentWindows.Checked = True
                Assert.IsTrue(Main.ignoreTransparentWindows)

                form.CheckBox_IgnoreChildWindows.Checked = False
                form.CheckBox_IgnoreLayeredWindows.Checked = False
                form.CheckBox_IgnoreTransparentWindows.Checked = False
                Assert.IsFalse(Main.ignoreChildWindows)
                Assert.IsFalse(Main.ignoreLayeredWindows)
                Assert.IsFalse(Main.ignoreTransparentWindows)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the "Top Most" check box updates the <see cref="Form.TopMost"/> property of the form.
        ''' </summary>
        <TestMethod>
        Public Sub TopMostCheckBox_UpdatesTopMost()

            Using form As New Main()
                form.CheckBox_TopMost.Checked = True
                Assert.IsTrue(form.TopMost)

                form.CheckBox_TopMost.Checked = False
                Assert.IsFalse(form.TopMost)
            End Using
        End Sub

#End Region

#Region " Refresh Rate "

        ''' <summary>
        ''' Verifies the parsing of the typed refresh rate: only integers within the allowed range are accepted.
        ''' </summary>
        '''
        ''' <param name="text">The typed text.</param>
        ''' <param name="expectedResult">The expected result.</param>
        ''' <param name="expectedInterval">The expected interval, when the result is <see langword="True"/>.</param>
        <TestMethod>
        <DataRow("250", True, 250)>
        <DataRow("10", True, 10)>
        <DataRow("5000", True, 5000)>
        <DataRow(" 300 ", True, 300)>
        <DataRow("9", False, 0)>
        <DataRow("5001", False, 0)>
        <DataRow("-100", False, 0)>
        <DataRow("abc", False, 0)>
        <DataRow("12.5", False, 0)>
        <DataRow("", False, 0)>
        <DataRow(Nothing, False, 0)>
        Public Sub TryParseRefreshInterval_AcceptsOnlyIntegersInRange(text As String, expectedResult As Boolean, expectedInterval As Integer)

            Dim interval As Integer = -1
            Dim result As Boolean = Main.TryParseRefreshInterval(text, 10D, 5000D, interval)

            Assert.AreEqual(expectedResult, result)
            If expectedResult Then
                Assert.AreEqual(expectedInterval, interval)
            Else
                Assert.AreEqual(-1, interval, "The interval must not be modified when the text is not valid.")
            End If
        End Sub

        ''' <summary>
        ''' Verifies how the typed refresh rate updates the timer interval.
        ''' <para></para>
        ''' The <see cref="NumericUpDown"/> clamps an out of range value to its <see cref="NumericUpDown.Minimum"/> or
        ''' <see cref="NumericUpDown.Maximum"/> when it validates the typed text (raising the text changed event again),
        ''' so the timer interval ends up being the clamped value, exactly as the user sees it in the control.
        ''' Invalid texts do not change the interval.
        ''' </summary>
        '''
        ''' <param name="text">The typed text.</param>
        ''' <param name="expectedInterval">The expected timer interval.</param>
        <TestMethod>
        <DataRow("250", 250)>
        <DataRow("10", 10)>
        <DataRow("5000", 5000)>
        <DataRow("9", 10)>
        <DataRow("5001", 5000)>
        <DataRow("abc", 100)>
        <DataRow("", 100)>
        Public Sub RefreshRateText_UpdatesTimerInterval(text As String, expectedInterval As Integer)

            Using form As New Main()
                form.RefreshTimer.Interval = 100

                form.NumericUpDown_Refresh.Text = text

                Assert.AreEqual(expectedInterval, form.RefreshTimer.Interval)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that changing the value with the up/down buttons (or programmatically) updates the timer interval.
        ''' </summary>
        <TestMethod>
        Public Sub RefreshRateValue_UpdatesTimerInterval()

            Using form As New Main()
                form.NumericUpDown_Refresh.Value = 750D
                Assert.AreEqual(750, form.RefreshTimer.Interval)

                form.NumericUpDown_Refresh.UpButton()
                Assert.AreEqual(751, form.RefreshTimer.Interval)
            End Using
        End Sub

#End Region

#Region " Theme "

        ''' <summary>
        ''' Verifies that switching the theme applies the colors of the theme to the form and its controls,
        ''' and updates the theme-dependent icons and tool tips.
        ''' </summary>
        <TestMethod>
        Public Sub SetTheme_AppliesPaletteAndIcons()

            Using form As New Main()
                form.SetTheme(AppTheme.Dark)
                Assert.AreEqual(AppTheme.Dark, form.Theme)
                Assert.AreEqual(ThemePalette.Dark.WindowBack, form.BackColor)
                Assert.AreEqual(ThemePalette.Dark.GridViewBack, form.PropertyGrid1.ViewBackColor)
                Assert.AreEqual(ThemePalette.Dark.SecondaryFore, form.Label_Hotkeys.ForeColor)
                Assert.AreEqual(FlatStyle.Flat, form.CheckBox_TopMost.FlatStyle)
                Dim darkImage As Image = form.Button_GitHub.Image
                StringAssert.Contains(form.ToolTip1.GetToolTip(form.Button_Theme), "light")

                form.SetTheme(AppTheme.Light)
                Assert.AreEqual(AppTheme.Light, form.Theme)
                Assert.AreEqual(ThemePalette.Light.WindowBack, form.BackColor)
                Assert.AreEqual(ThemePalette.Light.GridViewBack, form.PropertyGrid1.ViewBackColor)
                Assert.AreEqual(FlatStyle.Standard, form.CheckBox_TopMost.FlatStyle)
                Assert.AreNotSame(darkImage, form.Button_GitHub.Image, "The GitHub logo did not change with the theme.")
                StringAssert.Contains(form.ToolTip1.GetToolTip(form.Button_Theme), "dark")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the GitHub logo is white over the dark theme, and black over the light theme.
        ''' </summary>
        <TestMethod>
        Public Sub SetTheme_GitHubLogoContrastsWithBackground()

            Using form As New Main()
                form.SetTheme(AppTheme.Dark)
                Assert.IsTrue(MainFormTests.GetAverageOpaqueBrightness(DirectCast(form.Button_GitHub.Image, Bitmap)) > 0.8, "The logo is not light in the dark theme.")

                form.SetTheme(AppTheme.Light)
                Assert.IsTrue(MainFormTests.GetAverageOpaqueBrightness(DirectCast(form.Button_GitHub.Image, Bitmap)) < 0.2, "The logo is not dark in the light theme.")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that switching the theme of a form whose handle is created (dark title bar, dark scroll bars) does not throw.
        ''' </summary>
        <TestMethod>
        Public Sub SetTheme_WithHandleCreated_DoesNotThrow()

            Using form As New Main()
                form.PropertyGrid1.SelectedObject = New PixelColorPropertyInfo(Color.Red)
                Dim unused As IntPtr = form.Handle

                form.SetTheme(AppTheme.Dark)
                form.SetTheme(AppTheme.Light)
                form.SetTheme(AppTheme.Dark)
            End Using
        End Sub

#End Region

#Region " View Mode "

        ''' <summary>
        ''' Verifies that the expanded mode makes the window taller, and the compact mode restores the compact size.
        ''' </summary>
        <TestMethod>
        Public Sub SetViewMode_ResizesWindow()

            Using form As New Main()
                form.SetViewMode(False)
                Dim compact As Size = form.ClientSize
                Assert.AreEqual(form.CompactModeClientSize, compact)
                Assert.IsFalse(form.IsExpandedMode)
                StringAssert.Contains(form.ToolTip1.GetToolTip(form.Button_ViewMode), "expanded")

                form.SetViewMode(True)
                Assert.IsTrue(form.IsExpandedMode)
                Assert.AreEqual(Main.GetExpandedClientSize(compact), form.ClientSize)
                StringAssert.Contains(form.ToolTip1.GetToolTip(form.Button_ViewMode), "compact")

                form.SetViewMode(False)
                Assert.AreEqual(compact, form.ClientSize)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies the size of the client area in expanded mode: same width, taller.
        ''' </summary>
        <TestMethod>
        Public Sub GetExpandedClientSize_KeepsWidthAndIncreasesHeight()

            Dim compact As New Size(404, 261)
            Dim expanded As Size = Main.GetExpandedClientSize(compact)

            Assert.AreEqual(compact.Width, expanded.Width)
            Assert.IsTrue(expanded.Height > compact.Height * 2, $"Expanded height: {expanded.Height}")
            Assert.IsTrue(expanded.Height < Screen.PrimaryScreen.WorkingArea.Height OrElse Screen.PrimaryScreen.WorkingArea.Height < 600,
                          "The expanded window does not fit in a common screen.")
        End Sub

#End Region

#Region " Hotkeys Hint "

        ''' <summary>
        ''' Verifies the hotkeys hint text, depending on which global hotkeys could be registered.
        ''' </summary>
        <TestMethod>
        Public Sub BuildHotkeysHintText_ReflectsRegisteredHotkeys()

            Dim both As String = Main.BuildHotkeysHintText(True, True)
            StringAssert.Contains(both, "Ctrl+Alt+C: copy")
            StringAssert.Contains(both, "Ctrl+Alt+M: show or hide the magnifier")

            Dim none As String = Main.BuildHotkeysHintText(False, False)
            StringAssert.Contains(none, "Ctrl+C (this window focused)")
            Assert.IsFalse(none.Contains("Ctrl+Alt"), "Unregistered hotkeys are announced.")

            For Each text As String In {both, none, Main.BuildHotkeysHintText(True, False), Main.BuildHotkeysHintText(False, True)}
                Assert.AreEqual(2, text.Split({Environment.NewLine}, StringSplitOptions.None).Length, $"Not two lines: {text}")
            Next
        End Sub

#End Region

#Region " Magnifier "

        ''' <summary>
        ''' Verifies that the magnifier button shows and hides the magnifier.
        ''' </summary>
        <TestMethod>
        Public Sub ToggleMagnifier_ShowsAndHidesMagnifier()

            Using form As New Main()
                form.ToggleMagnifier()
                TestWindow.PumpMessages()
                Assert.IsTrue(form.IsMagnifierVisible)
                Assert.IsTrue(form.CheckBox_Magnifier.Checked)

                form.ToggleMagnifier()
                TestWindow.PumpMessages()
                Assert.IsFalse(form.IsMagnifierVisible)
                Assert.IsFalse(form.CheckBox_Magnifier.Checked)
            End Using
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Returns the average brightness (0 to 1) of the opaque pixels of an image.
        ''' </summary>
        '''
        ''' <param name="bmp">The image.</param>
        '''
        ''' <returns>The average brightness.</returns>
        Private Shared Function GetAverageOpaqueBrightness(bmp As Bitmap) As Double

            Dim total As Double = 0
            Dim count As Integer = 0
            For y As Integer = 0 To bmp.Height - 1
                For x As Integer = 0 To bmp.Width - 1
                    Dim px As Color = bmp.GetPixel(x, y)
                    If px.A > 200 Then
                        total += px.GetBrightness()
                        count += 1
                    End If
                Next
            Next

            Assert.IsTrue(count > 0, "The image has no opaque pixels.")
            Return total / count
        End Function

#End Region

    End Class

End Namespace

#End Region
