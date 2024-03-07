using CommandLine;
using Iot.Device.CharacterLcd;
using Iot.Device.Pcx857x;
using System.Device.Gpio;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs;

/// <summary>
/// LCD Display controller
/// </summary>
[Verb("Outputs-LCDDisplay", HelpText = "LCD Display operation.")]
public class LCDDisplayController : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("LCD Display operation started.");
        using var driver = new Pcf8574(I2cDevice.Create(new I2cConnectionSettings(1, 0x27)));
        using var display = new Lcd1602(
            registerSelectPin: 0,
            enablePin: 2,
            dataPins: [4, 5, 6, 7],
            backlightPin: 3,
            backlightBrightness: 0.7f,
            readWritePin: 1,
            controller: new GpioController(PinNumberingScheme.Logical, driver));
        display.Clear();
        display.DisplayOn = true;
        while (IsRunning())
        {
            display.SetCursorPosition(0, 0);
            display.Write($"Date: {DateTime.Now:yyyy-MM-dd}");
            display.SetCursorPosition(0, 1);
            display.Write($"Time: {DateTime.Now:HH:mm:ss}");
            Thread.Sleep(STEP_DURATION);
        }
        display.DisplayOn = false;
        DisplayService.WriteInformation("LCD Display operation completed.");
    }
    #endregion

}