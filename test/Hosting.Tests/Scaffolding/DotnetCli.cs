using System.Diagnostics;
using System.Text;

namespace Hosting.Tests.Scaffolding;

/// <summary>
///     Runs the <c>dotnet</c> CLI out of process, as a developer would.
/// </summary>
internal static class DotnetCli
{
    /// <summary>
    ///     Runs <c>dotnet</c> and fails with its output when it exits with a non-zero code.
    /// </summary>
    /// <param name="arguments">The arguments.</param>
    /// <param name="workingDirectory">The working directory.</param>
    /// <param name="environment">Environment variables to set (a <see langword="null" /> value removes the variable).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The combined standard output and error.</returns>
    public static async Task<string> Run(
        string arguments,
        string workingDirectory,
        IReadOnlyDictionary<string, string?>? environment = null,
        CancellationToken cancellationToken = default
    )
    {
        var psi = new ProcessStartInfo(Environment.GetEnvironmentVariable("DOTNET_HOST_PATH") is { Length: > 0 } host ? host : "dotnet", arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };
        psi.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        psi.Environment["DOTNET_NOLOGO"] = "1";
        // CI variables switch ContinuousIntegrationBuild and warnings on, which would make a local run differ from a CI run.
        foreach (var name in new[] { "GITHUB_ACTIONS", "CI", "TF_BUILD", "GITLAB_CI", "APPVEYOR", "TEAMCITY_VERSION", })
        {
            psi.Environment.Remove(name);
        }

        // The test process may have been started by a dotnet command that pinned its own toolset; the build under test resolves its own.
        foreach (var name in new[] { "MSBuildSDKsPath", "MSBUILD_EXE_PATH", "MSBuildExtensionsPath", "MSBuildLoadMicrosoftTargetsReadOnly", })
        {
            psi.Environment.Remove(name);
        }

        if (environment is not null)
        {
            foreach (var (name, value) in environment)
            {
                if (value is null) psi.Environment.Remove(name);
                else psi.Environment[name] = value;
            }
        }

        using var process = Process.Start(psi)!;
        var output = new StringBuilder();
        var gate = new object();
        void Append(string? line)
        {
            if (line is null) return;
            lock (gate) output.AppendLine(line);
        }

        process.OutputDataReceived += (_, e) => Append(e.Data);
        process.ErrorDataReceived += (_, e) => Append(e.Data);
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(TimeSpan.FromMinutes(10));
        try
        {
            await process.WaitForExitAsync(timeout.Token);
        }
        catch (OperationCanceledException)
        {
            process.Kill(entireProcessTree: true);
            throw;
        }

        // the output events are raised on other threads: wait for the streams to end before reading the result
        await process.WaitForExitAsync(CancellationToken.None);
        var text = output.ToString();
        return process.ExitCode == 0 ? text : throw new InvalidOperationException($"dotnet {arguments} failed in {workingDirectory} with exit code {process.ExitCode}:{Environment.NewLine}{text}");
    }
}
