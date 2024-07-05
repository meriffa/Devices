using Devices.Common.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Devices.Common.Extensions;

/// <summary>
/// Device client extension methods
/// </summary>
public static class DeviceClientExtensions
{

    #region Public Methods
    /// <summary>
    /// Configure device host
    /// </summary>
    /// <param name="host"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder ConfigreDeviceHost(this IHostBuilder host, string[] args)
    {
        return host
            .ConfigureHostConfiguration((configuration) =>
            {
                configuration.AddEnvironmentVariables();
            })
            .ConfigureAppConfiguration((context, configuration) =>
            {
                configuration.SetBasePath(AppDomain.CurrentDomain.BaseDirectory);
                configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                configuration.AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: true);
                configuration.AddEnvironmentVariables();
                configuration.AddCommandLine(args);
            });
    }

    /// <summary>
    /// Register device services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IServiceCollection AddDeviceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ClientOptions>(configuration.GetRequiredSection(nameof(ClientOptions)));
        services.AddSingleton<Services.DisplayService>();
        services.AddSingleton<Interfaces.Identification.IFingerprintService, Services.Identification.FingerprintServiceHost>();
        services.AddSingleton<Interfaces.Identification.IFingerprintService, Services.Identification.FingerprintServiceNetworkInterface>();
        services.AddSingleton<Interfaces.Identification.IIdentityService, Services.Identification.IdentityService>();
        return services;
    }
    #endregion

}