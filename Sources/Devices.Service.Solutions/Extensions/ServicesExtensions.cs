using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
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
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddServicesSolutions(this IServiceCollection services, IConfiguration configuration)
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
        builder.AddPolicy("GardenPolicy", policy => policy.RequireClaim(ClaimTypes.Role, ["Administrator", "User"]));
        return builder;
    }

    /// <summary>
    /// Add area authorizations
    /// </summary>
    /// <param name="options"></param>
    public static void AuthorizeAreasSolutions(this RazorPagesOptions options)
    {
        options.Conventions.AuthorizeAreaFolder("Garden", "/", "GardenPolicy");
    }
    #endregion

}