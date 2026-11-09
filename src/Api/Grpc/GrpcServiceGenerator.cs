using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates a code-first gRPC service (protobuf-net.Grpc) for every <c>*Service</c> operation host: a <c>[Service]</c> contract interface
///     with one method per operation, the host's implementation of it through the mediator, and <c>Map{Host}()</c>.
/// </summary>
/// <remarks>
///     The contract is generated in the server, so the contracts project needs no gRPC reference. Clients are generated from the service's
///     <c>.proto</c>, which protobuf-net can write for the contract.
/// </remarks>
[Generator]
public sealed class GrpcServiceGenerator : IIncrementalGenerator
{
    private const string Ct = "__cancellationToken";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var hosts = HostPipeline.Hosts(context, HostKind.GrpcService);
        context.RegisterSourceOutput(hosts, static (output, models) => Emit(output, models));
    }

    private const string Generated = "global::Rocket.Surgery.LaunchPad.Api.Generated.Grpc";

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts)
    {
        // The helpers use the transport's library, so only a project with a host of this kind gets them.
        if (!hosts.IsEmpty) output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.GrpcHelpers.g.cs", GrpcHelpers);
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is null) continue;
            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".Grpc.g.cs", Write(host, host.Profile, output));
        }
    }

    private static string Write(HostModel host, ProfileModel profile, SourceProductionContext output)
    {
        var operations = new List<(OperationModel Operation, string Service)>();
        foreach (var operation in host.Operations)
        {
            if (ServerDispatch.ServiceType(profile, operation) is { } service)
            {
                operations.Add((operation, service));
                continue;
            }

            output.ReportDiagnostic(
                DiagnosticInfo.Create(ApiDiagnostics.FeatureSkipped, operation.Location, operation.Name, host.Name, $"the {profile.Name} profile cannot dispatch {ServerEmit.KindName(operation.Kind)} operations")
                              .ToDiagnostic()
            );
        }

        var version = typeof(GrpcServiceGenerator).Assembly.GetName().Version;
        var contract = GrpcEmit.Contract(host, operations.Select(o => o.Operation), version);
        var implementation = Class(
                          host.Name,
                          Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword),
                          GrpcEmit.ModelConfiguration(host, operations.Select(o => o.Operation).ToArray())
                                  .Concat(host.CanExportProto ? [GetProto(host)] : [])
                                  .Concat(operations.Select(o => Operation(host, profile, o.Operation, o.Service))),
                          SyntaxFactory.IdentifierName(GrpcEmit.ContractName(host))
                      )
                     .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Grpc", version));
        var map = Method(
                Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                Type("global::Microsoft.AspNetCore.Builder.GrpcServiceEndpointConventionBuilder"),
                $"Map{host.Name}",
                Parameters(This(Type("global::Microsoft.AspNetCore.Routing.IEndpointRouteBuilder"), "endpoints")),
                [
                    Statement(Invoke(Member(Id(host.Name), "ConfigureGrpcModel"))),
                    Return(Invoke(GenericMember(Name("global::Microsoft.AspNetCore.Builder.GrpcEndpointRouteBuilderExtensions"), "MapGrpcService", SyntaxFactory.IdentifierName(host.Name)), Id("endpoints"))),
                ]
            )
           .Summary($"Maps <see cref=\"{host.Name}\" />. Register code-first gRPC with <c>services.AddCodeFirstGrpc()</c>.");
        var extensions = Class($"{host.Name}EndpointRouteBuilderExtensions", Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword), [map])
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Grpc", version));
        return Render(host.Namespace, [contract, implementation, extensions]);
    }

    // The service's .proto, for generating clients in any language: protobuf-net's schema of the generated contract, plus the messages of the
    // union error cases its operations send in the case-bin trailer, so clients can decode them.
    private static MemberDeclarationSyntax GetProto(HostModel host)
    {
        var generator = Type("global::ProtoBuf.Grpc.Reflection.SchemaGenerator");
        var contract = SyntaxFactory.IdentifierName(GrpcEmit.ContractName(host));
        var cases = GrpcEmit.ErrorCaseMessages(host.Operations).ToArray();
        IEnumerable<StatementSyntax> body = cases.Length == 0
            ? [Statement(Invoke(Id("ConfigureGrpcModel"))), Return(Invoke(GenericMember(New(generator), "GetSchema", contract)))]
            :
            [
                Statement(Invoke(Id("ConfigureGrpcModel"))),
                Var("__generator", New(generator)),
                .. cases.Select(c => Statement(Invoke(Member(Member(Member(Id("__generator"), "Options"), "Types"), "Add"), TypeOf(Type(c.Type))))),
                Return(Invoke(GenericMember(Id("__generator"), "GetSchema", contract))),
            ];
        return Method(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword), Type("string"), "GetProto", Parameters(), body)
           .Summary($"The <c>.proto</c> of service <c>{GrpcEmit.PackageName(host)}.{GrpcEmit.ServiceName(host)}</c>, to generate clients from (protobuf-net.Grpc.Reflection).");
    }

    private static MethodDeclarationSyntax Operation(HostModel host, ProfileModel profile, OperationModel operation, string service)
    {
        var send = ServerDispatch.Send(profile, operation, Id("__request"), Id(Ct));
        // A stream is relayed item by item when it has to be awaited first (Foundatio, Immediate.Handlers) or its items wrapped.
        var relay = operation.Kind == RequestKind.Stream && ( send.Awaited || !operation.ResponseIsMessage );
        var isAsync = operation.Kind != RequestKind.Stream || relay;
        var (returnType, parameters) = GrpcEmit.Signature(host, operation, withDefault: false);
        var wire = operation.RequestIsMessage ? Id("request") : Member(Id("request"), "Value");
        var body = new List<StatementSyntax>
        {
            Var(
                ServerDispatch.SenderParameter,
                GetRequiredService(
                    Type(service),
                    Member(Invoke(Member("global::Grpc.Core.ServerCallContextExtensions", "GetHttpContext"), NullForgiving(Member(Id("context"), "ServerCallContext"))), "RequestServices")
                )
            ),
            Var(Ct, Member(Id("context"), "CancellationToken")),
            Var("__request", operation.InputType is { } ? Invoke(Member(wire, "Create")) : wire),
        };
        var notFound = Throw(
            New(
                Type("global::Grpc.Core.RpcException"),
                New(Type("global::Grpc.Core.Status"), Member("global::Grpc.Core.StatusCode", "NotFound"), Literal(operation.Name + " found nothing"))
            )
        );
        if (operation.Union is { })
        {
            // A union: the value case answers as usual, each error case is the case's status with its name and members in the trailers. A
            // stream of unions relays its value cases and ends with the first error case.
            StatementSyntax[] Error(UnionCaseModel error, ExpressionSyntax value) =>
            [
                Throw(
                    Invoke(
                        Member($"{Generated}.ApiGrpcErrors", "Error"),
                        Member("global::Grpc.Core.StatusCode", error.GrpcStatus),
                        Literal(error.Name),
                        error.Members.IsEmpty ? Null : value
                    )
                ),
            ];
            ExpressionSyntax Wire(ExpressionSyntax value) => operation.ResponseIsMessage ? value : New(GrpcEmit.WireResponse(host, operation), value);
            if (operation.Kind == RequestKind.Stream)
            {
                body.Add(
                    AwaitForEach(
                        "__item",
                        ServerDispatch.Stream(send),
                        Id(Ct),
                        Block(ServerEmit.UnionResult(operation, value => [YieldReturn(Wire(value!)), Continue], Error, null, "__item"))
                    )
                );
            }
            else
            {
                body.Add(Var("__response", Await(send.Call)));
                body.AddRange(
                    ServerEmit.UnionResult(
                        operation,
                        value => [value is null ? Return() : Return(Wire(value))],
                        Error,
                        operation.ResponseNullable ? () => [notFound] : null
                    )
                );
            }

            return Method(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword), returnType, operation.Name, parameters, body)
               .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> through the mediator.");
        }

        switch (operation.Kind)
        {
            case RequestKind.Stream:
                body.Add(
                    relay
                        ? AwaitForEach("__item", ServerDispatch.Stream(send), Id(Ct), YieldReturn(operation.ResponseIsMessage ? Id("__item") : New(GrpcEmit.WireResponse(host, operation), Id("__item"))))
                        : Return(send.Call)
                );
                break;
            case RequestKind.Void:
                body.Add(Statement(Await(send.Call)));
                break;
            case RequestKind.Unary:
            case RequestKind.Infer:
            default:
                body.Add(Var("__response", Await(send.Call)));
                if (operation.ResponseNullable) body.Add(If(IsNull(Id("__response")), notFound));

                var value = operation.ResponseIsNullableValueType ? Member(Id("__response"), "Value") : Id("__response");
                body.Add(Return(operation.ResponseIsMessage ? value : New(GrpcEmit.WireResponse(host, operation), value)));
                break;
        }

        return Method(isAsync ? Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword) : Modifiers(SyntaxKind.PublicKeyword), returnType, operation.Name, parameters, body)
                        .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> through the mediator.");
    }

    private const string GrpcHelpers = """
// <auto-generated/>
#nullable enable
#pragma warning disable
namespace Rocket.Surgery.LaunchPad.Api.Generated.Grpc
{
    /// <summary>
    ///     Answers an error case of a union response: an RpcException with the case's status, its name in the <c>case</c> trailer and the case
    ///     itself, serialized by protobuf-net, in the <c>case-bin</c> trailer (left out for a case without members).
    /// </summary>
    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
    internal static class ApiGrpcErrors
    {
        public static global::Grpc.Core.RpcException Error(global::Grpc.Core.StatusCode status, string caseName, object? value)
        {
            var trailers = new global::Grpc.Core.Metadata { { "case", caseName } };
            if (value is not null)
            {
                using var stream = new global::System.IO.MemoryStream();
                global::ProtoBuf.Meta.RuntimeTypeModel.Default.Serialize(stream, value);
                trailers.Add("case-bin", stream.ToArray());
            }

            return new global::Grpc.Core.RpcException(new global::Grpc.Core.Status(status, caseName), trailers, caseName);
        }
    }
}
""";
}
