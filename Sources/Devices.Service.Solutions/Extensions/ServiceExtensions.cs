using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Service.Solutions.Extensions;

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
    public static IServiceCollection AddSolutionServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<Garden.Interfaces.IGardenService, Garden.Services.GardenService>();
        return services;
    }
    #endregion

}