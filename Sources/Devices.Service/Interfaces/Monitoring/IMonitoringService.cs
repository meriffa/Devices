using Devices.Common.Models.Monitoring;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Models.Monitoring;
using Devices.Service.Models.Security;
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
    /// <param name="user"></param>
    /// <returns></returns>
    List<MonitoringMetrics> GetMonitoringMetrics(User user);

    /// <summary>
    /// Save device metrics
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="serviceDate"></param>
    /// <param name="metrics"></param>
    void SaveDeviceMetrics(int deviceId, DateTime serviceDate, DeviceMetrics metrics);

    /// <summary>
    /// Return device outages
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="deviceId"></param>
    /// <param name="filter"></param>
    /// <returns></returns>
    List<DeviceOutage> GetDeviceOutages(IIdentityService identityService, int? deviceId, OutageFilter filter, User user);

    /// <summary>
    /// Upload device logs
    /// </summary>
    /// <param name="file"></param>
    void UploadDeviceLogs(IFormFile file);
    #endregion

}