using System.Collections.Concurrent;

using ZeroAlloc.Authorization;
using ZeroAlloc.Results;

namespace Rocket.Surgery.LaunchPad.Authorization;

/// <summary>Runs one policy for a caller, with the services of the call.</summary>
/// <param name="services">The services of the call, from which the policy is resolved.</param>
/// <param name="context">The caller.</param>
/// <param name="cancellationToken">The cancellation token of the call.</param>
public delegate ValueTask<UnitResult<AuthorizationFailure>> PolicyEvaluator(
    IServiceProvider services,
    ISecurityContext context,
    CancellationToken cancellationToken
);

/// <summary>One policy a request names: its name and arguments, the scopes it declares and how to run it.</summary>
/// <remarks>Describes one policy a request names.</remarks>
/// <param name="name">The policy's <c>[Policy]</c> name.</param>
/// <param name="arguments">The constant arguments the request passes it, as text; empty for none.</param>
/// <param name="policyType">The policy class.</param>
/// <param name="scopes">The scopes the policy declares with <see cref="RequiresScopesAttribute" />.</param>
/// <param name="evaluate">Runs the policy.</param>
[PublicAPI]
public sealed class PolicyRequirement(string name, string arguments, Type policyType, string[] scopes, PolicyEvaluator evaluate)
{

    /// <summary>The policy's <c>[Policy]</c> name.</summary>
    public string Name { get; } = name;

    /// <summary>The name with the arguments: what an outcome is recorded under.</summary>
    public string Key { get; } = arguments.Length == 0 ? name : name + "(" + arguments + ")";

    /// <summary>The policy class.</summary>
    public Type PolicyType { get; } = policyType;

    /// <summary>The scopes the policy declares.</summary>
    public IReadOnlyList<string> Scopes { get; } = scopes;

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
}

/// <summary>
///     The policies a request requires, written by LaunchPad's generator for every request with a policy attribute and registered when its
///     assembly loads.
/// </summary>
[PublicAPI]
public abstract class RequestAuthorizationPlan
{
    private static readonly ConcurrentDictionary<string, RequestAuthorizationPlan> Plans = new(StringComparer.Ordinal);

    private protected RequestAuthorizationPlan(Type requestType, PolicyGroup[] groups)
    {
        RequestType = requestType;
        Groups = groups;
        Id = requestType.FullName ?? requestType.Name;
    }

    /// <summary>The request.</summary>
    public Type RequestType { get; }

    /// <summary>What names the plan at the edge: the request's full type name.</summary>
    public string Id { get; }

    /// <summary>The requirements, all of which must pass.</summary>
    public IReadOnlyList<PolicyGroup> Groups { get; }

    /// <summary>What an edge policy name starts with; the rest is the plan's <see cref="Id" />.</summary>
    public const string PolicyPrefix = "launchpad:";

    /// <summary>The name of the edge policy that checks this plan.</summary>
    public string PolicyName => PolicyPrefix + Id;

    /// <summary>
    ///     The scopes that satisfy the plan, as alternatives: any one set is enough. A stacked policy adds its scopes to every alternative, a
    ///     group of alternatives multiplies them; duplicates and alternatives that contain another are dropped.
    /// </summary>
    public IReadOnlyList<IReadOnlyList<string>> ScopeAlternatives => field ??= Alternatives(Groups);

    /// <summary>The most alternatives that are published.</summary>
    public const int MaxAlternatives = 16;

    /// <summary>The plan of the request with this id, if it has policies.</summary>
    /// <param name="id">The request's full type name.</param>
    public static RequestAuthorizationPlan? Find(string id)
    {
        ArgumentNullException.ThrowIfNull(id);
        if (Plans.TryGetValue(id, out var plan)) return plan;
        // A plan is registered when its assembly's module initializer runs, which is when code of that assembly first runs: later than the
        // edge can ask for it. The request is found among the loaded assemblies and its module initialized.
        return Load(id) ? Plans.GetValueOrDefault(id) : null;
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "A request with a plan is rooted by the plan's registration; a type that was trimmed has no plan to find.")]
    private static bool Load(string id)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic || assembly.GetType(id, false) is not { } type) continue;
            System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(type.Module.ModuleHandle);
            return true;
        }

        return false;
    }

    private static IReadOnlyList<IReadOnlyList<string>> Alternatives(IReadOnlyList<PolicyGroup> groups)
    {
        var alternatives = new List<SortedSet<string>> { new(StringComparer.Ordinal) };
        foreach (var group in groups)
        {
            var next = new List<SortedSet<string>>();
            foreach (var alternative in alternatives)
            {
                foreach (var policy in group.Policies)
                {
                    var set = new SortedSet<string>(alternative, StringComparer.Ordinal);
                    set.UnionWith(policy.Scopes);
                    if (!next.Exists(set.SetEquals)) next.Add(set);
                }
            }

            alternatives = next.FindAll(a => !next.Exists(b => !ReferenceEquals(a, b) && b.IsProperSubsetOf(a)));
            if (alternatives.Count > MaxAlternatives) alternatives.RemoveRange(MaxAlternatives, alternatives.Count - MaxAlternatives);
        }

        return alternatives.ConvertAll(IReadOnlyList<string> (a) => [.. a]);
    }

    /// <summary>Registers the plan of a request. Called by generated code.</summary>
    /// <typeparam name="TRequest">The request.</typeparam>
    /// <param name="groups">The requirements, all of which must pass.</param>
    public static void Register<TRequest>(params PolicyGroup[] groups)
    {
        var plan = new RequestAuthorizationPlan<TRequest>(groups);
        RequestAuthorizationPlan<TRequest>.Instance = plan;
        Plans[plan.Id] = plan;
    }

    /// <summary>The caller as policies see them at the edge, where there is no request yet: reading the request throws.</summary>
    /// <param name="context">The caller.</param>
    public abstract ISecurityContext AtTheEdge(ISecurityContext context);
}

/// <summary>The plan of <typeparamref name="TRequest" />.</summary>
/// <typeparam name="TRequest">The request.</typeparam>
[PublicAPI]
public sealed class RequestAuthorizationPlan<TRequest> : RequestAuthorizationPlan
{
    // The plan is registered by the module initializer of the request's assembly, which may not have run when the plan is first asked for.
    static RequestAuthorizationPlan() => System.Runtime.CompilerServices.RuntimeHelpers.RunModuleConstructor(typeof(TRequest).Module.ModuleHandle);

    internal RequestAuthorizationPlan(PolicyGroup[] groups) : base(typeof(TRequest), groups) { }

    /// <summary>The plan, or null for a request without policies.</summary>
    public static RequestAuthorizationPlan<TRequest>? Instance { get; internal set; }

    /// <inheritdoc />
    public override ISecurityContext AtTheEdge(ISecurityContext context) => new EdgeSecurityContext(context);

    /// <summary>The caller as policies see them in the pipeline: with the request as the resource.</summary>
    /// <param name="context">The caller.</param>
    /// <param name="request">The request.</param>
    public ISecurityContext WithRequest(ISecurityContext context, TRequest request) => new RequestSecurityContext(context, request);

    private sealed class RequestSecurityContext(ISecurityContext inner, TRequest request) : IResourceSecurityContext<TRequest>
    {
        public string Id => inner.Id;
        public IReadOnlySet<string> Roles => inner.Roles;
        public IReadOnlyDictionary<string, string> Claims => inner.Claims;
        public TRequest Resource => request;
    }

    private sealed class EdgeSecurityContext(ISecurityContext inner) : IResourceSecurityContext<TRequest>
    {
        public string Id => inner.Id;
        public IReadOnlySet<string> Roles => inner.Roles;
        public IReadOnlyDictionary<string, string> Claims => inner.Claims;
        public TRequest Resource => throw new RequestNotAvailableException();
    }
}

/// <summary>
///     How a denial becomes a response: the conversions of <see cref="Unauthorized" /> and <see cref="Forbidden" /> to the response union,
///     registered by generated code.
/// </summary>
/// <typeparam name="TResponse">The response union.</typeparam>
[PublicAPI]
public static class AuthorizationResponse<TResponse>
{
    /// <summary>Converts <see cref="Unauthorized" /> to the response.</summary>
    public static Func<Unauthorized, TResponse>? Unauthorized { get; private set; }

    /// <summary>Converts <see cref="Forbidden" /> to the response.</summary>
    public static Func<Forbidden, TResponse>? Forbidden { get; private set; }

    /// <summary>Registers the conversions. Called by generated code.</summary>
    /// <param name="unauthorized">Converts <see cref="Unauthorized" /> to the response.</param>
    /// <param name="forbidden">Converts <see cref="Forbidden" /> to the response.</param>
    public static void Register(Func<Unauthorized, TResponse> unauthorized, Func<Forbidden, TResponse> forbidden)
    {
        Unauthorized = unauthorized;
        Forbidden = forbidden;
    }
}

/// <summary>A policy read the request at the edge, where there is none yet: it is decided in the pipeline instead.</summary>
internal sealed class RequestNotAvailableException() : InvalidOperationException("The request is not available where this policy is running.");
