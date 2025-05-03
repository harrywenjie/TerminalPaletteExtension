// File: Commands/LaunchTerminalProfileCommand.cs
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Diagnostics;
using System;
using System.Threading;

namespace TerminalPaletteExtension.Commands;

internal sealed partial class LaunchTerminalProfileCommand : InvokableCommand
{
    private readonly string _profileGuid;

    // We don't display this command directly, so Name/Icon might not be strictly needed  
    // but it's good practice to provide them.  
    public override string Name => "Launch Terminal Profile"; // Generic name  
    public override IconInfo Icon => new("\uE756"); // Example: Terminal icon  

    public LaunchTerminalProfileCommand(string profileGuid)
    {
        _profileGuid = profileGuid;
    }

    public override CommandResult Invoke()
    {
        try
        {
            // Use wt.exe command line interface to open a specific profile  
            ProcessStartInfo startInfo = new()
            {
                FileName = "wt.exe",
                Arguments = $"--profile \"{_profileGuid}\"", // Use the profile GUID  
                UseShellExecute = true, // Use ShellExecute to find wt.exe correctly            
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal
            };

            var process = Process.Start(startInfo);

            // Give the terminal a moment to initialize
            if (process != null)
            {
                // Wait a short period to allow the window to be created
                System.Threading.Thread.Sleep(300);

                // Try to ensure window is in foreground
                if (!process.HasExited)
                {
                    // Refresh process info to get latest window handle
                    process.Refresh();

                    if (process.MainWindowHandle != IntPtr.Zero)
                    {
                        // Set foreground window using Windows API
                        NativeMethods.SetForegroundWindow(process.MainWindowHandle);
                    }
                }
            }

            return CommandResult.Dismiss();
        }
        catch (Exception ex)
        {
            // Provide a longer-lasting, more detailed error message  
            return CommandResult.ShowToast($"{ex.Message}");
        }
    }

}
