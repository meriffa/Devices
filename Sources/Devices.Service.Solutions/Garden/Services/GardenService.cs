using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Models.Identification;
using Devices.Service.Options;
using Devices.Service.Services;
using Devices.Service.Services.Identification;
using Devices.Service.Solutions.Garden.Interfaces;
using Devices.Service.Solutions.Garden.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
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
    /// Return weather devices
    /// </summary>
    /// <returns></returns>
    public List<Device> GetDevices()
    {
        try
        {
            var result = new List<Device>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT DISTINCT
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation""
                FROM
                    ""Garden"".""WeatherCondition"" w JOIN
                    ""Device"" d ON d.""DeviceID"" = w.""DeviceID""
                ORDER BY
                    d.""DeviceName"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(IdentityService.GetDevice(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return weather conditions
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public List<DeviceWeatherCondition> GetDeviceWeatherConditions(int? deviceId)
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
                    d.""DeviceName"",
                    d.""DeviceLocation""
                FROM
                    ""Garden"".""WeatherCondition"" w JOIN
                    ""Device"" d ON d.""DeviceID"" = w.""DeviceID""
                WHERE
                    (d.""DeviceID"" = @DeviceID OR @DeviceID IS NULL);", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = (object?)deviceId ?? DBNull.Value;
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
    /// Return aggregate weather conditions
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="aggregationType"></param>
    /// <returns></returns>
    public List<AggregateWeatherCondition> GetAggregateWeatherConditions(int? deviceId, AggregationType aggregationType)
    {
        try
        {
            var result = new List<AggregateWeatherCondition>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    DATE_TRUNC(@AggregationType, w.""DeviceDate"") ""DeviceDate"",
                    MIN(w.""Temperature"") ""TemperatureMin"",
                    MAX(w.""Temperature"") ""TemperatureMax"",
                    AVG(w.""Temperature"") ""TemperatureAvg"",
                    MIN(w.""Humidity"") ""HumidityMin"",
                    MAX(w.""Humidity"") ""HumidityMax"",
                    AVG(w.""Humidity"") ""HumidityAvg"",
                    MIN(w.""Pressure"") ""PressureMin"",
                    MAX(w.""Pressure"") ""PressureMax"",
                    AVG(w.""Pressure"") ""PressureAvg"",
                    MIN(w.""Illuminance"") ""IlluminanceMin"",
                    MAX(w.""Illuminance"") ""IlluminanceMax"",
                    AVG(w.""Illuminance"") ""IlluminanceAvg"",
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation""
                FROM
                    ""Garden"".""WeatherCondition"" w JOIN
                    ""Device"" d ON d.""DeviceID"" = w.""DeviceID""
                WHERE
                    (d.""DeviceID"" = @DeviceID OR @DeviceID IS NULL)
                GROUP BY
                    DATE_TRUNC(@AggregationType, w.""DeviceDate""),
                    d.""DeviceID"",
                    d.""DeviceToken"",
                    d.""DeviceName"",
                    d.""DeviceLocation"",
                    d.""DeviceEnabled"";", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = (object?)deviceId ?? DBNull.Value;
            cmd.Parameters.Add("@AggregationType", NpgsqlDbType.Text).Value = GetAggregationTypeValue(aggregationType);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(new()
                {
                    Device = IdentityService.GetDevice(r),
                    DeviceDate = (DateTime)r["DeviceDate"],
                    Temperature = GetAggregateMeasurement(r, "Temperature"),
                    Humidity = GetAggregateMeasurement(r, "Humidity"),
                    Pressure = GetAggregateMeasurement(r, "Pressure"),
                    Illuminance = GetAggregateMeasurement(r, "Illuminance")
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

    #region Private Methods
    /// <summary>
    /// Return aggregate measurement instance
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="prefix"></param>
    /// <returns></returns>
    private static AggregateMeasurement GetAggregateMeasurement(NpgsqlDataReader reader, string prefix) => new()
    {
        Minimum = (double)(decimal)reader[$"{prefix}Min"],
        Maximum = (double)(decimal)reader[$"{prefix}Max"],
        Average = (double)(decimal)reader[$"{prefix}Avg"],
    };

    /// <summary>
    /// Return aggregation type value
    /// </summary>
    /// <param name="aggregationType"></param>
    /// <returns></returns>
    private static string GetAggregationTypeValue(AggregationType aggregationType) => aggregationType switch
    {
        AggregationType.Hourly => "hour",
        AggregationType.Daily => "day",
        AggregationType.Weekly => "week",
        AggregationType.Monthly => "month",
        _ => throw new($"Aggregation type '{aggregationType}' is not supported.")
    };
    #endregion

}