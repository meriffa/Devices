using Devices.Service.Interfaces.Identification;
using Devices.Service.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Devices.Service.Services.Security;

/// <summary>
/// Device authentication service
/// </summary>
/// <param name="options"></param>
/// <param name="logger"></param>
/// <param name="encoder"></param>
public class DeviceAuthenticationService(IIdentityService identityService, IOptionsMonitor<DeviceAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder) : AuthenticationHandler<DeviceAuthenticationOptions>(options, logger, encoder)
{

    #region Constants
    public const string AuthenticationScheme = "Devices";
    #endregion

    #region Private Fields
    private readonly IIdentityService identityService = identityService;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Handle device authentication
    /// </summary>
    /// <returns></returns>
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Headers.TryGetValue(Options.HeaderName, out var headerValue) && !string.IsNullOrEmpty(headerValue))
        {
            if (identityService.IsDeviceEnabled(headerValue!))
            {
                var claims = new List<Claim>()
                {
                    new(ClaimTypes.NameIdentifier, headerValue!),
                    new(ClaimTypes.Role, "Device")
                };
                return await Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new(new ClaimsIdentity(claims, Scheme.Name)), Scheme.Name)));
            }
            return await Task.FromResult(AuthenticateResult.Fail($"Invalid device id '{headerValue}' specified."));
        }
        return await Task.FromResult(AuthenticateResult.Fail("Device header not specified."));
    }
    #endregion

}