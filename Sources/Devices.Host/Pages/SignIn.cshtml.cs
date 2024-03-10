using Devices.Service.Interfaces.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Devices.Host.Pages;

/// <summary>
/// Sign in page
/// </summary>
/// <param name="logger"></param>
public class SignInModel(ILogger<SignInModel> logger, ISecurityService service) : PageModel
{

    #region Private Fields
    private readonly ILogger<SignInModel> logger = logger;
    private readonly ISecurityService service = service;
    #endregion

    #region Properties
    /// <summary>
    /// Sign in user name
    /// </summary>
    [BindProperty]
    public string Username { get; set; } = null!;

    /// <summary>
    /// Sign in password
    /// </summary>
    [BindProperty, DataType(DataType.Password)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Return URL
    /// </summary>
    public string? ReturnUrl { get; set; }

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
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    public async Task OnGetAsync(string? returnUrl = null)
    {
        if (!string.IsNullOrEmpty(ErrorMessage))
            ModelState.AddModelError(string.Empty, ErrorMessage);
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        ReturnUrl = returnUrl;
    }

    /// <summary>
    /// Post request
    /// </summary>
    /// <param name="returnUrl"></param>
    /// <returns></returns>
    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        ReturnUrl = returnUrl;
        if (ModelState.IsValid)
        {
            var user = service.GetUser(Username, Password);
            if (user != null)
            {
                var principal = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.FullName),
                    new(ClaimTypes.Email, user.Email),
                    new(ClaimTypes.Role, user.Role)
                ], CookieAuthenticationDefaults.AuthenticationScheme));
                var properties = new AuthenticationProperties
                {
                    AllowRefresh = true,
                    IsPersistent = true
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
                logger.LogInformation("User signed in (ID = {ID}, Name = '{Name}', Time = '{Time}').", user.Id, user.FullName, DateTime.UtcNow);
                return LocalRedirect(GetLocalUrl(Url, returnUrl));
            }
            else
                ModelState.AddModelError(string.Empty, "Invalid user name / password.");
        }
        return Page();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Return local url
    /// </summary>
    /// <param name="url"></param>
    /// <param name="localUrl"></param>
    /// <returns></returns>
    private static string GetLocalUrl(IUrlHelper url, string? localUrl) => !string.IsNullOrEmpty(localUrl) && url.IsLocalUrl(localUrl) ? localUrl : url.Page("/Index")!;
    #endregion

}