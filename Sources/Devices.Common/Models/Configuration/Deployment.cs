namespace Devices.Common.Models.Configuration;

/// <summary>
/// Deployment
/// </summary>
public class Deployment : PendingDeployment
{

    #region Properties
    /// <summary>
    /// Deployment id
    /// </summary>
    public required int Id { get; set; }

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