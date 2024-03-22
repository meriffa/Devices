using CommandLine;
using Devices.Client.Models;

namespace Devices.Client.Controllers;

/// <summary>
/// Task controller
/// </summary>
[Verb("execute", HelpText = "Execute tasks.")]
public class TaskController : Controller
{

    #region Properties
    /// <summary>
    /// Task types
    /// </summary>
    [Option('t', "tasks", Required = true, HelpText = "Task types.")]
    public TaskTypes Tasks { get; set; }

    /// <summary>
    /// Force identity refresh flag
    /// </summary>
    [Option('r', "refresh", Required = false, Default = false, HelpText = "Force identity refresh.")]
    public bool Refresh { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        if (Tasks.HasFlag(TaskTypes.Identity))
            ExecuteIdentityTask();
        if (Tasks.HasFlag(TaskTypes.Monitoring))
            ExecuteMonitoringTask();
        if (Tasks.HasFlag(TaskTypes.Configuration))
            ExecuteConfigurationTask();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Execute identity task
    /// </summary>
    private void ExecuteIdentityTask()
    {
        DisplayService.WriteInformation("Identity task started.");
        DisplayService.WriteInformation($"Device Token = {IdentityService.GetDeviceToken(Refresh)}");
        DisplayService.WriteInformation("Identity task completed.");
    }

    /// <summary>
    /// Execute monitoring task
    /// </summary>
    private void ExecuteMonitoringTask()
    {
        DisplayService.WriteInformation("Monitoring task started.");
        var metrics = MonitoringService.GetDeviceMetrics();
        DisplayService.WriteInformation($"Last Reboot = {metrics.LastRebootDate:yyyy-MM-dd HH:mm:ss}");
        DisplayService.WriteInformation($"CPU User Time = {metrics.Cpu.User:F1} %");
        DisplayService.WriteInformation($"CPU System Time = {metrics.Cpu.System:F1} %");
        DisplayService.WriteInformation($"CPU Idle Time = {metrics.Cpu.Idle:F1} %");
        DisplayService.WriteInformation($"Total Memory = {metrics.Memory.Total:N0} MB");
        DisplayService.WriteInformation($"Used Memory = {metrics.Memory.Used:N0} MB");
        DisplayService.WriteInformation($"Free Memory = {metrics.Memory.Free:N0} MB");
        DisplayService.WriteInformation("Monitoring task completed.");
    }

    /// <summary>
    /// Execute configuration task
    /// </summary>
    private void ExecuteConfigurationTask()
    {
        DisplayService.WriteInformation("Configuration task started.");
        ReleaseGraphService.Build();
        ReleaseGraphService.Validate();
        ReleaseGraphService.Execute();
        DisplayService.WriteInformation("Configuration task completed.");
    }
    #endregion

}