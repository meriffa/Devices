using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Models.Identification;
using Devices.Service.Solutions.Garden.Models;

namespace Devices.Service.Solutions.Garden.Interfaces;

/// <summary>
/// Garden service interface
/// </summary>
public interface IGardenService
{

    #region Public Methods
    /// <summary>
    /// Return weather devices
    /// </summary>
    /// <returns></returns>
    List<Device> GetDevices();

    /// <summary>
    /// Return device weather conditions
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    List<DeviceWeatherCondition> GetDeviceWeatherConditions(int? deviceId);

    /// <summary>
    /// Return aggregate weather conditions
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="aggregationType"></param>
    /// <returns></returns>
    List<AggregateWeatherCondition> GetAggregateWeatherConditions(int? deviceId, AggregationType aggregationType);

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="weatherCondition"></param>
    void SaveWeatherCondition(int deviceId, WeatherCondition weatherCondition);

    /// <summary>
    /// Return device camera notifications
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    List<DeviceCameraNotification> GetDeviceCameraNotifications(int? deviceId);

    /// <summary>
    /// Save camera notification
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="cameraNotification"></param>
    void SaveCameraNotification(int deviceId, CameraNotification cameraNotification);
    #endregion

}