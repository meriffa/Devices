using Devices.Common.Models.Monitoring;
using Devices.Service.Models.Monitoring;
using Microsoft.AspNetCore.Http;

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
    /// <param name="serviceDate"></param>
    /// <param name="metrics"></param>
    void SaveDeviceMetrics(int deviceId, DateTime serviceDate, DeviceMetrics metrics);

    /// <summary>
    /// Upload device logs
    /// </summary>
    /// <param name="file"></param>
    void UploadDeviceLogs(IFormFile file);
    #endregion

}