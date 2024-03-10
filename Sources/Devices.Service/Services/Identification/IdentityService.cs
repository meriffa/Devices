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
    /// Return device id
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    public string GetDeviceId(List<Fingerprint> fingerprints)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    d.""DeviceID""
                FROM
                    ""DeviceFingerprint"" f JOIN
                    ""Device"" d ON d.""DeviceID"" = f.""DeviceID""
                WHERE
                    f.""FingerprintType"" = @FingerprintType AND
                    f.""FingerprintValue"" = @FingerprintValue AND
                    d.""DeviceActive"" = TRUE;", cn);
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
    /// Check if device is enabled
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public bool IsDeviceEnabled(string deviceId)
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
                    ""DeviceID"" = @DeviceID AND
                    ""DeviceActive"" = TRUE;", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = deviceId;
            return cmd.ExecuteScalar() != null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return devices
    /// </summary>
    /// <returns></returns>
    public List<Device> GetDevices()
    {
        try
        {
            var result = new List<Device>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""DeviceID"",
                    ""DeviceName"",
                    ""DeviceActive""
                FROM
                    ""Device""
                ORDER BY
                    ""DeviceName"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetDevice(r));
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
    internal static Device GetDevice(NpgsqlDataReader reader) => new()
    {
        Id = (string)reader["DeviceID"],
        Name = (string)reader["DeviceName"],
        Active = (bool)reader["DeviceActive"]
    };
    #endregion

}