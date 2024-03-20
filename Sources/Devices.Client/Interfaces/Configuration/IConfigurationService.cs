using Devices.Common.Models.Configuration;

namespace Devices.Client.Interfaces.Configuration;

/// <summary>
/// Configuration service interface
/// </summary>
public interface IConfigurationService
{

    #region Public Methods
    /// <summary>
    /// Return pending device releases
    /// </summary>
    /// <returns></returns>
    List<Release> GetPendingReleases();

    /// <summary>
    /// Check if device release has completed successfully
    /// </summary>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    bool HasReleaseSucceeded(int releaseId);

    /// <summary>
    /// Download release package
    /// </summary>
    /// <param name="releaseId"></param>
    /// <param name="fileName"></param>
    void DownloadReleasePackage(int releaseId, string fileName);

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="release"></param>
    /// <param name="success"></param>
    /// <param name="details"></param>
    void SaveDeployment(Release release, bool success, string? details);
    #endregion

}