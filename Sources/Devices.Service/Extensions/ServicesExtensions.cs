using Devices.Service.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Devices.Service.Extensions;

/// <summary>
/// Services extension methods
/// </summary>
public static class ServicesExtensions
{

    #region Public Methods
    /// <summary>
    /// Register services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ServiceOptions>(configuration.GetSection(nameof(ServiceOptions)));
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<Interfaces.Security.ISecurityService, Services.Security.SecurityService>();
        services.AddScoped<Services.Security.WebAuthenticationService>();
        services.AddScoped<Interfaces.Identification.IIdentityService, Services.Identification.IdentityService>();
        services.AddScoped<Interfaces.Monitoring.IMonitoringService, Services.Monitoring.MonitoringService>();
        services.AddScoped<Interfaces.Configuration.IConfigurationService, Services.Configuration.ConfigurationService>();
        return services;
    }

    /// <summary>
    /// Register authentication & authorization
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static AuthorizationBuilder AddSecurity(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(90);
                options.SlidingExpiration = true;
                options.LoginPath = "/SignIn";
                options.LogoutPath = "/Services/Security/SignOut";
                options.AccessDeniedPath = "/AccessDenied";
                options.EventsType = typeof(Services.Security.WebAuthenticationService);
            })
            .AddScheme<DeviceAuthenticationOptions, Services.Security.DeviceAuthenticationService>(Services.Security.DeviceAuthenticationService.AuthenticationScheme, options => { });
        var builder = services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, Services.Security.DeviceAuthenticationService.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
        return builder;
    }

    /// <summary>
    /// Add authorization policies
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthorizationBuilder AddPolicies(this AuthorizationBuilder builder)
    {
        return builder
            .AddPolicy("WebPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
            })
            .AddPolicy("FrameworkPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Role, ["Administrator"]);
            })
            .AddPolicy("DevicePolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(Services.Security.DeviceAuthenticationService.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Role, ["Device"]);
            });
    }

    /// <summary>
    /// Add area authorizations
    /// </summary>
    /// <param name="options"></param>
    public static void AuthorizeAreas(this RazorPagesOptions options)
    {
        options.Conventions.AuthorizeAreaFolder("Framework", "/", "FrameworkPolicy");
    }
    #endregion

}