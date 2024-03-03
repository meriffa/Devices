using CommandLine;
using Iot.Device.Multiplexing;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// LED Bar Graph controller
/// </summary>
[Verb("LED-LEDBarGraph", HelpText = "LED Bar Graph operation.")]
public class LEDBarGraphController : LEDController
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
        DisplayService.WriteInformation($"LED Bar Graph operation started.");
        using var shiftRegister = new Sn74hc595(new Sn74hc595PinMapping(DATA_PIN_NUMBER, SHIFT_PIN_NUMBER, LATCH_PIN_NUMBER));
        var value = PinValue.High;
        while (IsRunning())
            value = !SetLEDValues(shiftRegister, value);
        shiftRegister.ShiftByte(0b0000_0000);
        DisplayService.WriteInformation($"LED Bar Graph operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set LED values
    /// </summary>
    /// <param name="shiftRegister"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    private static PinValue SetLEDValues(Sn74hc595 shiftRegister, PinValue value)
    {
        for (int i = 0; i < 8; i++)
        {
            shiftRegister.ShiftBit(value);
            shiftRegister.Latch();
            Thread.Sleep(100);
        }
        return value;
    }
    #endregion

}