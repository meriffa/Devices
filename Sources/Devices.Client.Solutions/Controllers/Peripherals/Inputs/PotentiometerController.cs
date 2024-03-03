using CommandLine;
using Devices.Client.Solutions.Peripherals.I2C;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Potentiometer controller
/// </summary>
[Verb("Inputs-Potentiometer", HelpText = "Potentiometer operation.")]
public class PotentiometerController : PeripheralsController
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Potentiometer operation started.");
        using var input = new ADS7830();
        while (IsRunning())
        {
            DisplayService.WriteInformation($"Value = {input.ReadInput(0)}");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation($"Potentiometer operation completed.");
    }
    #endregion

}