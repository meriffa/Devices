using CommandLine;
using Devices.Common.Solutions.Garden.Models;
using Iot.Device.Bmxx80;
using Iot.Device.Max44009;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Garden;

/// <summary>
/// Weather controller
/// </summary>
[Verb("Weather", HelpText = "Weather task.")]
public class WeatherController : Controller
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Weather task started.");
        var weatherCondition = GetWeatherCondition();
        GardenService.SaveWeatherCondition(weatherCondition);
        DisplayService.WriteInformation($"Temperature = {weatherCondition.Temperature:F2} ℃");
        DisplayService.WriteInformation($"Humidity = {weatherCondition.Humidity:F2} %");
        DisplayService.WriteInformation($"Pressure = {weatherCondition.Pressure:F2} hPa");
        DisplayService.WriteInformation($"Pressure = {weatherCondition.Illuminance:F2} Lux");
        DisplayService.WriteInformation("Weather task completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return current weather condition
    /// </summary>
    /// <returns></returns>
    private static WeatherCondition GetWeatherCondition()
    {
        using var temperatureDevice = I2cDevice.Create(new(busId: 1, Bmx280Base.SecondaryI2cAddress));
        using var temperatureSensor = new Bme280(temperatureDevice)
        {
            TemperatureSampling = Sampling.UltraHighResolution,
            HumiditySampling = Sampling.UltraHighResolution,
            PressureSampling = Sampling.UltraHighResolution
        };
        using var illuminanceDevice = I2cDevice.Create(new I2cConnectionSettings(busId: 1, Max44009.DefaultI2cAddress));
        using var illuminanceSensor = new Max44009(illuminanceDevice, IntegrationTime.Time100);
        var temperatureSensorData = temperatureSensor.Read();
        return new()
        {
            DeviceDate = DateTime.UtcNow,
            Temperature = temperatureSensorData.Temperature!.Value.DegreesCelsius,
            Humidity = temperatureSensorData.Humidity!.Value.Percent,
            Pressure = temperatureSensorData.Pressure!.Value.Hectopascals,
            Illuminance = illuminanceSensor.Illuminance
        };
    }
    #endregion

}