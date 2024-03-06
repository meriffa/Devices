using Devices.Service.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Service.Extensions;

/// <summary>
/// Service extension methods
/// </summary>
public static class ServiceExtensions
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
        services.AddScoped<Interfaces.Identification.IIdentityService, Services.Identification.IdentityService>();
        services.AddScoped<Interfaces.Monitoring.IMonitoringService, Services.Monitoring.MonitoringService>();
        services.AddScoped<Interfaces.Configuration.IConfigurationService, Services.Configuration.ConfigurationService>();
        return services;
    }
    #endregion

}