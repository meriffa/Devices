using CommandLine;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// RGB LED controller
/// </summary>
[Verb("LED-RGBLED", HelpText = "RGB LED operation.")]
public class RGBLEDController : LEDController
{

    #region Constants
    private const int RED_PIN_NUMBER = 17;
    private const int GREEN_PIN_NUMBER = 18;
    private const int BLUE_PIN_NUMBER = 27;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("RGB LED operation started.");
        using var controller = SetupController([RED_PIN_NUMBER, GREEN_PIN_NUMBER, BLUE_PIN_NUMBER]);
        using var redPWMChannel = SetupChannel(RED_PIN_NUMBER, dutyCycle: 1.0d);
        using var greenPWMChannel = SetupChannel(GREEN_PIN_NUMBER, dutyCycle: 1.0d);
        using var bluePWMChannel = SetupChannel(BLUE_PIN_NUMBER, dutyCycle: 1.0d);
        while (IsRunning())
        {
            SetOutputValue(controller, true, false, false);
            SetOutputValue(controller);
            SetOutputValue(controller, false, true, false);
            SetOutputValue(controller);
            SetOutputValue(controller, false, false, true);
            SetOutputValue(controller);
            SetOutputValue(controller, false, true, true);
            SetOutputValue(controller);
            SetOutputValue(controller, true, false, true);
            SetOutputValue(controller);
            SetOutputValue(controller, true, true, false);
            SetOutputValue(controller);
            SetOutputValue(controller, true, true, true);
            SetOutputValue(controller);
            SetOutputValue(redPWMChannel);
            SetOutputValue(controller);
            SetOutputValue(greenPWMChannel);
            SetOutputValue(controller);
            SetOutputValue(bluePWMChannel);
            SetOutputValue(controller);
        }
        DisplayService.WriteInformation("RGB LED operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set LED value
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="red"></param>
    /// <param name="green"></param>
    /// <param name="blue"></param>
    private static void SetOutputValue(GpioController controller, bool red = false, bool green = false, bool blue = false) => SetOutputValue(controller, [new(RED_PIN_NUMBER, red ? PinValue.Low : PinValue.High), new(GREEN_PIN_NUMBER, green ? PinValue.Low : PinValue.High), new(BLUE_PIN_NUMBER, blue ? PinValue.Low : PinValue.High)]);
    #endregion

}