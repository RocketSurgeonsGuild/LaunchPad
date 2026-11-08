using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Generates a code-first gRPC client (protobuf-net.Grpc) for every <c>*GrpcClient</c> operation host: the same <c>[Service]</c> contract the
///     server generator writes, typed methods named after the operations, one mediator handler per operation and an <c>Add{Host}()</c> that
///     registers it all through protobuf-net.Grpc's client factory.
/// </summary>
[Generator]
public sealed class GrpcClientGenerator : IIncrementalGenerator
{
    private const string Ct = "cancellationToken";

    /// <inheritdoc />
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(static output => HostPipeline.EmitMarker(output, HostKind.GrpcClient));
        var hosts = HostPipeline.Hosts(context, HostKind.GrpcClient);
        var assemblyName = context.CompilationProvider.Select(static (c, _) => c.AssemblyName ?? "Assembly");
        var mediatorGenerators = context.AnalyzerConfigOptionsProvider.Select(static (options, _) => HostPipeline.MediatorGenerators(options.GlobalOptions));
        context.RegisterSourceOutput(
            hosts.Combine(assemblyName).Combine(mediatorGenerators),
            static (output, pair) => Emit(output, pair.Left.Left, pair.Left.Right, pair.Right)
        );
    }

    private static void Emit(SourceProductionContext output, EquatableArray<HostModel> hosts, string assemblyName, EquatableArray<string>? mediatorGenerators)
    {
        foreach (var host in hosts)
        {
            foreach (var diagnostic in host.Diagnostics) output.ReportDiagnostic(diagnostic.ToDiagnostic());
            if (host.Profile is not { } profile) continue;
            ClientEmit.ReportHostChecks(output, host, profile, mediatorGenerators);
            output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".GrpcClient.g.cs", Write(host, profile, output));
        }

        ClientEmit.EmitShared(output, hosts, assemblyName);
    }

    private static string Write(HostModel host, ProfileModel profile, SourceProductionContext output)
    {
        var version = typeof(GrpcClientGenerator).Assembly.GetName().Version;
        var contract = SyntaxFactory.IdentifierName(GrpcEmit.ContractName(host));
        var (handlers, handled) = ClientEmit.Handlers(host, profile, output);
        var members = new List<MemberDeclarationSyntax>
        {
            Field(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword), contract, "__contract"),
            Constructor(host.Name, Parameters(Parameter(contract, "contract")), [ThrowIfNull("contract"), Statement(Assign(Id("__contract"), Id("contract")))])
               .Summary($"Creates the client. Register it with <c>services.Add{host.Name}()</c>."),
        };
        members.AddRange(GrpcEmit.ModelConfiguration(host.Operations));
        foreach (var operation in host.Operations)
        {
            members.Add(Operation(host, operation));
            members.Add(
                PartialHook($"Configure{operation.Name}", false, Ref(Type("global::Grpc.Core.CallOptions"), "options"))
                   .Summary($"Adjusts the call options (headers, deadline, credentials) for <c>{operation.Name}</c>. Implement it in your part of the host.")
            );
        }

        members.AddRange(handlers);
        var client = Class(host.Name, Modifiers(Accessibility(host.Accessibility), SyntaxKind.PartialKeyword), members)
           .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Client.Grpc", version));

        var factory = Name("global::ProtoBuf.Grpc.ClientFactory.ServicesExtensions");
        var add = Method(
                               Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
                               Type("global::Microsoft.Extensions.DependencyInjection.IHttpClientBuilder"),
                               $"Add{host.Name}",
                               Parameters(
                                   This(Type("global::Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services"),
                                   Parameter(SyntaxFactory.NullableType(Generic("global::System.Action", Type("global::Grpc.Net.ClientFactory.GrpcClientFactoryOptions"))), "configure", defaultValue: Null)
                               ),
                               new[] { ThrowIfNull("services") }
                                  .Concat(ClientEmit.Registrations(host, profile, handled))
                                  .Append(Statement(Invoke(Member(Id(host.Name), "ConfigureGrpcModel"))))
                                  .Append(
                                       Return(
                                           Conditional(
                                               IsNull(Id("configure")),
                                               Invoke(GenericMember(factory, "AddCodeFirstGrpcClient", contract), Id("services")),
                                               Invoke(GenericMember(factory, "AddCodeFirstGrpcClient", contract), Id("services"), Id("configure"))
                                           )
                                       )
                                   )
                           )
                          .Documented(
                               $"<summary>Registers <see cref=\"{host.Name}\" />, its gRPC client and one mediator handler per operation.</summary>\n"
                             + "<param name=\"services\">The services.</param>\n"
                             + "<param name=\"configure\">Sets the address and other client factory options.</param>\n"
                             + "<returns>The HttpClient builder behind the gRPC channel, for handlers and resilience.</returns>"
                           );
        var extensions = Class($"{host.Name}ServiceCollectionExtensions", Modifiers(Accessibility(host.Accessibility), SyntaxKind.StaticKeyword), [add])
                        .AddAttributeLists(GeneratedCode("Rocket.Surgery.LaunchPad.Api.Client.Grpc", version))
                        .Summary($"Registers <see cref=\"{host.Name}\" />{( handled.Length > 0 ? " and its mediator handlers" : "" )}.");
        return Render(host.Namespace, [GrpcEmit.Contract(host, host.Operations, version), client, extensions]);
    }

    private static MethodDeclarationSyntax Operation(HostModel host, OperationModel operation)
    {
        var wire = Type(operation.WireType);
        var cancellationToken = operation.Kind == RequestKind.Stream
            ? Parameter(Type(CancellationTokenType), Ct, Attribute("global::System.Runtime.CompilerServices.EnumeratorCancellation"), Default)
            : Parameter(Type(CancellationTokenType), Ct, defaultValue: Default);
        var returnType = operation.Kind switch
        {
            RequestKind.Stream => AsyncEnumerable(Type(operation.ResponseType!)),
            RequestKind.Void => Task(),
            RequestKind.Unary or RequestKind.Infer => Task(Type(operation.ResponseType!)),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
        ExpressionSyntax argument = operation.RequestIsMessage ? Id("request") : New(GrpcEmit.WireRequest(host, operation), Id("request"));
        var call = Invoke(Member(Id("__contract"), operation.Name), argument, Id("__options"));
        ExpressionSyntax Unwrap(ExpressionSyntax value) => operation.ResponseIsMessage ? value : Member(value, "Value");
        var body = new List<StatementSyntax>
        {
            ThrowIfNull("request"),
            Var("__options", New(Type("global::Grpc.Core.CallOptions")).WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(Id(Ct)).WithNameColon(SyntaxFactory.NameColon("cancellationToken")))))),
            Statement(Invoke(Id($"Configure{operation.Name}"), [RefArgument("__options")])),
        };
        switch (operation.Kind)
        {
            case RequestKind.Stream:
                body.Add(AwaitForEach("__item", call, Id(Ct), YieldReturn(Unwrap(Id("__item")))));
                break;
            case RequestKind.Void:
                body.Add(Statement(Await(call)));
                break;
            case RequestKind.Unary:
            case RequestKind.Infer:
            default:
                var result = Return(Unwrap(Parenthesized(Await(call))));
                // The server answers a null response with NotFound.
                body.Add(
                    operation.ResponseNullable
                        ? TryCatch(
                            Block(result),
                            Type("global::Grpc.Core.RpcException"),
                            "__exception",
                            Equal(Member(Id("__exception"), "StatusCode"), Member("global::Grpc.Core.StatusCode", "NotFound")),
                            Block(Return(Default))
                        )
                        : result
                );
                break;
        }

        return Method(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword), returnType, operation.Name, Parameters(Parameter(wire, "request"), cancellationToken), body)
                        .Summary($"Sends <see cref=\"{Cref(operation.RequestType)}\" /> to the <c>{operation.Name}</c> operation.");
    }
}
