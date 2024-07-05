using Devices.Common.Models.Monitoring;

namespace Devices.Common.Interfaces.Monitoring;

/// <summary>
/// Device metrics service interface
/// </summary>
public interface IDeviceMetricsService
{

    #region Public Methods
    /// <summary>
    /// Return device metrics
    /// </summary>
    /// <returns></returns>
    DeviceMetrics GetMetrics();
    #endregion

}