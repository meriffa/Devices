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
    void HandlePumpRequest(Action<string, int, bool> action);

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    /// <param name="sender"></param>
    void SendPresenceConfirmationRequest(string sender);

    /// <summary>
    /// Handle presence confirmation response
    /// </summary>
    /// <param name="action"></param>
    void HandlePresenceConfirmationResponse(Action action);

    /// <summary>
    /// Handle shutdown request
    /// </summary>
    /// <param name="action"></param>
    void HandleShutdownRequest(Action<string> action);

    /// <summary>
    /// Send shutdown response
    /// </summary>
    /// <param name="sender"></param>
    void SendShutdownResponse(string sender);
    #endregion

}