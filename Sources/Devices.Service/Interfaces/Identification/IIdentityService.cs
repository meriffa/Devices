using Devices.Common.Models.Identification;

namespace Devices.Service.Interfaces.Identification;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    Device GetDevice(List<Fingerprint> fingerprints);

    /// <summary>
    /// Verify device
    /// </summary>
    /// <param name="device"></param>
    void VerifyDevice(Device device);

    /// <summary>
    /// Return devices
    /// </summary>
    /// <returns></returns>
    List<Device> GetDevices();
    #endregion

}