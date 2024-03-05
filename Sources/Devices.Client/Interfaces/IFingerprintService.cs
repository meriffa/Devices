using Devices.Common.Models;

namespace Devices.Client.Interfaces;

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