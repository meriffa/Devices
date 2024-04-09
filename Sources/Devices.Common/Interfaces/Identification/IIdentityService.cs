namespace Devices.Common.Interfaces.Identification;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device bearer token
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    string GetDeviceBearerToken(bool refresh = false);
    #endregion

}