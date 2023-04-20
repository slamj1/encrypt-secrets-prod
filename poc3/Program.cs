using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SecretsLib;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace poc3;

internal class Program
{
    static async Task Main(string[] args)
    {
        var isService = !(Debugger.IsAttached || args.Contains("--console"));

        var environmentName = PocRuntimeEnvironment.GetBuildEnvironment();
        var directoryPath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()!.Location) ?? throw new NullReferenceException("directoryPath!");
#if !DEBUG
        var dictionary = await SecretsUtil.DecryptJsonFileAsync(directoryPath);
#endif

        var builder = new HostBuilder()
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                // Debug stuff - Don't use in PROD!!!
                //Console.WriteLine($"Dictionary values:");

                //foreach (var keyPair in dictionary)
                //{
                //    Console.WriteLine(keyPair.Key);
                //    Console.WriteLine(keyPair.Value);
                //}

                config.SetBasePath(directoryPath)
                    .AddJsonFile(Constants.SETTINGS.Appsettings_file, false)
                    .AddJsonFile($"{Constants.SETTINGS.Appsettings_file}.{environmentName}.json", optional: true)
#if DEBUG
                .AddJsonFile($"{Constants.SECRETS.SECRETS_FILENAME}", optional: false)
#else
                    .AddInMemoryCollection(dictionary)
#endif
                    .AddEnvironmentVariables();

                if (args != null)
                {
                    config.AddCommandLine(args);
                }

                // Show we successfully decrypted the transformed secrets file. Note, we can bind to configuration classes
                // as well and inject them using IOptions<>, since we are properly flattening and formatting the keypairs.
                // Also, we are only bulidn becasue we need access to the values here
                // Remove debug stuff for PROD!!
                var built = config.Build();
                Console.WriteLine($"Secret connection string is -> {built.GetConnectionString("Default")}");
                Console.WriteLine($"Second level value is -> {built.GetRequiredSection("ConnectionStrings:SecondLevel:Key1").Value}");
                Console.WriteLine($"A secret array2 value is -> {built.GetRequiredSection("ConnectionStrings:arrays:array2:1:Obj4").Value}");

            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddSerilogServices(hostContext.Configuration);
                var provider = services.BuildServiceProvider();
                var log = provider.GetService<ILogger>() ?? throw new NullReferenceException("log");
                log.Information("Serilog Initialized");
                log.Information($"Environment is -> {environmentName}");

            }).UseSerilog();

        if (isService)
        {
            await builder.UseWindowsService().Build().RunAsync();
        }
        else
        {
            await builder.RunConsoleAsync();
        }
    }
}