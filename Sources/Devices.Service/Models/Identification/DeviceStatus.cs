namespace Devices.Service.Models.Identification;

/// <summary>
/// Device status
/// </summary>
public class DeviceStatus
{

    #region Properties
    /// <summary>
    /// Device status device
    /// </summary>
    public required Device Device { get; set; }

    /// <summary>
    /// Device status token
    /// </summary>
    public required string Token { get; set; }

    /// <summary>
    /// Device status flag
    /// </summary>
    public required bool Enabled { get; set; }

    /// <summary>
    /// Device status level
    /// </summary>
    public required DeviceLevel Level { get; set; }

    /// <summary>
    /// Device status last monitoring device date & time
    /// </summary>
    public required DateTime? DeviceDate { get; set; }

    /// <summary>
    /// Device status uptime
    /// </summary>
    public required TimeSpan? Uptime { get; set; }

    /// <summary>
    /// Device status deployments
    /// </summary>
    public required List<DeviceDeployment> Deployments { get; set; }
    #endregion

}