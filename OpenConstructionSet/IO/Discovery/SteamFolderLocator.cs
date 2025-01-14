﻿using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace OpenConstructionSet.IO.Discovery;

/// <summary>
/// Gog implementation of a <see cref="IInstallationLocator"/>
/// </summary>
public class SteamFolderLocator : IInstallationLocator
{
    private static readonly Lazy<SteamFolderLocator> _default = new(() => new());

    /// <summary>
    /// Lazy initiated singleton for when DI is not being used
    /// </summary>
    public static SteamFolderLocator Default => _default.Value;

    /// <inheritdoc/>
    public string Id { get; } = "Steam";

    /// <inheritdoc/>
    public bool TryFind([MaybeNullWhen(false)] out LocatedFolders folders)
    {
        var folder = SteamFolder();

        if (string.IsNullOrWhiteSpace(folder))
        {
            folders = null;
            return false;
        }

        var gameFolder = "";
        string? contentFolder = null;

        foreach (var library in Libraries())
        {
            var path = Path.Combine(library, "steamapps", "common", "Kenshi");

            if (Directory.Exists(path))
            {
                gameFolder = path;
            }

            path = Path.Combine(library, "steamapps", "workshop", "content", "233860");

            if (Directory.Exists(path))
            {
                contentFolder = path;
            }
        }

        if (string.IsNullOrWhiteSpace(gameFolder) || !Directory.Exists(gameFolder))
        {
            folders = null;
            return false;
        }

        folders = new LocatedFolders(gameFolder, contentFolder);
        return true;

        string SteamFolder()
        {
            try
            {
                var registryKey = Environment.Is64BitProcess ? @"SOFTWARE\Wow6432Node\Valve\Steam" : @"SOFTWARE\Valve\Steam";

                using var key = Registry.LocalMachine.OpenSubKey(registryKey);

                return key?.GetValue("InstallPath")?.ToString() ?? "";
            }
            catch
            {
                return "";
            }
        }

        IEnumerable<string> Libraries()
        {
            var path = Path.Combine(folder, "steamapps", "libraryfolders.vdf");

            // [whitespace] "[number]" [whitespace] "[library path]"
            const string pattern = "^\\s+\"\\d+\"\\s+\"(.+)\"";

            var libraries = new List<string> { folder };

            IEnumerable<string> lines;

            try
            {
                lines = File.ReadLines(path);
            }
            catch
            {
                return Enumerable.Empty<string>();
            }

            foreach (var line in lines)
            {
                var match = Regex.Match(line, pattern);

                if (match.Success)
                {
                    var library = match.Groups[1].Value;

                    if (!Directory.Exists(library))
                    {
                        continue;
                    }

                    libraries.Add(library);
                }
            }

            return libraries;
        }
    }
}
