using CommandLine;
using Iot.Device.Uln2003;

namespace Devices.Client.Solutions.Controllers.Peripherals.Outputs.Motors;

/// <summary>
/// Stepper Motor (28BYJ-48, ULN2003APG) controller
/// </summary>
/// <remarks>
/// 28BYJ-48 stepper motor has total of 2,048 steps = 32 magnetic poles / 1:64 gear reduction ratio
/// </remarks>
[Verb("Motors-StepperMotor", HelpText = "Stepper Motor operation.")]
public class StepperMotorController : PeripheralsController
{

    #region Constants
    private const int BLUE_PIN_NUMBER = 4;
    private const int PINK_PIN_NUMBER = 17;
    private const int YELLOW_PIN_NUMBER = 27;
    private const int ORANGE_PIN_NUMBER = 22;
    #endregion

    #region Properties
    /// <summary>
    /// Motor speed
    /// </summary>
    [Option('s', "speed", Required = false, Default = 15, HelpText = "Motor speed [RPM].")]
    public int Speed { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Stepper Motor operation started.");
        using var motor = new Uln2003(BLUE_PIN_NUMBER, PINK_PIN_NUMBER, YELLOW_PIN_NUMBER, ORANGE_PIN_NUMBER);
        motor.RPM = (short)Speed;
        while (IsRunning())
        {
            RotateMotor(motor, StepperMode.HalfStep, 2048);             // 180 degrees clockwise
            RotateMotor(motor, StepperMode.FullStepDualPhase, -2048);   // 360 degrees counterclockwise
        }
        motor.Stop();
        DisplayService.WriteInformation("Stepper Motor operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Rotate motor
    /// </summary>
    /// <param name="motor"></param>
    /// <param name="mode"></param>
    /// <param name="steps"></param>
    private static void RotateMotor(Uln2003 motor, StepperMode mode, int steps)
    {
        motor.Mode = mode;
        motor.Step(steps);
        Thread.Sleep(STEP_DURATION);
    }
    #endregion

}