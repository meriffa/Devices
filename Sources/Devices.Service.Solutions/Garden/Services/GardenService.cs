using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Solutions.Garden.Interfaces;
using Microsoft.Extensions.Logging;

namespace Devices.Service.Solutions.Garden.Services;

/// <summary>
/// Garden service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class GardenService(ILogger<GardenService> logger, IGardenServiceData dataService) : IGardenService
{

    #region Private Fields
    private readonly ILogger<GardenService> logger = logger;
    private readonly IGardenServiceData dataService = dataService;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return weather conditions
    /// </summary>
    /// <returns></returns>
    public List<WeatherCondition> GetWeatherConditions()
    {
        try
        {
            return dataService.GetWeatherConditions();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="weatherCondition"></param>
    public void SaveWeatherCondition(WeatherCondition weatherCondition)
    {
        try
        {
            dataService.SaveWeatherCondition(weatherCondition);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

}