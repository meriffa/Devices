using Devices.Common.Models.Configuration;
using Devices.Common.Models.Identification;
using Devices.Service.Interfaces.Configuration;
using Devices.Service.Interfaces.Identification;
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

    /// <summary>
    /// Return pending device releases
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="service"></param>
    /// <param name="device"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<List<Release>> GetPendingReleases([FromServices] IIdentityService identityService, [FromServices] IConfigurationService service, Device device)
    {
        try
        {
            identityService.VerifyDevice(device);
            return Ok(service.GetPendingReleases(device));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return release package
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="service"></param>
    /// <param name="device"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult GetReleasePackage([FromServices] IIdentityService identityService, [FromServices] IConfigurationService service, Device device, int releaseId)
    {
        try
        {
            identityService.VerifyDevice(device);
            return new FileStreamResult(service.GetReleasePackage(device, releaseId), "application/octet-stream");
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return deployments
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<Deployment>> GetDeployments([FromServices] IConfigurationService service)
    {
        try
        {
            return Ok(service.GetDeployments());
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
    [HttpGet]
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
    /// <param name="identityService"></param>
    /// <param name="service"></param>
    /// <param name="deployment"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SaveDeployment([FromServices] IIdentityService identityService, [FromServices] IConfigurationService service, Deployment deployment)
    {
        try
        {
            identityService.VerifyDevice(deployment.Device);
            service.SaveDeployment(deployment);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}