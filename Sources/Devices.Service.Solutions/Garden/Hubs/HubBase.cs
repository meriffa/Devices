using Devices.Service.Interfaces.Identification;
using Devices.Service.Solutions.Garden.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Devices.Service.Solutions.Garden.Hubs;

/// <summary>
/// Base hub
/// </summary>
public abstract class HubBase<T> : Hub<T> where T : class, IHubBase
{

    #region Public Methods
    /// <summary>
    /// Send device presence confirmation request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendDevicePresenceConfirmationRequest(string recipient, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).DevicePresenceConfirmationRequest(Context.UserIdentifier!);
    }

    /// <summary>
    /// Send device presence confirmation response
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="state"></param>
    /// <returns></returns>
    [Authorize(Policy = "DevicePolicy")]
    public async Task SendDevicePresenceConfirmationResponse(string recipient, object state)
    {
        await Clients.User(recipient).DevicePresenceConfirmationResponse(state);
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