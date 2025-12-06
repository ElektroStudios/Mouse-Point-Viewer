#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Text

#End Region

<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class WindowHandlePropertyInfo

    Private ReadOnly hWnd As IntPtr

    <DisplayName("Decimal")>
    Public ReadOnly Property [Decimal] As Long
        Get
            Return Me.hWnd.ToInt64()
        End Get
    End Property

    <DisplayName("Hexadecimal")>
    Public ReadOnly Property Hexadecimal As String
        Get
            Return $"0x{Me.hWnd.ToInt64():X}"
        End Get
    End Property

    Public Sub New(hWnd As IntPtr)
        Me.hWnd = hWnd
    End Sub

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return $"{Me.[Decimal]} ({Me.Hexadecimal})"
    End Function

End Class
