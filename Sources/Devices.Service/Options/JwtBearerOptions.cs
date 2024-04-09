namespace Devices.Service.Options;

/// <summary>
/// JWT bearer options
/// </summary>
public class JwtBearerOptions
{

    #region Properties
    /// <summary>
    /// JWT bearer options issuer
    /// </summary>
    public required string Issuer { get; set; }

    /// <summary>
    /// JWT bearer options audience
    /// </summary>
    public required string Audience { get; set; }

    /// <summary>
    /// JWT bearer options signing key
    /// </summary>
    public required string SigningKey { get; set; }

    /// <summary>
    /// JWT bearer options expiration [minutes]
    /// </summary>
    public required int Expiration { get; set; }
    #endregion

}