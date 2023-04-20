using System.Security.Cryptography;
using System.Text;

////////////////////////////////////////////////////////////////////////////////////////////////////
// namespace: SCCryptoLib
//
// summary: Handy certificate helper methods from Damien Bowden
// See his article -> https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/.
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace SCCryptoLib;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   An asymmetric encrypt decrypt. </summary>
///
/// <remarks>   Slam, 3/30/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public class AsymmetricEncryptDecrypt
{
    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Encrypts. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="text"> The text. </param>
    /// <param name="rsa">  The rsa. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string Encrypt(string text, RSA rsa)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        byte[] cipherText = rsa.Encrypt(data, RSAEncryptionPadding.Pkcs1);
        return Convert.ToBase64String(cipherText);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Decrypts. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="text"> The text. </param>
    /// <param name="rsa">  The rsa. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string Decrypt(string text, RSA rsa)
    {
        byte[] data = Convert.FromBase64String(text);
        byte[] cipherText = rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        return Encoding.UTF8.GetString(cipherText);
    }
    #endregion
}