#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Drawing.Design
Imports System.Text

#End Region

<Editor(GetType(PixelColorEditor), GetType(UITypeEditor))>
<RefreshProperties(RefreshProperties.All)>
<DebuggerDisplay("{ToString()}")>
Friend NotInheritable Class PixelColorPropertyInfo

    <Browsable(False)>
    <[ReadOnly](True)>
    Friend ReadOnly Property Color As Color

    <DisplayName("RGB")>
    <[ReadOnly](True)>
    Public ReadOnly Property RGB As String
        Get
            Return $"{Me.Color.R}, {Me.Color.G}, {Me.Color.B}"
        End Get
    End Property

    <DisplayName("Win32")>
    <[ReadOnly](True)>
    Public ReadOnly Property Win32 As Integer
        Get
            Return ColorTranslator.ToWin32(Me.Color)
        End Get
    End Property

    <DisplayName("Web")>
    <[ReadOnly](True)>
    Public ReadOnly Property Web As String
        Get
            Return ColorTranslator.ToHtml(Me.Color)
        End Get
    End Property

    <DisplayName("Web Name")>
    <[ReadOnly](True)>
    Public ReadOnly Property WebName As String
        Get
            Dim name As String = Nothing
            For Each kc As KnownColor In [Enum].GetValues(GetType(KnownColor))
                Dim kColor As Color = Color.FromKnownColor(kc)
                If Not kColor.IsSystemColor AndAlso
                    kColor.R = Me.Color.R AndAlso
                    kColor.G = Me.Color.G AndAlso
                    kColor.B = Me.Color.B AndAlso
                    kColor.A = 255 Then ' 255 for "White" instead of "Transparent" (0)

                    name = kColor.Name
                    Exit For
                End If
            Next
            Return name
        End Get
    End Property

    Public Sub New(color As Color)
        Me.Color = color
    End Sub

    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Dim webColorConverter As New WebColorConverter()
        Dim colorString As String = DirectCast(webColorConverter.ConvertTo(Me.Color, GetType(String)), String)
        Return colorString
    End Function

End Class
