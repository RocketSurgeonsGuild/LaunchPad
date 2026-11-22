using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;

namespace Rocket.Surgery.LaunchPad.Analyzers.Authorization;

/// <summary>One policy a request names: its class, its constant arguments and the scopes the class declares.</summary>
/// <param name="Name">The <c>[Policy]</c> name.</param>
/// <param name="Type">The policy class, fully qualified; null when no class in the compilation or its references has the name (ZAUTH001).</param>
/// <param name="Interface">The <c>IAuthorizationPolicy</c> the class implements, fully qualified, through which it is called.</param>
/// <param name="Arguments">The arguments as C# expressions, each cast to the policy's parameter type.</param>
/// <param name="ArgumentsKey">The arguments as text, which with the name keys an outcome.</param>
/// <param name="Scopes">The scopes the class declares with <c>[RequiresScopes]</c>.</param>
internal sealed record PolicyModel(
    string Name,
    string? Type,
    string? Interface,
    EquatableArray<string> Arguments,
    string ArgumentsKey,
    EquatableArray<string> Scopes
);

/// <summary>One requirement of a request: a policy, or several of which any one is enough.</summary>
internal sealed record PolicyGroupModel(EquatableArray<PolicyModel> Policies);

/// <summary>The policies a request requires: every group must pass.</summary>
internal sealed record RequestPoliciesModel(EquatableArray<PolicyGroupModel> Groups)
{
    /// <summary>The most alternatives that are published (LPAD1052).</summary>
    public const int MaxAlternatives = 16;

    public static RequestPoliciesModel None { get; } = new(EquatableArray<PolicyGroupModel>.Empty);

    public bool IsEmpty => Groups.IsEmpty;

    /// <summary>
    ///     The names an endpoint carries for the policies every caller must pass (the <c>[RequirePolicy]</c> entries): each the policy's name,
    ///     with the arguments the request passes it, which is the key the runtime knows the policy by.
    /// </summary>
    public IEnumerable<string> EdgePolicies =>
        Groups.Where(g => g.Policies.Length == 1).Select(g => g.Policies[0]).Select(p => p.ArgumentsKey.Length == 0 ? p.Name : p.Name + "(" + p.ArgumentsKey + ")").Distinct(StringComparer.Ordinal);

    /// <summary>
    ///     Whether a group of alternatives (<c>[RequireAnyPolicy]</c>) is left to the pipeline: an endpoint has no way to say "any of these",
    ///     so it only requires a caller.
    /// </summary>
    public bool HasAlternatives => Groups.Any(g => g.Policies.Length > 1);

    /// <summary>
    ///     The scopes that satisfy the request, as alternatives: any one set is enough. AND groups add their scopes to every alternative, an
    ///     OR group multiplies them; duplicates and alternatives that contain another are dropped. Empty sets are kept: a request whose policies
    ///     declare no scopes has the one alternative of no scopes.
    /// </summary>
    public EquatableArray<EquatableArray<string>> Alternatives(out bool truncated)
    {
        truncated = false;
        var alternatives = new List<SortedSet<string>> { new(StringComparer.Ordinal) };
        foreach (var group in Groups)
        {
            alternatives = Expand(alternatives, group);
            if (alternatives.Count <= MaxAlternatives) continue;
            truncated = true;
            alternatives.RemoveRange(MaxAlternatives, alternatives.Count - MaxAlternatives);
        }

        return alternatives.Select(a => a.ToEquatableArray()).ToEquatableArray();
    }

    // Every alternative so far with each policy of the group, without duplicates or alternatives that contain another.
    private static List<SortedSet<string>> Expand(List<SortedSet<string>> alternatives, PolicyGroupModel group)
    {
        var next = alternatives
                  .SelectMany(_ => group.Policies, (alternative, policy) => new SortedSet<string>(alternative.Concat(policy.Scopes), StringComparer.Ordinal))
                  .Aggregate(new List<SortedSet<string>>(), (list, set) => list.Any(set.SetEquals) ? list : [.. list, set]);
        return [.. next.Where(a => !next.Any(b => !ReferenceEquals(a, b) && b.IsProperSubsetOf(a)))];
    }
}

/// <summary>
///     Reads the policies of a request (ZeroAlloc.Authorization's <c>[RequirePolicy]</c> and <c>[RequireAnyPolicy]</c>) and the scopes each
///     policy class declares (<c>[RequiresScopes]</c>). The one place that knows how they are declared: the plan generator, the diagnostics and
///     the API generators all read through it.
/// </summary>
internal static class PolicyReader
{
    public const string RequirePolicyAttribute = "ZeroAlloc.Authorization.RequirePolicyAttribute";
    public const string RequireAnyPolicyAttribute = "ZeroAlloc.Authorization.RequireAnyPolicyAttribute";
    public const string PolicyAttribute = "ZeroAlloc.Authorization.PolicyAttribute";
    public const string PolicyInterface = "ZeroAlloc.Authorization.IAuthorizationPolicy";
    public const string RequiresScopesAttribute = "Rocket.Surgery.LaunchPad.Authorization.RequiresScopesAttribute";
    public const string PlanType = "Rocket.Surgery.LaunchPad.Authorization.RequestAuthorizationPlan";
    public const string Unauthorized = "Rocket.Surgery.LaunchPad.Foundation.Unauthorized";
    public const string Forbidden = "Rocket.Surgery.LaunchPad.Foundation.Forbidden";

    public const string RequestPoliciesAttribute = "Rocket.Surgery.LaunchPad.Authorization.RequestPoliciesAttribute";
    public const string EdgeType = "Rocket.Surgery.LaunchPad.AspNetCore.Authorization.AuthorizationProblems";
    public const string GraphQLAuthorizeAttribute = "HotChocolate.Authorization.AuthorizeAttribute";
    public const string GraphQLScopesAttribute = "Rocket.Surgery.LaunchPad.HotChocolate.Authorization.GraphQLRequiresScopesAttribute";

    private const string AuthorizationAssembly = "ZeroAlloc.Authorization";

    private static readonly ConditionalWeakTable<Compilation, Dictionary<string, INamedTypeSymbol>> PolicyTables = new();

    /// <summary>Whether the type has a policy attribute.</summary>
    public static bool HasPolicies(ITypeSymbol request) =>
        request.GetAttributes().Any(a => a.AttributeClass.Is(RequirePolicyAttribute) || a.AttributeClass.Is(RequireAnyPolicyAttribute));

    /// <summary>The policies of a request, in attribute order; <see cref="RequestPoliciesModel.None" /> for a request without any.</summary>
    public static RequestPoliciesModel Read(ITypeSymbol request, Compilation compilation)
    {
        if (!HasPolicies(request)) return RequestPoliciesModel.None;
        var policies = Policies(compilation);
        var groups = new List<PolicyGroupModel>();
        foreach (var attribute in request.GetAttributes())
        {
            if (attribute.AttributeClass.Is(RequirePolicyAttribute) && attribute.ConstructorArguments is [{ Value: string name }, ..] arguments)
            {
                var values = arguments.Length > 1 && arguments[1].Kind == TypedConstantKind.Array && !arguments[1].IsNull ? arguments[1].Values.ToArray() : [];
                groups.Add(new(new[] { Policy(name, values, policies) }.ToEquatableArray()));
            }
            else if (attribute.AttributeClass.Is(RequireAnyPolicyAttribute)
                  && attribute.ConstructorArguments is [{ Kind: TypedConstantKind.Array, IsNull: false } names])
            {
                var any = AnyOf(names, policies);
                if (!any.IsEmpty) groups.Add(new(any));
            }
        }

        return new(groups.ToEquatableArray());
    }

    private static EquatableArray<PolicyModel> AnyOf(TypedConstant names, IReadOnlyDictionary<string, INamedTypeSymbol> policies) =>
        names.Values.Select(v => v.Value).OfType<string>().Select(n => Policy(n, [], policies)).ToEquatableArray();

    private static bool UsesAuthorization(IAssemblySymbol assembly) =>
        assembly.Modules.Any(m => m.ReferencedAssemblies.Any(r => r.Name == AuthorizationAssembly));

    /// <summary>The scopes a policy class declares.</summary>
    public static EquatableArray<string> Scopes(INamedTypeSymbol policy) =>
        policy
           .GetAttributes()
           .Where(a => a.AttributeClass.Is(RequiresScopesAttribute))
           .SelectMany(a => a.ConstructorArguments is [{ Kind: TypedConstantKind.Array, IsNull: false } scopes] ? scopes.Values.Select(v => v.Value).OfType<string>() : [])
           .Where(s => !string.IsNullOrWhiteSpace(s))
           .Distinct(StringComparer.Ordinal)
           .ToEquatableArray();

    /// <summary>The policy classes of the compilation and of the references that use ZeroAlloc.Authorization, by <c>[Policy]</c> name.</summary>
    public static IReadOnlyDictionary<string, INamedTypeSymbol> Policies(Compilation compilation) =>
        PolicyTables.GetValue(
            compilation,
            static compilation =>
            {
                var table = new Dictionary<string, INamedTypeSymbol>(StringComparer.Ordinal);
                Collect(compilation.Assembly.GlobalNamespace, table);
                foreach (var assembly in compilation.SourceModule.ReferencedAssemblySymbols)
                {
                    if (UsesAuthorization(assembly)) Collect(assembly.GlobalNamespace, table);
                }

                return table;
            }
        );

    private static PolicyModel Policy(string name, TypedConstant[] arguments, IReadOnlyDictionary<string, INamedTypeSymbol> policies)
    {
        var key = string.Join(",", arguments.Select(a => a.ToCSharpString()));
        if (!policies.TryGetValue(name, out var type)) return new(name, null, null, EquatableArray<string>.Empty, key, EquatableArray<string>.Empty);
        var contract = type.AllInterfaces.FirstOrDefault(i => i.OriginalDefinition.ContainingNamespace.ToDisplayString() + "." + i.Name == PolicyInterface && i.TypeArguments.Length == arguments.Length);
        var expressions = contract is null
            ? EquatableArray<string>.Empty
            : arguments.Select((a, index) => "(" + contract.TypeArguments[index].FullyQualified() + ")" + ( a.IsNull ? "null!" : a.ToCSharpString() )).ToEquatableArray();
        return new(name, type.FullyQualified(), contract?.FullyQualified(), expressions, key, Scopes(type));
    }

    private static void Collect(INamespaceSymbol @namespace, Dictionary<string, INamedTypeSymbol> table)
    {
        foreach (var member in @namespace.GetMembers())
        {
            if (member is INamespaceSymbol nested) Collect(nested, table);
            else if (member is INamedTypeSymbol type) Collect(type, table);
        }
    }

    private static void Collect(INamedTypeSymbol type, Dictionary<string, INamedTypeSymbol> table)
    {
        if (type.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(PolicyAttribute)) is { ConstructorArguments: [{ Value: string name }] } && !table.ContainsKey(name))
        {
            table.Add(name, type);
        }

        foreach (var nested in type.GetTypeMembers()) Collect(nested, table);
    }
}
