using Devices.Service.Options;
using Npgsql;

namespace Devices.Service.Services;

/// <summary>
/// PostgreSQL data service
/// </summary>
/// <param name="options"></param>
public abstract class DataService(DatabaseOptions options)
{

    #region Private Fields
    private readonly DatabaseOptions options = options;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Return connection
    /// </summary>
    /// <returns></returns>
    protected NpgsqlConnection GetConnection()
    {
        var cn = new NpgsqlConnection($"Host={options.Host};Database={options.Name};Username={options.Username};Password={options.Password};");
        cn.Open();
        return cn;
    }

    /// <summary>
    /// Return command
    /// </summary>
    /// <param name="text"></param>
    /// <param name="connection"></param>
    /// <returns></returns>
    protected NpgsqlCommand GetCommand(string text, NpgsqlConnection connection)
    {
        return new NpgsqlCommand(text, connection) { CommandTimeout = options.CommandTimeout };
    }
    #endregion

}