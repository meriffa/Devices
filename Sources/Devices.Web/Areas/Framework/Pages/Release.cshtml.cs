using Devices.Common.Models.Configuration;
using Devices.Service.Interfaces.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace Devices.Web.Areas.Framework.Pages;

/// <summary>
/// Release page
/// </summary>
/// <param name="logger"></param>
public class ReleaseModel(ILogger<ReleaseModel> logger, IConfigurationService service) : PageModel
{

    #region Private Fields
    private readonly ILogger<ReleaseModel> logger = logger;
    private readonly IConfigurationService service = service;
    #endregion

    #region Properties
    /// <summary>
    /// Page title
    /// </summary>
    public string Title { get; set; } = null!;

    /// <summary>
    /// Release id
    /// </summary>
    [FromQuery(Name = "releaseId")]
    public int? ReleaseId { get; set; }

    /// <summary>
    /// Release application id
    /// </summary>
    [BindProperty]
    public int ApplicationId { get; set; }

    /// <summary>
    /// Release applications
    /// </summary>
    public List<SelectListItem> Applications { get => service.GetApplications().Select(i => new SelectListItem(i.Name, i.Id.ToString())).ToList(); }

    /// <summary>
    /// Release package
    /// </summary>
    [BindProperty]
    public IFormFile Package { get; set; } = null!;

    /// <summary>
    /// Release version
    /// </summary>
    [BindProperty]
    public string Version { get; set; } = null!;

    /// <summary>
    /// Release action id
    /// </summary>
    [BindProperty]
    public int ActionId { get; set; }

    /// <summary>
    /// Release actions
    /// </summary>
    public List<SelectListItem> Actions { get => service.GetActions().Select(i => new SelectListItem($"{i.Parameters} {i.Arguments}", i.Id.ToString())).ToList(); }

    /// <summary>
    /// Release enabled flag
    /// </summary>
    [BindProperty]
    public bool Enabled { get; set; }

    /// <summary>
    /// Release allow concurrency flag
    /// </summary>
    [BindProperty]
    public bool AllowConcurrency { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    [TempData]
    public string? ErrorMessage { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Get request
    /// </summary>
    public void OnGet()
    {
        if (ReleaseId == null)
            Title = "New Release";
        else
        {
            Title = "Edit Release";
            var release = service.GetRelease(ReleaseId.Value);
            ApplicationId = release.Application.Id;
            Version = release.Version;
            ActionId = release.Action.Id;
            Enabled = release.Enabled;
            AllowConcurrency = release.AllowConcurrency;
        }
        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);
    }

    /// <summary>
    /// Post request
    /// </summary>
    /// <returns></returns>
    public IActionResult OnPost()
    {
        if (ModelState.IsValid)
            try
            {
                logger.LogInformation("Release saved (ID = {id}).", service.SaveRelease(new()
                {
                    Id = ReleaseId ?? 0,
                    ServiceDate = DateTime.UtcNow,
                    Application = new() { Id = ApplicationId, Name = "", Enabled = true, RequiredApplications = [] },
                    Package = Path.GetFileName(Package.FileName),
                    PackageHash = service.SaveReleasePackage(Package),
                    Version = Version,
                    Action = new() { Id = ActionId, Type = ActionType.Script, Parameters = "" },
                    Enabled = Enabled,
                    AllowConcurrency = AllowConcurrency
                }).Id);
                return RedirectToPage("Releases", new { area = "Framework" });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
        return Page();
    }
    #endregion

}