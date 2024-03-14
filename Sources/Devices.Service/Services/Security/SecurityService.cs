using Devices.Service.Interfaces.Security;
using Devices.Service.Models.Security;
using Devices.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NpgsqlTypes;
using System.Security.Cryptography;
using System.Text;

namespace Devices.Service.Services.Security;

/// <summary>
/// Security service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
public class SecurityService(ILogger<SecurityService> logger, IOptions<ServiceOptions> options) : DataService(options.Value.Database), ISecurityService
{

    #region Private Fields
    private readonly ILogger<SecurityService> logger = logger;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return user
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public User? GetUser(string username, string password)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""UserID"",
                    ""FullName"",
                    ""Email"",
                    ""Role""
                FROM
                    ""User""
                WHERE
                    LOWER(""Username"") = LOWER(@Username) AND
                    LOWER(""Password"") = LOWER(@Password) AND
                    ""UserEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@Username", NpgsqlDbType.Varchar, 128).Value = username;
            cmd.Parameters.Add("@Password", NpgsqlDbType.Varchar, 128).Value = HashPassword(password);
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return new()
                {
                    Id = (int)r["UserID"],
                    FullName = (string)r["FullName"],
                    Email = (string)r["Email"],
                    Role = (string)r["Role"]
                };
            return null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Check if user is enabled
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public bool IsUserEnabled(int userId)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""UserID""
                FROM
                    ""User""
                WHERE
                    ""UserID"" = @UserID AND
                    ""UserEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@UserID", NpgsqlDbType.Integer).Value = userId;
            return cmd.ExecuteScalar() != null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

    #region Private Members
    /// <summary>
    /// Hash password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static string HashPassword(string password) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    #endregion

}