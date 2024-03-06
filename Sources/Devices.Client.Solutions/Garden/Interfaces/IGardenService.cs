using Devices.Common.Solutions.Garden.Models;

namespace Devices.Client.Solutions.Garden.Interfaces;

/// <summary>
/// Garden service interface
/// </summary>
public interface IGardenService
{

    #region Public Methods
    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="weatherCondition"></param>
    void SaveWeatherCondition(WeatherCondition weatherCondition);
    #endregion

}