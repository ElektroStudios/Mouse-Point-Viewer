#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

#End Region

<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class PointPropertyContainer

    <DisplayName("Screen")>
    <[ReadOnly](True)>
    Public Property ScreenPointInfo As PointPropertyInfo

    <DisplayName("Client Window")>
    <[ReadOnly](True)>
    Public Property ClientPointInfo As PointPropertyInfo

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return $"{Me.ScreenPointInfo} | {Me.ClientPointInfo}"
    End Function

End Class
