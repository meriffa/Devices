using CommandLine;
using Devices.Common.Services;

namespace Devices.Client.Controllers;

/// <summary>
/// Configuration controller
/// </summary>
[Verb("Configuration", HelpText = "Configuration operation.")]
public class ConfigurationController : Controller
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Configuration operation started.");
        ReleaseGraphService.Build();
        ReleaseGraphService.Validate();
        ReleaseGraphService.Execute();
        DisplayService.WriteInformation("Configuration operation completed.");
    }
    #endregion

}