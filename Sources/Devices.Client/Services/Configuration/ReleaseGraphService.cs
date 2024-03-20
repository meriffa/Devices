using Devices.Client.Interfaces.Configuration;
using Devices.Common.Models.Configuration;
using Devices.Common.Options;
using Devices.Common.Services;
using Devices.Common.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Compression;
using System.Text;

namespace Devices.Client.Services.Configuration;

/// <summary>
/// Release graph service
/// </summary>
/// <param name="logger"></param>
/// <param name="displayService"></param>
/// <param name="configurationService"></param>
public class ReleaseGraphService(ILogger<ReleaseGraphService> logger, IOptions<ClientOptions> options, DisplayService displayService, IConfigurationService configurationService)
{

    #region Private Fields
    private readonly ILogger<ReleaseGraphService> logger = logger;
    private readonly LimitedConcurrencyTaskScheduler taskScheduler = new(options.Value.ParallelReleases);
    private List<ReleaseNode> Nodes = null!;
    private List<ReleaseNode> SourceNodes = null!;
    #endregion

    #region Public Methods
    /// <summary>
    /// Build release graph
    /// </summary>
    public void Build()
    {
        try
        {
            Nodes = GetNodes(configurationService.GetPendingReleases());
            FilterNodes();
            AddNodeLinks();
            SourceNodes = GetSourceNodes();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Validate release graph
    /// </summary>
    public void Validate()
    {
        try
        {
            ValidateNodes();
            ValidateNodeLinks();
            ValidateSourceNodes();
            ValidateNodesForCircularReferences();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Execute release graph
    /// </summary>
    public void Execute()
    {
        try
        {
            var activeNodes = StartSourceNodes();
            var completedNodes = new List<Task<ReleaseNode>>();
            var pendingNodes = new List<ReleaseNode>();
            while (activeNodes.Count != 0)
            {
                var task = Task.WhenAny(activeNodes).Result;
                activeNodes.Remove(task);
                completedNodes.Add(task);
                UpdatePendingNodes(task.Result, pendingNodes);
                UpdateActiveNodes(activeNodes, completedNodes, pendingNodes);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
    }
    #endregion

    #region Build Graph
    /// <summary>
    /// Return release nodes
    /// </summary>
    /// <param name="releases"></param>
    /// <returns></returns>
    private static List<ReleaseNode> GetNodes(List<Release> releases) => releases.Select(i => new ReleaseNode()
    {
        Release = i,
        UpstreamNodes = [],
        DownstreamNodes = [],
        Success = false
    }).ToList();

    /// <summary>
    /// Filter completed nodes
    /// </summary>
    private void FilterNodes()
    {
        var parentReleaseIds = Nodes.Where(i => i.Release.ParentReleaseIds.Length > 0).SelectMany(i => i.Release.ParentReleaseIds).Distinct();
        var completedReleaseIds = parentReleaseIds.Except(Nodes.Select(i => i.Release.Id)).ToArray();
        foreach (var releaseId in completedReleaseIds)
            if (configurationService.HasReleaseSucceeded(releaseId))
            {
                foreach (var node in Nodes)
                    if (node.Release.ParentReleaseIds.Contains(releaseId))
                        node.Release.ParentReleaseIds = node.Release.ParentReleaseIds.Where(i => i != releaseId).ToArray();
            }
            else
                RemoveChildNodes(releaseId);
    }

    /// <summary>
    /// Remove child release nodes
    /// </summary>
    /// <param name="parentReleaseId"></param>
    private void RemoveChildNodes(int parentReleaseId)
    {
        var childReleaseIds = Nodes.Where(i => i.Release.ParentReleaseIds.Contains(parentReleaseId)).Select(i => i.Release.Id).ToArray();
        foreach (var childReleaseId in childReleaseIds)
        {
            RemoveChildNodes(childReleaseId);
            var node = Nodes.FirstOrDefault(i => i.Release.Id == childReleaseId);
            if (node != null)
                Nodes.Remove(node);
        }
    }

    /// <summary>
    /// Add release node links
    /// </summary>
    private void AddNodeLinks()
    {
        foreach (var node in Nodes.Where(i => i.Release.ParentReleaseIds.Length > 0))
            foreach (var parentReleaseId in node.Release.ParentReleaseIds)
            {
                var parent = Nodes.First(i => i.Release.Id == parentReleaseId);
                parent.DownstreamNodes.Add(node);
                node.UpstreamNodes.Add(parent);
            }
    }

    /// <summary>
    /// Return source release nodes
    /// </summary>
    /// <returns></returns>
    private List<ReleaseNode> GetSourceNodes() => Nodes.Where(node => node.UpstreamNodes.Count == 0).ToList();
    #endregion

    #region Validate Graph
    /// <summary>
    /// Validate release nodes
    /// </summary>
    private void ValidateNodes()
    {
        foreach (var id in Nodes.GroupBy(node => node.Release.Id).Where(i => i.Count() > 1).Select(i => i.Key))
            throw new($"Duplicate release nodes specified (Release ID = {id}).");
    }

    /// <summary>
    /// Validate release node links
    /// </summary>
    private void ValidateNodeLinks()
    {
        foreach (var node in Nodes)
        {
            foreach (var downstreamNode in node.DownstreamNodes)
                if (node.Release.Id == downstreamNode.Release.Id)
                    throw new($"Invalid release node self reference specified (Release ID = {node.Release.Id}).");
            foreach (var upstreamNode in node.UpstreamNodes)
                if (node.Release.Id == upstreamNode.Release.Id)
                    throw new($"Invalid release node self reference specified (Release ID = {node.Release.Id}).");
        }
    }

    /// <summary>
    /// Validate source nodes
    /// </summary>
    private void ValidateSourceNodes()
    {
        if (Nodes.Count > 0 && SourceNodes.Count == 0)
            throw new("No release source nodes specified.");
    }

    /// <summary>
    /// Check release nodes for circular references
    /// </summary>
    private void ValidateNodesForCircularReferences()
    {
        foreach (var node in Nodes)
        {
            var visited = new HashSet<int>();
            TraverseDownstreamNodes(node, visited);
            if (visited.Contains(node.Release.Id))
                throw new($"Release node is part of circular reference path (Release ID = {node.Release.Id}).");
        }
    }

    /// <summary>
    /// Traverses all downstream release nodes
    /// </summary>
    /// <param name="node"></param>
    /// <param name="visited"></param>
    private static void TraverseDownstreamNodes(ReleaseNode node, HashSet<int> visited)
    {
        foreach (var downstreamNode in node.DownstreamNodes)
            if (!visited.Contains(downstreamNode.Release.Id))
            {
                visited.Add(downstreamNode.Release.Id);
                TraverseDownstreamNodes(downstreamNode, visited);
            }
    }
    #endregion

    #region Execute Graph
    /// <summary>
    /// Start source release nodes
    /// </summary>
    /// <param name="sourceNodes"></param>
    /// <returns></returns>
    private List<Task<ReleaseNode>> StartSourceNodes() => SourceNodes.Select(i => StartNode(i)).ToList();

    /// <summary>
    /// Update pending release nodes
    /// </summary>
    /// <param name="node"></param>
    /// <param name="pendingNodes"></param>
    private static void UpdatePendingNodes(ReleaseNode node, List<ReleaseNode> pendingNodes)
    {
        foreach (var downstreamNode in node.DownstreamNodes)
            if (node.Success)
            {
                if (!pendingNodes.Exists(i => i.Release.Id == downstreamNode.Release.Id))
                    pendingNodes.Add(downstreamNode);
            }
    }

    /// <summary>
    /// Update active release nodes
    /// </summary>
    /// <param name="activeNodes"></param>
    /// <param name="completedNodes"></param>
    /// <param name="pendingNodes"></param>
    private void UpdateActiveNodes(List<Task<ReleaseNode>> activeNodes, List<Task<ReleaseNode>> completedNodes, List<ReleaseNode> pendingNodes)
    {
        foreach (var node in pendingNodes.ToArray())
            if (IsNodeReadyToStart(node, completedNodes))
            {
                pendingNodes.Remove(node);
                activeNodes.Add(StartNode(node));
            }
    }

    /// <summary>
    /// Check release node is ready to start
    /// </summary>
    /// <param name="task"></param>
    /// <param name="completedNodes"></param>
    /// <returns></returns>
    private static bool IsNodeReadyToStart(ReleaseNode task, List<Task<ReleaseNode>> completedNodes)
    {
        if (task.UpstreamNodes.Count > 0)
        {
            if (task.UpstreamNodes.Exists(i => !i.Success))
                return false;
            foreach (var node in task.UpstreamNodes)
                if (!completedNodes.Exists(i => i.Result.Release.Id == node.Release.Id))
                    return false;
        }
        return true;
    }

    /// <summary>
    /// Start release node
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private Task<ReleaseNode> StartNode(ReleaseNode node) => Task.Factory.StartNew(() => DeployRelease(node), CancellationToken.None, TaskCreationOptions.AttachedToParent, taskScheduler);

    /// <summary>
    /// Deploy release
    /// </summary>
    /// <param name="node"></param>
    private ReleaseNode DeployRelease(ReleaseNode node)
    {
        try
        {
            displayService.WriteInformation($"Release deployment started (Release ID = {node.Release.Id}).");
            var folder = CreatePackageFolder(node.Release.Package);
            var package = DownloadPackage(node.Release, folder);
            ExtractPackage(package, folder);
            (node.Success, string details) = ExecuteAction(folder, node.Release.Action);
            if (Directory.Exists(folder))
                Directory.Delete(folder, true);
            configurationService.SaveDeployment(node.Release, node.Success, details);
            if (node.Success)
                displayService.WriteInformation($"Release deployment completed (Release ID = {node.Release.Id}).");
            else
                displayService.WriteWarning($"Release deployment failed (Release ID = {node.Release.Id}, Details = '{details}').");
        }
        catch (Exception ex)
        {
            node.Success = false;
            displayService.WriteError($"Release deployment failed (Release ID = {node.Release.Id}, Error = '{ex.Message}').");
        }
        return node;
    }
    #endregion

    #region Deploy Release
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
        displayService.WriteInformation($"Package folder created (Folder = '{path}').");
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
        configurationService.DownloadReleasePackage(release.Id, path);
        displayService.WriteInformation($"Package downloaded (File = '{path}').");
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
        displayService.WriteInformation($"Package extracted (File = '{fileName}', Folder = '{folder}').");
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
        displayService.WriteInformation($"Release action completed (Type = '{action.Type}', Parameters = '{action.Parameters}', Success = {success}, Details = '{details}').");
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
        using var process = Process.Start(new ProcessStartInfo(path) { WorkingDirectory = folder, RedirectStandardOutput = true, RedirectStandardError = true, Arguments = arguments ?? string.Empty });
        process!.WaitForExit();
        var details = new StringBuilder();
        details.Append(process.StandardOutput.ReadToEnd().Trim());
        var error = process.StandardError.ReadToEnd().Trim();
        if (!string.IsNullOrEmpty(error))
            details.AppendLine().AppendLine("ERROR:").Append(error);
        return (process.ExitCode == 0, details.ToString());
    }
    #endregion

}