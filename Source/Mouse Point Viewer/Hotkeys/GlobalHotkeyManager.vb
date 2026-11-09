#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Collections.Generic
Imports System.Diagnostics

Imports Win32

#End Region

''' <summary>
''' Registers system-wide hotkeys through the Win32 <c>RegisterHotKey</c> function,
''' and raises the <see cref="GlobalHotkeyManager.HotkeyPressed"/> event when one of them is pressed.
''' <para></para>
''' Unlike a low-level keyboard hook (<c>WH_KEYBOARD_LL</c>), no hook is installed:
''' the system itself detects the key combination and only posts a <c>WM_HOTKEY</c> message when it is pressed,
''' so there is no impact on the input performance of the system.
''' <para></para>
''' The messages are received by a hidden message-only window, so the hotkeys work regardless of which window is focused.
''' This class must be created and used on a thread with a message loop (the UI thread).
''' </summary>
Friend NotInheritable Class GlobalHotkeyManager : Inherits NativeWindow : Implements IDisposable

#Region " Private Fields "

    ''' <summary>
    ''' The registered hotkeys, by identifier.
    ''' </summary>
    Private ReadOnly registeredHotkeys As New Dictionary(Of Integer, HotkeyPressedEventArgs)

    ''' <summary>
    ''' The identifier for the next registered hotkey.
    ''' <para></para>
    ''' Applications must use identifiers in the range 0x0000 through 0xBFFF.
    ''' </summary>
    Private nextId As Integer = 1

    ''' <summary>
    ''' Flag to detect redundant calls to <see cref="GlobalHotkeyManager.Dispose"/>.
    ''' </summary>
    Private isDisposed As Boolean

#End Region

#Region " Events "

    ''' <summary>
    ''' Occurs when a registered hotkey is pressed.
    ''' </summary>
    Friend Event HotkeyPressed As EventHandler(Of HotkeyPressedEventArgs)

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the number of currently registered hotkeys.
    ''' </summary>
    Friend ReadOnly Property Count As Integer
        Get
            Return Me.registeredHotkeys.Count
        End Get
    End Property

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="GlobalHotkeyManager"/> class,
    ''' creating the hidden message-only window that receives the hotkey messages.
    ''' </summary>
    Friend Sub New()

        Me.CreateHandle(New CreateParams() With {
            .Caption = NameOf(GlobalHotkeyManager),
            .Parent = Constants.HWND_MESSAGE
        })
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Tries to register a system-wide hotkey.
    ''' </summary>
    '''
    ''' <param name="modifiers">
    ''' The modifier keys. Add <see cref="HotkeyModifiers.NoRepeat"/> to ignore the keyboard auto-repeat.
    ''' </param>
    '''
    ''' <param name="key">
    ''' The key (without modifiers).
    ''' </param>
    '''
    ''' <param name="refId">
    ''' Receives the identifier of the registered hotkey, or 0 if the registration failed.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the hotkey was registered;
    ''' <see langword="False"/> if the hotkey is already registered by another application (or by this one).
    ''' </returns>
    '''
    ''' <exception cref="ObjectDisposedException">
    ''' Thrown when this instance has been disposed.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="key"/> contains modifier flags, or it is <see cref="Keys.None"/>.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Function TryRegister(modifiers As HotkeyModifiers, key As Keys, ByRef refId As Integer) As Boolean

        Me.ThrowIfDisposed()

        If key = Keys.None OrElse (key And Keys.Modifiers) <> Keys.None Then
            Throw New ArgumentException("The key must be a single key without modifier flags.", NameOf(key))
        End If

        refId = 0
        Dim id As Integer = Me.nextId
        If Not NativeMethods.RegisterHotKey(Me.Handle, id, CUInt(modifiers), CUInt(key)) Then
            Return False
        End If

        Me.nextId += 1
        Me.registeredHotkeys.Add(id, New HotkeyPressedEventArgs(id, modifiers And Not HotkeyModifiers.NoRepeat, key))
        refId = id
        Return True
    End Function

    ''' <summary>
    ''' Unregisters a hotkey previously registered by <see cref="GlobalHotkeyManager.TryRegister"/>.
    ''' </summary>
    '''
    ''' <param name="id">
    ''' The identifier of the hotkey.
    ''' </param>
    '''
    ''' <exception cref="ObjectDisposedException">
    ''' Thrown when this instance has been disposed.
    ''' </exception>
    '''
    ''' <exception cref="ArgumentException">
    ''' Thrown when <paramref name="id"/> is not the identifier of a registered hotkey.
    ''' </exception>
    '''
    ''' <exception cref="ComponentModel.Win32Exception">
    ''' Thrown when the hotkey cannot be unregistered.
    ''' </exception>
    <DebuggerStepThrough>
    Friend Sub Unregister(id As Integer)

        Me.ThrowIfDisposed()

        If Not Me.registeredHotkeys.ContainsKey(id) Then
            Throw New ArgumentException($"There is no registered hotkey with identifier {id}.", NameOf(id))
        End If

        If Not NativeMethods.UnregisterHotKey(Me.Handle, id) Then
            Throw New ComponentModel.Win32Exception()
        End If

        Me.registeredHotkeys.Remove(id)
    End Sub

    ''' <summary>
    ''' Returns a value indicating whether the specified identifier belongs to a registered hotkey.
    ''' </summary>
    '''
    ''' <param name="id">
    ''' The identifier of the hotkey.
    ''' </param>
    '''
    ''' <returns>
    ''' <see langword="True"/> if the hotkey is registered, <see langword="False"/> otherwise.
    ''' </returns>
    Friend Function IsRegistered(id As Integer) As Boolean

        Return Me.registeredHotkeys.ContainsKey(id)
    End Function

    ''' <summary>
    ''' Returns the human-readable text of a hotkey (e.g. "Ctrl+Alt+C").
    ''' </summary>
    '''
    ''' <param name="modifiers">
    ''' The modifier keys (<see cref="HotkeyModifiers.NoRepeat"/> is ignored).
    ''' </param>
    '''
    ''' <param name="key">
    ''' The key.
    ''' </param>
    '''
    ''' <returns>
    ''' The human-readable text of the hotkey.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function FormatHotkey(modifiers As HotkeyModifiers, key As Keys) As String

        Dim modifiersText As String = GlobalHotkeyManager.FormatModifiers(modifiers)
        Dim keyText As String = GlobalHotkeyManager.FormatKey(key)
        Return If(modifiersText.Length = 0, keyText, $"{modifiersText}+{keyText}")
    End Function

    ''' <summary>
    ''' Returns the human-readable text of the modifier keys of a hotkey (e.g. "Ctrl+Alt").
    ''' </summary>
    '''
    ''' <param name="modifiers">
    ''' The modifier keys (<see cref="HotkeyModifiers.NoRepeat"/> is ignored).
    ''' </param>
    '''
    ''' <returns>
    ''' The human-readable text of the modifier keys, or an empty string if there are no modifier keys.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function FormatModifiers(modifiers As HotkeyModifiers) As String

        Dim parts As New List(Of String)(4)
        If (modifiers And HotkeyModifiers.Control) <> 0 Then parts.Add("Ctrl")
        If (modifiers And HotkeyModifiers.Shift) <> 0 Then parts.Add("Shift")
        If (modifiers And HotkeyModifiers.Alt) <> 0 Then parts.Add("Alt")
        If (modifiers And HotkeyModifiers.Windows) <> 0 Then parts.Add("Win")
        Return String.Join("+", parts)
    End Function

    ''' <summary>
    ''' Returns the human-readable text of a key (e.g. "C", "+", "Esc").
    ''' </summary>
    '''
    ''' <param name="key">
    ''' The key.
    ''' </param>
    '''
    ''' <returns>
    ''' The human-readable text of the key.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function FormatKey(key As Keys) As String

        Select Case key
            Case Keys.Oemplus : Return "+"
            Case Keys.OemMinus : Return "-"
            Case Keys.Add : Return "Num +"
            Case Keys.Subtract : Return "Num -"
            Case Keys.Escape : Return "Esc"
            Case Keys.D0 To Keys.D9 : Return (CInt(key) - CInt(Keys.D0)).ToString(Globalization.CultureInfo.InvariantCulture)
            Case Else : Return key.ToString()
        End Select
    End Function

#End Region

#Region " Protected Methods "

    ''' <summary>
    ''' Processes the window messages, raising <see cref="GlobalHotkeyManager.HotkeyPressed"/>
    ''' for the <c>WM_HOTKEY</c> messages of the registered hotkeys.
    ''' </summary>
    '''
    ''' <param name="m">
    ''' The window message.
    ''' </param>
    <DebuggerStepThrough>
    Protected Overrides Sub WndProc(ByRef m As Message)

        If m.Msg = Constants.WM_HOTKEY Then
            Dim id As Integer = m.WParam.ToInt32()
            Dim args As HotkeyPressedEventArgs = Nothing
            If Me.registeredHotkeys.TryGetValue(id, args) Then
                RaiseEvent HotkeyPressed(Me, args)
            End If
            m.Result = IntPtr.Zero
            Return
        End If

        MyBase.WndProc(m)
    End Sub

#End Region

#Region " Private Methods "

    ''' <summary>
    ''' Throws an <see cref="ObjectDisposedException"/> if this instance has been disposed.
    ''' </summary>
    Private Sub ThrowIfDisposed()

        If Me.isDisposed Then
            Throw New ObjectDisposedException(NameOf(GlobalHotkeyManager))
        End If
    End Sub

#End Region

#Region " IDisposable Implementation "

    ''' <summary>
    ''' Unregisters all the hotkeys and destroys the hidden message-only window.
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose

        If Me.isDisposed Then
            Return
        End If

        For Each id As Integer In Me.registeredHotkeys.Keys
            NativeMethods.UnregisterHotKey(Me.Handle, id)
        Next
        Me.registeredHotkeys.Clear()
        Me.DestroyHandle()

        Me.isDisposed = True
    End Sub

#End Region

End Class
