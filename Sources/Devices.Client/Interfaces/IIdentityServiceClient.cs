using Devices.Common.Models;

namespace Devices.Client.Interfaces;

/// <summary>
/// Identity service client interface
/// </summary>
public interface IIdentityServiceClient
{

    #region Public Methods
    /// <summary>
    /// Return device identity
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    Identity GetIdentity(bool refresh);
    #endregion

}