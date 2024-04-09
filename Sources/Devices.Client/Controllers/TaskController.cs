using CommandLine;
using Devices.Client.Models;
using System.Reflection;

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
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        using var mutex = new Mutex(true, @$"Global\{Assembly.GetExecutingAssembly().GetName().Name}", out var singleInstance);
        if (Tasks.HasFlag(TaskTypes.Monitoring) || Tasks.HasFlag(TaskTypes.Configuration))
            Common.Services.DisplayService.WriteTitle();
        if (Tasks.HasFlag(TaskTypes.Identity))
            ExecuteIdentityTask();
        if (Tasks.HasFlag(TaskTypes.Monitoring))
            ExecuteMonitoringTask();
        if (Tasks.HasFlag(TaskTypes.Configuration))
            ExecuteConfigurationTask(singleInstance);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Execute identity task
    /// </summary>
    private void ExecuteIdentityTask()
    {
        Console.WriteLine(IdentityService.GetDeviceBearerToken());
    }

    /// <summary>
    /// Execute monitoring task
    /// </summary>
    private void ExecuteMonitoringTask()
    {
        DisplayService.WriteInformation("Monitoring task started.");
        var metrics = MonitoringService.SaveDeviceMetrics();
        DisplayService.WriteInformation($"Last Reboot = {metrics.LastRebootDate:yyyy-MM-dd HH:mm:ss}");
        DisplayService.WriteInformation($"CPU User Time = {metrics.Cpu.User:F1} %");
        DisplayService.WriteInformation($"CPU System Time = {metrics.Cpu.System:F1} %");
        DisplayService.WriteInformation($"CPU Idle Time = {metrics.Cpu.Idle:F1} %");
        DisplayService.WriteInformation($"CPU Temperature = {metrics.Cpu.Temperature:F1} ℃");
        DisplayService.WriteInformation($"Total Memory = {metrics.Memory.Total:N0} MB");
        DisplayService.WriteInformation($"Used Memory = {metrics.Memory.Used:N0} MB");
        DisplayService.WriteInformation($"Free Memory = {metrics.Memory.Free:N0} MB");
        DisplayService.WriteInformation("Monitoring task completed.");
    }

    /// <summary>
    /// Execute configuration task
    /// </summary>
    /// <param name="singleInstance"></param>
    private void ExecuteConfigurationTask(bool singleInstance)
    {
        DisplayService.WriteInformation("Configuration task started.");
        var allowConcurrency = ReleaseGraphService.Build();
        if (singleInstance || allowConcurrency)
        {
            ReleaseGraphService.Validate();
            ReleaseGraphService.Execute();
            DisplayService.WriteInformation("Configuration task completed.");
        }
        else
            DisplayService.WriteWarning("Configuration task skipped. Another application instance is already running.");
    }
    #endregion

}