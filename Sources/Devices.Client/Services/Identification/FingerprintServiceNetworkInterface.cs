using Devices.Client.Interfaces.Identification;
using Devices.Common.Models.Identification;
using System.Net.NetworkInformation;

namespace Devices.Client.Services.Identification;

/// <summary>
/// Network Interface MAC addresses fingerprint service
/// </summary>
public class FingerprintServiceNetworkInterface : IFingerprintService
{

    #region Public Methods
    /// <summary>
    /// Return device fingerprints
    /// </summary>
    /// <returns></returns>
    public List<Fingerprint> GetFingerprints()
    {
        return NetworkInterface.GetAllNetworkInterfaces().Where(i => i.NetworkInterfaceType != NetworkInterfaceType.Loopback).Select(i => new Fingerprint()
        {
            Type = FingerprintType.NetworkInterface,
            Value = $"{i.NetworkInterfaceType}:{Convert.ToHexString(i.GetPhysicalAddress().GetAddressBytes())}"
        }).ToList();
    }
    #endregion

}