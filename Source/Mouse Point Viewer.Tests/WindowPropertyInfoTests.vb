#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " WindowPropertyInfoTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the window-related property info classes:
    ''' <see cref="WindowHandlePropertyInfo"/>, <see cref="CaptionPropertyInfo"/> and <see cref="ClassNamePropertyInfo"/>.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class WindowPropertyInfoTests

#Region " WindowHandlePropertyInfo "

        ''' <summary>
        ''' Verifies the decimal and hexadecimal representations of a window handle.
        ''' </summary>
        '''
        ''' <param name="handle">
        ''' The window handle value.
        ''' </param>
        '''
        ''' <param name="expectedHex">
        ''' The expected hexadecimal representation.
        ''' </param>
        '''
        ''' <param name="expectedString">
        ''' The expected string representation.
        ''' </param>
        <TestMethod>
        <DataRow(0, "0x0", "0 (0x0)")>
        <DataRow(255, "0xFF", "255 (0xFF)")>
        <DataRow(1715004, "0x1A2B3C", "1715004 (0x1A2B3C)")>
        <DataRow(Integer.MaxValue, "0x7FFFFFFF", "2147483647 (0x7FFFFFFF)")>
        Public Sub WindowHandlePropertyInfo_ValuesAndToString(handle As Integer, expectedHex As String, expectedString As String)

            Dim info As New WindowHandlePropertyInfo(New IntPtr(handle))

            Assert.AreEqual(CLng(handle), info.Decimal)
            Assert.AreEqual(expectedHex, info.Hexadecimal)
            Assert.AreEqual(expectedString, info.ToString())
        End Sub

#End Region

#Region " CaptionPropertyInfo "

        ''' <summary>
        ''' Verifies the text, the length and the string representation of a window caption.
        ''' </summary>
        '''
        ''' <param name="text">
        ''' The caption text.
        ''' </param>
        '''
        ''' <param name="expectedLength">
        ''' The expected length.
        ''' </param>
        <TestMethod>
        <DataRow("Untitled - Notepad", 18)>
        <DataRow("", 0)>
        <DataRow("日本語 ✓", 5)>
        Public Sub CaptionPropertyInfo_ValuesAndToString(text As String, expectedLength As Integer)

            Dim info As New CaptionPropertyInfo With {.Text = text}

            Assert.AreEqual(text, info.Text)
            Assert.AreEqual(expectedLength, info.Length)
            Assert.AreEqual(text, info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies that a caption without text has a length of zero.
        ''' </summary>
        <TestMethod>
        Public Sub CaptionPropertyInfo_NullText_LengthIsZero()

            Dim info As New CaptionPropertyInfo()

            Assert.AreEqual(0, info.Length)
        End Sub

#End Region

#Region " ClassNamePropertyInfo "

        ''' <summary>
        ''' Verifies the values and the string representation of a window class name.
        ''' </summary>
        <TestMethod>
        Public Sub ClassNamePropertyInfo_ValuesAndToString()

            Dim info As New ClassNamePropertyInfo With {
                .Name = "WindowsForms10.BUTTON.app.0.141b42a_r6_ad1",
                .RealName = "Button"
            }

            Assert.AreEqual("WindowsForms10.BUTTON.app.0.141b42a_r6_ad1", info.Name)
            Assert.AreEqual("Button", info.RealName)
            Assert.AreEqual(info.Name, info.ToString())
        End Sub

#End Region

    End Class

End Namespace

#End Region
