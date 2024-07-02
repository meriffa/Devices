namespace Devices.Service.Models.Security;

/// <summary>
/// Tenant
/// </summary>
public class Tenant
{

    #region Properties
    /// <summary>
    /// Tenant id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// Tenant name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Tenant email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// Tenant enabled flag
    /// </summary>
    public required bool Enabled { get; set; }
    #endregion

}