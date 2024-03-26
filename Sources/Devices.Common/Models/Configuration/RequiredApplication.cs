namespace Devices.Common.Models.Configuration;

/// <summary>
/// Required application
/// </summary>
public class RequiredApplication
{

    #region Properties
    /// <summary>
    /// Required application
    /// </summary>
    public required Application Application { get; set; }

    /// <summary>
    /// Required application minimum release version
    /// </summary>
    public required string? MinimumVersion { get; set; }
    #endregion

}