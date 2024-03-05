using Devices.Client.Interfaces;
using Devices.Common.Models;
using Devices.Common.Options;
using Devices.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Devices.Client.Services;

/// <summary>
/// Identity service client
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// <param name="fingerprintServices"></param>
public class IdentityServiceClient(ILogger<IdentityServiceClient> logger, IOptions<ClientOptions> options, IEnumerable<IFingerprintService> fingerprintServices) : ServiceClient(options.Value), IIdentityServiceClient
{

    #region Constants
    private const string IDENTITY_FILE = "Devices.Client.Identity.json";
    #endregion

    #region Private Fields
    private readonly ILogger<IdentityServiceClient> logger = logger;
    private readonly IEnumerable<IFingerprintService> fingerprintServices = fingerprintServices;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return device identity
    /// </summary>
    /// <param name="refresh"></param>
    /// <returns></returns>
    public Identity GetIdentity(bool refresh)
    {
        try
        {
            string path = Path.Combine(Options.ConfigurationFolder, IDENTITY_FILE);
            if (refresh || !File.Exists(path))
            {
                var content = new StringContent(JsonSerializer.Serialize(GetFingerprints()), Encoding.UTF8, "application/json");
                HttpResponseMessage response = Client.PostAsync($"/Service/Identity/GetIdentity", content).Result;
                response.EnsureSuccessStatusCode();
                SaveIdentity(Options.ConfigurationFolder, path, response.Content.ReadFromJsonAsync<Identity>().Result!);
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
    /// Load identity
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static Identity LoadIdentity(string path)
    {
        return JsonSerializer.Deserialize<Identity>(File.ReadAllText(path))!;
    }

    /// <summary>
    /// Save identity
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="path"></param>
    /// <param name="identity"></param>
    private static void SaveIdentity(string folder, string path, Identity identity)
    {
        if (!Path.Exists(folder))
            Directory.CreateDirectory(folder);
        File.WriteAllText(path, JsonSerializer.Serialize(identity));
    }
    #endregion

}