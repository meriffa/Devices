using CommandLine;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs;

/// <summary>
/// Generic Output controller
/// </summary>
[Verb("Outputs-GenericOutput", HelpText = "Generic Output operation.")]
public class GenericOutputController : PeripheralsController
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
        DisplayService.WriteInformation($"Generic Output operation started.");
        using var controller = SetupController([PIN_NUMBER]);
        while (IsRunning())
        {
            SetOutputValue(controller, PIN_NUMBER, PinValue.High);
            SetOutputValue(controller, PIN_NUMBER, PinValue.Low);
        }
        DisplayService.WriteInformation($"Generic Output operation completed.");
    }
    #endregion

}