using Devices.Common.Interfaces.Identification;
using Devices.Common.Models.Identification;
using System.Net.NetworkInformation;

namespace Devices.Common.Services.Identification;

/// <summary>
/// Host Name fingerprint service
/// </summary>
public class FingerprintServiceHost : IFingerprintService
{

    #region Public Methods
    /// <summary>
    /// Return device fingerprints
    /// </summary>
    /// <returns></returns>
    public List<Fingerprint> GetFingerprints()
    {
        var properties = IPGlobalProperties.GetIPGlobalProperties();
        return
        [
            new()
            {
                Type = FingerprintType.Host,
                Value = $"{properties.HostName}.{properties.DomainName}"
            }
        ];
    }
    #endregion

}