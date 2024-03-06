using Devices.Common.Models.Metrics;

namespace Devices.Client.Interfaces.Metrics;

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