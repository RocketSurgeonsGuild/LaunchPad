using ZeroAlloc.Authorization;
using ZeroAlloc.Results;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>Runs one policy for a caller, with the services of the call.</summary>
/// <param name="services">The services of the call, from which the policy is resolved: the plan's registration registers it.</param>
/// <param name="context">The caller.</param>
/// <param name="cancellationToken">The cancellation token of the call.</param>
public delegate ValueTask<UnitResult<AuthorizationFailure>> PolicyEvaluator(
    IServiceProvider services,
    ISecurityContext context,
    CancellationToken cancellationToken
);

/// <summary>One policy a request names: its name and arguments, the scopes it requires and how to run it.</summary>
/// <param name="name">The policy's <c>[Policy]</c> name.</param>
/// <param name="arguments">The constant arguments the request passes it, as text; empty for none.</param>
/// <param name="policyType">The policy class.</param>
/// <param name="scopeAlternatives">
///     The scopes the policy's scope attributes require, as alternatives: each a set of scopes that satisfies the policy. One empty set for a
///     policy without scope attributes.
/// </param>
/// <param name="evaluate">Runs the policy.</param>
[PublicAPI]
public sealed class PolicyRequirement(string name, string arguments, Type policyType, string[][] scopeAlternatives, PolicyEvaluator evaluate)
{
    /// <summary>The policy's <c>[Policy]</c> name.</summary>
    public string Name { get; } = name;

    /// <summary>The name with the arguments: what an endpoint names the policy by, and what an outcome is recorded under.</summary>
    public string Key { get; } = arguments.Length == 0 ? name : name + "(" + arguments + ")";

    /// <summary>The policy class.</summary>
    public Type PolicyType { get; } = policyType;

    /// <summary>The scopes that satisfy the policy, as alternatives: any one set is enough. For publishing: the policy itself checks them.</summary>
    public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives { get; } = scopeAlternatives.Length == 0 ? [[]] : scopeAlternatives;

    /// <summary>The scopes of the first alternative: what a challenge asks for.</summary>
    public IReadOnlyList<string> Scopes => ScopeAlternatives[0];

    /// <summary>Runs the policy.</summary>
    public PolicyEvaluator Evaluate { get; } = evaluate;
}

/// <summary>
///     One requirement of a request: a single policy (a <c>[RequirePolicy]</c>), or several of which any one is enough (a
///     <c>[RequireAnyPolicy]</c>). Every group of a request must pass.
/// </summary>
/// <param name="policies">The policies, any one of which is enough.</param>
[PublicAPI]
public sealed class PolicyGroup(params PolicyRequirement[] policies)
{
    /// <summary>The policies, any one of which is enough.</summary>
    public IReadOnlyList<PolicyRequirement> Policies { get; } = policies;

    /// <summary>The scopes that satisfy the group, as alternatives: those of each of its policies.</summary>
    public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives { get; } = [.. policies.SelectMany(policy => policy.ScopeAlternatives)];
}

/// <summary>
///     The policies a request requires. LaunchPad's generator writes one for every request with a policy attribute, and registers it with the
///     services of the application (<see cref="AuthorizationServiceCollectionExtensions.AddRequestAuthorizationPlan{TRequest}" />).
/// </summary>
[PublicAPI]
public abstract class RequestAuthorizationPlan
{
    /// <summary>The most alternatives that are published.</summary>
    public const int MaxAlternatives = 16;

    private protected RequestAuthorizationPlan(Type requestType, PolicyGroup[] groups)
    {
        RequestType = requestType;
        Groups = groups;
        Id = requestType.FullName ?? requestType.Name;
    }

    /// <summary>The request.</summary>
    public Type RequestType { get; }

    /// <summary>The request's full type name.</summary>
    public string Id { get; }

    /// <summary>The requirements, all of which must pass.</summary>
    public IReadOnlyList<PolicyGroup> Groups { get; }

    /// <summary>
    ///     The scopes that satisfy the plan, as alternatives: any one set is enough. A stacked policy adds its scopes to every alternative, a
    ///     group of alternatives multiplies them; duplicates and alternatives that contain another are dropped.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives => field ??= Alternatives(Groups);

    private static IReadOnlyList<IReadOnlyList<string>> Alternatives(IReadOnlyList<PolicyGroup> groups)
    {
        var alternatives = new List<SortedSet<string>> { new(StringComparer.Ordinal) };
        foreach (var group in groups)
        {
            var next = new List<SortedSet<string>>();
            foreach (var alternative in alternatives)
            {
                foreach (var scopes in group.ScopeAlternatives)
                {
                    var set = new SortedSet<string>(alternative, StringComparer.Ordinal);
                    set.UnionWith(scopes);
                    if (!next.Exists(set.SetEquals)) next.Add(set);
                }
            }

            alternatives = next.FindAll(a => !next.Exists(b => !ReferenceEquals(a, b) && b.IsProperSubsetOf(a)));
            if (alternatives.Count > MaxAlternatives) alternatives.RemoveRange(MaxAlternatives, alternatives.Count - MaxAlternatives);
        }

        return alternatives.ConvertAll(IReadOnlyList<string> (a) => [.. a]);
    }
}

/// <summary>The plan of <typeparamref name="TRequest" />.</summary>
/// <typeparam name="TRequest">The request.</typeparam>
/// <param name="groups">The requirements, all of which must pass.</param>
[PublicAPI]
public sealed class RequestAuthorizationPlan<TRequest>(params PolicyGroup[] groups) : RequestAuthorizationPlan(typeof(TRequest), groups);

/// <summary>
///     How a denial becomes a response: the conversions of <see cref="Unauthorized" /> and <see cref="Forbidden" /> to the response union.
///     LaunchPad's generator registers one for the response of every request with a policy
///     (<see cref="AuthorizationServiceCollectionExtensions.AddAuthorizationResponse{TResponse}" />).
/// </summary>
/// <typeparam name="TResponse">The response union.</typeparam>
/// <param name="unauthorized">Converts <see cref="Foundation.Unauthorized" /> to the response.</param>
/// <param name="forbidden">Converts <see cref="Foundation.Forbidden" /> to the response.</param>
[PublicAPI]
public sealed class AuthorizationResponse<TResponse>(Func<Unauthorized, TResponse> unauthorized, Func<Forbidden, TResponse> forbidden)
{
    /// <summary>Converts <see cref="Foundation.Unauthorized" /> to the response.</summary>
    public Func<Unauthorized, TResponse> Unauthorized { get; } = unauthorized;

    /// <summary>Converts <see cref="Foundation.Forbidden" /> to the response.</summary>
    public Func<Forbidden, TResponse> Forbidden { get; } = forbidden;
}

/// <summary>The plans of the application, by request, and the policies they name, by the name an endpoint carries. One for the application.</summary>
/// <param name="plans">Every plan that is registered.</param>
[PublicAPI]
public sealed class RequestAuthorizationPlans(IEnumerable<RequestAuthorizationPlan> plans)
{
    private readonly Dictionary<Type, RequestAuthorizationPlan> _plans = plans.GroupBy(p => p.RequestType).ToDictionary(g => g.Key, g => g.First());

    private Dictionary<string, PolicyRequirement>? _policies;

    /// <summary>Every plan of the application.</summary>
    public IReadOnlyCollection<RequestAuthorizationPlan> All => _plans.Values;

    /// <summary>The plan of a request, if it has policies.</summary>
    /// <param name="requestType">The request.</param>
    public RequestAuthorizationPlan? Find(Type requestType)
    {
        ArgumentNullException.ThrowIfNull(requestType);
        return _plans.GetValueOrDefault(requestType);
    }

    /// <summary>
    ///     A policy some request names, by its <see cref="PolicyRequirement.Key" />: the policy's name, with the arguments the request passes
    ///     it. Null for a name no request names, which is then a policy of the application's own.
    /// </summary>
    /// <param name="name">The policy name on an endpoint.</param>
    public PolicyRequirement? FindPolicy(string? name) =>
        name is null
            ? null
            : ( _policies ??= _plans
                             .Values.SelectMany(p => p.Groups)
                             .SelectMany(g => g.Policies)
                             .GroupBy(p => p.Key, StringComparer.Ordinal)
                             .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal) ).GetValueOrDefault(name);
}
