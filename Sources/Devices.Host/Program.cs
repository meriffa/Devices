using Devices.Service.Extensions;
using Devices.Service.Solutions.Extensions;
using Serilog;

namespace Devices.Host;

/// <summary>
/// Application class
/// </summary>
public class Program
{

    #region Initialization
    /// <summary>
    /// Application entry
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        CreateApplication(CreateBuilder(args)).Run();
    }
    #endregion

    #region Private Methods
    /// <summary>
    /// Create web application builder
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static WebApplicationBuilder CreateBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));
        ConfigureServices(builder.Services, builder.Configuration);
        return builder;
    }

    /// <summary>
    /// Configure services
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    private static void ConfigureServices(IServiceCollection services, ConfigurationManager configuration)
    {
        services.AddServices(configuration);
        services.AddServicesSolutions(configuration);
        services.AddSecurity();
        services.AddAuthorizationBuilder()
            .AddPolicies()
            .AddPoliciesSolutions();
        services.AddControllers()
            .AddApplicationPart(typeof(Service.Solutions.Garden.Controllers.GardenController).Assembly)
            .AddApplicationPart(typeof(Service.Controllers.IdentityController).Assembly);
        services.AddSwaggerGen(options => options.CustomSchemaIds(type => type.FullName));
        services.AddRazorPages(options =>
        {
            options.AuthorizeAreas();
            options.AuthorizeAreasSolutions();
        });
    }

    /// <summary>
    /// Create web application
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    private static WebApplication CreateApplication(WebApplicationBuilder builder)
    {
        var application = builder.Build();
        if (!application.Environment.IsDevelopment())
        {
            application.UseExceptionHandler("/Error");
            application.UseHsts();
        }
        application.UseSwagger();
        application.UseSwaggerUI();
        application.UseSerilogRequestLogging();
        application.UseHttpsRedirection();
        application.UseStaticFiles();
        application.UseRouting();
        application.UseSecurity();
        application.MapControllers();
        application.MapRazorPages();
        return application;
    }
    #endregion

}