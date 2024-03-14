using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Solutions.Garden.Models;

namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Garden service interface
/// </summary>
public interface IGardenService
{

    #region Public Methods
    /// <summary>
    /// Return device weather conditions
    /// </summary>
    /// <returns></returns>
    List<DeviceWeatherCondition> GetDeviceWeatherConditions();

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="weatherCondition"></param>
    void SaveWeatherCondition(int deviceId, WeatherCondition weatherCondition);
    #endregion

}