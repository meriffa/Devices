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
    /// <returns></returns>
    string GetDeviceBearerToken();
    #endregion

}