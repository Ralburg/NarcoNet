using Microsoft.Extensions.Logging;
using NarcoNet.Utilities;

namespace NarcoNet.Server.Services;

/// <summary>
/// Extension methods for filtering sync paths based on client type
/// </summary>
public static class ClientTypeFilterExtensions
{
    /// <summary>
    /// Filter sync paths to only those available for a specific client type
    /// </summary>
    /// <param name="syncPaths">List of sync paths to filter</param>
    /// <param name="clientType">Type of client requesting the paths</param>
    /// <param name="logger">Optional logger for debugging</param>
    /// <returns>Filtered list of sync paths appropriate for the client type</returns>
    public static List<SyncPath> FilterByClientType(
        this IEnumerable<SyncPath> syncPaths,  // CHANGED: was List<SyncPath>
        ClientType clientType,
        ILogger? logger = null)
    {
        ClientTypeFilter clientFilter = clientType.ToFilter();

        var filtered = syncPaths
            .Where(sp => (sp.ClientTypes & clientFilter) != 0)
            .ToList();

        if (logger != null)
        {
            var allPaths = syncPaths.ToList();  // Convert to list for counting
            int excluded = allPaths.Count - filtered.Count;
            if (excluded > 0)
            {
                logger.LogDebug(
                    "Filtered {ExcludedCount} sync paths not available for {ClientType} clients. " +
                    "Returning {IncludedCount} paths.",
                    excluded,
                    clientType,
                    filtered.Count);

                // Log which paths were excluded (only in debug mode)
                var excludedPaths = allPaths
                    .Where(sp => (sp.ClientTypes & clientFilter) == 0)
                    .Select(sp => $"{sp.Path} (ClientTypes={sp.ClientTypes})");

                foreach (var path in excludedPaths)
                {
                    logger.LogTrace("  Excluded: {Path}", path);
                }
            }
        }

        return filtered;
    }

    /// <summary>
    /// Get a summary of sync paths grouped by client type availability
    /// </summary>
    public static ClientTypeSyncSummary GetSyncSummary(this IEnumerable<SyncPath> syncPaths)  // CHANGED
    {
        var pathList = syncPaths.ToList();  // Convert once for multiple operations

        return new ClientTypeSyncSummary
        {
            TotalPaths = pathList.Count,
            RegularOnlyPaths = pathList.Count(sp => sp.ClientTypes == ClientTypeFilter.Regular),
            HeadlessOnlyPaths = pathList.Count(sp => sp.ClientTypes == ClientTypeFilter.Headless),
            SharedPaths = pathList.Count(sp => sp.ClientTypes == ClientTypeFilter.All),
            PathsByCategory = pathList
                .Where(sp => !string.IsNullOrEmpty(sp.Category))
                .GroupBy(sp => sp.Category)
                .ToDictionary(g => g.Key, g => g.Count())
        };
    }
}

/// <summary>
/// Summary information about sync path distribution across client types
/// </summary>
public class ClientTypeSyncSummary
{
    public int TotalPaths { get; init; }
    public int RegularOnlyPaths { get; init; }
    public int HeadlessOnlyPaths { get; init; }
    public int SharedPaths { get; init; }
    public Dictionary<string, int> PathsByCategory { get; init; } = new();

    public override string ToString()
    {
        var categories = PathsByCategory.Any()
            ? $", Categories: {string.Join(", ", PathsByCategory.Select(kvp => $"{kvp.Key}={kvp.Value}"))}"
            : "";

        return $"Total={TotalPaths}, Regular={RegularOnlyPaths}, " +
               $"Headless={HeadlessOnlyPaths}, Shared={SharedPaths}{categories}";
    }
}