using CommandLine;
using System.Device.Gpio;
using System.Reflection;

namespace Devices.Client.Solutions.Controllers.Garden;

/// <summary>
/// Watering controller
/// </summary>
[Verb("Watering", HelpText = "Watering task.")]
public class WateringController : Controller
{

    #region Private Members
    private static readonly int[] PIN_NUMBERS = [17, 27, 22, 5, 6, 26, 23, 24, 25, 16];
    private readonly EventWaitHandle shutdownRequest = new(false, EventResetMode.ManualReset);
    private int deviceId = 0;
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
            DisplayService.WriteInformation("Watering task started.");
            using var controller = GetController();
            if (Task.Run(async () => await StartPumpRequestHandlingTask(controller)).Result)
                shutdownRequest.WaitOne();
            foreach (var pin in PIN_NUMBERS)
                controller.Write(pin, PinValue.High);
            GardenHub.SendShutdownResponse(deviceId);
            GardenHub.Stop();
            DisplayService.WriteInformation("Watering task completed.");
        }
        else
            DisplayService.WriteWarning("Watering task skipped. Another task instance is already running.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return controller instance
    /// </summary>
    /// <returns></returns>
    private static GpioController GetController()
    {
        var controller = new GpioController(PinNumberingScheme.Logical);
        foreach (var pin in PIN_NUMBERS)
            controller.OpenPin(pin, PinMode.Output, PinValue.High);
        return controller;
    }

    /// <summary>
    /// Start pump request handling task
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private async Task<bool> StartPumpRequestHandlingTask(GpioController controller)
    {
        try
        {
            GardenHub.HandlePumpRequest((deviceId, pumpId, pumpState) =>
            {
                controller.Write(PIN_NUMBERS[pumpId - 1], pumpState ? PinValue.Low : PinValue.High);
            });
            GardenHub.HandleShutdownRequest((deviceId) =>
            {
                this.deviceId = deviceId;
                shutdownRequest.Set();
            });
            return await GardenHub.Start();
        }
        catch (Exception ex)
        {
            DisplayService.WriteError(ex);
            return false;
        }
    }
    #endregion

}