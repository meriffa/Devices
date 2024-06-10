namespace Devices.Client.Solutions.Garden.Interfaces;

/// <summary>
/// Base hub interface
/// </summary>
public interface IHubBase
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
    /// <param name="getDevicePresenceConfirmation"></param>
    void HandleDevicePresenceConfirmationRequest(Func<object?> getDevicePresenceConfirmation);

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