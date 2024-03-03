namespace Devices.Client.Solutions.Options;

/// <summary>
/// Service options
/// </summary>
public class ServiceOptions
{

    #region Properties
    /// <summary>
    /// Service options host
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Service options request timeout (in seconds)
    /// </summary>
    public int Timeout { get; set; } = 30;
    #endregion

}