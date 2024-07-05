using CommandLine;
using Devices.Client.Controllers;
using Devices.Common.Extensions;
using Devices.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

namespace Devices.Client;

/// <summary>
/// Application class
/// </summary>
class Program
{

    #region Initialization
    /// <summary>
    /// Application entry
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    static int Main(string[] args)
    {
        var host = CreateApplicationHost(args);
        try
        {
            Parser.Default
                .ParseArguments(args, Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(i => i.GetCustomAttributes(typeof(VerbAttribute), true).Length > 0)
                .ToArray())
                .WithParsed<Controller>(i => i.Execute(host.Services));
            return 0;
        }
        catch (Exception ex)
        {
            host.Services.GetRequiredService<DisplayService>().WriteError(ex);
            return -1;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Create application host
    /// </summary>
    /// <param name="args"></param>
    private static IHost CreateApplicationHost(string[] args)
    {
        return Host.CreateDefaultBuilder()
            .ConfigreDeviceHost(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDeviceServices(context.Configuration);
                services.AddSingleton<Common.Interfaces.Monitoring.IDeviceMetricsService, Common.Services.Monitoring.DeviceMetricsService>();
                services.AddSingleton<Common.Interfaces.Monitoring.IMonitoringService, Common.Services.Monitoring.MonitoringService>();
                services.AddSingleton<Common.Interfaces.Configuration.IConfigurationService, Common.Services.Configuration.ConfigurationService>();
                services.AddSingleton<Common.Services.Configuration.ReleaseGraphService>();
            })
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
            .Build();
    }
    #endregion

}