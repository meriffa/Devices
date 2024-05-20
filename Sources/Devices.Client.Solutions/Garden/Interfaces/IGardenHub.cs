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
    void Start();

    /// <summary>
    /// Stop hub client
    /// </summary>
    void Stop();

    /// <summary>
    /// Handle device presence confirmation request
    /// </summary>
    void HandleDevicePresenceConfirmationRequest();

    /// <summary>
    /// Handle pump request
    /// </summary>
    /// <param name="action"></param>
    void HandlePumpRequest(Action<int, bool> action);

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    void SendPresenceConfirmationRequest();

    /// <summary>
    /// Handle presence confirmation response
    /// </summary>
    /// <param name="action"></param>
    void HandlePresenceConfirmationResponse(Action action);

    /// <summary>
    /// Handle shutdown request
    /// </summary>
    /// <param name="action"></param>
    void HandleShutdownRequest(Action action);

    /// <summary>
    /// Send shutdown response
    /// </summary>
    void SendShutdownResponse();
    #endregion

}