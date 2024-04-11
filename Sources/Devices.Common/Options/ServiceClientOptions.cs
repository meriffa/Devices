namespace Devices.Common.Options;

/// <summary>
/// Service client options
/// </summary>
public class ServiceClientOptions
{

    #region Properties
    /// <summary>
    /// Service client options host
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Service client options request timeout [seconds]
    /// </summary>
    public int Timeout { get; set; } = 30;

    /// <summary>
    /// Service client options pre-request delay duration [seconds]
    /// </summary>
    public int DelayDuration { get; set; } = 1;

    /// <summary>
    /// Service client options retry count
    /// </summary>
    public int RetryCount { get; set; } = 3;
    #endregion

}