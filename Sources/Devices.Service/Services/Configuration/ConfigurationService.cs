using Action = Devices.Common.Models.Configuration.Action;
using Devices.Common.Models.Configuration;
using Devices.Common.Services;
using Devices.Service.Interfaces.Configuration;
using Devices.Service.Models.Configuration;
using Devices.Service.Options;
using Devices.Service.Services.Identification;
using Microsoft.AspNetCore.Http;
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
    /// Return actions
    /// </summary>
    /// <returns></returns>
    public List<Action> GetActions()
    {
        try
        {
            var result = new List<Action>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""ActionID"",
                    ""ActionType"",
                    ""ActionParameters"",
                    ""ActionArguments""
                FROM
                    ""Action""
                ORDER BY
                    ""ActionID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetAction(r));
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
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    r.""AllowConcurrency"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID""
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
    /// Return release
    /// </summary>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    public Release GetRelease(int releaseId)
    {
        try
        {
            var result = new List<Release>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    r.""AllowConcurrency"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID""
                WHERE
                    r.""ReleaseID"" = @ReleaseID;", cn);
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = releaseId;
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return GetRelease(r);
            throw new($"Invalid release specified (ReleaseID = {releaseId}).");
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
                @"SELECT
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    r.""AllowConcurrency"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
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
    /// Return required device releases
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="applications"></param>
    /// <returns></returns>
    public List<Release> GetRequiredReleases(int deviceId, List<RequiredApplication> applications)
    {
        try
        {
            var result = new List<Release>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    r.""AllowConcurrency"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    dd.""Success""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID"" LEFT JOIN
                    ""DeviceDeployment"" dd ON dd.""ReleaseID"" = r.""ReleaseID"" AND dd.""DeviceID"" = @DeviceID
                WHERE
                    r.""ApplicationID"" = @ApplicationID AND
                    (r.""Version"" >= @Version OR @Version IS NULL)
                ORDER BY
                    r.""Version"" DESC
                LIMIT 1;", cn);
            cmd.Parameters.Add("@DeviceID", NpgsqlDbType.Integer).Value = deviceId;
            cmd.Parameters.Add("@ApplicationID", NpgsqlDbType.Integer);
            cmd.Parameters.Add("@Version", NpgsqlDbType.Varchar, 64);
            foreach (var application in applications)
            {
                cmd.Parameters["@ApplicationID"].Value = application.Application.Id;
                cmd.Parameters["@Version"].Value = (object?)application.MinimumVersion ?? DBNull.Value;
                using var r = cmd.ExecuteReader();
                while (r.Read())
                    if (r["Success"] is DBNull || !(bool)r["Success"])
                        result.Add(GetRelease(r));
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save release
    /// </summary>
    /// <param name="release"></param>
    /// <returns></returns>
    public Release SaveRelease(Release release) => release.Id == 0 ? CreateRelease(release) : UpdateRelease(release);

    /// <summary>
    /// Enable / disable release
    /// </summary>
    /// <param name="releaseId"></param>
    /// <param name="enabled"></param>
    public void EnableDisableRelease(int releaseId, bool enabled)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"UPDATE ""Release"" SET
                    ""ReleaseEnabled"" = @ReleaseEnabled
                WHERE
                    ""ReleaseID"" = @ReleaseID;", cn);
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = releaseId;
            cmd.Parameters.Add("@ReleaseEnabled", NpgsqlDbType.Boolean).Value = enabled;
            cmd.ExecuteNonQuery();
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
    /// Save release package
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public string SaveReleasePackage(IFormFile file)
    {
        try
        {
            string fileName = Path.GetFileName(file.FileName);
            if (file.Length > 0 && fileName.EndsWith(".zip"))
            {
                var path = Path.Combine(options.PackageFolder, fileName);
                using (var stream = File.Create(path))
                    file.CopyTo(stream);
                var hash = CryptographyService.GetHash(path);
                logger.LogInformation("Release package uploaded (File = '{FileName}', Hash = '{Hash}').", fileName, hash);
                return hash;
            }
            throw new($"Invalid release package specified (File = '{fileName}', Size = {file.Length}).");
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
                @"SELECT
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
                    r.""AllowConcurrency"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    act.""ActionArguments"",
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation""
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
                @"SELECT
                    d.""DeviceID"",
                    d.""DeviceName"",
                    d.""DeviceLocation"",
                    r.""ReleaseID"",
                    r.""ServiceDate"",
                    r.""Package"",
                    r.""PackageHash"",
                    r.""Version"",
                    r.""ReleaseEnabled"",
                    r.""AllowConcurrency"",
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationEnabled"",
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
    private Application GetApplication(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["ApplicationID"],
        Name = (string)reader["ApplicationName"],
        Enabled = (bool)reader["ApplicationEnabled"],
        RequiredApplications = GetRequiredApplications((int)reader["ApplicationID"])
    };

    /// <summary>
    /// Return required applications
    /// </summary>
    /// <param name="applicationId"></param>
    /// <returns></returns>
    private List<RequiredApplication> GetRequiredApplications(int applicationId)
    {
        var result = new List<RequiredApplication>();
        using var cn = GetConnection();
        using var cmd = GetCommand(
            @"SELECT
                    a.""ApplicationID"",
                    a.""ApplicationName"",
                    a.""ApplicationEnabled"",
                    ad.""MinimumVersion""
                FROM
                    ""Application"" a JOIN
                    ""ApplicationDependency"" ad ON ad.""RequiredApplicationID"" = a.""ApplicationID""
                WHERE
                    ad.""ApplicationID"" = @ApplicationID
                ORDER BY
                    a.""ApplicationID"";", cn);
        cmd.Parameters.Add("@ApplicationID", NpgsqlDbType.Integer).Value = applicationId;
        using var r = cmd.ExecuteReader();
        while (r.Read())
            result.Add(new()
            {
                Application = GetApplication(r),
                MinimumVersion = r["MinimumVersion"] is DBNull ? null : (string)r["MinimumVersion"]
            });
        return result;
    }

    /// <summary>
    /// Return action instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static Action GetAction(NpgsqlDataReader reader) => new()
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
    private Release GetRelease(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["ReleaseID"],
        ServiceDate = (DateTime)reader["ServiceDate"],
        Application = GetApplication(reader),
        Package = (string)reader["Package"],
        PackageHash = reader["PackageHash"] is DBNull ? null : (string?)reader["PackageHash"],
        Version = (string)reader["Version"],
        Action = GetAction(reader),
        Enabled = (bool)reader["ReleaseEnabled"],
        AllowConcurrency = (bool)reader["AllowConcurrency"]
    };

    /// <summary>
    /// Return pending deployment
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private PendingDeployment GetPendingDeployment(NpgsqlDataReader reader) => new()
    {
        Device = IdentityService.GetDevice(reader),
        Release = GetRelease(reader)
    };

    /// <summary>
    /// Return completed deployment instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private CompletedDeployment GetCompletedDeployment(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["DeploymentID"],
        Device = IdentityService.GetDevice(reader),
        Release = GetRelease(reader),
        DeviceDate = (DateTime)reader["DeviceDate"],
        Success = (bool)reader["Success"],
        Details = reader["Details"] is DBNull ? null : (string?)reader["Details"]
    };

    /// <summary>
    /// Create new release
    /// </summary>
    /// <param name="release"></param>
    /// <returns></returns>
    private Release CreateRelease(Release release)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"INSERT INTO ""Release""
                    (""ServiceDate"",
                    ""ApplicationID"",
                    ""Package"",
                    ""PackageHash"",
                    ""Version"",
                    ""ActionID"",
                    ""ReleaseEnabled"",
                    ""AllowConcurrency"")
                VALUES
                    (@ServiceDate,
                    @ApplicationID,
                    @Package,
                    @PackageHash,
                    @Version,
                    @ActionID,
                    @ReleaseEnabled,
                    @AllowConcurrency)
                RETURNING
                    ""ReleaseID"";", cn);
            cmd.Parameters.Add("@ServiceDate", NpgsqlDbType.TimestampTz).Value = release.ServiceDate;
            cmd.Parameters.Add("@ApplicationID", NpgsqlDbType.Integer).Value = release.Application.Id;
            cmd.Parameters.Add("@Package", NpgsqlDbType.Varchar, 1024).Value = release.Package;
            cmd.Parameters.Add("@PackageHash", NpgsqlDbType.Varchar, 64).Value = (object?)release.PackageHash ?? DBNull.Value;
            cmd.Parameters.Add("@Version", NpgsqlDbType.Varchar, 64).Value = release.Version;
            cmd.Parameters.Add("@ActionID", NpgsqlDbType.Integer).Value = release.Action.Id;
            cmd.Parameters.Add("@ReleaseEnabled", NpgsqlDbType.Boolean).Value = release.Enabled;
            cmd.Parameters.Add("@AllowConcurrency", NpgsqlDbType.Boolean).Value = release.AllowConcurrency;
            release.Id = (int)cmd.ExecuteScalar()!;
            return release;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Update existing release
    /// </summary>
    /// <param name="release"></param>
    /// <returns></returns>
    private Release UpdateRelease(Release release)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"UPDATE ""Release"" SET
                    ""ServiceDate"" = @ServiceDate,
                    ""ApplicationID"" = @ApplicationID,
                    ""Package"" = @Package,
                    ""PackageHash"" = @PackageHash,
                    ""Version"" = @Version,
                    ""ActionID"" = @ActionID,
                    ""ReleaseEnabled"" = @ReleaseEnabled,
                    ""AllowConcurrency"" = @AllowConcurrency
                WHERE
                    ""ReleaseID"" = @ReleaseID;", cn);
            cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = release.Id;
            cmd.Parameters.Add("@ServiceDate", NpgsqlDbType.TimestampTz).Value = release.ServiceDate;
            cmd.Parameters.Add("@ApplicationID", NpgsqlDbType.Integer).Value = release.Application.Id;
            cmd.Parameters.Add("@Package", NpgsqlDbType.Varchar, 1024).Value = release.Package;
            cmd.Parameters.Add("@PackageHash", NpgsqlDbType.Varchar, 64).Value = (object?)release.PackageHash ?? DBNull.Value;
            cmd.Parameters.Add("@Version", NpgsqlDbType.Varchar, 64).Value = release.Version;
            cmd.Parameters.Add("@ActionID", NpgsqlDbType.Integer).Value = release.Action.Id;
            cmd.Parameters.Add("@ReleaseEnabled", NpgsqlDbType.Boolean).Value = release.Enabled;
            cmd.Parameters.Add("@AllowConcurrency", NpgsqlDbType.Boolean).Value = release.AllowConcurrency;
            cmd.ExecuteNonQuery();
            DeleteDeployments(cn, release);
            return release;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Delete deployments
    /// </summary>
    /// <param name="cn"></param>
    /// <param name="release"></param>
    private void DeleteDeployments(NpgsqlConnection cn, Release release)
    {
        using var cmd = GetCommand(@"DELETE FROM ""DeviceDeployment"" WHERE ""ReleaseID"" = @ReleaseID;", cn);
        cmd.Parameters.Add("@ReleaseID", NpgsqlDbType.Integer).Value = release.Id;
        cmd.ExecuteNonQuery();
    }
    #endregion

}