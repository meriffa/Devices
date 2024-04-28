using System.Reflection;
using CommandLine;

namespace Devices.Client.Solutions.Controllers.Garden;

/// <summary>
/// Watering controller
/// </summary>
[Verb("watering", HelpText = "Watering task.")]
public class WateringController : Controller
{

    private readonly EventWaitHandle shutdownRequest = new(false, EventResetMode.ManualReset);

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
            if (Task.Run(StartPumpRequestHandlingTask).Result)
                shutdownRequest.WaitOne();
            GardenHub.Stop();
            DisplayService.WriteInformation("Watering task completed.");
        }
        else
            DisplayService.WriteWarning("Watering task skipped. Another task instance is already running.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Start pump request handling task
    /// </summary>
    /// <returns></returns>
    private async Task<bool> StartPumpRequestHandlingTask()
    {
        try
        {
            GardenHub.HandlePumpRequest((deviceId, pumpId, pumpState) =>
            {
                DisplayService.WriteInformation("[Under Construction].");
            });
            GardenHub.HandleShutdownRequest((deviceId) =>
            {
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