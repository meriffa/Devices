using Devices.Client.Solutions.Garden.Interfaces;
using Devices.Common.Options;
using Devices.Common.Solutions.Garden.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Devices.Client.Solutions.Garden.Services;

/// <summary>
/// Garden service client
/// </summary>
public class GardenServiceClient : IGardenServiceClient, IDisposable
{

    #region Private Fields
    private readonly ILogger<GardenServiceClient> logger;
    private HttpClientHandler? handler;
    private HttpClient? client;
    private bool disposed = false;
    #endregion

    #region Initialization
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="options"></param>
    public GardenServiceClient(ILogger<GardenServiceClient> logger, IOptions<ClientOptions> options)
    {
        this.logger = logger;
        handler = new() { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
        client = new(handler) { BaseAddress = new Uri(options.Value.Service.Host), Timeout = TimeSpan.FromSeconds(options.Value.Service.Timeout) };
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Save weather condition
    /// </summary>
    /// <param name="weatherCondition"></param>
    public void SaveWeatherCondition(WeatherCondition weatherCondition)
    {
        try
        {
            var content = new StringContent(JsonSerializer.Serialize(weatherCondition), Encoding.UTF8, "application/json");
            HttpResponseMessage response = client!.PostAsync($"/Service/Solutions/Garden/SaveWeatherCondition", content).Result;
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Error}", ex.Message);
            throw;
        }
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
    ~GardenServiceClient() => Dispose(false);
    #endregion

}