using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.Client.Solutions.Controllers;

/// <summary>
///  Base controller
/// </summary>
public abstract class Controller
{

    #region Private Fields
    private IServiceProvider services = null!;
    private DisplayService? displayService;
    private IGardenServiceClient? gardenService;
    #endregion

    #region Properties
    /// <summary>
    /// Display service
    /// </summary>
    protected DisplayService DisplayService => displayService ??= services.GetRequiredService<DisplayService>();

    /// <summary>
    /// Garden service
    /// </summary>
    protected IGardenServiceClient GardenService => gardenService ??= services.GetRequiredService<IGardenServiceClient>();
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