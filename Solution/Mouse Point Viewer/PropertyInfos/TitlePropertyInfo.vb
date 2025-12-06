#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

#End Region

<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class TitlePropertyInfo

    <DisplayName("Text")>
    <[ReadOnly](True)>
    Public Property Text As String

    <DisplayName("Length")>
    <[ReadOnly](True)>
    Public ReadOnly Property Length As Integer
        Get
            Return If(Me.Text?.Length, 0)
        End Get
    End Property

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return Me.Text
    End Function

End Class
