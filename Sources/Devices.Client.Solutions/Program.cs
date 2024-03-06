using CommandLine;
using Devices.Client.Solutions.Controllers;
using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Client.Solutions.Garden.Services;
using Devices.Common.Options;
using Devices.Common.Services;
using Microsoft.Extensions.Configuration;
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
            DisplayService.WriteText("****************************");
            DisplayService.WriteText("* Devices.Client.Solutions *");
            DisplayService.WriteText("****************************");
            DisplayService.WriteText();
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
            })
            .ConfigureServices((context, services) =>
            {
                services.AddOptions<ClientOptions>().Bind(context.Configuration.GetRequiredSection(nameof(ClientOptions)));
                services.AddSingleton<DisplayService>();
                services.AddSingleton<IGardenService, GardenService>();
            })
            .UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration))
            .Build();
    }
    #endregion

}