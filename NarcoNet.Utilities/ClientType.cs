using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NarcoNet.Utilities;

/// <summary>
/// Represents the type of client connecting to the server
/// </summary>
public enum ClientType
{
    /// <summary>
    /// Regular player client with full UI and graphics
    /// </summary>
    Regular = 0,

    /// <summary>
    /// Headless client (AI/bot host) without graphics or UI
    /// </summary>
    Headless = 1
}

/// <summary>
/// Flags enum for filtering which client types can receive a sync path
/// </summary>
[Flags]
public enum ClientTypeFilter
{
    /// <summary>
    /// No clients (disabled)
    /// </summary>
    None = 0,

    /// <summary>
    /// Only regular clients with UI/graphics
    /// </summary>
    Regular = 1 << 0,  // 1

    /// <summary>
    /// Only headless clients (AI hosts)
    /// </summary>
    Headless = 1 << 1,  // 2

    /// <summary>
    /// All client types (default)
    /// </summary>
    All = Regular | Headless  // 3
}

/// <summary>
/// Extension methods for ClientType operations
/// </summary>
public static class ClientTypeExtensions
{
    /// <summary>
    /// Convert ClientType to ClientTypeFilter
    /// </summary>
    public static ClientTypeFilter ToFilter(this ClientType clientType)
    {
        return clientType switch
        {
            ClientType.Regular => ClientTypeFilter.Regular,
            ClientType.Headless => ClientTypeFilter.Headless,
            _ => ClientTypeFilter.All
        };
    }

    /// <summary>
    /// Check if a filter allows a specific client type
    /// </summary>
    public static bool AllowsClientType(this ClientTypeFilter filter, ClientType clientType)
    {
        ClientTypeFilter clientFilter = clientType.ToFilter();
        return (filter & clientFilter) != 0;
    }

    /// <summary>
    /// Parse client type from string (case-insensitive)
    /// </summary>
    public static ClientType ParseClientType(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return ClientType.Regular;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "headless" => ClientType.Headless,
            "regular" => ClientType.Regular,
            _ => ClientType.Regular
        };
    }
}
