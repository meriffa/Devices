using Devices.Common.Models.Monitoring;

namespace Devices.Client.Interfaces.Monitoring;

/// <summary>
/// Monitoring service interface
/// </summary>
public interface IMonitoringService
{

    #region Public Methods
    /// <summary>
    /// Return device metrics
    /// </summary>
    /// <returns></returns>
    DeviceMetrics GetDeviceMetrics();
    #endregion

}