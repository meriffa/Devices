using Devices.Common.Solutions.Garden.Models;

namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Garden service interface
/// </summary>
public interface IGardenService
{

    #region Public Methods
    /// <summary>
    /// Return weather conditions
    /// </summary>
    /// <returns></returns>
    List<WeatherCondition> GetWeatherConditions();

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="weatherCondition"></param>
    void SaveWeatherCondition(WeatherCondition weatherCondition);
    #endregion

}