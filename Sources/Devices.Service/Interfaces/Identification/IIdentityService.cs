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
    /// Return device id
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    string GetDeviceId(List<Fingerprint> fingerprints);

    /// <summary>
    /// Check if device is enabled
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    bool IsDeviceEnabled(string deviceId);

    /// <summary>
    /// Return devices
    /// </summary>
    /// <returns></returns>
    List<Device> GetDevices();
    #endregion

}