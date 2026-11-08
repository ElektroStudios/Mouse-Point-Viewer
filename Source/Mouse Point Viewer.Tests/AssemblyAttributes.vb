#Region " Option Statements "

Option Strict On
Option Explicit On
Option Infer Off

#End Region

' The tests share global state (the window search options of the Main form, which are Shared fields)
' and create real top-level windows on the desktop, so they must never run in parallel.
<Assembly: DoNotParallelize>
