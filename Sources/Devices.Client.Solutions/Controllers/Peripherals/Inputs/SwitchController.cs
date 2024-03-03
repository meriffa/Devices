using CommandLine;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Switch controller
/// </summary>
[Verb("Inputs-Switch", HelpText = "Switch operation.")]
public class SwitchController : PeripheralsController
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
        DisplayService.WriteInformation($"Switch operation started.");
        using var controller = SetupController([PIN_NUMBER], PinMode.Input);
        DisplayService.WriteInformation($"Initial State = {GetSwitchState(controller)}");
        controller.RegisterCallbackForPinValueChangedEvent(PIN_NUMBER, PinEventTypes.Rising | PinEventTypes.Falling, (_, e) => DisplayService.WriteInformation($"New State = {GetSwitchState(e.ChangeType)}"));
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        DisplayService.WriteInformation($"Switch operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return switch state
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private static string GetSwitchState(GpioController controller) => controller.Read(PIN_NUMBER) == PinValue.Low ? "On" : "Off";

    /// <summary>
    /// Return switch state
    /// </summary>
    /// <param name="changeType"></param>
    /// <returns></returns>
    private static string GetSwitchState(PinEventTypes changeType) => changeType == PinEventTypes.Falling ? "On" : "Off";
    #endregion

}