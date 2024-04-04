namespace Devices.Common.Solutions.Garden.Models;

/// <summary>
/// Camera notification
/// </summary>
public class CameraNotification
{

    #region Properties
    /// <summary>
    /// Camera notification device date & time
    /// </summary>
    public required DateTime DeviceDate { get; set; }

    /// <summary>
    /// Camera notification face count
    /// </summary>
    public required int FaceCount { get; set; }

    /// <summary>
    /// Camera notification motion region count
    /// </summary>
    public required int MotionRegionCount { get; set; }

    /// <summary>
    /// Camera notification video filename
    /// </summary>
    public required string VideoFileName { get; set; }
    #endregion

}