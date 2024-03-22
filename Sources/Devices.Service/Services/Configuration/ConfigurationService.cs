using Devices.Common.Models.Configuration;
using Devices.Service.Interfaces.Configuration;
using Devices.Service.Models.Configuration;
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
                    ""ApplicationEnabled""
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
                @"WITH ""cteReleaseDependency"" AS (
                    SELECT
                        ""ChildReleaseID"",
                        STRING_AGG(""ParentReleaseID""::text, ',' ORDER BY ""ParentReleaseID"") ""ParentReleaseIDs""
                    FROM
                        ""ReleaseDependency""
                    GROUP BY
                        ""ChildReleaseID"")
                SELECT
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    rd.""ParentReleaseIDs""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" LEFT JOIN
                    ""cteReleaseDependency"" rd ON rd.""ChildReleaseID"" = r.""ReleaseID""
                ORDER BY
                    r.""ReleaseID"";", cn);
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
    /// <param name="deviceId"></param>
    /// <returns></returns>
    public List<Release> GetPendingReleases(int deviceId)
    {
        try
        {
            var result = new List<Release>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"WITH ""cteReleaseDependency"" AS (
                    SELECT
                        ""ChildReleaseID"",
                        STRING_AGG(""ParentReleaseID""::text, ',' ORDER BY ""ParentReleaseID"") ""ParentReleaseIDs""
                    FROM
                        ""ReleaseDependency""
                    GROUP BY
                        ""ChildReleaseID"")
                SELECT
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    rd.""ParentReleaseIDs""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" JOIN
                    ""DeviceApplication"" da ON da.""ApplicationID"" = app.""ApplicationID"" LEFT JOIN
                    ""DeviceDeployment"" dd ON dd.""DeviceID"" = da.""DeviceID"" AND dd.""ReleaseID"" = r.""ReleaseID"" LEFT JOIN
                    ""cteReleaseDependency"" rd ON rd.""ChildReleaseID"" = r.""ReleaseID""
                WHERE
                    da.""DeviceID"" = @DeviceID AND
                    app.""ApplicationEnabled"" = TRUE AND
                    r.""ReleaseEnabled"" = TRUE AND
                    da.""DeviceApplicationEnabled"" = TRUE AND
                    dd.""DeploymentID"" IS NULL
                ORDER BY
                    r.""ReleaseID"";", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
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
    /// Check if device release has completed successfully
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    public bool HasReleaseSucceeded(int deviceId, int releaseId)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    dd.""Success""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""DeviceApplication"" da ON da.""ApplicationID"" = app.""ApplicationID"" JOIN
                    ""DeviceDeployment"" dd ON dd.""DeviceID"" = da.""DeviceID"" AND dd.""ReleaseID"" = r.""ReleaseID""
                WHERE
                    r.""ReleaseID"" = @ReleaseID and
                    da.""DeviceID"" = @DeviceID AND
                    app.""ApplicationEnabled"" = TRUE AND
                    r.""ReleaseEnabled"" = TRUE AND
                    da.""DeviceApplicationEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = releaseId;
            return (bool)cmd.ExecuteScalar()!;
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
    /// <param name="deviceId"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    public Stream GetReleasePackage(int deviceId, int releaseId)
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
                    app.""ApplicationEnabled"" = TRUE AND
                    r.""ReleaseEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = releaseId;
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
            return File.OpenRead(Path.Combine(options.PackageFolder, (string)cmd.ExecuteScalar()!));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return completed deployments
    /// </summary>
    /// <returns></returns>
    public List<CompletedDeployment> GetCompletedDeployments()
    {
        try
        {
            var result = new List<CompletedDeployment>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"WITH ""cteReleaseDependency"" AS (
                    SELECT
                        ""ChildReleaseID"",
                        STRING_AGG(""ParentReleaseID""::text, ',' ORDER BY ""ParentReleaseID"") ""ParentReleaseIDs""
                    FROM
                        ""ReleaseDependency""
                    GROUP BY
                        ""ChildReleaseID"")
                SELECT
                    dd.""DeploymentID"",
                    dd.""DeviceID"",
                    dd.""ReleaseID"",
                    dd.""DeviceDate"",
                    dd.""Success"",
                    dd.""Details"",
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation"",
                    rd.""ParentReleaseIDs""
                FROM
                    ""DeviceDeployment"" dd JOIN
                    ""Release"" r ON r.""ReleaseID"" = dd.""ReleaseID"" JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" JOIN
                    ""Device"" d ON d.""DeviceID"" = dd.""DeviceID"" LEFT JOIN
                    ""cteReleaseDependency"" rd ON rd.""ChildReleaseID"" = r.""ReleaseID""
                ORDER BY
                    dd.""DeploymentID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetCompletedDeployment(r));
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
                @"WITH ""cteReleaseDependency"" AS (
                    SELECT
                        ""ChildReleaseID"",
                        STRING_AGG(""ParentReleaseID""::text, ',' ORDER BY ""ParentReleaseID"") ""ParentReleaseIDs""
                    FROM
                        ""ReleaseDependency""
                    GROUP BY
                        ""ChildReleaseID"")
                SELECT
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation"",
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    rd.""ParentReleaseIDs""
                FROM
                    ""Device"" d JOIN
                    ""DeviceApplication"" da ON da.""DeviceID"" = d.""DeviceID"" JOIN
                    ""Application"" app ON app.""ApplicationID"" = da.""ApplicationID"" JOIN
                    ""Release"" r ON r.""ApplicationID"" = app.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" LEFT JOIN
                    ""DeviceDeployment"" dd ON dd.""DeviceID"" = d.""DeviceID"" AND dd.""ReleaseID"" = r.""ReleaseID"" LEFT JOIN
                    ""cteReleaseDependency"" rd ON rd.""ChildReleaseID"" = r.""ReleaseID""
                WHERE
                    d.""DeviceEnabled"" = TRUE AND
                    app.""ApplicationEnabled"" = TRUE AND
                    r.""ReleaseEnabled"" = TRUE AND
                    da.""DeviceApplicationEnabled"" = TRUE AND
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
    /// <param name="deviceId"></param>
    /// <param name="deployment"></param>
    public void SaveDeployment(int deviceId, Deployment deployment)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"INSERT INTO ""DeviceDeployment""
                    (""DeviceID"",
                    ""ReleaseID"",
                    ""DeviceDate"",
                    ""Success"",
                    ""Details"")
                VALUES
                    (@DeviceID,
                    @ReleaseID,
                    @DeviceDate,
                    @Success,
                    @Details);", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = deployment.Release.Id;
            cmd.Parameters.Add("@DeviceDate", NpgsqlDbType.TimestampTz).Value = deployment.DeviceDate;
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
    private static Application GetApplication(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["ApplicationID"],
        Name = (string)reader["ApplicationName"],
        Enabled = (bool)reader["ApplicationEnabled"]
    };

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
        ServiceDate = (DateTime)reader["ServiceDate"],
        Application = GetApplication(reader),
        Package = (string)reader["Package"],
        PackageHash = reader["PackageHash"] is DBNull ? null : (string?)reader["PackageHash"],
        Version = (string)reader["Version"],
        Action = GetAction(reader),
        Enabled = (bool)reader["ReleaseEnabled"],
        ParentReleaseIds = reader["ParentReleaseIDs"] is DBNull ? ([]) : Array.ConvertAll(((string)reader["ParentReleaseIDs"]).Split(','), i => Convert.ToInt32(i))
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
    /// Return completed deployment instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static CompletedDeployment GetCompletedDeployment(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["DeploymentID"],
        Device = IdentityService.GetDevice(reader),
        Release = GetRelease(reader),
        DeviceDate = (DateTime)reader["DeviceDate"],
        Success = (bool)reader["Success"],
        Details = reader["Details"] is DBNull ? null : (string?)reader["Details"]
    };
    #endregion

}