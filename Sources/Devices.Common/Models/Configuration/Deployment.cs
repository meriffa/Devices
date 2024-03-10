namespace Devices.Common.Models.Configuration;

/// <summary>
/// Deployment
/// </summary>
public class Deployment
{

    #region Properties
    /// <summary>
    /// Deployment release
    /// </summary>
    public required Release Release { get; set; }

    /// <summary>
    /// Deployment date & time
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Deployment success flag
    /// </summary>
    public required bool Success { get; set; }

    /// <summary>
    /// Deployment details
    /// </summary>
    public string? Details { get; set; }
    #endregion

}