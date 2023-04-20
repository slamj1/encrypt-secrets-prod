using System.Security.Cryptography;
using System.Text;

////////////////////////////////////////////////////////////////////////////////////////////////////
// namespace: SCCryptoLib
//
// summary:	Handy certificate helper methods from Damien Bowden
// See his article -> https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace SCCryptoLib;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   Symmetric encrypt/decrypt functions. </summary>
///
/// <remarks>   Slam, 3/30/2023. 
///              </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public class SymmetricEncryptDecrypt
{
    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Initializes the symmetric encryption key iv. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <returns>   A Tuple. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public (string Key, string IVBase64) InitSymmetricEncryptionKeyIV()
    {
        var key = GetEncodedRandomString(32); // 256
        Aes cipher = CreateCipher(key);
        var IVBase64 = Convert.ToBase64String(cipher.IV);
        return (key, IVBase64);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Encrypts. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="text"> The text. </param>
    /// <param name="IV">   The iv. </param>
    /// <param name="key">  The key. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string Encrypt(string text, string IV, string key)
    {
        Aes cipher = CreateCipher(key);
        cipher.IV = Convert.FromBase64String(IV);

        ICryptoTransform cryptTransform = cipher.CreateEncryptor();
        byte[] plaintext = Encoding.UTF8.GetBytes(text);
        byte[] cipherText = cryptTransform.TransformFinalBlock(plaintext, 0, plaintext.Length);

        return Convert.ToBase64String(cipherText);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypts. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="encryptedText">    The encrypted text. </param>
    /// <param name="IV">               The iv. </param>
    /// <param name="key">              The key. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string Decrypt(string encryptedText, string IV, string key)
    {
        Aes cipher = CreateCipher(key);
        cipher.IV = Convert.FromBase64String(IV);

        ICryptoTransform cryptTransform = cipher.CreateDecryptor();
        byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
        byte[] plainBytes = cryptTransform.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);

        return Encoding.UTF8.GetString(plainBytes);
    }
    #endregion

    #region Private methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets encoded random string. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="length">   The length. </param>
    ///
    /// <returns>   The encoded random string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private string GetEncodedRandomString(int length)
    {
        var base64 = Convert.ToBase64String(GenerateRandomBytes(length));
        return base64;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates a cipher. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="keyBase64">    The key base 64. </param>
    ///
    /// <returns>   The new cipher. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private Aes CreateCipher(string keyBase64)
    {
        // Default values: Keysize 256, Padding PKC27
        Aes cipher = Aes.Create();
        cipher.Mode = CipherMode.CBC; // Ensure the integrity of the ciphertext if using CBC
        cipher.Padding = PaddingMode.ISO10126;
        cipher.Key = Convert.FromBase64String(keyBase64);

        return cipher;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Generates a random bytes. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="length">   The length. </param>
    ///
    /// <returns>   An array of byte. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    private byte[] GenerateRandomBytes(int length)
    {
        var byteArray = new byte[length];
        RandomNumberGenerator.Fill(byteArray);
        return byteArray;
    }
    #endregion
}