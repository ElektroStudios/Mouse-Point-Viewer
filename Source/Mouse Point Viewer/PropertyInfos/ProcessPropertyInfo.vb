#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

Imports Win32

#End Region

<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class ProcessPropertyInfo

    <DisplayName("Name")>
    <[ReadOnly](True)>
    Public ReadOnly Property Name As String

    <DisplayName("ID")>
    <[ReadOnly](True)>
    Public ReadOnly Property Id As Integer

    <DisplayName("Architecture")>
    <[ReadOnly](True)>
    Public ReadOnly Property Architecture As String

    Public Sub New(pr As Process)
        Try
            Me.Name = pr.ProcessName
            Me.Id = pr.Id
            Me.Architecture = Me.GetProcessArchitecture(pr)
        Catch
        End Try
    End Sub

    Private Function GetProcessArchitecture(pr As Process) As String
        If Not Environment.Is64BitOperatingSystem Then
            Return "32-bit"
        End If

        Try
            Dim isWow64 As Boolean
            Return If(NativeMethods.IsWow64Process(pr.Handle, isWow64), If(isWow64, "32-bit", "64-bit"), "Unknown")
        Catch
            Return "Unknown"
        End Try
    End Function

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return $"{Me.Name}.exe"
    End Function


End Class
