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
Friend NotInheritable Class ClassNamePropertyInfo

    <DisplayName("Name")>
    <[ReadOnly](True)>
    Public Property Name As String

    <DisplayName("Real Name")>
    <[ReadOnly](True)>
    Public Property RealName As String

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return Me.Name
    End Function

End Class
