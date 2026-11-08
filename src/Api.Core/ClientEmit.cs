using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     What every client transport generates around its typed client: one mediator handler per operation in the profile's shape, their
///     registration, ZeroAlloc's extra Send overloads and Foundatio's module, and the checks on where a client host may live.
/// </summary>
internal static class ClientEmit
{
    private const string Tasks = "global::System.Threading.Tasks";
    private const string Ct = "cancellationToken";

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

    /// <summary>The nested mediator handler classes, for profiles that implement a handler interface; returns the handled operations.</summary>
    public static OperationModel[] WriteHandlers(SourceWriter w, HostModel host, ProfileModel profile, SourceProductionContext output)
    {
        var handled = HandledOperations(host, profile).ToArray();
        if (profile.ClientEmitter is ClientEmitter.HandlerInterface or ClientEmitter.HandlerInterfaceDiscovered)
        {
            foreach (var operation in handled)
            {
                if (operation.HandlerInterface is null || operation.HandlerMethodReturnType is null)
                {
                    output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.NoHandlerInterface, operation.Location, profile.Name, operation.RequestDisplayName, $"{host.Name}.{operation.Name}").ToDiagnostic());
                    continue;
                }

                w.Line();
                WriteHandler(w, host, profile, operation);
            }
        }
        else if (profile.ClientEmitter == ClientEmitter.None)
        {
            foreach (var operation in host.Operations)
            {
                output.ReportDiagnostic(DiagnosticInfo.Create(ApiDiagnostics.NoHandlerInterface, operation.Location, profile.Name, operation.RequestDisplayName, $"{host.Name}.{operation.Name}").ToDiagnostic());
            }
        }

        return handled;
    }

    /// <summary>Inside <c>Add{Host}()</c>: registers the client class and the handlers, and checks Foundatio's registry.</summary>
    public static void WriteRegistrations(SourceWriter w, HostModel host, ProfileModel profile, OperationModel[] handled)
    {
        w.Line($"global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAddTransient<{host.Name}>(services);");
        if (profile.ClientEmitter == ClientEmitter.HandlerInterface)
        {
            foreach (var operation in handled.Where(o => o.HandlerInterface is { }))
            {
                w.Line(
                    $"global::Microsoft.Extensions.DependencyInjection.Extensions.ServiceCollectionDescriptorExtensions.TryAdd(services, global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor.Describe(typeof({operation.HandlerInterface}), typeof({host.Name}.{operation.Name}Handler), global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.{profile.HandlerLifetime}));"
                );
            }
        }

        if (profile.ClientEmitter == ClientEmitter.Foundatio && handled.Length > 0) WriteFoundatioCheck(w, host, handled[0]);
    }

    private static void WriteHandler(SourceWriter w, HostModel host, ProfileModel profile, OperationModel operation)
    {
        var returnType = operation.HandlerMethodReturnType!;
        var handleMethod = operation.HandlerMethodName ?? "Handle";
        w.Line($"/// <summary>Handles <see cref=\"{Cref(operation.RequestType)}\" /> by sending it through <see cref=\"{host.Name}.{operation.Name}\" />.</summary>");
        w.Open($"public sealed class {operation.Name}Handler : {operation.HandlerInterface}");
        w.Line($"private readonly {host.Name} __client;");
        w.Line();
        w.Line($"public {operation.Name}Handler({host.Name} client) => __client = client;");
        w.Line();
        var call = $"__client.{operation.Name}(request, {Ct})";
        var isValueTask = returnType.StartsWith(Tasks + ".ValueTask", StringComparison.Ordinal);
        var returnsValue = returnType.Contains("<");
        string body;
        if (operation.Kind == RequestKind.Stream)
        {
            // Immediate.Handlers' stream handlers return ValueTask<IAsyncEnumerable<T>>.
            body = isValueTask ? $" => new {returnType}({call});" : $" => {call};";
        }
        else if (operation.Kind == RequestKind.Void && returnsValue)
        {
            // A Unit-returning void handler: the client method returns a Task, the handler returns Unit.
            body = null!;
            w.Open($"public async {returnType} {handleMethod}({operation.RequestType} request, global::System.Threading.CancellationToken {Ct})");
            w.Line($"await {call}.ConfigureAwait(false);");
            w.Line("return default;");
            w.Close();
        }
        else
        {
            body = isValueTask ? $" => new {returnType}({call});" : $" => {call};";
        }

        if (body is { }) w.Line($"public {returnType} {handleMethod}({operation.RequestType} request, global::System.Threading.CancellationToken {Ct}){body}");
        w.Close();
    }

    // AddMediator() builds and freezes Foundatio's handler registry from the [FoundatioModule] assemblies it scans. If this host's assembly wasn't
    // among them (AddMediator ran before the assembly was loaded, or was given an assembly list without it), the handlers are missing for good:
    // fail here with what to do instead of at the first Send.
    private static void WriteFoundatioCheck(SourceWriter w, HostModel host, OperationModel first)
    {
        var message = $"Foundatio's handler registry was built without {host.Name}'s handlers. Call services.Add{host.Name}() before services.AddMediator(), "
                    + $"and if AddMediator is given assemblies, include the one that declares {host.Name}.";
        w.Open("foreach (var __descriptor in services)");
        // ImplementationInstance throws on a keyed descriptor.
        w.Line("if (__descriptor.IsKeyedService || __descriptor.ServiceType != typeof(global::Foundatio.Mediator.HandlerRegistry) || __descriptor.ImplementationInstance is not global::Foundatio.Mediator.HandlerRegistry __registry) continue;");
        w.Line($"if (!global::System.Linq.Enumerable.Any(__registry.Registrations, r => r.HandlerClassName == {SourceWriter.Literal(FoundatioHandlerName(host, first))})) throw new global::System.InvalidOperationException({SourceWriter.Literal(message)});");
        w.Line("break;");
        w.Close();
    }

    private static string FoundatioHandlerName(HostModel host, OperationModel operation) => $"{host.Name}_{operation.Name}_Handler";

    private static string FoundatioModuleClass(string assemblyName) =>
        "global::Rocket.Surgery.LaunchPad.Api.Generated." + new string([.. assemblyName.Select(c => char.IsLetterOrDigit(c) ? c : '_')]) + "_MediatorHandlers";

    // ZeroAlloc generates `internal partial interface IMediator` and `internal partial class MediatorService` per project with one Send overload
    // per handler it sees in syntax; it can't see ours, so the missing overloads are added to its partials and route to the client.
    private static string WriteZeroAlloc(HostModel host)
    {
        var w = new SourceWriter();
        w.Line("// <auto-generated/>").Line("#nullable enable").Line("#pragma warning disable CS1591").Line();
        w.Open("namespace ZeroAlloc.Mediator");
        var operations = host.Operations.Where(o => o.Handlers && o.InputType is null).ToArray();
        w.Open("internal partial interface IMediator");
        foreach (var operation in operations) w.Line(ZeroAllocSignature(operation, true) + ";");
        w.Close();
        w.Open("internal partial class MediatorService");
        foreach (var operation in operations)
        {
            var client = $"global::Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetRequiredService<{host.FullyQualifiedName}>(_services)";
            var call = $"{client}.{operation.Name}(request, ct)";
            switch (operation.Kind)
            {
                case RequestKind.Stream:
                    w.Line($"public {ZeroAllocSignature(operation, false)} => {call};");
                    break;
                case RequestKind.Void:
                    w.Open($"public async {ZeroAllocSignature(operation, false)}");
                    w.Line($"await {call}.ConfigureAwait(false);");
                    w.Line("return default;");
                    w.Close();
                    break;
                case RequestKind.Unary:
                case RequestKind.Infer:
                default:
                    w.Line($"public {ZeroAllocSignature(operation, false)} => new({call});");
                    break;
            }
        }

        w.Close();
        w.Close();
        return w.ToString();
    }

    private static string ZeroAllocSignature(OperationModel operation, bool withDefault)
    {
        var ct = "global::System.Threading.CancellationToken ct" + ( withDefault ? " = default" : "" );
        return operation.Kind == RequestKind.Stream
            ? $"global::System.Collections.Generic.IAsyncEnumerable<{operation.ResponseType}> CreateStream({operation.RequestType} request, {ct})"
            : $"{Tasks}.ValueTask<{( operation.Kind == RequestKind.Void ? "global::ZeroAlloc.Mediator.Unit" : operation.ResponseType )}> Send({operation.RequestType} request, {ct})";
    }

    // Foundatio's AddMediator() scans loaded assemblies for [FoundatioModule] and calls the first static *_MediatorHandlers.AddHandlers it finds.
    // The client library doesn't run Foundatio's generator, so this is the only module class in it.
    private static string WriteFoundatioModule(List<(HostModel Host, OperationModel Operation)> operations, string assemblyName)
    {
        var w = new SourceWriter();
        w.Line("// <auto-generated/>").Line("#nullable enable").Line("#pragma warning disable").Line();
        w.Line("[assembly: global::Foundatio.Mediator.FoundatioModule]");
        w.Line();
        w.Open("namespace Rocket.Surgery.LaunchPad.Api.Generated");
        w.Open($"public static class {FoundatioModuleClass(assemblyName).Split('.').Last()}");
        w.Open("public static void AddHandlers(global::Microsoft.Extensions.DependencyInjection.IServiceCollection services, global::Foundatio.Mediator.HandlerRegistry registry)");
        foreach (var (host, operation) in operations)
        {
            var handlerName = FoundatioHandlerName(host, operation);
            w.Line("registry.AddHandler(new global::Foundatio.Mediator.HandlerRegistration(");
            w.Line($"    global::Foundatio.Mediator.MessageTypeKey.Get(typeof({operation.RequestType})),");
            w.Line($"    {SourceWriter.Literal(handlerName)},");
            w.Line("    static async (mediator, message, callContext, cancellationToken, responseType, skipAuthorization) =>");
            w.Line("    {");
            w.Line($"        var client = ({host.FullyQualifiedName})((global::System.IServiceProvider)mediator).GetService(typeof({host.FullyQualifiedName}))!;");
            w.Line(
                operation.Kind == RequestKind.Void
                    ? $"        await client.{operation.Name}(({operation.RequestType})message, cancellationToken).ConfigureAwait(false); return null;"
                    : operation.Kind == RequestKind.Stream
                        ? $"        return client.{operation.Name}(({operation.RequestType})message, cancellationToken);"
                        : $"        return await client.{operation.Name}(({operation.RequestType})message, cancellationToken).ConfigureAwait(false);"
            );
            w.Line("    },");
            w.Line("    null,");
            w.Line("    true,");
            w.Line($"    sourceHandlerName: {SourceWriter.Literal(host.Name)},");
            w.Line($"    methodName: {SourceWriter.Literal(operation.Name)}));");
        }

        w.Close();
        w.Close();
        w.Close();
        return w.ToString();
    }

    public static string Cref(string type) => type.Replace("global::", "").Replace('<', '{').Replace('>', '}');
}
