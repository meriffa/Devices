namespace Devices.Client.Solutions.Garden.Interfaces;

/// <summary>
/// Watering hub interface
/// </summary>
public interface IWateringHub : IHubBase
{

    #region Public Methods
    /// <summary>
    /// Handle pump request
    /// </summary>
    /// <param name="action"></param>
    void HandlePumpRequest(Action<int, bool> action);

    /// <summary>
    /// Send operator presence confirmation request
    /// </summary>
    void SendOperatorPresenceConfirmationRequest();

    /// <summary>
    /// Handle operator presence confirmation response
    /// </summary>
    /// <param name="action"></param>
    void HandleOperatorPresenceConfirmationResponse(Action action);
    #endregion

}