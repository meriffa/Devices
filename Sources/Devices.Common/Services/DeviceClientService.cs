using Devices.Common.Interfaces.Identification;
using Devices.Common.Models;
using Devices.Common.Options;

namespace Devices.Common.Services;

/// <summary>
/// Device client service
/// </summary>
public abstract class DeviceClientService : ClientService
{

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="options"></param>
    /// <param name="identityService"></param>
    public DeviceClientService(ClientOptions options, IIdentityService identityService) : base(options)
    {
        Client.DefaultRequestHeaders.Add(Constants.DeviceAuthenticationHeader, identityService.GetDeviceToken());
    }
    #endregion

}