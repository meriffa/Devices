using Devices.Common.Models;
using Devices.Service.Models;

namespace Devices.Service.Interfaces;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device identity
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    Identity GetIdentity(List<Fingerprint> fingerprints);

    /// <summary>
    /// Return devices
    /// </summary>
    /// <returns></returns>
    List<Device> GetDevices();
    #endregion

}