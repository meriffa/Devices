namespace Devices.Service.Options;

/// <summary>
/// Service options
/// </summary>
public class ServiceOptions
{

    #region Properties
    /// <summary>
    /// Service options database
    /// </summary>
    public required DatabaseOptions Database { get; set; }

    /// <summary>
    /// Service options JWT bearer
    /// </summary>
    public required JwtBearerOptions JwtBearer { get; set; }

    /// <summary>
    /// Service options package folder
    /// </summary>
    public required string PackageFolder { get; set; }

    /// <summary>
    /// Service options data protection folder
    /// </summary>
    public required string DataProtectionFolder { get; set; }

    /// <summary>
    /// Service options device logs folder
    /// </summary>
    public required string DeviceLogsFolder { get; set; }
    #endregion

}