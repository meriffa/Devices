using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Options;
using Devices.Common.Services;
using Devices.Common.Solutions.Garden.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Devices.Client.Solutions.Garden.Services;

/// <summary>
/// Garden service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class GardenService(ILogger<GardenService> logger, IOptions<ClientOptions> options) : ClientService(options.Value), IGardenService
{

    #region Private Fields
    private readonly ILogger<GardenService> logger = logger;
    #endregion

    #region Public Methods
    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="weatherCondition"></param>
    public void SaveWeatherCondition(WeatherCondition weatherCondition)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(weatherCondition), Encoding.UTF8, "application/json");
            HttpResponseMessage response = Client.PostAsync($"/Service/Solutions/Garden/SaveWeatherCondition", content).Result;
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

}