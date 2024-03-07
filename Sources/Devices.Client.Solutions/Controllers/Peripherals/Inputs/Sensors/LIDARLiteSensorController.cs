using CommandLine;
using Iot.Device.DistanceSensor;
using System.Device.Gpio;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// LIDAR Lite Sensor controller
/// </summary>
[Verb("Sensors-LIDARLiteSensor", HelpText = "LIDAR Lite Sensor operation.")]
public class LIDARLiteSensorController : PeripheralsController
{

    #region Constants
    private const int POWER_PIN_NUMBER = 17;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("LIDAR Lite Sensor operation started.");
        using var sensor = GetSensor();
        sensor.PowerOn();
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Distance = {sensor.MeasureDistance().Meters:F2} m");
            Thread.Sleep(STEP_DURATION);
        }
        sensor.PowerOff();
        DisplayService.WriteInformation("LIDAR Lite Sensor operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return LIDAR sensor
    /// </summary>
    /// <returns></returns>
    private static LidarLiteV3 GetSensor() => new(I2cDevice.Create(new I2cConnectionSettings(1, LidarLiteV3.DefaultI2cAddress)), new GpioController(), POWER_PIN_NUMBER);
    #endregion

}