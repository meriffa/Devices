namespace Devices.Common.Models.Configuration;

/// <summary>
/// Release node
/// </summary>
public class ReleaseNode
{

    #region Properties
    /// <summary>
    /// Release node release
    /// </summary>
    public required Release Release { get; set; }

    /// Release node upstream nodes
    /// </summary>
    public required List<ReleaseNode> UpstreamNodes { get; set; }

    /// <summary>
    /// Release node downstream nodes
    /// </summary>
    public required List<ReleaseNode> DownstreamNodes { get; set; }

    /// <summary>
    /// Release node success flag
    /// </summary>
    public bool Success { get; set; }
    #endregion

}