using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

internal enum HostKind
{
    Unknown,
    MinimalApi,
    Controller,
    GrpcService,
    GraphQLQuery,
    GraphQLMutation,
    HttpClient,
    GrpcClient,
    GraphQLClient,
}

internal static class KnownNames
{
    public const string Namespace = "Rocket.Surgery.LaunchPad.Api";
    public const string OperationAttribute = Namespace + ".OperationAttribute`1";
    public const string CreateOperationAttribute = Namespace + ".CreateOperationAttribute`2";
    public const string OperationHostAttribute = Namespace + ".OperationHostAttribute";
    public const string ApiOperationAttribute = Namespace + ".ApiOperationAttribute";
    public const string MediatorProfileAttribute = Namespace + ".MediatorProfileAttribute";
    public const string MediatorRequestAttribute = Namespace + ".MediatorRequestAttribute";
    public const string ApiRouteAttribute = Namespace + ".ApiRouteAttribute";
    public const string ApiQueryAttribute = Namespace + ".ApiQueryAttribute";
    public const string ApiHeaderAttribute = Namespace + ".ApiHeaderAttribute";
    public const string ApiBodyAttribute = Namespace + ".ApiBodyAttribute";
    public const string ApiServiceAttribute = Namespace + ".ApiServiceAttribute";
    public const string ApiIgnoreAttribute = Namespace + ".ApiIgnoreAttribute";

    public const string ControllerBase = "Microsoft.AspNetCore.Mvc.ControllerBase";
    public const string EndpointRouteBuilder = "Microsoft.AspNetCore.Routing.IEndpointRouteBuilder";
    public const string HttpClientFactory = "System.Net.Http.IHttpClientFactory";
    public const string ExtendObjectTypeAttribute = "HotChocolate.Types.ExtendObjectTypeAttribute";
    public const string ServiceContractAttribute = "System.ServiceModel.ServiceContractAttribute";
    public const string GrpcCallContext = "ProtoBuf.Grpc.CallContext";

    public static string MetadataName(this ITypeSymbol symbol)
    {
        var type = symbol is INamedTypeSymbol named ? named.OriginalDefinition : symbol;
        return type.ContainingType is { } containing
            ? containing.MetadataName() + "+" + type.MetadataName
            : type.ContainingNamespace is { IsGlobalNamespace: false } ns
                ? ns.ToDisplayString() + "." + type.MetadataName
                : type.MetadataName;
    }

    public static bool Is(this ITypeSymbol? symbol, string metadataName) => symbol is { } && symbol.MetadataName() == metadataName;

    public static bool InheritsFrom(this ITypeSymbol symbol, string metadataName)
    {
        for (var current = symbol.BaseType; current is { }; current = current.BaseType)
        {
            if (current.Is(metadataName)) return true;
        }

        return false;
    }

    public static string FullyQualified(this ITypeSymbol symbol) => symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);

    public static string FullyQualifiedNullable(this ITypeSymbol symbol) =>
        symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat.AddMiscellaneousOptions(SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier));
}

/// <summary>
///     Infers an operation host's transport from what the host declares (base type, transport attribute) and, failing that, from its name.
///     The rules are listed in the design document and in the LPAD1017 message.
/// </summary>
internal static class HostInference
{
    // Checked in order, so GrpcClient and GraphQLClient win over Client. A non-static *Endpoints class is still a Minimal API host;
    // LPAD1010 (with a code fix) asks for `static partial`.
    private static readonly (string Suffix, HostKind Kind, string Requires, string Library)[] NameRules =
    [
        ("Controller", HostKind.Controller, KnownNames.ControllerBase, "ASP.NET Core MVC"),
        ("Endpoints", HostKind.MinimalApi, KnownNames.EndpointRouteBuilder, "ASP.NET Core"),
        ("Api", HostKind.MinimalApi, KnownNames.EndpointRouteBuilder, "ASP.NET Core"),
        ("Service", HostKind.GrpcService, KnownNames.GrpcCallContext, "protobuf-net.Grpc"),
        ("Queries", HostKind.GraphQLQuery, KnownNames.ExtendObjectTypeAttribute, "HotChocolate"),
        ("Mutations", HostKind.GraphQLMutation, KnownNames.ExtendObjectTypeAttribute, "HotChocolate"),
        ("GrpcClient", HostKind.GrpcClient, KnownNames.GrpcCallContext, "protobuf-net.Grpc"),
        ("GraphQLClient", HostKind.GraphQLClient, KnownNames.HttpClientFactory, "Microsoft.Extensions.Http"),
        ("Client", HostKind.HttpClient, KnownNames.HttpClientFactory, "Microsoft.Extensions.Http"),
    ];

    public static (HostKind Kind, string? Problem) Infer(INamedTypeSymbol host, Compilation compilation)
    {
        if (host.InheritsFrom(KnownNames.ControllerBase)) return Require(HostKind.Controller, compilation, KnownNames.ControllerBase, "ASP.NET Core MVC");

        var extend = host.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.ExtendObjectTypeAttribute));
        if (extend is { })
        {
            var operation = extend.ConstructorArguments.FirstOrDefault().Value as string
             ?? extend.NamedArguments.FirstOrDefault(n => n.Key == "Name").Value.Value as string;
            return operation switch
            {
                "Query" => (HostKind.GraphQLQuery, null),
                "Mutation" => (HostKind.GraphQLMutation, null),
                _ => (HostKind.Unknown, $"[ExtendObjectType] names '{operation}', not Query or Mutation"),
            };
        }

        foreach (var (suffix, kind, requires, library) in NameRules)
        {
            if (host.Name.EndsWith(suffix, StringComparison.Ordinal)) return Require(kind, compilation, requires, library);
        }

        return (HostKind.Unknown, "neither its base type, its attributes nor its name match a transport");
    }

    public static string Describe(this HostKind kind) => kind switch
    {
        HostKind.MinimalApi => "Minimal API",
        HostKind.Controller => "MVC controller",
        HostKind.GrpcService => "gRPC service",
        HostKind.GraphQLQuery => "GraphQL query",
        HostKind.GraphQLMutation => "GraphQL mutation",
        HostKind.HttpClient => "HTTP client",
        HostKind.GrpcClient => "gRPC client",
        HostKind.GraphQLClient => "GraphQL client",
        HostKind.Unknown => "unknown",
        _ => throw new ArgumentOutOfRangeException(nameof(kind)),
    };

    /// <summary>Whether the transport is HTTP with routes and verbs (Minimal API, MVC, the HTTP client).</summary>
    public static bool IsHttp(this HostKind kind) => kind is HostKind.MinimalApi or HostKind.Controller or HostKind.HttpClient;

    public static bool IsClient(this HostKind kind) => kind is HostKind.HttpClient or HostKind.GrpcClient or HostKind.GraphQLClient;

    private static (HostKind, string?) Require(HostKind kind, Compilation compilation, string metadataName, string library) =>
        compilation.GetTypeByMetadataName(metadataName) is { }
            ? (kind, null)
            : (HostKind.Unknown, $"it looks like a {kind.Describe()} host, but {library} is not referenced");
}
