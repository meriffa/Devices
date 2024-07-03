using Devices.Service.Interfaces.Security;
using Devices.Service.Models.Security;
using Devices.Service.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Npgsql;
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
    /// Return tenants
    /// </summary>
    /// <returns></returns>
    public List<Tenant> GetTenants()
    {
        try
        {
            var result = new List<Tenant>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    ""TenantID"",
                    ""TenantName"",
                    ""Email"",
                    ""TenantEnabled""
                FROM
                    ""Tenant""
                ORDER BY
                    ""TenantID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetTenant(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return users
    /// </summary>
    /// <returns></returns>
    public List<User> GetUsers()
    {
        try
        {
            var result = new List<User>();
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    u.""UserID"",
                    u.""Username"",
                    u.""FullName"",
                    u.""Email"",
                    u.""Role"",
                    u.""UserEnabled"",
                    t.""TenantID"",
                    t.""TenantName"",
                    t.""Email"",
                    t.""TenantEnabled""
                FROM
                    ""User"" u JOIN
                    ""Tenant"" t ON t.""TenantID"" = u.""TenantID""
                ORDER BY
                    u.""UserID"";", cn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                result.Add(GetUser(r));
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Return user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public User GetUser(int userId)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"SELECT
                    u.""UserID"",
                    u.""Username"",
                    u.""FullName"",
                    u.""Email"",
                    u.""Role"",
                    u.""UserEnabled"",
                    t.""TenantID"",
                    t.""TenantName"",
                    t.""Email"",
                    t.""TenantEnabled""
                FROM
                    ""User"" u JOIN
                    ""Tenant"" t ON t.""TenantID"" = u.""TenantID""
                WHERE
                    u.""UserID"" = @UserID;", cn);
            cmd.Parameters.Add("@UserID", NpgsqlDbType.Integer).Value = userId;
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return GetUser(r);
            throw new($"Invalid user specified (UserID = {userId}).");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

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
                    u.""UserID"",
                    u.""Username"",
                    u.""FullName"",
                    u.""Email"",
                    u.""Role"",
                    u.""UserEnabled"",
                    t.""TenantID"",
                    t.""TenantName"",
                    t.""Email"",
                    t.""TenantEnabled""
                FROM
                    ""User"" u JOIN
                    ""Tenant"" t ON t.""TenantID"" = u.""TenantID""
                WHERE
                    LOWER(u.""Username"") = LOWER(@Username) AND
                    LOWER(u.""Password"") = LOWER(@Password) AND
                    u.""UserEnabled"" = TRUE AND
                    t.""TenantEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@Username", NpgsqlDbType.Varchar, 128).Value = username;
            cmd.Parameters.Add("@Password", NpgsqlDbType.Varchar, 128).Value = HashPassword(password);
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return GetUser(r);
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
                    u.""UserID""
                FROM
                    ""User"" u JOIN
                    ""Tenant"" t ON t.""TenantID"" = u.""TenantID""
                WHERE
                    u.""UserID"" = @UserID AND
                    u.""UserEnabled"" = TRUE AND
                    t.""TenantEnabled"" = TRUE;", cn);
            cmd.Parameters.Add("@UserID", NpgsqlDbType.Integer).Value = userId;
            return cmd.ExecuteScalar() != null;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Change user password
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="password"></param>
    public void ChangeUserPassword(int userId, string password)
    {
        try
        {
            using var cn = GetConnection();
            using var cmd = GetCommand(
                @"UPDATE ""User"" SET
                    ""Password"" = @Password
                WHERE
                    ""UserID"" = @UserID;", cn);
            cmd.Parameters.Add("@UserID", NpgsqlDbType.Integer).Value = userId;
            cmd.Parameters.Add("@Password", NpgsqlDbType.Varchar, 128).Value = HashPassword(password);
            cmd.ExecuteNonQuery();
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
    /// Return tenant instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static Tenant GetTenant(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["TenantID"],
        Name = (string)reader["TenantName"],
        Email = (string)reader["Email"],
        Enabled = (bool)reader["TenantEnabled"]
    };

    /// <summary>
    /// Return user instance
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static User GetUser(NpgsqlDataReader reader) => new()
    {
        Id = (int)reader["UserID"],
        Tenant = GetTenant(reader),
        Name = (string)reader["Username"],
        FullName = (string)reader["FullName"],
        Email = (string)reader["Email"],
        Role = (string)reader["Role"],
        Enabled = (bool)reader["UserEnabled"]
    };

    /// <summary>
    /// Hash password
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private static string HashPassword(string password) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
    #endregion

}