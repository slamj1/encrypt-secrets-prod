using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

////////////////////////////////////////////////////////////////////////////////////////////////////
// namespace: SCCryptoLib
//
// summary:	Handy certificate helper methods from Damien Bowden
// See his article -> https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/.
////////////////////////////////////////////////////////////////////////////////////////////////////
namespace SCCryptoLib;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   An utilities. </summary>
///
/// <remarks>   Slam, 3/30/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public static class Utils
{
    #region Public methods
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates rsa public key. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="certificate">  The certificate. </param>
    ///
    /// <returns>   The new rsa public key. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static RSA CreateRsaPublicKey(X509Certificate2 certificate)
    {
        RSA publicKeyProvider = certificate.GetRSAPublicKey() ?? throw new NullReferenceException(nameof(certificate));
        return publicKeyProvider;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Creates rsa private key. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="certificate">  The certificate. </param>
    ///
    /// <returns>   The new rsa private key. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static RSA CreateRsaPrivateKey(X509Certificate2 certificate)
    {
        RSA privateKeyProvider = certificate.GetRSAPrivateKey() ?? throw new NullReferenceException(nameof(certificate));
        return privateKeyProvider;
    }
    #endregion
}
