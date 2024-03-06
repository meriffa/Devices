using Devices.Common.Models.Identification;

namespace Devices.Client.Interfaces.Identification;

/// <summary>
/// Identity service interface
/// </summary>
public interface IIdentityService
{

    #region Public Methods
    /// <summary>
    /// Return device identity
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    Identity GetIdentity(bool refresh = false);
    #endregion

}