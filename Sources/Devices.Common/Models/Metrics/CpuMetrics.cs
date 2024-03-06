namespace Devices.Common.Models.Metrics;

/// <summary>
/// CPU metrics
/// </summary>
public class CpuMetrics
{

    #region Properties
    /// <summary>
    /// CPU metrics user time [%]
    /// </summary>
    public required float User { get; set; }

    /// <summary>
    /// CPU metrics system time [%]
    /// </summary>
    public required float System { get; set; }

    /// <summary>
    /// CPU metrics idle time [%]
    /// </summary>
    public required float Idle { get; set; }
    #endregion

}