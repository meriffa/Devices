using CommandLine;
using Iot.Device.ServoMotor;
using System.Device.Pwm.Drivers;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.Motors;

/// <summary>
/// Servo Motor controller
/// </summary>
[Verb("Motors-ServoMotor", HelpText = "Servo Motor operation.")]
public class ServoMotorController : PeripheralsController

{

    #region Constants
    private const int PIN_NUMBER = 18;
    private const int ANGLE_STEP_DURATION = 20;
    #endregion

    #region Properties
    /// <summary>
    /// Motor angle
    /// </summary>
    [Option('a', "angle", Required = false, Default = 180, HelpText = "Motor angle (0 ... 180).")]
    public int Angle { get; set; }

    /// <summary>
    /// Motor angle step
    /// </summary>
    [Option('s', "step", Required = false, Default = 1, HelpText = "Motor angle step.")]
    public int Step { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Servo Motor operation started.");
        using var motor = SetupServoMotor();
        var angle = 0;
        var step = Step;
        while (IsRunning())
        {
            motor.WriteAngle(angle);
            Thread.Sleep(ANGLE_STEP_DURATION);
            UpdateAngleAndStep(ref angle, ref step);
        }
        motor.Stop();
        DisplayService.WriteInformation($"Servo Motor operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Setup servo motor
    /// </summary>
    /// <returns></returns>
    private static ServoMotor SetupServoMotor()
    {
        var motor = new ServoMotor(new SoftwarePwmChannel(PIN_NUMBER, frequency: 50, dutyCycle: 0.0d, usePrecisionTimer: true), 180.0d, 500, 2500);
        motor.Start();
        return motor;
    }

    /// <summary>
    /// Update servo motor angle and step
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="step"></param>
    private void UpdateAngleAndStep(ref int angle, ref int step)
    {
        angle += step;
        if (angle < 0 || angle > Angle)
        {
            step *= -1;
            angle += step;
        }
    }
    #endregion

}