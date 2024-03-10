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
    /// User full name
    /// </summary>
    public required string FullName { get; set; }

    /// <summary>
    /// User email
    /// </summary>
    public required string Email { get; set; }

    /// <summary>
    /// User role
    /// </summary>
    public required string Role { get; set; }
    #endregion

}