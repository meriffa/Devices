using System.Diagnostics;
using System.Reflection;

namespace Devices.Common.Services;

/// <summary>
/// Assembly service
/// </summary>
public static class AssemblyService
{

    #region Public Methods
    /// <summary>
    /// Return assembly build version
    /// </summary>
    /// <returns></returns>
    public static string? GetBuildVersion() => FileVersionInfo.GetVersionInfo(Assembly.GetCallingAssembly().Location).FileVersion;
    #endregion

}