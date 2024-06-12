namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Camera hub interface
/// </summary>
public interface ICameraHub : IHubBase
{

    #region Public Methods
    /// <summary>
    /// Pan request
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task PanRequest(string sender, int value);

    /// <summary>
    /// Tilt request
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task TiltRequest(string sender, int value);

    /// <summary>
    /// Focus request
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task FocusRequest(string sender, double value);

    /// <summary>
    /// Zoom request
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public Task ZoomRequest(string sender, double value);
    #endregion

}