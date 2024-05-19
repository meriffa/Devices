using Devices.Service.Interfaces.Identification;
using Devices.Service.Solutions.Garden.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    /// <param name="recipient"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendPumpRequest(string recipient, int pumpIndex, bool pumpState, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).PumpRequest(Context.UserIdentifier!, pumpIndex, pumpState);
    }

    /// <summary>
    /// Send pump response
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="pumpIndex"></param>
    /// <param name="pumpState"></param>
    /// <param name="error"></param>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendPumpResponse(string recipient, int pumpIndex, bool pumpState, string? error)
    {
        await Clients.User(recipient).PumpResponse(pumpIndex, pumpState, error);
    }

    /// <summary>
    /// Send presence confirmation request
    /// </summary>
    /// <param name="recipient"></param>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendPresenceConfirmationRequest(string recipient)
    {
        await Clients.User(recipient).PresenceConfirmationRequest(Context.UserIdentifier!);
    }

    /// <summary>
    /// Send presence confirmation response
    /// </summary>
    /// <param name="recipient"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendPresenceConfirmationResponse(string recipient)
    {
        await Clients.User(recipient).PresenceConfirmationResponse();
    }

    /// <summary>
    /// Send shutdown request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendShutdownRequest(string recipient, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).ShutdownRequest(Context.UserIdentifier!);
    }

    /// <summary>
    /// Send shutdown response
    /// </summary>
    /// <param name="recipient"></param>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendShutdownResponse(string recipient)
    {
        await Clients.User(recipient).ShutdownResponse();
    }
    #endregion

}