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
///     The gRPC client generator writes the same contract from a client host, so client and server agree on the wire through the service name
///     (the group) and the operation names alone; the contracts project needs no gRPC reference.
/// </remarks>
[Generator]
public sealed class GrpcServiceGenerator : IIncrementalGenerator
{
    private const string Ct = "__cancellationToken";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static output => HostPipeline.EmitMarker(output, HostKind.GrpcService));
        var hosts = HostPipeline.Hosts(context, HostKind.GrpcService);
        context.RegisterSourceOutput(hosts, static (output, models) => Emit(output, models));
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts)
    {
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
                          GrpcEmit.ModelConfiguration(operations.Select(o => o.Operation)).Concat(operations.Select(o => Operation(host, profile, o.Operation, o.Service))),
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
                if (operation.ResponseNullable)
                {
                    var notFound = New(
                        Type("global::Grpc.Core.RpcException"),
                        New(Type("global::Grpc.Core.Status"), Member("global::Grpc.Core.StatusCode", "NotFound"), Literal(operation.Name + " found nothing"))
                    );
                    body.Add(If(IsNull(Id("__response")), Throw(notFound)));
                }

                var value = operation.ResponseIsNullableValueType ? Member(Id("__response"), "Value") : Id("__response");
                body.Add(Return(operation.ResponseIsMessage ? value : New(GrpcEmit.WireResponse(host, operation), value)));
                break;
        }

        return Method(isAsync ? Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword) : Modifiers(SyntaxKind.PublicKeyword), returnType, operation.Name, parameters, body)
                        .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> through the mediator.");
    }
}
