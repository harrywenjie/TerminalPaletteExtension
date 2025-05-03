// File: Services/TerminalProfileService.cs
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TerminalPaletteExtension.Models;
using System.Text.Json.Serialization; // Needed for source generation attributes

namespace TerminalPaletteExtension.Services;

// Define the source generation context for System.Text.Json
[JsonSourceGenerationOptions(WriteIndented = true, PropertyNameCaseInsensitive = true)] // Options to mimic default behavior somewhat
[JsonSerializable(typeof(TerminalSettings))] // Specify the root type to generate serialization logic for
// Add other related types if necessary, though usually the root type is enough if it contains the others
// [JsonSerializable(typeof(ProfileSettings))]
// [JsonSerializable(typeof(TerminalProfile))]
internal partial class SourceGenerationContext : JsonSerializerContext
{
    // The compiler generates the implementation for this partial class
}

// Service class to handle loading, caching, and providing Terminal profiles
internal sealed class TerminalProfileService
{
    // Simple Singleton pattern for easy access across the extension
    public static TerminalProfileService Instance { get; } = new TerminalProfileService();

    // Private fields for caching profile data
    private List<TerminalProfile> _allProfiles = [];
    private DateTime _lastReadTime = DateTime.MinValue;
    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(5); // Cache profiles for 5 minutes to avoid constant file reads

    // Private constructor ensures Singleton pattern and loads profiles on first access
    private TerminalProfileService()
    {
        LoadProfiles(); // Load profiles when the service instance is created
    }

    // Loads profiles from the Windows Terminal settings file
    private void LoadProfiles()
    {
        // Check cache validity first
        if (DateTime.UtcNow - _lastReadTime < _cacheDuration && _allProfiles.Count > 0)
        {
            return; // Use cached data if it's still fresh
        }

        _allProfiles = []; // Clear old data before reloading
        try
        {
            // Construct the path to settings.json dynamically
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            // The package name suffix might change in future Terminal versions, but this is standard
            string settingsPath = Path.Combine(localAppData, "Packages", "Microsoft.WindowsTerminal_8wekyb3d8bbwe", "LocalState", "settings.json");

            if (File.Exists(settingsPath))
            {
                string jsonContent = File.ReadAllText(settingsPath);

                // Use the generated source generation context for efficient and trim-safe deserialization
                var settings = JsonSerializer.Deserialize(jsonContent, SourceGenerationContext.Default.TerminalSettings);

                if (settings?.Profiles?.List != null)
                {
                    // Filter out profiles marked as hidden in the settings
                    _allProfiles = settings.Profiles.List.Where(p => !p.Hidden).ToList();
                    _lastReadTime = DateTime.UtcNow; // Update the cache timestamp
                }
            }
            else
            {
                // Handle case where settings file doesn't exist
                Console.WriteLine($"Windows Terminal settings file not found at: {settingsPath}");
                _allProfiles = []; // Ensure list is empty
            }
        }
        catch (JsonException jsonEx)
        {
            // Handle JSON parsing errors
            Console.WriteLine($"Error deserializing terminal settings JSON: {jsonEx.Message}");
            _allProfiles = []; // Ensure list is empty on error
        }
        catch (IOException ioEx)
        {
            // Handle file reading errors
            Console.WriteLine($"Error reading terminal settings file: {ioEx.Message}");
            _allProfiles = []; // Ensure list is empty on error
        }
        catch (Exception ex) // Catch any other unexpected errors
        {
            // Log generic error or handle appropriately
            Console.WriteLine($"Unexpected error loading terminal profiles: {ex.Message}");
            _allProfiles = []; // Ensure list is empty on error
        }
    }

    // Public method to get profiles, enforcing cache refresh logic
    // Allows filtering for SSH profiles based on the 'sshOnly' flag
    public IEnumerable<TerminalProfile> GetProfiles(bool sshOnly)
    {
        LoadProfiles(); // This call ensures the cache is checked/refreshed before returning data

        // Filter the loaded profiles based on whether their command line contains "ssh"
        return _allProfiles.Where(p =>
        {
            // Case-insensitive check for "ssh" in the command line
            bool isSsh = p.CommandLine?.Contains("ssh", StringComparison.OrdinalIgnoreCase) ?? false;
            // Return SSH profiles if sshOnly is true, otherwise return non-SSH profiles
            return sshOnly ? isSsh : !isSsh;
        });
    }

    // Static helper method to safely convert an icon path string into an IconInfo object
    // Handles various path types (URIs, files, embedded resources) and environment variables.
    public static IconInfo? GetIconInfo(string? iconPath)
    {
        if (string.IsNullOrWhiteSpace(iconPath))
        {
            return null; // No path provided
        }

        try
        {
            // Expand environment variables like %USERPROFILE%, %SystemRoot% etc.
            string expandedPath = Environment.ExpandEnvironmentVariables(iconPath);

            // Check if it's a URI scheme that IconInfo might handle directly (e.g., ms-appx:///)
            if (Uri.TryCreate(expandedPath, UriKind.Absolute, out Uri? uri) && !uri.IsFile)
            {
                // Let IconInfo handle potential URI schemes [6]
                return new IconInfo(expandedPath);
            }
            // Check if it's a valid file path OR contains a comma (indicating an embedded resource like shell32.dll,3) [6]
            else if (File.Exists(expandedPath) || expandedPath.Contains(','))
            {
                // IconInfo can handle absolute file paths and embedded resource paths [6]
                return new IconInfo(expandedPath);
            }
            else
            {
                // The path is not a recognizable URI, doesn't exist as a file, and isn't an embedded resource path
                Console.WriteLine($"Invalid or non-existent icon path after expansion: {expandedPath}");
                return null;
            }
        }
        catch (ArgumentException argEx)
        {
            // Catch errors during path expansion or IconInfo creation
            Console.WriteLine($"Argument error creating IconInfo for path '{iconPath}' (Expanded: '{Environment.ExpandEnvironmentVariables(iconPath)}'): {argEx.Message}");
            return null; // Return null on error
        }
        catch (Exception ex) // Catch other potential exceptions
        {
            Console.WriteLine($"Generic error creating IconInfo for path '{iconPath}': {ex.Message}");
            return null; // Return null on error
        }
    }
}