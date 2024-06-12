namespace Devices.Client.Solutions.Garden.Interfaces;

/// <summary>
/// Camera hub interface
/// </summary>
public interface ICameraHub : IHubBase
{

    #region Public Methods
    /// <summary>
    /// Handle pan request
    /// </summary>
    /// <param name="action"></param>
    void HandlePanRequest(Action<int> action);

    /// <summary>
    /// Handle tilt request
    /// </summary>
    /// <param name="action"></param>
    void HandleTiltRequest(Action<int> action);

    /// <summary>
    /// Handle focus request
    /// </summary>
    /// <param name="action"></param>
    void HandleFocusRequest(Action<double> action);

    /// <summary>
    /// Handle zoom request
    /// </summary>
    /// <param name="action"></param>
    void HandleZoomRequest(Action<double> action);
    #endregion

}