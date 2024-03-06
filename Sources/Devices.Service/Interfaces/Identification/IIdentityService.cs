using Devices.Common.Models.Identification;
using Devices.Service.Models.Identification;

namespace Devices.Service.Interfaces.Identification;

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
    /// Verify device identity
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    void VerifyIdentity(Identity identity);

    /// <summary>
    /// Return devices
    /// </summary>
    /// <returns></returns>
    List<Device> GetDevices();
    #endregion

}