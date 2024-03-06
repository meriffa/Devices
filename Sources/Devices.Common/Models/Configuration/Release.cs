namespace Devices.Common.Models.Configuration;

/// <summary>
/// Release
/// </summary>
public class Release
{

    #region Properties
    /// <summary>
    /// Release id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Release date & time
    /// </summary>
    public required DateTime Date { get; set; }

    /// <summary>
    /// Release application
    /// </summary>
    public required Application Application { get; set; }

    /// <summary>
    /// Release package
    /// </summary>
    public required string Package { get; set; }

    /// <summary>
    /// Release version
    /// </summary>
    public required string Version { get; set; }

    /// <summary>
    /// Release action
    /// </summary>
    public required Action Action { get; set; }

    /// <summary>
    /// Release active flag
    /// </summary>
    public required bool Active { get; set; }
    #endregion

}