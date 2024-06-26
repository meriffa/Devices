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
    /// Return device token
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    string GetDeviceToken(List<Fingerprint> fingerprints);

    /// <summary>
    /// Return device token
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    string GetDeviceToken(int deviceId);

    /// <summary>
    /// Return device bearer token
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    string GetDeviceBearerToken(List<Fingerprint> fingerprints);

    /// <summary>
    /// Return device id
    /// </summary>
    /// <param name="deviceToken"></param>
    /// <returns></returns>
    int? GetDeviceId(string deviceToken);

    /// <summary>
    /// Return device statuses
    /// </summary>
    /// <returns></returns>
    List<DeviceStatus> GetDeviceStatuses();

    /// <summary>
    /// Return devices
    /// </summary>
    /// <returns></returns>
    List<Device> GetDevices();

    /// <summary>
    /// Return device instance
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    Device GetDevice(int deviceId);
    #endregion

}