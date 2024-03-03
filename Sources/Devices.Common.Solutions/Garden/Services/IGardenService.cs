using Devices.Common.Models;
using Devices.Common.Solutions.Garden.Models;

namespace Devices.Common.Solutions.Garden.Services;

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
    /// <returns></returns>
    ServiceResult SaveWeatherCondition(WeatherCondition weatherCondition);
    #endregion

}