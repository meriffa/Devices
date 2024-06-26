using Devices.Service.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Devices.Service.Controllers;

/// <summary>
/// Security controller
/// </summary>
[ApiController, Route("/Service/[controller]/[action]"), Authorize(Policy = "WebPolicy")]
public class SecurityController : Controller
{

    #region Public Methods
    /// <summary>
    /// Sign out
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> SignOut([FromServices] ILogger<SecurityController> logger)
    {
        logger.LogInformation("User signed out (ID = {ID}, Name = '{Name}', Time = '{Time}').", User.GetUserId(), User.Identity?.Name, DateTime.UtcNow);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Index");
    }
    #endregion

}