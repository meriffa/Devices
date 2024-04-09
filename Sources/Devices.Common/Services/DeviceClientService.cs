using Devices.Common.Interfaces.Identification;
using Devices.Common.Models;
using Devices.Common.Options;
using Polly;
using Polly.Retry;
using System.Net;

namespace Devices.Common.Services;

/// <summary>
/// Device client service
/// </summary>
/// <param name="options"></param>
/// <param name="identityService"></param>
public abstract class DeviceClientService(ClientOptions options, IIdentityService identityService) : ClientService(options)
{

    #region Private Fields
    private readonly IIdentityService identityService = identityService;
    #endregion

    #region Protected Methods
    /// <summary>
    /// Send http request
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    protected HttpResponseMessage SendRequest(Func<HttpResponseMessage> callback)
    {
        SetupSecurity(false);
        var retryOptions = new RetryStrategyOptions<HttpResponseMessage>()
        {
            ShouldHandle = new PredicateBuilder<HttpResponseMessage>().HandleResult(i => i.StatusCode == HttpStatusCode.Unauthorized || i.StatusCode == HttpStatusCode.Forbidden),
            MaxRetryAttempts = 1,
            Delay = TimeSpan.Zero,
            OnRetry = args =>
            {
                SetupSecurity(true);
                return default;
            }
        };
        return new ResiliencePipelineBuilder<HttpResponseMessage>().AddRetry(retryOptions).Build().Execute(callback);
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Setup request security
    /// </summary>
    /// <param name="refresh"></param>
    private void SetupSecurity(bool refresh)
    {
        if (Client.DefaultRequestHeaders.Contains(Constants.AuthorizationHeader))
            Client.DefaultRequestHeaders.Remove(Constants.AuthorizationHeader);
        Client.DefaultRequestHeaders.Add(Constants.AuthorizationHeader, $"Bearer {identityService.GetDeviceBearerToken(refresh)}");
    }
    #endregion

}