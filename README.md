# Terminal Palette Extension

## Overview
Terminal Palette Extension adds rich Windows Terminal profile shortcuts to the Windows Command Palette. It discovers all of your profiles, shows their custom icons, and splits the list into SSH and non-SSH entries, making it far easier to jump into the right environment than the default “Open Windows Terminal profile” command.

## Features
- Displays friendly profile names and icons for every Windows Terminal profile.
- Breaks profiles into “SSH Connections” and “Local Profiles” sections for quick filtering.
- Remembers the previous window focus and brings the launched Terminal window forward automatically.
- Respects your Windows Terminal configuration—no extra setup required.

## Installation
1. Download the latest `AppPackages` zip from the [GitHub Releases](../../releases) page.
2. Extract the archive. You will find a `.cer` file and a `.msixbundle`.
3. Install the signing certificate (one-time step):
   - Double-click the `.cer` file → “Install Certificate…”.
   - Choose `Local Machine` → `Trusted People` store.
   - Finish the wizard; you should see a success message.
   - PowerShell alternative:
     ```powershell
     Import-Certificate -FilePath .\TerminalPaletteExtension.cer -CertStoreLocation Cert:\LocalMachine\TrustedPeople
     ```
4. Install the extension package:
   - Double-click the `.msixbundle`, or run:
     ```powershell
     Add-AppxPackage .\TerminalPaletteExtension_x64.msixbundle
     ```
5. Launch Windows Command Palette to verify the new “Terminal Palette” provider appears.

## Usage
1. Open the Windows Command Palette (use the shortcut configured on your build of Windows; in current preview builds it defaults to `Win` + `Shift` + `C`).
2. Type `Terminal Palette` and select the provider.
3. Choose between `SSH Connections` or `Local Profiles`, then pick the profile you want.
4. The extension starts the profile via `wt.exe` and focuses the new Terminal window automatically.

## Updating
Repeat the installation steps with the newer release. Windows will prompt to update the existing package—no need to reinstall the certificate.

## Building From Source
1. Install the .NET 9 SDK and the Windows App SDK workload in Visual Studio 2022 (17.10+).
2. Clone the repository and open `TerminalPaletteExtension.sln`.
3. Restore and build in `Debug` or `Release`.
4. Use **Build → Publish → Create App Packages** to produce a signed MSIX bundle for distribution.

## Troubleshooting
- **Package won’t install**: ensure the certificate is in the `Trusted People` store (Local Machine scope) and that you extracted the entire AppPackages folder, including dependencies.
- **Terminal window doesn’t focus**: make sure the Windows Terminal process is allowed to come to the foreground; interacting with another app during launch can still block focus due to OS rules.
- **Profiles missing**: confirm they exist in `%LOCALAPPDATA%\Packages\Microsoft.WindowsTerminal_8wekyb3d8bbwe\LocalState\settings.json` and are not marked `"hidden": true`.
