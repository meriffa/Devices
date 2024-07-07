using Devices.Service.Models.Identification;
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
            var deviceToken = context.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (deviceToken != null)
            {
                var device = service.GetDevice(deviceToken);
                if (device.Enabled)
                    context.Principal.AddIdentity(new ClaimsIdentity(new List<Claim>() { new(ClaimTypes.NameIdentifier, device.Id.ToString()) }.Concat(GetRoles(device)), JwtBearerDefaults.AuthenticationScheme));
            }
        }
        return Task.CompletedTask;
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return device roles
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    private static IEnumerable<Claim> GetRoles(Device device) => device.Roles.Select(i => new Claim(ClaimTypes.Role, i));
    #endregion

}