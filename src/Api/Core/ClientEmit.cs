using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Rocket.Surgery.LaunchPad.Api.Generators.ApiSyntax;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     What every client transport generates around its typed client: one mediator handler per operation in the profile's shape, their
///     registration, ZeroAlloc's extra Send overloads and Foundatio's module, and the checks on where a client host may live.
/// </summary>
internal static class ClientEmit
{
    private const string Ct = "cancellationToken";
    internal static readonly string[] sourceArray = ["mediator", "message", "callContext", "cancellationToken", "responseType", "skipAuthorization"];

    public static IEnumerable<OperationModel> HandledOperations(HostModel host, ProfileModel profile) =>
        profile.ClientEmitter == ClientEmitter.None ? [] : host.Operations.Where(o => o.Handlers && o.InputType is null);

    /// <summary>LPAD1012 and LPAD1013 for one client host.</summary>
    public static void ReportHostChecks(SourceProductionContext output, HostModel host, ProfileModel profile, EquatableArray<string>? mediatorGenerators)
    {
        if (HostPipeline.PlacementProblem(profile, mediatorGenerators) is { } problem)
        {
            output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.ClientHostInWrongProject, host.Location, host.Name, profile.Name, problem).ToDiagnostic());
        }

        foreach (var operation in HandledOperations(host, profile).Where(o => o.VisibleHandler is { }))
        {
            output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.HandWrittenHandlerVisible, operation.Location, operation.RequestDisplayName, operation.VisibleHandler!, host.Name).ToDiagnostic());
        }
    }

    /// <summary>
    ///     The extra files a host needs (ZeroAlloc's overloads), the cross-host checks (LPAD1018) and Foundatio's one module per compilation.
    /// </summary>
    public static void EmitShared(SourceProductionContext output, EquatableArray<HostModel> hosts, string assemblyName)
    {
        var foundatio = new List<(HostModel Host, OperationModel Operation)>();
        foreach (var host in hosts)
        {
            if (host.Profile is not { } profile) continue;
            if (profile.ClientEmitter == ClientEmitter.Foundatio) foundatio.AddRange(HandledOperations(host, profile).Select(o => (host, o)));
            if (profile.ClientEmitter == ClientEmitter.ZeroAlloc) output.AddSource(host.FullyQualifiedName.Replace("global::", "") + ".ZeroAlloc.g.cs", WriteZeroAlloc(host));
        }

        // One request type, one handler: two client hosts handling the same operation is ambiguous for every mediator.
        foreach (var duplicate in hosts.Where(h => h.Profile is { })
                                       .SelectMany(h => HandledOperations(h, h.Profile!).Select(o => (Host: h, Operation: o)))
                                       .GroupBy(x => x.Operation.RequestType)
                                       .Where(g => g.Count() > 1))
        {
            var items = duplicate.ToArray();
            output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.DuplicateClientHandler, items[1].Operation.Location, items[0].Operation.RequestDisplayName, items[0].Host.Name, items[1].Host.Name).ToDiagnostic());
        }

        if (foundatio.Count > 0) output.AddSource("Rocket.Surgery.LaunchPad.Api.Generated.FoundatioModule.g.cs", WriteFoundatioModule(foundatio, assemblyName));
    }

    /// <summary>The nested mediator handler classes, for profiles that implement a handler interface, and the operations that get a handler.</summary>
    public static (IReadOnlyList<MemberDeclarationSyntax> Handlers, OperationModel[] Handled) Handlers(HostModel host, ProfileModel profile, SourceProductionContext output)
    {
        var handled = HandledOperations(host, profile).ToArray();
        var handlers = new List<MemberDeclarationSyntax>();
        if (profile.ClientEmitter is ClientEmitter.HandlerInterface or ClientEmitter.HandlerInterfaceDiscovered)
        {
            foreach (var operation in handled)
            {
                if (operation.HandlerInterface is null || operation.HandlerMethodReturnType is null)
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.NoHandlerInterface, operation.Location, profile.Name, operation.RequestDisplayName, $"{host.Name}.{operation.Name}").ToDiagnostic());
                    continue;
                }

                handlers.Add(Handler(host, operation));
            }
        }
        else if (profile.ClientEmitter == ClientEmitter.None)
        {
            foreach (var operation in host.Operations)
            {
                output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.NoHandlerInterface, operation.Location, profile.Name, operation.RequestDisplayName, $"{host.Name}.{operation.Name}").ToDiagnostic());
            }
        }

        return (handlers, handled);
    }

    /// <summary>Inside <c>Add{Host}()</c>: registers the client class and the handlers, and checks Foundatio's registry.</summary>
    public static IEnumerable<StatementSyntax> Registrations(HostModel host, ProfileModel profile, OperationModel[] handled)
    {
        yield return Statement(Invoke(GenericMember(Name(ServiceCollectionDescriptors), "TryAddTransient", SyntaxFactory.IdentifierName(host.Name)), Id("services")));
        if (profile.ClientEmitter == ClientEmitter.HandlerInterface)
        {
            foreach (var operation in handled.Where(o => o.HandlerInterface is { }))
            {
                var descriptor = Invoke(
                    Member("global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor", "Describe"),
                    TypeOf(Type(operation.HandlerInterface!)),
                    TypeOf(SyntaxFactory.QualifiedName(SyntaxFactory.IdentifierName(host.Name), SyntaxFactory.IdentifierName(operation.Name + "Handler"))),
                    Member("global::Microsoft.Extensions.DependencyInjection.ServiceLifetime", profile.HandlerLifetime)
                );
                yield return Statement(Invoke(Member(ServiceCollectionDescriptors, "TryAdd"), Id("services"), descriptor));
            }
        }

        if (profile.ClientEmitter == ClientEmitter.Foundatio && handled.Length > 0) yield return FoundatioCheck(host, handled[0]);
    }

    private static ClassDeclarationSyntax Handler(HostModel host, OperationModel operation)
    {
        var returnType = operation.HandlerMethodReturnType!;
        var call = Invoke(Member(Id("__client"), operation.Name), Id("request"), Id(Ct));
        var isValueTask = returnType.StartsWith(Tasks + ".ValueTask", StringComparison.Ordinal);
        var parameters = Parameters(Parameter(Type(operation.RequestType), "request"), Parameter(Type(CancellationTokenType), Ct));
        var name = operation.HandlerMethodName ?? "Handle";
        // A Unit-returning void handler awaits the client and returns Unit; a ValueTask handler wraps the client's Task (or stream: Immediate.Handlers).
        var handle = operation.Kind == RequestKind.Void && returnType.Contains("<")
            ? Method(Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.AsyncKeyword), Type(returnType), name, parameters, [Statement(Await(call)), Return(Default)])
            : Method(Modifiers(SyntaxKind.PublicKeyword), Type(returnType), name, parameters, isValueTask ? New(Type(returnType), call) : call);
        return Class(
                   operation.Name + "Handler",
                   Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.SealedKeyword),
                   [
                       Field(Modifiers(SyntaxKind.PrivateKeyword, SyntaxKind.ReadOnlyKeyword), SyntaxFactory.IdentifierName(host.Name), "__client"),
                       Constructor(operation.Name + "Handler", Parameters(Parameter(SyntaxFactory.IdentifierName(host.Name), "client")), [Statement(Assign(Id("__client"), Id("client")))]),
                       handle,
                   ],
                   Type(operation.HandlerInterface!)
               )
              .Summary($"Handles <see cref=\"{Cref(operation.RequestType)}\" /> by sending it through <see cref=\"{host.Name}.{operation.Name}\" />.");
    }

    // AddMediator() builds and freezes Foundatio's handler registry from the [FoundatioModule] assemblies it scans. If this host's assembly wasn't
    // among them (AddMediator ran before the assembly was loaded, or was given an assembly list without it), the handlers are missing for good:
    // fail here with what to do instead of at the first Send.
    private static StatementSyntax FoundatioCheck(HostModel host, OperationModel first)
    {
        var message = $"Foundatio's handler registry was built without {host.Name}'s handlers. Call services.Add{host.Name}() before services.AddMediator(), "
                    + $"and if AddMediator is given assemblies, include the one that declares {host.Name}.";
        var registry = Type("global::Foundatio.Mediator.HandlerRegistry");
        var descriptor = Id("__descriptor");
        // ImplementationInstance throws on a keyed descriptor.
        var notTheRegistry = Or(
            Or(Member(descriptor, "IsKeyedService"), NotEqual(Member(descriptor, "ServiceType"), TypeOf(registry))),
            SyntaxFactory.IsPatternExpression(
                Member(descriptor, "ImplementationInstance"),
                SyntaxFactory.UnaryPattern(SyntaxFactory.DeclarationPattern(registry, SyntaxFactory.SingleVariableDesignation(SyntaxFactory.Identifier("__registry"))))
            )
        );
        var hasHandler = Invoke(
            Member("global::System.Linq.Enumerable", "Any"),
            Member(Id("__registry"), "Registrations"),
            Lambda("r", Equal(Member(Id("r"), "HandlerClassName"), Literal(FoundatioHandlerName(host, first))))
        );
        return ForEach(
            "__descriptor",
            Id("services"),
            Block(
                If(notTheRegistry, Continue),
                If(Not(hasHandler), Throw(New(Type("global::System.InvalidOperationException"), Literal(message)))),
                Break
            )
        );
    }

    private static string FoundatioHandlerName(HostModel host, OperationModel operation) => $"{host.Name}_{operation.Name}_Handler";

    private static string FoundatioModuleClass(string assemblyName) => new string([.. assemblyName.Select(c => char.IsLetterOrDigit(c) ? c : '_')]) + "_MediatorHandlers";

    // ZeroAlloc generates `internal partial interface IMediator` and `internal partial class MediatorService` per project with one Send overload
    // per handler it sees in syntax; it can't see ours, so the missing overloads are added to its partials and route to the client.
    private static string WriteZeroAlloc(HostModel host)
    {
        var operations = host.Operations.Where(o => o.Handlers && o.InputType is null).ToArray();
        var contract = Interface("IMediator", Modifiers(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword), operations.Select(o => (MemberDeclarationSyntax)ZeroAllocSignature(o, withDefault: true)));
        var implementation = Class(
            "MediatorService",
            Modifiers(SyntaxKind.InternalKeyword, SyntaxKind.PartialKeyword),
            operations.Select(
                operation =>
                {
                    var call = Invoke(Member(GetRequiredService(Type(host.FullyQualifiedName), Id("_services")), operation.Name), Id("request"), Id("ct"));
                    var signature = ZeroAllocSignature(operation, withDefault: false).WithModifiers(Modifiers(SyntaxKind.PublicKeyword)).WithSemicolonToken(default);
                    return (MemberDeclarationSyntax)( operation.Kind switch
                    {
                        RequestKind.Stream => signature.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(call)).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        RequestKind.Void => signature.AddModifiers(SyntaxFactory.Token(SyntaxKind.AsyncKeyword)).WithBody(Block(Statement(Await(call)), Return(Default))),
                        RequestKind.Unary or RequestKind.Infer => signature.WithExpressionBody(SyntaxFactory.ArrowExpressionClause(SyntaxFactory.ImplicitObjectCreationExpression().AddArgumentListArguments(SyntaxFactory.Argument(call))))
                                                                    .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        _ => throw new ArgumentOutOfRangeException(nameof(operation)),
                    } );
                }
            )
        );
        return Render("ZeroAlloc.Mediator", [contract, implementation]);
    }

    private static MethodDeclarationSyntax ZeroAllocSignature(OperationModel operation, bool withDefault)
    {
        var parameters = Parameters(
            Parameter(Type(operation.RequestType), "request"),
            Parameter(Type(CancellationTokenType), "ct", defaultValue: withDefault ? Default : null)
        );
        return operation.Kind == RequestKind.Stream
            ? Signature(AsyncEnumerable(Type(operation.ResponseType!)), "CreateStream", parameters)
            : Signature(ValueTask(Type(operation.Kind == RequestKind.Void ? "global::ZeroAlloc.Mediator.Unit" : operation.ResponseType!)), "Send", parameters);
    }

    // Foundatio's AddMediator() scans loaded assemblies for [FoundatioModule] and calls the first static *_MediatorHandlers.AddHandlers it finds.
    // The client library doesn't run Foundatio's generator, so this is the only module class in it.
    private static string WriteFoundatioModule(List<(HostModel Host, OperationModel Operation)> operations, string assemblyName)
    {
        var registrations = operations.Select(
            pair =>
            {
                var (host, operation) = pair;
                var message = Cast(Type(operation.RequestType), Id("message"));
                var client = Invoke(Member(Id("client"), operation.Name), message, Id("cancellationToken"));
                var handle = SyntaxFactory.ParenthesizedLambdaExpression()
                                          .WithModifiers(Modifiers(SyntaxKind.StaticKeyword, SyntaxKind.AsyncKeyword))
                                          .WithParameterList(
                                               SyntaxFactory.ParameterList(
                                                   SyntaxFactory.SeparatedList(
sourceArray.Select(p => SyntaxFactory.Parameter(SyntaxFactory.Identifier(p)))
                                                   )
                                               )
                                           )
                                          .WithBlock(
                                               Block(
                                                   new[]
                                                       {
                                                           Var(
                                                               "client",
                                                               NullForgiving(
                                                                   Cast(
                                                                       Type(host.FullyQualifiedName),
                                                                       Invoke(Member(Parenthesized(Cast(Type("global::System.IServiceProvider"), Id("mediator"))), "GetService"), TypeOf(Type(host.FullyQualifiedName)))
                                                                   )
                                                               )
                                                           ),
                                                       }.Concat(
                                                       operation.Kind switch
                                                       {
                                                           RequestKind.Void => [Statement(Await(client)), Return(Null)],
                                                           RequestKind.Stream => [Return(client)],
                                                           RequestKind.Unary or RequestKind.Infer => [Return(Await(client))],
                                                           _ => throw new ArgumentOutOfRangeException(nameof(operations)),
                                                       }
                                                   )
                                               )
                                           );
                var registration = New(
                    Type("global::Foundatio.Mediator.HandlerRegistration"),
                    [
                        Invoke(Member("global::Foundatio.Mediator.MessageTypeKey", "Get"), TypeOf(Type(operation.RequestType))),
                        Literal(FoundatioHandlerName(host, operation)),
                        handle,
                        Null,
                        True,
                    ]
                );
                registration = registration.AddArgumentListArguments(
                    SyntaxFactory.Argument(Literal(host.Name)).WithNameColon(SyntaxFactory.NameColon("sourceHandlerName")),
                    SyntaxFactory.Argument(Literal(operation.Name)).WithNameColon(SyntaxFactory.NameColon("methodName"))
                );
                return Statement(Invoke(Member(Id("registry"), "AddHandler"), registration));
            }
        );
        var addHandlers = Method(
            Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword),
            VoidType,
            "AddHandlers",
            Parameters(
                Parameter(Type("global::Microsoft.Extensions.DependencyInjection.IServiceCollection"), "services"),
                Parameter(Type("global::Foundatio.Mediator.HandlerRegistry"), "registry")
            ),
            registrations
        );
        var module = Class(FoundatioModuleClass(assemblyName), Modifiers(SyntaxKind.PublicKeyword, SyntaxKind.StaticKeyword), [addHandlers]);
        return Render("Rocket.Surgery.LaunchPad.Api.Generated", [module], Attribute("global::Foundatio.Mediator.FoundatioModule"));
    }
}
