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
    /// <param name="device"></param>
    /// <returns></returns>
    List<Release> GetPendingReleases(Device device);

    /// <summary>
    /// Return release package
    /// </summary>
    /// <param name="device"></param>
    /// <param name="releaseId"></param>
    /// <returns></returns>
    Stream GetReleasePackage(Device device, int releaseId);

    /// <summary>
    /// Return deployments
    /// </summary>
    /// <returns></returns>
    List<Deployment> GetDeployments();

    /// <summary>
    /// Return pending deployments
    /// </summary>
    /// <returns></returns>
    List<PendingDeployment> GetPendingDeployments();

    /// <summary>
    /// Save deployment
    /// </summary>
    /// <param name="deployment"></param>
    void SaveDeployment(Deployment deployment);
    #endregion

}