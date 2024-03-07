using CommandLine;
using System.Device.Gpio;
using System.Device.Pwm.Drivers;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.Motors;

/// <summary>
/// DC Motor (L293D) controller
/// </summary>
[Verb("Motors-DCMotorL293D", HelpText = "DC Motor (L293D) operation.")]
public class DCMotorL293DController : PeripheralsController
{

    #region Constants
    private const int ENABLE_PIN_NUMBER = 22;
    private const int INPUT_1_PIN_NUMBER = 27;
    private const int INPUT_2_PIN_NUMBER = 17;
    #endregion

    #region Properties
    /// <summary>
    /// Motor speed
    /// </summary>
    [Option('s', "speed", Required = true, HelpText = "Motor speed (-100 ... +100).")]
    public int Speed { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("DC Motor (L293D) operation started.");
        using var controller = SetupController([INPUT_1_PIN_NUMBER, INPUT_2_PIN_NUMBER]);
        using var motor = SetupMotor();
        SetSpeed(controller, motor, Speed);
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        SetSpeed(controller, motor, 0);
        motor.Stop();
        DisplayService.WriteInformation("DC Motor (L293D) operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Setup DC motor
    /// </summary>
    /// <returns></returns>
    private static SoftwarePwmChannel SetupMotor()
    {
        var pwmChannel = new SoftwarePwmChannel(ENABLE_PIN_NUMBER, dutyCycle: 0.0d, usePrecisionTimer: true);
        pwmChannel.Start();
        return pwmChannel;
    }

    /// <summary>
    /// Set motor direction & speed
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="motor"></param>
    /// <param name="speed"></param>
    private void SetSpeed(GpioController controller, SoftwarePwmChannel motor, int speed)
    {
        if (speed > 100 || speed < -100)
            throw new Exception($"{nameof(speed)} value is outside the allowed range (-100 ... +100).");
        controller.Write(INPUT_1_PIN_NUMBER, speed > 0 ? PinValue.High : PinValue.Low);
        controller.Write(INPUT_2_PIN_NUMBER, speed < 0 ? PinValue.High : PinValue.Low);
        motor.DutyCycle = Math.Abs(speed) / 100.0d;
        DisplayService.WriteInformation($"Speed = {speed}%, Direction = {GetDirection(speed)}");
    }

    /// <summary>
    /// Return motor direction
    /// </summary>
    /// <param name="speed"></param>
    /// <returns></returns>
    private static string GetDirection(int speed) => speed > 0 ? "Forward" : (speed < 0 ? "Reverse" : "Stopped");
    #endregion

}