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
    private bool disposed = false;
    #endregion

    #region Properties
    /// <summary>
    /// Service client options
    /// </summary>
    protected ClientOptions Options { get; private set; }
    /// <summary>
    /// HttpClient instance
    /// </summary>
    protected HttpClient Client => client!;
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