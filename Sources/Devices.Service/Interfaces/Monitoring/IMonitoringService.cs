using Devices.Common.Models.Monitoring;

namespace Devices.Service.Interfaces.Monitoring;

/// <summary>
/// Identity service interface
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
    /// Save monitoring metrics
    /// </summary>
    /// <param name="metrics"></param>
    void SaveMonitoringMetrics(MonitoringMetrics metrics);
    #endregion

}