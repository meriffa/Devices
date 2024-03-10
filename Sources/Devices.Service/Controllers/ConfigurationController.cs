using Devices.Common.Models.Configuration;
using Devices.Service.Extensions;
using Devices.Service.Interfaces.Configuration;
using Devices.Service.Models.Configuration;
using Microsoft.AspNetCore.Authorization;
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
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
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
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
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

    /// <summary>
    /// Return pending device releases
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "DevicePolicy")]
    public ActionResult<List<Release>> GetPendingReleases([FromServices] IConfigurationService service)
    {
        try
        {
            return Ok(service.GetPendingReleases(HttpContext.User.GetDeviceId()));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return release package
    /// </summary>
    /// <param name="service"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "DevicePolicy")]
    public ActionResult GetReleasePackage([FromServices] IConfigurationService service, int releaseId)
    {
        try
        {
            return new FileStreamResult(service.GetReleasePackage(HttpContext.User.GetDeviceId(), releaseId), "application/octet-stream");
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return completed deployments
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
    public ActionResult<List<Deployment>> GetCompletedDeployments([FromServices] IConfigurationService service)
    {
        try
        {
            return Ok(service.GetCompletedDeployments());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return pending deployments
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
    public ActionResult<List<PendingDeployment>> GetPendingDeployments([FromServices] IConfigurationService service)
    {
        try
        {
            return Ok(service.GetPendingDeployments());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="service"></param>
    /// <param name="deployment"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = "DevicePolicy")]
    public ActionResult SaveDeployment([FromServices] IConfigurationService service, Deployment deployment)
    {
        try
        {
            service.SaveDeployment(HttpContext.User.GetDeviceId(), deployment);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}