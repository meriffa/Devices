using CommandLine;
using Iot.Device.Multiplexing;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// LED Matrix controller
/// </summary>
[Verb("LED-LEDMatrix", HelpText = "LED Matrix operation.")]
public class LEDMatrixController : LEDController
{

    #region Constants
    private const int DATA_PIN_NUMBER = 17;
    private const int SHIFT_PIN_NUMBER = 22;
    private const int LATCH_PIN_NUMBER = 27;
    private static readonly byte[] BITMAP = [
        0b_00011100,
        0b_00100010,
        0b_01010001,
        0b_01000101,
        0b_01000101,
        0b_01010001,
        0b_00100010,
        0b_00011100];
    private static readonly byte[][] BITMAP_DIGITS = [
        [0x00, 0x00, 0x3E, 0x41, 0x41, 0x3E, 0x00, 0x00],  // "0"
        [0x00, 0x00, 0x21, 0x7F, 0x01, 0x00, 0x00, 0x00],  // "1"
        [0x00, 0x00, 0x23, 0x45, 0x49, 0x31, 0x00, 0x00],  // "2"
        [0x00, 0x00, 0x22, 0x49, 0x49, 0x36, 0x00, 0x00],  // "3"
        [0x00, 0x00, 0x0E, 0x32, 0x7F, 0x02, 0x00, 0x00],  // "4"
        [0x00, 0x00, 0x79, 0x49, 0x49, 0x46, 0x00, 0x00],  // "5"
        [0x00, 0x00, 0x3E, 0x49, 0x49, 0x26, 0x00, 0x00],  // "6"
        [0x00, 0x00, 0x60, 0x47, 0x48, 0x70, 0x00, 0x00],  // "7"
        [0x00, 0x00, 0x36, 0x49, 0x49, 0x36, 0x00, 0x00],  // "8"
        [0x00, 0x00, 0x32, 0x49, 0x49, 0x3E, 0x00, 0x00],  // "9"  
        [0x00, 0x00, 0x3F, 0x44, 0x44, 0x3F, 0x00, 0x00],  // "A"
        [0x00, 0x00, 0x7F, 0x49, 0x49, 0x36, 0x00, 0x00],  // "B"
        [0x00, 0x00, 0x3E, 0x41, 0x41, 0x22, 0x00, 0x00],  // "C"
        [0x00, 0x00, 0x7F, 0x41, 0x41, 0x3E, 0x00, 0x00],  // "D"
        [0x00, 0x00, 0x7F, 0x49, 0x49, 0x41, 0x00, 0x00],  // "E"
        [0x00, 0x00, 0x7F, 0x48, 0x48, 0x40, 0x00, 0x00]]; // "F"
    private static readonly byte[] BITMAP_CLEAR = [0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00];
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("LED Matrix operation started.");
        using var shiftRegister = new Sn74hc595(new Sn74hc595PinMapping(DATA_PIN_NUMBER, SHIFT_PIN_NUMBER, LATCH_PIN_NUMBER));
        while (IsRunning())
        {
            SetLEDColumns(shiftRegister, BITMAP);
            for (int i = 0; i < BITMAP_DIGITS.Length; i++)
                SetLEDColumns(shiftRegister, BITMAP_DIGITS[i]);
        }
        SetLEDColumns(shiftRegister, BITMAP_CLEAR);
        DisplayService.WriteInformation("LED Matrix operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set LED columns
    /// </summary>
    /// <param name="shiftRegister"></param>
    /// <param name="columns"></param>
    private static void SetLEDColumns(Sn74hc595 shiftRegister, byte[] columns)
    {
        for (int i = 0; i < 100; i++)
        {
            byte column = 0x80;
            for (int j = 0; j < 8; j++)
            {
                SetLEDColumn(shiftRegister, column, columns[j]);
                column >>= 1;
                Thread.Sleep(1);
            }
        }
    }

    /// <summary>
    /// Set LED column
    /// </summary>
    /// <param name="shiftRegister"></param>
    /// <param name="column"></param>
    /// <param name="value"></param>
    private static void SetLEDColumn(Sn74hc595 shiftRegister, byte column, byte value)
    {
        SetLEDValue(shiftRegister, value);
        SetLEDValue(shiftRegister, (byte)~column);
        shiftRegister.Latch();
    }

    /// <summary>
    /// Set LED value
    /// </summary>
    /// <param name="shiftRegister"></param>
    /// <param name="value"></param>
    private static void SetLEDValue(Sn74hc595 shiftRegister, byte value)
    {
        for (int i = 0; i < 8; i++)
            shiftRegister.ShiftBit(((0x80 & (value << i)) == 0x80) ? PinValue.High : PinValue.Low);
    }
    #endregion

}