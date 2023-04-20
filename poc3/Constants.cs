namespace poc3;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>  Constants. </summary>
///
/// <remarks>   Slam, 3/31/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public static class Constants
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An environment. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class ENVIRONMENT
    {
        /// <summary>   (Immutable) the local. </summary>
        public static readonly string LOCAL = nameof(LOCAL);

        /// <summary>   (Immutable) the development. </summary>
        public static readonly string DEV = nameof(DEV);

        /// <summary>   (Immutable) the test. </summary>
        public static readonly string TEST = nameof(TEST);

        /// <summary>   (Immutable) the development linux. </summary>
        public static readonly string DEVLinux = nameof(DEVLinux);

        /// <summary>   (Immutable) the test linux. </summary>
        public static readonly string TESTLinux = nameof(TESTLinux);

        /// <summary>   (Immutable) the product. </summary>
        public static readonly string PROD = nameof(PROD);

        /// <summary>   (Immutable) the uat linux. </summary>
        public static readonly string UATLinux = nameof(UATLinux);

        /// <summary>   (Immutable) the product linux. </summary>
        public static readonly string PRODLinux = nameof(PRODLinux);

        /// <summary>   (Immutable) the mobile. </summary>
        public static readonly string MOBILE = nameof(MOBILE);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Settings. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class SETTINGS
    {
        /// <summary>   (Immutable) the default. </summary>
        public static readonly string Default = nameof(Default);

        /// <summary>   (Immutable) the appsettings file. </summary>
        public static readonly string Appsettings_file = "appsettings.json";

        /// <summary>   (Immutable) list of names of the arrays. </summary>
        public static readonly string[] ArrayNames = { "array", "array2" };
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A systemd. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class SYSTEMD
    {
        /// <summary>   (Immutable) pathname of the credentials directory. </summary>
        public static readonly string CREDENTIALS_DIRECTORY = nameof(CREDENTIALS_DIRECTORY);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   An application. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class APPLICATION
    {
        /// <summary>   (Immutable) the first poc. </summary>
        public static readonly string poc3 = nameof(poc3);
        /// <summary>   (Immutable) the test. </summary>
        public static readonly string TEST = nameof(TEST);
        /// <summary>   (Immutable) the mobile. </summary>
        public static readonly string MOBILE = nameof(MOBILE);
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   A secrets. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static class SECRETS
    {
        /// <summary>   (Immutable) filename of the secrets file. </summary>
        public static readonly string SECRETS_FILENAME = $"secrets.json";
        /// <summary>   (Immutable) the temporary working dir. </summary>
        public static readonly string TMP_WORKING_DIR = "tmp";
        /// <summary>   (Immutable) filename of the encode secrets file. </summary>
        public static readonly string ENC_SECRETS_FILENAME = "secrets.json.enc";
        /// <summary>   (Immutable) filename of the cert file. </summary>
        public static readonly string CERT_FILENAME = "SymmetricCertificate.pfx";
        /// <summary>   (Immutable) filename of the encode key file. </summary>
        public static readonly string ENC_KEY_FILENAME = "symmetric.key.enc";
        /// <summary>   (Immutable) full pathname of the secrets file. </summary>
        public static readonly string SECRETS_PATH = $"{Path.DirectorySeparatorChar}sc-priv{Path.DirectorySeparatorChar}{APPLICATION.poc3}";
        /// <summary>   (Immutable) full pathname of the cert file. </summary>
        public static readonly string CERT_PATH = $"{Path.DirectorySeparatorChar}sc-certs{Path.DirectorySeparatorChar}{APPLICATION.poc3}";
        /// <summary>   (Immutable) the cert. </summary>
        public static readonly string CERT = nameof(CERT);
    }

}
