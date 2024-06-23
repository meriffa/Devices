namespace Devices.Service.Models.Monitoring;

/// <summary>
/// Outage
/// </summary>
public class Outage
{

    #region Properties
    /// <summary>
    /// Outage from
    /// </summary>
    public required DateTime From { get; set; }

    /// <summary>
    /// Outage to
    /// </summary>
    public required DateTime To { get; set; }

    /// <summary>
    /// Outage duration
    /// </summary>
    public required TimeSpan Duration { get; set; }
    #endregion

}