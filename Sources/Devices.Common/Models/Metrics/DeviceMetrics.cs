namespace Devices.Common.Models.Metrics;

/// <summary>
/// Device metrics
/// </summary>
public class DeviceMetrics
{

    #region Properties
    /// <summary>
    /// Device metrics date & time
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Device metrics las reboot date & time
    /// </summary>
    public required DateTime LastRebootDate { get; set; }

    /// <summary>
    /// Device CPU metrics
    /// </summary>
    public required CpuMetrics Cpu { get; set; }

    /// <summary>
    /// Device memory metrics
    /// </summary>
    public required MemoryMetrics Memory { get; set; }
    #endregion

}