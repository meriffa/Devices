using Devices.Common.Models;
using Microsoft.AspNetCore.Authentication;

namespace Devices.Service.Options;

/// <summary>
/// Device authentication options
/// </summary>
public class DeviceAuthenticationOptions : AuthenticationSchemeOptions
{

    #region Properties
    /// <summary>
    /// Device authentication options header name
    /// </summary>
    public string HeaderName { get; set; } = Constants.DeviceAuthenticationHeader;
    #endregion

}