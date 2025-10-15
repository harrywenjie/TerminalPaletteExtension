// File: Commands/LaunchTerminalProfileCommand.cs
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Diagnostics;
using System.Linq;

namespace TerminalPaletteExtension.Commands;

internal sealed partial class LaunchTerminalProfileCommand : InvokableCommand
{
    private readonly string _profileGuid;

    public override string Name => "Launch Terminal Profile";
    public override IconInfo Icon => new("\uE756");

    public LaunchTerminalProfileCommand(string profileGuid)
    {
        _profileGuid = profileGuid;
    }

    public override CommandResult Invoke()
    {
        try
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = "wt.exe",
                Arguments = $"--profile \"{_profileGuid}\"",
                UseShellExecute = true,
                CreateNoWindow = false,
                WindowStyle = ProcessWindowStyle.Normal,
            };

            NativeMethods.AllowSetForegroundWindow(NativeMethods.ASFW_ANY);
            Process? process = Process.Start(startInfo);

            if (process != null)
            {
                NativeMethods.AllowSetForegroundWindow(process.Id);
                WaitForInputIdleSafe(process);

                IntPtr terminalHandle = ResolveTerminalWindowHandle(process);
                if (terminalHandle != IntPtr.Zero)
                {
                    NativeMethods.ShowWindow(terminalHandle, NativeMethods.SW_RESTORE);
                    NativeMethods.SetForegroundWindow(terminalHandle);
                }
            }

            return CommandResult.Dismiss();
        }
        catch (Exception ex)
        {
            return CommandResult.ShowToast($"{ex.Message}");
        }
    }

    private static void WaitForInputIdleSafe(Process process)
    {
        try
        {
            if (!process.HasExited)
            {
                process.WaitForInputIdle(3000);
            }
        }
        catch (InvalidOperationException)
        {
        }
        catch (System.ComponentModel.Win32Exception)
        {
        }
        catch (NotSupportedException)
        {
        }
    }

    private static IntPtr ResolveTerminalWindowHandle(Process? launchedProcess)
    {
        if (launchedProcess is { HasExited: false })
        {
            launchedProcess.Refresh();
            if (launchedProcess.MainWindowHandle != IntPtr.Zero)
            {
                return launchedProcess.MainWindowHandle;
            }
        }

        Process[] terminalProcesses = Process.GetProcessesByName("WindowsTerminal");
        try
        {
            Process? candidate = terminalProcesses
                .Where(p => !p.HasExited)
                .OrderByDescending(p => p.StartTime)
                .FirstOrDefault();

            if (candidate != null)
            {
                candidate.Refresh();
                return candidate.MainWindowHandle;
            }
        }
        finally
        {
            foreach (Process proc in terminalProcesses)
            {
                proc.Dispose();
            }
        }

        return IntPtr.Zero;
    }
}
