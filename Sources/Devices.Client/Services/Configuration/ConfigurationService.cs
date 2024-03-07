using Devices.Client.Interfaces.Configuration;
using Devices.Client.Interfaces.Identification;
using Devices.Common.Models.Configuration;
using Devices.Common.Options;
using Devices.Common.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Devices.Client.Services.Configuration;

/// <summary>
/// Configuration service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// <param name="identityService"></param>
public class ConfigurationService(ILogger<ConfigurationService> logger, IOptions<ClientOptions> options, IIdentityService identityService) : ClientService(options.Value), IConfigurationService
{

    #region Private Fields
    private readonly ILogger<ConfigurationService> logger = logger;
    private readonly IIdentityService identityService = identityService;
    #endregion

    #region Public Methods
    /// <summary>
    /// Return pending device releases
    /// </summary>
    /// <returns></returns>
    public List<Release> GetPendingReleases()
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(identityService.GetIdentity()), Encoding.UTF8, "application/json");
            using var response = Client.PostAsync("/Service/Configuration/GetPendingReleases", content).Result;
            response.EnsureSuccessStatusCode();
            return response.Content.ReadFromJsonAsync<List<Release>>().Result!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Download release package
    /// </summary>
    /// <param name="releaseId"></param>
    /// <param name="fileName"></param>
    public void DownloadReleasePackage(int releaseId, string fileName)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(identityService.GetIdentity()), Encoding.UTF8, "application/json");
            using var response = Client.PostAsync($"/Service/Configuration/GetReleasePackage?releaseId={releaseId}", content).Result;
            response.EnsureSuccessStatusCode();
            using var stream = File.Create(fileName);
            response.Content.ReadAsStream().CopyTo(stream);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="release"></param>
    /// <param name="success"></param>
    /// <param name="details"></param>
    public void SaveDeployment(Release release, bool success, string? details)
    {
        try
        {
            var deployment = new Deployment()
            {
                Id = 0,
                Date = DateTime.UtcNow,
                Device = identityService.GetIdentity(),
                Release = release,
                Success = success,
                Details = details
            };
            var content = new StringContent(JsonSerializer.Serialize(deployment), Encoding.UTF8, "application/json");
            using var response = Client.PostAsync("/Service/Configuration/SaveDeployment", content).Result;
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

}