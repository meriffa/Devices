using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Devices.Service.Services.Security;

/// <summary>
/// Device token validation service
/// </summary>
public static class DeviceTokenValidationService
{

    #region Public Methods
    /// <summary>
    /// On token validated event
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static Task OnTokenValidated(TokenValidatedContext context)
    {
        if (context.Principal != null)
        {
            var service = context.HttpContext.RequestServices.GetRequiredService<Interfaces.Identification.IIdentityService>();
            if (context.Principal.FindFirstValue(ClaimTypes.NameIdentifier) is string deviceToken && service.GetDeviceId(deviceToken) is int deviceId)
                context.Principal.AddIdentity(new ClaimsIdentity([new(ClaimTypes.NameIdentifier, deviceId.ToString()), new(ClaimTypes.Role, "Device")], JwtBearerDefaults.AuthenticationScheme));
        }
        return Task.CompletedTask;
    }
    #endregion

}