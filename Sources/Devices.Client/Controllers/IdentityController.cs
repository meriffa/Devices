using CommandLine;

namespace Devices.Client.Controllers;

/// <summary>
/// Identity controller
/// </summary>
[Verb("Identity", HelpText = "Identity operation.")]
public class IdentityController : Controller
{

    #region Properties
    /// <summary>
    /// Force identity refresh flag
    /// </summary>
    [Option('r', "refresh", Default = false, HelpText = "Force identity refresh.")]
    public bool Refresh { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation($"Identity operation started.");
        DisplayService.WriteInformation($"Device ID = {IdentityService.GetIdentity(Refresh).Id}");
        DisplayService.WriteInformation($"Identity operation completed.");
    }
    #endregion

}