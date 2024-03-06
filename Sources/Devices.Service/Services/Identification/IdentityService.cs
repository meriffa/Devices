using Devices.Common.Models.Identification;
using Devices.Service.Interfaces.Identification;
using Devices.Service.Models.Identification;
using Devices.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    /// Return device identity
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    public Identity GetIdentity(List<Fingerprint> fingerprints)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    f.""DeviceID""
                FROM
                    ""DeviceFingerprint"" f JOIN
                    ""Device"" d ON d.""DeviceID"" = f.""DeviceID""
                WHERE
                    f.""FingerprintType"" = @FingerprintType AND
                    f.""FingerprintValue"" = @FingerprintValue AND
                    d.""Active"" = TRUE;", cn);
            cmd.Parameters.Add("@FingerprintType", NpgsqlDbType.Integer);
            cmd.Parameters.Add("@FingerprintValue", NpgsqlDbType.Varchar, 1024);
            foreach (var fingerprint in fingerprints)
            {
                cmd.Parameters["@FingerprintType"].Value = (int)fingerprint.Type;
                cmd.Parameters["@FingerprintValue"].Value = fingerprint.Value;
                if (cmd.ExecuteScalar() is string deviceId)
                    return new Identity() { Id = deviceId };
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
    /// Verify device identity
    /// </summary>
    /// <param name="fingerprints"></param>
    /// <returns></returns>
    public void VerifyIdentity(Identity identity)
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
                    ""Active"" = TRUE;", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = identity.Id;
            var deviceId = cmd.ExecuteScalar();
            if (deviceId == null || deviceId is DBNull)
                throw new($"Invalid device id '{identity.Id}' specified.");
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
                    ""Active""
                FROM
                    ""Device""
                ORDER BY
                    ""DeviceName"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(new()
                {
                    Identity = new Identity() { Id = (string)r["DeviceID"] },
                    Name = (string)r["DeviceName"],
                    Active = (bool)r["Active"]
                });
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

}