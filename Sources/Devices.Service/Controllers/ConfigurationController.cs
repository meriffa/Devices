using Devices.Common.Models.Configuration;
using Devices.Service.Interfaces.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Service.Controllers;

/// <summary>
/// Configuration controller
/// </summary>
[ApiController, Route("/Service/[controller]/[action]")]
public class ConfigurationController : ControllerBase
{

    #region Public Methods
    /// <summary>
    /// Return applications
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<Application>> GetApplications([FromServices] IConfigurationService service)
    {
        try
        {
            return Ok(service.GetApplications());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return releases
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<Release>> GetReleases([FromServices] IConfigurationService service)
    {
        try
        {
            return Ok(service.GetReleases());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}