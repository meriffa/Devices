using CommandLine;
using Devices.Client.Solutions.Peripherals.I2C;
using Devices.Common.Solutions.Garden.Models;
using System.Reflection;

namespace Devices.Client.Solutions.Controllers.Garden;

/// <summary>
/// Camera controller
/// </summary>
[Verb("Camera", HelpText = "Camera task.")]
public class CameraController : Controller
{

    #region Private Members
    private readonly EventWaitHandle shutdownRequest = new(false, EventResetMode.ManualReset);
    private readonly CameraState cameraState = new() { Pan = 90, Tilt = 90 };
    #endregion

    #region Properties
    /// <summary>
    /// I2C bus id
    /// </summary>
    [Option('b', "busId", Required = true, HelpText = "I2C bus id.")]
    public int BusId { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var mutex = new Mutex(true, @$"Global\{Assembly.GetExecutingAssembly().GetName().Name}.Watering", out var singleInstance);
        if (singleInstance)
        {
            DisplayService.WriteInformation("Camera task started.");
            using var panTiltDevice = new ArducamPanTilt(BusId);
            if (StartPumpRequestHandlingTask(panTiltDevice))
            {
                shutdownRequest.WaitOne();
                CameraHub.SendShutdownResponse();
                CameraHub.Stop();
            }
            DisplayService.WriteInformation("Camera task completed.");
        }
        else
            DisplayService.WriteWarning("Camera task skipped. Another task instance is already running.");

    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start pump request handling task
    /// </summary>
    /// <param name="panTiltDevice"></param>
    /// <returns></returns>
    private bool StartPumpRequestHandlingTask(ArducamPanTilt panTiltDevice)
    {
        try
        {
            CameraHub.HandleDevicePresenceConfirmationRequest(() => cameraState);
            CameraHub.HandleShutdownRequest(() => shutdownRequest.Set());
            CameraHub.HandlePanRequest((value) =>
            {
                cameraState.Pan = value;
                panTiltDevice.SetPan(value);
            });
            CameraHub.HandleTiltRequest((value) =>
            {
                cameraState.Tilt = value;
                panTiltDevice.SetTilt(value);
            });
            CameraHub.Start();
            return true;
        }
        catch (Exception ex)
        {
            DisplayService.WriteError(ex);
            return false;
        }
    }
    #endregion
}