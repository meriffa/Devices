using Devices.Common.Models.Metrics;

namespace Devices.Client.Interfaces.Metrics;

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
    MonitoringMetrics GetMonitoringMetrics();
    #endregion

}