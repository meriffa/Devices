using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Models.Identification;

namespace Devices.Service.Solutions.Garden.Models;

/// <summary>
/// Device weather condition
/// </summary>
public class DeviceWeatherCondition : WeatherCondition
{

    #region Properties
    /// <summary>
    /// Weather condition device
    /// </summary>
    public required Device Device { get; set; }
    #endregion

}