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
    public static int GetUserId(this ClaimsPrincipal claimsPrincipal) => Convert.ToInt32(claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier));

    /// <summary>
    /// Return device id
    /// </summary>
    /// <param name="claimsPrincipal"></param>
    /// <returns></returns>
    public static int GetDeviceId(this ClaimsPrincipal claimsPrincipal) => Convert.ToInt32(claimsPrincipal.Identities.First(i => i.Claims.FirstOrDefault(i => i.Type == ClaimTypes.Role) != null).Claims.First(i => i.Type == ClaimTypes.NameIdentifier).Value);
    #endregion

}