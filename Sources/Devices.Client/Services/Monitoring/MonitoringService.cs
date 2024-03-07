using Devices.Client.Interfaces.Identification;
using Devices.Client.Interfaces.Monitoring;
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
public class MonitoringService(ILogger<MonitoringService> logger, IOptions<ClientOptions> options, IIdentityService identityService, IDeviceMetricsService deviceMetricsService) : ClientService(options.Value), IMonitoringService
{

    #region Private Fields
    private readonly ILogger<MonitoringService> logger = logger;
    private readonly IIdentityService identityService = identityService;
    private readonly IDeviceMetricsService deviceMetricsService = deviceMetricsService;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return monitoring metrics
    /// </summary>
    /// <returns></returns>
    public MonitoringMetrics GetMonitoringMetrics()
    {
        try
        {
            var metrics = new MonitoringMetrics()
            {
                Device = identityService.GetDevice(),
                DeviceMetrics = deviceMetricsService.GetMetrics()
            };
            var content = new StringContent(JsonSerializer.Serialize(metrics), Encoding.UTF8, "application/json");
            using var response = Client.PostAsync("/Service/Monitoring/SaveMonitoringMetrics", content).Result;
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