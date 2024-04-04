using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Models.Identification;

namespace Devices.Service.Solutions.Garden.Models;

/// <summary>
/// Device camera notification
/// </summary>
public class DeviceCameraNotification : CameraNotification
{

    #region Properties
    /// <summary>
    /// Camera notification device
    /// </summary>
    public required Device Device { get; set; }
    #endregion

}