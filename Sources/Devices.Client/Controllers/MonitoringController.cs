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
        DisplayService.WriteInformation($"Last Reboot = {metrics.DeviceMetrics.LastRebootDate:yyyy-MM-dd HH:mm:ss}");
        DisplayService.WriteInformation($"CPU User Time = {metrics.DeviceMetrics.Cpu.User:F1} %");
        DisplayService.WriteInformation($"CPU System Time = {metrics.DeviceMetrics.Cpu.System:F1} %");
        DisplayService.WriteInformation($"CPU Idle Time = {metrics.DeviceMetrics.Cpu.Idle:F1} %");
        DisplayService.WriteInformation($"Total Memory = {metrics.DeviceMetrics.Memory.Total:N0} MB");
        DisplayService.WriteInformation($"Used Memory = {metrics.DeviceMetrics.Memory.Used:N0} MB");
        DisplayService.WriteInformation($"Free Memory = {metrics.DeviceMetrics.Memory.Free:N0} MB");
        DisplayService.WriteInformation("Monitoring operation completed.");
    }
    #endregion

}