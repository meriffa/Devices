using CommandLine;

namespace Devices.Client.Controllers;

/// <summary>
/// Monitoring controller
/// </summary>
[Verb("Monitoring", HelpText = "Monitoring operation.")]
public class MonitoringController : Controller
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Monitoring operation started.");
        var metrics = MonitoringService.GetMonitoringMetrics();
        DisplayService.WriteInformation($"Last Reboot = {metrics.Device.LastRebootDate:yyyy-MM-dd HH:mm:ss}");
        DisplayService.WriteInformation($"CPU User Time = {metrics.Device.Cpu.User:F1} %");
        DisplayService.WriteInformation($"CPU System Time = {metrics.Device.Cpu.System:F1} %");
        DisplayService.WriteInformation($"CPU Idle Time = {metrics.Device.Cpu.Idle:F1} %");
        DisplayService.WriteInformation($"Total Memory = {metrics.Device.Memory.Total:N0} MB");
        DisplayService.WriteInformation($"Used Memory = {metrics.Device.Memory.Used:N0} MB");
        DisplayService.WriteInformation($"Free Memory = {metrics.Device.Memory.Free:N0} MB");
        DisplayService.WriteInformation("Monitoring operation completed.");
    }
    #endregion

}