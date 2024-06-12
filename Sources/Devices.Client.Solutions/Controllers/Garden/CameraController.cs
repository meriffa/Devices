using CommandLine;
using Devices.Client.Solutions.Peripherals.Camera;
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
    private readonly CameraState cameraState = new() { Pan = 90, Tilt = 90, FocusMinimum = 0.0d, FocusMaximum = 0.0d, Focus = 0.0d, Zoom = 1.0d };
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
        using var mutex = new Mutex(true, @$"Global\{Assembly.GetExecutingAssembly().GetName().Name}.Camera", out var singleInstance);
        if (singleInstance)
        {
            DisplayService.WriteInformation("Camera task started.");
            using var cameraDevice = new RaspberryPiCameraModule(1920, 1080, 50, "https://HOST_SBC:8443/camera");
            using var panTiltDevice = new ArducamPanTilt(BusId);
            if (StartPumpRequestHandlingTask(cameraDevice, panTiltDevice))
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
    /// <param name="cameraDevice"></param>
    /// <param name="panTiltDevice"></param>
    /// <returns></returns>
    private bool StartPumpRequestHandlingTask(RaspberryPiCameraModule cameraDevice, ArducamPanTilt panTiltDevice)
    {
        try
        {
            CameraHub.HandleDevicePresenceConfirmationRequest(() =>
            {
                (cameraState.FocusMinimum, cameraState.FocusMaximum) = cameraDevice.GetFocusRange();
                return cameraState;
            });
            CameraHub.HandleShutdownRequest(() =>
            {
                cameraDevice.Stop();
                shutdownRequest.Set();
            });
            CameraHub.HandlePanRequest((value) => panTiltDevice.SetPan(cameraState.Pan = value));
            CameraHub.HandleTiltRequest((value) => panTiltDevice.SetTilt(cameraState.Tilt = value));
            CameraHub.HandleFocusRequest((value) => cameraDevice.SetFocus(cameraState.Focus = value));
            CameraHub.HandleZoomRequest((value) => cameraDevice.SetZoom(cameraState.Zoom = value));
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