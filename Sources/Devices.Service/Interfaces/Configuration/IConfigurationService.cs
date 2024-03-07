using Devices.Common.Models.Configuration;
using Devices.Common.Models.Identification;

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
    /// <param name="identity"></param>
    /// <returns></returns>
    List<Release> GetPendingReleases(Identity identity);

    /// <summary>
    /// Return release package
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    Stream GetReleasePackage(Identity identity, int releaseId);

    /// <summary>
    /// Return deployments
    /// </summary>
    /// <returns></returns>
    List<Deployment> GetDeployments();

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="deployment"></param>
    void SaveDeployment(Deployment deployment);
    #endregion

}