using CommandLine;
using Iot.Device.KeyMatrix;
using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals.Inputs;

/// <summary>
/// Keypad controller
/// </summary>
[Verb("Inputs-Keypad", HelpText = "Keypad operation.")]
public class KeypadController : PeripheralsController
{

    #region Constants
    private static readonly int[] OUTPUT_PIN_NUMBERS = [17, 27, 22, 10];
    private static readonly int[] INPUT_PIN_NUMBERS = [18, 23, 24, 25];
    private static readonly char[,] KEY_VALUE = {
        {'D', 'C', 'B', 'A'},
        {'#', '9', '6', '3'},
        {'0', '8', '5', '2'},
        {'*', '7', '4', '1'}
    };
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Keypad operation started.");
        using var keyMatrix = new KeyMatrix(OUTPUT_PIN_NUMBERS, INPUT_PIN_NUMBERS, TimeSpan.FromMilliseconds(20), PinMode.InputPullDown);
        keyMatrix.KeyEvent += (s, e) => { DisplayService.WriteInformation($"Key = '{GetKey(e.Output, e.Input)}', Event = {GetEventType(e.EventType)}"); };
        keyMatrix.StartListeningKeyEvent();
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        keyMatrix.StopListeningKeyEvent();
        DisplayService.WriteInformation("Keypad operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return event type
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    private static string GetEventType(PinEventTypes eventType) => eventType == PinEventTypes.Rising ? "Pressed" : "Released";

    /// <summary>
    /// Return key
    /// </summary>
    /// <param name="output"></param>
    /// <param name="input"></param>
    /// <returns></returns>
    private static char GetKey(int output, int input) => KEY_VALUE[output, input];
    #endregion

}