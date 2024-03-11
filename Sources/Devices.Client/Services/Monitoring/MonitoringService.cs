using Devices.Client.Interfaces.Monitoring;
using Devices.Common.Interfaces.Identification;
using Devices.Common.Models.Monitoring;
using Devices.Common.Options;
using Devices.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Devices.Client.Services.Monitoring;

/// <summary>
/// Monitoring service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// <param name="identityService"></param>
/// <param name="deviceMetricsService"></param>
public class MonitoringService(ILogger<MonitoringService> logger, IOptions<ClientOptions> options, IIdentityService identityService, IDeviceMetricsService deviceMetricsService) : DeviceClientService(options.Value, identityService), IMonitoringService
{

    #region Private Fields
    private readonly ILogger<MonitoringService> logger = logger;
    private readonly IDeviceMetricsService deviceMetricsService = deviceMetricsService;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return device metrics
    /// </summary>
    /// <returns></returns>
    public DeviceMetrics GetDeviceMetrics()
    {
        try
        {
            var metrics = deviceMetricsService.GetMetrics();
            var content = new StringContent(JsonSerializer.Serialize(metrics), Encoding.UTF8, "application/json");
            using var response = Client.PostAsync("/Service/Monitoring/SaveDeviceMetrics", content).Result;
            response.EnsureSuccessStatusCode();
            return metrics;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

}