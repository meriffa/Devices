using Devices.Common.Interfaces.Identification;
using Devices.Common.Models.Identification;
using Devices.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
    /// <param name="refresh"></param>
    /// <returns></returns>
    public string GetDeviceBearerToken(bool refresh = false)
    {
        try
        {
            string path = Path.Combine(Options.ConfigurationFolder, GetIdentityFile());
            if (refresh || !File.Exists(path))
            {
                var content = new StringContent(JsonSerializer.Serialize(GetFingerprints()), Encoding.UTF8, "application/json");
                using var response = Client.PostAsync("/Service/Identity/GetDeviceBearerToken", content).Result;
                response.EnsureSuccessStatusCode();
                SaveIdentity(Options.ConfigurationFolder, path, response.Content.ReadAsStringAsync().Result!);
            }
            return LoadIdentity(path);
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
    /// <returns></returns>
    private static string GetIdentityFile() => $"{Assembly.GetExecutingAssembly().GetName().Name}.DeviceBearerToken";

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
    private static string LoadIdentity(string path)
    {
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Save device identity
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="path"></param>
    /// <param name="deviceId"></param>
    private static void SaveIdentity(string folder, string path, string deviceId)
    {
        if (!Path.Exists(folder))
            Directory.CreateDirectory(folder);
        File.WriteAllText(path, deviceId);
    }
    #endregion

}