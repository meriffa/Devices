using Devices.Common.Models;
using Devices.Common.Solutions.Garden.Models;
using Devices.Common.Solutions.Garden.Services;
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
    /// Return weather conditions
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet]
    public ActionResult<List<WeatherCondition>> GetWeatherConditions([FromServices] IGardenService service)
    {
        try
        {
            return Ok(service.GetWeatherConditions());
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
    [HttpPost]
    public ServiceResult SaveWeatherCondition([FromServices] IGardenService service, WeatherCondition weatherCondition)
    {
        return service.SaveWeatherCondition(weatherCondition);
    }
    #endregion

}