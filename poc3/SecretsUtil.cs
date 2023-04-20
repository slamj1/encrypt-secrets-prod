using poc3;
using SCCryptoLib.sc;
using System.Text;

namespace SecretsLib;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   The secrets utility. </summary>
///
/// <remarks>   Slam, 3/31/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public static class SecretsUtil
{
    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypt JSON file. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ///
    /// <param name="directoryPath">    Full pathname of the directory file. </param>
    ///
    /// <returns>   A Dictionary&lt;string,string?&gt; </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static async Task<Dictionary<string, string?>> DecryptJsonFileAsync(string directoryPath)
    {
        string certPw = File.ReadAllText($"{Environment.GetEnvironmentVariable(Constants.SYSTEMD.CREDENTIALS_DIRECTORY)}{Path.DirectorySeparatorChar}{Constants.APPLICATION.poc3}",
                                            Encoding.ASCII).Trim(); //SM: Trim is very inportant here (aka LF handling)!!!

        // *** Debug stuff - Don't use in PROD!!! ***
        //Console.WriteLine($"Credentials dir for systemd -> {Environment.GetEnvironmentVariable(Constants.SYSTEMD.CREDENTIALS_DIRECTORY);}");

        //
        //Console.WriteLine($"Decrypting...");
        //Console.WriteLine($"{Constants.SECRETS.CERT_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.CERT_FILENAME}");
        //Console.WriteLine($"{directoryPath}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_SECRETS_FILENAME}");
        //Console.WriteLine(directoryPath);
        //Console.WriteLine($"{Constants.SECRETS.SECRETS_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_KEY_FILENAME}");


        return await DeployUtils.DecryptFileUsingSymmetric2Async($"{Constants.SECRETS.CERT_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.CERT_FILENAME}",
                                                                    certPw,
                                                                    $"{directoryPath}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_SECRETS_FILENAME}",
                                                                    $"{Constants.SECRETS.SECRETS_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_KEY_FILENAME}",
                                                                    Constants.SETTINGS.ArrayNames);
    }

    public static Dictionary<string, string?> DecryptJsonFile(string directoryPath)
    {
        string certPw = File.ReadAllText($"{Environment.GetEnvironmentVariable(Constants.SYSTEMD.CREDENTIALS_DIRECTORY)}{Path.DirectorySeparatorChar}{Constants.APPLICATION.poc3}",
                                            Encoding.ASCII).Trim(); //SM: Trim is very inportant here (aka LF handling)!!!

        // *** Debug stuff - Don't use in PROD!!! ***
        //Console.WriteLine($"Credentials dir for systemd -> {Environment.GetEnvironmentVariable(Constants.SYSTEMD.CREDENTIALS_DIRECTORY);}");

        //
        //Console.WriteLine($"Decrypting...");
        //Console.WriteLine($"{Constants.SECRETS.CERT_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.CERT_FILENAME}");
        //Console.WriteLine($"{directoryPath}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_SECRETS_FILENAME}");
        //Console.WriteLine(directoryPath);
        //Console.WriteLine($"{Constants.SECRETS.SECRETS_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_KEY_FILENAME}");


        return DeployUtils.DecryptFileUsingSymmetric2($"{Constants.SECRETS.CERT_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.CERT_FILENAME}",
                                                      certPw,
                                                      $"{directoryPath}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_SECRETS_FILENAME}",
                                                      $"{Constants.SECRETS.SECRETS_PATH}{Path.DirectorySeparatorChar}{Constants.SECRETS.ENC_KEY_FILENAME}",
                                                      null);
    }
    #endregion
}
