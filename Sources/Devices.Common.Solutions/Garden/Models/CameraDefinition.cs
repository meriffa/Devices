namespace Devices.Common.Solutions.Garden.Models;

/// <summary>
/// Camera definition
/// </summary>
public class CameraDefinition
{

    #region Properties
    /// <summary>
    /// Camera definition source
    /// </summary>
    public required string Source { get; set; }

    /// <summary>
    /// Camera definition width
    /// </summary>
    public required int Width { get; set; }

    /// <summary>
    /// Camera definition height
    /// </summary>
    public required int Height { get; set; }

    /// <summary>
    /// Camera definition frames per second
    /// </summary>
    public required int FramesPerSecond { get; set; }

    /// <summary>
    /// Camera definition bitrate
    /// </summary>
    public required int Bitrate { get; set; }

    /// <summary>
    /// Camera definition publish location
    /// </summary>
    public required string PublishLocation { get; set; }

    /// <summary>
    /// Camera definition view location
    /// </summary>
    public required string ViewLocation { get; set; }
    #endregion

}