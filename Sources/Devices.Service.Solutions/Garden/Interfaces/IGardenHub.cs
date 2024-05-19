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
    /// <param name="deviceId"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <returns></returns>
    public Task PumpRequest(int deviceId, int pumpIndex, bool pumpState);

    /// <summary>
    /// Pump response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public Task PumpResponse(int deviceId, int pumpIndex, bool pumpState, string? error);

    /// <summary>
    /// Presence confirmation request
    /// </summary>
    /// <returns></returns>
    public Task PresenceConfirmationRequest();

    /// <summary>
    /// Presence confirmation response
    /// </summary>
    /// <returns></returns>
    public Task PresenceConfirmationResponse();

    /// <summary>
    /// Shutdown request
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public Task ShutdownRequest(int deviceId);

    /// <summary>
    /// Shutdown response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public Task ShutdownResponse(int deviceId);
    #endregion

}