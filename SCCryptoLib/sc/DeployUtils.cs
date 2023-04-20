using CertificateManager;
using JsonFlatten;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SCCryptoLib.sc;
////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   Secrets deployment utilities. </summary>
///
/// <remarks>   Slam, 3/30/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public class DeployUtils : IDeployUtils
{
    #region Private members
    /// <summary>   (Immutable) the service provider. </summary>
    private readonly IServiceProvider _serviceProvider;

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Constructor. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="serviceProvider">  The service provider. </param>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    #endregion

    #region ctor
    public DeployUtils(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    #endregion

    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates the new aes key. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <returns>   The new new the es key. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public Task<(string, string)> CreateNewAESKey()
    {
        var symmetricEncryptDecrypt = new SymmetricEncryptDecrypt();
        return Task.FromResult(symmetricEncryptDecrypt.InitSymmetricEncryptionKeyIV());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates new rsa certificate asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="fileName">     Filename of the file. </param>
    /// <param name="type">         The type. </param>
    /// <param name="password">     (Optional) The password. </param>
    /// <param name="bitLength">    (Optional) Length of the bit. </param>
    ///
    /// <returns>   The create new rsa certificate. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public async Task<bool> CreateNewRSACertificateAsync(string fileName, X509ContentType type, string password = "", int bitLength = 4096)
    {
        var createCertificates = _serviceProvider.GetService<CreateCertificates>() ?? throw new NullReferenceException("GetService<CreateCertificates>");
        var cert = CreateRsaCertificates.CreateRsaCertificate(createCertificates, bitLength);

        byte[] certPfx = cert.Export(X509ContentType.Pfx, password);

        using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write))
        {
            await fs.WriteAsync(certPfx, 0, certPfx.Length);
            await fs.FlushAsync();
        }

        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Encrypts a file using symmetric asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="certificateFile">          The certificate file. </param>
    /// <param name="certificatePassword">      The certificate password. </param>
    /// <param name="filePathToEncrypt">        The file to encrypt. </param>
    /// <param name="unencryptedArtifactsPath"> Full pathname of the unencrypted artifacts file. </param>
    /// <param name="keyTuple">                 The key tuple. </param>
    /// <param name="prependIv">                (Optional) True to prepend iv. </param>
    ///
    /// <returns>   The encrypt file using symmetric. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public async Task<bool> EncryptFileUsingSymmetricAsync(string certificateFile,   
                                                           string certificatePassword,
                                                           string filePathToEncrypt, 
                                                           string unencryptedArtifactsPath,
                                                           (bool createKey, string? AESKey, string? IVBase64) keyTuple,
                                                           bool prependIv = true)
    {
        var loadedPfx = new X509Certificate2(certificateFile, certificatePassword) ?? throw new NullReferenceException(nameof(X509Certificate2));

        if (keyTuple.createKey)
        {
            (string key, string iv) result = await CreateNewAESKey();
            keyTuple.AESKey = result.key;
            keyTuple.IVBase64 = result.iv;
        } 

        _ = keyTuple.AESKey ?? throw new NullReferenceException(nameof(keyTuple.AESKey));
        _ = keyTuple.IVBase64 ?? throw new NullReferenceException(nameof(keyTuple.IVBase64));

        await EncryptFileAndWriteArtifactsAsync(loadedPfx, filePathToEncrypt, unencryptedArtifactsPath, keyTuple.AESKey, keyTuple.IVBase64, prependIv);
            
        return true;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypt file using symmetric asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="certificateFile">      The certificate file. </param>
    /// <param name="certificatePassword">  The certificate password. </param>
    /// <param name="filePathToDecrypt">    The file path to decrypt. </param>
    /// <param name="encKeyPath">           Full pathname of the encode key file. </param>
    /// <param name="arrayNamesToFlatten">  The array names to flatten. </param>
    ///
    /// <returns>   The decrypt file using symmetric. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public async Task<Dictionary<string, string?>> DecryptFileUsingSymmetricAsync(string certificateFile,
                                                                                  string certificatePassword,         
                                                                                  string filePathToDecrypt,
                                                                                  string encKeyPath,
                                                                                  string[]? arrayNamesToFlatten)
    {
        return await DecryptFileUsingSymmetric2Async(certificateFile,
                                                     certificatePassword,
                                                     filePathToDecrypt,
                                                     encKeyPath,
                                                     arrayNamesToFlatten);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypt file using symmetric 2 asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="certificateFile">      The certificate file. </param>
    /// <param name="certificatePassword">  The certificate password. </param>
    /// <param name="filePathToDecrypt">    The file path to decrypt. </param>
    /// <param name="encKeyPath">           Full pathname of the encode key file. </param>
    /// <param name="arrayNamesToFlatten">  The array names to flatten. </param>
    ///
    /// <returns>   The decrypt file using symmetric 2. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static async Task<Dictionary<string,string?>> DecryptFileUsingSymmetric2Async(string certificateFile,
                                                                                         string certificatePassword,
                                                                                         string filePathToDecrypt,
                                                                                         string encKeyPath,
                                                                                         string[]? arrayNamesToFlatten)
    {
        // Load the certificate and use the password we get from systemD
        var asymmetricEncryptDecrypt = new AsymmetricEncryptDecrypt();
        var loadedPfx = new X509Certificate2(certificateFile, certificatePassword) ?? throw new NullReferenceException($"Cannot load certficate file -> {certificateFile}");

        // Read in the file, grab the first 24 bytes containg the IV (salt) and strip it off
        string contents = await File.ReadAllTextAsync(filePathToDecrypt);
        var iv = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(contents).Take(24).ToArray());
        contents = contents.Substring(24);

        // Decrypt the symmetric AES key with the certificate
        string decryptedKey = DecryptSymmetricKey(encKeyPath, asymmetricEncryptDecrypt, loadedPfx);

        // Finally, decrypt the settings file
        var decryptedJson = new SymmetricEncryptDecrypt().Decrypt(contents, iv, decryptedKey);

        // Debug stuff - Don't use in PROD!!!
        //Console.WriteLine($"decryptedJson -> {decryptedJson}");

        return GetFlattenedDictionary(decryptedJson, arrayNamesToFlatten);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypt file using symmetric 2.No async version </summary>
    ///
    /// <remarks>   Slam, 4/13/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="certificateFile">      The certificate file. </param>
    /// <param name="certificatePassword">  The certificate password. </param>
    /// <param name="filePathToDecrypt">    The file path to decrypt. </param>
    /// <param name="encKeyPath">           Full pathname of the encode key file. </param>
    /// <param name="arrayNamesToFlatten">  The array names to flatten. </param>
    ///
    /// <returns>   A Dictionary&lt;string,string?&gt; </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static Dictionary<string, string?> DecryptFileUsingSymmetric2(string certificateFile,
                                                                         string certificatePassword,
                                                                         string filePathToDecrypt,
                                                                         string encKeyPath,
                                                                         string[]? arrayNamesToFlatten)
    {
        // Load the certificate and use the password we get from systemD
        var asymmetricEncryptDecrypt = new AsymmetricEncryptDecrypt();
        var loadedPfx = new X509Certificate2(certificateFile, certificatePassword) ?? throw new NullReferenceException($"Cannot load certficate file -> {certificateFile}");

        // Read in the file, grab the first 24 bytes containg the IV (salt) and strip it off
        string contents = File.ReadAllText(filePathToDecrypt);
        var iv = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(contents).Take(24).ToArray());
        contents = contents.Substring(24);

        // Decrypt the symmetric ket with the certificate
        string decryptedKey = DecryptSymmetricKey(encKeyPath, asymmetricEncryptDecrypt, loadedPfx);

        // Finally, decrypt the settings file
        var decryptedJson = new SymmetricEncryptDecrypt().Decrypt(contents, iv, decryptedKey);

        // Debug stuff - Don't use in PROD!!!
        //Console.WriteLine($"decryptedJson -> {decryptedJson}");

        return GetFlattenedDictionary(decryptedJson, arrayNamesToFlatten);
    }
    #endregion

    #region Private methods

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets flattened dictionary. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="decryptedJson">        The decrypted JSON. </param>
    /// <param name="arrayNamesToFlatten">  The array names to flatten. </param>
    ///
    /// <returns>   The flattened dictionary. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static Dictionary<string, string?> GetFlattenedDictionary(string decryptedJson,
                                                                      string[]? arrayNamesToFlatten)
    {
        return new Dictionary<string, object>(JObject.Parse(decryptedJson).Flatten()).ToDictionary(x =>
        {
            StringBuilder strWork = new StringBuilder(x.Key.Replace(".", ":"));

            foreach (var arrName in arrayNamesToFlatten ?? Enumerable.Empty<string>()) 
            { 
                if (x.Key.Contains(arrName))
                {
                    strWork = strWork.Replace($"[", ":").Replace("]", "");
                }
            }

            return strWork.ToString();

        }, 
        y => y.Value.ToString());
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypt symmetric key. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="encKeyPath">               Full pathname of the encode key file. </param>
    /// <param name="asymmetricEncryptDecrypt"> The asymmetric encrypt decrypt. </param>
    /// <param name="loadedPfx">                The loaded pfx. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static string DecryptSymmetricKey(string encKeyPath, 
                                                AsymmetricEncryptDecrypt asymmetricEncryptDecrypt,
                                                X509Certificate2 loadedPfx)
    {
        string keyEnc = File.ReadAllText(encKeyPath);
        return asymmetricEncryptDecrypt.Decrypt(keyEnc,
            Utils.CreateRsaPrivateKey(loadedPfx));
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Writes a file. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="outputPath">       Full pathname of the output file. </param>
    /// <param name="decryptedJson">    The decrypted JSON. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private static async Task WriteFile(string outputPath, string decryptedJson)
    {
        //Console.WriteLine($"Creating file -> {outputPath}");

        using (var writer = File.CreateText($"{outputPath}"))
        {
            await writer.WriteAsync(decryptedJson);
            writer.Flush();
            writer.Close();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Encrypts a file and write artifacts asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="loadedPfx">            The loaded pfx. </param>
    /// <param name="filePathToEncrypt">    The file path to encrypt. </param>
    /// <param name="artifactsPath">        Full pathname of the artifacts file. </param>
    /// <param name="aESKey">               The es key. </param>
    /// <param name="iVBase64">             Zero-based index of the v base 64. </param>
    /// <param name="prependIv">            True to prepend iv. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private async Task EncryptFileAndWriteArtifactsAsync(X509Certificate2 loadedPfx, string filePathToEncrypt, string artifactsPath, string aESKey, string iVBase64, bool prependIv)
    {
        var symmetricEncryptDecrypt = new SymmetricEncryptDecrypt();
        string contents = await File.ReadAllTextAsync(filePathToEncrypt) ?? throw new NullReferenceException(nameof(File.ReadAllTextAsync));
        var encryptedContents = symmetricEncryptDecrypt.Encrypt(contents, iVBase64, aESKey);

        await WriteEncryptedFileAndArtifactsAsync(loadedPfx, encryptedContents, filePathToEncrypt, artifactsPath, aESKey, iVBase64, prependIv);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Writes an encrypted file and artifacts asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="loadedPfx">            The loaded pfx. </param>
    /// <param name="encryptedContents">    The encrypted contents. </param>
    /// <param name="filePathToEncrypt">    The file path to encrypt. </param>
    /// <param name="artifactsPath">        Full pathname of the artifacts file. </param>
    /// <param name="aESKey">               The es key. </param>
    /// <param name="iVBase64">             Zero-based index of the v base 64. </param>
    /// <param name="prependIv">            True to prepend iv. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private async Task WriteEncryptedFileAndArtifactsAsync(X509Certificate2 loadedPfx, string encryptedContents, string filePathToEncrypt, string artifactsPath, string aESKey, string iVBase64, bool prependIv)
    {
        await WriteEncryptedFileAsync(encryptedContents, filePathToEncrypt, iVBase64, prependIv);
        await WriteUnEncryptedKeyAsync(artifactsPath, aESKey);
        await WriteEncryptedKeyAsync(artifactsPath, aESKey, loadedPfx);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Writes an encrypted key asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="artifactsPath">    Full pathname of the artifacts file. </param>
    /// <param name="aESKey">           The es key. </param>
    /// <param name="loadedPfx">        The loaded pfx. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private async Task WriteEncryptedKeyAsync(string artifactsPath, string aESKey, X509Certificate2 loadedPfx)
    {
        var asymmetricEncryptDecrypt = new AsymmetricEncryptDecrypt();
        var encryptedKey = asymmetricEncryptDecrypt.Encrypt(aESKey,
            Utils.CreateRsaPublicKey(loadedPfx));

        using (var writer = File.CreateText($"{artifactsPath}{Path.DirectorySeparatorChar}symmetric.key.enc"))
        {
            await writer.WriteAsync(encryptedKey);
            writer.Flush();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Writes an un encrypted key asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="artifactsPath">    Full pathname of the artifacts file. </param>
    /// <param name="aESKey">           The es key. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private async Task WriteUnEncryptedKeyAsync(string artifactsPath, string aESKey)
    {
        using (var writer = File.CreateText($"{artifactsPath}{Path.DirectorySeparatorChar}symmetric.key"))
        {
            await writer.WriteAsync(aESKey);
            writer.Flush();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Writes an encrypted file asynchronous. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="encryptedContents">    The encrypted contents. </param>
    /// <param name="filePathToEncrypt">    The file path to encrypt. </param>
    /// <param name="iVBase64">             Zero-based index of the v base 64. </param>
    /// <param name="prependIv">            True to prepend iv. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private async Task WriteEncryptedFileAsync(string encryptedContents, string filePathToEncrypt, string iVBase64, bool prependIv)
    {
        using (var writer = File.CreateText($"{filePathToEncrypt}.enc"))
        {
            if (prependIv)
            {
                writer.Write(iVBase64);
            }

            await writer.WriteAsync(encryptedContents);
            writer.Flush();
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Shred file. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="filePath"> Full pathname of the file. </param>
    ///
    /// <returns>   A Task. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private async Task ShredFile(string filePath)
    {
        await BashCommandUtils.RunCommandWithBash($"{Constants.BASH.SHRED_COMMAND} {filePath}");
    }
    #endregion
}
