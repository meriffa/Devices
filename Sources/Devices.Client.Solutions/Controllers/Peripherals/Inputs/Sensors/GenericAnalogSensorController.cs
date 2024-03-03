using CommandLine;
using Devices.Client.Solutions.Peripherals.I2C;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Generic Analog Sensor controller
/// </summary>
[Verb("Sensors-GenericAnalogSensor", HelpText = "Generic Analog Sensor operation.")]
public class GenericAnalogSensorController : PeripheralsController
{

    #region Properties
    /// <summary>
    /// Sensor level type
    /// </summary>
    [Option('l', "levelType", Required = false, Default = "Sensor", HelpText = "Sensor level type.")]
    public string LevelType { get; set; } = null!;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Generic Analog Sensor operation started.");
        using var sensor = new PCF8591();
        while (IsRunning())
        {
            DisplayService.WriteInformation($"{LevelType} Level = {sensor.ReadInput(0)}");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Generic Analog Sensor operation completed.");
    }
    #endregion

}