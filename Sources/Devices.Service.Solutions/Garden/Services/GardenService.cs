using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Options;
using Devices.Service.Services;
using Devices.Service.Services.Identification;
using Devices.Service.Solutions.Garden.Interfaces;
using Devices.Service.Solutions.Garden.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NpgsqlTypes;

namespace Devices.Service.Solutions.Garden.Services;

/// <summary>
/// Garden service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class GardenService(ILogger<GardenService> logger, IOptions<ServiceOptions> options) : DataService(options.Value.Database), IGardenService
{

    #region Private Fields
    private readonly ILogger<GardenService> logger = logger;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return weather conditions
    /// </summary>
    /// <returns></returns>
    public List<DeviceWeatherCondition> GetDeviceWeatherConditions()
    {
        try
        {
            var result = new List<DeviceWeatherCondition>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    w.""DeviceDate"",
                    w.""Temperature"",
                    w.""Humidity"",
                    w.""Pressure"",
                    w.""Illuminance"",
                    d.""DeviceID"",
                    d.""DeviceToken"",
                    d.""DeviceName"",
                    d.""DeviceLocation"",
                    d.""DeviceEnabled""                    
                FROM
                    ""Garden"".""WeatherCondition"" w JOIN
                    ""Device"" d ON d.""DeviceID"" = w.""DeviceID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(new()
                {
                    Device = IdentityService.GetDevice(r),
                    DeviceDate = (DateTime)r["DeviceDate"],
                    Temperature = (double)(decimal)r["Temperature"],
                    Humidity = (double)(decimal)r["Humidity"],
                    Pressure = (double)(decimal)r["Pressure"],
                    Illuminance = (double)(decimal)r["Illuminance"]
                });
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="weatherCondition"></param>
    public void SaveWeatherCondition(int deviceId, WeatherCondition weatherCondition)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"INSERT INTO ""Garden"".""WeatherCondition""
                    (""DeviceID"",
                    ""DeviceDate"",
                    ""Temperature"",
                    ""Humidity"",
                    ""Pressure"",
                    ""Illuminance"")
                VALUES
                    (@DeviceID,
                    @DeviceDate,
                    @Temperature,
                    @Humidity,
                    @Pressure,
                    @Illuminance);", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
            cmd.Parameters.Add("@DeviceDate", NpgsqlDbType.TimestampTz).Value = weatherCondition.DeviceDate;
            cmd.Parameters.Add("@Temperature", NpgsqlDbType.Numeric).Value = weatherCondition.Temperature;
            cmd.Parameters.Add("@Humidity", NpgsqlDbType.Numeric).Value = weatherCondition.Humidity;
            cmd.Parameters.Add("@Pressure", NpgsqlDbType.Numeric).Value = weatherCondition.Pressure;
            cmd.Parameters.Add("@Illuminance", NpgsqlDbType.Numeric).Value = weatherCondition.Illuminance;
            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

}