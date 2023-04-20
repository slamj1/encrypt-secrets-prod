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
/// <summary>   A digital signatures. </summary>
///
/// <remarks>   Slam, 3/30/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public class DigitalSignatures
{
    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Signs. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="text"> The text. </param>
    /// <param name="rsa">  The rsa. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public string Sign(string text, RSA rsa)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signature);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Verifies. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <param name="text">             The text. </param>
    /// <param name="signatureBase64">  The signature base 64. </param>
    /// <param name="rsa">              The rsa. </param>
    ///
    /// <returns>   True if it succeeds, false if it fails. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public bool Verify(string text, string signatureBase64, RSA rsa)
    {
        byte[] data = Encoding.UTF8.GetBytes(text);
        byte[] signature = Convert.FromBase64String(signatureBase64);
        bool isValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return isValid;
    }
    #endregion
}