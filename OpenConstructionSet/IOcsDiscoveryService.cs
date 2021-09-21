﻿using OpenConstructionSet.IO;
using System.Diagnostics.CodeAnalysis;

namespace OpenConstructionSet;

public interface IOcsDiscoveryService
{
    ModFolder? Discover(DirectoryInfo folder);
    ModFile? Discover(FileInfo file);
    IEnumerable<ModFile> DiscoverFiles(IEnumerable<string> files);
    IEnumerable<ModFolder> Discoverfolders(IEnumerable<string> folders);
    bool TryDiscoverFile(string path, [MaybeNullWhen(false)] out ModFile modFile);
    bool TryDiscoverFolder(DirectoryInfo folder, [MaybeNullWhen(false)] out ModFolder modFolder);
    bool TryDiscoverFolder(string folder, [MaybeNullWhen(false)] out ModFolder modFolder);
    Dictionary<string, GameFolders> TryFindAllGameFolders();
    bool TryFindGameFolders([MaybeNullWhen(false)] out GameFolders folders);
    bool TryFindGameFolders(string locatorId, [MaybeNullWhen(false)] out GameFolders folders);
}
