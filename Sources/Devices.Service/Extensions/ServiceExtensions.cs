using Devices.Service.Interfaces.Identification;
using Devices.Service.Interfaces.Metrics;
using Devices.Service.Options;
using Devices.Service.Services.Identification;
using Devices.Service.Services.Metrics;
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
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<IMonitoringService, MonitoringService>();
        return services;
    }
    #endregion

}