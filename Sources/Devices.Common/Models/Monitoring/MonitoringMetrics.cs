using Devices.Common.Models.Identification;

namespace Devices.Common.Models.Monitoring;

/// <summary>
/// Monitoring metrics
/// </summary>
public class MonitoringMetrics
{

    #region Properties
    /// <summary>
    /// Monitoring metrics device
    /// </summary>
    public required Device Device { get; set; }

    /// <summary>
    /// Monitoring metrics device metrics
    /// </summary>
    public required DeviceMetrics DeviceMetrics { get; set; }
    #endregion

}