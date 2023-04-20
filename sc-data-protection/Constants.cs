namespace sc_data_protection
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Constants. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    internal static class Constants
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Method requests </summary>
        ///
        /// <remarks>   Slam, 3/30/2023. </remarks>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public static class METHODS
        {
            /// <summary>   (Immutable) the create new rsa certificate. </summary>
            public const string CreateNewRSACertificate = nameof(CreateNewRSACertificate);

            /// <summary>   (Immutable) the encrypt file using symmetric. </summary>
            public const string EncryptFileUsingSymmetric = nameof(EncryptFileUsingSymmetric);

            /// <summary>   (Immutable) the decrypt file using symmetric. </summary>
            public const string DecryptFileUsingSymmetric = nameof(DecryptFileUsingSymmetric);
            
        }
    }
}
