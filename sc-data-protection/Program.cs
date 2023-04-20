using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SCCryptoLib.sc;
using Serilog;
using Serilog.Core;
using System.Security.Cryptography.X509Certificates;

namespace sc_data_protection;

public class Program
{
    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Main entry-point for this application. A crypto utility console app for deployment
    /// </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="args"> The arguments. </param>
    ///
    /// <returns>   Exit-code for the process - 1 for success, else 0 for fail. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static async Task<int> Main(string[] args)
    {
        if (string.IsNullOrEmpty(args[0]))
        {
            Log.Logger.Error($"Please provide a command");
            return 0;
        }

        var serviceProvider = AddServices();
        Log.Logger = GetLogger(AddConfiguration());

        Log.Logger.Information($"Executing command -> {args[0]}");

        switch (args[0])
        {
            case Constants.METHODS.CreateNewRSACertificate:
                return await TryExecuteCommand(() => CreateNewRSACertificate(serviceProvider, args));
                               
            case Constants.METHODS.EncryptFileUsingSymmetric:
                return await TryExecuteCommand(() => EncryptFileUsingSymmetric(serviceProvider, args));

            default:
                break;

        }

        Log.Logger.Error($"No valid command specified");
        return 0; 
    }
    #endregion

    #region Private methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Try execute command. </summary>
    ///
    /// <remarks>   Slam, 4/18/2023. </remarks>
    ///
    /// <param name="command">  The command. </param>
    ///
    /// <returns>   An int. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static async Task<int> TryExecuteCommand(Func<Task<int>> command)
    {
        try
        {
            return await command();
        }
        catch (Exception ex)
        {
            LogErrorExecutingCommand(command.Method.Name, ex);
            return 0;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Logs error executing command. </summary>
    ///
    /// <remarks>   Slam, 4/17/2023. </remarks>
    ///
    /// <param name="command">  The command. </param>
    /// <param name="ex">       The exception. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static void LogErrorExecutingCommand(string command, Exception ex)
    {
        Log.Logger.Error($"Exception executing {command} command -> {ex}");
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets a logger. </summary>
    ///
    /// <remarks>   Slam, 4/15/2023. </remarks>
    ///
    /// <param name="configuration">    The configuration. </param>
    ///
    /// <returns>   The logger. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static Logger GetLogger(IConfigurationRoot configuration)
    {
        return new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Adds configuration. </summary>
    ///
    /// <remarks>   Slam, 4/15/2023. </remarks>
    ///
    /// <returns>   An IConfigurationRoot. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static IConfigurationRoot AddConfiguration()
    {
        return new ConfigurationBuilder()
       .AddJsonFile("appsettings.json")
       .Build();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Adds services. </summary>
    ///
    /// <remarks>   Slam, 4/15/2023. </remarks>
    ///
    /// <returns>   A ServiceProvider. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static ServiceProvider AddServices()
    {
        return new ServiceCollection()
            .AddCertificateManager()
            .AddTransient<IDeployUtils, DeployUtils>()
            .BuildServiceProvider();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////4
    /// <summary>   Encrypts a file using symmetric. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="serviceProvider">  The service provider. </param>
    /// <param name="args">             The arguments. </param>
    ///
    /// <returns>   1 for success, 0 for fail </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static async Task<int> EncryptFileUsingSymmetric(ServiceProvider serviceProvider, string[] args)
    {
        if (!ValidateArgs(args, 5))
        {
            Log.Logger.Error($"Invalid Arguments for {nameof(EncryptFileUsingSymmetric)}");
            return 0;
        }

        var deployUtils = serviceProvider.GetService<IDeployUtils>() ?? throw new NullReferenceException(nameof(IDeployUtils));
        return Convert.ToInt32(await deployUtils.EncryptFileUsingSymmetricAsync(args[1], args[2], args[3], args[4], (true, null, null)));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates new rsa certificate. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="serviceProvider">  The service provider. </param>
    /// <param name="args">             The arguments. </param>
    ///
    /// <returns>   The new new rsa certificate. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static async Task<int> CreateNewRSACertificate(ServiceProvider serviceProvider, string[] args)
    {
        if (!ValidateArgs(args, 3))
        {
            Log.Logger.Error($"Invalid Arguments for {nameof(CreateNewRSACertificate)}");
            return 0;
        }

        var deployUtils = serviceProvider.GetService<IDeployUtils>() ?? throw new NullReferenceException(nameof(IDeployUtils));
        return Convert.ToInt32(await deployUtils.CreateNewRSACertificateAsync(args[1], X509ContentType.Pfx, args[2]));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Validates the arguments. </summary>
    ///
    /// <remarks>   Slam, 4/15/2023. </remarks>
    ///
    /// <param name="args">     The arguments. </param>
    /// <param name="count">    Number of. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    private static bool ValidateArgs(string[] args, int count)
    {
        return !args.Any(x => string.IsNullOrEmpty(x)) && args.Length == count;
    }
    #endregion
}
