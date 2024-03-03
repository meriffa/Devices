using CommandLine;
using Iot.Device.Multiplexing;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// 7-Segment 4-Digit LED Display controller
/// </summary>
[Verb("LED-LEDDisplay7Segment4Digit", HelpText = "7-Segment 4-Digit LED Display operation.")]
public class LEDDisplay7Segment4DigitController : LEDController
{

    #region Constants
    private const int DATA_PIN_NUMBER = 24;
    private const int SHIFT_PIN_NUMBER = 18;
    private const int LATCH_PIN_NUMBER = 23;
    private static readonly int[] DIGIT_SELECT_PIN_NUMBER = [10, 22, 27, 17];
    #endregion

    #region Private Fields
    private ushort value = 0;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"7-Segment 4-Digit LED Display operation started.");
        using var controller = SetupController(DIGIT_SELECT_PIN_NUMBER.ToDictionary(i => i, _ => PinValue.High));
        using var shiftRegister = new Sn74hc595(new Sn74hc595PinMapping(DATA_PIN_NUMBER, SHIFT_PIN_NUMBER, LATCH_PIN_NUMBER));
        using var timer = SetupTimer(1000);
        while (IsRunning())
            SetLEDValues(controller, shiftRegister, value);
        timer.Enabled = false;
        shiftRegister.ShiftByte(0xFF);
        DisplayService.WriteInformation($"7-Segment 4-Digit LED Display operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Setup timer
    /// </summary>
    /// <param name="interval"></param>
    /// <returns></returns>
    private System.Timers.Timer SetupTimer(double interval)
    {
        var timer = new System.Timers.Timer(interval) { AutoReset = true, Enabled = true };
        timer.Elapsed += (_, _) => value++;
        return timer;
    }

    /// <summary>
    /// Select digit
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="pins"></param>
    /// <param name="digit"></param>
    private static void SelectDigit(GpioController controller, int digit)
    {
        if (digit >= DIGIT_SELECT_PIN_NUMBER.Length)
            throw new($"Invalid digit value {digit} specified.");
        for (int i = 0; i < DIGIT_SELECT_PIN_NUMBER.Length; i++)
            controller.Write(DIGIT_SELECT_PIN_NUMBER[i], i == digit ? PinValue.Low : PinValue.High);
    }

    /// <summary>
    /// Set LED values
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="shiftRegister"></param>
    /// <param name="value"></param>
    private static void SetLEDValues(GpioController controller, Sn74hc595 shiftRegister, ushort value)
    {
        for (int i = 0; i < 4; i++)
        {
            SelectDigit(controller, i);
            shiftRegister.ShiftByte((byte)~DIGITS[value & 0xF]);
            value >>= 4;
            Thread.Sleep(3);
        }
    }
    #endregion

}