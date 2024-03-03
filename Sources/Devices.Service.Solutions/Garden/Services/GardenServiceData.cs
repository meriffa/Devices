using Devices.Common.Solutions.Garden.Models;
using Devices.Service.Options;
using Devices.Service.Services;
using Microsoft.Extensions.Options;
using NpgsqlTypes;

namespace Devices.Service.Solutions.Garden.Services;

/// <summary>
/// Garden data service
/// </summary>
/// <param name="options"></param>
public class GardenServiceData(IOptions<ServiceOptions> options) : PostgreSqlDataService(options.Value.Database), IGardenServiceData
{

    #region Weather Condition
    /// <summary>
    /// Return weather conditions
    /// </summary>
    /// <returns></returns>
    public List<WeatherCondition> GetWeatherConditions()
    {
        var result = new List<WeatherCondition>();
        using var cn = GetConnection();
        using var cmd = GetCommand(
            @"SELECT
                ""Date"",
                ""Temperature"",
                ""Humidity"",
                ""Pressure"",
                ""Illuminance""
            FROM
                ""Garden"".""WeatherCondition""
            ORDER BY
                ""Date"" DESC;", cn);
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add(new()
            {
                Date = (DateTime)r["Date"],
                Temperature = (double)(decimal)r["Temperature"],
                Humidity = (double)(decimal)r["Humidity"],
                Pressure = (double)(decimal)r["Pressure"],
                Illuminance = (double)(decimal)r["Illuminance"]
            });
        return result;
    }

    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="weatherCondition"></param>
    public void SaveWeatherCondition(WeatherCondition weatherCondition)
    {
        using var cn = GetConnection();
        using var cmd = GetCommand(
            @$"INSERT INTO ""Garden"".""WeatherCondition""
                (""Date"",
                ""Temperature"",
                ""Humidity"",
                ""Pressure"",
                ""Illuminance"")
            VALUES
                (@Date,
                @Temperature,
                @Humidity,
                @Pressure,
                @Illuminance);", cn);
        cmd.Parameters.Add("@Date", NpgsqlDbType.TimestampTz).Value = weatherCondition.Date;
        cmd.Parameters.Add("@Temperature", NpgsqlDbType.Numeric).Value = weatherCondition.Temperature;
        cmd.Parameters.Add("@Humidity", NpgsqlDbType.Numeric).Value = weatherCondition.Humidity;
        cmd.Parameters.Add("@Pressure", NpgsqlDbType.Numeric).Value = weatherCondition.Pressure;
        cmd.Parameters.Add("@Illuminance", NpgsqlDbType.Numeric).Value = weatherCondition.Illuminance;
        cmd.ExecuteNonQuery();
    }
    #endregion

}