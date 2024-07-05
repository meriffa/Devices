using Devices.Common.Models.Monitoring;

namespace Devices.Common.Interfaces.Monitoring;

/// <summary>
/// Monitoring service interface
/// </summary>
public interface IMonitoringService
{

    #region Public Methods
    /// <summary>
    /// Save device metrics
    /// </summary>
    /// <returns></returns>
    DeviceMetrics SaveDeviceMetrics();
    #endregion

}