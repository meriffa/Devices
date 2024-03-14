using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Devices.Host.Extensions;

/// <summary>
/// Page extension methods
/// </summary>
public static class PageExtensions
{

    #region Public Methods
    /// <summary>
    /// Check if page is selected
    /// </summary>
    /// <param name="viewData"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string IsSelected(ViewDataDictionary viewData, string value)
    {
        var title = viewData["Title"]!.ToString();
        return title != null && title.Equals(value, StringComparison.InvariantCultureIgnoreCase) ? " active" : string.Empty;
    }
    #endregion

}