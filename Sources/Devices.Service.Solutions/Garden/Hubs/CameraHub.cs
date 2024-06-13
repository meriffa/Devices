using Devices.Service.Interfaces.Identification;
using Devices.Service.Solutions.Garden.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Service.Solutions.Garden.Hubs;

/// <summary>
/// Camera hub
/// </summary>
[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme}, {CookieAuthenticationDefaults.AuthenticationScheme}")]
public class CameraHub : HubBase<ICameraHub>
{

    #region Public Methods
    /// <summary>
    /// Send device presence confirmation request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenCameraPolicy")]
    public override async Task SendDevicePresenceConfirmationRequest(string recipient, [FromServices] IIdentityService identityService)
    {
        await base.SendDevicePresenceConfirmationRequest(recipient, identityService);
    }

    /// <summary>
    /// Send shutdown request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenCameraPolicy")]
    public override async Task SendShutdownRequest(string recipient, [FromServices] IIdentityService identityService)
    {
        await base.SendShutdownRequest(recipient, identityService);
    }

    /// <summary>
    /// Send pan request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="value"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenCameraPolicy")]
    public async Task SendPanRequest(string recipient, int value, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).PanRequest(Context.UserIdentifier!, value);
    }

    /// <summary>
    /// Send tilt request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="value"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenCameraPolicy")]
    public async Task SendTiltRequest(string recipient, int value, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).TiltRequest(Context.UserIdentifier!, value);
    }

    /// <summary>
    /// Send focus request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="value"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenCameraPolicy")]
    public async Task SendFocusRequest(string recipient, double value, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).FocusRequest(Context.UserIdentifier!, value);
    }

    /// <summary>
    /// Send zoom request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="value"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenCameraPolicy")]
    public async Task SendZoomRequest(string recipient, double value, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).ZoomRequest(Context.UserIdentifier!, value);
    }
    #endregion

}