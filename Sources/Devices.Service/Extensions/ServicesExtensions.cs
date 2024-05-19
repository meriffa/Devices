using Devices.Service.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

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
    /// <returns></returns>
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<Interfaces.Security.ISecurityService, Services.Security.SecurityService>();
        services.AddScoped<Services.Security.WebAuthenticationService>();
        services.AddScoped<Interfaces.Identification.IIdentityService, Services.Identification.IdentityService>();
        services.AddScoped<Interfaces.Monitoring.IMonitoringService, Services.Monitoring.MonitoringService>();
        services.AddScoped<Interfaces.Configuration.IConfigurationService, Services.Configuration.ConfigurationService>();
        return services;
    }

    /// <summary>
    /// Register data protection, authentication & authorization
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceOptions"></param>
    /// <returns></returns>
    public static AuthorizationBuilder AddSecurity(this IServiceCollection services, ServiceOptions serviceOptions)
    {
        services.AddDataProtection()
            .PersistKeysToFileSystem(new(serviceOptions.DataProtectionFolder));
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromDays(90);
                options.SlidingExpiration = true;
                options.LoginPath = "/SignIn";
                options.LogoutPath = "/Service/Security/SignOut";
                options.AccessDeniedPath = "/AccessDenied";
                options.EventsType = typeof(Services.Security.WebAuthenticationService);
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents() { OnTokenValidated = Services.Security.DeviceTokenValidationService.OnTokenValidated };
                options.SaveToken = false;
                options.TokenValidationParameters = new()
                {
                    ValidAlgorithms = ["HS256"],
                    ValidIssuer = serviceOptions.JwtBearer.Issuer,
                    ValidateIssuer = true,
                    ValidAudience = serviceOptions.JwtBearer.Audience,
                    ValidateAudience = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(serviceOptions.JwtBearer.SigningKey)),
                    ValidateIssuerSigningKey = true
                };
            });
        var builder = services.AddAuthorizationBuilder()
            .SetDefaultPolicy(new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme, JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());
        return builder;
    }

    /// <summary>
    /// Add authorization policies
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="serviceOptions"></param>
    /// <returns></returns>
    public static AuthorizationBuilder AddPolicies(this AuthorizationBuilder builder, ServiceOptions serviceOptions)
    {
        return builder
            .AddPolicy("FrameworkPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Role, ["Administrator"]);
            })
            .AddPolicy("DevicePolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                policy.RequireClaim(ClaimTypes.Role, ["Device"]);
            })
            .AddPolicy("WebPolicy", policy =>
            {
                policy.AuthenticationSchemes.Add(CookieAuthenticationDefaults.AuthenticationScheme);
                policy.RequireAuthenticatedUser();
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