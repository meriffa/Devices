namespace Devices.Common.Solutions.Garden.Models;

/// <summary>
/// Camera state
/// </summary>
public class CameraState
{

    #region Properties
    /// <summary>
    /// Camera state pan (0 ... 180)
    /// </summary>
    public required int Pan { get; set; }

    /// <summary>
    /// Camera state tilt (0 ... 180)
    /// </summary>
    public required int Tilt { get; set; }
    #endregion

}