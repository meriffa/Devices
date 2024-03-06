using Devices.Common.Models.Identification;

namespace Devices.Service.Models.Identification;

/// <summary>
/// Device
/// </summary>
public class Device
{

    #region Properties
    /// <summary>
    /// Device identity
    /// </summary>
    public required Identity Identity { get; set; }

    /// <summary>
    /// Device name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Device active flag
    /// </summary>
    public required bool Active { get; set; }
    #endregion

}