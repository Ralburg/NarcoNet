namespace NarcoNet.Utilities;

public record SyncPath(
    string Path,
    string Name = "",
    bool Enabled = true,
    bool Enforced = false,
    bool Silent = false,
    bool RestartRequired = true,
    ClientTypeFilter ClientTypes = ClientTypeFilter.All,  // NEW
    string Category = "")  // NEW
{
    /// <summary>
    /// Display name for this sync path (defaults to Path if not specified)
    /// </summary>
    public string Name { get; init; } = string.IsNullOrEmpty(Name) ? Path : Name;

    /// <summary>
    /// Optional category for grouping related paths (e.g., "graphics", "ui", "core", "audio")
    /// </summary>
    public string Category { get; init; } = Category;

    /// <summary>
    /// Which client types can receive this sync path
    /// </summary>
    public ClientTypeFilter ClientTypes { get; init; } = ClientTypes;

    /// <summary>
    /// Check if this sync path should be sent to a specific client type
    /// </summary>
    public bool IsAvailableFor(ClientType clientType)
    {
        return ClientTypes.AllowsClientType(clientType);
    }
}