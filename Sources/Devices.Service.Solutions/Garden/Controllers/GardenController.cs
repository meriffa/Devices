using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Extensions;
using Devices.Service.Models.Identification;
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
    /// Return weather devices
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<List<Device>> GetWeatherDevices([FromServices] IGardenService service)
    {
        try
        {
            return Ok(service.GetWeatherDevices());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return device weather conditions
    /// </summary>
    /// <param name="service"></param>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<List<DeviceWeatherCondition>> GetDeviceWeatherConditions([FromServices] IGardenService service, int? deviceId)
    {
        try
        {
            return Ok(service.GetDeviceWeatherConditions(deviceId));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return aggregate weather conditions
    /// </summary>
    /// <param name="service"></param>
    /// <param name="deviceId"></param>
    /// <param name="aggregationType"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<List<AggregateWeatherCondition>> GetAggregateWeatherConditions([FromServices] IGardenService service, int? deviceId, AggregationType aggregationType)
    {
        try
        {
            return Ok(service.GetAggregateWeatherConditions(deviceId, aggregationType));
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

    /// <summary>
    /// Return watering devices
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<List<Device>> GetWateringDevices([FromServices] IGardenService service)
    {
        try
        {
            return Ok(service.GetWateringDevices());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return camera devices
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<List<Device>> GetCameraDevices([FromServices] IGardenService service)
    {
        try
        {
            return Ok(service.GetCameraDevices());
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return camera view location
    /// </summary>
    /// <param name="service"></param>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "GardenPolicy")]
    public ActionResult<string> GetCameraViewLocation([FromServices] IGardenService service, int deviceId)
    {
        try
        {
            return Ok(service.GetCameraDefinition(deviceId).ViewLocation);
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }

    /// <summary>
    /// Return camera definition
    /// </summary>
    /// <param name="service"></param>
    /// <returns></returns>
    [HttpGet, Authorize(Policy = "DevicePolicy")]
    public ActionResult<CameraDefinition> GetCameraDefinition([FromServices] IGardenService service)
    {
        try
        {
            return Ok(service.GetCameraDefinition(HttpContext.User.GetDeviceId()));
        }
        catch (Exception ex)
        {
            return Problem(statusCode: StatusCodes.Status500InternalServerError, title: ex.Message);
        }
    }
    #endregion

}