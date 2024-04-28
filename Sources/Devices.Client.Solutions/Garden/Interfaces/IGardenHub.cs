namespace Devices.Client.Solutions.Garden.Interfaces;

/// <summary>
/// Garden hub interface
/// </summary>
public interface IGardenHub
{

    #region Public Methods
    /// <summary>
    /// Start hub client
    /// </summary>
    /// <returns></returns>
    Task<bool> Start();

    /// <summary>
    /// Stop hub client
    /// </summary>
    /// <returns></returns>
    Task<bool> Stop();

    /// <summary>
    /// Handle pump request
    /// </summary>
    /// <param name="action"></param>
    void HandlePumpRequest(Action<int, int, bool> action);

    /// <summary>
    /// Handle shutdown request
    /// </summary>
    /// <param name="action"></param>
    void HandleShutdownRequest(Action<int> action);
    #endregion

}