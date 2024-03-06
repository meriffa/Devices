namespace Devices.Common.Models.Configuration;

/// <summary>
/// Action
/// </summary>
public class Action
{

    #region Properties
    /// <summary>
    /// Action id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Action type
    /// </summary>
    public required ActionType Type { get; set; }

    /// <summary>
    /// Action parameters
    /// </summary>
    public required string Parameters { get; set; }
    #endregion

}