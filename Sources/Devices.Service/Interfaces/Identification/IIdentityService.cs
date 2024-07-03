using Devices.Common.Models.Identification;
using Devices.Service.Models.Identification;
using Devices.Service.Models.Security;

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
    /// <param name="deviceId"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    List<DeviceStatus> GetDeviceStatuses(int? deviceId, User user);

    /// <summary>
    /// Return devices
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    List<Device> GetDevices(User user);

    /// <summary>
    /// Return device instance
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="user"></param>
    /// <returns></returns>
    Device GetDevice(int deviceId, User user);
    #endregion

}