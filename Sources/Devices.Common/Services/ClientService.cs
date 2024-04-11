using Devices.Common.Models;
using Devices.Common.Options;

namespace Devices.Common.Services;

/// <summary>
/// Client service
/// </summary>
public abstract class ClientService : IDisposable
{

    #region Private Fields
    private HttpClientHandler? handler;
    private HttpClient? client;
    private readonly Random random = new();
    private readonly int delayDuration;
    private readonly int retryCount;
    private bool disposed = false;
    #endregion

    #region Properties
    /// <summary>
    /// Service client options
    /// </summary>
    protected ClientOptions Options { get; private set; }
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="options"></param>
    public ClientService(ClientOptions options)
    {
        Options = options;
        handler = new() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
        client = new(handler) { BaseAddress = new Uri(options.Service.Host), Timeout = TimeSpan.FromSeconds(options.Service.Timeout) };
        delayDuration = options.Service.DelayDuration * 1000;
        retryCount = options.Service.RetryCount;
    }
    #endregion

    #region Protected Methods
    /// <summary>
    /// Add device authorization
    /// </summary>
    /// <param name="identity"></param>
    protected void AddDeviceAuthorization(string identity)
    {
        if (client!.DefaultRequestHeaders.Contains(Constants.AuthorizationHeader))
            client.DefaultRequestHeaders.Remove(Constants.AuthorizationHeader);
        client.DefaultRequestHeaders.Add(Constants.AuthorizationHeader, $"Bearer {identity}");
    }

    /// <summary>
    /// Send HTTP GET request
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="preRequestDelay"></param>
    /// <param name="retry"></param>
    /// <returns></returns>
    protected HttpResponseMessage GetRequest(string requestUri, bool preRequestDelay = true, bool retry = false) => SendRequest(requestUri, true, preRequestDelay: preRequestDelay, retry: retry);

    /// <summary>
    /// Send HTTP POST request
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="content"></param>
    /// <param name="preRequestDelay"></param>
    /// <param name="retry"></param>
    /// <returns></returns>
    protected HttpResponseMessage PostRequest(string requestUri, HttpContent? content = null, bool preRequestDelay = true, bool retry = false) => SendRequest(requestUri, false, content: content, preRequestDelay: preRequestDelay, retry: retry);
    #endregion

    #region Private Methods
    /// <summary>
    /// Send HTTP request
    /// </summary>
    /// <param name="requestUri"></param>
    /// <param name="getRequest"></param>
    /// <param name="content"></param>
    /// <param name="preRequestDelay"></param>
    /// <param name="retry"></param>
    /// <returns></returns>
    /// <exception cref="TimeoutException"></exception>
    private HttpResponseMessage SendRequest(string requestUri, bool getRequest, HttpContent? content = null, bool preRequestDelay = true, bool retry = false)
    {
        for (int iteration = 0; iteration < retryCount; iteration++)
            try
            {
                if (preRequestDelay)
                    Thread.Sleep(random.Next(delayDuration));
                return getRequest ? client!.GetAsync(requestUri).Result : client!.PostAsync(requestUri, content).Result;
            }
            catch (AggregateException ex)
            {
                if (retry && iteration < retryCount - 1)
                    continue;
                var innerException = ex.Flatten().InnerException;
                if (innerException != null)
                {
                    if (innerException is TaskCanceledException)
                        throw new TimeoutException($"The HTTP request has timed out (Method = '{(getRequest ? "GET" : "POST")}', URL = '{requestUri}').", innerException);
                    throw innerException;
                }
                throw;
            }
        throw new("Maximum number of operation retries has been reached.");
    }
    #endregion

    #region Finalization
    /// <summary>
    /// Finalization
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                client?.Dispose();
                client = null;
                handler?.Dispose();
                handler = null;
            }
            disposed = true;
        }
    }

    /// <summary>
    /// Finalization
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Finalization
    /// </summary>
    ~ClientService() => Dispose(false);
    #endregion

}