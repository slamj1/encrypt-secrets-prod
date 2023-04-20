namespace poc3;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   A poc 1 runtime environment. </summary>
///
/// <remarks>   Slam, 3/31/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public static class PocRuntimeEnvironment
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Gets build environment. </summary>
    ///
    /// <remarks>   Slam, 3/31/2023. </remarks>
    ///
    /// <returns>   The build environment. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static string GetBuildEnvironment()
    {
        string environmentName = "";
#if DEBUG
        environmentName = "";
#elif PRODLINUX
environmentName = Constants.ENVIRONMENT.PRODLinux;
#elif DEVLINUX
environmentName = Constants.ENVIRONMENT.DEVLinux;
#elif TESTLINUX
        environmentName = Constants.ENVIRONMENT.TESTLinux;
#endif
        return environmentName;
    }
}

