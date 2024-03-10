using System.Security.Claims;

namespace Devices.Service.Extensions;

/// <summary>
/// User extensions
/// </summary>
public static class UserExtensions
{

    #region Public Methods
    /// <summary>
    /// Return user id
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static int GetId(this ClaimsPrincipal claimsPrincipal) => Convert.ToInt32(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));
    #endregion

}