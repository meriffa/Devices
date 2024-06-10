using CommandLine;
using Devices.Client.Solutions.Controllers;
using Devices.Common.Extensions;
using Devices.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;

namespace Devices.Client.Solutions;

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
            DisplayService.WriteTitle();
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
            .ConfigreHost(args)
            .ConfigureServices((context, services) =>
            {
                services.AddDeviceServices(context.Configuration);
                services.AddSingleton<Garden.Interfaces.IGardenService, Garden.Services.GardenService>();
                services.AddSingleton<Garden.Interfaces.IWateringHub, Garden.Hubs.WateringHub>();
                services.AddSingleton<Garden.Interfaces.ICameraHub, Garden.Hubs.CameraHub>();
            })
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
            .Build();
    }
    #endregion

}