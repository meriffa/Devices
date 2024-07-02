using Devices.Service.Extensions;
using Devices.Service.Interfaces.Security;
using Devices.Service.Models.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Devices.Service.Controllers;

/// <summary>
/// Security controller
/// </summary>
[ApiController, Route("/Service/[controller]/[action]")]
public class SecurityController : ControllerBase
{

    #region Public Methods
    /// <summary>
    /// Return tenants
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
    public ActionResult<List<Tenant>> GetTenants([FromServices] ISecurityService service)
    {
        try
        {
            return Ok(service.GetTenants());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return users
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
    public ActionResult<List<User>> GetUsers([FromServices] ISecurityService service)
    {
        try
        {
            return Ok(service.GetUsers());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Sign out
    /// </summary>
    /// <param name="logger"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "WebPolicy")]
    public async Task<IActionResult> SignOut([FromServices] ILogger<SecurityController> logger)
    {
        logger.LogInformation("User signed out (ID = {ID}, Name = '{Name}', Time = '{Time}').", User.GetUserId(), User.Identity?.Name, DateTime.UtcNow);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToPage("/Index");
    }
    #endregion

}