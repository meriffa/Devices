using Devices.Common.Models.Identification;

namespace Devices.Common.Models.Monitoring;

/// <summary>
/// Monitoring metrics
/// </summary>
public class MonitoringMetrics
{

    #region Properties
    /// <summary>
    /// Monitoring metrics device identity
    /// </summary>
    public required Identity Identity { get; set; }

    /// <summary>
    /// Monitoring device metrics
    /// </summary>
    public required DeviceMetrics Device { get; set; }
    #endregion

}