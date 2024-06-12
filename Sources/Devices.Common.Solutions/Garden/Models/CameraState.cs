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

    /// <summary>
    /// Camera state focus minimum
    /// </summary>
    public required double FocusMinimum { get; set; }

    /// <summary>
    /// Camera state focus maximum
    /// </summary>
    public required double FocusMaximum { get; set; }

    /// <summary>
    /// Camera state focus
    /// </summary>
    public required double Focus { get; set; }

    /// <summary>
    /// Camera state zoom (1 ... 10)
    /// </summary>
    public required double Zoom { get; set; }
    #endregion

}