using CommandLine;
using Iot.Device.Imu;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Gyroscope Accelerometer Sensor (MPU6050) controller
/// </summary>
[Verb("Sensors-GyroscopeAccelerometerSensor", HelpText = "Gyroscope Accelerometer Sensor operation.")]
public class GyroscopeAccelerometerSensorController : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Gyroscope Accelerometer Sensor operation started.");
        using var sensor = new Mpu6050(I2cDevice.Create(new(1, Mpu6050.DefaultI2cAddress)));
        while (IsRunning())
        {
            var gyroscope = sensor.GetGyroscopeReading();
            var accelerometer = sensor.GetAccelerometer();
            DisplayService.WriteInformation($"Gyroscope [X = {gyroscope.X:0.000000}, Y = {gyroscope.Y:0.000000}, Z = {gyroscope.Z:0.000000}], Accelerometer [X = {accelerometer.X:0.000000} G, Y = {accelerometer.Y:0.000000} G, Z = {accelerometer.Z:0.000000} G], Temperature = {sensor.GetTemperature().DegreesCelsius:F2} ℃");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Gyroscope Accelerometer Sensor operation completed.");
    }
    #endregion

}