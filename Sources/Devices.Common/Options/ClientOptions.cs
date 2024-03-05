namespace Devices.Common.Options;

/// <summary>
/// Client options
/// </summary>
public class ClientOptions
{

    #region Properties
    /// <summary>
    /// Client options service
    /// </summary>
    public required ServiceClientOptions Service { get; set; }

    /// <summary>
    /// Client options configuration folder
    /// </summary>
    public required string ConfigurationFolder { get; set; }
    #endregion

}