using Devices.Common.Models.Configuration;

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
    #endregion

}