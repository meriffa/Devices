using Devices.Common.Models.Monitoring;
using Devices.Service.Models.Monitoring;

namespace Devices.Service.Interfaces.Monitoring;

/// <summary>
/// Monitoring service interface
/// </summary>
public interface IMonitoringService
{

    #region Public Methods
    /// <summary>
    /// Return monitoring metrics
    /// </summary>
    /// <returns></returns>
    List<MonitoringMetrics> GetMonitoringMetrics();

    /// <summary>
    /// Save device metrics
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="metrics"></param>
    void SaveDeviceMetrics(int deviceId, DeviceMetrics metrics);
    #endregion

}