using Devices.Client.Interfaces;
using Devices.Common.Models;
using System.Net.NetworkInformation;

namespace Devices.Client.Services;

/// <summary>
/// Fingerprint service using host name
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