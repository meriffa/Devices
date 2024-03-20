using Devices.Service.Models.Identification;

namespace Devices.Service.Solutions.Garden.Models;

/// <summary>
/// Aggregate weather condition
/// </summary>
public class AggregateWeatherCondition
{

    #region Properties
    /// <summary>
    /// Aggregate weather condition device
    /// </summary>
    public required Device Device { get; set; }

    /// <summary>
    /// Weather condition device date & time
    /// </summary>
    public required DateTime DeviceDate { get; set; }

    /// <summary>
    /// Weather condition temperature [℃]
    /// </summary>
    public required AggregateMeasurement Temperature { get; set; }

    /// <summary>
    /// Weather condition humidity [%]
    /// </summary>
    public required AggregateMeasurement Humidity { get; set; }

    /// <summary>
    /// Weather condition pressure [hPa]
    /// </summary>
    public required AggregateMeasurement Pressure { get; set; }

    /// <summary>
    /// Weather condition illuminance [Lux]
    /// </summary>
    public required AggregateMeasurement Illuminance { get; set; }
    #endregion

}