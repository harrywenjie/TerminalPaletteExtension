// File: Commands/NativeMethods.cs
using System;
using System.Runtime.InteropServices;

namespace TerminalPaletteExtension.Commands;

internal static class NativeMethods
{
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool SetForegroundWindow(IntPtr hWnd);
}
