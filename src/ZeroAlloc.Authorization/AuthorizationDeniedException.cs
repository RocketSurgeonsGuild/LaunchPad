namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>
///     A request was denied where there is no response to answer with: a stream request, or a response that has no case for the denial. It
///     carries the <see cref="Unauthorized" /> or <see cref="Forbidden" /> problem, which each transport writes as that case.
/// </summary>
/// <param name="problem">The <see cref="Unauthorized" /> or <see cref="Forbidden" /> problem.</param>
[PublicAPI]
public sealed class AuthorizationDeniedException(Problem problem) : Exception(problem.Detail ?? problem.GetType().Name)
{
    /// <summary>The <see cref="Unauthorized" /> or <see cref="Forbidden" /> problem.</summary>
    public Problem Problem { get; } = problem;
}
