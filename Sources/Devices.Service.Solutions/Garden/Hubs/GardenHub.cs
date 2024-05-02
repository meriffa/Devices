using Microsoft.AspNetCore.SignalR;

namespace Devices.Service.Solutions.Garden.Hubs;

/// <summary>
/// Garden hub
/// </summary>
public class GardenHub : Hub
{

    #region Public Methods
    /// <summary>
    /// Send pump request
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <returns></returns>
    public async Task SendPumpRequest(int deviceId, int pumpIndex, bool pumpState)
    {
        await Clients.All.SendAsync("PumpRequest", deviceId, pumpIndex, pumpState);
    }

    /// <summary>
    /// Send pump response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public async Task SendPumpResponse(int deviceId, int pumpIndex, bool pumpState, string? error)
    {
        await Clients.All.SendAsync("PumpResponse", deviceId, pumpIndex, pumpState, error);
    }

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    /// <returns></returns>
    public async Task SendPresenceConfirmationRequest()
    {
        await Clients.All.SendAsync("PresenceConfirmationRequest");
    }

    /// <summary>
    /// Send presence confirmation response
    /// </summary>
    /// <returns></returns>
    public async Task SendPresenceConfirmationResponse()
    {
        await Clients.All.SendAsync("PresenceConfirmationResponse");
    }

    /// <summary>
    /// Send shutdown request
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public async Task SendShutdownRequest(int deviceId)
    {
        await Clients.All.SendAsync("ShutdownRequest", deviceId);
    }

    /// <summary>
    /// Send shutdown response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public async Task SendShutdownResponse(int deviceId)
    {
        await Clients.All.SendAsync("ShutdownResponse", deviceId);
    }
    #endregion

}