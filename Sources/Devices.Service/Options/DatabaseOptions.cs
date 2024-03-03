namespace Devices.Service.Options;

/// <summary>
/// Database options
/// </summary>
public class DatabaseOptions
{

    #region Properties
    /// <summary>
    /// Database options host
    /// </summary>
    public required string Host { get; set; }

    /// <summary>
    /// Database options name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Database options username
    /// </summary>
    public required string Username { get; set; }

    /// <summary>
    /// Database options password
    /// </summary>
    public required string Password { get; set; }

    /// <summary>
    /// Database options command timeout (in seconds)
    /// </summary>
    public int CommandTimeout { get; set; } = 30;
    #endregion

}