using Devices.Service.Models.Identification;

namespace Devices.Service.Models.Monitoring;

/// <summary>
/// Device outage
/// </summary>
public class DeviceOutage
{

    #region Properties
    /// <summary>
    /// Device outage device
    /// </summary>
    public required Device Device { get; set; }

    /// <summary>
    /// Device outage instance
    /// </summary>
    public required Outage Outage { get; set; }
    #endregion

}