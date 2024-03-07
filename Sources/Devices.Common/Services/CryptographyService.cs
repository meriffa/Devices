using System.Security.Cryptography;

namespace Devices.Common.Services;

/// <summary>
/// Cryptography service
/// </summary>
public static class CryptographyService
{

    #region Public Methods
    /// <summary>
    /// Return SHA256 hash
    /// </summary>
    /// <param name="buffer"></param>
    /// <returns></returns>
    public static string GetHash(byte[] buffer) => Convert.ToHexString(SHA256.HashData(buffer));

    /// <summary>
    /// Return SHA256 hash
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public static string GetHash(string fileName)
    {
        using var stream = File.OpenRead(fileName);
        return Convert.ToHexString(SHA256.HashData(stream));
    }
    #endregion

}