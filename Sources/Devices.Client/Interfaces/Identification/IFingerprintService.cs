using Devices.Common.Models.Identification;

namespace Devices.Client.Interfaces.Identification;

/// <summary>
/// Fingerprint service interface
/// </summary>
public interface IFingerprintService
{

    #region Public Methods
    /// <summary>
    /// Return device fingerprints
    /// </summary>
    /// <returns></returns>
    List<Fingerprint> GetFingerprints();
    #endregion

}