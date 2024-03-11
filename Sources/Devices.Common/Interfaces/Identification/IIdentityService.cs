namespace Devices.Common.Interfaces.Identification;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device id
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    string GetDeviceId(bool refresh = false);
    #endregion

}