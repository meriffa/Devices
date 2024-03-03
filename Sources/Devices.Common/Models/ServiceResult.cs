namespace Devices.Common.Models;

/// <summary>
/// Service result
/// </summary>
public class ServiceResult
{

    #region Properties
    /// <summary>
    /// Service result success flag
    /// </summary>
    public required bool Success { get; set; }

    /// <summary>
    /// Service result error message
    /// </summary>
    public string? ErrorMessage { get; set; }
    #endregion

    #region Public Methods
    /// <summary>
    /// Return service result success
    /// </summary>
    public static ServiceResult Ok()
    {
        return new() { Success = true };
    }

    /// <summary>
    /// Return service result error
    /// </summary>
    /// <param name="exception"></param>
    /// <returns></returns>
    public static ServiceResult Error(Exception exception)
    {
        return new() { Success = false, ErrorMessage = exception.Message };
    }

    /// <summary>
    /// Return service result error
    /// </summary>
    /// <param name="errorMessage"></param>
    /// <returns></returns>
    public static ServiceResult Error(string errorMessage)
    {
        return new() { Success = false, ErrorMessage = errorMessage };
    }
    #endregion

}