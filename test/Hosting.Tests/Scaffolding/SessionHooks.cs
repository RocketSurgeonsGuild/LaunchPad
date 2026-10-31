using TUnit.Core.Interfaces;

// Every test runs dotnet builds, which use several cores each: one test per two processors keeps a CI runner from being swamped.
[assembly: ParallelLimiter<Hosting.Tests.Scaffolding.BuildsInParallel>]

namespace Hosting.Tests.Scaffolding;

internal sealed class BuildsInParallel : IParallelLimit
{
    public int Limit => Math.Max(1, Environment.ProcessorCount / 2);
}

internal static class SessionHooks
{
    [After(TestSession)]
    public static void CleanUp() => PackedHosting.Clean();
}
