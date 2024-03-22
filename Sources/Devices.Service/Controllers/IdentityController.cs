using Devices.Common.Models.Identification;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Models.Identification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Service.Controllers;

/// <summary>
/// Identity controller
/// </summary>
[ApiController, Route("/Service/[controller]/[action]"), Authorize(Policy = "FrameworkPolicy")]
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
    /// Return device statuses
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<DeviceStatus>> GetDeviceStatuses([FromServices] IIdentityService service)
    {
        try
        {
            return Ok(service.GetDeviceStatuses());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}