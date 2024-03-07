using CommandLine;
using Devices.Client.Solutions.Peripherals.I2C;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Joystick controller
/// </summary>
[Verb("Inputs-Joystick", HelpText = "Joystick operation.")]
public class JoystickController : PeripheralsController
{

    #region Converter Type
    /// <summary>
    /// Converter type
    /// </summary>
    public enum ConverterType
    {
        PCF8591,
        ADS7830
    }
    #endregion

    #region Constants
    private const int PIN_NUMBER = 18;
    #endregion

    #region Properties
    /// <summary>
    /// A/D Converter type
    /// </summary>
    [Option('t', "type", Required = true, HelpText = "A/D Converter type.")]
    public ConverterType Converter { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Joystick operation started.");
        using var controller = SetupController([PIN_NUMBER], PinMode.InputPullUp);
        using var input = SetupADConverter();
        DisplayService.WriteInformation($"Initial Button State = {GetJoystickButtonState(controller)}");
        controller.RegisterCallbackForPinValueChangedEvent(PIN_NUMBER, PinEventTypes.Rising | PinEventTypes.Falling, (_, e) => DisplayService.WriteInformation($"New Button State = {GetJoystickButtonState(e.ChangeType)}"));
        while (IsRunning())
        {
            DisplayService.WriteInformation($"X = {input.ReadInput(0)}, Y = {input.ReadInput(1)}");
            Thread.Sleep(STEP_DURATION);
        }
        DisplayService.WriteInformation("Joystick operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Setup A/D converter
    /// </summary>
    /// <returns></returns>
    private IADConverter SetupADConverter() => Converter == ConverterType.PCF8591 ? new PCF8591() : new ADS7830();

    /// <summary>
    /// Return joystick button state
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private static string GetJoystickButtonState(GpioController controller) => controller.Read(PIN_NUMBER) == PinValue.Low ? "Pressed" : "Released";

    /// <summary>
    /// Return joystick button state
    /// </summary>
    /// <param name="changeType"></param>
    /// <returns></returns>
    private static string GetJoystickButtonState(PinEventTypes changeType) => changeType == PinEventTypes.Falling ? "Pressed" : "Released";
    #endregion

}