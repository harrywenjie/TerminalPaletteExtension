// File: Pages/TerminalPaletteExtensionPage.cs
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using TerminalPaletteExtension.Pages; // Add this using directive

namespace TerminalPaletteExtension;

internal sealed partial class TerminalPaletteExtensionPage : ListPage
{
    public TerminalPaletteExtensionPage()
    {
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
        Title = "Terminal Palette";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        // Replace the TODO item with links to your submenus [2]
        return [
            new ListItem(new SshProfilesPage())
            {
                Title = "SSH Profiles",
                // Optionally add a subtitle or specific icon
                Subtitle = "Launch SSH terminal sessions",
                // Fix: Explicitly use IconInfo constructor
                Icon = new IconInfo("\uE8C8") // Example: Cloud icon
            },
            new ListItem(new OtherProfilesPage())
            {
                Title = "Other Profiles",
                Subtitle = "Launch other terminal profiles",
                // Fix: Explicitly use IconInfo constructor
                Icon = new IconInfo("\uE756") // Example: Terminal icon
            }
        ];
    }
}