using CommandLine;
using Devices.Client.Solutions.Peripherals.I2C;

namespace Devices.Client.Solutions.Controllers.Garden;

/// <summary>
/// Camera controller
/// </summary>
[Verb("Camera", HelpText = "Camera task.")]
public class CameraController : Controller
{

    #region Properties
    /// <summary>
    /// I2C bus id
    /// </summary>
    [Option('b', "busId", Required = true, HelpText = "I2C bus id.")]
    public int BusId { get; set; }

    /// <summary>
    /// Pan degrees
    /// </summary>
    [Option('p', "pan", Required = true, HelpText = "Pan degrees.")]
    public int Pan { get; set; }

    /// <summary>
    /// Tilt degrees
    /// </summary>
    [Option('t', "tilt", Required = true, HelpText = "Tilt degrees.")]
    public int Tilt { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Camera task started.");
        using var device = new ArducamPanTilt(BusId);
        device.SetPan(Pan);
        device.SetTilt(Tilt);
        DisplayService.WriteInformation("Camera task completed.");
    }
    #endregion

}