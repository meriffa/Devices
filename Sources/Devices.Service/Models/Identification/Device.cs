namespace Devices.Service.Models.Identification;

/// <summary>
/// Device
/// </summary>
public class Device
{

    #region Properties
    /// <summary>
    /// Device id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Device name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Device location
    /// </summary>
    public required string Location { get; set; }
    #endregion

}