// File: Models/TerminalProfile.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TerminalPaletteExtension.Models;

// Represents the structure within the "list" array in settings.json
internal sealed class TerminalProfile
{
    [JsonPropertyName("guid")]
    public string? Guid { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("commandline")]
    public string? CommandLine { get; set; }

    [JsonPropertyName("icon")]
    public string? Icon { get; set; }

    [JsonPropertyName("hidden")]
    public bool Hidden { get; set; }

    // Add other properties if needed, like startingDirectory
    [JsonPropertyName("startingDirectory")]
    public string? StartingDirectory { get; set; }
}

// Represents the top-level structure containing the profile list
internal sealed class TerminalSettings
{
    [JsonPropertyName("profiles")]
    public ProfileSettings? Profiles { get; set; }
}

internal sealed class ProfileSettings
{
    [JsonPropertyName("list")]
    public List<TerminalProfile>? List { get; set; }
    // We don't need "defaults" for now
}