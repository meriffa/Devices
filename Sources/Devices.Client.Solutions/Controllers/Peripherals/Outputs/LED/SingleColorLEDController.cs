using CommandLine;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// Single Color LED controller
/// </summary>
[Verb("LED-SingleColorLED", HelpText = "Single Color LED operation.")]
public class SingleColorLEDController : LEDController
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
        DisplayService.WriteInformation($"Single Color LED operation started.");
        using var controller = SetupController([PIN_NUMBER]);
        while (IsRunning())
        {
            SetOutputValue(controller, true);
            SetOutputValue(controller);
        }
        DisplayService.WriteInformation($"Single Color LED operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set LED value
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="value"></param>
    private static void SetOutputValue(GpioController controller, bool value = false) => SetOutputValue(controller, [new(PIN_NUMBER, value ? PinValue.High : PinValue.Low)]);
    #endregion

}