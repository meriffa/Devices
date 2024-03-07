using CommandLine;
using Devices.Client.Solutions.Peripherals.OneWire;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Digital Temperature Sensor (DS18B20) controller
/// </summary>
[Verb("Sensors-DigitalTemperatureSensor", HelpText = "Digital Temperature Sensor operation.")]
public class DigitalTemperatureSensorController : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Digital Temperature Sensor operation started.");
        var sensor = new DS18B20();
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Temperature = {sensor.ReadTemperature().DegreesCelsius:F2} ℃");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation("Digital Temperature Sensor operation completed.");
    }
    #endregion

}