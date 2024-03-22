using Devices.Common.Models.Identification;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Models.Identification;
using Devices.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace Devices.Service.Services.Identification;

/// <summary>
/// Identity service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class IdentityService(ILogger<IdentityService> logger, IOptions<ServiceOptions> options) : DataService(options.Value.Database), IIdentityService
{

    #region Private Fields
    private readonly ILogger<IdentityService> logger = logger;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return device token
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    public string GetDeviceToken(List<Fingerprint> fingerprints)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    d.""DeviceToken""
                FROM
                    ""DeviceFingerprint"" f JOIN
                    ""Device"" d ON d.""DeviceID"" = f.""DeviceID""
                WHERE
                    f.""FingerprintType"" = @FingerprintType AND
                    f.""FingerprintValue"" = @FingerprintValue AND
                    d.""DeviceEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@FingerprintType", NpgsqlDbType.Integer);
            cmd.Parameters.Add("@FingerprintValue", NpgsqlDbType.Varchar, 1024);
            foreach (var fingerprint in fingerprints)
            {
                cmd.Parameters["@FingerprintType"].Value = (int)fingerprint.Type;
                cmd.Parameters["@FingerprintValue"].Value = fingerprint.Value;
                if (cmd.ExecuteScalar() is string result)
                    return result;
            }
            throw new("Unknown device specified.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return device id
    /// </summary>
    /// <param name="deviceToken"></param>
    /// <returns></returns>
    public int? GetDeviceId(string deviceToken)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""DeviceID""
                FROM
                    ""Device""
                WHERE
                    ""DeviceToken"" = @DeviceToken AND
                    ""DeviceEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@DeviceToken", NpgsqlDbType.Varchar, 64).Value = deviceToken;
            return (int?)cmd.ExecuteScalar();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return device statuses
    /// </summary>
    /// <returns></returns>
    public List<DeviceStatus> GetDeviceStatuses()
    {
        try
        {
            var result = new List<DeviceStatus>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"WITH ""cteDeviceMetric"" AS (
                    SELECT
                        ""DeviceID"",
                        MAX(""DeviceDate"") ""DeviceDate"",
                        MAX(""LastReboot"") ""LastReboot""
                    FROM
                        ""DeviceMetric""
                    GROUP BY
                        ""DeviceID"")
                SELECT
                    d.""DeviceID"",
                    d.""DeviceToken"",
                    d.""DeviceName"",
                    d.""DeviceLocation"",
                    d.""DeviceEnabled"",
                    dm.""DeviceDate"",
                    dm.""LastReboot""
                FROM
                    ""Device"" d LEFT JOIN
                    ""cteDeviceMetric"" dm ON dm.""DeviceID"" = d.""DeviceID""
                ORDER BY
                    d.""DeviceName"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetDeviceStatus(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

    #region Internal Methods
    /// <summary>
    /// Return device instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static Device GetDevice(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["DeviceID"],
        Name = (string)reader["DeviceName"],
        Location = (string)reader["DeviceLocation"]
    };

    /// <summary>
    /// Return device status instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    public DeviceStatus GetDeviceStatus(NpgsqlDataReader reader) => new()
    {
        Device = GetDevice(reader),
        Token = (string)reader["DeviceToken"],
        Enabled = (bool)reader["DeviceEnabled"],
        Level = reader["DeviceDate"] is DBNull ? DeviceLevel.Red : GetDeviceLevel((DateTime)reader["DeviceDate"]),
        DeviceDate = reader["DeviceDate"] is DBNull ? null : (DateTime)reader["DeviceDate"],
        Uptime = reader["LastReboot"] is DBNull ? null : DateTime.UtcNow.Subtract((DateTime)reader["LastReboot"]),
        Deployments = GetDeviceDeployments((int)reader["DeviceID"])
    };

    /// <summary>
    /// Return device level
    /// </summary>
    /// <param name="deviceDate"></param>
    /// <returns></returns>
    private static DeviceLevel GetDeviceLevel(DateTime deviceDate) => DateTime.UtcNow.Subtract(deviceDate).TotalMinutes switch
    {
        <= 5 => DeviceLevel.Green,
        <= 10 => DeviceLevel.Amber,
        _ => DeviceLevel.Red
    };

    /// <summary>
    /// Return device deployments
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    private List<DeviceDeployment> GetDeviceDeployments(int deviceId)
    {
        var result = new List<DeviceDeployment>();
        using var cn = GetConnection();
        using var cmd = GetCommand(
            @"WITH ""cteDeviceDeployment"" AS (
                    SELECT
                    ""DeviceID"",
                    ""ReleaseID"",
                    MAX(""DeviceDate"") ""DeviceDate""
                FROM
                    ""DeviceDeployment""
                GROUP BY
                    ""DeviceID"",
                    ""ReleaseID"")
            SELECT
                a.""ApplicationName"",
                r.""Version"",
                dd.""Success""
            FROM
                ""DeviceDeployment"" dd JOIN
                ""cteDeviceDeployment"" f ON f.""DeviceID"" = dd.""DeviceID"" AND f.""ReleaseID"" = dd.""ReleaseID"" AND f.""DeviceDate"" = dd.""DeviceDate"" JOIN
                ""Release"" r ON r.""ReleaseID"" = dd.""ReleaseID"" JOIN
                ""Application"" a ON a.""ApplicationID"" = r.""ApplicationID""
            WHERE
                dd.""DeviceID"" = @DeviceID;", cn);
        cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add(new()
            {
                Application = (string)r["ApplicationName"],
                Version = (string)r["Version"],
                Success = (bool)r["Success"]
            });
        return result;
    }
    #endregion

}