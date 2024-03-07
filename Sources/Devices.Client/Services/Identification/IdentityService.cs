using Devices.Client.Interfaces.Identification;
using Devices.Common.Models.Identification;
using Devices.Common.Options;
using Devices.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Reflection;
using System.Text;
using System.Text.Json;

namespace Devices.Client.Services.Identification;

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
    /// Return device
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    public Device GetDevice(bool refresh = false)
    {
        try
        {
            string path = Path.Combine(Options.ConfigurationFolder, GetConfigurationFile());
            if (refresh || !File.Exists(path))
            {
                var content = new StringContent(JsonSerializer.Serialize(GetFingerprints()), Encoding.UTF8, "application/json");
                using var response = Client.PostAsync("/Service/Identity/GetDevice", content).Result;
                response.EnsureSuccessStatusCode();
                SaveDevice(Options.ConfigurationFolder, path, response.Content.ReadFromJsonAsync<Device>().Result!);
            }
            return LoadDevice(path);
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
    /// Return configuration file
    /// </summary>
    /// <returns></returns>
    private static string GetConfigurationFile() => $"{Assembly.GetExecutingAssembly().GetName().Name}.Device.json";

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
    /// Load device
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static Device LoadDevice(string path)
    {
        return JsonSerializer.Deserialize<Device>(File.ReadAllText(path))!;
    }

    /// <summary>
    /// Save device
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="path"></param>
    /// <param name="device"></param>
    private static void SaveDevice(string folder, string path, Device device)
    {
        if (!Path.Exists(folder))
            Directory.CreateDirectory(folder);
        File.WriteAllText(path, JsonSerializer.Serialize(device));
    }
    #endregion

}