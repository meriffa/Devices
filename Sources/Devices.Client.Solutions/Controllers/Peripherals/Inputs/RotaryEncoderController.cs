using CommandLine;
using Iot.Device.RotaryEncoder;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Rotary Encoder controller
/// </summary>
[Verb("Inputs-RotaryEncoder", HelpText = "Rotary Encoder operation.")]
public class RotaryEncoderController : PeripheralsController
{

    #region Constants
    private const int A_PIN_NUMBER = 17; // Clock signal
    private const int B_PIN_NUMBER = 18; // Data signal
    private const int BUTTON_PIN_NUMBER = 27;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Rotary Encoder operation started.");
        using var controller = SetupController([BUTTON_PIN_NUMBER], PinMode.InputPullUp);
        using var input = new ScaledQuadratureEncoder(A_PIN_NUMBER, B_PIN_NUMBER, PinEventTypes.Falling, 20, 0.1d, 0.0d, 100.0d)
        {
            Value = 50.0d,
            Debounce = TimeSpan.FromMilliseconds(2)
        };
        input.ValueChanged += (_, e) => DisplayService.WriteInformation($"Value = {e.Value:0.0}");
        DisplayService.WriteInformation($"Initial Button State = {GetButtonState(controller)}");
        controller.RegisterCallbackForPinValueChangedEvent(BUTTON_PIN_NUMBER, PinEventTypes.Rising | PinEventTypes.Falling, (_, e) => DisplayService.WriteInformation($"New Button State = {GetButtonState(e.ChangeType)}"));
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        DisplayService.WriteInformation("Rotary Encoder operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return button state
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private static string GetButtonState(GpioController controller) => controller.Read(BUTTON_PIN_NUMBER) == PinValue.Low ? "Pressed" : "Released";

    /// <summary>
    /// Return button state
    /// </summary>
    /// <param name="changeType"></param>
    /// <returns></returns>
    private static string GetButtonState(PinEventTypes changeType) => changeType == PinEventTypes.Falling ? "Pressed" : "Released";
    #endregion

}