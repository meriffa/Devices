using Devices.Common.Interfaces.Configuration;
using Devices.Common.Interfaces.Identification;
using Devices.Common.Models.Configuration;
using Devices.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Devices.Common.Services.Configuration;

/// <summary>
/// Configuration service
/// </summary>
/// <param name="logger"></param>
/// <param name="options"></param>
/// <param name="identityService"></param>
public class ConfigurationService(ILogger<ConfigurationService> logger, IOptions<ClientOptions> options, IIdentityService identityService) : DeviceClientService(options.Value, identityService), IConfigurationService
{

    #region Private Fields
    private readonly ILogger<ConfigurationService> logger = logger;
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
            using var response = GetRequest("/Service/Configuration/GetPendingReleases", retry: true);
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
    /// Return required device releases
    /// </summary>
    /// <param name="applications"></param>
    /// <returns></returns>
    public List<Release> GetRequiredReleases(List<RequiredApplication> applications)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(applications), Encoding.UTF8, "application/json");
            using var response = PostRequest("/Service/Configuration/GetRequiredReleases", content, retry: true);
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
            using var response = GetRequest($"/Service/Configuration/GetReleasePackage?releaseId={releaseId}", retry: true);
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
                DeviceDate = DateTime.UtcNow,
                Release = release,
                Success = success,
                Details = details
            };
            var content = new StringContent(JsonSerializer.Serialize(deployment), Encoding.UTF8, "application/json");
            using var response = PostRequest("/Service/Configuration/SaveDeployment", content);
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