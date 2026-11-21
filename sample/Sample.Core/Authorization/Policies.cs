using Rocket.Surgery.LaunchPad.Authorization;

using ZeroAlloc.Authorization;
using ZeroAlloc.Results;

namespace Sample.Core.Authorization;

/// <summary>
///     The policies of the sample's operations and the scopes they require. Each request names one with <c>[RequirePolicy]</c>; LaunchPad
///     checks the scope before the policy runs, and states it as the scope of the operation on every transport.
/// </summary>
public static class Policies
{
    /// <summary>Read rockets.</summary>
    public const string RocketsRead = "Rockets.Read";

    /// <summary>Create, change and remove rockets.</summary>
    public const string RocketsWrite = "Rockets.Write";

    /// <summary>Read launch records.</summary>
    public const string LaunchRecordsRead = "LaunchRecords.Read";

    /// <summary>Create, change and remove launch records.</summary>
    public const string LaunchRecordsWrite = "LaunchRecords.Write";
}

/// <summary>The scopes the sample's identity provider grants.</summary>
public static class Scopes
{
    /// <summary>Read rockets.</summary>
    public const string RocketsRead = "rockets:read";

    /// <summary>Create, change and remove rockets.</summary>
    public const string RocketsWrite = "rockets:write";

    /// <summary>Read launch records.</summary>
    public const string LaunchRecordsRead = "launch-records:read";

    /// <summary>Create, change and remove launch records.</summary>
    public const string LaunchRecordsWrite = "launch-records:write";

    /// <summary>Every scope, with what it grants.</summary>
    public static IReadOnlyDictionary<string, string> All { get; } = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        [RocketsRead] = "Read rockets",
        [RocketsWrite] = "Create, change and remove rockets",
        [LaunchRecordsRead] = "Read launch records",
        [LaunchRecordsWrite] = "Create, change and remove launch records",
    };
}

// A policy that only requires its scope has nothing of its own to decide: LaunchPad has checked the scope by the time it runs.
internal abstract class ScopePolicy : IAuthorizationPolicy
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default) => new(UnitResult<AuthorizationFailure>.Success());
}

[Policy(Policies.RocketsRead), RequiresScopes(Scopes.RocketsRead)]
internal sealed class RocketsReadPolicy : ScopePolicy;

[Policy(Policies.RocketsWrite), RequiresScopes(Scopes.RocketsWrite)]
internal sealed class RocketsWritePolicy : ScopePolicy;

[Policy(Policies.LaunchRecordsRead), RequiresScopes(Scopes.LaunchRecordsRead)]
internal sealed class LaunchRecordsReadPolicy : ScopePolicy;

[Policy(Policies.LaunchRecordsWrite), RequiresScopes(Scopes.LaunchRecordsWrite)]
internal sealed class LaunchRecordsWritePolicy : ScopePolicy;
