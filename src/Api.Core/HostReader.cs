using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Rocket.Surgery.LaunchPad.Api.Generators;

/// <summary>
///     Reads an operation host and every operation it lists into equatable models. All semantic work happens here, against the current
///     compilation, so the emit step only ever sees records.
/// </summary>
internal static class HostReader
{
    public static HostModel Read(INamedTypeSymbol host, Compilation compilation, (ProfileModel? Profile, string? Problem) profile, CancellationToken cancellationToken)
    {
        var diagnostics = new List<DiagnosticInfo>();
        var (kind, _) = HostInference.Infer(host, compilation);
        var settings = host.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.OperationHostAttribute));
        var hostArgs = Named(settings);
        var hostNoun = Naming.HostNoun(host.Name);
        var group = String(hostArgs, "Group") ?? Naming.KebabCase(hostNoun);
        var hostHandlers = Bool(hostArgs, "Handlers") ?? true;

        if (profile.Profile is null && kind != HostKind.Unknown)
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.NoMediatorProfile, LocationInfo.From(host), host.Name, profile.Problem ?? "unknown"));
        }

        var entries = host.GetAttributes()
                          .Where(a => a.AttributeClass is { IsGenericType: true } c && ( c.Is(KnownNames.OperationAttribute) || c.Is(KnownNames.CreateOperationAttribute) ))
                          .ToArray();

        var operations = new List<OperationModel>();
        if (profile.Profile is { } mediator)
        {
            foreach (var entry in entries)
            {
                cancellationToken.ThrowIfCancellationRequested();
                if (ReadOperation(entry, host, hostNoun, kind, mediator, hostHandlers, compilation, diagnostics) is { } operation) operations.Add(operation);
            }

            ResolveCreatedLocations(operations, diagnostics);
            foreach (var duplicate in operations.GroupBy(o => (o.Verb, Route: NormalizeRoute(o.Route))).Where(g => g.Count() > 1))
            {
                var first = duplicate.First();
                foreach (var other in duplicate.Skip(1))
                {
                    diagnostics.Add(
                        DiagnosticInfo.Create(ApiDiagnostics.DuplicateRoute, other.Location, first.Verb.ToString().ToUpperInvariant(), RouteText.Join(group, first.Route), first.Name, other.Name)
                    );
                }
            }
        }

        return new HostModel(
            host.ContainingNamespace is { IsGlobalNamespace: false } ns ? ns.ToDisplayString() : null,
            host.Name,
            host.FullyQualified(),
            host.DeclaredAccessibility == Accessibility.Public ? "public" : "internal",
            kind,
            group,
            String(hostArgs, "RoutePrefix") ?? "api",
            String(hostArgs, "ClientName") ?? host.Name,
            String(hostArgs, "Version"),
            // JsonOptionsPreset: Web = 0, Default = 1.
            Int(hostArgs, "JsonOptions") is not 1,
            host.BaseType is { SpecialType: not SpecialType.System_Object },
            host.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is("Microsoft.AspNetCore.Mvc.RouteAttribute"))?.ConstructorArguments.FirstOrDefault().Value as string,
            host.GetAttributes().Any(a => a.AttributeClass.Is("Microsoft.AspNetCore.Mvc.ApiControllerAttribute")),
            profile.Profile,
            operations.ToEquatableArray(),
            diagnostics.ToEquatableArray(),
            LocationInfo.From(host)
        );
    }

    private static OperationModel? ReadOperation(
        AttributeData entry,
        INamedTypeSymbol host,
        string hostNoun,
        HostKind hostKind,
        ProfileModel profile,
        bool hostHandlers,
        Compilation compilation,
        List<DiagnosticInfo> diagnostics
    )
    {
        var location = LocationInfo.From(entry);
        if (entry.AttributeClass!.TypeArguments[0] is not INamedTypeSymbol request) return null;
        var args = Named(entry);
        var apiOperation = Named(request.GetAttributes().FirstOrDefault(a => a.AttributeClass.Is(KnownNames.ApiOperationAttribute)));
        var name = Naming.OperationName(request.Name, request.ContainingType?.Name);

        // Response and request kind, from the profile.
        var explicitResponse = Type(apiOperation, "Response");
        var (requestKind, response, kindModel) = MatchKind(request, host, profile, explicitResponse, compilation);
        if (requestKind == RequestKind.Infer)
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.ResponseNotFound, location, request.ToDisplayString(), profile.Name));
            return null;
        }

        var isUnit = response is { } && profile.UnitType is { } unit && response.Is(unit);
        if (isUnit) requestKind = RequestKind.Void;

        // An input type that the transport carries instead of the request, converted with Create().
        var input = Type(args, "Input") as INamedTypeSymbol;
        if (input is { } && !input.GetMembers("Create").OfType<IMethodSymbol>().Any(m => m is { IsStatic: false, Parameters.Length: 0, DeclaredAccessibility: Accessibility.Public } && SymbolEqualityComparer.Default.Equals(m.ReturnType, request)))
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.InputWithoutCreate, location, input.ToDisplayString(), request.ToDisplayString()));
            return null;
        }

        var wireType = input ?? request;
        var members = ReadMembers(wireType);
        var (inferredVerb, creates, remainder) = Naming.InferVerb(name);
        var verb = Naming.TryParseVerb(String(args, "Verb") ?? String(apiOperation, "Verb"), out var explicitVerb) ? explicitVerb : inferredVerb;
        var hasId = members.Any(m => string.Equals(m.Name, "Id", StringComparison.OrdinalIgnoreCase));
        var route = String(args, "Route") ?? String(apiOperation, "Path") ?? Naming.DefaultPath(remainder, hostNoun, verb, hasId);
        var tokens = RouteText.Tokens(route).ToArray();

        var boundMembers = Bind(wireType, members, tokens, verb, route, name, location, diagnostics, out var bindingFailed);
        var canConstruct = boundMembers.All(m => m.ConstructorIndex >= 0 || m.CanAssign || m.CanInit || m.Source == BindingSource.Ignored);
        var hasBody = verb is HttpVerb.Post or HttpVerb.Put or HttpVerb.Patch;
        if (!hasBody && !canConstruct)
        {
            var blocker = boundMembers.First(m => m.ConstructorIndex < 0 && !m.CanAssign && !m.CanInit && m.Source != BindingSource.Ignored);
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RequestNotConstructible, location, wireType.ToDisplayString(), name, blocker.Name));
            bindingFailed = true;
        }

        var supportsWith = wireType.IsRecord || wireType.IsValueType;
        if (hasBody && !supportsWith)
        {
            foreach (var routeMember in boundMembers.Where(m => m.Source is BindingSource.Route or BindingSource.Header or BindingSource.Service && !m.CanAssign))
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RequestNotConstructible, location, wireType.ToDisplayString(), name, routeMember.Name));
                bindingFailed = true;
            }
        }

        if (bindingFailed) return null;

        var successStatus = Int(apiOperation, "SuccessStatusCode") is { } status and > 0
            ? status
            : requestKind == RequestKind.Void
                ? 204
                : creates && verb == HttpVerb.Post ? 201 : 200;

        // The handler interface a client handler implements, closed over this request and response.
        string? handlerInterface = null, handlerReturnType = null;
        INamedTypeSymbol? closedHandler = null;
        if (kindModel?.HandlerInterface is { } handlerName && compilation.GetTypeByMetadataName(handlerName) is { } openHandler)
        {
            // A markerless stream (Immediate.Handlers) is a handler whose response is the IAsyncEnumerable<T> itself.
            var handlerResponse = kindModel.RequestInterface is null && requestKind == RequestKind.Stream && response is { }
                ? compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerable`1")?.Construct(response)
                : response;
            var closed = openHandler.Arity == 1 ? openHandler.Construct(request) : handlerResponse is { } ? openHandler.Construct(request, handlerResponse) : null;
            var method = closed?.GetMembers(kindModel.HandleMethod).OfType<IMethodSymbol>().FirstOrDefault();
            if (closed is { } && method is { })
            {
                closedHandler = closed;
                handlerInterface = closed.FullyQualifiedNullable();
                handlerReturnType = method.ReturnType.FullyQualifiedNullable();
            }
        }

        string? createdTarget = null;
        if (entry.AttributeClass.Is(KnownNames.CreateOperationAttribute) && entry.AttributeClass.TypeArguments[1] is INamedTypeSymbol locationRequest)
        {
            createdTarget = Naming.OperationName(locationRequest.Name, locationRequest.ContainingType?.Name);
            successStatus = 201;
        }

        var responseIsNullableValueType = response is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
        var responseNullable = response is { } && ( response.NullableAnnotation == NullableAnnotation.Annotated || responseIsNullableValueType );
        var responseNotNull = response is INamedTypeSymbol nullableValue && responseIsNullableValueType
            ? nullableValue.TypeArguments[0]
            : response?.WithNullableAnnotation(NullableAnnotation.NotAnnotated);
        var responseMembers = ( responseNotNull as INamedTypeSymbol )?.GetMembers()
                                                                   .OfType<IPropertySymbol>()
                                                                   .Where(p => p is { IsStatic: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } })
                                                                   .Select(p => p.Name)
                                                                   .ToEquatableArray() ?? EquatableArray<string>.Empty;
        return new OperationModel(
            name,
            request.FullyQualified(),
            request.ToDisplayString(),
            supportsWith,
            input?.FullyQualified(),
            requestKind,
            kindModel?.RequestInterface is null,
            response?.FullyQualifiedNullable(),
            responseNotNull?.FullyQualified(),
            responseNullable,
            responseIsNullableValueType,
            isUnit,
            responseMembers,
            verb,
            successStatus,
            route,
            tokens.ToEquatableArray(),
            boundMembers.ToEquatableArray(),
            canConstruct,
            String(args, "Summary") ?? String(apiOperation, "Summary"),
            String(args, "Description") ?? String(apiOperation, "Description"),
            String(args, "Name") ?? String(apiOperation, "Name") ?? name,
            ( Strings(args, "Tags") ?? Strings(apiOperation, "Tags") ?? [] ).ToEquatableArray(),
            ( Strings(args, "Policy") ?? [] ).ToEquatableArray(),
            ( Strings(args, "Roles") ?? [] ).ToEquatableArray(),
            ( Strings(args, "AuthenticationSchemes") ?? [] ).ToEquatableArray(),
            Bool(args, "AllowAnonymous") ?? false,
            String(args, "Version"),
            Bool(args, "Handlers") ?? hostHandlers,
            handlerInterface,
            handlerReturnType,
            kindModel?.HandleMethod,
            createdTarget,
            null,
            EquatableArray<string>.Empty,
            hostKind.IsClient() ? FindHandWrittenHandler(request, closedHandler, compilation)?.ToDisplayString() : null,
            location
        );
    }

    // A CreateOperation's Location points at another operation of the same host; its route values come from response members.
    private static void ResolveCreatedLocations(List<OperationModel> operations, List<DiagnosticInfo> diagnostics)
    {
        for (var i = 0; i < operations.Count; i++)
        {
            var operation = operations[i];
            if (operation.CreatedLocationOperation is not { } targetName) continue;
            var target = operations.FirstOrDefault(o => o.Name == targetName);
            if (target is null)
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.CreatedLocationInvalid, operation.Location, operation.Name, targetName, "it is not declared on the same host"));
                continue;
            }

            var missing = target.RouteTokens.FirstOrDefault(t => operation.ResponseMembers.All(m => !string.Equals(m, t, StringComparison.OrdinalIgnoreCase)));
            if (missing is { })
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.CreatedLocationInvalid, operation.Location, operation.Name, targetName, $"its response has no member for the route token '{missing}'"));
                continue;
            }

            var tokenMembers = target.RouteTokens.Select(t => operation.ResponseMembers.First(m => string.Equals(m, t, StringComparison.OrdinalIgnoreCase)));
            operations[i] = operation with { CreatedLocationRoute = target.Route, CreatedLocationTokens = tokenMembers.ToEquatableArray() };
        }
    }

    // A handler someone wrote for the request, declared next to it: in its containing type or its namespace, in any assembly (LPAD1013).
    // Only those places are searched, so the check stays cheap; a handler elsewhere still makes a second handler, but isn't reported.
    private static INamedTypeSymbol? FindHandWrittenHandler(INamedTypeSymbol request, INamedTypeSymbol? closedHandler, Compilation compilation)
    {
        var candidates = new List<INamedTypeSymbol>();
        if (request.ContainingType is { } containing) candidates.AddRange(containing.GetTypeMembers());
        if (compilation.GetCompilationNamespace(request.ContainingNamespace) is { } ns)
        {
            foreach (var type in ns.GetTypeMembers())
            {
                candidates.Add(type);
                candidates.AddRange(type.GetTypeMembers());
            }
        }

        return candidates.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).FirstOrDefault(c => IsHandlerOf(c, request, closedHandler));
    }

    private static bool IsHandlerOf(INamedTypeSymbol candidate, INamedTypeSymbol request, INamedTypeSymbol? closedHandler)
    {
        if (candidate.TypeKind != TypeKind.Class || SymbolEqualityComparer.Default.Equals(candidate, request)) return false;
        // The profile's handler interface, or any *Handler interface over the request (ZeroAlloc's IRequestHandler<,>).
        if (candidate.AllInterfaces.Any(i => SymbolEqualityComparer.Default.Equals(i, closedHandler)
             || i.Name.EndsWith("Handler", StringComparison.Ordinal) && i.TypeArguments.FirstOrDefault() is { } first && SymbolEqualityComparer.Default.Equals(first, request)))
        {
            return true;
        }

        // Convention handlers: Immediate.Handlers' [Handler] classes and Foundatio's *Handler/*Consumer classes.
        return candidate.GetMembers()
                        .OfType<IMethodSymbol>()
                        .Any(m => m.Name is "Handle" or "HandleAsync" or "Consume" or "ConsumeAsync" && m.Parameters.FirstOrDefault() is { } p && SymbolEqualityComparer.Default.Equals(p.Type, request));
    }

    private static (RequestKind Kind, ITypeSymbol? Response, RequestKindModel? Model) MatchKind(
        INamedTypeSymbol request,
        INamedTypeSymbol host,
        ProfileModel profile,
        ITypeSymbol? explicitResponse,
        Compilation compilation
    )
    {
        foreach (var kind in profile.Kinds)
        {
            if (kind.RequestInterface is null) continue;
            var match = request.AllInterfaces.FirstOrDefault(i => i.Is(kind.RequestInterface));
            if (match is null) continue;
            var resolved = kind.Kind == RequestKind.Infer ? match.Arity == 0 ? RequestKind.Void : RequestKind.Unary : kind.Kind;
            var response = match.Arity > kind.ResponseTypeArgumentIndex ? match.TypeArguments[kind.ResponseTypeArgumentIndex] : null;
            return (resolved, response, kind);
        }

        // No marker interface: the response comes from [ApiOperation(Response)] or, for Immediate.Handlers, from the [Handler] method.
        var markerless = profile.Kinds.FirstOrDefault(k => k.RequestInterface is null);
        if (markerless is null) return (RequestKind.Infer, null, null);
        // typeof() can't say "nullable", so a [Handler] returning the same type wins: it may carry the annotation (RocketModel?).
        var handlerResponse = FindImmediateHandlerResponse(request, host, compilation);
        var responseType = explicitResponse is null || SymbolEqualityComparer.Default.Equals(handlerResponse, explicitResponse) ? handlerResponse ?? explicitResponse : explicitResponse;
        if (responseType is null) return (RequestKind.Infer, null, null);
        if (responseType is INamedTypeSymbol { Name: "IAsyncEnumerable", Arity: 1 } stream) return (RequestKind.Stream, stream.TypeArguments[0], markerless);
        return (RequestKind.Unary, responseType, markerless);
    }

    // An Immediate.Handlers [Handler] for the request, looked for next to the request and next to the host, so the search stays cheap.
    private static ITypeSymbol? FindImmediateHandlerResponse(INamedTypeSymbol request, INamedTypeSymbol host, Compilation compilation)
    {
        var candidates = new List<INamedTypeSymbol>();
        if (request.ContainingType is { } containing) candidates.Add(containing);
        foreach (var ns in new[] { request.ContainingNamespace, host.ContainingNamespace })
        {
            if (compilation.GetCompilationNamespace(ns) is { } merged) candidates.AddRange(merged.GetTypeMembers());
        }

        foreach (var candidate in candidates.Distinct<INamedTypeSymbol>(SymbolEqualityComparer.Default).Where(c => c.GetAttributes().Any(a => a.AttributeClass.Is("Immediate.Handlers.Shared.HandlerAttribute"))))
        {
            var handle = candidate.GetMembers()
                                  .OfType<IMethodSymbol>()
                                  .FirstOrDefault(m => m.Name is "Handle" or "HandleAsync" && m.Parameters.Length > 0 && SymbolEqualityComparer.Default.Equals(m.Parameters[0].Type, request));
            if (handle?.ReturnType is INamedTypeSymbol { Arity: 1 } awaitable && awaitable.Name is "ValueTask" or "Task") return awaitable.TypeArguments[0];
        }

        return null;
    }

    private sealed record RawMember(string Name, ITypeSymbol Type, bool CanAssign, bool CanInit, int ConstructorIndex, ImmutableArray<AttributeData> Attributes);

    private static List<RawMember> ReadMembers(INamedTypeSymbol type)
    {
        var properties = new List<IPropertySymbol>();
        for (var current = type; current is { SpecialType: not SpecialType.System_Object }; current = current.BaseType)
        {
            properties.AddRange(
                current.GetMembers()
                       .OfType<IPropertySymbol>()
                       .Where(p => p is { IsStatic: false, IsIndexer: false, DeclaredAccessibility: Accessibility.Public, GetMethod: { } } && p.Name != "EqualityContract")
                       .Where(p => properties.All(existing => existing.Name != p.Name))
            );
        }

        // The public constructor with the most parameters that all map to members (a record's primary constructor).
        var constructor = type.InstanceConstructors
                              .Where(c => c.DeclaredAccessibility == Accessibility.Public)
                              .Where(c => c.Parameters.All(p => properties.Any(m => string.Equals(m.Name, p.Name, StringComparison.OrdinalIgnoreCase))))
                              .OrderByDescending(c => c.Parameters.Length)
                              .FirstOrDefault();
        return [.. properties
              .Select(
                   p => new RawMember(
                       p.Name,
                       p.Type,
                       p.SetMethod is { IsInitOnly: false, DeclaredAccessibility: Accessibility.Public },
                       p.SetMethod is { IsInitOnly: true, DeclaredAccessibility: Accessibility.Public },
                       constructor is null ? -1 : constructor.Parameters.IndexOf(constructor.Parameters.FirstOrDefault(c => string.Equals(c.Name, p.Name, StringComparison.OrdinalIgnoreCase))!),
                       p.GetAttributes()
                   )
               )];
    }

    private static List<MemberModel> Bind(
        INamedTypeSymbol wireType,
        List<RawMember> members,
        string[] tokens,
        HttpVerb verb,
        string route,
        string operationName,
        LocationInfo? location,
        List<DiagnosticInfo> diagnostics,
        out bool failed
    )
    {
        failed = false;
        var hasBody = verb is HttpVerb.Post or HttpVerb.Put or HttpVerb.Patch;
        foreach (var token in tokens.Where(t => members.All(m => !string.Equals(m.Name, t, StringComparison.OrdinalIgnoreCase))))
        {
            diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RouteTokenWithoutMember, location, route, operationName, token, wireType.ToDisplayString()));
            failed = true;
        }

        var result = new List<MemberModel>();
        foreach (var member in members)
        {
            var attribute = member.Attributes.Select(a => a.AttributeClass?.ToDisplayString()).ToArray();
            var named = member.Attributes.Select(Named).ToArray();
            var token = tokens.FirstOrDefault(t => string.Equals(t, member.Name, StringComparison.OrdinalIgnoreCase));
            var wireName = named.Select(n => String(n, "Name")).FirstOrDefault(n => n is { }) ?? Naming.CamelCase(member.Name);
            // An explicit attribute wins; then a route token; then the verb decides between body and query.
            var source = Has(attribute, KnownNames.ApiIgnoreAttribute) ? BindingSource.Ignored
                : Has(attribute, KnownNames.ApiServiceAttribute, "Microsoft.AspNetCore.Mvc.FromServicesAttribute") ? BindingSource.Service
                : Has(attribute, KnownNames.ApiHeaderAttribute, "Microsoft.AspNetCore.Mvc.FromHeaderAttribute") ? BindingSource.Header
                : Has(attribute, KnownNames.ApiBodyAttribute, "Microsoft.AspNetCore.Mvc.FromBodyAttribute") ? BindingSource.Body
                : Has(attribute, KnownNames.ApiRouteAttribute, "Microsoft.AspNetCore.Mvc.FromRouteAttribute") || token is { } ? BindingSource.Route
                : Has(attribute, KnownNames.ApiQueryAttribute, "Microsoft.AspNetCore.Mvc.FromQueryAttribute") ? BindingSource.Query
                : hasBody ? BindingSource.Body : BindingSource.Query;
            if (source == BindingSource.Route && token is null)
            {
                diagnostics.Add(DiagnosticInfo.Create(ApiDiagnostics.RouteMemberNotInRoute, location, wireType.Name, member.Name, route, Naming.CamelCase(member.Name)));
                failed = true;
            }

            if (source == BindingSource.Route && token is { }) wireName = token;
            var (isSimple, isCollection) = Classify(member.Type, source == BindingSource.Query);
            if (source is BindingSource.Route or BindingSource.Query or BindingSource.Header && !isSimple)
            {
                var where = source == BindingSource.Route ? "route" : source == BindingSource.Header ? "headers" : "query string";
                var descriptor = IsComplex(member.Type) ? ApiDiagnostics.ComplexMemberNotBindable : ApiDiagnostics.MemberNotParsable;
                diagnostics.Add(DiagnosticInfo.Create(descriptor, location, wireType.Name, member.Name, member.Type.ToDisplayString(), where));
                failed = true;
            }

            result.Add(
                new MemberModel(
                    member.Name,
                    wireName,
                    member.Type.FullyQualifiedNullable(),
                    member.Type.WithNullableAnnotation(NullableAnnotation.NotAnnotated).FullyQualified(),
                    source,
                    member.Type.NullableAnnotation == NullableAnnotation.Annotated || member.Type.OriginalDefinition.SpecialType == SpecialType.System_Nullable_T,
                    isCollection,
                    member.CanAssign,
                    member.CanInit,
                    member.ConstructorIndex,
                    source is BindingSource.Route or BindingSource.Query or BindingSource.Header ? ParseKindOf(Element(member.Type)) : ParseKind.None,
                    source is BindingSource.Route or BindingSource.Query or BindingSource.Header ? Element(member.Type).WithNullableAnnotation(NullableAnnotation.NotAnnotated).FullyQualified() : null
                )
            );
        }

        return result;
    }

    private static bool Has(string?[] attributes, params string[] names) => attributes.Any(a => a is { } && names.Any(n => a == n || a == n.Replace("`1", "")));

    // The type a single route, query or header value parses to: the element of an array, the underlying type of a Nullable<T>.
    private static ITypeSymbol Element(ITypeSymbol type)
    {
        if (type is IArrayTypeSymbol array) type = array.ElementType;
        return type is INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable ? nullable.TypeArguments[0] : type;
    }

    private static ParseKind ParseKindOf(ITypeSymbol type) => type switch
    {
        { SpecialType: SpecialType.System_String } => ParseKind.String,
        { TypeKind: TypeKind.Enum } => ParseKind.Enum,
        _ when type.ToDisplayString() == "System.Uri" => ParseKind.Uri,
        _ when type.AllInterfaces.Any(i => i.Is("System.IParsable`1")) => ParseKind.Parsable,
        _ when TryParseMethods(type).Any(m => m.Parameters.Length == 3 && m.Parameters[1].Type.ToDisplayString() == "System.IFormatProvider") => ParseKind.TryParseWithProvider,
        _ when TryParseMethods(type).Any(m => m.Parameters.Length == 2 && m.Parameters[1].RefKind == RefKind.Out) => ParseKind.TryParse,
        _ => ParseKind.None,
    };

    // Static bool TryParse(string, ...) methods.
    private static IEnumerable<IMethodSymbol> TryParseMethods(ITypeSymbol type) =>
        type.GetMembers("TryParse")
            .OfType<IMethodSymbol>()
            .Where(m => m.IsStatic && m.ReturnType.SpecialType == SpecialType.System_Boolean && m.Parameters.Length > 0 && m.Parameters[0].Type.SpecialType == SpecialType.System_String);

    private static (bool Simple, bool Collection) Classify(ITypeSymbol type, bool allowCollection)
    {
        if (IsSimple(type)) return (true, false);
        if (allowCollection && type is IArrayTypeSymbol { ElementType: var element } && IsSimple(element)) return (true, true);
        return (false, false);
    }

    private static bool IsComplex(ITypeSymbol type) => type.TypeKind is TypeKind.Class or TypeKind.Interface && type.SpecialType != SpecialType.System_String || type is IArrayTypeSymbol;

    private static bool IsSimple(ITypeSymbol type) => type switch
    {
        INamedTypeSymbol { OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } nullable => IsSimple(nullable.TypeArguments[0]),
        { SpecialType: >= SpecialType.System_Boolean and <= SpecialType.System_String or SpecialType.System_DateTime } => true,
        { TypeKind: TypeKind.Enum } => true,
        _ => type.ToDisplayString() is "System.Guid" or "System.DateTimeOffset" or "System.DateOnly" or "System.TimeOnly" or "System.TimeSpan" or "System.Uri" or "System.Version"
         || type.AllInterfaces.Any(i => i.Is("System.IParsable`1"))
         || TryParseMethods(type).Any(m => m.Parameters.Length is 2 or 3 && m.Parameters[^1].RefKind == RefKind.Out),
    };

    private static string NormalizeRoute(string route) => string.Join("/", route.Trim('/').Split('/').Select(s => s.StartsWith("{", StringComparison.Ordinal) ? "{}" : s.ToLowerInvariant()));

    private static Dictionary<string, TypedConstant> Named(AttributeData? attribute) =>
        attribute?.NamedArguments.ToDictionary(kv => kv.Key, kv => kv.Value) ?? [];

    private static string? String(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) ? v.Value as string : null;

    private static bool? Bool(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) && v.Value is bool b ? b : null;

    private static int? Int(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) && v.Value is int i ? i : null;

    private static ITypeSymbol? Type(Dictionary<string, TypedConstant> args, string name) => args.TryGetValue(name, out var v) ? v.Value as ITypeSymbol : null;

    private static string[]? Strings(Dictionary<string, TypedConstant> args, string name) =>
        args.TryGetValue(name, out var v) && v.Kind == TypedConstantKind.Array ? [.. v.Values.Select(x => x.Value).OfType<string>()] : null;
}
