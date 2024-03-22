namespace Devices.Service.Models.Identification;

/// <summary>
/// Device deployment
/// </summary>
public class DeviceDeployment
{

    #region Properties
    /// <summary>
    /// Device deployment application
    /// </summary>
    public required string Application { get; set; }

    /// <summary>
    /// Device deployment version
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Device deployment success flag
    /// </summary>
    public required bool Success { get; set; }
    #endregion

}