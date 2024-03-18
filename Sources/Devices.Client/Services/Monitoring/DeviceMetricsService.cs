using Devices.Client.Interfaces.Monitoring;
using Devices.Common.Models.Monitoring;
using System.Diagnostics;
using System.Globalization;

namespace Devices.Client.Services.Monitoring;

/// <summary>
/// Device metrics service
/// </summary>
public class DeviceMetricsService : IDeviceMetricsService
{

    #region Public Methods
    /// <summary>
    /// Return device metrics
    /// </summary>
    /// <returns></returns>
    public DeviceMetrics GetMetrics()
    {
        if (OperatingSystem.IsLinux())
        {
            var lastRebootDate = GetLinuxLastRebootDate();
            var cpu = GetLinuxCpuMetrics();
            var memory = GetLinuxMemoryMetrics();
            return new()
            {
                DeviceDate = DateTime.UtcNow,
                LastRebootDate = lastRebootDate,
                Cpu = cpu,
                Memory = memory
            };
        }
        throw new("Device metrics not supported on current platform.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return Linux last reboot date
    /// </summary>
    /// <returns></returns>
    private static DateTime GetLinuxLastRebootDate()
    {
        using var process = Process.Start(new ProcessStartInfo("uptime") { Arguments = "-s", RedirectStandardOutput = true });
        var output = process!.StandardOutput.ReadToEnd().Trim();
        return DateTime.ParseExact(output, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToUniversalTime();
    }

    /// <summary>
    /// Return Linux CPU metrics
    /// </summary>
    /// <returns></returns>
    private static CpuMetrics GetLinuxCpuMetrics()
    {
        using var process = Process.Start(new ProcessStartInfo("top") { Arguments = "-bn2", RedirectStandardOutput = true });
        var lines = process!.StandardOutput.ReadToEnd().Split("\n");
        var cpu = lines[2][(lines[2].IndexOf(':') + 1)..].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new()
        {
            User = Convert.ToSingle(cpu[0][..cpu[0].IndexOf(' ')]) + Convert.ToSingle(cpu[2][..cpu[2].IndexOf(' ')]),
            System = Convert.ToSingle(cpu[1][..cpu[1].IndexOf(' ')]),
            Idle = Convert.ToSingle(cpu[3][..cpu[3].IndexOf(' ')])
        };
    }

    /// <summary>
    /// Return Linux memory metrics
    /// </summary>
    /// <returns></returns>
    private static MemoryMetrics GetLinuxMemoryMetrics()
    {
        using var process = Process.Start(new ProcessStartInfo("free") { Arguments = "-m", RedirectStandardOutput = true });
        var lines = process!.StandardOutput.ReadToEnd().Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return new()
        {
            Total = Convert.ToInt32(memory[1]),
            Used = Convert.ToInt32(memory[2]),
            Free = Convert.ToInt32(memory[3])
        };
    }
    #endregion

}