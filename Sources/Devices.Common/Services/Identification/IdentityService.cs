using Devices.Common.Interfaces.Identification;
using Devices.Common.Models.Identification;
using Devices.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Devices.Common.Services.Identification;

/// <summary>
/// Identity service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// <param name="fingerprintServices"></param>
public class IdentityService(ILogger<IdentityService> logger, IOptions<ClientOptions> options, IEnumerable<IFingerprintService> fingerprintServices) : ClientService(options.Value), IIdentityService
{

    #region Private Fields
    private readonly ILogger<IdentityService> logger = logger;
    private readonly IEnumerable<IFingerprintService> fingerprintServices = fingerprintServices;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return device bearer token
    /// </summary>
    /// <returns></returns>
    public string GetDeviceBearerToken()
    {
        try
        {
            var path = GetIdentityFile(Options.ConfigurationFolder);
            if (LoadIdentity(path) is string identity)
            {
                AddDeviceAuthorization(identity);
                using var response = Client.GetAsync("/Service/Identity/ValidateDeviceBearerToken").Result;
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    return GetDeviceBearerToken(path);
                response.EnsureSuccessStatusCode();
                return identity;
            }
            return GetDeviceBearerToken(path);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return device identity file
    /// </summary>
    /// <param name="folder"></param>
    /// <returns></returns>
    private static string GetIdentityFile(string folder) => Path.Combine(folder, $"{Assembly.GetExecutingAssembly().GetName().Name}.DeviceBearerToken");

    /// <summary>
    /// Return device fingerprints
    /// </summary>
    /// <returns></returns>
    private List<Fingerprint> GetFingerprints()
    {
        var fingerprints = new List<Fingerprint>();
        foreach (var fingerprintService in fingerprintServices)
            fingerprints.AddRange(fingerprintService.GetFingerprints());
        return fingerprints;
    }

    /// <summary>
    /// Load device identity
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static string? LoadIdentity(string path) => File.Exists(path) ? File.ReadAllText(path) : null;

    /// <summary>
    /// Save device identity
    /// </summary>
    /// <param name="path"></param>
    /// <param name="identity"></param>
    /// <returns></returns>
    private static string SaveIdentity(string path, string identity)
    {
        var folder = Path.GetDirectoryName(path);
        if (!Path.Exists(folder))
            Directory.CreateDirectory(folder!);
        File.WriteAllText(path, identity);
        return identity;
    }

    /// <summary>
    /// Return device bearer token
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private string GetDeviceBearerToken(string path)
    {
        var content = new StringContent(JsonSerializer.Serialize(GetFingerprints()), Encoding.UTF8, "application/json");
        using var response = Client.PostAsync("/Service/Identity/GetDeviceBearerToken", content).Result;
        response.EnsureSuccessStatusCode();
        return SaveIdentity(path, response.Content.ReadAsStringAsync().Result!);
    }
    #endregion

}