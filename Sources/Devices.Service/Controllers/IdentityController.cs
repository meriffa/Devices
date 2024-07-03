using Devices.Common.Models.Identification;
using Devices.Service.Extensions;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Interfaces.Security;
using Devices.Service.Models.Identification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Service.Controllers;

/// <summary>
/// Identity controller
/// </summary>
[ApiController, Route("/Service/[controller]/[action]")]
public class IdentityController : ControllerBase
{

    #region Public Methods
    /// <summary>
    /// Return device token
    /// </summary>
    /// <param name="service"></param>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public ActionResult<string> GetDeviceToken([FromServices] IIdentityService service, List<Fingerprint> fingerprints)
    {
        try
        {
            return Ok(service.GetDeviceToken(fingerprints));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return device bearer token
    /// </summary>
    /// <param name="service"></param>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public ActionResult<string> GetDeviceBearerToken([FromServices] IIdentityService service, List<Fingerprint> fingerprints)
    {
        try
        {
            return Ok(service.GetDeviceBearerToken(fingerprints));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Validate device bearer token
    /// </summary>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "DevicePolicy")]
    public ActionResult ValidateDeviceBearerToken()
    {
        try
        {
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return device statuses
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="securityService"></param>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
    public ActionResult<List<DeviceStatus>> GetDeviceStatuses([FromServices] IIdentityService identityService, [FromServices] ISecurityService securityService, int? deviceId)
    {
        try
        {
            return Ok(identityService.GetDeviceStatuses(deviceId, securityService.GetUser(User.GetUserId())));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return devices
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="securityService"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
    public ActionResult<List<Device>> GetDevices([FromServices] IIdentityService identityService, [FromServices] ISecurityService securityService)
    {
        try
        {
            return Ok(identityService.GetDevices(securityService.GetUser(User.GetUserId())));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}