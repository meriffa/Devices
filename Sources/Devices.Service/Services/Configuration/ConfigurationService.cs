using Devices.Common.Models.Configuration;
using Devices.Common.Models.Identification;
using Devices.Service.Interfaces.Configuration;
using Devices.Service.Options;
using Devices.Service.Services.Identification;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace Devices.Service.Services.Configuration;

/// <summary>
/// Configuration service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class ConfigurationService(ILogger<ConfigurationService> logger, IOptions<ServiceOptions> options) : DataService(options.Value.Database), IConfigurationService
{

    #region Private Fields
    private readonly ILogger<ConfigurationService> logger = logger;
    private readonly ServiceOptions options = options.Value;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return applications
    /// </summary>
    /// <returns></returns>
    public List<Application> GetApplications()
    {
        try
        {
            var result = new List<Application>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""ApplicationID"",
                    ""ApplicationName"",
                    ""ApplicationActive""
                FROM
                    ""Application""
                ORDER BY
                    ""ApplicationID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetApplication(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return releases
    /// </summary>
    /// <returns></returns>
    public List<Release> GetReleases()
    {
        try
        {
            var result = new List<Release>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    r.""ReleaseID"",
                    r.""Date"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseActive"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationActive"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID""
                ORDER BY
                    app.""ApplicationID"",
                    r.""Date"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetRelease(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return pending device releases
    /// </summary>
    /// <param name="device"></param>
    /// <returns></returns>
    public List<Release> GetPendingReleases(Device device)
    {
        try
        {
            var result = new List<Release>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    r.""ReleaseID"",
                    r.""Date"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseActive"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationActive"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" JOIN
                    ""DeviceApplication"" da ON da.""ApplicationID"" = app.""ApplicationID"" LEFT JOIN
                    ""DeviceDeployment"" dd ON dd.""DeviceID"" = da.""DeviceID"" AND dd.""ReleaseID"" = r.""ReleaseID""
                WHERE
                    da.""DeviceID"" = @DeviceID AND
                    app.""ApplicationActive"" = TRUE AND
                    r.""ReleaseActive"" = TRUE AND
                    dd.""DeploymentID"" IS NULL
                ORDER BY
                    r.""ReleaseID"",
                    r.""Date"";", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = device.Id;
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetRelease(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return release package
    /// </summary>
    /// <param name="device"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    public Stream GetReleasePackage(Device device, int releaseId)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    r.""Package""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""DeviceApplication"" d ON d.""ApplicationID"" = app.""ApplicationID""
                WHERE
                    r.""ReleaseID"" = @ReleaseID AND
                    d.""DeviceID"" = @DeviceID AND
                    app.""ApplicationActive"" = TRUE AND
                    r.""ReleaseActive"" = TRUE;", cn);
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = releaseId;
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = device.Id;
            return File.OpenRead(Path.Combine(options.PackageFolder, (string)cmd.ExecuteScalar()!));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return deployments
    /// </summary>
    /// <returns></returns>
    public List<Deployment> GetDeployments()
    {
        try
        {
            var result = new List<Deployment>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    dd.""DeploymentID"",
                    dd.""DeviceID"",
                    dd.""ReleaseID"",
                    dd.""Date"",
                    dd.""Success"",
                    dd.""Details"",
                    r.""ReleaseID"",
                    r.""Date"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseActive"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationActive"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceActive""
                FROM
                    ""DeviceDeployment"" dd JOIN
                    ""Release"" r ON r.""ReleaseID"" = dd.""ReleaseID"" JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" JOIN
                    ""Device"" d ON d.""DeviceID"" = dd.""DeviceID""
                ORDER BY
                    dd.""DeploymentID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetDeployment(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return pending deployments
    /// </summary>
    /// <returns></returns>
    public List<PendingDeployment> GetPendingDeployments()
    {
        try
        {
            var result = new List<PendingDeployment>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceActive"",
                    r.""ReleaseID"",
                    r.""Date"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseActive"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationActive"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments""
                FROM
                    ""Device"" d JOIN
                    ""DeviceApplication"" da ON da.""DeviceID"" = d.""DeviceID"" JOIN
                    ""Application"" app ON app.""ApplicationID"" = da.""ApplicationID"" JOIN
                    ""Release"" r ON r.""ApplicationID"" = app.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" LEFT JOIN
                    ""DeviceDeployment"" dd ON dd.""DeviceID"" = d.""DeviceID"" AND dd.""ReleaseID"" = r.""ReleaseID""
                WHERE
                    d.""DeviceActive"" = TRUE AND
                    app.""ApplicationActive"" = TRUE AND
                    r.""ReleaseActive"" = TRUE AND
                    dd.""DeploymentID"" IS NULL
                ORDER BY
                    d.""DeviceID"",
                    r.""ReleaseID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetPendingDeployment(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="deployment"></param>
    public void SaveDeployment(Deployment deployment)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"INSERT INTO ""DeviceDeployment""
                    (""DeviceID"",
                    ""ReleaseID"",
                    ""Date"",
                    ""Success"",
                    ""Details"")
                VALUES
                    (@DeviceID,
                    @ReleaseID,
                    @Date,
                    @Success,
                    @Details);", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Varchar, 64).Value = deployment.Device.Id;
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = deployment.Release.Id;
            cmd.Parameters.Add("@Date", NpgsqlDbType.TimestampTz).Value = deployment.Date;
            cmd.Parameters.Add("@Success", NpgsqlDbType.Boolean).Value = deployment.Success;
            cmd.Parameters.Add("@Details", NpgsqlDbType.Text).Value = (object?)deployment.Details ?? DBNull.Value;
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
    /// Return application instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static Application GetApplication(NpgsqlDataReader reader) => new() { Id = (int)reader["ApplicationID"], Name = (string)reader["ApplicationName"], Active = (bool)reader["ApplicationActive"] };

    /// <summary>
    /// Return action instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static Common.Models.Configuration.Action GetAction(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["ActionID"],
        Type = (ActionType)(int)reader["ActionType"],
        Parameters = (string)reader["ActionParameters"],
        Arguments = reader["ActionArguments"] is DBNull ? null : (string?)reader["ActionArguments"]
    };

    /// <summary>
    /// Return release instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static Release GetRelease(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["ReleaseID"],
        Date = (DateTime)reader["Date"],
        Application = GetApplication(reader),
        Package = (string)reader["Package"],
        PackageHash = (string)reader["PackageHash"],
        Version = (string)reader["Version"],
        Action = GetAction(reader),
        Active = (bool)reader["ReleaseActive"]
    };

    /// <summary>
    /// Return pending deployment
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static PendingDeployment GetPendingDeployment(NpgsqlDataReader reader) => new()
    {
        Device = IdentityService.GetDevice(reader),
        Release = GetRelease(reader)
    };

    /// <summary>
    /// Return deployment instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static Deployment GetDeployment(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["DeploymentID"],
        Device = IdentityService.GetDevice(reader),
        Release = GetRelease(reader),
        Date = (DateTime)reader["Date"],
        Success = (bool)reader["Success"],
        Details = reader["Details"] is DBNull ? null : (string?)reader["Details"]
    };
    #endregion

}