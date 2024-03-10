using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Devices.Service.Extensions;

/// <summary>
/// Application extension methods
/// </summary>
public static class ApplicationExtensions
{

    #region Public Methods
    /// <summary>
    /// Adds web authentication & authorization
    /// </summary>
    /// <param name="application"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseSecurity(this IApplicationBuilder application)
    {
        application.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Strict, Secure = CookieSecurePolicy.Always });
        application.UseAuthentication();
        application.UseAuthorization();
        return application;
    }
    #endregion

}