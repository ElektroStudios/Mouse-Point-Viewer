#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Windows.Forms

Imports Win32

#End Region

#Region " GlobalHotkeyManagerTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Tests for the <see cref="GlobalHotkeyManager"/> class.
    ''' <para></para>
    ''' The registration tests use an unusual key combination (Ctrl+Shift+Alt+F24) that no application should use.
    ''' </summary>
    <STATestClass>
    <TestCategory("UI")>
    Public NotInheritable Class GlobalHotkeyManagerTests

#Region " Constants "

        ''' <summary>
        ''' The modifier keys of the test hotkey.
        ''' </summary>
        Private Const TestModifiers As HotkeyModifiers = HotkeyModifiers.Control Or HotkeyModifiers.Shift Or HotkeyModifiers.Alt

        ''' <summary>
        ''' The key of the test hotkey.
        ''' </summary>
        Private Const TestKey As Keys = Keys.F24

#End Region

#Region " HotkeyModifiers "

        ''' <summary>
        ''' Verifies that the <see cref="HotkeyModifiers"/> values match the Win32 <c>MOD_*</c> values.
        ''' </summary>
        <TestMethod>
        Public Sub HotkeyModifiers_MatchWin32Values()

            Assert.AreEqual(&H1UI, CUInt(HotkeyModifiers.Alt))
            Assert.AreEqual(&H2UI, CUInt(HotkeyModifiers.Control))
            Assert.AreEqual(&H4UI, CUInt(HotkeyModifiers.Shift))
            Assert.AreEqual(&H8UI, CUInt(HotkeyModifiers.Windows))
            Assert.AreEqual(&H4000UI, CUInt(HotkeyModifiers.NoRepeat))
            Assert.AreEqual(&H312, Constants.WM_HOTKEY)
        End Sub

#End Region

#Region " Formatting "

        ''' <summary>
        ''' Verifies the human-readable text of hotkeys.
        ''' </summary>
        '''
        ''' <param name="modifiers">The modifier keys (<see cref="HotkeyModifiers"/> value).</param>
        ''' <param name="key">The key.</param>
        ''' <param name="expected">The expected text.</param>
        <TestMethod>
        <DataRow(CUInt(HotkeyModifiers.Control Or HotkeyModifiers.Alt), Keys.C, "Ctrl+Alt+C")>
        <DataRow(CUInt(HotkeyModifiers.Control Or HotkeyModifiers.Alt Or HotkeyModifiers.NoRepeat), Keys.M, "Ctrl+Alt+M")>
        <DataRow(CUInt(HotkeyModifiers.None), Keys.Escape, "Esc")>
        <DataRow(CUInt(HotkeyModifiers.NoRepeat), Keys.Escape, "Esc")>
        <DataRow(CUInt(HotkeyModifiers.Windows Or HotkeyModifiers.Alt Or HotkeyModifiers.Shift Or HotkeyModifiers.Control), Keys.D5, "Ctrl+Shift+Alt+Win+5")>
        <DataRow(CUInt(HotkeyModifiers.Control Or HotkeyModifiers.Alt), Keys.Oemplus, "Ctrl+Alt++")>
        <DataRow(CUInt(HotkeyModifiers.Control Or HotkeyModifiers.Alt), Keys.Subtract, "Ctrl+Alt+Num -")>
        <DataRow(CUInt(HotkeyModifiers.Shift), Keys.F12, "Shift+F12")>
        Public Sub FormatHotkey_ReturnsReadableText(modifiers As UInteger, key As Keys, expected As String)

            Assert.AreEqual(expected, GlobalHotkeyManager.FormatHotkey(CType(modifiers, HotkeyModifiers), key))
        End Sub

        ''' <summary>
        ''' Verifies the human-readable text of modifier keys.
        ''' </summary>
        <TestMethod>
        Public Sub FormatModifiers_ReturnsReadableText()

            Assert.AreEqual("Ctrl+Alt", GlobalHotkeyManager.FormatModifiers(HotkeyModifiers.Control Or HotkeyModifiers.Alt Or HotkeyModifiers.NoRepeat))
            Assert.AreEqual("", GlobalHotkeyManager.FormatModifiers(HotkeyModifiers.None))
            Assert.AreEqual("Win", GlobalHotkeyManager.FormatModifiers(HotkeyModifiers.Windows))
        End Sub

#End Region

#Region " Registration "

        ''' <summary>
        ''' Verifies that a hotkey is registered, and that the same hotkey cannot be registered twice system-wide
        ''' until it is unregistered.
        ''' </summary>
        <TestMethod>
        Public Sub TryRegister_ThenUnregister_ReleasesHotkey()

            Using first As New GlobalHotkeyManager(),
                  second As New GlobalHotkeyManager()

                Dim id As Integer
                Assert.IsTrue(first.TryRegister(TestModifiers, TestKey, id), "The test hotkey is in use by another application.")
                Assert.AreNotEqual(0, id)
                Assert.IsTrue(first.IsRegistered(id))
                Assert.AreEqual(1, first.Count)

                Dim otherId As Integer = -1
                Assert.IsFalse(second.TryRegister(TestModifiers, TestKey, otherId), "The same hotkey was registered twice.")
                Assert.AreEqual(0, otherId)
                Assert.AreEqual(0, second.Count)

                first.Unregister(id)
                Assert.IsFalse(first.IsRegistered(id))
                Assert.AreEqual(0, first.Count)

                Assert.IsTrue(second.TryRegister(TestModifiers, TestKey, otherId), "The hotkey was not released.")
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that disposing the manager unregisters all its hotkeys.
        ''' </summary>
        <TestMethod>
        Public Sub Dispose_UnregistersAllHotkeys()

            Dim id As Integer
            Using first As New GlobalHotkeyManager()
                Assert.IsTrue(first.TryRegister(TestModifiers, TestKey, id), "The test hotkey is in use by another application.")
                Assert.IsTrue(first.TryRegister(TestModifiers, Keys.F23, id), "The test hotkey is in use by another application.")
            End Using

            Using second As New GlobalHotkeyManager()
                Assert.IsTrue(second.TryRegister(TestModifiers, TestKey, id))
                Assert.IsTrue(second.TryRegister(TestModifiers, Keys.F23, id))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that each registered hotkey gets a different identifier.
        ''' </summary>
        <TestMethod>
        Public Sub TryRegister_AssignsUniqueIds()

            Using manager As New GlobalHotkeyManager()
                Dim ids As New HashSet(Of Integer)
                Dim id As Integer
                For Each key As Keys In {Keys.F21, Keys.F22, Keys.F23, Keys.F24}
                    Assert.IsTrue(manager.TryRegister(TestModifiers, key, id), $"The test hotkey {key} is in use by another application.")
                    Assert.IsTrue(ids.Add(id), $"Duplicated identifier: {id}")
                Next
                Assert.AreEqual(4, manager.Count)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that invalid keys throw <see cref="ArgumentException"/>.
        ''' </summary>
        '''
        ''' <param name="key">The invalid key.</param>
        <TestMethod>
        <DataRow(Keys.None)>
        <DataRow(Keys.Control Or Keys.C)>
        <DataRow(Keys.Alt Or Keys.F24)>
        Public Sub TryRegister_InvalidKey_ThrowsArgumentException(key As Keys)

            Using manager As New GlobalHotkeyManager()
                Dim id As Integer
                Assert.ThrowsExactly(Of ArgumentException)(Function() manager.TryRegister(TestModifiers, key, id))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that unregistering an unknown identifier throws <see cref="ArgumentException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub Unregister_UnknownId_ThrowsArgumentException()

            Using manager As New GlobalHotkeyManager()
                Assert.ThrowsExactly(Of ArgumentException)(Sub() manager.Unregister(12345))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a disposed manager throws <see cref="ObjectDisposedException"/>, and that it can be disposed twice.
        ''' </summary>
        <TestMethod>
        Public Sub Disposed_ThrowsObjectDisposedException()

            Dim manager As New GlobalHotkeyManager()
            manager.Dispose()
            manager.Dispose()

            Dim id As Integer
            Assert.ThrowsExactly(Of ObjectDisposedException)(Function() manager.TryRegister(TestModifiers, TestKey, id))
            Assert.ThrowsExactly(Of ObjectDisposedException)(Sub() manager.Unregister(1))
        End Sub

#End Region

#Region " HotkeyPressed "

        ''' <summary>
        ''' Verifies that a <c>WM_HOTKEY</c> message for a registered hotkey raises <see cref="GlobalHotkeyManager.HotkeyPressed"/>
        ''' with the identifier, the modifier keys (without <see cref="HotkeyModifiers.NoRepeat"/>) and the key.
        ''' </summary>
        <TestMethod>
        Public Sub WmHotkey_RegisteredId_RaisesHotkeyPressed()

            Using manager As New GlobalHotkeyManager()
                Dim id As Integer
                Assert.IsTrue(manager.TryRegister(TestModifiers Or HotkeyModifiers.NoRepeat, TestKey, id), "The test hotkey is in use by another application.")

                Dim received As New List(Of HotkeyPressedEventArgs)
                AddHandler manager.HotkeyPressed, Sub(sender As Object, e As HotkeyPressedEventArgs) received.Add(e)

                NativeMethods.SendMessage(manager.Handle, Constants.WM_HOTKEY, New IntPtr(id), IntPtr.Zero)

                Assert.AreEqual(1, received.Count)
                Assert.AreEqual(id, received(0).Id)
                Assert.AreEqual(TestModifiers, received(0).Modifiers)
                Assert.AreEqual(TestKey, received(0).Key)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that <c>WM_HOTKEY</c> messages for unknown (or unregistered) identifiers are ignored.
        ''' </summary>
        <TestMethod>
        Public Sub WmHotkey_UnknownId_IsIgnored()

            Using manager As New GlobalHotkeyManager()
                Dim id As Integer
                Assert.IsTrue(manager.TryRegister(TestModifiers, TestKey, id), "The test hotkey is in use by another application.")
                manager.Unregister(id)

                Dim raised As Boolean
                AddHandler manager.HotkeyPressed, Sub(sender As Object, e As HotkeyPressedEventArgs) raised = True

                NativeMethods.SendMessage(manager.Handle, Constants.WM_HOTKEY, New IntPtr(id), IntPtr.Zero)
                NativeMethods.SendMessage(manager.Handle, Constants.WM_HOTKEY, New IntPtr(9999), IntPtr.Zero)

                Assert.IsFalse(raised)
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the hidden window that receives the hotkey messages is a message-only window
        ''' (it is not a top-level window, so it never appears on the desktop).
        ''' </summary>
        <TestMethod>
        Public Sub Window_IsMessageOnly()

            Using manager As New GlobalHotkeyManager()
                Assert.AreNotEqual(IntPtr.Zero, manager.Handle)
                Assert.IsFalse(WindowHelper.EnumTopLevelWindowsInZOrder().Contains(manager.Handle))
            End Using
        End Sub

#End Region

    End Class

End Namespace

#End Region
