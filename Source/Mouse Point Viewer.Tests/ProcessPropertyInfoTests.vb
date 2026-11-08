#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics

#End Region

#Region " ProcessPropertyInfoTests "

Namespace MousePointViewer.Tests

    ''' <summary>
    ''' Unit tests for the <see cref="ProcessPropertyInfo"/> class.
    ''' </summary>
    <TestClass>
    Public NotInheritable Class ProcessPropertyInfoTests

#Region " Constants "

        ''' <summary>
        ''' A process identifier that never belongs to a running process
        ''' (Windows process identifiers are multiples of 4).
        ''' </summary>
        Private Const NonExistentPid As Integer = Integer.MaxValue

#End Region

#Region " Constructor "

        ''' <summary>
        ''' Verifies that the constructor stores the values as-is.
        ''' </summary>
        <TestMethod>
        Public Sub Constructor_StoresValues()

            Dim info As New ProcessPropertyInfo("notepad", 1234, ProcessPropertyInfo.Architecture64Bit)

            Assert.AreEqual("notepad", info.Name)
            Assert.AreEqual(1234, info.Id)
            Assert.AreEqual("64-bit", info.Architecture)
        End Sub

        ''' <summary>
        ''' Verifies that a null architecture throws <see cref="ArgumentNullException"/>.
        ''' </summary>
        <TestMethod>
        Public Sub Constructor_NullArchitecture_ThrowsArgumentNullException()

            Assert.ThrowsExactly(Of ArgumentNullException)(Function() New ProcessPropertyInfo("notepad", 1234, Nothing))
        End Sub

#End Region

#Region " ToString "

        ''' <summary>
        ''' Verifies that the string representation is the executable name.
        ''' </summary>
        <TestMethod>
        Public Sub ToString_WithName_ReturnsExecutableName()

            Dim info As New ProcessPropertyInfo("explorer", 4321, ProcessPropertyInfo.Architecture64Bit)

            Assert.AreEqual("explorer.exe", info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies that the string representation of a process without name is <see cref="ProcessPropertyInfo.UnavailableName"/>,
        ''' instead of a bare ".exe".
        ''' </summary>
        <TestMethod>
        Public Sub ToString_WithoutName_ReturnsUnavailable()

            Dim info As New ProcessPropertyInfo(Nothing, 4321, ProcessPropertyInfo.ArchitectureUnknown)

            Assert.AreEqual(ProcessPropertyInfo.UnavailableName, info.ToString())
        End Sub

#End Region

#Region " FromProcessId "

        ''' <summary>
        ''' Verifies the information retrieved for the current process.
        ''' </summary>
        <TestMethod>
        Public Sub FromProcessId_CurrentProcess_ReturnsNameIdAndArchitecture()

            Using current As Process = Process.GetCurrentProcess()
                Dim info As ProcessPropertyInfo = ProcessPropertyInfo.FromProcessId(current.Id)

                Assert.AreEqual(current.ProcessName, info.Name)
                Assert.AreEqual(current.Id, info.Id)
                Assert.AreEqual(ProcessPropertyInfoTests.GetExpectedArchitectureOfCurrentProcess(), info.Architecture)
                Assert.AreEqual($"{current.ProcessName}.exe", info.ToString())
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that a process that is not running (e.g. it exited between two refreshes)
        ''' does not throw, and returns an unavailable name and an unknown architecture.
        ''' </summary>
        <TestMethod>
        Public Sub FromProcessId_NonExistentProcess_ReturnsUnavailableInfo()

            Dim info As ProcessPropertyInfo = ProcessPropertyInfo.FromProcessId(NonExistentPid)

            Assert.IsNull(info.Name)
            Assert.AreEqual(NonExistentPid, info.Id)
            Assert.AreEqual(ProcessPropertyInfo.ArchitectureUnknown, info.Architecture)
            Assert.AreEqual(ProcessPropertyInfo.UnavailableName, info.ToString())
        End Sub

        ''' <summary>
        ''' Verifies that the information of a process that has already exited is reported as unavailable.
        ''' </summary>
        <TestMethod>
        Public Sub FromProcessId_ExitedProcess_ReturnsUnavailableInfo()

            Dim pid As Integer
            Dim psi As New ProcessStartInfo("cmd.exe", "/c exit") With {
                .CreateNoWindow = True,
                .UseShellExecute = False
            }
            Using pr As Process = Process.Start(psi)
                pid = pr.Id
                Assert.IsTrue(pr.WaitForExit(10000), "The child process did not exit in time.")
            End Using

            ' Once the last handle to the process object is closed, the PID is no longer valid.
            Dim info As ProcessPropertyInfo = ProcessPropertyInfo.FromProcessId(pid)

            ' The PID could theoretically be reused by a new process; in that case there is nothing to assert.
            If info.Name Is Nothing Then
                Assert.AreEqual(ProcessPropertyInfo.ArchitectureUnknown, info.Architecture)
                Assert.AreEqual(ProcessPropertyInfo.UnavailableName, info.ToString())
            Else
                Assert.AreNotEqual("cmd", info.Name, "The exited process is still reported as running.")
            End If
        End Sub

#End Region

#Region " GetProcessArchitecture "

        ''' <summary>
        ''' Verifies the architecture of the current process.
        ''' </summary>
        <TestMethod>
        Public Sub GetProcessArchitecture_CurrentProcess_MatchesEnvironment()

            Using current As Process = Process.GetCurrentProcess()
                Assert.AreEqual(ProcessPropertyInfoTests.GetExpectedArchitectureOfCurrentProcess(),
                                ProcessPropertyInfo.GetProcessArchitecture(current.Id))
            End Using
        End Sub

        ''' <summary>
        ''' Verifies that the architecture of a process that cannot be opened is
        ''' <see cref="ProcessPropertyInfo.ArchitectureUnknown"/> on 64-bit operating systems.
        ''' </summary>
        <TestMethod>
        Public Sub GetProcessArchitecture_NonExistentProcess_ReturnsUnknown()

            If Not Environment.Is64BitOperatingSystem Then
                Assert.Inconclusive("On 32-bit operating systems every process is 32-bit.")
            End If

            Assert.AreEqual(ProcessPropertyInfo.ArchitectureUnknown, ProcessPropertyInfo.GetProcessArchitecture(NonExistentPid))
        End Sub

        ''' <summary>
        ''' Verifies that the architecture is resolved for every running process whose handle can be opened with
        ''' full access rights (as v1.3 did through <see cref="Process.Handle"/>), so the current implementation is never worse.
        ''' <para></para>
        ''' The current implementation opens the process with limited query rights,
        ''' so it also resolves elevated processes when running without administrator rights.
        ''' </summary>
        <TestMethod>
        Public Sub GetProcessArchitecture_AllProcesses_ResolvesAtLeastWhatFullAccessResolves()

            If Not Environment.Is64BitOperatingSystem Then
                Assert.Inconclusive("On 32-bit operating systems every process is 32-bit.")
            End If

            Dim processes As Process() = Process.GetProcesses()
            Try
                Dim checkedCount As Integer = 0
                For Each pr As Process In processes
                    Dim fullAccessOk As Boolean
                    Try
                        fullAccessOk = pr.Handle <> IntPtr.Zero AndAlso Not pr.HasExited
                    Catch ex As Exception When TypeOf ex Is ComponentModel.Win32Exception OrElse TypeOf ex Is InvalidOperationException
                        fullAccessOk = False
                    End Try

                    If fullAccessOk Then
                        Dim architecture As String = ProcessPropertyInfo.GetProcessArchitecture(pr.Id)
                        Assert.AreNotEqual(ProcessPropertyInfo.ArchitectureUnknown, architecture,
                                           $"Architecture not resolved for '{pr.ProcessName}' (PID {pr.Id}).")
                        checkedCount += 1
                    End If
                Next

                Assert.IsTrue(checkedCount > 0, "No process could be checked.")
            Finally
                For Each pr As Process In processes
                    pr.Dispose()
                Next
            End Try
        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Returns the expected architecture string of the current process.
        ''' </summary>
        '''
        ''' <returns>
        ''' The expected architecture string.
        ''' </returns>
        Private Shared Function GetExpectedArchitectureOfCurrentProcess() As String

            ' A 64-bit process is never WOW64; a 32-bit process is WOW64 on 64-bit operating systems.
            Return If(Environment.Is64BitProcess, ProcessPropertyInfo.Architecture64Bit, ProcessPropertyInfo.Architecture32Bit)
        End Function

#End Region

    End Class

End Namespace

#End Region
