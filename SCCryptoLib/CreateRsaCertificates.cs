using CertificateManager;
using CertificateManager.Models;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

////////////////////////////////////////////////////////////////////////////////////////////////////
// namespace: SCCryptoLib
//
// summary:	Handy certificate helper methods from Damien Bowden
// See his article -> https://damienbod.com/2020/08/19/symmetric-and-asymmetric-encryption-in-net-core/
////////////////////////////////////////////////////////////////////////////////////////////////////

namespace SCCryptoLib
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Create rsa certificates. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class CreateRsaCertificates
    {
        #region Public methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates rsa certificate. </summary>
        ///
        /// <remarks>   Slam, 3/30/2023. </remarks>
        ///
        /// <param name="createCertificates">   The create certificates. </param>
        /// <param name="keySize">              Size of the key. </param>
        ///
        /// <returns>   The new rsa certificate. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static X509Certificate2 CreateRsaCertificate(
          CreateCertificates createCertificates, int keySize)
        {
            var basicConstraints = new BasicConstraints
            {
                CertificateAuthority = true,
                HasPathLengthConstraint = true,
                PathLengthConstraint = 2,
                Critical = false
            };

            var subjectAlternativeName = new SubjectAlternativeName
            {
                DnsName = new List<string>
                {
                    "symmetric.saasycloud.com",
                }
            };

            var x509KeyUsageFlags = X509KeyUsageFlags.KeyCertSign
               | X509KeyUsageFlags.DigitalSignature
               | X509KeyUsageFlags.KeyEncipherment
               | X509KeyUsageFlags.CrlSign
               | X509KeyUsageFlags.DataEncipherment
               | X509KeyUsageFlags.NonRepudiation
               | X509KeyUsageFlags.KeyAgreement;

            var enhancedKeyUsages = new OidCollection
            {
                OidLookup.CodeSigning,
                OidLookup.SecureEmail,
                OidLookup.TimeStamping
            };

            var certificate = createCertificates.NewRsaSelfSignedCertificate(
                new DistinguishedName { CommonName = "symmetric.saasycloud.com" },
                basicConstraints,
                new ValidityPeriod
                {
                    ValidFrom = DateTimeOffset.UtcNow,
                    ValidTo = DateTimeOffset.UtcNow.AddYears(1)
                },
                subjectAlternativeName,
                enhancedKeyUsages,
                x509KeyUsageFlags,
                new RsaConfiguration
                {
                    KeySize = keySize,
                    RSASignaturePadding = RSASignaturePadding.Pkcs1,
                    HashAlgorithmName = HashAlgorithmName.SHA256
                });

            return certificate;
        }
        #endregion
    }
}