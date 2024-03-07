using Devices.Common.Models.Identification;

namespace Devices.Common.Models.Configuration;

/// <summary>
/// Deployment
/// </summary>
public class Deployment
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
    /// Deployment device
    /// </summary>
    public required Identity Device { get; set; }

    /// <summary>
    /// Deployment release
    /// </summary>
    public required Release Release { get; set; }

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