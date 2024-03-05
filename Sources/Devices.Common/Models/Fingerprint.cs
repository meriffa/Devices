namespace Devices.Common.Models;

/// <summary>
/// Fingerprint
/// </summary>
public class Fingerprint
{

    #region Properties
    /// <summary>
    /// Fingerprint type
    /// </summary>
    public required FingerprintType Type { get; set; }

    /// <summary>
    /// Fingerprint value
    /// </summary>
    public required string Value { get; set; }
    #endregion

}