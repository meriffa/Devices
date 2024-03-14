namespace Devices.Common.Models.Configuration;

/// <summary>
/// Application
/// </summary>
public class Application
{

    #region Properties
    /// <summary>
    /// Application id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Application name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Application enabled flag
    /// </summary>
    public required bool Enabled { get; set; }
    #endregion

}