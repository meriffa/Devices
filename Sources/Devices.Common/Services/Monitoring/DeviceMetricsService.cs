using Devices.Common.Interfaces.Monitoring;
using Devices.Common.Models.Monitoring;
using System.Diagnostics;
using System.Globalization;

namespace Devices.Common.Services.Monitoring;

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
            var kernelVersion = GetLinuxKernelVersion();
            var cpu = GetLinuxCpuMetrics();
            var memory = GetLinuxMemoryMetrics();
            var disk = GetLinuxDiskMetrics();
            return new()
            {
                DeviceDate = DateTime.UtcNow,
                LastRebootDate = lastRebootDate,
                KernelVersion = kernelVersion,
                Cpu = cpu,
                Memory = memory,
                Disk = disk
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
        process!.WaitForExit();
        var output = process.StandardOutput.ReadToEnd().Trim();
        return DateTime.ParseExact(output, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture).ToUniversalTime();
    }

    /// <summary>
    /// Return Linux kernel version
    /// </summary>
    /// <returns></returns>
    private static string GetLinuxKernelVersion()
    {
        using var process = Process.Start(new ProcessStartInfo("uname") { Arguments = "-a", RedirectStandardOutput = true });
        process!.WaitForExit();
        return process.StandardOutput.ReadToEnd().Trim();
    }

    /// <summary>
    /// Return Linux CPU metrics
    /// </summary>
    /// <returns></returns>
    private static CpuMetrics GetLinuxCpuMetrics()
    {
        using var process = Process.Start(new ProcessStartInfo("top") { Arguments = "-bn2", RedirectStandardOutput = true });
        process!.WaitForExit();
        var lines = process.StandardOutput.ReadToEnd().Split("\n");
        var cpu = lines[2][(lines[2].IndexOf(':') + 1)..].Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        return new()
        {
            User = Convert.ToDouble(cpu[0][..cpu[0].IndexOf(' ')]) + Convert.ToDouble(cpu[2][..cpu[2].IndexOf(' ')]),
            System = Convert.ToDouble(cpu[1][..cpu[1].IndexOf(' ')]),
            Idle = Convert.ToDouble(cpu[3][..cpu[3].IndexOf(' ')]),
            Temperature = GetLinuxCpuTemperature()
        };
    }

    /// <summary>
    /// Return Linux CPU temperature
    /// </summary>
    /// <returns></returns>
    private static double GetLinuxCpuTemperature()
    {
        using var process = Process.Start(new ProcessStartInfo("cat") { Arguments = "/sys/class/thermal/thermal_zone0/temp", RedirectStandardOutput = true });
        process!.WaitForExit();
        try
        {
            return Convert.ToDouble(process.StandardOutput.ReadToEnd().Split("\n")[0]) / 1000.0d;
        }
        catch
        {
            return 0.0d;
        }
    }

    /// <summary>
    /// Return Linux memory metrics
    /// </summary>
    /// <returns></returns>
    private static MemoryMetrics GetLinuxMemoryMetrics()
    {
        using var process = Process.Start(new ProcessStartInfo("free") { Arguments = "-m", RedirectStandardOutput = true });
        process!.WaitForExit();
        var lines = process.StandardOutput.ReadToEnd().Split("\n");
        var memory = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return new()
        {
            Total = Convert.ToInt32(memory[1]),
            Used = Convert.ToInt32(memory[2]),
            Free = Convert.ToInt32(memory[3])
        };
    }

    /// <summary>
    /// Return Linux disk metrics
    /// </summary>
    /// <returns></returns>
    private static DiskMetrics GetLinuxDiskMetrics()
    {
        using var process = Process.Start(new ProcessStartInfo("df") { Arguments = "-BM --output=size,used,avail .", RedirectStandardOutput = true });
        process!.WaitForExit();
        var lines = process.StandardOutput.ReadToEnd().Split("\n");
        var disk = lines[1].Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return new()
        {
            Total = Convert.ToInt32(disk[0][..^1]),
            Used = Convert.ToInt32(disk[1][..^1]),
            Free = Convert.ToInt32(disk[2][..^1])
        };
    }
    #endregion

}