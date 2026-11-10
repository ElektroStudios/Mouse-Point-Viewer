#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

''' <summary>
''' Specifies the visual theme of the application.
''' </summary>
Friend Enum AppTheme As Integer ' NOSONAR

    ''' <summary>
    ''' The light theme (the original design of the application).
    ''' </summary>
    Light = 0

    ''' <summary>
    ''' The dark theme.
    ''' </summary>
    Dark = 1

End Enum
