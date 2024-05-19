using Devices.Service.Solutions.Garden.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Devices.Service.Solutions.Garden.Hubs;

/// <summary>
/// Garden hub
/// </summary>
[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}, {CookieAuthenticationDefaults.AuthenticationScheme}")]
public class GardenHub : Hub<IGardenHub>
{

    #region Public Methods
    /// <summary>
    /// Send pump request
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendPumpRequest(int deviceId, int pumpIndex, bool pumpState)
    {
        await Clients.All.PumpRequest(deviceId, pumpIndex, pumpState);
    }

    /// <summary>
    /// Send pump response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendPumpResponse(int deviceId, int pumpIndex, bool pumpState, string? error)
    {
        await Clients.All.PumpResponse(deviceId, pumpIndex, pumpState, error);
    }

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendPresenceConfirmationRequest()
    {
        await Clients.All.PresenceConfirmationRequest();
    }

    /// <summary>
    /// Send presence confirmation response
    /// </summary>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendPresenceConfirmationResponse()
    {
        await Clients.All.PresenceConfirmationResponse();
    }

    /// <summary>
    /// Send shutdown request
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendShutdownRequest(int deviceId)
    {
        await Clients.All.ShutdownRequest(deviceId);
    }

    /// <summary>
    /// Send shutdown response
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendShutdownResponse(int deviceId)
    {
        await Clients.All.ShutdownResponse(deviceId);
    }
    #endregion

}