#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

#Region " Imports "

Imports System.Diagnostics
Imports System.Reflection
Imports System.Runtime.CompilerServices

#End Region

#Region " PropertyGrid Extensions "

Namespace DevCase.Extensions.PropertyGridExtensions

    ''' <summary>
    ''' Provides extension methods for the <see cref="PropertyGrid"/> type.
    ''' </summary>
    <HideModuleName>
    Friend Module PropertyGridExtensions

#Region " Public Extension Methods "

        ''' <summary>
        ''' Returns a value indicating whether the source <see cref="PropertyGrid"/> has been initialized.
        ''' <para></para>
        ''' A <see cref="PropertyGrid"/> is considered initialized when it is displayed once in a <see cref="Form"/>.
        ''' </summary>
        '''
        ''' <param name="propGrid">
        ''' The source <see cref="PropertyGrid"/>.
        ''' </param>
        ''' 
        ''' <returns>
        ''' <see langword="True"/> if the <see cref="PropertyGrid"/> has been initialized, <see langword="False"/> otherwise.
        ''' </returns>
        <DebuggerStepThrough>
        <Extension>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Friend Function IsInitialized(propGrid As PropertyGrid) As Boolean

            Return PropertyGridExtensions.GetInternalLabelWidth(propGrid) <> -1

        End Function

        ''' <summary>
        ''' Sets the position of the splitter control (moves it to left or rigth sides) of the source the <see cref="PropertyGrid"/>.
        ''' <para></para>
        ''' Note that the <see cref="PropertyGrid"/> must be initialized (displayed once in a <see cref="Form"/> 
        ''' to let the control internally calculate the columns width) in order to be able set the splitter control position.
        ''' </summary>
        '''
        ''' <param name="propGrid">
        ''' The source <see cref="PropertyGrid"/>.
        ''' </param>
        ''' 
        ''' <param name="xPos">
        ''' The desired X position of the splitter control.
        ''' </param>
        <DebuggerStepThrough>
        <Extension>
        <EditorBrowsable(EditorBrowsableState.Always)>
        Friend Sub SetSplitterPosition(propGrid As PropertyGrid, xPos As Integer)

            If Not PropertyGridExtensions.IsInitialized(propGrid) Then
                Throw New InvalidOperationException("The PropertyGrid must be initialized in order to set the splitter control position.")
            End If

            Dim internalGridView As Control = PropertyGridExtensions.GetPropertyGridInternal(propGrid)
            Dim moveSplitterToMethod As MethodInfo = internalGridView.GetType().GetMethod("MoveSplitterTo", BindingFlags.NonPublic Or BindingFlags.Instance)
            moveSplitterToMethod.Invoke(internalGridView, {xPos})

        End Sub

#End Region

#Region " Private Methods "

        ''' <summary>
        ''' Uses Reflection to get the internal PropertyGridView instance of the source <see cref="PropertyGrid"/>.
        ''' </summary>
        ''' 
        ''' <param name="propGrid">
        ''' The source <see cref="PropertyGrid"/>.
        ''' </param>
        ''' 
        ''' <returns>
        ''' The internal PropertyGridView instance of the source <see cref="PropertyGrid"/>
        ''' </returns>
        Private Function GetPropertyGridInternal(propGrid As PropertyGrid) As Control

            ' Class: System.Windows.Forms.PropertyGridInternal.PropertyGridView

            Dim getPropertyGridViewMethod As MethodInfo = GetType(PropertyGrid).GetMethod("GetPropertyGridView", BindingFlags.NonPublic Or BindingFlags.Instance)
            Dim gridView As Control = DirectCast(getPropertyGridViewMethod.Invoke(propGrid, Array.Empty(Of Object)()), Control)
            Return gridView

            ' We can also retrieve the internal PropertyGridView this other way:
            ' ------------------------------------------------------------------
            ' Dim gridViewFieldInfo As FieldInfo = propGrid.GetType().GetField("gridView", BindingFlags.Instance Or BindingFlags.NonPublic)
            ' Dim gridView As Control = DirectCast(gridViewFieldInfo.GetValue(propGrid), Control)
            ' Return gridView

        End Function

        ''' <summary>
        ''' Uses Reflection to get the value of the "InternalLabelWidth" property of the source <see cref="PropertyGrid"/>.
        ''' </summary>
        ''' 
        ''' <param name="propGrid">
        ''' The source <see cref="PropertyGrid"/>.
        ''' </param>
        ''' 
        ''' <returns>
        ''' The value of the "InternalLabelWidth" property of the source <see cref="PropertyGrid"/>
        ''' </returns>
        Private Function GetInternalLabelWidth(propGrid As PropertyGrid) As Integer

            Dim internalGridView As Control = PropertyGridExtensions.GetPropertyGridInternal(propGrid)
            Dim propInfo As PropertyInfo = internalGridView.GetType().GetProperty("InternalLabelWidth", BindingFlags.NonPublic Or BindingFlags.Instance)
            Return CInt(propInfo.GetValue(internalGridView))

        End Function

#End Region

    End Module

End Namespace

#End Region
