# Mouse Point Viewer Change Log 📋

## v1.4 *(current)* 🆕
#### 🚀 New Features:
 - **Global Hotkeys:** The values can now be copied to the clipboard with the **Ctrl+Alt+C** global hotkey, which works regardless of which window is focused (previously, Ctrl+C only worked while this window was focused; it is kept as a local shortcut). The hotkeys are registered through the Windows `RegisterHotKey` API: no keyboard hook is installed, so there is no impact on the system input performance. A notification confirms the copied values, and a warning is shown if a hotkey is already in use by another application.
 - **Magnifier:** Added a magnifier (button with a magnifying glass icon, or **Ctrl+Alt+M** global hotkey) that follows the mouse cursor and shows a zoomed view of the screen around it, highlighting the pixel under the cursor and showing its color. The zoom (2x to 10x, with a pixel grid from 6x) is changed with **Ctrl+Alt+Plus / Ctrl+Alt+Minus** (main keyboard or numeric keypad), and the magnifier is closed with **Esc**, Ctrl+Alt+M or its button. These zoom and close hotkeys are only registered while the magnifier is visible. The magnifier never takes the focus, never covers the inspected pixel, and does not appear in the taskbar nor in Alt+Tab.
 - **Dark Theme:** Added a dark theme, which can be toggled with a button (sun/moon icon). The title bar follows the selected theme, and the property grid scroll bars are dark too. On first run, the theme follows the Windows applications theme. The selected theme is remembered between sessions.
 - **Compact / Expanded Mode:** Added a button to switch between the compact mode (the original small window) and an expanded mode (a taller window with the top-level properties expanded). The selected mode is remembered between sessions.

#### 🌟 Improvements:
 - The GitHub link has been replaced with a GitHub icon button that adapts to the selected theme.
 - The property grid is now populated immediately when the application starts, instead of waiting for the first refresh.
 - When the process that owns the window under the cursor is no longer available, its name is displayed as "N/A" (instead of a bare ".exe").

#### 🛠️ Fixes:
 - **Process Architecture:** The architecture of elevated or protected processes (e.g. processes running as administrator or SYSTEM) is now correctly displayed, instead of "Unknown". The process is now opened with limited query access rights.
 - **Process Exit Crash:** Fixed an unhandled exception when the process that owns the window under the cursor exited between two refreshes.
 - **Handle Leak:** The process object retrieved on every refresh is now disposed, instead of leaking a process handle on each timer tick until garbage collection.
 - **Pixel Hit-Test Fallback:** Fixed the pixel hit-test of layered windows (used by the "Ignore transparent windows" option): when the pixel could not be copied from the window, the device context was released twice, so the screen-sampling fallback was never applied and the window was always considered visible.
 - **Copy to Clipboard:** Fixed an unhandled exception when pressing Ctrl+C before the information was populated, and when the clipboard was being used by another process (an error message is now shown).
 - **Window Real Class Name:** Fixed the buffer size passed to `RealGetWindowClass`.
 - **Refresh Timer:** The refresh timer was enabled in the window designer, so it started as soon as the window was created, before it was shown. It now starts when the window is shown.

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