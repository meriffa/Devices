using Devices.Common.Models.Identification;

namespace Devices.Client.Interfaces.Identification;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    Device GetDevice(bool refresh = false);
    #endregion

}