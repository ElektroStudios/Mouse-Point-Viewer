#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

''' <summary>
''' Defines the colors of a visual theme of the application.
''' </summary>
Friend NotInheritable Class ThemePaletteUInteger ' NOSONAR

#Region " Properties "

    ''' <summary>
    ''' Gets the theme that this palette represents.
    ''' </summary>
    Friend ReadOnly Property Theme As AppTheme

    ''' <summary>
    ''' Gets the background color of the windows.
    ''' </summary>
    Friend ReadOnly Property WindowBack As Color

    ''' <summary>
    ''' Gets the foreground (text) color of the windows.
    ''' </summary>
    Friend ReadOnly Property WindowFore As Color

    ''' <summary>
    ''' Gets the color of secondary texts (e.g. hints).
    ''' </summary>
    Friend ReadOnly Property SecondaryFore As Color

    ''' <summary>
    ''' Gets the color of the texts that notify a successful action (e.g. "copied to clipboard").
    ''' </summary>
    Friend ReadOnly Property SuccessFore As Color

    ''' <summary>
    ''' Gets the background color of buttons and input controls.
    ''' </summary>
    Friend ReadOnly Property ControlBack As Color

    ''' <summary>
    ''' Gets the foreground color of buttons and input controls.
    ''' </summary>
    Friend ReadOnly Property ControlFore As Color

    ''' <summary>
    ''' Gets the color of the borders.
    ''' </summary>
    Friend ReadOnly Property Border As Color

    ''' <summary>
    ''' Gets the background color of a button when the mouse is over it.
    ''' </summary>
    Friend ReadOnly Property ButtonHoverBack As Color

    ''' <summary>
    ''' Gets the background color of a button when it is pressed or checked.
    ''' </summary>
    Friend ReadOnly Property ButtonPressedBack As Color

    ''' <summary>
    ''' Gets the background color of the <see cref="PropertyGrid"/> (around the grid view).
    ''' </summary>
    Friend ReadOnly Property GridBack As Color

    ''' <summary>
    ''' Gets the background color of the grid view of the <see cref="PropertyGrid"/>.
    ''' </summary>
    Friend ReadOnly Property GridViewBack As Color

    ''' <summary>
    ''' Gets the foreground color of the grid view of the <see cref="PropertyGrid"/>.
    ''' </summary>
    Friend ReadOnly Property GridViewFore As Color

    ''' <summary>
    ''' Gets the color of the grid lines and of the left margin of the <see cref="PropertyGrid"/>.
    ''' </summary>
    Friend ReadOnly Property GridLine As Color

    ''' <summary>
    ''' Gets the background color of the selected item of the <see cref="PropertyGrid"/>.
    ''' </summary>
    Friend ReadOnly Property GridSelectedBack As Color

    ''' <summary>
    ''' Gets the foreground color of the selected item of the <see cref="PropertyGrid"/>.
    ''' </summary>
    Friend ReadOnly Property GridSelectedFore As Color

    ''' <summary>
    ''' Gets the foreground color of the read-only values of the <see cref="PropertyGrid"/>
    ''' (all the displayed values are read-only).
    ''' </summary>
    Friend ReadOnly Property GridReadOnlyFore As Color

#End Region

#Region " Shared Properties "

    ''' <summary>
    ''' Gets the palette of the light theme, which keeps the original (system colors) design of the application.
    ''' </summary>
    Friend Shared ReadOnly Property Light As New ThemePalette(
        theme:=AppTheme.Light,
        windowBack:=SystemColors.Window,
        windowFore:=SystemColors.ControlText,
        secondaryFore:=SystemColors.GrayText,
        successFore:=Color.FromArgb(16, 124, 16),
        controlBack:=SystemColors.Window,
        controlFore:=SystemColors.ControlText,
        border:=SystemColors.ControlDark,
        buttonHoverBack:=Color.FromArgb(229, 241, 251),
        buttonPressedBack:=Color.FromArgb(204, 228, 247),
        gridBack:=SystemColors.Control,
        gridViewBack:=SystemColors.Window,
        gridViewFore:=SystemColors.WindowText,
        gridLine:=SystemColors.InactiveBorder,
        gridSelectedBack:=SystemColors.Highlight,
        gridSelectedFore:=SystemColors.HighlightText,
        gridReadOnlyFore:=SystemColors.GrayText)

    ''' <summary>
    ''' Gets the palette of the dark theme.
    ''' </summary>
    Friend Shared ReadOnly Property Dark As New ThemePalette(
        theme:=AppTheme.Dark,
        windowBack:=Color.FromArgb(32, 32, 32),
        windowFore:=Color.FromArgb(241, 241, 241),
        secondaryFore:=Color.FromArgb(170, 170, 170),
        successFore:=Color.FromArgb(108, 203, 95),
        controlBack:=Color.FromArgb(45, 45, 45),
        controlFore:=Color.FromArgb(241, 241, 241),
        border:=Color.FromArgb(80, 80, 80),
        buttonHoverBack:=Color.FromArgb(62, 62, 62),
        buttonPressedBack:=Color.FromArgb(0, 84, 153),
        gridBack:=Color.FromArgb(32, 32, 32),
        gridViewBack:=Color.FromArgb(38, 38, 38),
        gridViewFore:=Color.FromArgb(241, 241, 241),
        gridLine:=Color.FromArgb(55, 55, 55),
        gridSelectedBack:=Color.FromArgb(0, 84, 153),
        gridSelectedFore:=Color.White,
        gridReadOnlyFore:=Color.FromArgb(208, 208, 208))

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ThemePalette"/> class.
    ''' </summary>
    '''
    ''' <param name="theme">The theme that this palette represents.</param>
    ''' <param name="windowBack">The background color of the windows.</param>
    ''' <param name="windowFore">The foreground color of the windows.</param>
    ''' <param name="secondaryFore">The color of secondary texts.</param>
    ''' <param name="successFore">The color of the texts that notify a successful action.</param>
    ''' <param name="controlBack">The background color of buttons and input controls.</param>
    ''' <param name="controlFore">The foreground color of buttons and input controls.</param>
    ''' <param name="border">The color of the borders.</param>
    ''' <param name="buttonHoverBack">The background color of a button when the mouse is over it.</param>
    ''' <param name="buttonPressedBack">The background color of a button when it is pressed or checked.</param>
    ''' <param name="gridBack">The background color of the <see cref="PropertyGrid"/>.</param>
    ''' <param name="gridViewBack">The background color of the grid view.</param>
    ''' <param name="gridViewFore">The foreground color of the grid view.</param>
    ''' <param name="gridLine">The color of the grid lines.</param>
    ''' <param name="gridSelectedBack">The background color of the selected item.</param>
    ''' <param name="gridSelectedFore">The foreground color of the selected item.</param>
    ''' <param name="gridReadOnlyFore">The foreground color of the read-only values.</param>
    Private Sub New(theme As AppTheme,
                    windowBack As Color, windowFore As Color, secondaryFore As Color, successFore As Color,
                    controlBack As Color, controlFore As Color, border As Color,
                    buttonHoverBack As Color, buttonPressedBack As Color,
                    gridBack As Color, gridViewBack As Color, gridViewFore As Color, gridLine As Color,
                    gridSelectedBack As Color, gridSelectedFore As Color, gridReadOnlyFore As Color)

        Me.Theme = theme
        Me.WindowBack = windowBack
        Me.WindowFore = windowFore
        Me.SecondaryFore = secondaryFore
        Me.SuccessFore = successFore
        Me.ControlBack = controlBack
        Me.ControlFore = controlFore
        Me.Border = border
        Me.ButtonHoverBack = buttonHoverBack
        Me.ButtonPressedBack = buttonPressedBack
        Me.GridBack = gridBack
        Me.GridViewBack = gridViewBack
        Me.GridViewFore = gridViewFore
        Me.GridLine = gridLine
        Me.GridSelectedBack = gridSelectedBack
        Me.GridSelectedFore = gridSelectedFore
        Me.GridReadOnlyFore = gridReadOnlyFore
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Returns the palette of the specified theme.
    ''' </summary>
    '''
    ''' <param name="theme">
    ''' The theme.
    ''' </param>
    '''
    ''' <returns>
    ''' The palette of the theme.
    ''' </returns>
    '''
    ''' <exception cref="ComponentModel.InvalidEnumArgumentException">
    ''' Thrown when <paramref name="theme"/> is not a defined <see cref="AppTheme"/> value.
    ''' </exception>
    Friend Shared Function FromTheme(theme As AppTheme) As ThemePalette

        Select Case theme
            Case AppTheme.Light : Return ThemePalette.Light
            Case AppTheme.Dark : Return ThemePalette.Dark
            Case Else : Throw New ComponentModel.InvalidEnumArgumentException(NameOf(theme), theme, GetType(AppTheme))
        End Select
    End Function

#End Region

End Class
