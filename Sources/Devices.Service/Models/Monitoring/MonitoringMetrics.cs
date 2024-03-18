using Devices.Common.Models.Monitoring;
using Devices.Service.Models.Identification;

namespace Devices.Service.Models.Monitoring;

/// <summary>
/// Monitoring metrics
/// </summary>
public class MonitoringMetrics
{

    #region Properties
    /// <summary>
    /// Monitoring metrics service date & time
    /// </summary>
    public required DateTime ServiceDate { get; set; }

    /// <summary>
    /// Monitoring metrics device date & time offset
    /// </summary>
    public required TimeSpan DeviceDateOffset { get; set; }

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