using System.Security.Cryptography.X509Certificates;

namespace SCCryptoLib.sc;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   Interface for deploy utilities. </summary>
///
/// <remarks>   Slam, 3/30/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public interface IDeployUtils
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates new rsa certificate asynchronous. </summary>
    ///
    /// <param name="fileName">     Filename of the file. </param>
    /// <param name="type">         The type. </param>
    /// <param name="password">     (Optional) The password. </param>
    /// <param name="bitLength">    (Optional) Length of the bit. </param>
    ///
    /// <returns>   The create new rsa certificate. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    Task<bool> CreateNewRSACertificateAsync(string fileName, X509ContentType type, string password = "", int bitLength = 4096);

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates the new aes key. </summary>
    ///
    /// <returns>   The new new the es key. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    Task<(string, string)> CreateNewAESKey();

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Encrypts a file using symmetric asynchronous. </summary>
    ///
    /// <param name="certificateFile">          The certificate file. </param>
    /// <param name="certificatePassword">      The certificate password. </param>
    /// <param name="fileToEncrypt">            The file to encrypt. </param>
    /// <param name="unencryptedArtifactsPath"> Full pathname of the unencrypted artifacts file. </param>
    /// <param name="keyTuple">                 The key tuple. </param>
    /// <param name="prependIv">                (Optional) True to prepend iv. </param>
    ///
    /// <returns>   The encrypt file using symmetric. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    Task<bool> EncryptFileUsingSymmetricAsync(string certificateFile,
                                              string certificatePassword,
                                              string fileToEncrypt,
                                              string unencryptedArtifactsPath,
                                              (bool createKey, string? AESKey, string? IVBase64) keyTuple,
                                              bool prependIv = true);

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypt file using symmetric asynchronous. </summary>
    ///
    /// <param name="certificateFile">      The certificate file. </param>
    /// <param name="certificatePassword">  The certificate password. </param>
    /// <param name="filePathToDecrypt">    The file path to decrypt. </param>
    /// <param name="encKeyPath">           Full pathname of the encode key file. </param>
    /// <param name="arrayNamesToFlatten">  The array names to flatten. </param>
    ///
    /// <returns>   The decrypt file using symmetric. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    Task<Dictionary<string, string?>> DecryptFileUsingSymmetricAsync(string certificateFile,
                                                                     string certificatePassword,
                                                                     string filePathToDecrypt,
                                                                     string encKeyPath,
                                                                     string[]? arrayNamesToFlatten);
};
