using CommandLine;
using Iot.Device.Hcsr04;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Ultrasonic Distance Sensor (HC-SR04) controller
/// </summary>
[Verb("Sensors-UltrasonicDistanceSensor", HelpText = "Ultrasonic Distance Sensor operation.")]
public class UltrasonicDistanceSensorController : PeripheralsController
{

    #region Constants
    private const int TRIGGER_PIN_NUMBER = 17;
    private const int ECHO_PIN_NUMBER = 18;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Ultrasonic Distance Sensor operation started.");
        using var sensor = new Hcsr04(TRIGGER_PIN_NUMBER, ECHO_PIN_NUMBER);
        while (IsRunning())
        {
            if (sensor.TryGetDistance(out var distance))
                DisplayService.WriteInformation($"Distance = {distance.Centimeters:0.00} cm");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Ultrasonic Distance Sensor operation completed.");
    }
    #endregion

}