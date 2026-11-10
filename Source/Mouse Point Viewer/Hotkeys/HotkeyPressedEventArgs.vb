#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

''' <summary>
''' Provides data for the <see cref="GlobalHotkeyManager.HotkeyPressed"/> event.
''' </summary>
Friend NotInheritable Class HotkeyPressedEventArgs : Inherits EventArgs ' NOSONAR

#Region " Properties "

    ''' <summary>
    ''' Gets the identifier of the pressed hotkey, as returned by <see cref="GlobalHotkeyManager.Register"/>.
    ''' </summary>
    Friend ReadOnly Property Id As Integer

    ''' <summary>
    ''' Gets the modifier keys of the pressed hotkey (without <see cref="HotkeyModifiers.NoRepeat"/>).
    ''' </summary>
    Friend ReadOnly Property Modifiers As HotkeyModifiers

    ''' <summary>
    ''' Gets the key of the pressed hotkey.
    ''' </summary>
    Friend ReadOnly Property Key As Keys

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="HotkeyPressedEventArgs"/> class.
    ''' </summary>
    '''
    ''' <param name="id">
    ''' The identifier of the pressed hotkey.
    ''' </param>
    '''
    ''' <param name="modifiers">
    ''' The modifier keys of the pressed hotkey.
    ''' </param>
    '''
    ''' <param name="key">
    ''' The key of the pressed hotkey.
    ''' </param>
    Friend Sub New(id As Integer, modifiers As HotkeyModifiers, key As Keys)

        Me.Id = id
        Me.Modifiers = modifiers
        Me.Key = key
    End Sub

#End Region

End Class
