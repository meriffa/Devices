using CommandLine;
using Devices.Client.Solutions.Peripherals.SPI;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Real Time Clock (DS1302) controller
/// </summary>
[Verb("Inputs-RealTimeClockDS1302", HelpText = "Real Time Clock (DS1302) operation.")]
public class RealTimeClockDS1302Controller : PeripheralsController
{

    #region Constants
    private const int CLOCK_PIN_NUMBER = 23;
    private const int DATA_PIN_NUMBER = 24;
    private const int CLOCK_SELECT_PIN_NUMBER = 25;
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Real Time Clock (DS1302) operation started.");
        using var clock = new DS1302(CLOCK_PIN_NUMBER, DATA_PIN_NUMBER, CLOCK_SELECT_PIN_NUMBER);
        if (!clock.Enabled)
            clock.Enabled = true;
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Value = {clock.DateTime:yyyy-MM-dd HH:mm:ss}");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation("Real Time Clock (DS1302) operation completed.");
    }
    #endregion

}