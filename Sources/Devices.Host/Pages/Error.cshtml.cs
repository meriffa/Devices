using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace Devices.Host.Pages;

/// <summary>
/// Error page
/// </summary>
/// <param name="logger"></param>
[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true), IgnoreAntiforgeryToken]
public class ErrorModel(ILogger<ErrorModel> logger) : PageModel
{

    #region Private Fields
    private readonly ILogger<ErrorModel> logger = logger;
    #endregion

    #region Properties
    /// <summary>
    /// Request ID
    /// </summary>
    public string? RequestId { get; set; }

    /// <summary>
    /// Exception message
    /// </summary>
    public string? ExceptionMessage { get; set; }

    /// <summary>
    /// Exception path
    /// </summary>
    public string? ExceptionPath { get; set; }

    /// <summary>
    /// Show Request ID flag
    /// </summary>
    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    #endregion

    #region Public Methods
    /// <summary>
    /// Get request
    /// </summary>
    public void OnGet()
    {
        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
        IExceptionHandlerPathFeature? exception = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
        ExceptionMessage = exception?.Error.Message ?? "N/A";
        ExceptionPath = exception?.Path ?? "N/A";
        logger.LogError(exception?.Error, "Web application error.");
    }
    #endregion

}