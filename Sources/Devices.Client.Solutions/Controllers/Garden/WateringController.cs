using CommandLine;
using System.Device.Gpio;
using System.Reflection;
using System.Timers;

namespace Devices.Client.Solutions.Controllers.Garden;

/// <summary>
/// Watering controller
/// </summary>
[Verb("Watering", HelpText = "Watering task.")]
public class WateringController : Controller
{

    #region Private Members
    private static readonly int[] PIN_NUMBERS = [17, 27, 22, 5, 6, 26, 23, 24, 25, 16];
    private static readonly bool[] pumpStates = new bool[PIN_NUMBERS.Length];
    private readonly EventWaitHandle shutdownRequest = new(false, EventResetMode.ManualReset);
    private static readonly object watchdogTimerSync = new();
    private readonly System.Timers.Timer watchdogTimer = new(TimeSpan.FromSeconds(10));
    private bool presenceRequested;
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
            SetupWatchdogTimer();
            if (StartPumpRequestHandlingTask(controller))
            {
                shutdownRequest.WaitOne();
                ClearOutputs(controller);
                watchdogTimer.Stop();
                GardenHub.SendShutdownResponse();
                GardenHub.Stop();
            }
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
    /// Clear outputs
    /// </summary>
    /// <param name="controller"></param>
    private static void ClearOutputs(GpioController controller)
    {
        foreach (var pin in PIN_NUMBERS)
            controller.Write(pin, PinValue.High);
    }

    /// <summary>
    /// Start pump request handling task
    /// </summary>
    /// <param name="controller"></param>
    /// <returns></returns>
    private bool StartPumpRequestHandlingTask(GpioController controller)
    {
        try
        {
            GardenHub.HandleDevicePresenceConfirmationRequest();
            GardenHub.HandlePumpRequest((pumpIndex, pumpState) =>
            {
                controller.Write(PIN_NUMBERS[pumpIndex], pumpState ? PinValue.Low : PinValue.High);
                pumpStates[pumpIndex] = pumpState;
            });
            GardenHub.HandlePresenceConfirmationResponse(() =>
            {
                presenceRequested = false;
            });
            GardenHub.HandleShutdownRequest(() =>
            {
                shutdownRequest.Set();
            });
            GardenHub.Start();
            return true;
        }
        catch (Exception ex)
        {
            DisplayService.WriteError(ex);
            return false;
        }
    }

    /// <summary>
    /// Setup watchdog timer
    /// </summary>
    private void SetupWatchdogTimer()
    {
        presenceRequested = false;
        watchdogTimer.AutoReset = true;
        watchdogTimer.Elapsed += HandleWatchdogTimerEvent;
        watchdogTimer.Start();
    }

    /// <summary>
    /// Handle watchdog timer event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void HandleWatchdogTimerEvent(object? sender, ElapsedEventArgs e)
    {
        lock (watchdogTimerSync)
            if (pumpStates.Contains(true))
                if (!presenceRequested)
                {
                    GardenHub.SendPresenceConfirmationRequest();
                    presenceRequested = true;
                }
                else
                {
                    DisplayService.WriteWarning("Presence confirmation not received. Shutting down.");
                    shutdownRequest.Set();
                }
            else
                presenceRequested = false;
    }
    #endregion

}