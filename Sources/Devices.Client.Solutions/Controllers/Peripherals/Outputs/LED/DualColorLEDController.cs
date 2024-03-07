using CommandLine;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// Dual Color LED controller
/// </summary>
[Verb("LED-DualColorLED", HelpText = "Dual Color LED operation.")]
public class DualColorLEDController : LEDController
{

    #region Constants
    private const int RED_PIN_NUMBER = 17;
    private const int GREEN_PIN_NUMBER = 18;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Dual Color LED operation started.");
        using var controller = SetupController([RED_PIN_NUMBER, GREEN_PIN_NUMBER]);
        using var redPWMChannel = SetupChannel(RED_PIN_NUMBER);
        using var greenPWMChannel = SetupChannel(GREEN_PIN_NUMBER);
        while (IsRunning())
        {
            SetOutputValue(controller, true, false);
            SetOutputValue(controller);
            SetOutputValue(controller, true, true);
            SetOutputValue(controller);
            SetOutputValue(controller, false, true);
            SetOutputValue(controller);
            SetOutputValue(redPWMChannel);
            SetOutputValue(controller);
            SetOutputValue(greenPWMChannel);
            SetOutputValue(controller);
        }
        DisplayService.WriteInformation("Dual Color LED operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set LED value
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="red"></param>
    /// <param name="green"></param>
    private static void SetOutputValue(GpioController controller, bool red = false, bool green = false) => SetOutputValue(controller, [new(RED_PIN_NUMBER, red ? PinValue.High : PinValue.Low), new(GREEN_PIN_NUMBER, green ? PinValue.High : PinValue.Low)]);
    #endregion

}