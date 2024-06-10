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
    /// Send pan request
    /// </summary>
    /// <param name="recipient"></param>
    /// <param name="value"></param>
    /// <param name="identityService"></param>
    /// <returns></returns>
    [Authorize(Policy = "GardenPolicy")]
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
    [Authorize(Policy = "GardenPolicy")]
    public async Task SendTiltRequest(string recipient, int value, [FromServices] IIdentityService identityService)
    {
        await Clients.User(identityService.GetDeviceToken(Convert.ToInt32(recipient))).TiltRequest(Context.UserIdentifier!, value);
    }
    #endregion

}