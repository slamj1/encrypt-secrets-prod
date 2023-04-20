using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace poc3;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   A serilog extensions. </summary>
///
/// <remarks>   Slam, 3/31/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public static class SerilogExtensions
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Adds the serilog services. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ///
    /// <param name="services">         The services. </param>
    /// <param name="configuration">    The configuration. </param>
    ///
    /// <returns>   An IServiceCollection. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static IServiceCollection AddSerilogServices(this IServiceCollection services,
                                                        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

        return services.AddSingleton(Log.Logger);
    }
}
