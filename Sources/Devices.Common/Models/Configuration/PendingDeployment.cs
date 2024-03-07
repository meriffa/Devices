using Devices.Common.Models.Identification;

namespace Devices.Common.Models.Configuration;

/// <summary>
/// Pending deployment
/// </summary>
public class PendingDeployment
{

    #region Properties
    /// <summary>
    /// Pending deployment device
    /// </summary>
    public required Device Device { get; set; }

    /// <summary>
    /// Pending deployment release
    /// </summary>
    public required Release Release { get; set; }
    #endregion

}