// File: Pages/OtherProfilesPage.cs
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Collections.Generic;
using TerminalPaletteExtension.Commands;
using TerminalPaletteExtension.Models;
using TerminalPaletteExtension.Services; // We'll create this next

namespace TerminalPaletteExtension.Pages;

internal sealed partial class OtherProfilesPage : ListPage
{
    public OtherProfilesPage()
    {
        Icon = new("\uE756"); // Example: Terminal icon
        Title = "Other Profiles";
        Name = "Open";
    }

    public override IListItem[] GetItems()
    {
        var profiles = TerminalProfileService.Instance.GetProfiles(sshOnly: false); // Get non-SSH profiles
        var listItems = new List<ListItem>();

        foreach (var profile in profiles)
        {
            if (profile.Guid != null) // Ensure profile has a GUID
            {
                var command = new LaunchTerminalProfileCommand(profile.Guid);
                listItems.Add(new ListItem(command)
                {
                    Title = profile.Name ?? "Unnamed Profile",
                    // Fix: Call GetIconInfo directly on the class name
                    Icon = TerminalProfileService.GetIconInfo(profile.Icon) ?? IconHelpers.FromRelativePath("Assets\\StoreLogo.png"),
                });
            }
        }
        return listItems.ToArray();
    }
}