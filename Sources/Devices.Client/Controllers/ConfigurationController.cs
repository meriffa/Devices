using CommandLine;
using Devices.Common.Models.Configuration;
using Devices.Common.Services;
using System.Diagnostics;
using System.IO.Compression;

namespace Devices.Client.Controllers;

/// <summary>
/// Configuration controller
/// </summary>
[Verb("Configuration", HelpText = "Configuration operation.")]
public class ConfigurationController : Controller
{

    #region Public Methods
    /// <summary>
    /// Execute controller
    /// </summary>
    protected override void Execute()
    {
        DisplayService.WriteInformation("Configuration operation started.");
        foreach (var release in ConfigurationService.GetPendingReleases())
            DeployRelease(release);
        DisplayService.WriteInformation("Configuration operation completed.");
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Deploy release
    /// </summary>
    /// <param name="release"></param>
    private void DeployRelease(Release release)
    {
        try
        {
            DisplayService.WriteInformation($"Release deployment started (Release ID = {release.Id}).");
            var folder = CreatePackageFolder(release.Package);
            var package = DownloadPackage(release, folder);
            ExtractPackage(package, folder);
            (bool success, string details) = ExecuteAction(folder, release.Action);
            ConfigurationService.SaveDeployment(release, success, details);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
            if (success)
                DisplayService.WriteInformation($"Release deployment completed (Release ID = {release.Id}).");
            else
                DisplayService.WriteWarning($"Release deployment failed (Release ID = {release.Id}, Details = '{details}').");
        }
        catch (Exception ex)
        {
            DisplayService.WriteError($"Release deployment failed (Release ID = {release.Id}, Error = '{ex.Message}').");
        }
    }

    /// <summary>
    /// Create package folder
    /// </summary>
    /// <param name="package"></param>
    /// <returns></returns>
    private string CreatePackageFolder(string package)
    {
        var path = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(package));
        if (Directory.Exists(path))
            Directory.Delete(path, true);
        Directory.CreateDirectory(path);
        DisplayService.WriteInformation($"Package folder created (Folder = '{path}').");
        return path;
    }

    /// <summary>
    /// Download package
    /// </summary>
    /// <param name="release"></param>
    /// <param name="folder"></param>
    /// <returns></returns>
    private string DownloadPackage(Release release, string folder)
    {
        var path = Path.Combine(folder, release.Package);
        ConfigurationService.DownloadReleasePackage(release.Id, path);
        DisplayService.WriteInformation($"Package downloaded (File = '{path}').");
        if (release.PackageHash != null && !release.PackageHash.Equals(CryptographyService.GetHash(path), StringComparison.InvariantCultureIgnoreCase))
            throw new($"Package '{release.Package}' integrity verification failed.");
        return path;
    }

    /// <summary>
    /// Extract package
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="folder"></param>
    private void ExtractPackage(string fileName, string folder)
    {
        ZipFile.ExtractToDirectory(fileName, folder);
        File.Delete(fileName);
        DisplayService.WriteInformation($"Package extracted (File = '{fileName}', Folder = '{folder}').");
    }

    /// <summary>
    /// Execute release action
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    private (bool success, string details) ExecuteAction(string folder, Common.Models.Configuration.Action action)
    {
        (var success, var details) = action.Type switch
        {
            ActionType.Script => ExecuteActionScript(folder, action.Parameters, action.Arguments),
            _ => throw new($"Action type '{action.Type}' is not supported.")
        };
        DisplayService.WriteInformation($"Release action completed (Type = '{action.Type}', Parameters = '{action.Parameters}').");
        return (success, details);
    }

    /// <summary>
    /// Execute release script action
    /// </summary>
    /// <param name="folder"></param>
    /// <param name="fileName"></param>
    /// <param name="arguments"></param>
    /// <returns></returns>
    private static (bool success, string details) ExecuteActionScript(string folder, string fileName, string? arguments)
    {
        var path = Path.Combine(folder, fileName);
        using var process = Process.Start(new ProcessStartInfo(path) { WorkingDirectory = folder, RedirectStandardOutput = true, Arguments = arguments ?? string.Empty });
        process!.WaitForExit();
        return (process.ExitCode == 0, process.StandardOutput.ReadToEnd().Trim());
    }
    #endregion
}