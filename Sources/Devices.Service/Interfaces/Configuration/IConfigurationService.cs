using Devices.Common.Models.Configuration;
using Devices.Service.Models.Configuration;

namespace Devices.Service.Interfaces.Configuration;

/// <summary>
/// Configuration service interface
/// </summary>
public interface IConfigurationService
{

    #region Public Methods
    /// <summary>
    /// Return applications
    /// </summary>
    /// <returns></returns>
    List<Application> GetApplications();

    /// <summary>
    /// Return releases
    /// </summary>
    /// <returns></returns>
    List<Release> GetReleases();

    /// <summary>
    /// Return pending device releases
    /// </summary>
    /// <param name="deviceId"></param>
    /// <returns></returns>
    List<Release> GetPendingReleases(string deviceId);

    /// <summary>
    /// Return release package
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    Stream GetReleasePackage(string deviceId, int releaseId);

    /// <summary>
    /// Return completed deployments
    /// </summary>
    /// <returns></returns>
    List<CompletedDeployment> GetCompletedDeployments();

    /// <summary>
    /// Return pending deployments
    /// </summary>
    /// <returns></returns>
    List<PendingDeployment> GetPendingDeployments();

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="deviceId"></param>
    /// <param name="deployment"></param>
    void SaveDeployment(string deviceId, Deployment deployment);
    #endregion

}