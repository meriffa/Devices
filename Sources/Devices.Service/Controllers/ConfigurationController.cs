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
    /// <param name="identity"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult<List<Release>> GetPendingReleases([FromServices] IIdentityService identityService, [FromServices] IConfigurationService service, Identity identity)
    {
        try
        {
            identityService.VerifyIdentity(identity);
            return Ok(service.GetPendingReleases(identity));
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
    /// <param name="identity"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult GetReleasePackage([FromServices] IIdentityService identityService, [FromServices] IConfigurationService service, Identity identity, int releaseId)
    {
        try
        {
            identityService.VerifyIdentity(identity);
            return new FileStreamResult(service.GetReleasePackage(identity, releaseId), "application/octet-stream");
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
            identityService.VerifyIdentity(deployment.Device);
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