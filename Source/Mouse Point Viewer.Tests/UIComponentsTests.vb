#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Design
Imports System.Windows.Forms

Imports DevCase.Extensions.PropertyGridExtensions

Imports MousePointViewer.Tests.Helpers

#End Region

#Region " UIComponentsTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Tests for the user interface components:
    ''' <see cref="PixelColorEditor"/>, the <see cref="PropertyGrid"/> extensions, and the <see cref="Main"/> form.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class UIComponentsTests

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

#Region " PixelColorEditor "

        ''' <summary>
        ''' Verifies that the editor only paints a value preview, without a drop-down or modal editor.
        ''' </summary>
        <TestMethod>
        Public Sub PixelColorEditor_PaintsPreviewOnly()

            Dim editor As New PixelColorEditor()

            Assert.AreEqual(UITypeEditorEditStyle.None, editor.GetEditStyle(Nothing))
            Assert.IsTrue(editor.GetPaintValueSupported(Nothing))
        End Sub

        ''' <summary>
        ''' Verifies that the preview box is filled with the pixel color, and only within the given bounds.
        ''' </summary>
        <TestMethod>
        Public Sub PixelColorEditor_PaintValue_FillsBoundsWithColor()

            Dim c As Color = Color.FromArgb(12, 200, 99)

            Using bmp As New Bitmap(20, 20),
                  g As Graphics = Graphics.FromImage(bmp)

                g.Clear(Color.White)
                Dim bounds As New Rectangle(2, 3, 10, 8)
                Dim editor As New PixelColorEditor()
                editor.PaintValue(New PaintValueEventArgs(Nothing, New PixelColorPropertyInfo(c), g, bounds))

                Assert.AreEqual(c.ToArgb(), bmp.GetPixel(2, 3).ToArgb(), "Top-left corner of the bounds.")
                Assert.AreEqual(c.ToArgb(), bmp.GetPixel(11, 10).ToArgb(), "Bottom-right corner of the bounds.")
                Assert.AreEqual(Color.White.ToArgb(), bmp.GetPixel(12, 10).ToArgb(), "Outside the bounds (right).")
                Assert.AreEqual(Color.White.ToArgb(), bmp.GetPixel(1, 3).ToArgb(), "Outside the bounds (left).")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that values of other types (the grid may pass them) are not painted and do not throw.
        ''' </summary>
        <TestMethod>
        Public Sub PixelColorEditor_PaintValue_OtherValueType_PaintsNothing()

            Using bmp As New Bitmap(10, 10),
                  g As Graphics = Graphics.FromImage(bmp)

                g.Clear(Color.White)
                Dim editor As New PixelColorEditor()
                editor.PaintValue(New PaintValueEventArgs(Nothing, Color.Red, g, New Rectangle(0, 0, 10, 10)))

                Assert.AreEqual(Color.White.ToArgb(), bmp.GetPixel(5, 5).ToArgb())
            End Using
        End Sub

#End Region

#Region " PropertyGrid Extensions "

        ''' <summary>
        ''' Verifies that a <see cref="PropertyGrid"/> that has never been displayed is not initialized,
        ''' and that setting its splitter position throws <see cref="InvalidOperationException"/>.
        ''' <para></para>
        ''' This also verifies that the reflection on the internal members of the
        ''' <see cref="PropertyGrid"/> works with the installed .NET Framework.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_NotDisplayed_IsNotInitialized()

            Using grid As New PropertyGrid()
                Assert.IsFalse(grid.IsInitialized())
                Assert.ThrowsExactly(Of InvalidOperationException)(Sub() grid.SetSplitterPosition(100))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a displayed <see cref="PropertyGrid"/> is initialized,
        ''' and that its splitter can be moved.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_Displayed_IsInitialized_AndSplitterCanBeMoved()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim grid As New PropertyGrid() With {.Dock = DockStyle.Fill, .SelectedObject = New PixelColorPropertyInfo(Color.Red)}
                window.Form.Controls.Add(grid)
                window.ShowAndPaint()

                Assert.IsTrue(grid.IsInitialized())
                grid.SetSplitterPosition(100)
                grid.SetSplitterPosition(150)
            End Using
        End Sub

#End Region

#Region " Main Form "

        ''' <summary>
        ''' Verifies the default state of the option controls of the main form.
        ''' </summary>
        <TestMethod>
        Public Sub MainForm_DefaultState()

            Using form As New Main()
                Assert.IsFalse(form.CheckBox_IgnoreChildWindows.Checked)
                Assert.IsFalse(form.CheckBox_IgnoreLayeredWindows.Checked)
                Assert.IsFalse(form.CheckBox_IgnoreTransparentWindows.Checked)
                Assert.IsFalse(form.CheckBox_TopMost.Checked)
                Assert.AreEqual(10D, form.NumericUpDown_Refresh.Minimum)
                Assert.AreEqual(5000D, form.NumericUpDown_Refresh.Maximum)
                Assert.AreEqual(100D, form.NumericUpDown_Refresh.Value)
                Assert.AreEqual(100, form.RefreshTimer.Interval)
                Assert.IsFalse(form.RefreshTimer.Enabled, "The timer must not start before the form is shown.")
                Assert.IsTrue(form.KeyPreview, "KeyPreview is required for the Ctrl+C shortcut.")
                Assert.IsNotNull(form.Icon)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that each "Ignore ..." check box updates its corresponding window search option.
        ''' </summary>
        <TestMethod>
        Public Sub MainForm_IgnoreCheckBoxes_UpdateWindowSearchOptions()

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
        Public Sub MainForm_TopMostCheckBox_UpdatesTopMost()

            Using form As New Main()
                form.CheckBox_TopMost.Checked = True
                Assert.IsTrue(form.TopMost)

                form.CheckBox_TopMost.Checked = False
                Assert.IsFalse(form.TopMost)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the refresh rate text updates the timer interval only when it is a valid integer
        ''' within the allowed range.
        ''' </summary>
        '''
        ''' <param name="text">
        ''' The refresh rate text typed by the user.
        ''' </param>
        '''
        ''' <param name="expectedInterval">
        ''' The expected timer interval.
        ''' </param>
        <TestMethod>
        <DataRow("250", 250)>
        <DataRow("10", 10)>
        <DataRow("5000", 5000)>
        <DataRow("9", 100)>
        <DataRow("5001", 100)>
        <DataRow("abc", 100)>
        <DataRow("", 100)>
        Public Sub MainForm_RefreshRateText_UpdatesTimerIntervalWhenValid(text As String, expectedInterval As Integer)

            Using form As New Main()
                form.RefreshTimer.Interval = 100

                form.NumericUpDown_Refresh.Text = text

                Assert.AreEqual(expectedInterval, form.RefreshTimer.Interval)
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
