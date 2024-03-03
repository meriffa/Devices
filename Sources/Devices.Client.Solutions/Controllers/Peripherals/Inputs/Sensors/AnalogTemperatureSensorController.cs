using CommandLine;
using Devices.Client.Solutions.Peripherals.I2C;
using UnitsNet;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Analog Temperature Sensor controller
/// </summary>
[Verb("Sensors-AnalogTemperatureSensor", HelpText = "Analog Temperature Sensor operation.")]
public class AnalogTemperatureSensorController : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Analog Temperature Sensor operation started.");
        using var sensor = new PCF8591();
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Temperature = {GetTemperature(sensor.ReadInput(0)).DegreesCelsius:F2} ℃");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Analog Temperature Sensor operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return temperature
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private static Temperature GetTemperature(int value)
    {
        var vr = 5.0d * value / 255.0d;
        var rt = 10000.0d * vr / (5.0d - vr);
        return Temperature.FromDegreesCelsius(1.0d / ((Math.Log(rt / 10000.0d) / 3950.0d) + (1.0d / (273.15d + 25.0d))) - 273.15d);
    }
    #endregion

}