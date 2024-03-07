namespace Devices.Common.Models.Identification;

/// <summary>
/// Device
/// </summary>
public class Device
{

    #region Properties
    /// <summary>
    /// Device id
    /// </summary>
    public required string Id { get; set; }

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