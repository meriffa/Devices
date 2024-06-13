using Devices.Service.Solutions.Garden.Hubs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Devices.Service.Solutions.Extensions;

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
    /// <returns></returns>
    public static IServiceCollection AddServicesSolutions(this IServiceCollection services)
    {
        services.AddScoped<Garden.Interfaces.IGardenService, Garden.Services.GardenService>();
        return services;
    }

    /// <summary>
    /// Add authorization policies
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AuthorizationBuilder AddPoliciesSolutions(this AuthorizationBuilder builder)
    {
        return builder
            .AddPolicy("GardenPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Role, ["Administrator", "UserCamera", "User"]);
            })
            .AddPolicy("GardenCameraPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Role, ["Administrator", "UserCamera"]);
            });
    }

    /// <summary>
    /// Add area authorizations
    /// </summary>
    /// <param name="options"></param>
    public static void AuthorizeAreasSolutions(this RazorPagesOptions options)
    {
        options.Conventions.AuthorizeAreaFolder("Garden", "/", "GardenPolicy");
        options.Conventions.AuthorizeAreaPage("Garden", "/Camera", "GardenCameraPolicy");
    }

    /// <summary>
    /// Register SignalR hubs
    /// </summary>
    /// <param name="application"></param>
    public static void MapSignalRHubsSolutions(this WebApplication application)
    {
        application.MapHub<WateringHub>("/Hub/Solutions/Watering");
        application.MapHub<CameraHub>("/Hub/Solutions/Camera");
    }
    #endregion

}