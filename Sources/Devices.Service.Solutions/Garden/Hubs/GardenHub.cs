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
    /// <param name="pumpId"></param>
    /// <param name="pumpState"></param>
    /// <returns></returns>
    public async Task SendPumpRequest(int deviceId, int pumpId, bool pumpState)
    {
        await Clients.All.SendAsync("PumpRequest", deviceId, pumpId, pumpState);
    }

    /// <summary>
    /// Send pump response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="pumpId"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    public async Task SendPumpResponse(int deviceId, int pumpId, bool pumpState, string? error)
    {
        await Clients.All.SendAsync("PumpResponse", deviceId, pumpId, pumpState, error);
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
    #endregion

}