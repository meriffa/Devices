namespace Devices.Common.Models.Monitoring;

/// <summary>
/// CPU metrics
/// </summary>
public class CpuMetrics
{

    #region Properties
    /// <summary>
    /// CPU metrics user time [%]
    /// </summary>
    public required double User { get; set; }

    /// <summary>
    /// CPU metrics system time [%]
    /// </summary>
    public required double System { get; set; }

    /// <summary>
    /// CPU metrics idle time [%]
    /// </summary>
    public required double Idle { get; set; }

    /// <summary>
    /// CPU metrics temperature [C]
    /// </summary>
    public required double Temperature { get; set; }
    #endregion

}