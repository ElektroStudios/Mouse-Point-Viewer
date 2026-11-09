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
    ''' <see cref="PixelColorEditor"/> and the <see cref="PropertyGrid"/> extensions.
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

#Region " PropertyGrid Extensions (Grid Items) "

        ''' <summary>
        ''' Verifies that the root grid item is found, and that its children are the properties of the selected object.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_GetRootGridItem_ReturnsRootWithTopLevelProperties()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim grid As PropertyGrid = UIComponentsTests.CreateGridWithInfo(window)

                Dim root As GridItem = grid.GetRootGridItem()

                Assert.AreEqual(GridItemType.Root, root.GridItemType)
                Assert.IsNull(root.Parent)
                Assert.AreEqual(7, root.GridItems.Count)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the root grid item is also found when a nested item is selected.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_GetRootGridItem_NestedItemSelected_ReturnsRoot()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim grid As PropertyGrid = UIComponentsTests.CreateGridWithInfo(window)
                Dim firstTopLevel As GridItem = grid.GetRootGridItem().GridItems(0)
                firstTopLevel.Expanded = True
                grid.SelectedGridItem = firstTopLevel.GridItems(0)

                Dim root As GridItem = grid.GetRootGridItem()

                Assert.AreEqual(GridItemType.Root, root.GridItemType)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a <see cref="PropertyGrid"/> without selected object throws <see cref="InvalidOperationException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_GetRootGridItem_NoSelectedObject_ThrowsInvalidOperationException()

            Using grid As New PropertyGrid()
                Assert.ThrowsExactly(Of InvalidOperationException)(Function() grid.GetRootGridItem())
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the top-level items are expanded (only one level), and then collapsed.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_SetTopLevelItemsExpanded_ExpandsOneLevelAndCollapses()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim grid As PropertyGrid = UIComponentsTests.CreateGridWithInfo(window)

                grid.SetTopLevelItemsExpanded(True)
                For Each item As GridItem In grid.GetRootGridItem().GridItems
                    Assert.IsTrue(item.Expanded, $"'{item.Label}' is not expanded.")
                    For Each child As GridItem In item.GridItems
                        Assert.IsFalse(child.Expanded, $"'{item.Label} > {child.Label}' is expanded.")
                    Next
                Next

                grid.SetTopLevelItemsExpanded(False)
                For Each item As GridItem In grid.GetRootGridItem().GridItems
                    Assert.IsFalse(item.Expanded, $"'{item.Label}' is not collapsed.")
                Next
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the expanded state of the top-level items survives the refresh performed on every timer tick,
        ''' in which all the nested information objects are replaced.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_ExpandedItems_SurviveRefreshWithNewValues()

            Using window As New TestWindow(TestWindow.GetCenteredBounds(), Color.White)
                Dim grid As PropertyGrid = UIComponentsTests.CreateGridWithInfo(window)
                grid.SetTopLevelItemsExpanded(True)

                Dim info As AllInfoContainer = DirectCast(grid.SelectedObject, AllInfoContainer)
                info.Process = New ProcessPropertyInfo("explorer", 4321, ProcessPropertyInfo.Architecture64Bit)
                info.PixelColor = New PixelColorPropertyInfo(Color.Blue)
                grid.Refresh()

                For Each item As GridItem In grid.GetRootGridItem().GridItems
                    Assert.IsTrue(item.Expanded, $"'{item.Label}' was collapsed by the refresh.")
                Next
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the internal grid view (the control that contains the scroll bars) is found.
        ''' </summary>
        <TestMethod>
        Public Sub PropertyGrid_GetGridView_ReturnsInternalPropertyGridView()

            Using grid As New PropertyGrid()
                Dim view As Control = grid.GetGridView()

                Assert.IsNotNull(view)
                Assert.AreEqual("PropertyGridView", view.GetType().Name)
                Assert.AreSame(grid, view.Parent)
            End Using
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Adds a docked <see cref="PropertyGrid"/> that displays a populated <see cref="AllInfoContainer"/>
        ''' to the specified test window, and shows the window.
        ''' </summary>
        '''
        ''' <param name="window">
        ''' The test window.
        ''' </param>
        '''
        ''' <returns>
        ''' The <see cref="PropertyGrid"/>.
        ''' </returns>
        Private Shared Function CreateGridWithInfo(window As TestWindow) As PropertyGrid

            Dim info As New AllInfoContainer With {
                .Process = New ProcessPropertyInfo("notepad", 1234, ProcessPropertyInfo.Architecture64Bit),
                .Caption = New CaptionPropertyInfo With {.Text = "Untitled - Notepad"},
                .ClassName = New ClassNamePropertyInfo With {.Name = "Notepad", .RealName = "Notepad"},
                .WindowHandle = New WindowHandlePropertyInfo(New IntPtr(&H1A2B3C)),
                .WindowBounds = New WindowBoundsPropertyContainer With {
                    .RectangleInfo = New RectanglePropertyInfo(New Rectangle(100, 50, 640, 480)),
                    .ClientRectangleInfo = New RectanglePropertyInfo(New Rectangle(108, 81, 624, 441))
                },
                .Point = New PointPropertyContainer With {
                    .ScreenPointInfo = New PointPropertyInfo(New Point(300, 200)),
                    .ClientPointInfo = New PointPropertyInfo(New Point(192, 119))
                },
                .PixelColor = New PixelColorPropertyInfo(Color.Red)
            }

            Dim grid As New PropertyGrid() With {
                .Dock = DockStyle.Fill,
                .PropertySort = PropertySort.NoSort,
                .ToolbarVisible = False,
                .HelpVisible = False,
                .SelectedObject = info
            }
            window.Form.Controls.Add(grid)
            window.ShowAndPaint()
            Return grid
        End Function

#End Region

    End Class

End Namespace

#End Region
