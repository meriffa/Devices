namespace Devices.Common.Models.Monitoring;

/// <summary>
/// Device metrics
/// </summary>
public class DeviceMetrics
{

    #region Properties
    /// <summary>
    /// Device metrics device date & time
    /// </summary>
    public required DateTime DeviceDate { get; set; }

    /// <summary>
    /// Device metrics last reboot date & time
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