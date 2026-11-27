using Rocket.Surgery.LaunchPad.Authorization;
using Rocket.Surgery.LaunchPad.Foundation;
using ZeroAlloc.Mediator;
using ZeroAlloc.Results;

namespace ZeroAlloc.Authorization.Tests;

// Policies that are the scopes they require: LaunchPad writes their EvaluateAsync.
[Policy("Rockets.Read"), RequireScope("rockets:read")]
public sealed partial class RocketsRead : IAuthorizationPolicy;

[Policy("Rockets.Write"), RequireScope("rockets:write")]
public sealed partial class RocketsWrite : IAuthorizationPolicy;

// every [RequireScope] and one of each [RequireAnyScope]
[Policy("Rockets.Launch"), RequireScope("rockets:read", "rockets:write"), RequireAnyScope("pads:east", "pads:west")]
public sealed partial class RocketsLaunch : IAuthorizationPolicy;

[Policy("Admin")]
public sealed class Admin(PolicyCalls calls) : IAuthorizationPolicy
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, CancellationToken ct = default)
    {
        calls.Record("Admin");
        return new(
            ctx.Roles.Contains("admin")
                ? UnitResult<AuthorizationFailure>.Success()
                : UnitResult<AuthorizationFailure>.Failure(new("role.missing", "Only an administrator may do this."))
        );
    }
}

[Policy("MinimumClearance")]
public sealed class MinimumClearance : IAuthorizationPolicy<int>
{
    public ValueTask<UnitResult<AuthorizationFailure>> EvaluateAsync(ISecurityContext ctx, int level, CancellationToken ct = default) =>
        new(
            ctx.Claims.TryGetValue("clearance", out var value) && int.Parse(value, System.Globalization.CultureInfo.InvariantCulture) >= level
                ? UnitResult<AuthorizationFailure>.Success()
                : UnitResult<AuthorizationFailure>.Failure(new("clearance.low", $"Clearance {level} is required."))
        );
}

/// <summary>What ran, so a test can tell a policy ran once.</summary>
public sealed class PolicyCalls
{
    private readonly List<string> _calls = [];

    public IReadOnlyList<string> Calls
    {
        get
        {
            lock (_calls) return [.. _calls];
        }
    }

    public void Record(string policy)
    {
        lock (_calls) _calls.Add(policy);
    }
}

public sealed record RocketModel(string Name);

public union RocketResult(RocketModel, NotFound, Unauthorized, Forbidden);

public union AnyResult(RocketModel, Problem);

public readonly record struct Ping : IRequest<RocketModel>;

[RequirePolicy("Rockets.Read")]
public readonly record struct GetRocket(string Name) : IRequest<RocketResult>;

[RequirePolicy("Rockets.Write")]
[RequirePolicy("Admin")]
public readonly record struct DeleteRocket(string Name) : IRequest<AnyResult>;

[RequireAnyPolicy("Rockets.Write", "Admin")]
public readonly record struct LaunchRocket(string Name) : IRequest<RocketResult>;

[RequirePolicy("MinimumClearance", 3)]
public readonly record struct InspectRocket(string Name) : IRequest<RocketResult>;

[RequirePolicy("Rockets.Read")]
public readonly record struct ListRockets : IStreamRequest<RocketModel>;

[RequirePolicy("Rockets.Launch")]
public readonly record struct CountdownRocket(string Name) : IRequest<RocketResult>;

public class Handlers
    : IRequestHandler<Ping, RocketModel>,
      IRequestHandler<GetRocket, RocketResult>,
      IRequestHandler<DeleteRocket, AnyResult>,
      IRequestHandler<LaunchRocket, RocketResult>,
      IRequestHandler<InspectRocket, RocketResult>,
      IRequestHandler<CountdownRocket, RocketResult>,
      IStreamRequestHandler<ListRockets, RocketModel>
{
    public ValueTask<RocketModel> Handle(Ping request, CancellationToken ct) => new(new RocketModel("ping"));
    public ValueTask<RocketResult> Handle(GetRocket request, CancellationToken ct) => new(new RocketModel(request.Name));
    public ValueTask<AnyResult> Handle(DeleteRocket request, CancellationToken ct) => new(new RocketModel(request.Name));
    public ValueTask<RocketResult> Handle(LaunchRocket request, CancellationToken ct) => new(new RocketModel(request.Name));
    public ValueTask<RocketResult> Handle(InspectRocket request, CancellationToken ct) => new(new RocketModel(request.Name));
    public ValueTask<RocketResult> Handle(CountdownRocket request, CancellationToken ct) => new(new RocketModel(request.Name));

    public async IAsyncEnumerable<RocketModel> Handle(ListRockets request, [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        await Task.Yield();
        yield return new("Falcon");
    }
}

public sealed class Caller(string id, string? scope = null, params string[] roles) : ISecurityContext
{
    public string Id { get; } = id;
    public IReadOnlySet<string> Roles { get; } = new HashSet<string>(roles, StringComparer.Ordinal);
    public Dictionary<string, string> Values { get; } = scope is null ? [] : new() { ["scope"] = scope };
    public IReadOnlyDictionary<string, string> Claims => Values;
}
