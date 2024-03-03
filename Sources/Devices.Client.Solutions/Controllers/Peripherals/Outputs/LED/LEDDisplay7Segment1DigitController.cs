using CommandLine;
using Iot.Device.Multiplexing;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// 7-Segment 1-Digit LED Display controller
/// </summary>
[Verb("LED-LEDDisplay7Segment1Digit", HelpText = "7-Segment 1-Digit LED Display operation.")]
public class LEDDisplay7Segment1DigitController : LEDController
{

    #region Constants
    private const int DATA_PIN_NUMBER = 17;
    private const int SHIFT_PIN_NUMBER = 22;
    private const int LATCH_PIN_NUMBER = 27;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"7-Segment 1-Digit LED Display operation started.");
        using var shiftRegister = new Sn74hc595(new Sn74hc595PinMapping(DATA_PIN_NUMBER, SHIFT_PIN_NUMBER, LATCH_PIN_NUMBER));
        var index = 0;
        while (IsRunning())
            SetLEDValues(shiftRegister, DIGITS[index++ % DIGITS.Length]);
        shiftRegister.ShiftByte(0xFF);
        DisplayService.WriteInformation($"7-Segment 1-Digit LED Display operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set LED values
    /// </summary>
    /// <param name="shiftRegister"></param>
    /// <param name="value"></param>
    private void SetLEDValues(Sn74hc595 shiftRegister, byte value)
    {
        shiftRegister.ShiftByte((byte)~value);
        Thread.Sleep(STEP_DURATION);
    }
    #endregion

}