using System.Device.Gpio;

namespace Devices.Client.Solutions.Controllers.Peripherals;

/// <summary>
/// Base peripherals controller
/// </summary>
public abstract class PeripheralsController : Controller
{

    #region Constants
    protected const int STEP_DURATION = 1000;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Setup GPIO controller
    /// </summary>
    /// <param name="pins"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected GpioController SetupController(int[] pins, PinMode mode = PinMode.Output)
    {
        return SetupController(GetController(), pins, mode);
    }

    /// <summary>
    /// Setup GPIO controller
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="pins"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected static GpioController SetupController(GpioController controller, int[] pins, PinMode mode)
    {
        foreach (var pin in pins)
            controller.OpenPin(pin, mode);
        return controller;
    }

    /// <summary>
    /// Setup GPIO controller
    /// </summary>
    /// <param name="pins"></param>
    /// <param name="mode"></param>
    /// <returns></returns>
    protected GpioController SetupController(Dictionary<int, PinValue> pins, PinMode mode = PinMode.Output)
    {
        GpioController controller = GetController();
        foreach (var pin in pins)
            controller.OpenPin(pin.Key, mode, pin.Value);
        return controller;
    }

    /// <summary>
    /// Set output value
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="pinNumber"></param>
    /// <param name="value"></param>
    protected static void SetOutputValue(GpioController controller, int pinNumber, PinValue value)
    {
        controller.Write(pinNumber, value);
        Thread.Sleep(STEP_DURATION);
    }

    /// <summary>
    /// Set output value
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="pinValues"></param>
    protected static void SetOutputValue(GpioController controller, ReadOnlySpan<PinValuePair> pinValues)
    {
        controller.Write(pinValues);
        Thread.Sleep(STEP_DURATION);
    }

    /// <summary>
    /// Check if application is running
    /// </summary>
    /// <returns></returns>
    protected static bool IsRunning() => Console.IsInputRedirected || !Console.KeyAvailable || Console.ReadKey(true).Key != ConsoleKey.Enter;
    #endregion

    #region Private Methods
    /// <summary>
    /// Return controller instance
    /// </summary>
    /// <returns></returns>
    private GpioController GetController()
    {
        DisplayService.WriteInformation("Press [Enter] to exit.");
        return new(PinNumberingScheme.Logical);
    }
    #endregion

}