namespace Devices.Service.Models.Security;

/// <summary>
/// User
/// </summary>
public class User
{

    #region Properties
    /// <summary>
    /// User id
    /// </summary>
    public required int Id { get; set; }

    /// <summary>
    /// User tenant
    /// </summary>
    public required Tenant Tenant { get; set; }

    /// <summary>
    /// User name
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// User full name
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User roles
    /// </summary>
    public required string[] Roles { get; set; }

    /// <summary>
    /// User enabled flag
    /// </summary>
    public required bool Enabled { get; set; }
    #endregion

}