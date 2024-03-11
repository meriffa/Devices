using Devices.Client.Interfaces.Configuration;
using Devices.Client.Interfaces.Monitoring;
using Devices.Common.Interfaces.Identification;
using Devices.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Client.Controllers;

/// <summary>
///  Base controller
/// </summary>
public abstract class Controller
{

    #region Private Fields
    private IServiceProvider services = null!;
    private DisplayService? displayService;
    private IIdentityService? identityService;
    private IMonitoringService? monitoringService;
    private IConfigurationService? configurationService;
    #endregion

    #region Properties
    /// <summary>
    /// Display service
    /// </summary>
    protected DisplayService DisplayService => displayService ??= services.GetRequiredService<DisplayService>();

    /// <summary>
    /// Identity service
    /// </summary>
    protected IIdentityService IdentityService => identityService ??= services.GetRequiredService<IIdentityService>();

    /// <summary>
    /// Monitoring service
    /// </summary>
    protected IMonitoringService MonitoringService => monitoringService ??= services.GetRequiredService<IMonitoringService>();

    /// <summary>
    /// Configuration service
    /// </summary>
    protected IConfigurationService ConfigurationService => configurationService ??= services.GetRequiredService<IConfigurationService>();
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    /// <param name="services"></param>
    public void Execute(IServiceProvider services)
    {
        this.services = services;
        Execute();
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected abstract void Execute();
    #endregion

}