using CommandLine;
using Iot.Device.DCMotor;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.Motors;

/// <summary>
/// DC Motor (L298N) controller
/// </summary>
[Verb("Motors-DCMotorL298N", HelpText = "DC Motor (L298N) operation.")]
public class DCMotorL298NController : PeripheralsController
{

    #region Constants
    private const int SPEED_CONTROL_PIN_NUMBER = 6;
    private const int DIRECTION_1_PIN_NUMBER = 27;
    private const int DIRECTION_2_PIN_NUMBER = 22;
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
        DisplayService.WriteInformation($"DC Motor (L298N) operation started.");
        using var motor = new DCMotorWithStartStop(DCMotor.Create(SPEED_CONTROL_PIN_NUMBER, DIRECTION_1_PIN_NUMBER, DIRECTION_2_PIN_NUMBER));
        SetSpeed(motor, Speed);
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        motor.Stop();
        DisplayService.WriteInformation($"DC Motor (L298N) operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set motor direction & speed
    /// </summary>
    /// <param name="motor"></param>
    /// <param name="speed"></param>
    /// <exception cref="Exception"></exception>
    private void SetSpeed(DCMotorWithStartStop motor, int speed)
    {
        if (speed > 100 || speed < -100)
            throw new Exception($"{nameof(speed)} value is outside the allowed range (-100 ... +100).");
        motor.Speed = speed / 100.0d;
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