namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Garden hub interface
/// </summary>
public interface IGardenHub
{

    #region Public Methods
    /// <summary>
    /// Pump request
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <returns></returns>
    public Task PumpRequest(string sender, int pumpIndex, bool pumpState);

    /// <summary>
    /// Pump response
    /// </summary>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public Task PumpResponse(int pumpIndex, bool pumpState, string? error);

    /// <summary>
    /// Presence confirmation request
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task PresenceConfirmationRequest(string sender);

    /// <summary>
    /// Presence confirmation response
    /// </summary>
    /// <returns></returns>
    public Task PresenceConfirmationResponse();

    /// <summary>
    /// Shutdown request
    /// </summary>
    /// <param name="sender"></param>
    /// <returns></returns>
    public Task ShutdownRequest(string sender);

    /// <summary>
    /// Shutdown response
    /// </summary>
    /// <returns></returns>
    public Task ShutdownResponse();
    #endregion

}