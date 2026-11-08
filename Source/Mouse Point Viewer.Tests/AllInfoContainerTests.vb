#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

#End Region

#Region " AllInfoContainerTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the <see cref="AllInfoContainer"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class AllInfoContainerTests

#Region " ToString "

        ''' <summary>
        ''' Verifies the exact text copied to the clipboard with the Ctrl+C keyboard shortcut.
        ''' </summary>
        <TestMethod>
        Public Sub ToString_Populated_ReturnsClipboardText()

            Dim info As AllInfoContainer = AllInfoContainerTests.CreatePopulated()

            Dim expectedLines As String() = {
                "Process",
                $"{vbTab}Name: notepad.exe",
                $"{vbTab}ID: 1234",
                $"{vbTab}Architecture: 64-bit",
                "",
                "Window Caption",
                $"{vbTab}Text: Untitled - Notepad",
                $"{vbTab}Length: 18",
                "",
                "Window Class",
                $"{vbTab}Name: Notepad",
                $"{vbTab}Real Name: Notepad",
                "",
                "Window Handle",
                $"{vbTab}Decimal: 1715004",
                $"{vbTab}Hexadecimal: 0x1A2B3C",
                "",
                "Window Bounds",
                $"{vbTab}Rectangle",
                $"{vbTab}{vbTab}X: 100",
                $"{vbTab}{vbTab}Y: 50",
                $"{vbTab}{vbTab}Width: 640",
                $"{vbTab}{vbTab}Height: 480",
                $"{vbTab}Client Rectangle",
                $"{vbTab}{vbTab}X: 108",
                $"{vbTab}{vbTab}Y: 81",
                $"{vbTab}{vbTab}Width: 624",
                $"{vbTab}{vbTab}Height: 441",
                "",
                "Mouse Point",
                $"{vbTab}Screen",
                $"{vbTab}{vbTab}X: 300",
                $"{vbTab}{vbTab}Y: 200",
                $"{vbTab}Client Window",
                $"{vbTab}{vbTab}X: 192",
                $"{vbTab}{vbTab}Y: 119",
                "",
                "Pixel Color",
                $"{vbTab}RGB: 255, 0, 0",
                $"{vbTab}Win32: 255",
                $"{vbTab}Web: #FF0000",
                $"{vbTab}Web Name: Red"
            }
            Dim expected As String = String.Join(Environment.NewLine, expectedLines) & Environment.NewLine

            Assert.AreEqual(expected, info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies that an unavailable process name is written as <see cref="ProcessPropertyInfo.UnavailableName"/>
        ''' instead of a bare ".exe".
        ''' </summary>
        <TestMethod>
        Public Sub ToString_UnavailableProcessName_WritesUnavailable()

            Dim info As AllInfoContainer = AllInfoContainerTests.CreatePopulated()
            info.Process = New ProcessPropertyInfo(Nothing, 1234, ProcessPropertyInfo.ArchitectureUnknown)

            StringAssert.Contains(info.ToString(), $"{vbTab}Name: {ProcessPropertyInfo.UnavailableName}{Environment.NewLine}")
        End Sub

        ''' <summary>
        ''' Verifies that requesting the text before the information is populated
        ''' (e.g. pressing Ctrl+C before the first refresh) throws <see cref="InvalidOperationException"/>
        ''' instead of a <see cref="NullReferenceException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub ToString_NotPopulated_ThrowsInvalidOperationException()

            Dim info As New AllInfoContainer()

            Assert.ThrowsExactly(Of InvalidOperationException)(Sub() info.ToString())
        End Sub

#End Region

#Region " IsPopulated "

        ''' <summary>
        ''' Verifies that a new instance is not populated, and a fully populated instance is.
        ''' </summary>
        <TestMethod>
        Public Sub IsPopulated_NewAndFullyPopulated()

            Assert.IsFalse(New AllInfoContainer().IsPopulated)
            Assert.IsTrue(AllInfoContainerTests.CreatePopulated().IsPopulated)
        End Sub

        ''' <summary>
        ''' Verifies that the instance is not populated when any single piece of information is missing.
        ''' </summary>
        <TestMethod>
        Public Sub IsPopulated_AnyMissingPart_ReturnsFalse()

            Dim removers As New List(Of Action(Of AllInfoContainer)) From {
                Sub(i) i.Process = Nothing,
                Sub(i) i.Caption = Nothing,
                Sub(i) i.ClassName = Nothing,
                Sub(i) i.WindowHandle = Nothing,
                Sub(i) i.WindowBounds = Nothing,
                Sub(i) i.WindowBounds.RectangleInfo = Nothing,
                Sub(i) i.WindowBounds.ClientRectangleInfo = Nothing,
                Sub(i) i.Point = Nothing,
                Sub(i) i.Point.ScreenPointInfo = Nothing,
                Sub(i) i.Point.ClientPointInfo = Nothing,
                Sub(i) i.PixelColor = Nothing
            }

            For index As Integer = 0 To removers.Count - 1
                Dim info As AllInfoContainer = AllInfoContainerTests.CreatePopulated()
                removers(index).Invoke(info)

                Assert.IsFalse(info.IsPopulated, $"Remover #{index} did not make the instance unpopulated.")
                Assert.ThrowsExactly(Of InvalidOperationException)(Sub() info.ToString(), $"Remover #{index}")
            Next
        End Sub

#End Region

#Region " Attributes "

        ''' <summary>
        ''' Verifies the properties displayed in the <see cref="PropertyGrid"/>, and their display names, in declaration order.
        ''' </summary>
        <TestMethod>
        Public Sub BrowsableProperties_HaveExpectedDisplayNames()

            Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(GetType(AllInfoContainer), {BrowsableAttribute.Yes})
            Dim names As New List(Of String)()
            For Each pd As PropertyDescriptor In props
                names.Add(pd.DisplayName)
                Assert.IsTrue(pd.IsReadOnly, $"Property '{pd.Name}' is not read-only.")
            Next

            CollectionAssert.AreEquivalent(
                {"Process", "Window Caption", "Window Class", "Window Handle", "Window Bounds", "Mouse Point", "Pixel Color"},
                names)
        End Sub

        ''' <summary>
        ''' Verifies that every nested information type is expandable in the <see cref="PropertyGrid"/>.
        ''' </summary>
        '''
        ''' <param name="typeName">
        ''' The name of the information type.
        ''' </param>
        <TestMethod>
        <DataRow(NameOf(ProcessPropertyInfo))>
        <DataRow(NameOf(CaptionPropertyInfo))>
        <DataRow(NameOf(ClassNamePropertyInfo))>
        <DataRow(NameOf(WindowHandlePropertyInfo))>
        <DataRow(NameOf(WindowBoundsPropertyContainer))>
        <DataRow(NameOf(RectanglePropertyInfo))>
        <DataRow(NameOf(PointPropertyContainer))>
        <DataRow(NameOf(PointPropertyInfo))>
        Public Sub NestedTypes_AreExpandable(typeName As String)

            Dim t As Type = GetType(AllInfoContainer).Assembly.GetType(typeName, throwOnError:=True)

            Assert.IsInstanceOfType(Of ExpandableObjectConverter)(TypeDescriptor.GetConverter(t))
        End Sub

        ''' <summary>
        ''' Verifies that the pixel color property is expandable in the <see cref="PropertyGrid"/>
        ''' (through the attribute of the property, as its type has a custom editor).
        ''' </summary>
        <TestMethod>
        Public Sub PixelColorProperty_IsExpandable()

            Dim pd As PropertyDescriptor = TypeDescriptor.GetProperties(GetType(AllInfoContainer))(NameOf(AllInfoContainer.PixelColor))

            Assert.IsInstanceOfType(Of ExpandableObjectConverter)(pd.Converter)
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Creates a fully populated <see cref="AllInfoContainer"/> with known values.
        ''' </summary>
        '''
        ''' <returns>
        ''' The populated <see cref="AllInfoContainer"/>.
        ''' </returns>
        Private Shared Function CreatePopulated() As AllInfoContainer

            Return New AllInfoContainer With {
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
                .PixelColor = New PixelColorPropertyInfo(Color.FromArgb(255, 0, 0))
            }
        End Function

#End Region

    End Class

End Namespace

#End Region
