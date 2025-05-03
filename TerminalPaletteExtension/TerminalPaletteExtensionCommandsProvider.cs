// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// File: TerminalPaletteExtensionCommandsProvider.cs
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TerminalPaletteExtension.Pages; // Make sure this is included

namespace TerminalPaletteExtension;

internal sealed partial class TerminalPaletteExtensionCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;

    public TerminalPaletteExtensionCommandsProvider()
    {
        // This is the DisplayName shown FOR the provider
        DisplayName = "Terminal Palette"; // Or your desired name

        // This Icon is associated with the PROVIDER itself.
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");        

        // Create the command item(s) to be shown in the main list.
        _commands =
        [
            new CommandItem(new TerminalPaletteExtensionPage()) // Command that opens your main page
            {
                // This Title is shown IN THE LIST for this entry
                Title = DisplayName, // Use the provider's display name for the list item title

                // *** FIX: Explicitly set the Icon for the LIST ITEM ***
                Icon = new IconInfo("\uE756") // Use the same icon or a different one
            },
            // You could add other top-level commands here if needed [4]
            // Example: new CommandItem(new SomeOtherDirectCommand()) { Title = "Do Something Else Directly", Icon = ... }
        ];
    }

    // This method returns the items to show in the main Command Palette list
    public override ICommandItem[] TopLevelCommands()
    {
        return _commands;
    }  
}
