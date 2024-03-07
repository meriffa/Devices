using CommandLine;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs.Sensors;

/// <summary>
/// Laser Tripwire controller
/// </summary>
[Verb("Sensors-LaserTripwire", HelpText = "Laser Tripwire operation.")]
public class LaserTripwireController : PeripheralsController
{

    #region Constants
    private const int LASER_PIN_NUMBER = 17;
    private const int SENSOR_PIN_NUMBER = 21;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Laser Tripwire operation started.");
        using var controller = SetupController(SetupController([SENSOR_PIN_NUMBER], PinMode.InputPullUp), [LASER_PIN_NUMBER], PinMode.Output);
        controller.Write(LASER_PIN_NUMBER, PinValue.High);
        DisplayService.WriteInformation($"Initial State = {GetSensorState(controller)}");
        controller.RegisterCallbackForPinValueChangedEvent(SENSOR_PIN_NUMBER, PinEventTypes.Rising | PinEventTypes.Falling, (_, e) => DisplayService.WriteInformation($"New State = {GetSensorState(e.ChangeType)}"));
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        controller.Write(LASER_PIN_NUMBER, PinValue.Low);
        DisplayService.WriteInformation("Laser Tripwire operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return sensor state
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private static string GetSensorState(GpioController controller) => controller.Read(SENSOR_PIN_NUMBER) == PinValue.Low ? "Connected" : "Disconnected";

    /// <summary>
    /// Return sensor state
    /// </summary>
    /// <param name="changeType"></param>
    /// <returns></returns>
    private static string GetSensorState(PinEventTypes changeType) => changeType == PinEventTypes.Falling ? "Connected" : "Disconnected";
    #endregion

}