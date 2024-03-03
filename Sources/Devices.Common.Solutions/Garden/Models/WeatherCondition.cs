namespace Devices.Common.Solutions.Garden.Models;

/// <summary>
/// Weather condition
/// </summary>
public class WeatherCondition
{

    #region Properties
    /// <summary>
    /// Weather condition date & time
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Weather condition temperature [℃]
    /// </summary>
    public required double Temperature { get; set; }

    /// <summary>
    /// Weather condition humidity [%]
    /// </summary>
    public required double Humidity { get; set; }

    /// <summary>
    /// Weather condition pressure [hPa]
    /// </summary>
    public required double Pressure { get; set; }

    /// <summary>
    /// Weather condition illuminance [Lux]
    /// </summary>
    public required double Illuminance { get; set; }
    #endregion

}