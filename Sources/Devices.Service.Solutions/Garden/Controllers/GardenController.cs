using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Extensions;
using Devices.Service.Solutions.Garden.Interfaces;
using Devices.Service.Solutions.Garden.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Devices.Service.Solutions.Garden.Controllers;

/// <summary>
/// Garden controller
/// </summary>
[ApiController, Route("/Service/Solutions/[controller]/[action]")]
public class GardenController : ControllerBase
{

    #region Public Methods
    /// <summary>
    /// Return device weather conditions
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<List<DeviceWeatherCondition>> GetDeviceWeatherConditions([FromServices] IGardenService service)
    {
        try
        {
            return Ok(service.GetDeviceWeatherConditions());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="service"></param>
    /// <param name="weatherCondition"></param>
    /// <returns></returns>
    [HttpPost, Authorize(Policy = "DevicePolicy")]
    public ActionResult SaveWeatherCondition([FromServices] IGardenService service, WeatherCondition weatherCondition)
    {
        try
        {
            service.SaveWeatherCondition(HttpContext.User.GetDeviceId(), weatherCondition);
            return Ok();
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}