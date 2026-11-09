#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

''' <summary>
''' Specifies the modifier keys of a global hotkey registered by <see cref="GlobalHotkeyManager"/>.
''' <para></para>
''' The values match the <c>MOD_*</c> values of the Win32 <c>RegisterHotKey</c> function.
''' </summary>
<Flags>
Friend Enum HotkeyModifiers As UInteger

    ''' <summary>
    ''' No modifier key.
    ''' </summary>
    None = 0UI

    ''' <summary>
    ''' Either ALT key (<c>MOD_ALT</c>).
    ''' </summary>
    Alt = 1UI << 0

    ''' <summary>
    ''' Either CTRL key (<c>MOD_CONTROL</c>).
    ''' </summary>
    Control = 1UI << 1

    ''' <summary>
    ''' Either SHIFT key (<c>MOD_SHIFT</c>).
    ''' </summary>
    Shift = 1UI << 2

    ''' <summary>
    ''' Either WINDOWS key (<c>MOD_WIN</c>).
    ''' </summary>
    Windows = 1UI << 3

    ''' <summary>
    ''' Changes the hotkey behavior so that the keyboard auto-repeat does not yield multiple hotkey notifications
    ''' (<c>MOD_NOREPEAT</c>).
    ''' </summary>
    NoRepeat = 1UI << 14

End Enum
