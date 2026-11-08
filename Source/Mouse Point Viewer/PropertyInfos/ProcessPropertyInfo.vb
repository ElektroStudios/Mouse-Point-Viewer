#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

Imports Microsoft.Win32.SafeHandles

Imports Win32

#End Region

''' <summary>
''' Represents the process information shown in the <see cref="PropertyGrid"/>.
''' </summary>
<TypeConverter(GetType(ExpandableObjectConverter))>
Friend NotInheritable Class ProcessPropertyInfo

#Region " Constants "

    ''' <summary>
    ''' The architecture string for 32-bit processes.
    ''' </summary>
    Friend Const Architecture32Bit As String = "32-bit"

    ''' <summary>
    ''' The architecture string for 64-bit processes.
    ''' </summary>
    Friend Const Architecture64Bit As String = "64-bit"

    ''' <summary>
    ''' The architecture string used when the architecture cannot be determined.
    ''' </summary>
    Friend Const ArchitectureUnknown As String = "Unknown"

    ''' <summary>
    ''' The text displayed instead of the executable name when the process name is not available
    ''' (e.g. the process exited between two refreshes).
    ''' </summary>
    Friend Const UnavailableName As String = "N/A"

#End Region

#Region " Properties "

    ''' <summary>
    ''' Gets the process name (without the ".exe" extension),
    ''' or <see langword="Nothing"/> if the process name is not available.
    ''' </summary>
    <DisplayName("Name")>
    <[ReadOnly](True)>
    Public ReadOnly Property Name As String

    ''' <summary>
    ''' Gets the process identifier.
    ''' </summary>
    <DisplayName("ID")>
    <[ReadOnly](True)>
    Public ReadOnly Property Id As Integer

    ''' <summary>
    ''' Gets the process architecture
    ''' (<see cref="Architecture32Bit"/>, <see cref="Architecture64Bit"/> or <see cref="ArchitectureUnknown"/>).
    ''' </summary>
    <DisplayName("Architecture")>
    <[ReadOnly](True)>
    Public ReadOnly Property Architecture As String

#End Region

#Region " Constructors "

    ''' <summary>
    ''' Initializes a new instance of the <see cref="ProcessPropertyInfo"/> class.
    ''' </summary>
    '''
    ''' <param name="name">
    ''' The process name (without the ".exe" extension), or <see langword="Nothing"/> if it is not available.
    ''' </param>
    '''
    ''' <param name="id">
    ''' The process identifier.
    ''' </param>
    '''
    ''' <param name="architecture">
    ''' The process architecture.
    ''' </param>
    '''
    ''' <exception cref="ArgumentNullException">
    ''' Thrown when <paramref name="architecture"/> is <see langword="Nothing"/>.
    ''' </exception>
    Friend Sub New(name As String, id As Integer, architecture As String)

        If architecture Is Nothing Then
            Throw New ArgumentNullException(NameOf(architecture))
        End If

        Me.Name = name
        Me.Id = id
        Me.Architecture = architecture
    End Sub

#End Region

#Region " Public Methods "

    ''' <summary>
    ''' Creates a new <see cref="ProcessPropertyInfo"/> for the process with the specified identifier.
    ''' <para></para>
    ''' The underlying <see cref="Process"/> object is disposed before returning.
    ''' <para></para>
    ''' If the process is no longer running (it may exit between two refreshes),
    ''' the returned instance has a <see langword="Nothing"/> <see cref="ProcessPropertyInfo.Name"/>
    ''' and an <see cref="ArchitectureUnknown"/> architecture.
    ''' </summary>
    '''
    ''' <param name="pid">
    ''' The process identifier.
    ''' </param>
    '''
    ''' <returns>
    ''' The resulting <see cref="ProcessPropertyInfo"/>.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function FromProcessId(pid As Integer) As ProcessPropertyInfo

        Dim name As String
        Try
            Using pr As Process = Process.GetProcessById(pid)
                name = pr.ProcessName
            End Using

        Catch ex As ArgumentException
            ' The process is not running (it exited before GetProcessById).
            Return New ProcessPropertyInfo(Nothing, pid, ArchitectureUnknown)

        Catch ex As InvalidOperationException
            ' The process exited before reading its name.
            Return New ProcessPropertyInfo(Nothing, pid, ArchitectureUnknown)

        End Try

        Return New ProcessPropertyInfo(name, pid, ProcessPropertyInfo.GetProcessArchitecture(pid))
    End Function

    ''' <summary>
    ''' Determines the architecture of the process with the specified identifier.
    ''' <para></para>
    ''' The process is opened with <see cref="Constants.PROCESS_QUERY_LIMITED_INFORMATION"/> access,
    ''' which also succeeds for elevated and protected processes.
    ''' </summary>
    '''
    ''' <param name="pid">
    ''' The process identifier.
    ''' </param>
    '''
    ''' <returns>
    ''' <see cref="Architecture32Bit"/>, <see cref="Architecture64Bit"/>,
    ''' or <see cref="ArchitectureUnknown"/> if the process cannot be opened or queried.
    ''' </returns>
    <DebuggerStepThrough>
    Friend Shared Function GetProcessArchitecture(pid As Integer) As String

        If Not Environment.Is64BitOperatingSystem Then
            Return Architecture32Bit
        End If

        Using hProcess As SafeProcessHandle = NativeMethods.OpenProcess(Constants.PROCESS_QUERY_LIMITED_INFORMATION, False, pid)
            If hProcess.IsInvalid Then
                Return ArchitectureUnknown
            End If

            Dim isWow64 As Boolean
            If Not NativeMethods.IsWow64Process(hProcess, isWow64) Then
                Return ArchitectureUnknown
            End If

            Return If(isWow64, Architecture32Bit, Architecture64Bit)
        End Using
    End Function

    ''' <summary>
    ''' Returns the executable name of the process (e.g. "explorer.exe"),
    ''' or <see cref="UnavailableName"/> if the process name is not available.
    ''' </summary>
    '''
    ''' <returns>
    ''' The executable name of the process.
    ''' </returns>
    <DebuggerStepThrough>
    Public Overrides Function ToString() As String

        Return If(Me.Name Is Nothing, UnavailableName, $"{Me.Name}.exe")
    End Function

#End Region

End Class
