using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Rocket.Surgery.LaunchPad.Analyzers.Api.Core;
using static Rocket.Surgery.LaunchPad.Analyzers.Api.Core.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Analyzers.Api.Grpc;

/// <summary>
///     Implements proto-first gRPC services: a host deriving the service base Grpc.Tools generated from a <c>.proto</c>
///     (<c>partial class RocketsService : Rockets.RocketsBase</c>) gets an override of each listed operation's rpc, which maps the proto request to
///     the operation's request, sends it through the mediator and maps the response back, plus <c>Map{Host}()</c>.
/// </summary>
/// <remarks>
///     The proto messages are mapped with ZeroAlloc.Mapping, through mappings the user declares (<c>[Map&lt;GetRocketRequest, GetRocket.Request&gt;]</c>
///     on a static partial class): a generator can't see another generator's output, so attributes generated here would never get bodies.
/// </remarks>
[Generator]
public sealed class GrpcProtoServiceGenerator : IIncrementalGenerator
{
    private const string Ct = "__cancellationToken";
    private const string Generated = "global::Rocket.Surgery.LaunchPad.Api.Generated.Grpc";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var hosts = HostPipeline.Hosts(context, HostKind.GrpcProtoService);
        var maps = context.SyntaxProvider.ForAttributeWithMetadataName(KnownNames.MapAttribute, IsClass, static (c, _) => Mappings(c, reverse: false)).Collect();
        var reverseMaps = context.SyntaxProvider.ForAttributeWithMetadataName(KnownNames.ReverseMapAttribute, IsClass, static (c, _) => Mappings(c, reverse: true)).Collect();
        var referenced = context.CompilationProvider.Select(static (compilation, _) => compilation.GetTypeByMetadataName(KnownNames.MapAttribute) is { });
        var mappings = maps.Combine(reverseMaps)
                           .Select(static (pair, _) => Flatten(pair.Left, pair.Right))
                           .WithTrackingName("ProtoMappings");
        context.RegisterSourceOutput(hosts.Combine(mappings).Combine(referenced), static (output, input) => Emit(output, input.Left.Left, input.Left.Right, input.Right));
    }

    private static bool IsClass(SyntaxNode node, CancellationToken _) => node is ClassDeclarationSyntax;

    private static EquatableArray<MappingModel> Flatten(System.Collections.Immutable.ImmutableArray<EquatableArray<MappingModel>> maps, System.Collections.Immutable.ImmutableArray<EquatableArray<MappingModel>> reverseMaps)
    {
        var result = new List<MappingModel>();
        foreach (var mapper in maps)
        {
            foreach (var mapping in mapper) result.Add(mapping);
        }

        foreach (var mapper in reverseMaps)
        {
            foreach (var mapping in mapper) result.Add(mapping);
        }

        return result.ToEquatableArray();
    }

    // [Map<S, D>] gives Map(S) → D; [ReverseMap<S, D>] gives both directions.
    private static EquatableArray<MappingModel> Mappings(GeneratorAttributeSyntaxContext context, bool reverse)
    {
        var mapperType = (INamedTypeSymbol)context.TargetSymbol;
        var mapper = mapperType.FullyQualified();
        var result = new List<MappingModel>();
        foreach (var attribute in context.Attributes)
        {
            if (attribute.AttributeClass is not { TypeArguments: [var source, var destination] }) continue;
            result.Add(new(mapper, source.FullyQualified(), destination.FullyQualified(), IsProtoMessage(destination), DeclaresInPlace(mapperType, source, destination)));
            if (reverse) result.Add(new(mapper, destination.FullyQualified(), source.FullyQualified(), IsProtoMessage(source), DeclaresInPlace(mapperType, destination, source)));
        }

        return result.ToEquatableArray();
    }

    // ZeroAlloc.Mapping's update-in-place form, partial void Map(TSrc src, TDst existingDst): the one that sets settable properties.
    private static bool DeclaresInPlace(INamedTypeSymbol mapper, ITypeSymbol source, ITypeSymbol destination)
    {
        foreach (var member in mapper.GetMembers("Map"))
        {
            if (member is IMethodSymbol { IsStatic: true, ReturnsVoid: true, Parameters: [var from, var to] }
             && SymbolEqualityComparer.Default.Equals(from.Type, source)
             && SymbolEqualityComparer.Default.Equals(to.Type, destination))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsProtoMessage(ITypeSymbol type)
    {
        foreach (var contract in type.AllInterfaces)
        {
            if (contract.Is(KnownNames.ProtobufMessage)) return true;
        }

        return false;
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts, EquatableArray<MappingModel> mappings, bool mappingReferenced)
    {
        if (hosts.IsEmpty) return;
        if (mappingReferenced) output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.GrpcProtoHelpers.g.cs", GrpcProtoHelpers);
        var mappers = new Dictionary<(string Source, string Destination), MappingModel>();
        foreach (var mapping in mappings)
        {
            if (!mappers.ContainsKey((mapping.Source, mapping.Destination))) mappers.Add((mapping.Source, mapping.Destination), mapping);
        }

        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is null) continue;
            if (!mappingReferenced)
            {
                output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.MappingNotReferenced, host.Location, host.Name).ToDiagnostic());
                continue;
            }

            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".GrpcProto.g.cs", Write(host, host.Profile, mappers, output));
        }
    }

    private static string Write(HostModel host, ProfileModel profile, Dictionary<(string Source, string Destination), MappingModel> mappers, SourceProductionContext output)
    {
        var methods = new List<MemberDeclarationSyntax>();
        foreach (var operation in host.Operations)
        {
            if (ServerDispatch.ServiceType(profile, operation) is not { } service)
            {
                output.ReportDiagnostic(
                    DiagnosticInfo.Create(ApiDiagnostics.FeatureSkipped, operation.Location, operation.Name, host.Name, $"the {profile.Name} profile cannot dispatch {ServerEmit.KindName(operation.Kind)} operations")
                                  .ToDiagnostic()
                );
                continue;
            }

            if (Operation(host, profile, operation, service, mappers, output) is { } method) methods.Add(method);
        }

        var version = typeof(GrpcProtoServiceGenerator).Assembly.GetName().Version;
        var implementation = Class(host.Name, Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword), methods)
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Grpc", version));
        var map = Method(
                Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                Type("global::Microsoft.AspNetCore.Builder.GrpcServiceEndpointConventionBuilder"),
                $"Map{host.Name}",
                Parameters(This(Type("global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder"), "endpoints")),
                [Return(Invoke(GenericMember(Name("global::Microsoft.AspNetCore.Builder.GrpcEndpointRouteBuilderExtensions"), "MapGrpcService", SyntaxFactory.IdentifierName(host.Name)), Id("endpoints")))]
            )
           .Summary($"Maps <see cref=\"{host.Name}\" />. Register gRPC with <c>services.AddGrpc()</c>.");
        var extensions = Class($"{host.Name}EndpointRouteBuilderExtensions", Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword), [map])
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Grpc", version));
        return Render(host.Namespace, [implementation, extensions]);
    }

    private static MethodDeclarationSyntax? Operation(
        HostModel host,
        ProfileModel profile,
        OperationModel operation,
        string service,
        Dictionary<(string Source, string Destination), MappingModel> mappers,
        SourceProductionContext output
    )
    {
        var rpc = operation.Rpc!;
        // The mappings this operation needs, declared by the user: the proto request to the request (or its input type), and the response
        // (the union's value case) to the proto response. A void operation answers an empty proto response.
        var missing = false;
        // A proto message is filled in place: ZeroAlloc.Mapping's constructor form would leave it empty.
        ExpressionSyntax Call(MappingModel mapping, ExpressionSyntax value)
        {
            if (mapping.InPlace) return Invoke(Member($"{Generated}.ApiGrpcProtoMapping", "Into"), value, New(Type(mapping.Destination)), Member(mapping.Mapper, "Map"));
            if (!mapping.DestinationIsProtoMessage) return Invoke(Member(mapping.Mapper, "Map"), value);
            output.ReportDiagnostic(
                DiagnosticInfo.Create(ApiDiagnostics.MappingNotInPlace, operation.Location, mapping.Mapper.Replace("global::", ""), mapping.Source.Replace("global::", ""), mapping.Destination.Replace("global::", ""))
                              .ToDiagnostic()
            );
            missing = true;
            return Default;
        }

        ExpressionSyntax Map(string source, string destination, ExpressionSyntax value)
        {
            if (mappers.TryGetValue((source, destination), out var mapping)) return Call(mapping, value);
            output.ReportDiagnostic(
                DiagnosticInfo.Create(ApiDiagnostics.MappingNotDeclared, operation.Location, operation.Name, host.Name, source.Replace("global::", ""), destination.Replace("global::", ""))
                              .ToDiagnostic()
            );
            missing = true;
            return Default;
        }

        var mappedRequest = Map(rpc.RequestMessage, operation.WireType, Id("request"));
        ExpressionSyntax? Response(ExpressionSyntax? value) =>
            value is null ? New(Type(rpc.ResponseMessage)) : Map(operation.ResponseTypeNotNull!, rpc.ResponseMessage, value);
        // Checked up front, so a missing response mapping is reported once, not per case.
        if (operation.Kind != RequestKind.Void) Response(Id("__value"));
        // An error case mapped to a proto message travels in the case-bin trailer, so its mapping is checked the same way.
        foreach (var error in operation.Union?.Errors ?? EquatableArray<UnionCaseModel>.Empty)
        {
            if (FindProtoMessage(mappers, error.Type) is { } mapping) Call(mapping, Id("__error"));
        }

        if (missing) return null;

        var send = ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct));
        var body = new List<StatementSyntax>
        {
            Var(
                ServerDispatch.SenderParameter,
                GetRequiredService(Type(service), Member(Invoke(Member("global::Grpc.Core.ServerCallContextExtensions", "GetHttpContext"), Id("context")), "RequestServices"))
            ),
            Var(Ct, Member(Id("context"), "CancellationToken")),
            Var("__request", operation.InputType is { } ? Invoke(Member(mappedRequest, "Create")) : mappedRequest),
        };
        var notFound = Throw(
            New(
                Type("global::Grpc.Core.RpcException"),
                New(Type("global::Grpc.Core.Status"), Member("global::Grpc.Core.StatusCode", "NotFound"), Literal(operation.Name + " found nothing"))
            )
        );
        StatementSyntax Write(ExpressionSyntax value) => Statement(Await(Invoke(Member(Id("responseStream"), "WriteAsync"), Response(value)!)));

        // A union: the value case answers as usual, each error case is the case's status with its name in the case trailer and, when the user
        // maps the case to a proto message, that message in the case-bin trailer.
        StatementSyntax[] Error(UnionCaseModel error, ExpressionSyntax value)
        {
            var message = FindProtoMessage(mappers, error.Type) is { } mapping ? Call(mapping, value) : Null;
            return
            [
                Throw(
                    Invoke(
                        Member($"{Generated}.ApiGrpcProtoErrors", "Error"),
                        new[] { Member("global::Grpc.Core.StatusCode", error.GrpcStatus), Literal(error.Name), message }
                           .Concat(error.DetailMember is { } detail ? new[] { Member(value, detail) } : [])
                    )
                ),
            ];
        }

        if (rpc.ServerStreaming)
        {
            var item = operation.Union is { }
                ? Block(ServerEmit.UnionResult(operation, value => [Write(value!), Continue], Error, null, "__item"))
                : Block(Write(Id("__item")));
            body.Add(AwaitForEach("__item", ServerDispatch.Stream(send), Id(Ct), item));
        }
        else if (operation.Union is { })
        {
            body.Add(Var("__response", Await(send.Call)));
            body.AddRange(ServerEmit.UnionResult(operation, value => [Return(Response(value))], Error, operation.ResponseNullable ? () => [notFound] : null));
        }
        else if (operation.Kind == RequestKind.Void)
        {
            body.Add(Statement(Await(send.Call)));
            body.Add(Return(Response(null)));
        }
        else
        {
            body.Add(Var("__response", Await(send.Call)));
            if (operation.ResponseNullable) body.Add(If(IsNull(Id("__response")), notFound));
            body.Add(Return(Response(operation.ResponseIsNullableValueType ? Member(Id("__response"), "Value") : Id("__response"))));
        }

        var parameters = rpc.ServerStreaming
            ? Parameters(
                Parameter(Type(rpc.RequestMessage), "request"),
                Parameter(Generic("global::Grpc.Core.IServerStreamWriter", Type(rpc.ResponseMessage)), "responseStream"),
                Parameter(Type("global::Grpc.Core.ServerCallContext"), "context")
            )
            : Parameters(Parameter(Type(rpc.RequestMessage), "request"), Parameter(Type("global::Grpc.Core.ServerCallContext"), "context"));
        return Method(
                Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.OverrideKeyword, SyntaxKind.AsyncKeyword),
                rpc.ServerStreaming ? Task() : Task(Type(rpc.ResponseMessage)),
                rpc.Method,
                parameters,
                body
            )
           .Authorize(operation)
           .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> through the mediator.");
    }

    // The proto message the user maps an error case to, if any: its case-bin trailer.
    private static MappingModel? FindProtoMessage(Dictionary<(string Source, string Destination), MappingModel> mappers, string source)
    {
        foreach (var mapping in mappers.Values)
        {
            if (mapping.DestinationIsProtoMessage && string.Equals(mapping.Source, source, StringComparison.Ordinal)) return mapping;
        }

        return null;
    }

    private const string GrpcProtoHelpers = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace Rocket.Surgery.LaunchPad.Api.Generated.Grpc
{
    /// <summary>
    ///     Answers an error case of a union response on a proto-first service: an RpcException with the case's status, its name in the <c>case</c>
    ///     trailer and, when the case is mapped to a proto message, that message in the <c>case-bin</c> trailer.
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiGrpcProtoErrors
    {
        public static global::Grpc.Core.RpcException Error(global::Grpc.Core.StatusCode status, string caseName, global::Google.Protobuf.IMessage? message, string? detail = null)
        {
            var trailers = new global::Grpc.Core.Metadata { { "case", caseName } };
            if (message is not null) trailers.Add("case-bin", global::Google.Protobuf.MessageExtensions.ToByteArray(message));
            return new global::Grpc.Core.RpcException(new global::Grpc.Core.Status(status, detail ?? caseName), trailers, detail ?? caseName);
        }
    }

    /// <summary>Fills a new proto message through ZeroAlloc.Mapping's update-in-place form: proto messages have settable properties, no constructor parameters.</summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiGrpcProtoMapping
    {
        public static TDestination Into<TSource, TDestination>(TSource source, TDestination destination, global::System.Action<TSource, TDestination> map)
        {
            map(source, destination);
            return destination;
        }
    }
}
""";
}
