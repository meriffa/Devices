using Devices.Client.Interfaces;
using Devices.Common.Models;

namespace Devices.Client.Services;

/// <summary>
/// Fingerprint service using SSH public keys
/// </summary>
public class FingerprintServiceSSH : IFingerprintService
{

    #region Public Methods
    /// <summary>
    /// Return device fingerprints
    /// </summary>
    /// <returns></returns>
    public List<Fingerprint> GetFingerprints()
    {
        if (Directory.Exists("/etc/ssh"))
            return Directory.GetFiles("/etc/ssh", "ssh_host_*_key.pub").Select(i => new Fingerprint()
            {
                Type = FingerprintType.SSH,
                Value = File.ReadAllText(i).Trim()
            }).ToList();
        return [];
    }
    #endregion

}