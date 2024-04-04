namespace Devices.Common.Models.Monitoring;

/// <summary>
/// Disk metrics
/// </summary>
public class DiskMetrics
{

    #region Properties
    /// <summary>
    /// Disk metrics total [MB]
    /// </summary>
    public required int Total { get; set; }

    /// <summary>
    /// Disk metrics used [MB]
    /// </summary>
    public required int Used { get; set; }

    /// <summary>
    /// Disk metrics free [MB]
    /// </summary>
    public required int Free { get; set; }
    #endregion

}