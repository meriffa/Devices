namespace Devices.Common.Interfaces.Identification;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device token
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    string GetDeviceToken(bool refresh = false);
    #endregion

}