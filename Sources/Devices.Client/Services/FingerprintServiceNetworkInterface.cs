using Devices.Client.Interfaces;
using Devices.Common.Models;
using System.Net.NetworkInformation;

namespace Devices.Client.Services;

/// <summary>
/// Fingerprint service using network interface MAC addresses
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