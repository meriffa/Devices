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
    #endregion

}