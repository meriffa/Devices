using Devices.Common.Models.Monitoring;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Interfaces.Monitoring;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Service.Controllers;

/// <summary>
/// Monitoring controller
/// </summary>
[ApiController, Route("/Service/[controller]/[action]")]
public class MonitoringController : ControllerBase
{

    #region Public Methods
    /// <summary>
    /// Return monitoring metrics
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<MonitoringMetrics>> GetMonitoringMetrics([FromServices] IMonitoringService service)
    {
        try
        {
            return Ok(service.GetMonitoringMetrics());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Save monitoring metrics
    /// </summary>
    /// <param name="identityService"></param>
    /// <param name="service"></param>
    /// <param name="metrics"></param>
    /// <returns></returns>
    [HttpPost]
    public ActionResult SaveMonitoringMetrics([FromServices] IIdentityService identityService, [FromServices] IMonitoringService service, MonitoringMetrics metrics)
    {
        try
        {
            identityService.VerifyIdentity(metrics.Identity);
            service.SaveMonitoringMetrics(metrics);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}