using Devices.Common.Models;
using Devices.Service.Interfaces;
using Devices.Service.Models;
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
    /// Return device identity
    /// </summary>
    /// <param name="service"></param>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<Identity> GetIdentity([FromServices] IIdentityService service, [FromBody] List<Fingerprint> fingerprints)
    {
        try
        {
            return Ok(service.GetIdentity(fingerprints));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return devices
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<Device>> GetDevices([FromServices] IIdentityService service)
    {
        try
        {
            return Ok(service.GetDevices());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}