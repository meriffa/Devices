using CommandLine;
using Iot.Device.DCMotor;
using Iot.Device.ExplorerHat;
using Iot.Device.MotorHat;
using System.Device.I2c;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.Motors;

/// <summary>
/// DC Motor (HAT) controller
/// </summary>
[Verb("Motors-DCMotorHAT", HelpText = "DC Motor (HAT) operation.")]
public class DCMotorHATController : PeripheralsController
{

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
        DisplayService.WriteInformation($"DC Motor (HAT) operation started.");
        using var hat = new MotorHat(new I2cConnectionSettings(1, 0x40), 1600, new WaveshareMotorPinProvider());
        using var motor = hat.CreateDCMotor(1);
        SetSpeed(motor, Speed);
        while (IsRunning())
            Thread.Sleep(STEP_DURATION);
        motor.Stop();
        DisplayService.WriteInformation($"DC Motor (HAT) operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Set motor direction & speed
    /// </summary>
    /// <param name="motor"></param>
    /// <param name="speed"></param>
    /// <exception cref="Exception"></exception>
    private void SetSpeed(DCMotor motor, int speed)
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