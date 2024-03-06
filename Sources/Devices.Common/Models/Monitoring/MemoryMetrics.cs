namespace Devices.Common.Models.Monitoring;

/// <summary>
/// Memory metrics
/// </summary>
public class MemoryMetrics
{

    #region Properties
    /// <summary>
    /// Memory metrics total memory [MB]
    /// </summary>
    public required int Total { get; set; }

    /// <summary>
    /// Memory metrics used memory [MB]
    /// </summary>
    public required int Used { get; set; }

    /// <summary>
    /// Memory metrics free memory [MB]
    /// </summary>
    public required int Free { get; set; }
    #endregion

}