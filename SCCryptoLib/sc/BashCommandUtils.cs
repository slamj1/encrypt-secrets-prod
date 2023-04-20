using System.Diagnostics;

namespace SCCryptoLib.sc;

////////////////////////////////////////////////////////////////////////////////////////////////////
/// <summary>   A bash command utilities. </summary>
///
/// <remarks>   Slam, 3/30/2023. </remarks>
////////////////////////////////////////////////////////////////////////////////////////////////////

public class BashCommandUtils
{
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    /// <summary>   Executes the 'command with bash' operation. </summary>
    ///
    /// <remarks>   Slam, 3/30/2023. </remarks>
    ///
    /// <exception cref="NullReferenceException">   Thrown when a value was unexpectedly null. </exception>
    ///
    /// <param name="command">  The command. </param>
    ///
    /// <returns>   A string. </returns>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public static async Task<string> RunCommandWithBash(string command)
    {
        var psi = new ProcessStartInfo();
        psi.FileName = Constants.BASH.BASH_BIN;
        psi.Arguments = $"-c \"{command}\"";
        psi.RedirectStandardOutput = true;
        psi.UseShellExecute = false;
        psi.CreateNoWindow = true;

        var process = Process.Start(psi) ?? throw new NullReferenceException(nameof(Process.Start));
        await process.WaitForExitAsync();

        return process.StandardOutput.ReadToEnd();
    }
}
