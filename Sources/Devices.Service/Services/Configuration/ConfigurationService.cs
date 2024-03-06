using Devices.Common.Models.Configuration;
using Devices.Service.Interfaces.Configuration;
using Devices.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;

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
                    app.""ApplicationID"",
                    app.""ApplicationName"",
                    app.""ApplicationActive"",
                    r.""Package"",
                    r.""Version"",
                    act.""ActionID"",
                    act.""ActionType"",
                    act.""ActionParameters"",
                    r.""ReleaseActive""
                FROM
                    ""Release"" r JOIN
                    ""Application"" app ON app.""ApplicationID"" = r.""ApplicationID"" JOIN
                    ""Action"" act ON act.""ActionID"" = r.""ActionID""
                ORDER BY
                    app.""ApplicationID"",
                    r.""Date"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(new()
                {
                    Id = (int)r["ReleaseID"],
                    Date = (DateTime)r["Date"],
                    Application = GetApplication(r),
                    Package = (string)r["Package"],
                    Version = (string)r["Version"],
                    Action = GetAction(r),
                    Active = (bool)r["ReleaseActive"]
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
    private static Common.Models.Configuration.Action GetAction(NpgsqlDataReader reader) => new() { Id = (int)reader["ActionID"], Type = (ActionType)(int)reader["ActionType"], Parameters = (string)reader["ActionParameters"] };
    #endregion

}