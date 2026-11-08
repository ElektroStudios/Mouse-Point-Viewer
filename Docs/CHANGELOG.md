# Mouse Point Viewer Change Log 📋

## v1.4 *(current)* 🆕
#### 🌟 Improvements:
 - The property grid is now populated immediately when the application starts, instead of waiting for the first refresh.
 - When the process that owns the window under the cursor is no longer available, its name is displayed as "N/A" (instead of a bare ".exe").

#### 🛠️ Fixes:
 - **Process Architecture:** The architecture of elevated or protected processes (e.g. processes running as administrator or SYSTEM) is now correctly displayed, instead of "Unknown". The process is now opened with limited query access rights.
 - **Process Exit Crash:** Fixed an unhandled exception when the process that owns the window under the cursor exited between two refreshes.
 - **Handle Leak:** The process object retrieved on every refresh is now disposed, instead of leaking a process handle on each timer tick until garbage collection.
 - **Pixel Hit-Test Fallback:** Fixed the pixel hit-test of layered windows (used by the "Ignore transparent windows" option): when the pixel could not be copied from the window, the device context was released twice, so the screen-sampling fallback was never applied and the window was always considered visible.
 - **Copy to Clipboard (Ctrl+C):** Fixed an unhandled exception when pressing Ctrl+C before the information was populated, and when the clipboard was being used by another process (an error message is now shown).
 - **Window Real Class Name:** Fixed the buffer size passed to `RealGetWindowClass`.

## v1.3 🔄
#### 🌟 Improvements:
 - An "Ignore layered windows" checkbox has been added.
 - An "Ignore transparent windows" checkbox has been added.
 - Replaced all property fields with expandable ones having the following values:
   - Title: Text, Length
   - Class: Name, Real Name
   - Process: Name, ID, Architecture
   - Window Handle: Decimal, Hexadecimal
   - Window Bounds: Rectangle, Client Rectangle
   - Point: Screen, Client Window
   - Pixel Color: RGB, Win32, Web, Web Name
 - Improved the formatting sent to clipboard on Ctrl+C keyboard shortcut.

## v1.2 🔄
#### 🌟 Improvements:
 - The user interface has been completely redesigned from scratch.
 - A "Top Most" checkbox has been added.
 - A "Color Under Cursor" field has been added.
 - The "Window Handle" field now also displays its hexadecimal representation.
 - Added the ability to copy values to the clipboard using Ctrl+C keyboard shortcut.

## v1.1 🔄
#### 🌟 Improvements:
 - Source-code upgraded to .NET Framework 4.8

## v1.0 🔄
Initial Release.