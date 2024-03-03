using System.Device.Pwm.Drivers;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.LED;

/// <summary>
/// LED controller
/// </summary>
public abstract class LEDController : PeripheralsController
{

    #region Constants
    private const int FREQUENCY = 2000;
    private const int DIM_DELAY = 10;
    private const double DIM_STEP = 0.01d;
    protected static readonly byte[] DIGITS = [0x3F, 0x06, 0x5B, 0x4F, 0x66, 0x6D, 0x7D, 0x07, 0x7F, 0x6F, 0x77, 0x7C, 0x39, 0x5E, 0x79, 0x71, 0x80];
    #endregion

    #region Protected Methods
    /// <summary>
    /// Setup PWM channel
    /// </summary>
    /// <param name="pinNumber"></param>
    /// <param name="frequency"></param>
    /// <param name="dutyCycle"></param>
    /// <returns></returns>
    protected static SoftwarePwmChannel SetupChannel(int pinNumber, int frequency = FREQUENCY, double dutyCycle = 0.0d) => new(pinNumber, frequency, dutyCycle, true);

    /// <summary>
    /// Set output value
    /// </summary>
    /// <param name="pwmChannel"></param>
    protected static void SetOutputValue(SoftwarePwmChannel pwmChannel)
    {
        pwmChannel.Start();
        for (double fill = 0.0d; fill <= 1.0d; fill += DIM_STEP)
        {
            pwmChannel.DutyCycle = fill;
            Thread.Sleep(DIM_DELAY);
        }
        for (double fill = 1.0d; fill >= 0.0d; fill -= DIM_STEP)
        {
            pwmChannel.DutyCycle = fill;
            Thread.Sleep(DIM_DELAY);
        }
        pwmChannel.Stop();
        Thread.Sleep(STEP_DURATION);
    }
    #endregion

}