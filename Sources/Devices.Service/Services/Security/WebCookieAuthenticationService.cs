using Devices.Service.Extensions;
using Devices.Service.Interfaces.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Devices.Service.Services.Security;

/// <summary>
/// Web cookie authentication events
/// </summary>
/// <param name="options"></param>
public class WebCookieAuthenticationService(ISecurityService service) : CookieAuthenticationEvents
{

    #region Private Fields
    private readonly ISecurityService service = service;
    #endregion

    #region Public Methods
    /// <summary>
    /// Validate principal
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        var userId = context.Principal?.GetId();
        if (userId == null || !service.IsUserEnabled(userId.Value))
        {
            context.RejectPrincipal();
            await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
    #endregion

}