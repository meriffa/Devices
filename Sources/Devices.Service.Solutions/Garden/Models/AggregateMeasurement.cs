namespace Devices.Service.Solutions.Garden.Models;

/// <summary>
/// Aggregate measurement
/// </summary>
public class AggregateMeasurement
{

    #region Properties
    /// <summary>
    /// Aggregate measurement minimum
    /// </summary>
    public required double Minimum { get; set; }

    /// <summary>
    /// Aggregate measurement maximum
    /// </summary>
    public required double Maximum { get; set; }

    /// <summary>
    /// Aggregate measurement average
    /// </summary>
    public required double Average { get; set; }
    #endregion

}