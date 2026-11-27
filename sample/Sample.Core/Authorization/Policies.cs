using Rocket.Surgery.LaunchPad.Authorization;

using ZeroAlloc.Authorization;

namespace Sample.Core.Authorization;

/// <summary>
///     The policies of the sample's operations and the scopes they require. Each request names one with <c>[RequirePolicy]</c>; the policy
///     checks the caller's scope, and LaunchPad states it as the scope of the operation on every transport.
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

// Each policy is the scope it requires and nothing else, so LaunchPad writes its EvaluateAsync: the caller is checked for the scope wherever
// the request is dispatched, by a server, a worker or anything else.
[Policy(Policies.RocketsRead), RequireScope(Scopes.RocketsRead)]
internal sealed partial class RocketsReadPolicy : IAuthorizationPolicy;

[Policy(Policies.RocketsWrite), RequireScope(Scopes.RocketsWrite)]
internal sealed partial class RocketsWritePolicy : IAuthorizationPolicy;

[Policy(Policies.LaunchRecordsRead), RequireScope(Scopes.LaunchRecordsRead)]
internal sealed partial class LaunchRecordsReadPolicy : IAuthorizationPolicy;

[Policy(Policies.LaunchRecordsWrite), RequireScope(Scopes.LaunchRecordsWrite)]
internal sealed partial class LaunchRecordsWritePolicy : IAuthorizationPolicy;
