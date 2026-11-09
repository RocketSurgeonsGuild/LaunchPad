using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>One member of an error case, carried in the transport's error shape.</summary>
/// <param name="Name">The C# property name.</param>
/// <param name="WireName">The camel-cased name on the wire (a ProblemDetails extension, a GraphQL error extension).</param>
internal sealed record UnionMemberModel(string Name, string WireName);

/// <summary>An error case of a union response and what each transport answers for it.</summary>
/// <param name="Name">The case name the wire carries: the case type's simple name (<c>NotFound</c>).</param>
/// <param name="Type">The case type, fully qualified.</param>
/// <param name="StatusCode">The HTTP status.</param>
/// <param name="GrpcStatus">The <c>Grpc.Core.StatusCode</c> member name.</param>
/// <param name="GraphQLCode">The GraphQL error category (<c>NOT_FOUND</c>), carried next to the case name.</param>
/// <param name="NestedName">For a nested case type, its name with its container's in front (<c>GetRocketNotFound</c>), as protobuf-net names it.</param>
/// <param name="IsMessage">The case is a class protobuf-net can serialize as it is (the gRPC <c>case-bin</c> trailer).</param>
internal sealed record UnionCaseModel(
    string Name,
    string Type,
    int StatusCode,
    string GrpcStatus,
    string GraphQLCode,
    EquatableArray<UnionMemberModel> Members,
    string? NestedName,
    bool IsMessage
);

/// <summary>
///     A response that is a union of its value case and its error cases. The operation's response type is the value case; the handler
///     returns <see cref="Type" />, and the server answers each error case in its transport's error shape.
/// </summary>
/// <param name="Type">The union type the handler returns, fully qualified.</param>
/// <param name="ValueInterface">
///     The <c>IUnion</c> the server reads <c>Value</c> through, when the union doesn't expose it publicly (an explicit implementation).
/// </param>
/// <param name="Nullable">The handler returns <c>U?</c>: a null union answers like a null value case.</param>
internal sealed record UnionModel(string Type, string? ValueInterface, bool Nullable, EquatableArray<UnionCaseModel> Errors);

/// <summary>
///     Union responses: how the generators recognise a union type and what each of its error cases answers.
/// </summary>
/// <remarks>
///     <para>
///         <see cref="TryRead" /> is the one place that knows what a union looks like, so it can follow LaunchPad's union types
///         (RocketSurgeonsGuild/LaunchPad#3147) without touching the emitters. It follows C# 15: a type marked with
///         <c>System.Runtime.CompilerServices.UnionAttribute</c> (matched by full name, since on .NET 10 every assembly has its own internal
///         polyfill), whose case types are the parameter types of its public
///         one-parameter constructors, in declaration order, and whose current case is its <c>object? Value</c>. A <c>union</c> declaration
///         lowers to exactly that; so does #3147's portable <c>[Union] partial struct</c>, whose partial constructors are in source before its
///         generator implements them.
///     </para>
///     <para>
///         An error case is known by <c>[ApiError]</c> on its type or by its name; the value case is the first case that is neither, and any
///         other case that is neither is LPAD1025. A union whose cases are all error cases takes its first case as the value.
///     </para>
/// </remarks>
internal static class UnionResults
{
    public const string UnionAttribute = "System.Runtime.CompilerServices.UnionAttribute";
    public const string UnionInterface = "System.Runtime.CompilerServices.IUnion";

    /// <summary>
    ///     A case name's statuses, matched in this order: the first rule whose word the name starts or ends with wins. The table is the one
    ///     #3147 lists for LaunchPad's standard error cases (<c>Rocket.Surgery.LaunchPad.Primitives.NotFound</c>, <c>Invalid</c>, …), whose
    ///     names it matches.
    /// </summary>
    private static readonly (string Word, int Status, string Grpc, string GraphQL)[] Rules =
    [
        ("NotFound", 404, "NotFound", "NOT_FOUND"),
        ("Unauthorized", 401, "Unauthenticated", "UNAUTHORIZED"),
        ("Unauthenticated", 401, "Unauthenticated", "UNAUTHORIZED"),
        ("Forbidden", 403, "PermissionDenied", "FORBIDDEN"),
        ("Conflict", 409, "FailedPrecondition", "CONFLICT"),
        ("Validation", 400, "InvalidArgument", "INVALID"),
        ("Invalid", 400, "InvalidArgument", "INVALID"),
    ];

    /// <summary>Endings ignored when a case name is matched: <c>RocketNotFoundError</c> is a not-found case.</summary>
    private static readonly string[] Endings = ["Error", "Exception", "Problem", "Failure", "Result"];

    private static readonly string[] UnitTypes = ["MediatR.Unit", "Mediator.Unit", "ChannelMediator.Unit", "ZeroAlloc.Mediator.Unit", "System.ValueTuple"];

    /// <summary>A value case that means "no value": a mediator's unit type, or <c>ValueTuple</c>.</summary>
    public static bool IsUnit(ITypeSymbol type) => UnitTypes.Any(type.Is);

    /// <summary>The words a case name can match, for the diagnostic.</summary>
    public static string Words => "NotFound, Invalid, Validation, Unauthorized, Forbidden or Conflict";

    /// <summary>
    ///     Reads <paramref name="type" /> as a union: its case types in declaration order, and the <c>IUnion</c> to read <c>Value</c> through
    ///     when the union doesn't expose it publicly. False when it isn't a union or has fewer than two cases.
    /// </summary>
    public static bool TryRead(ITypeSymbol? type, out INamedTypeSymbol union, out IReadOnlyList<ITypeSymbol> cases, out string? valueInterface)
    {
        union = null!;
        cases = [];
        valueInterface = null;
        if (type is not INamedTypeSymbol named) return false;
        // Nullable<U> of a struct union: the union is U.
        if (named.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T && named.TypeArguments[0] is INamedTypeSymbol inner) named = inner;
        // #3147's contract: the attribute, by full name (on .NET 10 every assembly has its own internal polyfill). IUnion alone gives a type
        // no union support in C#, so it doesn't count.
        if (!named.GetAttributes().Any(a => a.AttributeClass.Is(UnionAttribute))) return false;

        cases = named.InstanceConstructors
                     .Where(c => c is { DeclaredAccessibility: Accessibility.Public, IsImplicitlyDeclared: false, Parameters.Length: 1, IsGenericMethod: false })
                     .Where(c => c.Parameters[0].RefKind is RefKind.None or RefKind.In)
                     .Select(c => c.Parameters[0].Type)
                     .ToArray();
        if (cases.Count < 2) return false;

        union = named;
        var publicValue = named.GetMembers("Value").OfType<IPropertySymbol>().Any(p => p is { IsStatic: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } });
        valueInterface = publicValue ? null : named.AllInterfaces.FirstOrDefault(i => i.Is(UnionInterface))?.FullyQualified();
        return true;
    }

    /// <summary>
    ///     An error case's statuses: from <c>[ApiError]</c> on its type, else from its name. Null when neither says, which is LPAD1025.
    /// </summary>
    public static (int Status, string Grpc, string GraphQL)? Status(ITypeSymbol errorCase)
    {
        var attribute = errorCase.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.ApiErrorAttribute));
        var byName = FromName(errorCase.Name);
        if (attribute is null) return byName;

        var status = attribute.ConstructorArguments.FirstOrDefault().Value as int? ?? byName?.Status ?? 500;
        var named = attribute.NamedArguments.ToDictionary(kv => kv.Key, kv => kv.Value.Value);
        var grpc = named.TryGetValue("GrpcStatus", out var g) && g is int grpcStatus && GrpcStatusNames.TryGetValue(grpcStatus, out var grpcName)
            ? grpcName
            : byName is { } n && n.Status == status ? n.Grpc : GrpcFromHttp(status);
        var graphQL = named.TryGetValue("GraphQLCode", out var c) && c is string { Length: > 0 } code
            ? code
            : byName is { } m && m.Status == status ? m.GraphQL : GraphQLFromHttp(status);
        return (status, grpc, graphQL);
    }

    /// <summary>The statuses a case name implies, or null when it matches no rule.</summary>
    public static (int Status, string Grpc, string GraphQL)? FromName(string name)
    {
        foreach (var ending in Endings)
        {
            if (name.Length > ending.Length && name.EndsWith(ending, StringComparison.Ordinal))
            {
                name = name[..^ending.Length];
                break;
            }
        }

        foreach (var (word, status, grpc, graphQL) in Rules)
        {
            if (name.StartsWith(word, StringComparison.Ordinal) || name.EndsWith(word, StringComparison.Ordinal)) return (status, grpc, graphQL);
        }

        return null;
    }

    /// <summary>The camel-cased public, readable instance properties of an error case, carried as its members on the wire.</summary>
    public static EquatableArray<UnionMemberModel> Members(ITypeSymbol errorCase)
    {
        var members = new List<UnionMemberModel>();
        for (var current = errorCase; current is { SpecialType: not SpecialType.System_Object and not SpecialType.System_ValueType }; current = current.BaseType)
        {
            if (current.SpecialType != SpecialType.None) break;
            members.AddRange(
                current.GetMembers()
                       .OfType<IPropertySymbol>()
                       .Where(p => p is { IsStatic: false, IsIndexer: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } } && p.Name != "EqualityContract")
                       .Where(p => members.All(m => m.Name != p.Name))
                       .Select(p => new UnionMemberModel(p.Name, Naming.CamelCase(p.Name)))
            );
        }

        return members.ToEquatableArray();
    }

    // The gRPC status for an HTTP status an [ApiError] names without one: the mapping gRPC's own HTTP/2 gateway uses.
    private static string GrpcFromHttp(int status) => status switch
    {
        400 or 422 => "InvalidArgument",
        401 => "Unauthenticated",
        403 => "PermissionDenied",
        404 or 410 => "NotFound",
        409 => "FailedPrecondition",
        412 => "FailedPrecondition",
        429 => "ResourceExhausted",
        499 => "Cancelled",
        501 => "Unimplemented",
        503 => "Unavailable",
        504 => "DeadlineExceeded",
        >= 400 and < 500 => "FailedPrecondition",
        _ => "Internal",
    };

    private static string GraphQLFromHttp(int status) => status switch
    {
        400 or 422 => "INVALID",
        401 => "UNAUTHORIZED",
        403 => "FORBIDDEN",
        404 or 410 => "NOT_FOUND",
        409 => "CONFLICT",
        >= 400 and < 500 => "BAD_REQUEST",
        _ => "INTERNAL_SERVER_ERROR",
    };

    // Grpc.Core.StatusCode by value, so [ApiError(GrpcStatus = 5)] needs no gRPC reference in the contracts project.
    private static readonly Dictionary<int, string> GrpcStatusNames = new()
    {
        [0] = "OK",
        [1] = "Cancelled",
        [2] = "Unknown",
        [3] = "InvalidArgument",
        [4] = "DeadlineExceeded",
        [5] = "NotFound",
        [6] = "AlreadyExists",
        [7] = "PermissionDenied",
        [8] = "ResourceExhausted",
        [9] = "FailedPrecondition",
        [10] = "Aborted",
        [11] = "OutOfRange",
        [12] = "Unimplemented",
        [13] = "Internal",
        [14] = "Unavailable",
        [15] = "DataLoss",
        [16] = "Unauthenticated",
    };
}
