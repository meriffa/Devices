using CommandLine;
using Iot.Device.Bmp180;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Barometric Sensor (BMP180) controller
/// </summary>
[Verb("Sensors-BarometricSensor", HelpText = "Barometric Sensor operation.")]
public class BarometricSensorController : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Barometric Sensor operation started.");
        using var sensor = GetSensor();
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Temperature = {sensor.ReadTemperature().DegreesCelsius:F2} ℃, Pressure = {sensor.ReadPressure().Hectopascals:0.##} hPa, Altitude = {sensor.ReadAltitude().Meters:0.##} m");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Barometric Sensor operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return barometric sensor
    /// </summary>
    /// <returns></returns>
    private static Bmp180 GetSensor()
    {
        var sensor = new Bmp180(I2cDevice.Create(new(1, Bmp180.DefaultI2cAddress)));
        sensor.SetSampling(Sampling.Standard);
        return sensor;
    }
    #endregion

}