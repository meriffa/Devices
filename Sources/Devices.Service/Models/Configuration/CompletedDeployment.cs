using Devices.Common.Models.Configuration;
using Devices.Service.Models.Identification;

namespace Devices.Service.Models.Configuration;

/// <summary>
/// Completed deployment
/// </summary>
public class CompletedDeployment : Deployment
{

    #region Properties
    /// <summary>
    /// Completed deployment id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Completed deployment device
    /// </summary>
    public required Device Device { get; set; }
    #endregion

}