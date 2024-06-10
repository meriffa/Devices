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
    #endregion

}