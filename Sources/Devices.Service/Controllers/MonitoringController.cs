using Devices.Common.Models.Monitoring;
using Devices.Service.Extensions;
using Devices.Service.Interfaces.Monitoring;
using Devices.Service.Models.Monitoring;
using Microsoft.AspNetCore.Authorization;
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
    [HttpGet, Authorize(Policy = "FrameworkPolicy")]
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
    /// Save device metrics
    /// </summary>
    /// <param name="service"></param>
    /// <param name="metrics"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = "DevicePolicy")]
    public ActionResult SaveDeviceMetrics([FromServices] IMonitoringService service, DeviceMetrics metrics)
    {
        try
        {
            service.SaveDeviceMetrics(HttpContext.User.GetDeviceId(), DateTime.UtcNow, metrics);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}