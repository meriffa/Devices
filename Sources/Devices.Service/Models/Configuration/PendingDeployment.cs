using Devices.Common.Models.Configuration;
using Devices.Service.Models.Identification;

namespace Devices.Service.Models.Configuration;

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