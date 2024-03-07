using CommandLine;
using Iot.Device.DHTxx;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Temperature & Humidity Sensor (DHT11) controller
/// </summary>
[Verb("Sensors-TemperatureAndHumiditySensor", HelpText = "Temperature & Humidity Sensor operation.")]
public class TemperatureAndHumiditySensorController : PeripheralsController
{

    #region Constants
    private const int PIN_NUMBER = 17;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Temperature & Humidity Sensor operation started.");
        using var sensor = new Dht11(PIN_NUMBER);
        while (IsRunning())
        {
            if (sensor.TryReadTemperature(out var temperature) && sensor.TryReadHumidity(out var humidity))
                DisplayService.WriteInformation($"Temperature = {temperature.DegreesCelsius} ℃, Humidity = {humidity.Percent} %");
            else
                DisplayService.WriteInformation($"Temperature = N/A, Humidity = N/A");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation("Temperature & Humidity Sensor operation completed.");
    }
    #endregion

}