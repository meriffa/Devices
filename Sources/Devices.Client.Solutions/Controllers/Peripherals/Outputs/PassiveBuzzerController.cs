using CommandLine;
using Iot.Device.Buzzer;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs;

/// <summary>
/// Passive Buzzer controller
/// </summary>
[Verb("Outputs-PassiveBuzzer", HelpText = "Passive Buzzer operation.")]
public class PassiveBuzzerController : PeripheralsController
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
        DisplayService.WriteInformation($"Passive Buzzer operation started.");
        using var buzzer = new Buzzer(PIN_NUMBER);
        while (IsRunning())
        {
            buzzer.PlayTone(440, 1000);
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Passive Buzzer operation completed.");
    }
    #endregion

}