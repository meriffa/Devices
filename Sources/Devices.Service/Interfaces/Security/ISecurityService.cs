using Devices.Common.Models.Security;

namespace Devices.Service.Interfaces.Security;

/// <summary>
/// Security service interface
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public interface ISecurityService
{

    #region Public Methods
    /// <summary>
    /// Return user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    User? GetUser(string username, string password);

    /// <summary>
    /// Check if user is enabled
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    bool IsUserEnabled(int userId);
    #endregion

}