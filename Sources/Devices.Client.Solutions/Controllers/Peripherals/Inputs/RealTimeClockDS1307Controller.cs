using CommandLine;
using Iot.Device.Rtc;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Real Time Clock (DS1307) controller
/// </summary>
[Verb("Inputs-RealTimeClockDS1307", HelpText = "Real Time Clock (DS1307) operation.")]
public class RealTimeClockDS1307Controller : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Real Time Clock (DS1307) operation started.");
        using var clock = new Ds1307(I2cDevice.Create(new I2cConnectionSettings(1, Ds1307.DefaultI2cAddress)));
        clock.DateTime = DateTime.Now;
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Value = {clock.DateTime:yyyy-MM-dd HH:mm:ss}");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation("Real Time Clock (DS1307) operation completed.");
    }
    #endregion

}